using Models;
using ViewModels;
using DAL;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Helpers;

namespace BLL
{
    public class AccountingService : BaseService<Journal>
    {
        private readonly LedgerAccountDAL _ledgerAccountDAL;
        private readonly JournalDAL _journalDAL;
        private readonly JournalLineDAL _journalLineDAL;

        public AccountingService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(config, httpContextAccessor)
        {
            _ledgerAccountDAL = new LedgerAccountDAL(config);
            _journalDAL = new JournalDAL(config);
            _journalLineDAL = new JournalLineDAL(config);
            _dal = _journalDAL;
        }

        private void ValidateJournal(Journal journal, SqlConnection conn, SqlTransaction tran)
        {
            // Journal object cannot be null
            if (journal == null)
                throw new ArgumentNullException(nameof(journal));

            // Journal Date is mandatory
            if (journal.JournalDate == DateTime.MinValue)
                throw new InvalidOperationException("Journal date is required.");

            // Journal must contain at least 2 lines
            if (journal.Lines == null || journal.Lines.Count < 2)
                throw new InvalidOperationException("Journal must contain at least two accounting entries.");

            // Narration required
            if (string.IsNullOrWhiteSpace(journal.Narration))
                throw new InvalidOperationException("Journal narration is required.");

            foreach (var line in journal.Lines)
            {
                // Ledger Account must be selected
                if (line.LedgerAccountId <= 0)
                    throw new InvalidOperationException("Ledger account is required.");

                // Ledger Account must exist
                var ledger = _ledgerAccountDAL.Get(line.LedgerAccountId, conn, tran);

                if (ledger == null)
                    throw new InvalidOperationException($"Ledger account {line.LedgerAccountId} does not exist.");

                // Negative amounts not allowed
                if (line.DebitAmount < 0)
                    throw new InvalidOperationException("Debit amount cannot be negative.");

                if (line.CreditAmount < 0)
                    throw new InvalidOperationException("Credit amount cannot be negative.");

                // Both Debit and Credit cannot be zero
                if (line.DebitAmount == 0 && line.CreditAmount == 0)
                {
                    throw new InvalidOperationException("Either debit or credit amount must be entered.");
                }

                // Both Debit and Credit cannot have value simultaneously
                if (line.DebitAmount > 0 && line.CreditAmount > 0)
                {
                    throw new InvalidOperationException("A journal line cannot contain both debit and credit amount.");
                }

                // Amount cannot be negative
                if (line.DebitAmount < 0 || line.CreditAmount < 0)
                {
                    throw new InvalidOperationException("Journal amount must be greater than zero.");
                }
            }

            // Total Debit must equal Total Credit
            decimal totalDebit = journal.Lines.Sum(x => x.DebitAmount);
            decimal totalCredit = journal.Lines.Sum(x => x.CreditAmount);

            if (totalDebit != totalCredit)
            {
                throw new InvalidOperationException($"Journal is out of balance. Debit={totalDebit:N2}, Credit={totalCredit:N2}");
            }
            
            // Total amount cannot be zero
            if (totalDebit <= 0 || totalCredit <= 0)
            {
                throw new InvalidOperationException("Journal total amount must be greater than zero.");
            }

            // Same ledger should not appear twice on same side
            var duplicateDebits = journal.Lines
                                    .Where(x => x.DebitAmount > 0)
                                    .GroupBy(x => x.LedgerAccountId)
                                    .Any(g => g.Count() > 1);

            if (duplicateDebits)
            {
                throw new InvalidOperationException("Duplicate debit ledger entries found.");
            }

            var duplicateCredits = journal.Lines
                                    .Where(x => x.CreditAmount > 0)
                                    .GroupBy(x => x.LedgerAccountId)
                                    .Any(g => g.Count() > 1);

            if (duplicateCredits)
            {
                throw new InvalidOperationException("Duplicate credit ledger entries found.");
            }
        }

        private bool BuildLine(int clinicId, Journal journal, string ledgerAccountCode, string postingType, decimal amount, 
                                    SqlConnection conn, SqlTransaction tran)
        {
            JournalLine line = null;

/*            decimal amount = bill.BillLines
                                .Where(x => x.ServiceType == ((serviceType=="")? x.ServiceType : serviceType))
                                .Sum(x => x.NetAmount); */

            if (amount > 0)
            {
                journal.Lines.Add(line = new JournalLine(clinicId)
                {
                    LedgerAccountId = _ledgerAccountDAL.GetId(ledgerAccountCode, conn, tran),
                    CreditAmount = (postingType == "CR") ? amount : 0,
                    DebitAmount = (postingType == "DR") ? amount : 0
                });

                return true;
            }

            return false;
        }

        public Journal BuildJournal(Bill bill, SqlConnection conn, SqlTransaction tran)
        {
            Journal journal = new Journal(bill);

            // Debit - Patient Receivable
            BuildLine(bill.ClinicId, journal, LedgerAccountCode.PatientReceivable, "DR", bill.NetAmount, conn, tran);

            // Debit - Discount Allowed
            BuildLine(bill.ClinicId, journal, LedgerAccountCode.DiscountAllowed, "DR", bill.DiscountAmount, conn, tran);

            // Credit - Consultation Revenue
            BuildLine(bill.ClinicId, journal, LedgerAccountCode.RegistrationRevenue, "CR", 
                                    bill.BillLines
                                        .Where(x => x.ServiceType == "Registration")
                                        .Sum(x => x.GrossAmount), conn, tran);

            // Credit - Radiology Revenue
            BuildLine(bill.ClinicId, journal, LedgerAccountCode.RadiologyRevenue, "CR", 
                                    bill.BillLines
                                        .Where(x => x.ServiceType == "Radiology")
                                        .Sum(x => x.GrossAmount), conn, tran);

            // Credit - Consultation Revenue
            BuildLine(bill.ClinicId, journal, LedgerAccountCode.ConsultationRevenue, "CR", 
                                    bill.BillLines
                                        .Where(x => x.ServiceType == "Consultation")
                                        .Sum(x => x.GrossAmount), conn, tran);

            // Credit - Lab Revenue
            BuildLine(bill.ClinicId, journal, LedgerAccountCode.LabRevenue, "CR", 
                                    bill.BillLines
                                        .Where(x => x.ServiceType == "Lab Tests")
                                        .Sum(x => x.GrossAmount), conn, tran);

            // Credit - Pharmacy Revenue
            BuildLine(bill.ClinicId, journal, LedgerAccountCode.PharmacyRevenue, "CR", 
                                    bill.BillLines
                                        .Where(x => x.ServiceType == "Pharmacy")
                                        .Sum(x => x.GrossAmount), conn, tran);

            // Credit - Output GST Payable
            BuildLine(bill.ClinicId, journal, LedgerAccountCode.OutputGSTPayable, "CR", bill.TaxAmount, conn, tran);

            return journal;
        }

        // This has to be outside conn and tran in Calling service
        // as this uses its own conn
        public Journal BuildJournal(Payment payment, SqlConnection conn, SqlTransaction tran)
        {
            Journal journal = new Journal(payment);

            // Debit - Cash
            BuildLine(payment.ClinicId, journal, LedgerAccountCode.CashInHand, "DR", payment.Amount, conn, tran);
    
            // Credit - Patient Receivable
            BuildLine(payment.ClinicId, journal, LedgerAccountCode.PatientReceivable, "CR", payment.Amount, conn, tran);

            return journal;
        }

        // This will always be called from some other service
        // Calling service is responsible to provide conn and tran
        // and appropriate try-catch block for transaction management and exception handling
        public int PostJournal(Journal journal, SqlConnection conn, SqlTransaction tran)
        {
            ValidateJournal(journal, conn, tran);

            int journalHeaderId = _journalDAL.Add(journal, conn, tran);

            foreach (var line in journal.Lines)
            {
                line.JournalHeaderId = journalHeaderId;

                _journalLineDAL.Add(line, conn, tran);
            }

            return journalHeaderId;
        }

    }

}

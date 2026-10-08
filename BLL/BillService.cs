using DAL;
using Models;
using ViewModels;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BLL
{
    public class BillService : BaseService<Bill>
    {
        private readonly BillDAL _billDAL;
        private readonly BillLineDAL _billLineDAL;
        private readonly BillingChargeDAL _billingChargeDAL;
        private readonly PatientDAL _patientDAL;
        private readonly PaymentDAL _paymentDAL;
        private readonly VisitDAL _visitDAL;
        private readonly ClinicDAL _clinicDAL;
        private readonly AppointmentDAL _appointmentDAL;
        private readonly DoctorDAL _doctorDAL;
        private readonly AccountingService _accounting;

        public BillService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(config, httpContextAccessor)
        {
            _billDAL = new BillDAL(config);
            _billLineDAL = new BillLineDAL(config);
            _billingChargeDAL = new BillingChargeDAL(config);
            _patientDAL = new PatientDAL(config);
            _paymentDAL = new PaymentDAL(config);
            _visitDAL = new VisitDAL(config);
            _clinicDAL = new ClinicDAL(config);
            _appointmentDAL = new AppointmentDAL(config);
            _doctorDAL = new DoctorDAL(config);
            _accounting = new AccountingService(config, httpContextAccessor);
            _dal = _billDAL;
        }

        public override int Create(Bill bill)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            using var tran = conn.BeginTransaction();

            try
            {
                // Create Bill from appointment booking
                // or select a doctor's name from billing screen (other than external)
                if (bill.AppointmentId.HasValue)
                {
                    bill.Appointment = _appointmentDAL.Get(bill.AppointmentId.Value, ClinicId, conn, tran);

                    if (bill.Appointment == null)
                        throw new InvalidOperationException("Appointment not found.");

                    bill.VisitId = bill.Appointment.VisitId;
                }
                else
                {
                    // Add Bill from Menu - find out any existing visit for today
                    // Visit exists but no appointments
                    var visit = _visitDAL.GetByPatientAndDate(bill.PatientId, bill.BillDate, conn, tran);

                    if (visit != null)
                    {
                        bill.VisitId = visit.VisitId;
                        bill.DoctorId = null; //external doctor referring some tests/investigations
                    }
                }

                if (bill.VisitId == 0)
                {
                    // create a visit for bill date and patient id
                    Visit visit = new Visit
                    {
                        VisitDate = bill.BillDate,
                        VisitType = "New",
                        ClinicId = ClinicId,
                        PatientId = bill.PatientId
                    };
                    
                    bill.VisitId = _visitDAL.Add(visit, conn, tran);
                }

                // Visit remains the source of PatientId.
                bill.Visit = _visitDAL.Get(bill.VisitId, ClinicId, conn, tran);

                if (bill.Visit == null)
                    throw new InvalidOperationException("Visit not found.");

                bill.PatientId = bill.Visit.PatientId;
                bill.Patient = _patientDAL.Get(bill.PatientId, ClinicId, conn, tran);

                bill.ClinicId = ClinicId;
                bill.Clinic = _clinicDAL.Get(bill.ClinicId, ClinicId, conn, tran);

                // Doctor details not required for creating bill
        /*        if (bill.Appointment != null)
                {
                    bill.DoctorId = bill.Appointment.DoctorId;
                    bill.Doctor = _doctorDAL.Get(bill.DoctorId.Value, ClinicId, conn, tran);
                } */

                foreach (var line in bill.BillLines)
                {
                    line.PatientId = bill.PatientId;

                    var charge = _billingChargeDAL.Get(line.ReferenceId, ClinicId, conn, tran);

                    if (charge == null)
                    {
                        throw new InvalidOperationException(
                            "Billing charge not found for this clinic.");
                    }

                    line.ReferenceCode = charge.ChargeCode;
                    line.Description = charge.Description;
                    line.ServiceType = charge.ServiceType;
                    line.AvailableQuantity = line.Quantity;

                    if (bill.BillType == "Service")
                    {
                        if (charge.ChargeCode == "CONSULT")
                        {
                            if (!bill.AppointmentId.HasValue || bill.Appointment == null)
                            {
                                throw new InvalidOperationException(
                                    "A doctor appointment is required for consultation charges.");
                            }

                            if (!bill.AppointmentId.HasValue)
                                throw new InvalidOperationException(
                                    "A doctor appointment is required for consultation charges.");

                            if (!bill.Appointment.Fee.HasValue)
                                throw new InvalidOperationException(
                                    "Consultation fee is not configured for the selected doctor.");

                            // Append doctor name to consultation description
                            if (!string.IsNullOrWhiteSpace(bill.Appointment.DoctorName))
                            {
                                line.Description =
                                    $"{charge.Description} - {bill.Appointment.DoctorName}";
                            }

                            line.Rate = bill.Appointment.Fee.Value; 
                        }
                        else
                        {
                            line.Rate = charge.Rate;
                        }
                    }
                }

                bill.GrossAmount = bill.BillLines.Sum(x => x.GrossAmount);
                bill.DiscountAmount = bill.BillLines.Sum(x => x.DiscountAmount);

                bill.BillNo = _billDAL.GetNextNo(ClinicId, bill.BillDate, "Bill", conn, tran);

                bill.CreatedBy = UserId;

                int billId = Add(bill, conn, tran);
                bill.BillId = billId;

                foreach (var line in bill.BillLines)
                {
                    line.BillId = billId;
                    _billLineDAL.Add(line, conn, tran);
                }

                Journal journal = _accounting.BuildJournal(bill, conn, tran);

                _accounting.PostJournal(journal, conn, tran);

                tran.Commit();

                return billId;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public bool UpdateAvailableQuantities(List<BillLine> lines)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            using var tran = conn.BeginTransaction();

            try
            {
                bool result = _billLineDAL.UpdateAvailableQuantities(lines, conn, tran);
                tran.Commit();

                return result;
            }
            catch
            {
                tran.Rollback();
                throw;
            }            
        }

        public override Bill Get(int BillId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                var bill = base.Get(BillId, conn);
                if (bill != null)
                {
                    bill.Clinic = _clinicDAL.Get(ClinicId, ClinicId, conn);
                    bill.ClinicId = ClinicId;
                    bill.Patient = _patientDAL.Get(bill.PatientId, ClinicId, conn);
                    bill.Visit = _visitDAL.Get(bill.VisitId, ClinicId, conn);
                    bill.VisitDate = bill.Visit?.VisitDate;
                    bill.Appointment = bill.AppointmentId.HasValue 
                                                    ? _appointmentDAL.Get(bill.AppointmentId.Value, ClinicId, conn) 
                                                    : null;
                    bill.DoctorId = bill.Appointment != null ? bill.Appointment.DoctorId : null;
                    bill.Doctor = bill.DoctorId.HasValue ? _doctorDAL.Get(bill.DoctorId.Value, ClinicId, conn) : null;
                    bill.BillLines = _billLineDAL.GetByBillId(BillId, conn);

                    bill.Payments = _paymentDAL.GetPaymentsByBill(BillId, conn);
                    bill.TotalPaidAmount = _paymentDAL.GetTotalPaid(BillId, conn);
                }

                return bill;
            }
            catch
            {
                throw;
            }
            
        }

        public List<Bill> ListByAppointment(int appointmentId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            return _billDAL.GetByAppointment(appointmentId, ClinicId, conn);
        }

        public List<Bill> ListByVisit(int visitId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            return _billDAL.GetByVisit(visitId, ClinicId, conn);
        }

        public BillingCreateVM GetNewBillByAppointment(int? appointmentId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            var bill = new Bill
            {
                BillDate = DateTime.Today,
                AppointmentId = appointmentId,
                ClinicId = ClinicId
            };

            var vm = new BillingCreateVM
            {
                Bill = bill
            };

            if (!appointmentId.HasValue)
                return vm;

            var appointment = _appointmentDAL.Get(appointmentId.Value, ClinicId, conn);

            if (appointment == null)
                return vm;

            bill.AppointmentId = appointment.AppointmentId;
            bill.Appointment = appointment;
            bill.VisitId = appointment.VisitId;
            bill.PatientId = appointment.PatientId;

            bill.Patient = _patientDAL.Get(bill.PatientId, ClinicId, conn);

            if (bill.Patient != null)
            {
                bill.PatientName =
                    $"{bill.Patient.FirstName} {bill.Patient.LastName}".Trim();
            }

            bill.Visit = _visitDAL.Get(bill.VisitId, ClinicId, conn);

            bill.VisitDate = bill.Visit?.VisitDate;

            bill.DoctorId = appointment.DoctorId;

            if (bill.DoctorId.HasValue)
            {
                bill.Doctor = _doctorDAL.Get(bill.DoctorId.Value, ClinicId, conn);
            }

            bill.ClinicId = ClinicId;
            bill.Clinic = _clinicDAL.Get(ClinicId, ClinicId, conn);

            var billCount = bill.VisitId > 0 ? _billDAL.GetVisitBillCount(bill.VisitId, ClinicId, conn) : 0;

            vm.Bill = bill;
            vm.PreviousBillCount = billCount;

            return vm;
        }

        public int GetVisitBillCount(int visitId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            return _billDAL.GetVisitBillCount(visitId, ClinicId, conn);            
        }

        public BillingCreateVM GetBillingContext(int patientId, DateTime billDate, int? appointmentId = null)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            var vm = new BillingCreateVM
            {
                Bill = new Bill
                {
                    PatientId = patientId,
                    BillDate = billDate,
                    ClinicId = ClinicId
                }
            };

            // ----------------------------------------
            // Patient
            // ----------------------------------------

            var patient = _patientDAL.Get(patientId, ClinicId, conn);

            if (patient == null)
                return vm;

            vm.Bill.Patient = patient;

            vm.Bill.PatientName = $"{patient.FirstName} {patient.LastName}".Trim();

            // ========================================================
            // SPECIFIC APPOINTMENT CONTEXT
            // ========================================================

            if (appointmentId.HasValue)
            {
                var appointment = _appointmentDAL.Get(appointmentId.Value, ClinicId, conn);

                if (appointment == null)
                    return vm;

                /*
                * Make sure the appointment actually belongs
                * to the requested patient.
                */
                if (appointment.PatientId != patientId)
                    return vm;

                vm.Bill.AppointmentId = appointment.AppointmentId;
                vm.Bill.VisitId = appointment.VisitId;
                vm.Bill.VisitDate = appointment.AppointmentDate;
                vm.Bill.Appointment = appointment;

                // ----------------------------------------
                // Visit
                // ----------------------------------------

                if (appointment.VisitId > 0)
                {
                    var visit = _visitDAL.Get(appointment.VisitId, ClinicId, conn);

                    vm.Bill.Visit = visit;

                    if (visit != null)
                    {
                        vm.Bill.VisitDate = visit.VisitDate;

                        vm.PreviousBillCount = _billDAL.GetVisitBillCount(visit.VisitId, ClinicId, conn);
                    }
                }


                // ----------------------------------------
                // Only this appointment
                // ----------------------------------------

                vm.Appointments = new List<BillingAppointmentVM>
                    {
                        new BillingAppointmentVM
                        {
                            AppointmentId = appointment.AppointmentId,
                            DoctorId = appointment.DoctorId,
                            DoctorName = appointment.DoctorName,
                            Fee = appointment.Fee
                        }
                    };


                // ----------------------------------------
                // Consultation charge
                // ----------------------------------------

                var consultationCharge = _billingChargeDAL.GetByCode("CONSULT", ClinicId, conn);

                vm.ConsultationChargeId = consultationCharge?.ChargeId;

                return vm;
            }


            // ========================================================
            // NORMAL PATIENT / DATE BILLING CONTEXT
            // ========================================================

            var visitByDate = _visitDAL.GetByPatientAndDate(patientId, billDate, conn);

            if (visitByDate == null)
            {
                /*
                * No visit yet.
                *
                * BillService.Create() will create the visit
                * when the bill is actually saved.
                */
                return vm;
            }

            vm.Bill.VisitId = visitByDate.VisitId;
            vm.Bill.Visit = visitByDate;
            vm.Bill.VisitDate = visitByDate.VisitDate;

            // ----------------------------------------
            // Appointments for this visit
            // ----------------------------------------

            var appointments = _appointmentDAL.GetAll(patientId, visitByDate.VisitId, ClinicId, conn);

            /*
            * One appointment per distinct
            * Patient + Visit + Doctor.
            *
            * If the same doctor has multiple appointments,
            * retain the oldest appointment by AppointmentId.
            */
            var distinctAppointments = appointments
                    .Where(x => x.Status != "Cancelled")
                    .GroupBy(x => new
                    {
                        x.PatientId,
                        x.VisitId,
                        x.DoctorId
                    })
                    .Select(g =>
                        g.OrderBy(x => x.AppointmentId)
                        .First())
                    .ToList();

            vm.Appointments = distinctAppointments
                    .Select(x => new BillingAppointmentVM
                    {
                        AppointmentId = x.AppointmentId,
                        DoctorId = x.DoctorId,
                        DoctorName = x.DoctorName,
                        Fee = x.Fee
                    })
                    .ToList();


            // ----------------------------------------
            // Previous bills
            // ----------------------------------------

            vm.PreviousBillCount = _billDAL.GetVisitBillCount(visitByDate.VisitId, ClinicId, conn);

            // ----------------------------------------
            // Consultation charge
            // ----------------------------------------

            var charge = _billingChargeDAL.GetByCode("CONSULT", ClinicId, conn);

            vm.ConsultationChargeId = charge?.ChargeId;

            return vm;
        }

        public override List<Bill> List()
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _billDAL.GetAllByClinic(ClinicId, conn);
            }
            catch
            {
                throw;
            }

        }

        public List<Bill> ListByPatient(int patientId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _billDAL.ListByPatient(ClinicId, patientId, conn);
            }
            catch
            {
                throw;
            }

        }

        public List<SelectListItem>  ListRefundableBillsByPatient(int patientId, int? selectedBillId = null)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return ListRefundableBillsByPatient(patientId, selectedBillId, conn);
            }
            catch
            {
                throw;
            }

        }

        public List<SelectListItem>  ListRefundableBillsByPatient(int patientId, int? selectedBillId,
                                                                    SqlConnection conn, SqlTransaction? tran=null)
        {
            List<Bill> bills = _billDAL.ListRefundableBillsByPatient(ClinicId, patientId, selectedBillId, conn, tran);

            return bills.Select(x => new SelectListItem
            {
                Value = x.BillId.ToString(),
                Text = x.BillNo + " " + x.BillDate.ToString("dd-MMM-yyyy") + " " + x.NetAmount,
                Selected = x.BillId == selectedBillId
            }).ToList();            
        }

        public List<BillLine> ListBillLines(int billId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _billLineDAL.GetByBillId(billId, conn);
            }
            catch
            {
                throw;
            }
            
        }

    }
}

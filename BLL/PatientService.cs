using DAL;
using Models;
using ViewModels;
using Helpers;

namespace BLL
{    
    public class PatientService : BaseService<Patient>
    {
        private readonly PatientDAL _patientDAL;
        private readonly VisitDAL _visitDAL;
        private readonly AppointmentDAL _appointmentDAL;
        private readonly BillDAL _billDAL;
        private readonly DepartmentDAL _departmentDAL;
        private readonly ConsultationDAL _consultationDAL;
        private readonly BillLineDAL _billLineDAL;
        private readonly PaymentDAL _paymentDAL;

        public PatientService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(config, httpContextAccessor)
        {
            _patientDAL = new PatientDAL(config);
            _visitDAL = new VisitDAL(config);
            _appointmentDAL = new AppointmentDAL(config);
            _billDAL = new BillDAL(config);
            _departmentDAL = new DepartmentDAL(config);
            _consultationDAL = new ConsultationDAL(config);
            _billLineDAL = new BillLineDAL(config);
            _paymentDAL = new PaymentDAL(config);
            _dal = _patientDAL;
        }

        public List<Patient> Search(PatientSearchVM searchVM)
        {
            if (searchVM == null)
                throw new ArgumentNullException(nameof(searchVM));

            string searchString = searchVM.SearchString?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(searchString))
                return new List<Patient>();

            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                // Always use the current clinic context.
                searchVM.ClinicId = ClinicId;
                searchVM.SearchString = searchString;

                return _patientDAL.Search(searchVM, conn);
            }
            catch
            {
                throw;
            }
        }

        public List<Patient> Search(string term)
        {
            string searchString = term?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(searchString))
                return new List<Patient>();

            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                var vm = new PatientSearchVM
                {
                    ClinicId = ClinicId,
                    SearchString = searchString
                };

                return _patientDAL.Search(vm, conn);
            }
            catch
            {
                throw;
            }
        }

        public override int Create(Patient model)
        {
            int id = base.Create(model);
            model.PatientId = id;
            SetCache(new PatientSelectionVM(model));
            
            return id;
        }

        public override Patient Get(int id)
        {
            Patient p = base.Get(id);

            if (p != null)
                SetCache(new PatientSelectionVM(p));
            return p;
        }

        public override List<Patient> List()
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _patientDAL.ListByClinic(ClinicId, conn);
            }
            catch
            {
                throw;
            }
        }

        public Patient GetFirstOrDefault(List<Patient> patients)
        {
            Patient p;
            
            if (patients == null || patients.Count == 0)
            {
                p = new Patient
                {
                    PatientId = 0,
                    FirstName = "No",
                    LastName = "Record"
                };
            }
            else
            {
                p = patients[0];
                SetCache(new PatientSelectionVM(p));
            }

            return p;
        }

        public PatientHistoryVM GetHistory(int patientId, int page = 1)
        {
            const int pageSize = 3;

            // Never allow an invalid page number.
            page = Math.Max(page, 1);

            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                // ---------------------------------------------------------
                // Get patient
                // ---------------------------------------------------------

                var patient = _dal.Get(patientId, ClinicId, conn);

                if (patient == null)
                {
                    return new PatientHistoryVM();
                }

                // Keep the selected patient in the global patient cache.
                SetCache(new PatientSelectionVM(patient));


                // ---------------------------------------------------------
                // Create ViewModel
                // ---------------------------------------------------------

                var vm = new PatientHistoryVM(patient)
                {
                    CurrentPage = page,
                    PageSize = pageSize
                };


                // ---------------------------------------------------------
                // Get visits ONCE
                // ---------------------------------------------------------

                var allVisits = _visitDAL
                    .ListByPatient(patientId, conn)
                    .OrderByDescending(x => x.VisitDate)
                    .ToList();


                // Total number of visits
                vm.TotalVisits = allVisits.Count;


                // ---------------------------------------------------------
                // Get only the visits required for this page
                // ---------------------------------------------------------

                var visits = allVisits
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // ---------------------------------------------------------
                // Retrieve related information
                // ---------------------------------------------------------

                var appointments = _appointmentDAL.ListByPatient(patientId, conn);
                var bills = _billDAL.ListByPatient(ClinicId, patientId, conn);
                var billLines = _billLineDAL.ListByPatient(patientId, conn);
                var payments = _paymentDAL.ListByPatient(patientId, conn);

                vm.TotalAppointments = appointments.Count;
                vm.TotalBills = bills.Count;
                vm.TotalPayments = payments.Count;

                // ---------------------------------------------------------
                // No visits for requested page
                // ---------------------------------------------------------

                if (visits.Count == 0)
                {
                    return vm;
                }

                // ---------------------------------------------------------
                // Build visit history
                // ---------------------------------------------------------

                foreach (var visit in visits)
                {
                    var visitHistoryVM = new VisitHistoryVM(visit);


                    // -----------------------------------------------------
                    // Appointments / Prescriptions
                    // -----------------------------------------------------

                    foreach (var appointment in appointments)
                    {
                        if (visit.VisitId != appointment.VisitId)
                            continue;


                        var prescription =
                            _consultationDAL.GetPrescription(
                                appointment.AppointmentId,
                                conn);


                        var appointmentHistoryVM =
                            new AppointmentHistoryVM(
                                appointment,
                                prescription);


                        visitHistoryVM.Appointments.Add(
                            appointmentHistoryVM);
                    }


                    // -----------------------------------------------------
                    // Bills / Bill Lines / Payments
                    // -----------------------------------------------------

                    foreach (var bill in bills)
                    {
                        if (visit.VisitId != bill.VisitId)
                            continue;


                        foreach (var line in billLines)
                        {
                            if (line.BillId == bill.BillId)
                            {
                                bill.BillLines.Add(line);
                            }
                        }


                        foreach (var payment in payments)
                        {
                            if (payment.BillId == bill.BillId)
                            {
                                bill.Payments.Add(payment);
                            }
                        }


                        visitHistoryVM.Bills.Add(bill);
                    }


                    vm.Visits.Add(visitHistoryVM);
                }


                return vm;
            }
            catch
            {
                throw;
            }
        }

        public bool SetCache(PatientSelectionVM p)
        {
            if (p == null)
                return false;

            PatientId = p.PatientId;
            PatientDisplayName = p.FullName;
            PatientGender = p.Gender ?? "";
            PatientAge = p.Age;
            PatientPhone = p.Phone;
            // Clear last visit details of previously selected patient
            LastVisitId = 0;
            LastVisitDate = null;

            return true;
        }

        public PatientSelectionVM GetCache()
        {
            PatientSelectionVM p = new PatientSelectionVM();

            p.PatientId = PatientId;
            p.FullName = PatientDisplayName;
            p.Gender = PatientGender !=  "" ? PatientGender : null;
            p.Age = PatientAge;
            p.Phone = PatientPhone;
            // Clear last visit details of previously selected patient
            p.LastVisitId = LastVisitId;
            p.LastVisitDate = LastVisitDate != null ? FormatDateTime(LastVisitDate) : null;

            return p;
        }

    }
}


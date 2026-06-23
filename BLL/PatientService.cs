using DAL;
using Models;
using ViewModels;

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
        private readonly PharmacologyDAL _pharmacologyDAL;

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
            _pharmacologyDAL = new PharmacologyDAL(config);
            _dal = _patientDAL;
        }

        public List<Patient> Search(PatientSearchVM searchVM)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                searchVM.ClinicId = ClinicId;
                return _patientDAL.Search(searchVM, conn);
            }
            catch
            {
                throw;
            }
        }

        public List<Patient> Search(string term)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                PatientSearchVM vm = new PatientSearchVM();
                vm.ClinicId = ClinicId;
                vm.SearchString = term;
                return _patientDAL.Search(vm, conn);
            }
            catch
            {
                throw;
            }
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
            }

            return p;
        }

        public PatientHistoryVM GetHistory(int patientId, int page = 1)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                var patient = _dal.Get(patientId, conn);

                if (patient == null)
                    return new PatientHistoryVM();

                PatientHistoryVM vm = new PatientHistoryVM(patient);

                var visits = _visitDAL.ListByPatient(patientId, conn)
                        .OrderByDescending(x => x.VisitDate)
                        .Skip((page - 1) * 3)
                        .Take(3)
                        .ToList();

                vm.TotalVisits = _visitDAL.ListByPatient(patientId, conn).Count();

                var appointments = _appointmentDAL.ListByPatient(patientId, conn);

                var bills = _billDAL.ListByPatient(ClinicId, patientId, conn);

                var billLines = _billLineDAL.ListByPatient(patientId, conn);

                var payments = _paymentDAL.ListByPatient(patientId, conn);

                foreach (var visit in visits)
                {
                    VisitHistoryVM visitHistoryVM = new VisitHistoryVM(visit);

                    foreach (var appointment in appointments)
                    {
                        if (visit.VisitId == appointment.VisitId)
                        {
                            var prescription = _consultationDAL.GetPrescription(appointment.AppointmentId, conn);

                            AppointmentHistoryVM appointmentHistoryVM = new AppointmentHistoryVM(appointment, prescription);

                            visitHistoryVM.Appointments.Add(appointmentHistoryVM);
                        }
                    }

                    foreach (var bill in bills)
                    {
                        if (visit.VisitId == bill.VisitId)
                        {
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

    }
}


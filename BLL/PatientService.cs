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
            searchVM.ClinicId = ClinicId;
            return _patientDAL.Search(searchVM);
        }

        public override List<Patient> List()
        {
            return _patientDAL.ListByClinic(ClinicId);
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
            var patient = _dal.Get(patientId);

            if (patient == null)
                return new PatientHistoryVM();

            PatientHistoryVM vm = new PatientHistoryVM(patient);

            var visits = _visitDAL.ListByPatient(patientId)
                      .OrderByDescending(x => x.VisitDate)
                      .Skip((page - 1) * 3)
                      .Take(3)
                      .ToList();

            vm.TotalVisits = _visitDAL.ListByPatient(patientId).Count();

            var appointments = _appointmentDAL.ListByPatient(patientId);

            var bills = _billDAL.ListByPatient(ClinicId, patientId);

            var billLines = _billLineDAL.ListByPatient(patientId);

            var payments = _paymentDAL.ListByPatient(patientId);

            foreach (var visit in visits)
            {
                VisitHistoryVM visitHistoryVM = new VisitHistoryVM(visit);

                foreach (var appointment in appointments)
                {
                    if (visit.VisitId == appointment.VisitId)
                    {
                        var prescription = _consultationDAL.GetPrescription(appointment.AppointmentId);

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

    }
}


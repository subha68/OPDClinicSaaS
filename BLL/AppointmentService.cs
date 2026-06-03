using Models;
using ViewModels;
using DAL;
using System;
using System.Collections.Generic;

namespace BLL
{
    public class AppointmentService : BaseService<Appointment>
    {
        private readonly AppointmentDAL _appointmentDAL;
        private readonly VisitDAL _visitDAL;
        private readonly AppointmentSlotDAL _slotDAL;

        public AppointmentService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(httpContextAccessor)
        {
            _appointmentDAL = new AppointmentDAL(config);
            _visitDAL = new VisitDAL(config);
            _slotDAL = new AppointmentSlotDAL(config);
            _dal = _appointmentDAL;
        }

        public List<Appointment> GetTodaysAppointments(int doctorId)
        {
            return _appointmentDAL.GetTodaysAppointments(doctorId);
        }

        public List<Appointment> ListByPatient(int patientId)
        {
            return _appointmentDAL.ListByPatient(patientId);
        }
        
        public void UpdateStatus(int appointmentId, string status)
        {
            _appointmentDAL.UpdateStatus(appointmentId, status);
        }

        public int BookAppointment(AppointmentCreateVM vm)
        {
            Visit visit = _visitDAL.GetByPatientAndDate(vm.PatientId, vm.AppointmentDate);

            if (visit == null)
            {
                visit = new Visit
                {
                    VisitDate = vm.AppointmentDate,
                    VisitType = vm.VisitType,
                    ClinicId = ClinicId,
                    PatientId = vm.PatientId
                };

                vm.VisitId = _visitDAL.Add(visit);
            }
            else
            {
                vm.VisitId = visit.VisitId;
            }

            var appointment = new Appointment
            {
                DoctorId = vm.DoctorId,
                ClinicId = ClinicId,
                PatientId = vm.PatientId,
                VisitId = vm.VisitId,
                AppointmentDate = vm.AppointmentDate,
                SlotTime = TimeSpan.Parse(vm.SlotTime),
                Status = "Booked"
            };

            int appointmentId = _appointmentDAL.Add(appointment);

            _slotDAL.IncrementBookedCount(vm.SlotId);

            return appointmentId;            
        }


    }
}
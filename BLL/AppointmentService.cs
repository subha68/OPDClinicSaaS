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

        public AppointmentService(IConfiguration config)
        {
            _appointmentDAL = new AppointmentDAL(config);
            _dal = _appointmentDAL;
        }

        public List<Appointment> GetTodaysAppointments(int doctorId)
        {
            return _appointmentDAL.GetTodaysAppointments(doctorId);
        }

        public List<Appointment> GetByPatientId(int patientId)
        {
            return _appointmentDAL.GetByPatientId(patientId);
        }
        
        public void UpdateStatus(int appointmentId, string status)
        {
            _appointmentDAL.UpdateStatus(appointmentId, status);
        }

    }
}
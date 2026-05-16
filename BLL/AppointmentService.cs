using Models;
using ViewModels;
using DAL;
using System;
using System.Collections.Generic;

namespace BLL
{
    public class AppointmentService
    {
        private readonly AppointmentDAL _appointmentDAL;
        private readonly ConsultationDAL _consultationDAL;

        public AppointmentService(IConfiguration config)
        {
            _appointmentDAL = new AppointmentDAL(config);
            _consultationDAL = new ConsultationDAL(config);
        }

        public void Insert(AppointmentVM obj)
        {
            _appointmentDAL.Insert(obj);
        }

        public List<AppointmentVM> GetTodaysAppointments(int doctorId)
        {
            return _appointmentDAL.GetTodaysAppointments(doctorId);
        }

        public void UpdateStatus(int appointmentId, string status)
        {
            _appointmentDAL.UpdateStatus(appointmentId, status);
        }

        public ConsultationVM GetConsultationVM(int appointmentId)
        {
            return _appointmentDAL.GetConsultationVM(appointmentId);
        }

        public void SaveConsultation(ConsultationVM vm)
        {
            _consultationDAL.SaveConsultation(vm);
        }
    }
}
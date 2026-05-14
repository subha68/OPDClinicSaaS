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

        public AppointmentService(IConfiguration config)
        {
            _appointmentDAL = new AppointmentDAL(config);
        }

        public void Insert(Appointment obj)
        {
            _appointmentDAL.Insert(obj);
        }
    }
}
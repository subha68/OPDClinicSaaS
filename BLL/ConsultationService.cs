using Models;
using ViewModels;
using DAL;
using System;
using System.Collections.Generic;

namespace BLL
{
    public class ConsultationService : BaseService<ConsultationVM>
    {
        private readonly ConsultationDAL _consultationDAL;

        public ConsultationService(IConfiguration config)
        {
            _consultationDAL = new ConsultationDAL(config);
            _dal = _consultationDAL;
        }

        public ConsultationVM GetConsultationVM(int appointmentId)
        {
            return _consultationDAL.GetConsultationVM(appointmentId);
        }

        public int SaveConsultation(ConsultationVM vm)
        {
            return _dal.Add(vm);
        }

        public PrescriptionVM GetPrescription(int appointmentId)
        {
            return _consultationDAL.GetPrescription(appointmentId);
        }
    }
}
using Models;
using ViewModels;
using DAL;
using System;
using System.Collections.Generic;

namespace BLL
{
    public class ConsultationService : BaseService<Consultation>
    {
        private readonly ConsultationDAL _consultationDAL;

        public ConsultationService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(config, httpContextAccessor)
        {
            _consultationDAL = new ConsultationDAL(config);
            _dal = _consultationDAL;
        }

        public Consultation GetConsultation(int appointmentId)
        {
            return _consultationDAL.GetConsultation(appointmentId);
        }

        public int SaveConsultation(Consultation model)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                int id = _dal.Add(model, conn, tran);
                tran.Commit();
                return id;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public PrescriptionVM GetPrescription(int appointmentId)
        {
            return _consultationDAL.GetPrescription(appointmentId);
        }

        public List<Consultation> GetConsultationsByPatientId(int patientId)
        {
            return _consultationDAL.GetConsultationsByPatientId(patientId);
        }

        public List<PrescriptionVM> GetPrescriptionsByPatientId(int patientId)
        {
            return _consultationDAL.GetPrescriptionsByPatientId(patientId);
        }
    }
}
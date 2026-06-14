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
        private readonly PharmacologyDAL _pharmacologyDAL;
        private readonly AppointmentDAL _appointmentDAL;
        private readonly MedicineDAL _medicineDAL;

        public ConsultationService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(config, httpContextAccessor)
        {
            _consultationDAL = new ConsultationDAL(config);
            _pharmacologyDAL = new PharmacologyDAL(config);
            _appointmentDAL = new AppointmentDAL(config);
            _medicineDAL = new MedicineDAL(config);
            _dal = _consultationDAL;
        }

        public Consultation GetConsultation(int appointmentId)
        {
            Appointment app = _appointmentDAL.Get(appointmentId);
            Consultation cons = _consultationDAL.Get(appointmentId);
            if (cons == null)
            {
                cons = new Consultation();
            }
            cons.AppointmentId = app.AppointmentId;
            cons.PatientId = app.PatientId;
            return cons;
        }

        public int SaveConsultation(Consultation model)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                int consultationId = _dal.Add(model, conn, tran);

                foreach(var med in model.pharmacologyAdvices)
                {
                    med.ConsultationId = consultationId;
                    med.AppointmentId = model.AppointmentId;

                    // add prescribed medicine if not in Medicine Master
                    if(med.MedicineId == null)
                    {
                        var medicine = _medicineDAL.GetByName(med.MedicineName);

                        if(medicine == null)
                        {
                            medicine = new Medicine
                            {
                                MedicineName = med.MedicineName,
                                GenericName = med.MedicineName,
                                UnitOfMeasure = "Tab",   
                                DosageForm = "Tablet",
                                ScheduleType = "OTC",
                                ReorderLevel = 0,
                                CreatedFromPrescription = true,
                                IsActive = true
                            };

                            medicine.MedicineId = _medicineDAL.Add(medicine, conn, tran);
                        }

                        med.MedicineId = medicine.MedicineId;
                    }

                    _pharmacologyDAL.Add(med, conn, tran);
                }

                _appointmentDAL.UpdateStatus(model.AppointmentId, "Completed", conn, tran);

                tran.Commit();
                return consultationId;
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
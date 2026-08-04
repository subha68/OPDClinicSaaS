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
        private readonly PrescriptionTemplateDAL _templateDAL;
        private readonly DoctorDAL _doctorDAL;
        private readonly AppointmentDAL _appointmentDAL;
        private readonly MedicineDAL _medicineDAL;
        private readonly PatientDAL _patientDAL;

        public ConsultationService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(config, httpContextAccessor)
        {
            _consultationDAL = new ConsultationDAL(config);
            _pharmacologyDAL = new PharmacologyDAL(config);
            _templateDAL = new PrescriptionTemplateDAL(config);
            _doctorDAL = new DoctorDAL(config);
            _appointmentDAL = new AppointmentDAL(config);
            _medicineDAL = new MedicineDAL(config);
            _patientDAL = new PatientDAL(config);
            _dal = _consultationDAL;
        }

        public Consultation GetConsultation(int appointmentId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                Appointment app = _appointmentDAL.Get(appointmentId, conn);
                Consultation cons = _consultationDAL.GetByAppointment(appointmentId, conn);
                Patient patient = _patientDAL.Get(app.PatientId, conn);
                if (cons == null)   // consultation yet to be started
                {
                    PrescriptionTemplate template = _templateDAL.GetByDoctor(app.DoctorId, conn);
                    if (template == null)
                    {
                        Doctor doctor = _doctorDAL.Get(app.DoctorId, conn);
                        template = _templateDAL.GetByDepartment(doctor.DepartmentId, conn);
                    }

                    cons = new Consultation(template, 
                                            app.AppointmentId,
                                            app.AppointmentDate,
                                            app.SlotStartTime,
                                            app.DoctorId, 
                                            app.DoctorName, 
                                            app.PatientId, 
                                            app.PatientName,
                                            patient.DOB,
                                            patient.Gender, 
                                            app.ClinicId);
                }
                else
                {
                    cons.Sections = _consultationDAL.ListSections(cons.ConsultationId, conn);
                    var rows = _consultationDAL.ListRows(cons.ConsultationId, conn);
                    var templateRows = _consultationDAL.ListTemplateRows(cons.ConsultationId, conn);
                    var fields = _consultationDAL.ListFields(cons.ConsultationId, conn);
                    var options = _consultationDAL.ListOptions(cons.ConsultationId, conn);

                    var rowsBySection = rows.ToLookup(r => r.SectionId);
                    var templateRowBySection = templateRows.ToLookup(r => r.SectionId);
                    var fieldsByRow = fields.ToLookup(f => f.RowId);
                    var optionsByField = options.ToLookup(o => o.FieldId);

                    foreach (var section in cons.Sections)
                    {
                        section.Rows = rowsBySection[section.SectionId]
                            .OrderBy(r => r.DisplayOrder)
                            .ToList();

                        foreach (var row in section.Rows)
                        {
                            row.Fields = fieldsByRow[row.RowId]
                                .OrderBy(f => f.DisplayOrder)
                                .ToList();

                            foreach (var field in row.Fields)
                            {
                                field.Options = optionsByField[field.FieldId]
                                    .OrderBy(o => o.DisplayOrder)
                                    .ToList();
                            }
                        }

                        section.TemplateRow = templateRowBySection[section.SectionId].FirstOrDefault();

                        if (section.TemplateRow != null)
                        {
                            section.TemplateRow.Fields = fieldsByRow[section.TemplateRow.RowId]
                                .OrderBy(f => f.DisplayOrder)
                                .ToList();

                            foreach (var field in section.TemplateRow.Fields)
                            {
                                field.Options = optionsByField[field.FieldId]
                                    .OrderBy(o => o.DisplayOrder)
                                    .ToList();
                            }
                        }                        

                    }
                }

                return cons;
            }
            catch
            {
                throw;
            }
        }

        public int SaveDraft(Consultation model)
        {
            model.Status = "Draft"; // Ensure the status is set to Draft when saving a draft

            return SaveConsultation(model);
        }

        public int CompleteConsultation(Consultation model)
        {
            model.Status = "Completed"; // Ensure the status is set to Completed when completing a consultation
            return SaveConsultation(model);
        }

        public int SaveConsultation(Consultation model)
        {
            if (model.ConsultationId > 0)
            {
                UpdateConsultation(model);
            }
            else
            {
                AddConsultation(model);
            }

            return model.ConsultationId;
        }

        public int AddConsultation(Consultation model)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                model.CreatedBy = UserId;

                int consultationId = _dal.Add(model, conn, tran);

                _consultationDAL.AddSections(consultationId, model.Sections, conn, tran);

                foreach (var section in model.Sections)
                {
                    if (section.IsPharmacologyAdvice)
                    {
                        _consultationDAL.AddPharmacologyAdvices(consultationId, section.RXRows, conn, tran);
                    }

                    _consultationDAL.AddRows(consultationId, section.SectionId, ClinicId, section.Rows, conn, tran);

                    foreach(var row in section.Rows)
                    {
                        _consultationDAL.AddFields(consultationId, section.SectionId, row.RowId, ClinicId, 
                                                        row.Fields, conn, tran);

                        foreach (var field in row.Fields)
                        {
                            _consultationDAL.AddOptions(consultationId, section.SectionId, row.RowId, field.FieldId, 
                                                            field.Options, conn, tran);
                        }
                    }

                    if (section.IsMultiline && section.TemplateRow != null)
                    {
                        _consultationDAL.AddTemplateRow(consultationId, section.SectionId, ClinicId, section.TemplateRow, 
                                                                conn, tran);

                        _consultationDAL.AddFields(consultationId, section.SectionId, section.TemplateRow.RowId, ClinicId, 
                                                            section.TemplateRow.Fields, conn, tran);

                        foreach (var field in section.TemplateRow.Fields)
                        {
                            _consultationDAL.AddOptions(consultationId, section.SectionId, field.RowId, ClinicId, 
                                                            field.Options, conn, tran);
                        }
                    }
                }

                if (model.Status == "Completed")
                    _consultationDAL.DeleteTemplateRows(consultationId, conn, tran);

                _appointmentDAL.UpdateStatus(model.AppointmentId, 
                                            model.Status == "Completed" ? "Completed" : "In Consultation", 
                                            null, conn, tran);

                tran.Commit();
                return consultationId;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public bool UpdateConsultation(Consultation model)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                model.CreatedBy = UserId;

                bool result = _dal.Update(model, conn, tran);

                _consultationDAL.UpdateSections(model.Sections, conn, tran);

                foreach (var section in model.Sections)
                {
                    if (section.RXRows != null && section.RXRows.Count > 0)
                    {
                        _consultationDAL.UpdatePharmacologyAdvices(section.RXRows, conn, tran);
                    }

                    List<ConsultationRow> rowsToBeAdded = new List<ConsultationRow>();
                    List<ConsultationRow> rowsToBeUpdated = new List<ConsultationRow>();
                    List<ConsultationRow> rowsToBeDeleted = new List<ConsultationRow>();

                    foreach (var row in section.Rows)
                    {
                        switch(row.RowState)
                        {
                            case RowState.Added:
                                rowsToBeAdded.Add(row);
                                break;
                            case RowState.Modified:
                                rowsToBeUpdated.Add(row);
                                break;
                            case RowState.Deleted:
                                rowsToBeDeleted.Add(row);
                                break;
                            case RowState.Unchanged:
                                break;
                        }

                        foreach (var field in row.Fields)
                        {
                            _consultationDAL.UpdateOptions(field.Options, conn, tran);
                        }
                    }

                    if (rowsToBeAdded.Count > 0)
                    {
                        _consultationDAL.AddRows(model.ConsultationId, section.SectionId, ClinicId,
                                                            rowsToBeAdded, conn, tran);
                        foreach (var row in rowsToBeAdded)
                        {
                            _consultationDAL.AddFields(model.ConsultationId, section.SectionId, row.RowId, ClinicId,
                                                            row.Fields, conn, tran);

                            foreach (var field in row.Fields)
                            {
                                _consultationDAL.AddOptions(model.ConsultationId, section.SectionId, row.RowId, ClinicId,
                                                            field.Options, conn, tran);                                
                            }
                        }
                    }

                    if (rowsToBeUpdated.Count > 0)
                    {
                        _consultationDAL.UpdateRows(rowsToBeUpdated, conn, tran);

                        foreach (var row in rowsToBeUpdated)
                        {
                            _consultationDAL.UpdateFields(row.Fields, conn, tran);

                            foreach (var field in row.Fields)
                            {
                                _consultationDAL.UpdateOptions(field.Options, conn, tran);                                
                            }
                        }
                    }

                    if (rowsToBeDeleted.Count > 0)
                    {
                        _consultationDAL.DeleteRows(rowsToBeDeleted, conn, tran);
                    }
                }

                if (model.Status == "Completed")
                    _consultationDAL.DeleteTemplateRows(model.ConsultationId, conn, tran);

                _appointmentDAL.UpdateStatus(model.AppointmentId, model.Status, null, conn, tran);

                tran.Commit();
                return result;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public PrescriptionVM GetPrescription(int appointmentId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _consultationDAL.GetPrescription(appointmentId, conn);
            }
            catch
            {
                throw;
            }
        }

        public List<Consultation> ListByPatient(int patientId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _consultationDAL.ListByPatient(patientId, conn);
            }
            catch
            {
                throw;
            }
        }

        public List<PrescriptionVM> ListPrescriptionsByPatient(int patientId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _consultationDAL.ListPrescriptionsByPatient(patientId, conn);
            }
            catch
            {
                throw;
            }
        }
    }
}
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
        private readonly PrescriptionTemplateDAL _templateDAL;
        private readonly DoctorDAL _doctorDAL;
        private readonly AppointmentDAL _appointmentDAL;
        private readonly MedicineDAL _medicineDAL;
        private readonly PatientDAL _patientDAL;

        public ConsultationService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(config, httpContextAccessor)
        {
            _consultationDAL = new ConsultationDAL(config);
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
                Appointment app = _appointmentDAL.Get(appointmentId, ClinicId, conn);
                Consultation cons = _consultationDAL.GetByAppointment(ClinicId, appointmentId, conn);
                Patient patient = _patientDAL.Get(app.PatientId, ClinicId, conn);
                if (cons == null)   // consultation yet to be started
                {
                    PrescriptionTemplate template = _templateDAL.GetByDoctor(app.DoctorId, conn);
                    if (template == null)
                    {
                        Doctor doctor = _doctorDAL.Get(app.DoctorId, ClinicId, conn);
                        template = _templateDAL.GetByDepartment(doctor.DepartmentId, ClinicId, conn);
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
                    cons.Sections = _consultationDAL.ListSections(cons.ConsultationId, ClinicId, conn);
                    var rows = _consultationDAL.ListRows(cons.ConsultationId, ClinicId, conn);
                    var templateRows = _consultationDAL.ListTemplateRows(cons.ConsultationId, ClinicId, conn);
                    var fields = _consultationDAL.ListFields(cons.ConsultationId, ClinicId, conn);
                    var options = _consultationDAL.ListOptions(cons.ConsultationId, ClinicId, conn);

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
                                if (section.IsMultiline)
                                    field.HideLabel = true;

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
                                if (section.IsMultiline)
                                    field.HideLabel = true;

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
            model.Status = ConsultationStatus.Draft; // Ensure the status is set to Draft when saving a draft

            return SaveConsultation(model);
        }

        public int CompleteConsultation(Consultation model)
        {
            model.Status = ConsultationStatus.Completed; // Ensure the status is set to Completed when completing a consultation
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
                model.ConsultationId = AddConsultation(model);
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
                model.ClinicId = ClinicId;
                model.CreatedBy = UserId;

                int consultationId = _dal.Add(model, conn, tran);

                foreach (var section in model.Sections)
                {
                    section.ConsultationId = consultationId;
                    section.ClinicId = ClinicId;
                }

                _consultationDAL.AddSections(model.Sections, conn, tran);

                foreach (var section in model.Sections)
                {
                    foreach (var row in section.Rows)
                    {
                        row.SectionId = section.SectionId;
                        row.ClinicId = ClinicId;
                    }

                    _consultationDAL.AddRows(section.Rows, conn, tran);

                    foreach(var row in section.Rows)
                    {
                        foreach (var field in row.Fields)
                        {
                            field.RowId = row.RowId;
                            field.ClinicId = ClinicId;
                        }

                        _consultationDAL.AddFields(row.Fields, conn, tran);

                        foreach (var field in row.Fields)
                        {
                            foreach (var option in field.Options)
                            {
                                option.FieldId = field.FieldId;
                                option.ClinicId = ClinicId;
                            }

                            _consultationDAL.AddOptions(field.Options, conn, tran);
                        }
                    }

                    if (section.IsMultiline && section.TemplateRow != null)
                    {
                        section.TemplateRow.SectionId = section.SectionId;
                        section.TemplateRow.ClinicId = ClinicId;

                        _consultationDAL.AddTemplateRow(section.TemplateRow, conn, tran);

                        foreach (var field in section.TemplateRow.Fields)
                        {
                            field.RowId = section.TemplateRow.RowId;
                            field.ClinicId = ClinicId;
                        }

                        _consultationDAL.AddFields(section.TemplateRow.Fields, conn, tran);

                        foreach (var field in section.TemplateRow.Fields)
                        {
                            foreach (var option in field.Options)
                            {
                                option.FieldId = field.FieldId;
                                option.ClinicId = ClinicId;
                            }

                            _consultationDAL.AddOptions(field.Options, conn, tran);
                        }
                    }
                }

                if (model.IsCompleted)
                {
                    _consultationDAL.DeleteTemplateRows(consultationId, ClinicId, conn, tran);
                    _consultationDAL.DeleteUnusedOptions(consultationId, ClinicId, conn, tran);
                    _consultationDAL.DeleteUnusedFields(consultationId, ClinicId, conn, tran);
                }

                _appointmentDAL.UpdateStatus(model.AppointmentId, 
                                            model.IsCompleted ? "Completed" : "In Consultation", 
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
                    List<ConsultationRow> rowsToBeAdded = new List<ConsultationRow>();
                    List<ConsultationRow> rowsToBeUpdated = new List<ConsultationRow>();
                    List<ConsultationRow> rowsToBeDeleted = new List<ConsultationRow>();

                    foreach (var row in section.Rows)
                    {
                        row.ClinicId = ClinicId;

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

                    /*    foreach (var field in row.Fields)
                        {
                            _consultationDAL.UpdateOptions(field.Options, conn, tran);
                        } */
                    }

                    if (rowsToBeAdded.Count > 0)
                    {
                        foreach (var row in rowsToBeAdded)
                        {
                            row.ClinicId = ClinicId;
                        }

                        _consultationDAL.AddRows(rowsToBeAdded, conn, tran);

                        foreach (var row in rowsToBeAdded)
                        {
                            foreach (var field in row.Fields)
                            {
                                field.RowId = row.RowId;
                                field.ClinicId = ClinicId;
                            }

                            _consultationDAL.AddFields(row.Fields, conn, tran);

                            foreach (var field in row.Fields)
                            {
                                foreach (var option in field.Options)
                                {
                                    option.FieldId = field.FieldId;
                                    option.ClinicId = ClinicId;
                                }

                                _consultationDAL.AddOptions(field.Options, conn, tran);                                
                            }
                        }
                    }

                    if (rowsToBeUpdated.Count > 0)
                    {
                        foreach (var row in rowsToBeUpdated)
                        {
                            row.ClinicId = ClinicId;
                        }

                        _consultationDAL.UpdateRows(rowsToBeUpdated, conn, tran);

                        foreach (var row in rowsToBeUpdated)
                        {
                            foreach (var field in row.Fields)
                            {
                                field.ClinicId = ClinicId;

                                foreach (var option in field.Options)
                                {
                                    option.ClinicId = ClinicId;
                                }

                                _consultationDAL.UpdateOptions(field.Options, conn, tran);
                            }

                            _consultationDAL.UpdateFields(row.Fields, conn, tran);
                        }                        
                    }

                    if (rowsToBeDeleted.Count > 0)
                    {
                        _consultationDAL.DeleteRows(rowsToBeDeleted, conn, tran);
                    }
                }

                if (model.IsCompleted)
                {
                    _consultationDAL.DeleteTemplateRows(model.ConsultationId, ClinicId, conn, tran);
                    _consultationDAL.DeleteUnusedOptions(model.ConsultationId, ClinicId, conn, tran);
                    _consultationDAL.DeleteUnusedFields(model.ConsultationId, ClinicId, conn, tran);
                }

                string appointmentStatus = model.IsCompleted ? "Completed" : "In Consultation";
                _appointmentDAL.UpdateStatus(model.AppointmentId, appointmentStatus, null, conn, tran);

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
                PrescriptionVM prescription = _consultationDAL.GetPrescription(appointmentId, conn);

                List<PrescriptionSectionVM> sections = _consultationDAL.ListPrescriptionSections(prescription.ConsultationId, conn);
                List<PrescriptionRowVM> rows = _consultationDAL.ListPrescriptionRows(prescription.ConsultationId, conn);
                List<PrescriptionFieldVM> fields = _consultationDAL.ListPrescriptionFields(prescription.ConsultationId, conn);
                List<PrescriptionOptionVM> options = _consultationDAL.ListPrescriptionOptions(prescription.ConsultationId, conn);

                prescription.Sections = sections;

                foreach (var section in sections)
                {
                    section.Rows = rows
                        .Where(r => r.SectionId == section.SectionId)
                        .ToList();
                }

                foreach (var row in rows)
                {
                    row.Fields = fields
                        .Where(r => r.RowId == row.RowId)
                        .ToList();                    
                }

                foreach (var field in fields)
                {
                    field.Options = options
                        .Where(r => r.FieldId == field.FieldId)
                        .ToList();
                }

                return prescription;
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
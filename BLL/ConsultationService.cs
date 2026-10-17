using Models;
using ViewModels;
using DAL;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Text.RegularExpressions;


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
        private readonly VisitDAL _visitDAL;
        private readonly VitalDefinitionDAL _vitalDefDAL;
        private readonly ConsultationVitalDAL _rowVitalDAL;
        private readonly ConsultationTestResultDAL _rowResultDAL;
        private readonly PatientService _patient;
        private readonly VitalService _vital;
        private readonly TestResultService _testResult;

        public ConsultationService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(config, httpContextAccessor)
        {
            _consultationDAL = new ConsultationDAL(config);
            _templateDAL = new PrescriptionTemplateDAL(config);
            _doctorDAL = new DoctorDAL(config);
            _appointmentDAL = new AppointmentDAL(config);
            _medicineDAL = new MedicineDAL(config);
            _patientDAL = new PatientDAL(config);
            _visitDAL = new VisitDAL(config);
            _vitalDefDAL = new VitalDefinitionDAL(config);
            _rowVitalDAL = new ConsultationVitalDAL(config);
            _rowResultDAL = new ConsultationTestResultDAL(config);

            _patient = new PatientService(config, httpContextAccessor);
            _vital = new VitalService(config, httpContextAccessor);
            _testResult = new TestResultService(config, httpContextAccessor);

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
                Visit visit = _visitDAL.Get(app.VisitId, ClinicId, conn);

                if (cons == null)   // consultation yet to be started
                {
                    PrescriptionTemplate template = _templateDAL.GetByDoctor(app.DoctorId, ClinicId, conn);
                    if (template == null)
                    {
                        Doctor doctor = _doctorDAL.Get(app.DoctorId, ClinicId, conn);
                        template = _templateDAL.GetByDepartment(doctor.DepartmentId, ClinicId, conn);
                    }

                    if (template == null)
                    {
                        template = _templateDAL.GetByClinic(ClinicId, conn);
                    }

                    // below constructor creates consultation, section and row(s)
                    // also creates fields unless section is vital or test results

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

                    // check for vitals & test results
                    foreach (var templateSection in template.Sections)
                    {
                        if (templateSection.SectionType == SectionType.Vital)
                        {
                            foreach (var sec in cons.Sections)
                            {
                                if (sec.SectionName != templateSection.SectionName)
                                    continue;

                                RefreshVitals(app.PatientId, app.VisitId, visit.VisitDate, ClinicId, sec.Rows[0], conn);
                                break;
                            }
                        }
                        else if (templateSection.SectionType == SectionType.TestResult)
                        {
                            foreach (var sec in cons.Sections)
                            {
                                if (sec.SectionName != templateSection.SectionName)
                                    continue;

                                RefreshTestResults(app.PatientId, ClinicId, sec.Rows[0], conn);
                                break;
                            }
                        }
                        else
                        {
                            // do nothing - taken care of by constructor
                        }                        
                    }
                }
                else   // consultation already started
                {
                    cons.Sections = _consultationDAL.ListSections(cons.ConsultationId, ClinicId, conn);
                    var rows = _consultationDAL.ListRows(cons.ConsultationId, ClinicId, conn);
                    var templateRows = _consultationDAL.ListTemplateRows(cons.ConsultationId, ClinicId, conn);
                    var vitals = _rowVitalDAL.ListByConsultation(cons.ConsultationId, ClinicId, conn);
                    var testResultGroups = _rowResultDAL.ListGroupsByConsultation(cons.ConsultationId, ClinicId, conn);
                    var testResults = _rowResultDAL.ListByConsultation(cons.ConsultationId, ClinicId, conn);
                    var fields = _consultationDAL.ListFields(cons.ConsultationId, ClinicId, conn);
                    var options = _consultationDAL.ListOptions(cons.ConsultationId, ClinicId, conn);

                    var rowsBySection = rows.ToLookup(r => r.SectionId);
                    var templateRowBySection = templateRows.ToLookup(r => r.SectionId);
                    var vitalsByRow = vitals.ToLookup(r => r.RowId);
                    var testResultGroupsByRow = testResultGroups.ToLookup(r => r.RowId);
                    var fieldsByRow = fields.ToLookup(f => f.RowId);
                    var optionsByField = options.ToLookup(o => o.FieldId);

                    foreach (var section in cons.Sections)
                    {
                        section.Rows = rowsBySection[section.SectionId]
                            .OrderBy(r => r.DisplayOrder)
                            .ToList();

                    //    if (!section.Rows.Any())
                    //        continue;

                        foreach (var row in section.Rows)
                        {
                            if (section.SectionType == SectionType.Vital)
                            {
                                if (cons.IsCompleted)
                                {
                                    row.Vitals = vitalsByRow[row.RowId]
                                        .OrderBy(f => f.DisplayOrder)
                                        .ToList();
                                }
                                else
                                {
                                    RefreshVitals(app.PatientId, app.VisitId, visit.VisitDate, ClinicId, row, conn);                                        
                                }

                                continue;
                            }

                            if (section.SectionType == SectionType.TestResult)
                            {
                                if (cons.IsCompleted)
                                {
                                    row.TestResultGroups = testResultGroupsByRow[row.RowId]
                                        .OrderByDescending(f => f.TestDate)
                                        .ToList();

                                    foreach (var group in row.TestResultGroups)
                                    {
                                        group.TestResults = testResults
                                                        .Where(r => r.ConsultationTestResultGroupId == group.ConsultationTestResultGroupId)
                                                        .OrderBy(f => f.PanelDisplayOrder)
                                                        .ThenBy(f => f.ParameterDisplayOrder)
                                                        .ToList();
                                    }
                                }
                                else
                                {
                                    RefreshTestResults(app.PatientId, ClinicId, row, conn);
                                }

                                continue;
                            }

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

                // set the patient cache
                if (patient != null)
                    _patient.SetCache(new PatientSelectionVM(patient));
                
                return cons;
            }
            catch
            {
                throw;
            }
        }

        private void RefreshVitals(int patientId, int visitId, DateTime visitDate, int clinicId, 
                                                        ConsultationRow row, SqlConnection conn, SqlTransaction? tran = null)
        {
            // delete consultation row vitals, if any, from consultation model
            row.Vitals ??= [];
            row.Vitals.Clear();            

            // get current vitals for most recent visit of patient
            var vitals = _vital.GetVitalsByVisit(patientId, visitId, ClinicId, conn, tran);

            foreach (var vital in vitals)
            {
                var rowVital = new ConsultationVital(vital);
                rowVital.VisitDate = visitDate;
                rowVital.RowId = row.RowId;

                row.Vitals.Add(rowVital);
            }

            row.Vitals = row.Vitals
                            .OrderBy(v => v.DisplayOrder ?? int.MaxValue)
                            .ToList();
                            
            return;
        }

        private void RefreshTestResults(int patientId, int clinicId, ConsultationRow row, 
                                            SqlConnection conn, SqlTransaction? tran = null)
        {
            // delete consultation row test results, if any, from consultation model
            row.TestResultGroups ??= [];
            row.TestResultGroups.Clear();            

            // get latest test result groups & results for patient
            List<TestResultGroup> groups = _testResult.GetLatestByParameter(patientId, clinicId, conn, tran);

            List<ConsultationTestResultGroup> consGroups = new List<ConsultationTestResultGroup>();

            foreach (var group in groups)
            {
                var consGroup = new ConsultationTestResultGroup(group);

                consGroup.RowId = row.RowId;

                if (group.Results != null && group.Results.Count > 0)
                {
                    foreach (var result in group.Results)
                    {
                        consGroup.TestResults ??= [];
                        var consResult = new ConsultationTestResult(result);
                        consGroup.TestResults.Add(consResult);
                    }

                    consGroup.TestResults = consGroup.TestResults
                                                .OrderBy(r => r.PanelDisplayOrder)
                                                .ThenBy(r => r.ParameterDisplayOrder)
                                                .ToList();
                } 

                consGroups.Add(consGroup);
            }

            row.TestResultGroups = consGroups
                                    .OrderByDescending(g => g.TestDate)
                                    .ToList();

            return;
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

                    if (section.SectionType == SectionType.Vital)
                        OverrideConsultationVitals(section.Rows[0].Vitals, section.Rows[0].RowId, ClinicId, conn, tran);
                    else if (section.SectionType == SectionType.TestResult)
                        OverrideConsultationTestResults(section.Rows[0].TestResultGroups, section.Rows[0].RowId, 
                                                            model.PatientId, ClinicId, conn, tran);
                    else
                    {
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

                List<ConsultationRow> rowsToBeAdded = new List<ConsultationRow>();
                List<ConsultationRow> rowsToBeUpdated = new List<ConsultationRow>();
                List<ConsultationRow> rowsToBeDeleted = new List<ConsultationRow>();

                foreach (var section in model.Sections)
                {
                    foreach (var row in section.Rows)
                    {
                        if (section.SectionType == SectionType.Vital)
                        {
                            OverrideConsultationVitals(row.Vitals, row.RowId, ClinicId, conn, tran);
                            continue;
                        }
                        
                        if (section.SectionType == SectionType.TestResult)
                        {
                            OverrideConsultationTestResults(row.TestResultGroups, row.RowId, model.PatientId, 
                                                                                ClinicId, conn, tran);
                            continue;
                        }
                            
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

                    }
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

        private void OverrideConsultationVitals(List<ConsultationVital> vitals, int rowId, int clinicId,
                                                    SqlConnection conn, SqlTransaction tran)
        {
            // delete exisiting vitals within consultation row from database
            _rowVitalDAL.DeleteByRow(rowId, clinicId, conn, tran);

            vitals ??= [];

            // insert new vitals in consultation row into database
            foreach (var vital in vitals)
            {
                vital.RowId = rowId;
                vital.ClinicId = clinicId;
            }

            _rowVitalDAL.Add(vitals, conn, tran);
        }

        private void OverrideConsultationTestResults(List<ConsultationTestResultGroup> resultGroups, int rowId, 
                                                    int patientId, int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            // delete exisiting test results within consultation row from database
            _rowResultDAL.DeleteByRow(rowId, clinicId, conn, tran);
            _rowResultDAL.DeleteGroupsByRow(rowId, clinicId, conn, tran);

            resultGroups ??= [];

            // insert new vitals in consultation row into database
            foreach (var group in resultGroups)
            {
                group.RowId = rowId;
                group.ClinicId = clinicId;
                group.PatientId = patientId;
            }

            _rowResultDAL.AddResultGroups(resultGroups, conn, tran);

            foreach (var group in resultGroups)
            {
                foreach (var result in group.TestResults)
                {
                    result.ConsultationTestResultGroupId = group.ConsultationTestResultGroupId;
                    result.ClinicId = clinicId;
                }

                _rowResultDAL.Add(group.TestResults, conn, tran);
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
                List<PrescriptionVitalVM> vitals = _consultationDAL.ListPrescriptionVitals(prescription.ConsultationId, ClinicId, 
                                                                                                    conn);
                List<PrescriptionTestResultVM> testResults = _consultationDAL.ListPrescriptionTestResults(prescription.ConsultationId, 
                                                                                                        ClinicId, conn);

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

                foreach (var row in rows)
                {
                    row.Vitals = vitals
                        .Where(r => r.RowId == row.RowId)
                        .Where(r => !r.IsCompositionItem)
                        .OrderBy(r => r.DisplayOrder)
                        .ToList();                    
                }

                foreach (var row in rows)
                {
                    row.TestResults = testResults
                        .Where(r => r.RowId == row.RowId)
                        .OrderByDescending(r => r.TestDate)
                        .ThenBy(r => r.PanelDisplayOrder)
                        .ThenBy(r => r.ParameterDisplayOrder)
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
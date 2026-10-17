using Models;
using ViewModels;
using DAL;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Helpers;

namespace BLL
{
    public class PrescriptionTemplateService : BaseService<PrescriptionTemplate>
    {
        private readonly PrescriptionTemplateDAL _templateDAL;
        private readonly DepartmentDAL _departmentDAL;
        private readonly DoctorDAL _doctorDAL;

        public PrescriptionTemplateService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(config, httpContextAccessor)
        {
            _templateDAL = new PrescriptionTemplateDAL(config);
            _departmentDAL = new DepartmentDAL(config);
            _doctorDAL = new DoctorDAL(config);
            _dal = _templateDAL;
        }

        public override int Add(PrescriptionTemplate template, SqlConnection conn, SqlTransaction tran)
        {
            if (template == null)
                throw new ArgumentNullException(nameof(template));

            if (template.TemplateId > 0)
                return template.TemplateId;

            template.Sections ??= new List<PrescriptionTemplateSection>();

            // Insert the template first so that its generated TemplateId
            // is available to all child records.
            int templateId = _templateDAL.Add(template, conn, tran);

            int? targetClinicId = template.ClinicId;

            foreach (var section in template.Sections)
            {
                section.TemplateId = templateId;
                section.ClinicId = targetClinicId;
            }

            if (template.Sections.Count > 0)
                _templateDAL.AddSections(template.Sections, conn, tran);

            var fields = new List<PrescriptionTemplateField>();

            foreach (var section in template.Sections)
            {
                section.Fields ??= new List<PrescriptionTemplateField>();

                foreach (var field in section.Fields)
                {
                    field.TemplateId = templateId;
                    field.SectionId = section.SectionId;
                    field.ClinicId = targetClinicId;
                    field.Options ??= new List<PrescriptionTemplateFieldOption>();

                    fields.Add(field);
                }
            }

            if (fields.Count > 0)
                _templateDAL.AddFields(fields, conn, tran);

            var options = new List<PrescriptionTemplateFieldOption>();

            foreach (var section in template.Sections)
            {
                foreach (var field in section.Fields)
                {
                    foreach (var option in field.Options)
                    {
                        option.TemplateId = templateId;
                        option.SectionId = section.SectionId;
                        option.FieldId = field.FieldId;
                        option.ClinicId = targetClinicId;

                        options.Add(option);
                    }
                }
            }

            if (options.Count > 0)
                _templateDAL.AddOptions(options, conn, tran);

            return templateId;
        }


        public override bool Update(PrescriptionTemplate template, SqlConnection conn, SqlTransaction tran)
        {
            if (template == null)
                throw new ArgumentNullException(nameof(template));

            if (template.TemplateId <= 0)
                throw new ArgumentException(
                    "TemplateId must be greater than zero.",
                    nameof(template));

            template.Sections ??= new List<PrescriptionTemplateSection>();

            int? targetClinicId = template.ClinicId;

            bool affected = _templateDAL.Update(template, conn, tran);

            var newSections = new List<PrescriptionTemplateSection>();
            var modifiedSections = new List<PrescriptionTemplateSection>();

            foreach (var section in template.Sections)
            {
                section.TemplateId = template.TemplateId;
                section.ClinicId = targetClinicId;

                if (section.SectionId > 0)
                    modifiedSections.Add(section);
                else
                    newSections.Add(section);
            }

            if (newSections.Count > 0)
            {
                _templateDAL.AddSections(newSections, conn, tran);
                affected = true;
            }

            if (modifiedSections.Count > 0)
                affected |= _templateDAL.UpdateSections(modifiedSections, conn, tran);

            var newFields = new List<PrescriptionTemplateField>();
            var modifiedFields = new List<PrescriptionTemplateField>();

            foreach (var section in template.Sections)
            {
                section.Fields ??= new List<PrescriptionTemplateField>();

                foreach (var field in section.Fields)
                {
                    field.TemplateId = template.TemplateId;
                    field.SectionId = section.SectionId;
                    field.ClinicId = targetClinicId;
                    field.Options ??= new List<PrescriptionTemplateFieldOption>();

                    if (field.FieldId > 0)
                        modifiedFields.Add(field);
                    else
                        newFields.Add(field);
                }
            }

            if (newFields.Count > 0)
            {
                _templateDAL.AddFields(newFields, conn, tran);
                affected = true;
            }

            if (modifiedFields.Count > 0)
                affected |= _templateDAL.UpdateFields(modifiedFields, conn, tran);

            var newOptions = new List<PrescriptionTemplateFieldOption>();
            var modifiedOptions = new List<PrescriptionTemplateFieldOption>();

            foreach (var section in template.Sections)
            {
                foreach (var field in section.Fields)
                {
                    foreach (var option in field.Options)
                    {
                        option.TemplateId = template.TemplateId;
                        option.SectionId = section.SectionId;
                        option.FieldId = field.FieldId;
                        option.ClinicId = targetClinicId;

                        if (option.OptionId > 0)
                            modifiedOptions.Add(option);
                        else
                            newOptions.Add(option);
                    }
                }
            }

            if (newOptions.Count > 0)
            {
                _templateDAL.AddOptions(newOptions, conn, tran);
                affected = true;
            }

            if (modifiedOptions.Count > 0)
                affected |= _templateDAL.UpdateOptions(modifiedOptions, conn, tran);

            return affected;
        }

        public PrescriptionTemplate GetDefault()
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _templateDAL.GetDefault(ClinicId, conn);
            }
            catch
            {
                throw;
            }

        }

        public PrescriptionTemplate GetByDepartment(int departmentId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _templateDAL.GetByDepartment(departmentId, ClinicId, conn);
            }
            catch
            {
                throw;
            }

        }

        public PrescriptionTemplate GetByDoctor(int doctorId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _templateDAL.GetByDoctor(doctorId, ClinicId, conn);
            }
            catch
            {
                throw;
            }

        }

        public List<PrescriptionTemplateSection> GetSections(int templateId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _templateDAL.GetSections(templateId, conn);
            }
            catch
            {
                throw;
            }

        }

        public List<PrescriptionTemplateField> GetFieldsForSection(int sectionId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _templateDAL.GetFieldsForSection(sectionId, conn);
            }
            catch
            {
                throw;
            }

        }

        public List<PrescriptionTemplateFieldOption> GetOptionsForField(int fieldId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _templateDAL.GetOptionsForField(fieldId, conn);
            }
            catch
            {
                throw;
            }

        }

        public void SaveSectionOrder(List<SectionOrderVM> sections)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                _templateDAL.SaveSectionOrder(sections, GetClinicId(), conn, tran);
                tran.Commit();
            }
            catch
            {
                tran.Rollback();
                throw;
            }

        }

        public void SaveFieldOrder(List<FieldOrderVM> fields)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                _templateDAL.SaveFieldOrder(fields, GetClinicId(), conn, tran);
                tran.Commit();
            }
            catch
            {
                tran.Rollback();
                throw;
            }

        }

        public void SaveOptionOrder(List<OptionOrderVM> options)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                _templateDAL.SaveOptionOrder(options, GetClinicId(), conn, tran);
                tran.Commit();
            }
            catch
            {
                tran.Rollback();
                throw;
            }

        }

        public PrescriptionTemplateSection ToggleSection(int templateId, int sectionId)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                bool rowsAffected = _templateDAL.ToggleSection(templateId, sectionId, GetClinicId(), conn, tran);
                var model = _templateDAL.GetSection(templateId, sectionId, conn, tran);
                tran.Commit();
                return model;
            }
            catch
            {
                tran.Rollback();
                throw;
            }

        }

        public PrescriptionTemplateField ToggleField(int templateId, int fieldId)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                bool rowsAffected = _templateDAL.ToggleField(templateId, fieldId, GetClinicId(), conn, tran);
                var model = _templateDAL.GetField(templateId, fieldId, conn, tran);
                tran.Commit();
                return model;
            }
            catch
            {
                tran.Rollback();
                throw;
            }

        }

        public PrescriptionTemplateFieldOption ToggleOption(int templateId, int optionId)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                bool rowsAffected = _templateDAL.ToggleOption(templateId, optionId, GetClinicId(), conn, tran);
                var model = _templateDAL.GetOption(templateId, optionId, conn, tran);
                tran.Commit();
                return model;
            }
            catch
            {
                tran.Rollback();
                throw;
            }

        }

        public int InsertSection(int templateId, int sectionId, string position, string sectionName)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                int newSectionId = 0;

                var section = _templateDAL.GetSection(templateId, sectionId, conn, tran);

                int currDisplayOrder = section.DisplayOrder;

                if (position == "Before")
                    newSectionId = _templateDAL.InsertSectionBefore(templateId, sectionId, GetClinicId(), 
                                                                    sectionName, currDisplayOrder, conn, tran);
                else
                    newSectionId = _templateDAL.InsertSectionAfter(templateId, sectionId, GetClinicId(), 
                                                                    sectionName, currDisplayOrder, conn, tran);

                _templateDAL.AddField(templateId, newSectionId, GetClinicId(), "New Field", "Text", conn, tran);

                tran.Commit();
                return newSectionId;
            }
            catch
            {
                tran.Rollback();
                throw;
            }

        }

        public PrescriptionTemplateField InsertField(int templateId, int sectionId, int fieldId, string position, string fieldName)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                int newFieldId = 0;

                var field = _templateDAL.GetField(templateId, fieldId, conn, tran);

                int currOrder = field.DisplayOrder;

                if (position == "Before")
                    newFieldId = _templateDAL.InsertFieldBefore(templateId, sectionId, fieldId, GetClinicId(), 
                                                                    fieldName, currOrder, conn, tran);
                else
                    newFieldId = _templateDAL.InsertFieldAfter(templateId, sectionId, fieldId, GetClinicId(), 
                                                                    fieldName, currOrder, conn, tran);

                var newField = _templateDAL.GetField(templateId, newFieldId, conn, tran);

                tran.Commit();
                return newField;
            }
            catch
            {
                tran.Rollback();
                throw;
            }

        }

        public PrescriptionTemplateFieldOption InsertOption(int templateId, int fieldId, int optionId, 
                                                        string position, string optionName)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                int newOptionId = 0;

                var option = _templateDAL.GetOption(templateId, optionId, conn, tran);

                int currOrder = option.DisplayOrder;

                if (position == "Before")
                    newOptionId = _templateDAL.InsertOptionBefore(templateId, option.SectionId, fieldId, optionId, GetClinicId(),
                                                                    optionName, currOrder, conn, tran);
                else
                    newOptionId = _templateDAL.InsertOptionAfter(templateId, option.SectionId, fieldId, optionId, GetClinicId(),
                                                                    optionName, currOrder, conn, tran);

                var newOption = _templateDAL.GetOption(templateId, newOptionId, conn, tran);

                tran.Commit();
                return newOption;
            }
            catch
            {
                tran.Rollback();
                throw;
            }

        }

        public bool SaveSectionName(int templateId, int sectionId, string sectionName)
        {
            if (templateId == 0 || sectionId == 0)
                return false;

            string name = (sectionName ?? "").Trim();

            if (string.IsNullOrWhiteSpace(name))
                return false;

            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                bool updated = _templateDAL.SaveSectionName(templateId, sectionId, GetClinicId(), name, conn, tran);

                if (!updated)
                {
                    tran.Rollback();
                    return false;
                }

                tran.Commit();
                return true;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public bool SaveFieldName(int templateId, int fieldId, string fieldName)
        {
            if (templateId == 0 || fieldId == 0)
                return false;

            string name = (fieldName ?? "").Trim();

            if (string.IsNullOrWhiteSpace(name))
                return false;

            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                bool updated = _templateDAL.SaveFieldName(templateId, fieldId, GetClinicId(), name, conn, tran);

                if (!updated)
                {
                    tran.Rollback();
                    return false;
                }

                tran.Commit();
                return true;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public bool SaveOptionName(int templateId, int optionId, string optionName)
        {
            if (templateId == 0 || optionId == 0)
                return false;

            string name = (optionName ?? "").Trim();

            if (string.IsNullOrWhiteSpace(name))
                return false;

            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                bool updated = _templateDAL.SaveOptionName(templateId, optionId, GetClinicId(), name, conn, tran);

                if (!updated)
                {
                    tran.Rollback();
                    return false;
                }

                tran.Commit();
                return true;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public FieldTypeChangeVM SaveFieldType(int templateId, int fieldId, string fieldTypeCode, bool force)
        {
            FieldTypeChangeVM vm = null;

            if (fieldId <= 0 || string.IsNullOrWhiteSpace(fieldTypeCode))
            {
                return new FieldTypeChangeVM(false, "BadRequest", "Invalid request.");
            }

            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                var field = _templateDAL.GetField(templateId, fieldId, conn, tran);

                if (field == null)
                {
                    vm = new FieldTypeChangeVM(false, "NotFound", "Field not found.");
                    tran.Commit();
                    return vm;
                }

                var currentType = field.FieldTypeCode;
                var optionCount = field.OptionCount;
                var sectionId = field.SectionId;

                // Check if this field type supports options
                bool currentSupportsOptions =
                    currentType == "Dropdown" ||
                    currentType == "CheckboxGroup" ||
                    currentType == "RadioGroup";

                bool newSupportsOptions =
                    fieldTypeCode == "Dropdown" ||
                    fieldTypeCode == "CheckboxGroup" ||
                    fieldTypeCode == "RadioGroup";

                // If switching FROM options-type TO non-options-type
                // and options exist → require confirmation
                if (currentSupportsOptions && !newSupportsOptions && optionCount > 0 && !force)
                {
                    vm = new FieldTypeChangeVM(false,
                            "ConfirmationRequired",
                            $"Changing field type will delete {optionCount} existing option(s). Do you want to continue?",
                            true);
                    tran.Commit();
                    return vm;
                }

                // If confirmed OR safe change → apply update
                if (currentSupportsOptions && !newSupportsOptions && optionCount > 0 && force)
                {
                    bool result =_templateDAL.DeleteOptions(templateId, fieldId, GetClinicId(), conn, tran);

                    if (!result)
                    {
                        vm = new FieldTypeChangeVM(false, "Error", "Failed to delete existing options.");
                        tran.Rollback();
                        return vm;
                    }
                }

                bool updateResult = _templateDAL.UpdateFieldType(templateId, fieldId, GetClinicId(), fieldTypeCode, conn, tran);

                if (!updateResult)
                {
                    vm = new FieldTypeChangeVM(false, "Error", "Failed to update field type.");
                    tran.Rollback();
                    return vm;
                }

                // If switching FROM non-options-type TO options-type
                // then add one options line
                if (!currentSupportsOptions && newSupportsOptions)
                {
                    _templateDAL.AddOption(templateId, sectionId, GetClinicId(), fieldId, "New", true, 1, conn, tran);
                }

                //var fieldTypeName = _template.GetFieldTypeName(fieldTypeCode);

                vm = new FieldTypeChangeVM(true, "Success", fieldTypeCode, fieldTypeCode, newSupportsOptions);
                tran.Commit();
                return vm;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public List<FieldType> ListFieldTypes()
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _templateDAL.ListFieldTypes(conn);
            }
            catch
            {
                throw;
            }
        }

        public List<PrescriptionTemplate> Search(string term)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _templateDAL.SearchFromLibrary(term, conn);
            }
            catch
            {
                throw;
            }

        }

        public bool SaveSectionOptions(int templateId, int sectionId, bool isMultiline, bool isRx, bool isVital, bool isTestResult)
        {
            if (templateId == 0 || sectionId == 0)
                return false;

            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                if (isRx)
                    isMultiline = true;

                bool saveResult = _templateDAL.SaveSectionOptions(templateId, sectionId, GetClinicId(), isMultiline, isRx, isVital,
                                                isTestResult, conn, tran);

                if (!saveResult)
                {
                    tran.Rollback();
                    return false;
                }

                PrescriptionTemplateSection section = _templateDAL.GetSectionAndAllFields(templateId, sectionId, conn, tran);

                List<List<object>> rxFields = new List<List<object>>
                { 
                    // Fieldabel, FieldTypeCode, DisplayOrder, FieldCode, FieldSource
                    new List<object> {"Medicine", "Lookup", 1, FieldCode.Medicine, "Medicines"},
                    new List<object> {"Dose", "Dropdown", 2, FieldCode.Dose, null},
                    new List<object> {"Timing", "Dropdown", 3, FieldCode.Timing, null},
                    new List<object> {"Duration", "Number", 4, FieldCode.Duration, null},
                    new List<object> {"Unit", "Dropdown", 5, FieldCode.Unit, null},
                    new List<object> {"Instructions", "Text", 6, FieldCode.Instructions, null}                        
                };

                Dictionary<string, List<string>> rxOptions = new ()
                {
                    // Key - FieldLabel, Values - OptionText
                    {"Dose", new List<string> {"Once Daily", "Twice Daily", "Thrice Daily", "SOS"}},
                    {"Timing", new List<string> {"Before Meal", "After Meal", "During Meal", "At Bedtime"}},
                    {"Unit", new List<string> {"Days", "Weeks", "Months", "Years"}}
                };

                if (isRx)
                    EnableRxFields(templateId, section, rxFields, rxOptions, conn, tran);
                else
                    ArchiveRxFields(section, rxFields, conn, tran);

                bool archiveStandardFields = isRx || isVital || isTestResult;

                _templateDAL.UpdateFieldsArchival(templateId, sectionId, GetClinicId(), archiveStandardFields, conn, tran);
                _templateDAL.UpdateOptionsArchival(templateId, sectionId, GetClinicId(), archiveStandardFields, conn, tran);

                tran.Commit();
                return true;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        private void EnableRxFields(int templateId, PrescriptionTemplateSection section, 
                                    List<List<object>> rxFields, Dictionary<string, List<string>> rxOptions,
                                    SqlConnection conn, SqlTransaction tran)
        {
            List <PrescriptionTemplateField> rxNewFields = new List <PrescriptionTemplateField>();
            List <PrescriptionTemplateField> rxModifiedFields = new List <PrescriptionTemplateField>();

            PrescriptionTemplateField field = null;
            bool optionFound = false;

            string fieldLabel;
            string fieldTypeCode;
            string fieldCode;
            int displayOrder;
            string fieldSource;

            foreach (var rxField in rxFields)
            {
                // Fieldabel, FieldTypeCode, DisplayOrder, FieldCode, FieldSource
                fieldLabel = (string)rxField[0];
                fieldTypeCode = (string)rxField[1];
                displayOrder = (int)rxField[2];
                fieldCode = (string)rxField[3];
                fieldSource = rxField[4] != null ? (string)rxField[4] : null;
                                    
                optionFound = false;
                foreach(var rxOption in rxOptions)
                {
                    if ((string)rxField[0] == rxOption.Key)
                    {
                        // section, rxNewFields, rxModifiedFields, 
                        // fieldLabel, fieldTypeCode, fieldCode, displayOrder, fieldSource, options

                        FindField(section, rxNewFields, rxModifiedFields, 
                                    fieldLabel, fieldTypeCode, fieldCode, displayOrder, null, 
                                                        (IEnumerable<string>)rxOption.Value);
                        optionFound = true;                                               
                    }
                }

                if (!optionFound)
                {
                    if (fieldSource == null)
                        FindField(section, rxNewFields, rxModifiedFields, 
                                    fieldLabel, fieldTypeCode, fieldCode, displayOrder, null, null);
                    else
                        FindField(section, rxNewFields, rxModifiedFields, 
                                    fieldLabel, fieldTypeCode, fieldCode, displayOrder, fieldSource, null);
                }
            }

            if (rxNewFields.Count > 0)
            {
                _templateDAL.AddFields(rxNewFields, conn, tran);
                foreach (var aField in rxNewFields)
                {
                    foreach (var anOption in aField.Options)
                    {
                        anOption.FieldId = aField.FieldId;
                    }

                    if (aField.Options.Count > 0)
                        _templateDAL.AddOptions(aField.Options, conn, tran);
                }
            }

            if (rxModifiedFields.Count > 0)
            {
                _templateDAL.UpdateFields(rxModifiedFields, conn, tran);
                foreach (var aField in rxModifiedFields)
                {
                    if (aField.Options.Count > 0)
                    {
                        _templateDAL.UpdateOptions(aField.Options, conn, tran);
                    }
                }
            }

        }

        private void ArchiveRxFields(PrescriptionTemplateSection section, List<List<object>> rxFields,
                                        SqlConnection conn, SqlTransaction tran)
        {
            List<PrescriptionTemplateField> fields = new();
            PrescriptionTemplateField f;

            bool isClinicEdit = GetClinicId().HasValue && GetClinicId().Value > 0;

            foreach (var rxField in rxFields)
            {
                f = Archive(section, (string)rxField[0]);

                if (f != null)
                {
                    f.TemplateId = section.TemplateId;
                    f.SectionId = section.SectionId;
                    f.IsCustomized = isClinicEdit;
                    fields.Add(f);
                }
            }

            if (fields.Count > 0)
            {
                _templateDAL.UpdateFields(fields, conn, tran);

                foreach (var field in fields)
                {
                    if (field.Options.Count > 0)
                    {
                        foreach (var option in field.Options)
                        {
                            option.IsCustomized = isClinicEdit;
                        }
                        
                        _templateDAL.UpdateOptions(field.Options, conn, tran);
                    }
                }
            }
        }

        private PrescriptionTemplateField Archive(PrescriptionTemplateSection section, string fieldLabel)
        {
            foreach (var field in section.Fields)
            {
                if (field.FieldLabel == fieldLabel)
                {
                    if (!field.IsArchived)
                        field.IsArchived = true;

                    foreach (var option in field.Options)
                    {
                        option.IsArchived = true;
                    }

                    return field;
                }
            }

            return null;
        }

        private void FindField(PrescriptionTemplateSection section,
                                        List<PrescriptionTemplateField> rxNewFields, 
                                        List<PrescriptionTemplateField> rxModifiedFields,
                                        string fieldLabel, string fieldTypeCode, string fieldCode, int displayOrder, string fieldSource,
                                        IEnumerable<string> options)
        {
            bool isClinicEdit = GetClinicId().HasValue && GetClinicId().Value > 0;

            foreach (var field in section.Fields)
            {
                if (field.FieldLabel == fieldLabel)
                {
                    field.IsArchived = false;
                    field.IsCustomized = isClinicEdit;

                    if (field.Options.Count > 0)
                    {
                        foreach (var option in field.Options)
                        {
                            option.IsArchived = false;
                            option.IsCustomized = isClinicEdit;
                        }
                    }

                    rxModifiedFields.Add(field);
                    return;
                }
            }

            var newField = new PrescriptionTemplateField(section.TemplateId, section.SectionId, ClinicId, 
                                                            fieldLabel, fieldTypeCode, displayOrder, fieldCode, fieldSource);

            if (options != null)
            {
                int optionDisplayOrder = 1;

                foreach (var option in options)
                {
                    newField.Options.Add(
                        new PrescriptionTemplateFieldOption(section.TemplateId, section.SectionId, 
                                                                newField.FieldId, ClinicId, option, optionDisplayOrder++)
                    );
                }
            }

            section.Fields.Add(newField);
            rxNewFields.Add(newField);

            return;            
        }

        public bool SaveFieldSource(int templateId, int fieldId, string fieldSource)
        {
            bool result;

            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                result = _templateDAL.SaveFieldSource(templateId, fieldId, GetClinicId(), fieldSource, conn, tran);
                tran.Commit();
                return result;
            }
            catch
            {
                tran.Rollback();
                throw;
            }  
        }

        public int SyncClinicTemplate(int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            return SyncTemplate(clinicId, null, null, conn, tran);
        }

        public int SyncClinicDepartmentTemplate(int clinicId, int departmentId, SqlConnection conn, SqlTransaction tran)
        {
            return SyncTemplate(clinicId, departmentId, null, conn, tran);
        }

        public int SyncClinicDoctorTemplate(int clinicId, int doctorId, SqlConnection conn, SqlTransaction tran)
        {
            return SyncTemplate(clinicId, null, doctorId, conn, tran);
        }


        private int SyncTemplate(int clinicId, int? departmentId, int? doctorId, SqlConnection conn, SqlTransaction tran)
        {
            if (clinicId <= 0)
                throw new ArgumentException("A valid ClinicId is required.", nameof(clinicId));

            PrescriptionTemplate source = null;
            PrescriptionTemplate target = null;

            if (doctorId.HasValue)
            {
                if (doctorId.Value <= 0)
                    throw new ArgumentException("A valid DoctorId is required.", nameof(doctorId.Value));

                // Clinic Department -> Clinic Doctor
                // Locate the clinic Department template.
                var doctor = _doctorDAL.Get(doctorId.Value, clinicId, conn, tran);
                if (doctor == null)
                {
                    throw new InvalidOperationException(
                        $"Doctor {doctorId.Value} was not found for clinic {clinicId}.");
                }

                var department = _departmentDAL.Get(doctor.DepartmentId, clinicId, conn, tran);
                if (department == null)
                {
                    throw new InvalidOperationException(
                        $"Department {doctor.DepartmentId} was not found for clinic {clinicId}.");
                }

                source = _templateDAL.GetByCode(department.DepartmentCode, clinicId, department.DepartmentId, null, conn, tran);

                if (source == null)
                    source = _templateDAL.GetByCode("DEFAULT", clinicId, null, null, conn, tran);

                if (source == null)
                    throw new InvalidOperationException("Unable to load the source prescription template.");

                target = _templateDAL.GetByDoctor(doctor.DoctorId, clinicId, conn, tran);

                if (target == null)
                {
                    // Build a separate target tree. Do not reuse source objects
                    target = new PrescriptionTemplate(source, clinicId, department.DepartmentId, doctor.DoctorId, 
                                                        doctor.DoctorId.ToString());
                    return Add(target, conn, tran);
                }
            }
            else if (departmentId.HasValue)
            {
                if (departmentId.Value <= 0)
                    throw new ArgumentException("A valid DepartmentId is required.", nameof(departmentId.Value));

                // Global Department -> Clinic Department
                // Locate the global Department template.
                var department = _departmentDAL.Get(departmentId.Value, clinicId, conn, tran);

                if (department == null)
                {
                    throw new InvalidOperationException(
                        $"Department {departmentId.Value} was not found for clinic {clinicId}.");
                }

                source = _templateDAL.GetByCode(department.DepartmentCode, null, null, null, conn, tran);

                if (source == null)
                    source = _templateDAL.GetByCode("DEFAULT", clinicId, null, null, conn, tran);

                if (source == null)
                    throw new InvalidOperationException("Unable to load the source prescription template.");

                target = _templateDAL.GetByDepartment(department.DepartmentId, clinicId, conn, tran);

                if (target == null)
                {
                    // Build a separate target tree. Do not reuse source objects
                    target = new PrescriptionTemplate(source, clinicId, department.DepartmentId, null, 
                                                        department.DepartmentCode);
                    return Add(target, conn, tran);
                }
            }          
            else
            {
                // Global Default -> Clinic Default
                // Locate the global DEFAULT template.
                source = _templateDAL.GetByCode("DEFAULT", null, null, null, conn, tran);
                if (source == null)
                    throw new InvalidOperationException("Unable to load the Global Default prescription template.");

                target = _templateDAL.GetByClinic(clinicId, conn, tran);

                if (target == null)
                {
                    // Build a separate target tree. Do not reuse source objects
                    target = new PrescriptionTemplate(source, clinicId, null, null, "DEFAULT");
                    return Add(target, conn, tran);
                }
            }

            // Retain the target TemplateId and synchronize its source reference.
            target.ClinicId = clinicId;
            target.SourceTemplateId = source.TemplateId;
            // target.TemplateCode = source.TemplateCode; -- retain existing TemplateCode of target template
            target.TemplateName = source.TemplateName;
            target.IsActive = source.IsActive;

            target.Sections ??= new List<PrescriptionTemplateSection>();

            foreach (var sourceSection in source.Sections)
            {
                var targetSection = target.Sections.FirstOrDefault(s => s.SourceSectionId == sourceSection.SectionId);

                if (targetSection == null)
                {
                    // This source section is new to the clinic template.
                    // Its descendants will be inserted by Update().
                    target.Sections.Add(new PrescriptionTemplateSection(sourceSection, clinicId));
                    continue;
                }

                // Update inherited properties only when the section is not customized.
                if (!targetSection.IsCustomized)
                {
                    targetSection.SectionName = sourceSection.SectionName;
                    targetSection.DisplayOrder = sourceSection.DisplayOrder;
                    targetSection.IsActive = sourceSection.IsActive;
                    targetSection.Multiline = sourceSection.Multiline;
                    targetSection.IsPharmacologyAdvice = sourceSection.IsPharmacologyAdvice;
                    targetSection.IsVital = sourceSection.IsVital;
                    targetSection.IsTestResult = sourceSection.IsTestResult;
                    targetSection.IsArchived = sourceSection.IsArchived;
                }

                // Keep the immediate source relationship even if customized.
                targetSection.SourceSectionId = sourceSection.SectionId;
                targetSection.TemplateId = target.TemplateId;
                targetSection.ClinicId = clinicId;
                targetSection.Fields ??= new List<PrescriptionTemplateField>();

                foreach (var sourceField in sourceSection.Fields)
                {
                    var targetField = targetSection.Fields.FirstOrDefault(
                        f => f.SourceFieldId == sourceField.FieldId);

                    if (targetField == null)
                    {
                        targetSection.Fields.Add(new PrescriptionTemplateField(sourceField, clinicId, targetSection.SectionId));
                        continue;
                    }

                    if (!targetField.IsCustomized)
                    {
                        targetField.FieldLabel = sourceField.FieldLabel;
                        targetField.FieldTypeCode = sourceField.FieldTypeCode;
                        targetField.FieldCode = sourceField.FieldCode;
                        targetField.FieldSource = sourceField.FieldSource;
                        targetField.DisplayOrder = sourceField.DisplayOrder;
                        targetField.IsActive = sourceField.IsActive;
                        targetField.IsArchived = sourceField.IsArchived;
                        targetField.MergeWith = sourceField.MergeWith;
                    }

                    targetField.SourceFieldId = sourceField.FieldId;
                    targetField.TemplateId = target.TemplateId;
                    targetField.SectionId = targetSection.SectionId;
                    targetField.ClinicId = clinicId;
                    targetField.Options ??= new List<PrescriptionTemplateFieldOption>();

                    foreach (var sourceOption in sourceField.Options)
                    {
                        var targetOption = targetField.Options.FirstOrDefault(
                            o => o.SourceOptionId == sourceOption.OptionId);

                        if (targetOption == null)
                        {
                            targetField.Options.Add(new PrescriptionTemplateFieldOption(sourceOption, clinicId,
                                                                            targetSection.SectionId, targetField.FieldId));
                            continue;
                        }

                        if (!targetOption.IsCustomized)
                        {
                            targetOption.OptionText = sourceOption.OptionText;
                            targetOption.DisplayOrder = sourceOption.DisplayOrder;
                            targetOption.IsActive = sourceOption.IsActive;
                            targetOption.IsArchived = sourceOption.IsArchived;
                        }

                        targetOption.SourceOptionId = sourceOption.OptionId;
                        targetOption.TemplateId = target.TemplateId;
                        targetOption.SectionId = targetSection.SectionId;
                        targetOption.FieldId = targetField.FieldId;
                        targetOption.ClinicId = clinicId;
                    }
                }
            }

            // Reuse the normal BLL persistence workflow. Existing rows are updated;
            // rows with zero IDs are inserted. Nothing is deleted.
            Update(target, conn, tran);

            return target.TemplateId;
        }

    }

}
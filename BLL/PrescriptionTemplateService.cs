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

        public PrescriptionTemplateService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(config, httpContextAccessor)
        {
            _templateDAL = new PrescriptionTemplateDAL(config);
            _dal = _templateDAL;
        }

        public override int Add(PrescriptionTemplate template, SqlConnection conn, SqlTransaction tran)
        {
            int templateId = 0;
            int sectionId = 0;
            int fieldId = 0;
            int optionId = 0;

            if (template.TemplateId > 0)
            {
                templateId = template.TemplateId;
                return templateId;
            }

            templateId = _templateDAL.Add(template, conn, tran);

            foreach (var section in template.Sections)
            {
                section.TemplateId = template.TemplateId;
                section.ClinicId = ClinicId;
            }

            sectionId = _templateDAL.AddSections(template.Sections, conn, tran);

            List<PrescriptionTemplateField> fields = new List<PrescriptionTemplateField>();

            foreach (var section in template.Sections)
            {
                foreach (var field in section.Fields)
                {
                    field.TemplateId = template.TemplateId;
                    field.SectionId = section.SectionId;
                    field.ClinicId = ClinicId;
                    
                    fields.Add(field);
                }
            }

            fieldId = _templateDAL.AddFields(fields, conn, tran);

            List<PrescriptionTemplateFieldOption> options = new List<PrescriptionTemplateFieldOption>();

            foreach (var section in template.Sections)
            {
                foreach (var field in section.Fields)
                {
                    foreach (var option in field.Options)
                    {
                        option.TemplateId = template.TemplateId;
                        option.SectionId = section.SectionId;
                        option.FieldId = field.FieldId;
                        option.ClinicId = ClinicId;

                        options.Add(option);
                    }
                }
            }

            optionId = _templateDAL.AddOptions(options, conn, tran);

            return templateId;
        }

        public override bool Update(PrescriptionTemplate template, SqlConnection conn, SqlTransaction tran)
        {
            bool templatesAffected = false;
            bool sectionsAffected = false;
            bool fieldsAffected = false;
            bool optionsAffected = false;

            if (template.TemplateId > 0)
            {
                templatesAffected = _templateDAL.Update(template, conn, tran);
            }

            List<PrescriptionTemplateSection> newSections = new List<PrescriptionTemplateSection>();
            List<PrescriptionTemplateSection> modifiedSections = new List<PrescriptionTemplateSection>();

            foreach (var section in template.Sections)
            {
                section.TemplateId = template.TemplateId;
                section.ClinicId = ClinicId;

                if (section.SectionId > 0)
                    modifiedSections.Add(section);
                else
                    newSections.Add(section);
            }

            if (newSections.Count > 0)
                _templateDAL.AddSections(newSections, conn, tran);

            if (modifiedSections.Count > 0)
                sectionsAffected = _templateDAL.UpdateSections(modifiedSections, conn, tran);;

            List<PrescriptionTemplateField> newFields = new List<PrescriptionTemplateField>();
            List<PrescriptionTemplateField> modifiedFields = new List<PrescriptionTemplateField>();

            foreach (var section in template.Sections)
            {
                foreach (var field in section.Fields)
                {
                    field.TemplateId = template.TemplateId;
                    field.SectionId = section.SectionId;
                    field.ClinicId = ClinicId;

                    if (field.FieldId > 0)
                        modifiedFields.Add(field);
                    else
                        newFields.Add(field);
                }
            }

            if (newFields.Count > 0)
                _templateDAL.AddFields(newFields, conn, tran);

            if (modifiedFields.Count > 0)
                fieldsAffected = _templateDAL.UpdateFields(modifiedFields, conn, tran);

            List<PrescriptionTemplateFieldOption> newOptions = new List<PrescriptionTemplateFieldOption>();
            List<PrescriptionTemplateFieldOption> modifiedOptions = new List<PrescriptionTemplateFieldOption>();

            foreach (var section in template.Sections)
            {
                foreach (var field in section.Fields)
                {
                    foreach (var option in field.Options)
                    {
                        option.TemplateId = template.TemplateId;
                        option.SectionId = section.SectionId;
                        option.FieldId = field.FieldId;
                        option.ClinicId = ClinicId;

                        if (option.OptionId > 0)
                            modifiedOptions.Add(option);
                        else
                            newOptions.Add(option);
                    }
                }
            }

            if (newOptions.Count > 0)
                _templateDAL.AddOptions(newOptions, conn, tran);

            if (modifiedOptions.Count > 0)
                optionsAffected = _templateDAL.UpdateOptions(modifiedOptions, conn, tran);

            return templatesAffected || sectionsAffected || fieldsAffected || optionsAffected;
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
                _templateDAL.SaveSectionOrder(sections, conn, tran);
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
                _templateDAL.SaveFieldOrder(fields, conn, tran);
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
                _templateDAL.SaveOptionOrder(options, conn, tran);
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
                bool rowsAffected = _templateDAL.ToggleSection(templateId, sectionId, conn, tran);
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
                bool rowsAffected = _templateDAL.ToggleField(templateId, fieldId, conn, tran);
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
                bool rowsAffected = _templateDAL.ToggleOption(templateId, optionId, conn, tran);
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
                    newSectionId = _templateDAL.InsertSectionBefore(templateId, sectionId, ClinicId, 
                                                                    sectionName, currDisplayOrder, conn, tran);
                else
                    newSectionId = _templateDAL.InsertSectionAfter(templateId, sectionId, ClinicId, 
                                                                    sectionName, currDisplayOrder, conn, tran);

                _templateDAL.AddField(templateId, newSectionId, ClinicId, "New Field", "Text", conn, tran);

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
                    newFieldId = _templateDAL.InsertFieldBefore(templateId, sectionId, fieldId, ClinicId, 
                                                                    fieldName, currOrder, conn, tran);
                else
                    newFieldId = _templateDAL.InsertFieldAfter(templateId, sectionId, fieldId, ClinicId, 
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
                    newOptionId = _templateDAL.InsertOptionBefore(templateId, option.SectionId, fieldId, optionId, ClinicId,
                                                                    optionName, currOrder, conn, tran);
                else
                    newOptionId = _templateDAL.InsertOptionAfter(templateId, option.SectionId, fieldId, optionId, ClinicId,
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
                _templateDAL.SaveSectionName(templateId, sectionId, name, conn, tran);

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
                _templateDAL.SaveFieldName(templateId, fieldId, name, conn, tran);

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
                _templateDAL.SaveOptionName(templateId, optionId, name, conn, tran);

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
                    _templateDAL.DeleteOptions(templateId, fieldId, conn, tran);
                }

                _templateDAL.UpdateFieldType(templateId, fieldId, fieldTypeCode, conn, tran);

                // If switching FROM non-options-type TO options-type
                // then add one options line
                if (!currentSupportsOptions && newSupportsOptions)
                {
                    _templateDAL.AddOption(templateId, sectionId, ClinicId, fieldId, "New", true, 1, conn, tran);
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

        public bool SaveSectionOptions(int templateId, int sectionId, bool isMultiline, bool isRx)
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

                _templateDAL.SaveSectionOptions(templateId, sectionId, isMultiline, isRx, conn, tran);

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
                {
                    bool isArchived = true;
                    _templateDAL.UpdateFieldsArchival(templateId, sectionId, isArchived, conn, tran);
                    _templateDAL.UpdateOptionsArchival(templateId, sectionId, isArchived, conn, tran);

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
                else
                {
                    bool isArchived = false;
                    _templateDAL.UpdateFieldsArchival(templateId, sectionId, isArchived, conn, tran);
                    _templateDAL.UpdateOptionsArchival(templateId, sectionId, isArchived, conn, tran);

                    List <PrescriptionTemplateField> fields = new List <PrescriptionTemplateField>();

                    foreach (var rxField in rxFields)
                    {
                        fields.Add(Archive(section, (string)rxField[0]));
                    }

                    _templateDAL.UpdateFields(fields, conn, tran);
                    foreach (var field in fields)
                    {
                        _templateDAL.UpdateOptions(field.Options, conn, tran);
                    }
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
            foreach (var field in section.Fields)
            {
                if (field.FieldLabel == fieldLabel)
                {
                    field.IsArchived = false;

                    if (field.Options.Count > 0)
                    {
                        foreach (var option in field.Options)
                        {
                            option.IsArchived = false;
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
                result = _templateDAL.SaveFieldSource(templateId, fieldId, fieldSource, conn, tran);
                tran.Commit();
                return result;
            }
            catch
            {
                tran.Rollback();
                throw;
            }  
        }
    }

}
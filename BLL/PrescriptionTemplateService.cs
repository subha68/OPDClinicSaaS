using Models;
using ViewModels;
using DAL;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
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

        public PrescriptionTemplate GetByDepartment(int departmentId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _templateDAL.GetByDepartment(departmentId, conn);
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
                return _templateDAL.GetByDoctor(doctorId, conn);
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
                _templateDAL.SaveSectionOptions(templateId, sectionId, isMultiline, isRx, conn, tran);

                if (isRx)
                {
                    bool isArchived = true;
                    _templateDAL.UpdateFieldsArchival(templateId, sectionId, isArchived, conn, tran);
                    _templateDAL.UpdateOptionsArchival(templateId, sectionId, isArchived, conn, tran);
                }
                else
                {
                    bool isArchived = false;
                    _templateDAL.UpdateFieldsArchival(templateId, sectionId, isArchived, conn, tran);
                    _templateDAL.UpdateOptionsArchival(templateId, sectionId, isArchived, conn, tran);
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
   

    }

}
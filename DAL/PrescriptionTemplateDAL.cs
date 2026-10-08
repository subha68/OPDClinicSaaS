using Microsoft.Data.SqlClient;
using Models;
using ViewModels;

namespace DAL
{
    public class PrescriptionTemplateDAL : BaseDAL, IDAL<PrescriptionTemplate>
    {
        private readonly DbHelper _db;

        public PrescriptionTemplateDAL(IConfiguration configuration)
        {
            _db = new DbHelper(configuration);
        }

        public List<PrescriptionTemplate> GetAll(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            // only template is retrieved

            List<PrescriptionTemplate> templates = new List<PrescriptionTemplate>();

            string templateQuery = @"SELECT t.*, d.Name 
                                    FROM PrescriptionTemplates t
                                    INNER JOIN Departments d
                                        ON t.DepartmentId = d.DepartmentId";
            using SqlCommand templateCmd = new SqlCommand(templateQuery, conn);
            if (tran != null)
            {
                templateCmd.Transaction = tran;
            }

            using var templateReader = templateCmd.ExecuteReader();

            if (templateReader.Read())
            {
                templates.Add(new PrescriptionTemplate
                {
                    TemplateId = (int)templateReader["TemplateId"],
                    ClinicId = (int)templateReader["ClinicId"],
                    DepartmentId = (int)templateReader["DepartmentId"],
                    DepartmentName = templateReader["Name"].ToString(),
                    TemplateName = templateReader["TemplateName"].ToString(),
                    IsActive = (bool)templateReader["IsActive"],
                });
            }

            return templates;
        }

        public PrescriptionTemplate Get(int templateId, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            PrescriptionTemplate template = null;

            // retrieve template

            string templateQuery = @"SELECT t.*, d.Name AS DepartmentName 
                                    FROM PrescriptionTemplates t
                                    LEFT JOIN Departments d
                                        ON t.DepartmentId = d.DepartmentId
                                    WHERE TemplateId = @TemplateId";
            if (clinicId == 0)
                templateQuery += " AND t.ClinicId IS NULL";
            else
                templateQuery += " AND t.ClinicId = @ClinicId";

            using SqlCommand templateCmd = new SqlCommand(templateQuery, conn);
            if (tran != null)
            {
                templateCmd.Transaction = tran;
            }

            templateCmd.Parameters.AddWithValue("@TemplateId", templateId);
            if (clinicId != 0)
                templateCmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using (var templateReader = templateCmd.ExecuteReader())
            {
                if (templateReader.Read())
                {
                    template = new PrescriptionTemplate
                    {
                        TemplateId = (int)templateReader["TemplateId"],
                        ClinicId = templateReader["ClinicId"] != DBNull.Value ? (int)templateReader["ClinicId"] : 0,
                        DepartmentId = templateReader["DepartmentId"] != DBNull.Value ? (int)templateReader["DepartmentId"] : 0,
                        DepartmentName = templateReader["DepartmentName"] != DBNull.Value 
                                                                                ? templateReader["DepartmentName"].ToString() 
                                                                                : null,
                        TemplateCode = templateReader["TemplateCode"].ToString(),
                        TemplateName = templateReader["TemplateName"].ToString(),
                        SourceTemplateId = templateReader["SourceTemplateId"] != DBNull.Value 
                                                                                ? (int)templateReader["SourceTemplateId"] 
                                                                                : 0,
                        IsActive = (bool)templateReader["IsActive"]
                    };
                }
            }

            if (template == null)
                return null; // Not found

            // Now retrieve sections & fields
            template.Sections = GetSectionsAndFields(template.TemplateId, conn, tran);

            return template;
        }

        public PrescriptionTemplate GetByClinic(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            PrescriptionTemplate template = null;

            // retrieve template

            string templateQuery = @"SELECT t.* 
                                    FROM PrescriptionTemplates t 
                                    WHERE t.ClinicId = @ClinicId
                                    AND (t.DepartmentId IS NULL OR t.DepartmentId = 0)
                                    AND (t.DoctorId IS NULL OR t.DoctorId = 0)";

            using SqlCommand templateCmd = new SqlCommand(templateQuery, conn);
            if (tran != null)
            {
                templateCmd.Transaction = tran;
            }

            templateCmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using (var templateReader = templateCmd.ExecuteReader())
            {

                if (templateReader.Read())
                {
                    template = new PrescriptionTemplate
                    {
                        TemplateId = (int)templateReader["TemplateId"],
                        ClinicId = templateReader["ClinicId"] != DBNull.Value ? (int)templateReader["ClinicId"] : 0,
                        DepartmentId = templateReader["DepartmentId"] != DBNull.Value ? (int)templateReader["DepartmentId"] : 0,
                        TemplateCode = templateReader["TemplateCode"].ToString(),
                        TemplateName = templateReader["TemplateName"].ToString(),
                        SourceTemplateId = templateReader["SourceTemplateId"] != DBNull.Value 
                                                                                ? (int)templateReader["SourceTemplateId"] 
                                                                                : 0,
                        IsActive = (bool)templateReader["IsActive"]
                    };
                }
            }

            if (template == null)
                return null; // Not found

            // Now retrieve sections & fields
            template.Sections = GetSectionsAndFields(template.TemplateId, conn, tran);

            return template;
        }

        public PrescriptionTemplate GetByDepartment(int departmentId, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            PrescriptionTemplate template = null;

            // retrieve template

            string templateQuery = @"SELECT t.*, d.Name as DepartmentName 
                                    FROM PrescriptionTemplates t
                                    INNER JOIN Departments d
                                        ON t.DepartmentId = d.DepartmentId 
                                    WHERE t.DepartmentId = @DepartmentId
                                    AND (t.DoctorId IS NULL OR t.DoctorId = 0)";
            if (clinicId == 0)
                templateQuery += " AND t.ClinicId IS NULL";
            else
                templateQuery += " AND t.ClinicId = @ClinicId";

            using SqlCommand templateCmd = new SqlCommand(templateQuery, conn);
            if (tran != null)
            {
                templateCmd.Transaction = tran;
            }

            templateCmd.Parameters.AddWithValue("@DepartmentId", departmentId);
            if (clinicId != 0)
                templateCmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using (var templateReader = templateCmd.ExecuteReader())
            {

                if (templateReader.Read())
                {
                    template = new PrescriptionTemplate
                    {
                        TemplateId = (int)templateReader["TemplateId"],
                        ClinicId = templateReader["ClinicId"] != DBNull.Value ? (int)templateReader["ClinicId"] : 0,
                        DepartmentId = templateReader["DepartmentId"] != DBNull.Value ? (int)templateReader["DepartmentId"] : 0,
                        DepartmentName = templateReader["DepartmentName"] != DBNull.Value 
                                                                                ? templateReader["DepartmentName"].ToString() 
                                                                                : null,
                        TemplateCode = templateReader["TemplateCode"].ToString(),
                        TemplateName = templateReader["TemplateName"].ToString(),
                        SourceTemplateId = templateReader["SourceTemplateId"] != DBNull.Value 
                                                                                ? (int)templateReader["SourceTemplateId"] 
                                                                                : 0,
                        IsActive = (bool)templateReader["IsActive"]
                    };
                }
            }

            if (template == null)
                return null; // Not found

            // Now retrieve sections & fields
            template.Sections = GetSectionsAndFields(template.TemplateId, conn, tran);

            return template;
        }

        public PrescriptionTemplate GetByDoctor(int doctorId, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            PrescriptionTemplate template = null;

            // retrieve template

            string templateQuery = @"SELECT t.*, d.Name 
                                    FROM PrescriptionTemplates t
                                    INNER JOIN Departments d
                                        ON t.DepartmentId = d.DepartmentId 
                                    WHERE t.DoctorId = @DoctorId
                                    AND t.ClinicId = @ClinicId";

            using SqlCommand templateCmd = new SqlCommand(templateQuery, conn);
            if (tran != null)
            {
                templateCmd.Transaction = tran;
            }

            templateCmd.Parameters.AddWithValue("@DoctorId", doctorId);
            templateCmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using (var templateReader = templateCmd.ExecuteReader())
            {

                if (templateReader.Read())
                {
                    template = new PrescriptionTemplate
                    {
                        TemplateId = (int)templateReader["TemplateId"],
                        ClinicId = (int)templateReader["ClinicId"],
                        DepartmentId = (int)templateReader["DepartmentId"],
                        DepartmentName = templateReader["Name"].ToString(),
                        TemplateName = templateReader["TemplateName"].ToString(),
                        IsActive = (bool)templateReader["IsActive"]
                    };
                }
            }

            if (template == null)
                return null; // Not found

            // Now retrieve sections & fields
            template.Sections = GetSectionsAndFields(template.TemplateId, conn, tran);

            return template;
        }

        public PrescriptionTemplate GetByCode(string templateCode, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            PrescriptionTemplate template = null;

            // retrieve template

            string templateQuery = @"SELECT t.* 
                                    FROM PrescriptionTemplates t
                                    WHERE TemplateCode = @TemplateCode";
            if (clinicId == 0)
                templateQuery += " AND ClinicId IS NULL;";
            else
                templateQuery += " AND ClinicId = @ClinicId;";

            using SqlCommand templateCmd = new SqlCommand(templateQuery, conn);
            if (tran != null)
            {
                templateCmd.Transaction = tran;
            }

            templateCmd.Parameters.AddWithValue("@TemplateCode", templateCode);
            if (clinicId != 0)
                templateCmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using (var templateReader = templateCmd.ExecuteReader())
            {
                if (templateReader.Read())
                {
                    template = new PrescriptionTemplate
                    {
                        TemplateId = (int)templateReader["TemplateId"],
                        TemplateName = templateReader["TemplateName"].ToString(),
                        TemplateCode = templateReader["TemplateCode"].ToString(),
                        IsActive = (bool)templateReader["IsActive"]
                    };
                }
            }

            if (template == null)
                return null; // Not found

            // Now retrieve sections & fields
            template.Sections = GetSectionsAndFields(template.TemplateId, conn, tran);

            return template;
        }

        public List<PrescriptionTemplateSection> GetSectionsAndFields(int templateId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<PrescriptionTemplateSection> sections = GetSections(templateId, conn, tran);

            List<PrescriptionTemplateField> fields = GetFieldsForTemplate(templateId, conn, tran);

            List<PrescriptionTemplateFieldOption> options = GetOptionsForTemplate(templateId, conn, tran);

            // arrange the field options
            foreach (var field in fields)
            {
                foreach (var option in options)
                {
                    if (field.FieldId == option.FieldId)
                    {
                        var matchedOption = option;
                        field.Options.Add(matchedOption);
                    }    
                }
            }

            // arrange the fields
            foreach (var section in sections)
            {
                foreach (var field in fields)
                {
                    if (section.SectionId == field.SectionId)
                    {
                        var matchedField = field;
                        section.Fields.Add(matchedField);
                    }    
                }
            }

            return sections;
        }

        public PrescriptionTemplateSection GetSectionAndAllFields(int templateId, int sectionId, SqlConnection conn, SqlTransaction? tran = null)
        {
            PrescriptionTemplateSection section = GetSection(templateId, sectionId, conn, tran);

            List<PrescriptionTemplateField> fields = GetAllFieldsForTemplate(templateId, conn, tran);

            List<PrescriptionTemplateFieldOption> options = GetAllOptionsForTemplate(templateId, conn, tran);

            // arrange the field options
            foreach (var field in fields)
            {
                foreach (var option in options)
                {
                    if (field.FieldId == option.FieldId)
                    {
                        var matchedOption = option;
                        field.Options.Add(matchedOption);
                    }    
                }
            }

            // arrange the fields
            foreach (var field in fields)
            {
                if (section.SectionId == field.SectionId)
                {
                    var matchedField = field;
                    section.Fields.Add(matchedField);
                }    
            }

            return section;
        }

        public PrescriptionTemplateSection GetSectionAndFields(int templateId, int sectionId, SqlConnection conn, SqlTransaction? tran = null)
        {
            PrescriptionTemplateSection section = GetSection(templateId, sectionId, conn, tran);

            List<PrescriptionTemplateField> fields = GetFieldsForTemplate(templateId, conn, tran);

            List<PrescriptionTemplateFieldOption> options = GetOptionsForTemplate(templateId, conn, tran);

            // arrange the field options
            foreach (var field in fields)
            {
                foreach (var option in options)
                {
                    if (field.FieldId == option.FieldId)
                    {
                        var matchedOption = option;
                        field.Options.Add(matchedOption);
                    }    
                }
            }

            // arrange the fields
            foreach (var field in fields)
            {
                if (section.SectionId == field.SectionId)
                {
                    var matchedField = field;
                    section.Fields.Add(matchedField);
                }    
            }

            return section;
        }

        public List<PrescriptionTemplateSection> GetSections(int templateId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<PrescriptionTemplateSection> sections = new List<PrescriptionTemplateSection>();

            string sectionQuery = @"SELECT * FROM PrescriptionTemplateSections 
                                        WHERE TemplateId = @TemplateId
                                        ORDER BY DisplayOrder";
            using SqlCommand sectionCmd = new SqlCommand(sectionQuery, conn);
            if (tran != null)
            {
                sectionCmd.Transaction = tran;
            }

            sectionCmd.Parameters.AddWithValue("@TemplateId", templateId);

            using (var sectionReader = sectionCmd.ExecuteReader())
            {
                while (sectionReader.Read())
                {
                    sections.Add(new PrescriptionTemplateSection
                    {
                        TemplateId = (int)sectionReader["TemplateId"],
                        ClinicId = sectionReader["ClinicId"] != DBNull.Value ? (int)sectionReader["ClinicId"] : 0,
                        SectionId = (int)sectionReader["SectionId"],
                        SectionName = sectionReader["SectionName"].ToString(),
                        DisplayOrder = (int)sectionReader["DisplayOrder"],
                        IsActive = (bool)sectionReader["IsActive"],
                        Multiline = (bool)sectionReader["Multiline"],
                        IsPharmacologyAdvice = (bool)sectionReader["IsPharmacologyAdvice"],
                        IsVital = (bool)sectionReader["IsVital"],
                        IsTestResult = (bool)sectionReader["IsTestResult"]
                    });
                }
            }

            return sections;
        }

        public List<PrescriptionTemplateField> GetAllFieldsForTemplate(int templateId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<PrescriptionTemplateField> fields = new List<PrescriptionTemplateField>();

            string fieldQuery = @"SELECT * FROM PrescriptionTemplateFields 
                                        WHERE TemplateId = @TemplateId
                                        ORDER BY DisplayOrder";
            using SqlCommand fieldCmd = new SqlCommand(fieldQuery, conn);
            if (tran != null)
            {
                fieldCmd.Transaction = tran;
            }

            fieldCmd.Parameters.AddWithValue("@TemplateId", templateId);

            using (var fieldReader = fieldCmd.ExecuteReader())
            {

                while (fieldReader.Read())
                {
                    fields.Add(new PrescriptionTemplateField
                    {
                        TemplateId = (int)fieldReader["TemplateId"],
                        ClinicId = fieldReader["ClinicId"] != DBNull.Value ? (int)fieldReader["ClinicId"] : 0,
                        SectionId = (int)fieldReader["SectionId"],
                        FieldId = (int)fieldReader["FieldId"],
                        FieldLabel = fieldReader["FieldLabel"].ToString(),
                        FieldTypeCode = fieldReader["FieldTypeCode"].ToString(),
                        FieldCode = fieldReader["FieldCode"] != null ? fieldReader["FieldCode"].ToString() : null,
                        FieldSource = fieldReader["FieldSource"] != null ? fieldReader["FieldSource"].ToString() : null,
                        IsActive = (bool)fieldReader["IsActive"],
                        DisplayOrder = (int)fieldReader["DisplayOrder"],
                        IsArchived = (bool)fieldReader["IsArchived"]
                    });

                }
            }

            return fields;
        }

        public List<PrescriptionTemplateField> GetFieldsForTemplate(int templateId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<PrescriptionTemplateField> fields = new List<PrescriptionTemplateField>();

            string fieldQuery = @"SELECT * FROM PrescriptionTemplateFields 
                                        WHERE TemplateId = @TemplateId
                                        AND IsArchived = 0
                                        ORDER BY DisplayOrder";
            using SqlCommand fieldCmd = new SqlCommand(fieldQuery, conn);
            if (tran != null)
            {
                fieldCmd.Transaction = tran;
            }

            fieldCmd.Parameters.AddWithValue("@TemplateId", templateId);

            using (var fieldReader = fieldCmd.ExecuteReader())
            {

                while (fieldReader.Read())
                {
                    fields.Add(new PrescriptionTemplateField
                    {
                        TemplateId = (int)fieldReader["TemplateId"],
                        ClinicId = fieldReader["ClinicId"] != DBNull.Value ? (int)fieldReader["ClinicId"] : 0,
                        SectionId = (int)fieldReader["SectionId"],
                        FieldId = (int)fieldReader["FieldId"],
                        FieldLabel = fieldReader["FieldLabel"].ToString(),
                        FieldTypeCode = fieldReader["FieldTypeCode"].ToString(),
                        FieldCode = fieldReader["FieldCode"] != null ? fieldReader["FieldCode"].ToString() : null,
                        IsActive = (bool)fieldReader["IsActive"],
                        DisplayOrder = (int)fieldReader["DisplayOrder"],
                        IsArchived = (bool)fieldReader["IsArchived"],
                        FieldSource = fieldReader["FieldSource"] != null ? fieldReader["FieldSource"].ToString() : null
                    });

                }
            }

            return fields;
        }

        public List<PrescriptionTemplateField> GetFieldsForSection(int sectionId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<PrescriptionTemplateField> fields = new List<PrescriptionTemplateField>();

            string fieldQuery = @"SELECT * FROM PrescriptionTemplateFields 
                                    WHERE SectionId = @SectionId
                                    AND IsArchived = 0
                                    ORDER BY DisplayOrder";
            using SqlCommand fieldCmd = new SqlCommand(fieldQuery, conn);
            if (tran != null)
            {
                fieldCmd.Transaction = tran;
            }

            fieldCmd.Parameters.AddWithValue("@SectionId", sectionId);

            using (var fieldReader = fieldCmd.ExecuteReader())
            {

                while (fieldReader.Read())
                {
                    fields.Add(new PrescriptionTemplateField
                    {
                        TemplateId = (int)fieldReader["TemplateId"],
                        ClinicId = (int)fieldReader["ClinicId"],
                        SectionId = (int)fieldReader["SectionId"],
                        FieldId = (int)fieldReader["FieldId"],
                        FieldLabel = fieldReader["FieldLabel"].ToString(),
                        FieldTypeCode = fieldReader["FieldTypeCode"].ToString(),
                        FieldCode = fieldReader["FieldCode"] != null ? fieldReader["FieldCode"].ToString() : null,
                        IsActive = (bool)fieldReader["IsActive"],
                        DisplayOrder = (int)fieldReader["DisplayOrder"],
                        IsArchived = (bool)fieldReader["IsArchived"],
                        FieldSource = fieldReader["FieldSource"] != null ? fieldReader["FieldSource"].ToString() : null
                    });

                }
            }

            return fields;
        }

        public List<PrescriptionTemplateFieldOption> GetAllOptionsForTemplate(int templateId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<PrescriptionTemplateFieldOption> options = new List<PrescriptionTemplateFieldOption>();

            string optionQuery = @"SELECT * FROM PrescriptionTemplateFieldOptions 
                                        WHERE TemplateId = @TemplateId
                                        ORDER BY DisplayOrder";
            using SqlCommand optionCmd = new SqlCommand(optionQuery, conn);
            if (tran != null)
            {
                optionCmd.Transaction = tran;
            }

            optionCmd.Parameters.AddWithValue("@TemplateId", templateId);

            using (var optionReader = optionCmd.ExecuteReader())
            {

                while (optionReader.Read())
                {
                    options.Add(new PrescriptionTemplateFieldOption
                    {
                        TemplateId = (int)optionReader["TemplateId"],
                        ClinicId = optionReader["ClinicId"] != DBNull.Value ? (int)optionReader["ClinicId"] : 0,
                        SectionId = (int)optionReader["SectionId"],
                        FieldId = (int)optionReader["FieldId"],
                        OptionId = (int)optionReader["OptionId"],
                        OptionText = optionReader["OptionText"].ToString(),
                        IsActive = (bool)optionReader["IsActive"],
                        IsArchived = (bool)optionReader["IsArchived"],
                        DisplayOrder = (int)optionReader["DisplayOrder"]
                    });

                }
            }

            return options;            
        }

        public List<PrescriptionTemplateFieldOption> GetOptionsForTemplate(int templateId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<PrescriptionTemplateFieldOption> options = new List<PrescriptionTemplateFieldOption>();

            string optionQuery = @"SELECT * FROM PrescriptionTemplateFieldOptions 
                                        WHERE TemplateId = @TemplateId
                                        AND IsArchived = 0
                                        ORDER BY DisplayOrder";
            using SqlCommand optionCmd = new SqlCommand(optionQuery, conn);
            if (tran != null)
            {
                optionCmd.Transaction = tran;
            }

            optionCmd.Parameters.AddWithValue("@TemplateId", templateId);

            using (var optionReader = optionCmd.ExecuteReader())
            {

                while (optionReader.Read())
                {
                    options.Add(new PrescriptionTemplateFieldOption
                    {
                        TemplateId = (int)optionReader["TemplateId"],
                        ClinicId = optionReader["ClinicId"] != DBNull.Value ? (int)optionReader["ClinicId"] : 0,
                        SectionId = (int)optionReader["SectionId"],
                        FieldId = (int)optionReader["FieldId"],
                        OptionId = (int)optionReader["OptionId"],
                        OptionText = optionReader["OptionText"].ToString(),
                        IsActive = (bool)optionReader["IsActive"],
                        IsArchived = (bool)optionReader["IsArchived"],
                        DisplayOrder = (int)optionReader["DisplayOrder"]

                    });

                }
            }

            return options;            
        }

        public List<PrescriptionTemplateFieldOption> GetOptionsForField(int fieldId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<PrescriptionTemplateFieldOption> options = new List<PrescriptionTemplateFieldOption>();

            string optionQuery = @"SELECT * FROM PrescriptionTemplateFieldOptions 
                                        WHERE FieldId = @FieldId
                                        AND IsArchived = 0
                                        ORDER BY DisplayOrder";
            using SqlCommand optionCmd = new SqlCommand(optionQuery, conn);
            if (tran != null)
            {
                optionCmd.Transaction = tran;
            }

            optionCmd.Parameters.AddWithValue("@FieldId", fieldId);

            using (var optionReader = optionCmd.ExecuteReader())
            {

                while (optionReader.Read())
                {
                    options.Add(new PrescriptionTemplateFieldOption
                    {
                        TemplateId = (int)optionReader["TemplateId"],
                        ClinicId = (int)optionReader["ClinicId"],
                        SectionId = (int)optionReader["SectionId"],
                        FieldId = (int)optionReader["FieldId"],
                        OptionText = optionReader["OptionText"].ToString(),
                        IsActive = (bool)optionReader["IsActive"],
                        IsArchived = (bool)optionReader["IsArchived"],
                        DisplayOrder = (int)optionReader["DisplayOrder"]

                    });

                }
            }

            return options;            
        }

        public int Add(PrescriptionTemplate template, SqlConnection conn, SqlTransaction tran)
        {
            string templateQuery = @"INSERT INTO PrescriptionTemplates
                             (ClinicId, DepartmentId, DoctorId, TemplateCode, TemplateName, SourceTemplateId, IsActive) 
                             VALUES 
                             (@ClinicId, @DepartmentId, @DoctorId, @TemplateCode, @TemplateName, @SourceTemplateId, @IsActive);
                                SELECT SCOPE_IDENTITY();";
            using SqlCommand templateCmd = new SqlCommand(templateQuery, conn, tran);

            templateCmd.Parameters.AddWithValue("@ClinicId", template.ClinicId);
            templateCmd.Parameters.AddWithValue("@DepartmentId", template.DepartmentId.HasValue ? template.DepartmentId.Value : DBNull.Value);
            templateCmd.Parameters.AddWithValue("@DoctorId", template.DoctorId.HasValue ? template.DoctorId.Value : DBNull.Value);
            templateCmd.Parameters.AddWithValue("@TemplateCode", template.TemplateCode);
            templateCmd.Parameters.AddWithValue("@TemplateName", template.TemplateName);
            templateCmd.Parameters.AddWithValue("@SourceTemplateId", template.SourceTemplateId.HasValue 
                                                                                            ? template.SourceTemplateId.Value
                                                                                                : DBNull.Value);
            templateCmd.Parameters.AddWithValue("@IsActive", template.IsActive);

            int templateId = Convert.ToInt32(templateCmd.ExecuteScalar());

            template.TemplateId = templateId;

            return templateId;
        }

        public int AddSections(List<PrescriptionTemplateSection> sections, SqlConnection conn, SqlTransaction tran)
        {
            int sectionId = 0;

            foreach (var section in sections)
            {
                string sectionQuery = @"INSERT INTO PrescriptionTemplateSections
                             (TemplateId, ClinicId, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice,
                             IsVital, IsTestResult) 
                             VALUES 
                             (@TemplateId, @ClinicId, @SectionName, @DisplayOrder, @IsActive, @Multiline, @IsPharmacologyAdvice,
                             @IsVital, @IsTestResult);
                                SELECT SCOPE_IDENTITY();";
                using SqlCommand sectionCmd = new SqlCommand(sectionQuery, conn, tran);

                sectionCmd.Parameters.AddWithValue("@TemplateId", section.TemplateId);
                sectionCmd.Parameters.AddWithValue("@ClinicId", section.ClinicId);
                sectionCmd.Parameters.AddWithValue("@SectionName", section.SectionName);
                sectionCmd.Parameters.AddWithValue("@DisplayOrder", section.DisplayOrder);
                sectionCmd.Parameters.AddWithValue("@IsActive", section.IsActive ? 1 : 0);
                sectionCmd.Parameters.AddWithValue("@Multiline", section.Multiline ? 1 : 0);
                sectionCmd.Parameters.AddWithValue("@IsPharmacologyAdvice", section.IsPharmacologyAdvice ? 1 : 0);
                sectionCmd.Parameters.AddWithValue("@IsVital", section.IsVital ? 1 : 0);
                sectionCmd.Parameters.AddWithValue("@IsTestResult", section.IsTestResult ? 1 : 0);

                sectionId = Convert.ToInt32(sectionCmd.ExecuteScalar());

                section.SectionId = sectionId;
            }

            return sectionId;
        }

        public int AddFields(List<PrescriptionTemplateField> fields, SqlConnection conn, SqlTransaction tran)
        {
            int fieldId = 0;

            foreach (var field in fields)
            {
                string fieldQuery = @"INSERT INTO PrescriptionTemplateFields
                            (TemplateId, SectionId, ClinicId, FieldLabel, FieldTypeCode, FieldCode, IsActive, 
                                DisplayOrder, IsArchived, FieldSource) 
                            VALUES 
                            (@TemplateId, @SectionId, @ClinicId, @FieldLabel, @FieldTypeCode, @FieldCode, @IsActive, 
                                @DisplayOrder, 0, @FieldSource);
                                SELECT SCOPE_IDENTITY();";
                using SqlCommand fieldCmd = new SqlCommand(fieldQuery, conn, tran);

                fieldCmd.Parameters.AddWithValue("@TemplateId", field.TemplateId);
                fieldCmd.Parameters.AddWithValue("@SectionId", field.SectionId);
                fieldCmd.Parameters.AddWithValue("@ClinicId", field.ClinicId);
                fieldCmd.Parameters.AddWithValue("@FieldLabel", field.FieldLabel);
                fieldCmd.Parameters.AddWithValue("@FieldTypeCode", field.FieldTypeCode);
                fieldCmd.Parameters.AddWithValue("@FieldCode", field.FieldCode == null ? DBNull.Value : field.FieldCode);
                fieldCmd.Parameters.AddWithValue("@IsActive", field.IsActive);
                fieldCmd.Parameters.AddWithValue("@DisplayOrder", field.DisplayOrder);
                fieldCmd.Parameters.AddWithValue("@FieldSource", field.FieldSource == null ? DBNull.Value : field.FieldSource);

                fieldId = Convert.ToInt32(fieldCmd.ExecuteScalar());

                field.FieldId = fieldId;
            }

            return fieldId;
        }

        public int AddOptions(List<PrescriptionTemplateFieldOption> options, SqlConnection conn, SqlTransaction tran)
        {
            int optionId = 0;

            foreach (var option in options)
            {
                string optionQuery = @"INSERT INTO PrescriptionTemplateFieldOptions
                            (TemplateId, SectionId, ClinicId, FieldId, OptionText, IsActive, DisplayOrder, IsArchived) 
                            VALUES 
                            (@TemplateId, @SectionId, @ClinicId, @FieldId, @OptionText, @IsActive, @DisplayOrder, 0);
                                SELECT SCOPE_IDENTITY();";
                using SqlCommand optionCmd = new SqlCommand(optionQuery, conn, tran);

                optionCmd.Parameters.AddWithValue("@TemplateId", option.TemplateId);
                optionCmd.Parameters.AddWithValue("@SectionId", option.SectionId);
                optionCmd.Parameters.AddWithValue("@ClinicId", option.ClinicId);
                optionCmd.Parameters.AddWithValue("@FieldId", option.FieldId);
                optionCmd.Parameters.AddWithValue("@OptionText", option.OptionText);
                optionCmd.Parameters.AddWithValue("@IsActive", option.IsActive);
                optionCmd.Parameters.AddWithValue("@DisplayOrder", option.DisplayOrder);

                optionId = Convert.ToInt32(optionCmd.ExecuteScalar());

                option.OptionId = optionId;
            }

            return optionId;
        }

        public bool Update(PrescriptionTemplate template, SqlConnection conn, SqlTransaction tran)
        {
            int templateRowsAffected = 0;

            string templateQuery = @"UPDATE PrescriptionTemplates
                                    SET ClinicId = @ClinicId, 
                                        DepartmentId = @DepartmentId, 
                                        TemplateName = @TemplateName, 
                                        IsActive = @IsActive
                                    WHERE TemplateId = @TemplateId";
            using SqlCommand templateCmd = new SqlCommand(templateQuery, conn, tran);

            templateCmd.Parameters.AddWithValue("@ClinicId", template.ClinicId);
            templateCmd.Parameters.AddWithValue("@DepartmentId", template.DepartmentId);
            templateCmd.Parameters.AddWithValue("@TemplateName", template.TemplateName);
            templateCmd.Parameters.AddWithValue("@IsActive", template.IsActive);
            templateCmd.Parameters.AddWithValue("@TemplateId", template.TemplateId);

            templateRowsAffected = templateCmd.ExecuteNonQuery();

            return templateRowsAffected > 0;
        }

        public bool UpdateSections(List<PrescriptionTemplateSection> sections, SqlConnection conn, SqlTransaction tran)
        {
            int sectionRowsAffected = 0;

            foreach (var section in sections)
            {
                string sectionQuery = @"UPDATE PrescriptionTemplateSections
                                            SET TemplateId = @TemplateId, 
                                                ClinicId = @ClinicId, 
                                                SectionName = @SectionName, 
                                                DisplayOrder = @DisplayOrder,
                                                IsActive = @IsActive,
                                                Multiline = @Multiline,
                                                IsPharmacologyAdvice = @IsPharmacologyAdvice,
                                                IsVital = @IsVital,
                                                IsTestResult = @IsTestResult
                                            WHERE SectionId = @SectionId";
                using SqlCommand sectionCmd = new SqlCommand(sectionQuery, conn, tran);

                sectionCmd.Parameters.AddWithValue("@TemplateId", section.TemplateId);
                sectionCmd.Parameters.AddWithValue("@ClinicId", section.ClinicId);
                sectionCmd.Parameters.AddWithValue("@SectionName", section.SectionName);
                sectionCmd.Parameters.AddWithValue("@DisplayOrder", section.DisplayOrder);
                sectionCmd.Parameters.AddWithValue("@SectionId", section.SectionId);
                sectionCmd.Parameters.AddWithValue("@IsActive", section.IsActive ? 1 : 0);
                sectionCmd.Parameters.AddWithValue("@Multiline", section.Multiline ? 1 : 0);
                sectionCmd.Parameters.AddWithValue("@IsPharmacologyAdvice", section.IsPharmacologyAdvice ? 1 : 0);
                sectionCmd.Parameters.AddWithValue("@IsVital", section.IsVital ? 1 : 0);
                sectionCmd.Parameters.AddWithValue("@IsTestResult", section.IsTestResult ? 1 : 0);            

                sectionRowsAffected += sectionCmd.ExecuteNonQuery();
            }

            return sectionRowsAffected > 0;
        }

        public bool UpdateFields(List<PrescriptionTemplateField> fields, SqlConnection conn, SqlTransaction tran)
        {
            int fieldRowsAffected = 0;

            foreach (var field in fields)
            {
                string fieldQuery = @"UPDATE PrescriptionTemplateFields
                                        SET TemplateId = @TemplateId,
                                            SectionId = @SectionId, 
                                            ClinicId = @ClinicId, 
                                            FieldLabel = @FieldLabel, 
                                            FieldTypeCode = @FieldTypeCode, 
                                            FieldCode = @FieldCode, 
                                            IsActive = @IsActive,
                                            IsArchived = @IsArchived,
                                            DisplayOrder = @DisplayOrder,
                                            FieldSource = @FieldSource 
                                        WHERE FieldId = @FieldId";
                using SqlCommand fieldCmd = new SqlCommand(fieldQuery, conn, tran);

                fieldCmd.Parameters.AddWithValue("@TemplateId", field.TemplateId);
                fieldCmd.Parameters.AddWithValue("@SectionId", field.SectionId);
                fieldCmd.Parameters.AddWithValue("@ClinicId", field.ClinicId);
                fieldCmd.Parameters.AddWithValue("@FieldLabel", field.FieldLabel);
                fieldCmd.Parameters.AddWithValue("@FieldTypeCode", field.FieldTypeCode);
                fieldCmd.Parameters.AddWithValue("@FieldCode", field.FieldCode == null ? DBNull.Value : field.FieldCode);
                fieldCmd.Parameters.AddWithValue("@IsActive", field.IsActive);
                fieldCmd.Parameters.AddWithValue("@IsArchived", field.IsArchived);
                fieldCmd.Parameters.AddWithValue("@DisplayOrder", field.DisplayOrder);
                fieldCmd.Parameters.AddWithValue("@FieldSource", field.FieldSource == null ? DBNull.Value : field.FieldSource);
                fieldCmd.Parameters.AddWithValue("@FieldId", field.FieldId);

                fieldRowsAffected += fieldCmd.ExecuteNonQuery();
            }

            return fieldRowsAffected > 0;
        }

        public bool UpdateOptions(List<PrescriptionTemplateFieldOption> options, SqlConnection conn, SqlTransaction tran)
        {
            int optionRowsAffected = 0;

            foreach (var option in options)
            {
                string optionQuery = @"UPDATE PrescriptionTemplateFieldOptions
                                        SET TemplateId = @TemplateId,
                                            SectionId = @SectionId,
                                            FieldId = @FieldId, 
                                            ClinicId = @ClinicId, 
                                            OptionText = @OptionText, 
                                            IsActive = @IsActive, 
                                            IsArchived = @IsArchived,
                                            DisplayOrder = @DisplayOrder 
                                        WHERE OptionId = @OptionId";
                using SqlCommand optionCmd = new SqlCommand(optionQuery, conn, tran);

                optionCmd.Parameters.AddWithValue("@TemplateId", option.TemplateId);
                optionCmd.Parameters.AddWithValue("@SectionId", option.SectionId);
                optionCmd.Parameters.AddWithValue("@ClinicId", option.ClinicId);
                optionCmd.Parameters.AddWithValue("@FieldId", option.FieldId);
                optionCmd.Parameters.AddWithValue("@OptionText", option.OptionText);
                optionCmd.Parameters.AddWithValue("@IsActive", option.IsActive);
                optionCmd.Parameters.AddWithValue("@IsArchived", option.IsArchived);
                optionCmd.Parameters.AddWithValue("@DisplayOrder", option.DisplayOrder);
                optionCmd.Parameters.AddWithValue("@OptionId", option.OptionId);

                optionRowsAffected += optionCmd.ExecuteNonQuery();
            }

            return optionRowsAffected > 0;
        }

        public bool Delete(int templateId, int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            int optionRowsAffected = 0;
            int fieldRowsAffected = 0;
            int sectionRowsAffected = 0;
            int templateRowsAffected = 0;

            var model = Get(templateId, clinicId, conn, tran);

            if (model != null)
            {
                // start deletion from Field Option records
                string optionQuery = @"DELETE FROM PrescriptionTemplateFieldOptions WHERE TemplateId = @TemplateId";
                using SqlCommand optionCmd = new SqlCommand(optionQuery, conn, tran);
                optionCmd.Parameters.AddWithValue("@TemplateId", templateId);
                optionRowsAffected = optionCmd.ExecuteNonQuery();

                // start deletion from Field records
                string fieldQuery = @"DELETE FROM PrescriptionTemplateFields WHERE TemplateId = @TemplateId";
                using SqlCommand fieldCmd = new SqlCommand(fieldQuery, conn, tran);
                fieldCmd.Parameters.AddWithValue("@TemplateId", templateId);
                fieldRowsAffected = fieldCmd.ExecuteNonQuery();

                // then delete Section records
                string sectionQuery = @"DELETE FROM PrescriptionTemplateSections WHERE TemplateId = @TemplateId";
                using SqlCommand sectionCmd = new SqlCommand(sectionQuery, conn, tran);
                sectionCmd.Parameters.AddWithValue("@TemplateId", templateId);
                sectionRowsAffected = sectionCmd.ExecuteNonQuery();

                // and finally delete the Template
                string templateQuery = @"DELETE FROM PrescriptionTemplates WHERE TemplateId = @TemplateId";
                using SqlCommand templateCmd = new SqlCommand(templateQuery, conn, tran);
                templateCmd.Parameters.AddWithValue("@TemplateId", templateId);
                templateRowsAffected = templateCmd.ExecuteNonQuery();

            }

            return templateRowsAffected > 0 || sectionRowsAffected > 0 || fieldRowsAffected > 0 || optionRowsAffected > 0;

        }

        public int GetIdByCode(string templateCode, int clinicId, int departmentId, int doctorId, 
                                    SqlConnection conn, SqlTransaction tran)
        {
            int templateId = 0;

            string templateQuery = @"SELECT TemplateId 
                                    FROM PrescriptionTemplates
                                    WHERE TemplateCode = @TemplateCode";
            if (clinicId == 0)
                templateQuery += " AND ClinicId IS NULL";
            else
                templateQuery += " AND ClinicId = @ClinicId";

            if (departmentId == 0)
                templateQuery += " AND DepartmentId IS NULL";
            else
                templateQuery += " AND DepartmentId = @DepartmentId";

            if (doctorId == 0)
                templateQuery += " AND DoctorId IS NULL";
            else
                templateQuery += " AND DoctorId = @DoctorId";

            using SqlCommand templateCmd = new SqlCommand(templateQuery, conn);
            if (tran != null)
            {
                templateCmd.Transaction = tran;
            }

            templateCmd.Parameters.AddWithValue("@TemplateCode", templateCode);

            if (clinicId != 0)
                templateCmd.Parameters.AddWithValue("@ClinicId", clinicId);

            if (departmentId != 0)
                templateCmd.Parameters.AddWithValue("@DepartmentId", departmentId);
            
            if (doctorId != 0)
                templateCmd.Parameters.AddWithValue("@DoctorId", doctorId);

            using (var templateReader = templateCmd.ExecuteReader())
            {
                if (templateReader.Read())
                {
                    templateId = (int)templateReader["TemplateId"];
                }
            }

            return templateId;
            
        }

        public int CloneTemplate(string templateCode, int srcClinicId, int tgtClinicId, int tgtDepartmentId,
                                                SqlConnection conn, SqlTransaction tran)
        {
            int templateId = GetIdByCode(templateCode, srcClinicId, 0, 0, conn, tran);
            if (templateId == 0)
                return templateId;

            PrescriptionTemplate template = Get(templateId, srcClinicId, conn, tran);

            template.ClinicId = tgtClinicId;
            template.DepartmentId = tgtDepartmentId;
            template.SourceTemplateId = template.TemplateId;
            template.TemplateId = 0;

            Add(template, conn, tran);

            foreach (var section in template.Sections)
            {
                section.TemplateId = template.TemplateId;
                section.ClinicId = template.ClinicId;
            }

            AddSections(template.Sections, conn, tran);

            foreach (var section in template.Sections)
            {
                foreach (var field in section.Fields)
                {
                    field.SectionId = section.SectionId;
                    field.TemplateId = template.TemplateId;
                    field.ClinicId = template.ClinicId;
                }

                AddFields(section.Fields, conn, tran);

                foreach (var field in section.Fields)
                {
                    foreach (var option in field.Options)
                    {
                        option.FieldId = field.FieldId;
                        option.SectionId = section.SectionId;
                        option.TemplateId = template.TemplateId;
                        option.ClinicId = template.ClinicId;
                    }

                    AddOptions(field.Options, conn, tran);
                }
            }
            return template.TemplateId;
        }

        public int CloneTemplate(int templateId, int srcClinicId, int tgtClinicId, int tgtDepartmentId, 
                                                SqlConnection conn, SqlTransaction tran)
        {
            PrescriptionTemplate template = Get(templateId, srcClinicId, conn, tran);

            template.ClinicId = tgtClinicId;
            template.DepartmentId = tgtDepartmentId;
            template.SourceTemplateId = template.TemplateId;
            template.TemplateId = 0;

            Add(template, conn, tran);

            foreach (var section in template.Sections)
            {
                section.TemplateId = template.TemplateId;
                section.ClinicId = template.ClinicId;
            }

            AddSections(template.Sections, conn, tran);

            foreach (var section in template.Sections)
            {
                foreach (var field in section.Fields)
                {
                    field.SectionId = section.SectionId;
                    field.TemplateId = template.TemplateId;
                    field.ClinicId = template.ClinicId;
                }

                AddFields(section.Fields, conn, tran);

                foreach (var field in section.Fields)
                {
                    foreach (var option in field.Options)
                    {
                        option.FieldId = field.FieldId;
                        option.SectionId = section.SectionId;
                        option.TemplateId = template.TemplateId;
                        option.ClinicId = template.ClinicId;
                    }

                    AddOptions(field.Options, conn, tran);
                }
            }
            return template.TemplateId;
        }

        public void SaveSectionOrder(List<SectionOrderVM> sections, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                UPDATE PrescriptionTemplateSections
                SET DisplayOrder = @DisplayOrder
                WHERE TemplateId = @TemplateId
                AND SectionId = @SectionId";

            foreach (var section in sections)
            {
                using SqlCommand cmd = new SqlCommand(query, conn, tran);

                cmd.Parameters.AddWithValue("@TemplateId", section.TemplateId);
                cmd.Parameters.AddWithValue("@SectionId", section.SectionId);
                cmd.Parameters.AddWithValue("@DisplayOrder", section.DisplayOrder);

                cmd.ExecuteNonQuery();
            }
        }        

        public void SaveFieldOrder(List<FieldOrderVM> fields, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                UPDATE PrescriptionTemplateFields
                SET DisplayOrder = @DisplayOrder
                WHERE TemplateId = @TemplateId
                AND SectionId = @SectionId
                AND FieldId = @FieldId";

            foreach (var field in fields)
            {
                using SqlCommand cmd = new SqlCommand(query, conn, tran);

                cmd.Parameters.AddWithValue("@TemplateId", field.TemplateId);
                cmd.Parameters.AddWithValue("@SectionId", field.SectionId);
                cmd.Parameters.AddWithValue("@FieldId", field.FieldId);
                cmd.Parameters.AddWithValue("@DisplayOrder", field.DisplayOrder);

                cmd.ExecuteNonQuery();
            }
        }        

        public void SaveOptionOrder(List<OptionOrderVM> options, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                UPDATE PrescriptionTemplateFieldOptions
                SET DisplayOrder = @DisplayOrder
                WHERE TemplateId = @TemplateId
                AND FieldId = @FieldId
                AND OptionId = @OptionId";

            foreach (var option in options)
            {
                using SqlCommand cmd = new SqlCommand(query, conn, tran);

                cmd.Parameters.AddWithValue("@TemplateId", option.TemplateId);
                cmd.Parameters.AddWithValue("@FieldId", option.FieldId);
                cmd.Parameters.AddWithValue("@OptionId", option.OptionId);
                cmd.Parameters.AddWithValue("@DisplayOrder", option.DisplayOrder);

                cmd.ExecuteNonQuery();
            }
        }

        public bool ToggleSection(int templateId, int sectionId, SqlConnection conn, SqlTransaction tran)
        {
            int rowsAffected = 0;

            string query = @"UPDATE PrescriptionTemplateSections
                                        SET IsActive = CASE
                                                        WHEN IsActive = 1 THEN 0
                                                        ELSE 1
                                                    END
                                        WHERE TemplateId = @TemplateId
                                        AND SectionId = @SectionId;";
            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TemplateId", templateId);
            cmd.Parameters.AddWithValue("@SectionId", sectionId);

            rowsAffected = cmd.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public PrescriptionTemplateSection GetSection(int templateId, int sectionId, SqlConnection conn, SqlTransaction tran)
        {
            string sectionQuery = @"SELECT * FROM PrescriptionTemplateSections 
                                        WHERE TemplateId = @TemplateId
                                        AND SectionId = @SectionId";
            using SqlCommand sectionCmd = new SqlCommand(sectionQuery, conn);
            if (tran != null)
            {
                sectionCmd.Transaction = tran;
            }

            sectionCmd.Parameters.AddWithValue("@TemplateId", templateId);
            sectionCmd.Parameters.AddWithValue("@SectionId", sectionId);

            using (var sectionReader = sectionCmd.ExecuteReader())
            {
                if (sectionReader.Read())
                {
                    return new PrescriptionTemplateSection
                    {
                        TemplateId = (int)sectionReader["TemplateId"],
                        ClinicId = (int)sectionReader["ClinicId"],
                        SectionId = (int)sectionReader["SectionId"],
                        SectionName = sectionReader["SectionName"].ToString(),
                        DisplayOrder = (int)sectionReader["DisplayOrder"],
                        IsActive = (bool)sectionReader["IsActive"],
                        Multiline = (bool)sectionReader["Multiline"],
                        IsPharmacologyAdvice = (bool)sectionReader["IsPharmacologyAdvice"],
                        IsVital = (bool)sectionReader["IsVital"],
                        IsTestResult = (bool)sectionReader["IsTestResult"] 
                    };
                }
            }

            return null;           
        }

        public bool ToggleField(int templateId, int fieldId, SqlConnection conn, SqlTransaction tran)
        {
            int rowsAffected = 0;

            string query = @"UPDATE PrescriptionTemplateFields
                                        SET IsActive = CASE
                                                        WHEN IsActive = 1 THEN 0
                                                        ELSE 1
                                                    END
                                        WHERE TemplateId = @TemplateId
                                        AND FieldId = @FieldId;";
            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TemplateId", templateId);
            cmd.Parameters.AddWithValue("@FieldId", fieldId);

            rowsAffected = cmd.ExecuteNonQuery();

            return rowsAffected > 0;
        }
       
        public PrescriptionTemplateField GetField(int templateId, int fieldId, SqlConnection conn, SqlTransaction tran)
        {
            string fieldQuery = @"SELECT f.*, COUNT(o.OptionId) as OptionCount
                                    FROM PrescriptionTemplateFields f
                                    LEFT JOIN PrescriptionTemplateFieldOptions o 
                                        ON f.TemplateId = o.TemplateId 
                                        AND f.FieldId = o.FieldId
                                    WHERE f.TemplateId = @TemplateId
                                    AND f.FieldId = @FieldId
                                    GROUP BY f.TemplateId, f.ClinicId, f.SectionId, f.FieldId, 
                                            f.FieldLabel, f.FieldTypeCode, f.IsActive, f.DisplayOrder, f.IsRequired, f.IsArchived, f.FieldSource";
            using SqlCommand fieldCmd = new SqlCommand(fieldQuery, conn);
            if (tran != null)
            {
                fieldCmd.Transaction = tran;
            }

            fieldCmd.Parameters.AddWithValue("@TemplateId", templateId);
            fieldCmd.Parameters.AddWithValue("@FieldId", fieldId);

            using (var fieldReader = fieldCmd.ExecuteReader())
            {
                if (fieldReader.Read())
                {
                    return (new PrescriptionTemplateField
                    {
                        TemplateId = (int)fieldReader["TemplateId"],
                        ClinicId = (int)fieldReader["ClinicId"],
                        SectionId = (int)fieldReader["SectionId"],
                        FieldId = (int)fieldReader["FieldId"],
                        FieldLabel = fieldReader["FieldLabel"].ToString(),
                        FieldTypeCode = fieldReader["FieldTypeCode"].ToString(),
                        IsActive = (bool)fieldReader["IsActive"],
                        IsArchived = (bool)fieldReader["IsArchived"],
                        DisplayOrder = (int)fieldReader["DisplayOrder"],
                        OptionCount = (int)fieldReader["OptionCount"],
                        FieldSource = fieldReader["FieldSource"]!= DBNull.Value ? fieldReader["FieldSource"].ToString() : null
                    });
                }
            }

            return null;
        }

        public bool ToggleOption(int templateId, int optionId, SqlConnection conn, SqlTransaction tran)
        {
            int rowsAffected = 0;

            string query = @"UPDATE PrescriptionTemplateFieldOptions
                                        SET IsActive = CASE
                                                        WHEN IsActive = 1 THEN 0
                                                        ELSE 1
                                                    END
                                        WHERE TemplateId = @TemplateId
                                        AND OptionId = @OptionId;";
            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TemplateId", templateId);
            cmd.Parameters.AddWithValue("@OptionId", optionId);

            rowsAffected = cmd.ExecuteNonQuery();

            return rowsAffected > 0;
        }
       
        public PrescriptionTemplateFieldOption GetOption(int templateId, int optionId, SqlConnection conn, SqlTransaction tran)
        {
            string fieldQuery = @"SELECT * FROM PrescriptionTemplateFieldOptions 
                                    WHERE TemplateId = @TemplateId
                                    AND OptionId = @OptionId
                                    ORDER BY DisplayOrder";
            using SqlCommand fieldCmd = new SqlCommand(fieldQuery, conn);
            if (tran != null)
            {
                fieldCmd.Transaction = tran;
            }

            fieldCmd.Parameters.AddWithValue("@TemplateId", templateId);
            fieldCmd.Parameters.AddWithValue("@OptionId", optionId);

            using (var fieldReader = fieldCmd.ExecuteReader())
            {
                if (fieldReader.Read())
                {
                    return (new PrescriptionTemplateFieldOption
                    {
                        TemplateId = (int)fieldReader["TemplateId"],
                        ClinicId = (int)fieldReader["ClinicId"],
                        SectionId = (int)fieldReader["SectionId"],
                        FieldId = (int)fieldReader["FieldId"],
                        OptionId = (int)fieldReader["OptionId"],
                        OptionText = fieldReader["OptionText"].ToString(),
                        IsActive = (bool)fieldReader["IsActive"],
                        IsArchived = (bool)fieldReader["IsArchived"],
                        DisplayOrder = (int)fieldReader["DisplayOrder"]
                    });
                }
            }

            return null;
        }

        public int InsertSectionBefore(int templateId, int sectionId, int clinicId, string sectionName,
                                            int currentOrder, SqlConnection conn, SqlTransaction tran)
        {
            int updateRowsAffected = 0;

            string updateQuery = @"UPDATE PrescriptionTemplateSections
                                SET DisplayOrder = DisplayOrder + 1
                                WHERE TemplateId = @TemplateId
                                AND DisplayOrder >= @CurrentOrder;";
            using SqlCommand updateCmd = new SqlCommand(updateQuery, conn, tran);

            updateCmd.Parameters.AddWithValue("@TemplateId", templateId);
            updateCmd.Parameters.AddWithValue("@CurrentOrder", currentOrder);

            updateRowsAffected = updateCmd.ExecuteNonQuery();

            string insertQuery = @"INSERT INTO PrescriptionTemplateSections
                                    (TemplateId, ClinicId, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice
                                    IsVital, IsTestResult)
                                    VALUES
                                    (@TemplateId, @ClinicId, @SectionName, @CurrentOrder, 1, 0, 0, 0, 0);
                                    SELECT SCOPE_IDENTITY();";
            using SqlCommand insertCmd = new SqlCommand(insertQuery, conn, tran);

            insertCmd.Parameters.AddWithValue("@TemplateId", templateId);
            insertCmd.Parameters.AddWithValue("@ClinicId", clinicId);
            insertCmd.Parameters.AddWithValue("@SectionName", sectionName);
            insertCmd.Parameters.AddWithValue("@CurrentOrder", currentOrder);

            int id = Convert.ToInt32(insertCmd.ExecuteScalar());

            return id;
        }

        public int InsertSectionAfter(int templateId, int sectionId, int clinicId, string sectionName,
                                            int currentOrder, SqlConnection conn, SqlTransaction tran)
        {
            int updateRowsAffected = 0;

            string updateQuery = @"UPDATE PrescriptionTemplateSections
                                        SET DisplayOrder = DisplayOrder + 1
                                        WHERE TemplateId = @TemplateId
                                        AND DisplayOrder > @CurrentOrder;";
            using SqlCommand updateCmd = new SqlCommand(updateQuery, conn, tran);

            updateCmd.Parameters.AddWithValue("@TemplateId", templateId);
            updateCmd.Parameters.AddWithValue("@CurrentOrder", currentOrder);

            updateRowsAffected = updateCmd.ExecuteNonQuery();

            string insertQuery = @"INSERT INTO PrescriptionTemplateSections
                                    (TemplateId, ClinicId, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice,
                                    IsVital, IsTestResult)
                                    VALUES
                                    (@TemplateId, @ClinicId, @SectionName, @CurrentOrder + 1, 1, 0, 0, 0, 0);
                                    SELECT SCOPE_IDENTITY();";
            using SqlCommand insertCmd = new SqlCommand(insertQuery, conn, tran);

            insertCmd.Parameters.AddWithValue("@TemplateId", templateId);
            insertCmd.Parameters.AddWithValue("@ClinicId", clinicId);
            insertCmd.Parameters.AddWithValue("@SectionName", sectionName);
            insertCmd.Parameters.AddWithValue("@CurrentOrder", currentOrder);

            int id = Convert.ToInt32(insertCmd.ExecuteScalar());

            return id;
        }

        public int AddField(int templateId, int sectionId, int clinicId, string fieldName, string fieldType, 
                                            SqlConnection conn, SqlTransaction tran)
        {
            string insertQuery = @"INSERT INTO PrescriptionTemplateFields
                                    (TemplateId, ClinicId, SectionId, FieldLabel, FieldTypeCode, DisplayOrder, IsActive, IsArchived)
                                    VALUES
                                    (@TemplateId, @ClinicId, @SectionId, @FieldLabel, @FieldTypeCode, 1, 1, 0);
                                    SELECT SCOPE_IDENTITY();";
            using SqlCommand insertCmd = new SqlCommand(insertQuery, conn, tran);

            insertCmd.Parameters.AddWithValue("@TemplateId", templateId);
            insertCmd.Parameters.AddWithValue("@ClinicId", clinicId);
            insertCmd.Parameters.AddWithValue("@SectionId", sectionId);
            insertCmd.Parameters.AddWithValue("@FieldLabel", fieldName);
            insertCmd.Parameters.AddWithValue("@FieldTypeCode", fieldType);

            int id = Convert.ToInt32(insertCmd.ExecuteScalar());

            return id;
        }

        public int InsertFieldBefore(int templateId, int sectionId, int fieldId, int clinicId, string fieldLabel,
                                            int currentOrder, SqlConnection conn, SqlTransaction tran)
        {
            int updateRowsAffected = 0;

            string updateQuery = @"UPDATE PrescriptionTemplateFields
                                SET DisplayOrder = DisplayOrder + 1
                                WHERE TemplateId = @TemplateId
                                AND SectionId = @SectionId
                                AND DisplayOrder >= @CurrentOrder;";
            using SqlCommand updateCmd = new SqlCommand(updateQuery, conn, tran);

            updateCmd.Parameters.AddWithValue("@TemplateId", templateId);
            updateCmd.Parameters.AddWithValue("@SectionId", sectionId);
            updateCmd.Parameters.AddWithValue("@CurrentOrder", currentOrder);

            updateRowsAffected = updateCmd.ExecuteNonQuery();

            string insertQuery = @"INSERT INTO PrescriptionTemplateFields
                                    (TemplateId, ClinicId, SectionId, FieldLabel, FieldTypeCode, DisplayOrder, IsActive, IsArchived)
                                    VALUES
                                    (@TemplateId, @ClinicId, @SectionId, @FieldLabel, @FieldTypeCode, @CurrentOrder, 1, 0);
                                    SELECT SCOPE_IDENTITY();";
            using SqlCommand insertCmd = new SqlCommand(insertQuery, conn, tran);

            insertCmd.Parameters.AddWithValue("@TemplateId", templateId);
            insertCmd.Parameters.AddWithValue("@ClinicId", clinicId);
            insertCmd.Parameters.AddWithValue("@SectionId", sectionId);
            insertCmd.Parameters.AddWithValue("@FieldLabel", fieldLabel);
            insertCmd.Parameters.AddWithValue("@FieldTypeCode", "Text");
            insertCmd.Parameters.AddWithValue("@CurrentOrder", currentOrder);

            int id = Convert.ToInt32(insertCmd.ExecuteScalar());

            return id;
        }

        public int InsertFieldAfter(int templateId, int sectionId, int fieldId, int clinicId, string fieldLabel,
                                            int currentOrder, SqlConnection conn, SqlTransaction tran)
        {
            int updateRowsAffected = 0;

            string updateQuery = @"UPDATE PrescriptionTemplateFields
                                        SET DisplayOrder = DisplayOrder + 1
                                        WHERE TemplateId = @TemplateId
                                        AND SectionId = @SectionId
                                        AND DisplayOrder > @CurrentOrder;";
            using SqlCommand updateCmd = new SqlCommand(updateQuery, conn, tran);

            updateCmd.Parameters.AddWithValue("@TemplateId", templateId);
            updateCmd.Parameters.AddWithValue("@SectionId", sectionId);
            updateCmd.Parameters.AddWithValue("@CurrentOrder", currentOrder);

            updateRowsAffected = updateCmd.ExecuteNonQuery();

            string insertQuery = @"INSERT INTO PrescriptionTemplateFields
                                    (TemplateId, ClinicId, SectionId, FieldLabel, FieldTypeCode, DisplayOrder, IsActive, IsArchived)
                                    VALUES
                                    (@TemplateId, @ClinicId, @SectionId, @FieldLabel, @FieldTypeCode, @CurrentOrder + 1, 1, 0);
                                    SELECT SCOPE_IDENTITY();";
            using SqlCommand insertCmd = new SqlCommand(insertQuery, conn, tran);

            insertCmd.Parameters.AddWithValue("@TemplateId", templateId);
            insertCmd.Parameters.AddWithValue("@ClinicId", clinicId);
            insertCmd.Parameters.AddWithValue("@SectionId", sectionId);
            insertCmd.Parameters.AddWithValue("@FieldLabel", fieldLabel);
            insertCmd.Parameters.AddWithValue("@FieldTypeCode", "Text");
            insertCmd.Parameters.AddWithValue("@CurrentOrder", currentOrder);

            int id = Convert.ToInt32(insertCmd.ExecuteScalar());

            return id;
        }

        public int InsertOptionBefore(int templateId, int sectionId, int fieldId, int optionId, int clinicId,  
                                        string optionText, int currentOrder, SqlConnection conn, SqlTransaction tran)
        {
            int updateRowsAffected = 0;

            string updateQuery = @"UPDATE PrescriptionTemplateFieldOptions
                                SET DisplayOrder = DisplayOrder + 1
                                WHERE TemplateId = @TemplateId
                                AND FieldId = @FieldId
                                AND DisplayOrder >= @CurrentOrder;";
            using SqlCommand updateCmd = new SqlCommand(updateQuery, conn, tran);

            updateCmd.Parameters.AddWithValue("@TemplateId", templateId);
            updateCmd.Parameters.AddWithValue("@FieldId", fieldId);
            updateCmd.Parameters.AddWithValue("@CurrentOrder", currentOrder);

            updateRowsAffected = updateCmd.ExecuteNonQuery();

            string insertQuery = @"INSERT INTO PrescriptionTemplateFieldOptions
                                    (TemplateId, ClinicId, SectionId, FieldId, OptionText, DisplayOrder, IsActive, IsArchived)
                                    VALUES
                                    (@TemplateId, @ClinicId, @SectionId, @FieldId, @OptionText, @CurrentOrder, 1, 0);
                                    SELECT SCOPE_IDENTITY();";

            using SqlCommand insertCmd = new SqlCommand(insertQuery, conn, tran);

            insertCmd.Parameters.AddWithValue("@TemplateId", templateId);
            insertCmd.Parameters.AddWithValue("@ClinicId", clinicId);
            insertCmd.Parameters.AddWithValue("@SectionId", sectionId);
            insertCmd.Parameters.AddWithValue("@FieldId", fieldId);
            insertCmd.Parameters.AddWithValue("@OptionText", optionText);
            insertCmd.Parameters.AddWithValue("@CurrentOrder", currentOrder);

            int id = Convert.ToInt32(insertCmd.ExecuteScalar());

            return id;
        }

        public int InsertOptionAfter(int templateId, int sectionId, int fieldId, int optionId, int clinicId,  
                                        string optionText, int currentOrder, SqlConnection conn, SqlTransaction tran)
        {
            int updateRowsAffected = 0;

            string updateQuery = @"UPDATE PrescriptionTemplateFieldOptions
                                        SET DisplayOrder = DisplayOrder + 1
                                        WHERE TemplateId = @TemplateId
                                        AND FieldId = @FieldId
                                        AND DisplayOrder > @CurrentOrder;";
            using SqlCommand updateCmd = new SqlCommand(updateQuery, conn, tran);

            updateCmd.Parameters.AddWithValue("@TemplateId", templateId);
            updateCmd.Parameters.AddWithValue("@FieldId", fieldId);
            updateCmd.Parameters.AddWithValue("@CurrentOrder", currentOrder);

            updateRowsAffected = updateCmd.ExecuteNonQuery();

            string insertQuery = @"INSERT INTO PrescriptionTemplateFieldOptions
                                    (TemplateId, ClinicId, SectionId, FieldId, OptionText, DisplayOrder, IsActive, IsArchived)
                                    VALUES
                                    (@TemplateId, @ClinicId, @SectionId, @FieldId, @OptionText, @CurrentOrder + 1, 1, 0);
                                    SELECT SCOPE_IDENTITY();";
            using SqlCommand insertCmd = new SqlCommand(insertQuery, conn, tran);

            insertCmd.Parameters.AddWithValue("@TemplateId", templateId);
            insertCmd.Parameters.AddWithValue("@ClinicId", clinicId);
            insertCmd.Parameters.AddWithValue("@SectionId", sectionId);
            insertCmd.Parameters.AddWithValue("@FieldId", fieldId);
            insertCmd.Parameters.AddWithValue("@OptionText", optionText);
            insertCmd.Parameters.AddWithValue("@CurrentOrder", currentOrder);

            int id = Convert.ToInt32(insertCmd.ExecuteScalar());

            return id;
        }

        public bool SaveSectionName(int templateId, int sectionId, string name, SqlConnection conn, SqlTransaction tran)
        {
            int rowsAffected = 0;

            string query = @"UPDATE PrescriptionTemplateSections
                                        SET SectionName = @SectionName
                                        WHERE TemplateId = @TemplateId
                                        AND SectionId = @SectionId;";
            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TemplateId", templateId);
            cmd.Parameters.AddWithValue("@SectionId", sectionId);
            cmd.Parameters.AddWithValue("@SectionName", name);

            rowsAffected = cmd.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public bool SaveFieldName(int templateId, int fieldId, string name, SqlConnection conn, SqlTransaction tran)
        {
            int rowsAffected = 0;

            string query = @"UPDATE PrescriptionTemplateFields
                                        SET FieldLabel = @FieldLabel
                                        WHERE TemplateId = @TemplateId
                                        AND FieldId = @FieldId;";
            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TemplateId", templateId);
            cmd.Parameters.AddWithValue("@FieldId", fieldId);
            cmd.Parameters.AddWithValue("@FieldLabel", name);

            rowsAffected = cmd.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public bool SaveOptionName(int templateId, int optionId, string name, SqlConnection conn, SqlTransaction tran)
        {
            int rowsAffected = 0;

            string query = @"UPDATE PrescriptionTemplateFieldOptions
                                        SET OptionText = @OptionText
                                        WHERE TemplateId = @TemplateId
                                        AND OptionId = @OptionId;";
            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TemplateId", templateId);
            cmd.Parameters.AddWithValue("@OptionId", optionId);
            cmd.Parameters.AddWithValue("@OptionText", name);

            rowsAffected = cmd.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public bool DeleteOptions(int templateId, int fieldId, SqlConnection conn, SqlTransaction tran)
        {
            int optionRowsAffected = 0;

            string optionQuery = @"DELETE FROM PrescriptionTemplateFieldOptions 
                                    WHERE TemplateId = @TemplateId
                                    AND FieldId = @FieldId";
            using SqlCommand optionCmd = new SqlCommand(optionQuery, conn, tran);

            optionCmd.Parameters.AddWithValue("@TemplateId", templateId);
            optionCmd.Parameters.AddWithValue("@FieldId", fieldId);

            optionRowsAffected = optionCmd.ExecuteNonQuery();

            return optionRowsAffected > 0;
        }

        public bool UpdateFieldType(int templateId, int fieldId, string fieldTypeCode, 
                                        SqlConnection conn, SqlTransaction tran)
        {
            int rowsAffected = 0;

            string query = @"UPDATE PrescriptionTemplateFields
                                        SET FieldTypeCode = @FieldTypeCode
                                        WHERE TemplateId = @TemplateId
                                        AND FieldId = @FieldId;";
            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TemplateId", templateId);
            cmd.Parameters.AddWithValue("@FieldId", fieldId);
            cmd.Parameters.AddWithValue("@FieldTypeCode", fieldTypeCode);

            rowsAffected = cmd.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public int AddOption(int templateId, int sectionId, int clinicId, int fieldId, string optionText,  
                                bool isActive, int displayOrder, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"INSERT INTO PrescriptionTemplateFieldOptions
                                    (TemplateId, SectionId, ClinicId, FieldId, OptionText, IsActive, IsArchived, DisplayOrder) 
                                    VALUES 
                                    (@TemplateId, @SectionId, @ClinicId, @FieldId, @OptionText, @IsActive, 0, @DisplayOrder);
                                        SELECT SCOPE_IDENTITY();";
            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TemplateId", templateId);
            cmd.Parameters.AddWithValue("@SectionId", sectionId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);
            cmd.Parameters.AddWithValue("@FieldId", fieldId);
            cmd.Parameters.AddWithValue("@OptionText", optionText);
            cmd.Parameters.AddWithValue("@IsActive", isActive);
            cmd.Parameters.AddWithValue("@DisplayOrder", displayOrder);

            int id = Convert.ToInt32(cmd.ExecuteScalar());

            return id;

        }


        public List<FieldType> ListFieldTypes(SqlConnection conn, SqlTransaction? tran = null)
        {
            List<FieldType> fieldTypes = new List<FieldType>();

            string query = @"SELECT * FROM FieldTypes";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                fieldTypes.Add(new FieldType
                {
                    FieldTypeCode = dr["FieldTypeCode"]?.ToString() ?? ""
                });
            }

            return fieldTypes;
        }

        public List<PrescriptionTemplate> SearchFromLibrary(string term, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<PrescriptionTemplate>();

            string query = @"SELECT TOP 10
                                TemplateId,
                                TemplateName,
                                SourceTemplateId
                            FROM PrescriptionTemplates
                            WHERE TemplateName LIKE '%' + @term + '%'
                            AND IsActive = 1
                            AND ClinicId IS NULL
                            ORDER BY TemplateName";

            var cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@term", term);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new PrescriptionTemplate
                {
                    TemplateId = (int)reader["TemplateId"],
                    TemplateName = reader["TemplateName"].ToString(),
                    SourceTemplateId = reader["SourceTemplateId"] != DBNull.Value ? (int)reader["SourceTemplateId"] : 0
                });
            }

            return list;
        }

        public bool SaveSectionOptions(int templateId, int sectionId, bool isMultiline, bool isRx, 
                                            bool isVital, bool isTestResult, SqlConnection conn, SqlTransaction tran)
        {
            int rowsAffected = 0;

            string query = @"UPDATE PrescriptionTemplateSections
                                        SET Multiline = @IsMultiline,
                                            IsPharmacologyAdvice = @IsRx,
                                            IsVital = @IsVital,
                                            IsTestResult = @IsTestResult
                                        WHERE TemplateId = @TemplateId
                                        AND SectionId = @SectionId;";
            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TemplateId", templateId);
            cmd.Parameters.AddWithValue("@SectionId", sectionId);
            cmd.Parameters.AddWithValue("@IsMultiline", isMultiline ? 1 : 0);
            cmd.Parameters.AddWithValue("@IsRx", isRx ? 1 : 0);
            cmd.Parameters.AddWithValue("@IsVital", isVital ? 1 : 0);
            cmd.Parameters.AddWithValue("@IsTestResult", isTestResult ? 1 : 0);

            rowsAffected = cmd.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public bool UpdateFieldsArchival(int templateId, int sectionId, bool isArchived, SqlConnection conn, SqlTransaction tran)
        {
            int rowsAffected = 0;
            bool existingIsArchived = !isArchived;

            string query = @"UPDATE PrescriptionTemplateFields
                                        SET IsArchived = @IsArchived
                                        WHERE TemplateId = @TemplateId
                                        AND SectionId = @SectionId
                                        AND IsArchived = @ExistingIsArchived;";
            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TemplateId", templateId);
            cmd.Parameters.AddWithValue("@SectionId", sectionId);
            cmd.Parameters.AddWithValue("@IsArchived", isArchived);
            cmd.Parameters.AddWithValue("@ExistingIsArchived", existingIsArchived);

            rowsAffected = cmd.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public bool UpdateOptionsArchival(int templateId, int sectionId, bool isArchived, SqlConnection conn, SqlTransaction tran)
        {
            int rowsAffected = 0;
            bool existingIsArchived = !isArchived;

            string query = @"UPDATE PrescriptionTemplateFieldOptions
                                        SET IsArchived = @IsArchived
                                        WHERE TemplateId = @TemplateId
                                        AND SectionId = @SectionId
                                        AND IsArchived = @ExistingIsArchived;";
            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TemplateId", templateId);
            cmd.Parameters.AddWithValue("@SectionId", sectionId);
            cmd.Parameters.AddWithValue("@IsArchived", isArchived);
            cmd.Parameters.AddWithValue("@ExistingIsArchived", existingIsArchived);

            rowsAffected = cmd.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public bool SaveFieldSource(int templateId, int fieldId, string fieldSource, SqlConnection conn, SqlTransaction tran)
        {
            int rowsAffected = 0;

            string query = @"UPDATE PrescriptionTemplateFields
                                        SET FieldSource = @FieldSource
                                        WHERE TemplateId = @TemplateId
                                        AND FieldId = @FieldId;";
            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TemplateId", templateId);
            cmd.Parameters.AddWithValue("@FieldId", fieldId);
            cmd.Parameters.AddWithValue("@FieldSource", fieldSource);

            rowsAffected = cmd.ExecuteNonQuery();

            return rowsAffected > 0;            
        }

    }

}
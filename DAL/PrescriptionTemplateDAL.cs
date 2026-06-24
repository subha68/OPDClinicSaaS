using Microsoft.Data.SqlClient;
using Models;

namespace DAL
{
    public class PrescriptionTemplateDAL : IDAL<PrescriptionTemplate>
    {
        private readonly DbHelper _db;

        public PrescriptionTemplateDAL(IConfiguration configuration)
        {
            _db = new DbHelper(configuration);
        }

        public List<PrescriptionTemplate> GetAll(SqlConnection conn, SqlTransaction? tran = null)
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
                    IsActive = (bool)templateReader["IsActive"]
                });
            }

            return templates;
        }

        public PrescriptionTemplate Get(int templateId, SqlConnection conn, SqlTransaction? tran = null)
        {
            PrescriptionTemplate template = null;

            // retrieve template

            string templateQuery = @"SELECT t.*, d.Name 
                                    FROM PrescriptionTemplates t
                                    INNER JOIN Departments d
                                        ON t.DepartmentId = d.DepartmentId
                                    WHERE TemplateId = @TemplateId";
            using SqlCommand templateCmd = new SqlCommand(templateQuery, conn);
            if (tran != null)
            {
                templateCmd.Transaction = tran;
            }

            templateCmd.Parameters.AddWithValue("@TemplateId", templateId);

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

        public PrescriptionTemplate GetByDepartment(int departmentId, SqlConnection conn, SqlTransaction? tran = null)
        {
            PrescriptionTemplate template = null;

            // retrieve template

            string templateQuery = @"SELECT t.*, d.Name 
                                    FROM PrescriptionTemplates t
                                    INNER JOIN Departments d
                                        ON t.DepartmentId = d.DepartmentId 
                                    WHERE DepartmentId = @DepartmentId";
            using SqlCommand templateCmd = new SqlCommand(templateQuery, conn);
            if (tran != null)
            {
                templateCmd.Transaction = tran;
            }

            templateCmd.Parameters.AddWithValue("@DepartmentId", departmentId);

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

        public PrescriptionTemplate GetByDoctor(int doctorId, SqlConnection conn, SqlTransaction? tran = null)
        {
            PrescriptionTemplate template = null;

            // retrieve template

            string templateQuery = @"SELECT t.*, d.Name 
                                    FROM PrescriptionTemplates t
                                    INNER JOIN Departments d
                                        ON t.DepartmentId = d.DepartmentId 
                                    WHERE DoctorId = @DoctorId";
            using SqlCommand templateCmd = new SqlCommand(templateQuery, conn);
            if (tran != null)
            {
                templateCmd.Transaction = tran;
            }

            templateCmd.Parameters.AddWithValue("@DoctorId", doctorId);

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

        private List<PrescriptionTemplateSection> GetSectionsAndFields(int templateId, SqlConnection conn, SqlTransaction? tran = null)
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
                        ClinicId = (int)sectionReader["ClinicId"],
                        SectionId = (int)sectionReader["SectionId"],
                        SectionName = sectionReader["SectionName"].ToString(),
                        DisplayOrder = (int)sectionReader["DisplayOrder"]
                    });
                }
            }

            return sections;
        }

        public List<PrescriptionTemplateField> GetFieldsForTemplate(int templateId, SqlConnection conn, SqlTransaction? tran = null)
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
                        ClinicId = (int)fieldReader["ClinicId"],
                        SectionId = (int)fieldReader["SectionId"],
                        FieldId = (int)fieldReader["FieldId"],
                        FieldLabel = fieldReader["FieldLabel"].ToString(),
                        FieldTypeCode = fieldReader["FieldTypeCode"].ToString(),
                        IsRequired = (bool)fieldReader["IsRequired"],
                        DisplayOrder = (int)fieldReader["DisplayOrder"]

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
                        IsRequired = (bool)fieldReader["IsRequired"],
                        DisplayOrder = (int)fieldReader["DisplayOrder"]

                    });

                }
            }

            return fields;
        }

        public List<PrescriptionTemplateFieldOption> GetOptionsForTemplate(int templateId, SqlConnection conn, SqlTransaction? tran = null)
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
                        ClinicId = (int)optionReader["ClinicId"],
                        SectionId = (int)optionReader["SectionId"],
                        FieldId = (int)optionReader["FieldId"],
                        OptionText = optionReader["OptionText"].ToString(),
                        OptionValue = optionReader["OptionValue"].ToString(),
                        IsRequired = (bool)optionReader["IsRequired"],
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
                        OptionValue = optionReader["OptionValue"].ToString(),
                        IsRequired = (bool)optionReader["IsRequired"],
                        DisplayOrder = (int)optionReader["DisplayOrder"]

                    });

                }
            }

            return options;            
        }

        public int Add(PrescriptionTemplate template, SqlConnection conn, SqlTransaction tran)
        {
            string templateQuery = @"INSERT INTO PrescriptionTemplates
                             (ClinicId, DepartmentId, TemplateName, IsActive) 
                             VALUES 
                             (@ClinicId, @DepartmentId, @TemplateName, @IsActive);
                                SELECT SCOPE_IDENTITY();";
            using SqlCommand templateCmd = new SqlCommand(templateQuery, conn, tran);

            templateCmd.Parameters.AddWithValue("@ClinicId", template.ClinicId);
            templateCmd.Parameters.AddWithValue("@DepartmentId", template.DepartmentId);
            templateCmd.Parameters.AddWithValue("@TemplateName", template.TemplateName);
            templateCmd.Parameters.AddWithValue("@IsActive", template.IsActive);

            int templateId = Convert.ToInt32(templateCmd.ExecuteScalar());

            foreach (var section in template.Sections)
            {
                section.TemplateId = templateId;
                section.ClinicId = template.ClinicId;
                
                string sectionQuery = @"INSERT INTO PrescriptionTemplateSections
                             (TemplateId, ClinicId, SectionName, DisplayOrder) 
                             VALUES 
                             (@TemplateId, @ClinicId, @SectionName, @DisplayOrder);
                                SELECT SCOPE_IDENTITY();";
                using SqlCommand sectionCmd = new SqlCommand(sectionQuery, conn, tran);

                sectionCmd.Parameters.AddWithValue("@TemplateId", section.TemplateId);
                sectionCmd.Parameters.AddWithValue("@ClinicId", section.ClinicId);
                sectionCmd.Parameters.AddWithValue("@SectionName", section.SectionName);
                sectionCmd.Parameters.AddWithValue("@DisplayOrder", section.DisplayOrder);

                int sectionId = Convert.ToInt32(sectionCmd.ExecuteScalar());

                foreach (var field in section.Fields)
                {
                    field.TemplateId = templateId;
                    field.SectionId = sectionId;
                    field.ClinicId = template.ClinicId;
                    
                    string fieldQuery = @"INSERT INTO PrescriptionTemplateFields
                                (TemplateId, SectionId, ClinicId, FieldLabel, FieldType, IsRequired, DisplayOrder) 
                                VALUES 
                                (@TemplateId, @SectionId, @ClinicId, @FieldLabel, @FieldType, @IsRequired, @DisplayOrder);
                                    SELECT SCOPE_IDENTITY();";
                    using SqlCommand fieldCmd = new SqlCommand(fieldQuery, conn, tran);

                    fieldCmd.Parameters.AddWithValue("@TemplateId", field.TemplateId);
                    fieldCmd.Parameters.AddWithValue("@SectionId", field.SectionId);
                    fieldCmd.Parameters.AddWithValue("@ClinicId", field.ClinicId);
                    fieldCmd.Parameters.AddWithValue("@FieldLabel", field.FieldLabel);
                    fieldCmd.Parameters.AddWithValue("@FieldType", field.FieldTypeCode);
                    fieldCmd.Parameters.AddWithValue("@IsRequired", field.IsRequired);
                    fieldCmd.Parameters.AddWithValue("@DisplayOrder", field.DisplayOrder);

                    int fieldId = Convert.ToInt32(fieldCmd.ExecuteScalar());

                    foreach (var option in field.Options)
                    {
                        option.TemplateId = templateId;
                        option.SectionId = sectionId;
                        option.FieldId = fieldId;
                        option.ClinicId = template.ClinicId;
                        
                        string optionQuery = @"INSERT INTO PrescriptionTemplateFieldOptions
                                    (TemplateId, SectionId, ClinicId, FieldId, OptionText, OptionValue, IsRequired, DisplayOrder) 
                                    VALUES 
                                    (@TemplateId, @SectionId, @ClinicId, @FieldId, @OptionText, @OptionValue, @IsRequired, @DisplayOrder);
                                        SELECT SCOPE_IDENTITY();";
                        using SqlCommand optionCmd = new SqlCommand(optionQuery, conn, tran);

                        optionCmd.Parameters.AddWithValue("@TemplateId", option.TemplateId);
                        optionCmd.Parameters.AddWithValue("@SectionId", option.SectionId);
                        optionCmd.Parameters.AddWithValue("@ClinicId", option.ClinicId);
                        optionCmd.Parameters.AddWithValue("@FieldId", option.FieldId);
                        optionCmd.Parameters.AddWithValue("@OptionText", option.OptionText);
                        optionCmd.Parameters.AddWithValue("@OptionValue", option.OptionValue);
                        optionCmd.Parameters.AddWithValue("@IsRequired", option.IsRequired);
                        optionCmd.Parameters.AddWithValue("@DisplayOrder", option.DisplayOrder);

                        int optionId = Convert.ToInt32(optionCmd.ExecuteScalar());
                    }

                }

            }

            return templateId;
        }

        public bool Update(PrescriptionTemplate template, SqlConnection conn, SqlTransaction tran)
        {
            int templateRowsAffected = 0;
            int sectionRowsAffected = 0;
            int fieldRowsAffected = 0;
            int optionRowsAffected = 0;

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

            foreach (var section in template.Sections)
            {
                section.TemplateId = template.TemplateId;
                section.ClinicId = template.ClinicId;
                
                string sectionQuery = @"UPDATE PrescriptionTemplateSections
                                            SET TemplateId = @TemplateId, 
                                                ClinicId = @ClinicId, 
                                                SectionName = @SectionName, 
                                                DisplayOrder = @DisplayOrder
                                            WHERE SectionId = @SectionId";
                using SqlCommand sectionCmd = new SqlCommand(sectionQuery, conn, tran);

                sectionCmd.Parameters.AddWithValue("@TemplateId", section.TemplateId);
                sectionCmd.Parameters.AddWithValue("@ClinicId", section.ClinicId);
                sectionCmd.Parameters.AddWithValue("@SectionName", section.SectionName);
                sectionCmd.Parameters.AddWithValue("@DisplayOrder", section.DisplayOrder);
                sectionCmd.Parameters.AddWithValue("@SectionId", section.SectionId);

                sectionRowsAffected += templateCmd.ExecuteNonQuery();

                foreach (var field in section.Fields)
                {
                    field.TemplateId = section.TemplateId;
                    field.SectionId = section.SectionId;
                    field.ClinicId = template.ClinicId;
                    
                    string fieldQuery = @"UPDATE PrescriptionTemplateFields
                                            SET TemplateId = @TemplateId,
                                                SectionId = @SectionId, 
                                                ClinicId = @ClinicId, 
                                                FieldLabel = @FieldLabel, 
                                                FieldType = @FieldType, 
                                                IsRequired = @IsRequired, 
                                                DisplayOrder = @DisplayOrder 
                                            WHERE FieldId = @FieldId";
                    using SqlCommand fieldCmd = new SqlCommand(fieldQuery, conn, tran);

                    fieldCmd.Parameters.AddWithValue("@TemplateId", field.TemplateId);
                    fieldCmd.Parameters.AddWithValue("@SectionId", field.SectionId);
                    fieldCmd.Parameters.AddWithValue("@ClinicId", field.ClinicId);
                    fieldCmd.Parameters.AddWithValue("@FieldLabel", field.FieldLabel);
                    fieldCmd.Parameters.AddWithValue("@FieldType", field.FieldTypeCode);
                    fieldCmd.Parameters.AddWithValue("@IsRequired", field.IsRequired);
                    fieldCmd.Parameters.AddWithValue("@DisplayOrder", field.DisplayOrder);
                    fieldCmd.Parameters.AddWithValue("@FieldId", field.FieldId);

                    fieldRowsAffected += fieldCmd.ExecuteNonQuery();

                    foreach (var option in field.Options)
                    {
                        option.TemplateId = section.TemplateId;
                        option.SectionId = section.SectionId;
                        option.FieldId = field.FieldId;
                        option.ClinicId = template.ClinicId;
                        
                        string optionQuery = @"UPDATE PrescriptionTemplateFieldOptions
                                                SET TemplateId = @TemplateId,
                                                    SectionId = @SectionId,
                                                    FieldId = @FieldId, 
                                                    ClinicId = @ClinicId, 
                                                    OptionText = @OptionText, 
                                                    OptionValue = @OptionValue, 
                                                    IsRequired = @IsRequired, 
                                                    DisplayOrder = @DisplayOrder 
                                                WHERE OptionId = @OptionId";
                        using SqlCommand optionCmd = new SqlCommand(optionQuery, conn, tran);

                        optionCmd.Parameters.AddWithValue("@TemplateId", option.TemplateId);
                        optionCmd.Parameters.AddWithValue("@SectionId", option.SectionId);
                        optionCmd.Parameters.AddWithValue("@ClinicId", option.ClinicId);
                        optionCmd.Parameters.AddWithValue("@FieldId", option.FieldId);
                        optionCmd.Parameters.AddWithValue("@OptionText", option.OptionText);
                        optionCmd.Parameters.AddWithValue("@OptionValue", option.OptionValue);
                        optionCmd.Parameters.AddWithValue("@IsRequired", option.IsRequired);
                        optionCmd.Parameters.AddWithValue("@DisplayOrder", option.DisplayOrder);
                        optionCmd.Parameters.AddWithValue("@FieldId", option.FieldId);

                        optionRowsAffected += optionCmd.ExecuteNonQuery();
                    }

                }

            }

            return templateRowsAffected > 0 || sectionRowsAffected > 0 || fieldRowsAffected > 0 || optionRowsAffected > 0;
        }

        public bool Delete(int templateId, SqlConnection conn, SqlTransaction tran)
        {
            int optionRowsAffected = 0;
            int fieldRowsAffected = 0;
            int sectionRowsAffected = 0;
            int templateRowsAffected = 0;

            var model = Get(templateId, conn, tran);

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
                templateRowsAffected = sectionCmd.ExecuteNonQuery();

            }

            return templateRowsAffected > 0 || sectionRowsAffected > 0 || fieldRowsAffected > 0 || optionRowsAffected > 0;

        }
    }

}
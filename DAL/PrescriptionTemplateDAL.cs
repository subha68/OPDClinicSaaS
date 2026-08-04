using Microsoft.Data.SqlClient;
using Models;
using ViewModels;

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
                    IsActive = (bool)templateReader["IsActive"],
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
                                    WHERE t.DepartmentId = @DepartmentId";
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
                        DisplayOrder = (int)sectionReader["DisplayOrder"],
                        IsActive = (bool)sectionReader["IsActive"],
                        Multiline = (bool)sectionReader["Multiline"],
                        IsPharmacologyAdvice = (bool)sectionReader["IsPharmacologyAdvice"]
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
                        ClinicId = (int)fieldReader["ClinicId"],
                        SectionId = (int)fieldReader["SectionId"],
                        FieldId = (int)fieldReader["FieldId"],
                        FieldLabel = fieldReader["FieldLabel"].ToString(),
                        FieldTypeCode = fieldReader["FieldTypeCode"].ToString(),
                        IsActive = (bool)fieldReader["IsActive"],
                        DisplayOrder = (int)fieldReader["DisplayOrder"],
                        IsArchived = (bool)fieldReader["IsArchived"]
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
                        IsActive = (bool)fieldReader["IsActive"],
                        DisplayOrder = (int)fieldReader["DisplayOrder"],
                        IsArchived = (bool)fieldReader["IsArchived"]
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
                        ClinicId = (int)optionReader["ClinicId"],
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
                             (TemplateId, ClinicId, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice) 
                             VALUES 
                             (@TemplateId, @ClinicId, @SectionName, @DisplayOrder, @IsActive, @Multiline, @IsPharmacologyAdvice);
                                SELECT SCOPE_IDENTITY();";
                using SqlCommand sectionCmd = new SqlCommand(sectionQuery, conn, tran);

                sectionCmd.Parameters.AddWithValue("@TemplateId", section.TemplateId);
                sectionCmd.Parameters.AddWithValue("@ClinicId", section.ClinicId);
                sectionCmd.Parameters.AddWithValue("@SectionName", section.SectionName);
                sectionCmd.Parameters.AddWithValue("@DisplayOrder", section.DisplayOrder);
                sectionCmd.Parameters.AddWithValue("@IsActive", section.IsActive);
                sectionCmd.Parameters.AddWithValue("@Multiline", section.Multiline);
                sectionCmd.Parameters.AddWithValue("@IsPharmacologyAdvice", section.IsPharmacologyAdvice);

                int sectionId = Convert.ToInt32(sectionCmd.ExecuteScalar());

                foreach (var field in section.Fields)
                {
                    field.TemplateId = templateId;
                    field.SectionId = sectionId;
                    field.ClinicId = template.ClinicId;
                    
                    string fieldQuery = @"INSERT INTO PrescriptionTemplateFields
                                (TemplateId, SectionId, ClinicId, FieldLabel, FieldType, IsActive, DisplayOrder, IsArchived) 
                                VALUES 
                                (@TemplateId, @SectionId, @ClinicId, @FieldLabel, @FieldType, @IsActive, @DisplayOrder, 0);
                                    SELECT SCOPE_IDENTITY();";
                    using SqlCommand fieldCmd = new SqlCommand(fieldQuery, conn, tran);

                    fieldCmd.Parameters.AddWithValue("@TemplateId", field.TemplateId);
                    fieldCmd.Parameters.AddWithValue("@SectionId", field.SectionId);
                    fieldCmd.Parameters.AddWithValue("@ClinicId", field.ClinicId);
                    fieldCmd.Parameters.AddWithValue("@FieldLabel", field.FieldLabel);
                    fieldCmd.Parameters.AddWithValue("@FieldType", field.FieldTypeCode);
                    fieldCmd.Parameters.AddWithValue("@IsActive", field.IsActive);
                    fieldCmd.Parameters.AddWithValue("@DisplayOrder", field.DisplayOrder);

                    int fieldId = Convert.ToInt32(fieldCmd.ExecuteScalar());

                    foreach (var option in field.Options)
                    {
                        option.TemplateId = templateId;
                        option.SectionId = sectionId;
                        option.FieldId = fieldId;
                        option.ClinicId = template.ClinicId;
                        
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
                                                DisplayOrder = @DisplayOrder,
                                                IsActive = @IsActive,
                                                Multiline = @Multiline,
                                                IsPharmacologyAdvice = @IsPharmacologyAdvice
                                            WHERE SectionId = @SectionId";
                using SqlCommand sectionCmd = new SqlCommand(sectionQuery, conn, tran);

                sectionCmd.Parameters.AddWithValue("@TemplateId", section.TemplateId);
                sectionCmd.Parameters.AddWithValue("@ClinicId", section.ClinicId);
                sectionCmd.Parameters.AddWithValue("@SectionName", section.SectionName);
                sectionCmd.Parameters.AddWithValue("@DisplayOrder", section.DisplayOrder);
                sectionCmd.Parameters.AddWithValue("@SectionId", section.SectionId);
                sectionCmd.Parameters.AddWithValue("@IsActive", section.IsActive);
                sectionCmd.Parameters.AddWithValue("@Multiline", section.Multiline);
                sectionCmd.Parameters.AddWithValue("@IsPharmacologyAdvice", section.IsPharmacologyAdvice);

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
                                                IsActive = @IsActive,
                                                IsArchived = @IsArchived,
                                                DisplayOrder = @DisplayOrder 
                                            WHERE FieldId = @FieldId";
                    using SqlCommand fieldCmd = new SqlCommand(fieldQuery, conn, tran);

                    fieldCmd.Parameters.AddWithValue("@TemplateId", field.TemplateId);
                    fieldCmd.Parameters.AddWithValue("@SectionId", field.SectionId);
                    fieldCmd.Parameters.AddWithValue("@ClinicId", field.ClinicId);
                    fieldCmd.Parameters.AddWithValue("@FieldLabel", field.FieldLabel);
                    fieldCmd.Parameters.AddWithValue("@FieldType", field.FieldTypeCode);
                    fieldCmd.Parameters.AddWithValue("@IsActive", field.IsActive);
                    fieldCmd.Parameters.AddWithValue("@IsArchived", field.IsArchived);
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
                        IsPharmacologyAdvice = (bool)sectionReader["IsPharmacologyAdvice"] 
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
                                            f.FieldLabel, f.FieldTypeCode, f.IsActive, f.DisplayOrder, f.IsRequired";
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
                        OptionCount = (int)fieldReader["OptionCount"]
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
                                    (TemplateId, ClinicId, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice)
                                    VALUES
                                    (@TemplateId, @ClinicId, @SectionName, @CurrentOrder, 1, 0, 0);
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
                                    (TemplateId, ClinicId, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice)
                                    VALUES
                                    (@TemplateId, @ClinicId, @SectionName, @CurrentOrder + 1, 1, 0, 0);
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
                                t.TemplateId,
                                t.TemplateName,
                                t.DepartmentId,
                                d.Name AS DepartmentName
                            FROM PrescriptionTemplates t
                            INNER JOIN Clinics c
                                ON (t.ClinicId = c.ClinicId
                                AND c.Name = 'Library')
                            LEFT JOIN Departments d
                                ON t.DepartmentId = d.DepartmentId
                            WHERE t.TemplateName LIKE '%' + @term + '%'
                            AND t.IsActive = 1
                            ORDER BY t.TemplateName";

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
                    DepartmentId = reader["DepartmentId"] != DBNull.Value ? (int)reader["DepartmentId"] : 0,
                    DepartmentName = reader["DepartmentName"]?.ToString() ?? ""
                });
            }

            return list;
        }

        public bool SaveSectionOptions(int templateId, int sectionId, bool isMultiline, bool isRx, SqlConnection conn, SqlTransaction tran)
        {
            int rowsAffected = 0;

            string query = @"UPDATE PrescriptionTemplateSections
                                        SET Multiline = @IsMultiline,
                                            IsPharmacologyAdvice = @IsRx
                                        WHERE TemplateId = @TemplateId
                                        AND SectionId = @SectionId;";
            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TemplateId", templateId);
            cmd.Parameters.AddWithValue("@SectionId", sectionId);
            cmd.Parameters.AddWithValue("@IsMultiline", isMultiline);
            cmd.Parameters.AddWithValue("@IsRx", isRx);

            rowsAffected = cmd.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public bool UpdateFieldsArchival(int templateId, int sectionId, bool isArchived, SqlConnection conn, SqlTransaction tran)
        {
            int rowsAffected = 0;

            string query = @"UPDATE PrescriptionTemplateFields
                                        SET IsArchived = @IsArchived
                                        WHERE TemplateId = @TemplateId
                                        AND SectionId = @SectionId;";
            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TemplateId", templateId);
            cmd.Parameters.AddWithValue("@SectionId", sectionId);
            cmd.Parameters.AddWithValue("@IsArchived", isArchived);

            rowsAffected = cmd.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public bool UpdateOptionsArchival(int templateId, int sectionId, bool isArchived, SqlConnection conn, SqlTransaction tran)
        {
            int rowsAffected = 0;

            string query = @"UPDATE PrescriptionTemplateFieldOptions
                                        SET IsArchived = @IsArchived
                                        WHERE TemplateId = @TemplateId
                                        AND SectionId = @SectionId;";
            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TemplateId", templateId);
            cmd.Parameters.AddWithValue("@SectionId", sectionId);
            cmd.Parameters.AddWithValue("@IsArchived", isArchived);

            rowsAffected = cmd.ExecuteNonQuery();

            return rowsAffected > 0;
        }



    }

}
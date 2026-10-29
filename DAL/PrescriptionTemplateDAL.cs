using Microsoft.Data.SqlClient;
using Models;
using ViewModels;

namespace DAL
{
    public class PrescriptionTemplateDAL : BaseDAL, IDAL<PrescriptionTemplate>
    {
        private readonly DbHelper _db;

        private const string TemplateScopePredicate = @"
            EXISTS
            (
                SELECT 1
                FROM PrescriptionTemplates t
                WHERE t.TemplateId = @TemplateId
                AND
                (
                    (@ClinicId IS NULL AND t.ClinicId IS NULL)
                    OR t.ClinicId = @ClinicId
                )
            )";

        private const string TemplateClinicScopePredicate = @"
        (
            ((@ClinicId IS NULL AND t.ClinicId IS NULL)
            OR t.ClinicId = @ClinicId)
        )";

        private const string TemplateClinicDepartmentDoctorScopePredicate = @"
        (
            ((@ClinicId IS NULL AND t.ClinicId IS NULL)
              OR t.ClinicId = @ClinicId)
            AND
            ((@DepartmentId IS NULL AND t.DepartmentId IS NULL)
              OR t.DepartmentId = @DepartmentId)
            AND
            ((@DoctorId IS NULL AND t.DoctorId IS NULL)
              OR t.DoctorId = @DoctorId)
        )";


        private const string CustomizedValueSql = @"
            CASE
                WHEN @ClinicId IS NULL OR @ClinicId = 0 THEN 0
                ELSE 1
            END";

        public PrescriptionTemplateDAL(IConfiguration configuration)
        {
            _db = new DbHelper(configuration);
        }

        public PrescriptionTemplate Get(int templateId, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            return Get(templateId, (int?)clinicId, conn, tran);
        }

        public List<PrescriptionTemplate> GetAll(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            return GetAll((int?)clinicId, conn, tran);
        }

        public List<PrescriptionTemplate> GetAll(int? clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            // only template is retrieved

            List<PrescriptionTemplate> templates = new List<PrescriptionTemplate>();

            string templateQuery = $@"SELECT t.*, 
                                    d.Name as DepartmentName, 
                                    dr.Salutation + ' ' + dr.FirstName + ' ' + dr.LastName as DoctorName 
                                    FROM PrescriptionTemplates t
                                    LEFT JOIN Departments d
                                        ON t.DepartmentId = d.DepartmentId
                                    LEFT JOIN Doctors dr
                                        ON t.DoctorId = dr.DoctorId
                                    WHERE {TemplateClinicScopePredicate}";
            using SqlCommand templateCmd = new SqlCommand(templateQuery, conn);
            if (tran != null)
            {
                templateCmd.Transaction = tran;
            }

            templateCmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId.Value : DBNull.Value);

            using var templateReader = templateCmd.ExecuteReader();

            while (templateReader.Read())
            {
                templates.Add(MapTemplate(templateReader));
            }

            return templates;
        }

        public PrescriptionTemplate Get(int templateId, int? clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            PrescriptionTemplate template = null;

            // retrieve template

            string templateQuery = $@"SELECT t.*, 
                                    d.Name as DepartmentName, 
                                    dr.Salutation + ' ' + dr.FirstName + ' ' + dr.LastName as DoctorName 
                                    FROM PrescriptionTemplates t
                                    LEFT JOIN Departments d
                                        ON t.DepartmentId = d.DepartmentId
                                    LEFT JOIN Doctors dr
                                        ON t.DoctorId = dr.DoctorId
                                    WHERE TemplateId = @TemplateId
                                    AND {TemplateScopePredicate}";

            using SqlCommand templateCmd = new SqlCommand(templateQuery, conn);
            if (tran != null)
            {
                templateCmd.Transaction = tran;
            }

            templateCmd.Parameters.AddWithValue("@TemplateId", templateId);
            templateCmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId.Value : DBNull.Value);

            using (var templateReader = templateCmd.ExecuteReader())
            {
                if (templateReader.Read())
                {
                    template = MapTemplate(templateReader);
                }
            }

            if (template == null)
                return null; // Not found

            // Now retrieve sections & fields
            template.Sections = GetSectionsAndFields(template.TemplateId, conn, tran);

            return template;
        }

        public PrescriptionTemplate GetTemplateWithAllSections(int templateId, int? clinicId, 
                                                                SqlConnection conn, SqlTransaction? tran = null)
        {
            PrescriptionTemplate template = null;

            // retrieve template

            string templateQuery = $@"SELECT t.*, 
                                    d.Name as DepartmentName, 
                                    dr.Salutation + ' ' + dr.FirstName + ' ' + dr.LastName as DoctorName 
                                    FROM PrescriptionTemplates t
                                    LEFT JOIN Departments d
                                        ON t.DepartmentId = d.DepartmentId
                                    LEFT JOIN Doctors dr
                                        ON t.DoctorId = dr.DoctorId
                                    WHERE TemplateId = @TemplateId
                                    AND {TemplateScopePredicate}";

            using SqlCommand templateCmd = new SqlCommand(templateQuery, conn);
            if (tran != null)
            {
                templateCmd.Transaction = tran;
            }

            templateCmd.Parameters.AddWithValue("@TemplateId", templateId);
            templateCmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId.Value : DBNull.Value);

            using (var templateReader = templateCmd.ExecuteReader())
            {
                if (templateReader.Read())
                {
                    template = MapTemplate(templateReader);
                }
            }

            if (template == null)
                return null; // Not found

            // Now retrieve sections & fields
            template.Sections = GetAllSectionsAndFields(template.TemplateId, conn, tran);

            return template;
        }

        public PrescriptionTemplate GetByClinic(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            PrescriptionTemplate template = null;

            // retrieve template

            string templateQuery = @"SELECT t.*, 
                                    d.Name as DepartmentName, 
                                    dr.Salutation + ' ' + dr.FirstName + ' ' + dr.LastName as DoctorName
                                    FROM PrescriptionTemplates t 
                                    LEFT JOIN Departments d
                                        ON t.DepartmentId = d.DepartmentId
                                    LEFT JOIN Doctors dr
                                        ON t.DoctorId = dr.DoctorId
                                    WHERE t.ClinicId = @ClinicId
                                    AND (t.DepartmentId IS NULL OR t.DepartmentId = 0)
                                    AND (t.DoctorId IS NULL OR t.DoctorId = 0)
                                    AND TemplateCode = 'DEFAULT'";

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
                    template = MapTemplate(templateReader);
                }
            }

            if (template == null)
                return null; // Not found

            // Now retrieve sections & fields
            template.Sections = GetAllSectionsAndFields(template.TemplateId, conn, tran);

            return template;
        }

        public PrescriptionTemplate GetDefault(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            PrescriptionTemplate template = null;

            // retrieve template

            string templateQuery = $@"SELECT t.*, 
                                    d.Name as DepartmentName, 
                                    dr.Salutation + ' ' + dr.FirstName + ' ' + dr.LastName as DoctorName 
                                    FROM PrescriptionTemplates t
                                    LEFT JOIN Departments d
                                        ON t.DepartmentId = d.DepartmentId 
                                    LEFT JOIN Doctors dr
                                        ON t.DoctorId = dr.DoctorId
                                    WHERE (t.DepartmentId IS NULL OR t.DepartmentId = 0)
                                    AND (t.DoctorId IS NULL OR t.DoctorId = 0)
                                    AND t.ClinicId = @ClinicId
                                    AND t.TemplateCode = 'DEFAULT'";

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
                    template = MapTemplate(templateReader);
                }
            }

            if (template == null)
                return null; // Not found

            // Now retrieve sections & fields
            template.Sections = GetAllSectionsAndFields(template.TemplateId, conn, tran);

            return template;
        }

        public PrescriptionTemplate GetByDepartment(int departmentId, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            PrescriptionTemplate template = null;

            // retrieve template

            string templateQuery = $@"SELECT t.*, 
                                    d.Name as DepartmentName, 
                                    dr.Salutation + ' ' + dr.FirstName + ' ' + dr.LastName as DoctorName 
                                    FROM PrescriptionTemplates t
                                    INNER JOIN Departments d
                                        ON t.DepartmentId = d.DepartmentId 
                                    LEFT JOIN Doctors dr
                                        ON t.DoctorId = dr.DoctorId
                                    WHERE t.DepartmentId = @DepartmentId
                                    AND (t.DoctorId IS NULL OR t.DoctorId = 0)
                                    AND t.ClinicId = @ClinicId";

            using SqlCommand templateCmd = new SqlCommand(templateQuery, conn);
            if (tran != null)
            {
                templateCmd.Transaction = tran;
            }

            templateCmd.Parameters.AddWithValue("@DepartmentId", departmentId);
            templateCmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using (var templateReader = templateCmd.ExecuteReader())
            {

                if (templateReader.Read())
                {
                    template = MapTemplate(templateReader);
                }
            }

            if (template == null)
                return null; // Not found

            // Now retrieve sections & fields
            template.Sections = GetAllSectionsAndFields(template.TemplateId, conn, tran);

            return template;
        }

        public PrescriptionTemplate GetByDoctor(int doctorId, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            PrescriptionTemplate template = null;

            // retrieve template

            string templateQuery = @"SELECT t.*, 
                                    d.Name as DepartmentName, 
                                    dr.Salutation + ' ' + dr.FirstName + ' ' + dr.LastName as DoctorName 
                                    FROM PrescriptionTemplates t
                                    INNER JOIN Departments d
                                        ON t.DepartmentId = d.DepartmentId 
                                    INNER JOIN Doctors dr
                                        ON t.DoctorId = dr.DoctorId
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
                    template = MapTemplate(templateReader);
                }
            }

            if (template == null)
                return null; // Not found

            // Now retrieve sections & fields
            template.Sections = GetAllSectionsAndFields(template.TemplateId, conn, tran);

            return template;
        }

        public PrescriptionTemplate GetByCode(string templateCode, int? clinicId, int? departmentId, int? doctorId,
                                                SqlConnection conn, SqlTransaction? tran = null)
        {
            PrescriptionTemplate template = null;

            // retrieve template

            string templateQuery = $@"SELECT t.*, 
                                    d.Name as DepartmentName, 
                                    dr.Salutation + ' ' + dr.FirstName + ' ' + dr.LastName as DoctorName
                                    FROM PrescriptionTemplates t
                                    LEFT JOIN Departments d
                                        ON t.DepartmentId = d.DepartmentId
                                    LEFT JOIN Doctors dr
                                        ON t.DoctorId = dr.DoctorId
                                    WHERE TemplateCode = @TemplateCode
                                    AND {TemplateClinicDepartmentDoctorScopePredicate}";

            using SqlCommand templateCmd = new SqlCommand(templateQuery, conn);
            if (tran != null)
            {
                templateCmd.Transaction = tran;
            }

            templateCmd.Parameters.AddWithValue("@TemplateCode", templateCode);
            templateCmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId.Value : DBNull.Value);
            templateCmd.Parameters.AddWithValue("@DepartmentId", departmentId.HasValue ? departmentId.Value : DBNull.Value);
            templateCmd.Parameters.AddWithValue("@DoctorId", doctorId.HasValue ? doctorId.Value : DBNull.Value);

            using (var templateReader = templateCmd.ExecuteReader())
            {
                if (templateReader.Read())
                {
                    template = MapTemplate(templateReader);
                }
            }

            if (template == null)
                return null; // Not found

            // Now retrieve sections & fields
            template.Sections = GetAllSectionsAndFields(template.TemplateId, conn, tran);

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

        public List<PrescriptionTemplateSection> GetAllSectionsAndFields(int templateId, 
                                                                        SqlConnection conn, SqlTransaction? tran = null)
        {
            List<PrescriptionTemplateSection> sections = GetAllSections(templateId, conn, tran);

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
                                        AND IsArchived = 0
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
                    sections.Add(MapSection(sectionReader));
                }
            }

            return sections;
        }

        public List<PrescriptionTemplateSection> GetAllSections(int templateId, SqlConnection conn, SqlTransaction? tran = null)
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
                    sections.Add(MapSection(sectionReader));
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
                    fields.Add(MapField(fieldReader));
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
                    fields.Add(MapField(fieldReader));
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
                    fields.Add(MapField(fieldReader));
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
                    options.Add(MapOption(optionReader));
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
                    options.Add(MapOption(optionReader));
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
                    options.Add(MapOption(optionReader));
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

            templateCmd.Parameters.AddWithValue("@ClinicId", template.ClinicId.HasValue 
                                                                    ? template.ClinicId.Value : DBNull.Value);
            templateCmd.Parameters.AddWithValue("@DepartmentId", template.DepartmentId.HasValue 
                                                                    ? template.DepartmentId.Value : DBNull.Value);
            templateCmd.Parameters.AddWithValue("@DoctorId", template.DoctorId.HasValue 
                                                                    ? template.DoctorId.Value : DBNull.Value);
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
                             (TemplateId, ClinicId, SectionName, DisplayOrder, IsActive, IsArchived, Multiline, 
                             IsPharmacologyAdvice, IsVital, IsTestResult, SourceSectionId, IsCustomized) 
                             VALUES 
                             (@TemplateId, @ClinicId, @SectionName, @DisplayOrder, @IsActive, @IsArchived, @Multiline, 
                             @IsPharmacologyAdvice, @IsVital, @IsTestResult, @SourceSectionId, @IsCustomized);
                                SELECT SCOPE_IDENTITY();";
                using SqlCommand sectionCmd = new SqlCommand(sectionQuery, conn, tran);

                sectionCmd.Parameters.AddWithValue("@TemplateId", section.TemplateId);
                sectionCmd.Parameters.AddWithValue("@ClinicId", section.ClinicId.HasValue 
                                                                            ? section.ClinicId.Value : 
                                                                            DBNull.Value);
                sectionCmd.Parameters.AddWithValue("@SourceSectionId", section.SourceSectionId.HasValue 
                                                                            ? section.SourceSectionId.Value : 
                                                                            DBNull.Value);

                sectionCmd.Parameters.AddWithValue("@SectionName", section.SectionName);
                sectionCmd.Parameters.AddWithValue("@DisplayOrder", section.DisplayOrder);
                sectionCmd.Parameters.AddWithValue("@IsActive", section.IsActive ? 1 : 0);
                sectionCmd.Parameters.AddWithValue("@IsArchived", section.IsArchived ? 1 : 0);
                sectionCmd.Parameters.AddWithValue("@Multiline", section.Multiline ? 1 : 0);
                sectionCmd.Parameters.AddWithValue("@IsPharmacologyAdvice", section.IsPharmacologyAdvice ? 1 : 0);
                sectionCmd.Parameters.AddWithValue("@IsVital", section.IsVital ? 1 : 0);
                sectionCmd.Parameters.AddWithValue("@IsTestResult", section.IsTestResult ? 1 : 0);
                sectionCmd.Parameters.AddWithValue("@IsCustomized", section.IsCustomized ? 1 : 0);

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
                            (TemplateId, SectionId, ClinicId, SourceFieldId, FieldLabel, FieldTypeCode, FieldCode, IsActive, 
                                DisplayOrder, IsArchived, FieldSource, MergeWith, IsCustomized) 
                            VALUES 
                            (@TemplateId, @SectionId, @ClinicId, @SourceFieldId, @FieldLabel, @FieldTypeCode, @FieldCode, @IsActive, 
                                @DisplayOrder, @IsArchived, @FieldSource, @MergeWith, @IsCustomized);
                                SELECT SCOPE_IDENTITY();";
                using SqlCommand fieldCmd = new SqlCommand(fieldQuery, conn, tran);

                fieldCmd.Parameters.AddWithValue("@TemplateId", field.TemplateId);
                fieldCmd.Parameters.AddWithValue("@SectionId", field.SectionId);
                fieldCmd.Parameters.AddWithValue("@ClinicId", field.ClinicId.HasValue 
                                                                    ? field.ClinicId.Value : DBNull.Value);
                fieldCmd.Parameters.AddWithValue("@SourceFieldId", field.SourceFieldId.HasValue
                                                                    ? field.SourceFieldId.Value : DBNull.Value);
                fieldCmd.Parameters.AddWithValue("@FieldLabel", field.FieldLabel);
                fieldCmd.Parameters.AddWithValue("@FieldTypeCode", field.FieldTypeCode);
                fieldCmd.Parameters.AddWithValue("@FieldCode", field.FieldCode == null ? DBNull.Value : field.FieldCode);
                fieldCmd.Parameters.AddWithValue("@IsActive", field.IsActive);
                fieldCmd.Parameters.AddWithValue("@DisplayOrder", field.DisplayOrder);
                fieldCmd.Parameters.AddWithValue("@IsArchived", field.IsArchived ? 1 : 0);
                fieldCmd.Parameters.AddWithValue("@FieldSource", field.FieldSource == null ? DBNull.Value : field.FieldSource);
                fieldCmd.Parameters.AddWithValue("@MergeWith", field.MergeWith == null ? DBNull.Value : field.MergeWith);
                fieldCmd.Parameters.AddWithValue("@IsCustomized", field.IsCustomized);

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
                            (TemplateId, SectionId, ClinicId, FieldId, OptionText, IsActive, DisplayOrder, IsArchived, 
                            SourceOptionId, IsCustomized) 
                            VALUES 
                            (@TemplateId, @SectionId, @ClinicId, @FieldId, @OptionText, @IsActive, @DisplayOrder, @IsArchived, 
                            @SourceOptionId, @IsCustomized);
                                SELECT SCOPE_IDENTITY();";
                using SqlCommand optionCmd = new SqlCommand(optionQuery, conn, tran);

                optionCmd.Parameters.AddWithValue("@TemplateId", option.TemplateId);
                optionCmd.Parameters.AddWithValue("@SectionId", option.SectionId);
                optionCmd.Parameters.AddWithValue("@ClinicId", option.ClinicId.HasValue 
                                                                            ? option.ClinicId.Value 
                                                                            : DBNull.Value);
                optionCmd.Parameters.AddWithValue("@FieldId", option.FieldId);
                optionCmd.Parameters.AddWithValue("@OptionText", option.OptionText);
                optionCmd.Parameters.AddWithValue("@IsActive", option.IsActive);
                optionCmd.Parameters.AddWithValue("@DisplayOrder", option.DisplayOrder);
                optionCmd.Parameters.AddWithValue("@SourceOptionId", option.SourceOptionId.HasValue 
                                                                            ? option.SourceOptionId.Value 
                                                                            : DBNull.Value);
                optionCmd.Parameters.AddWithValue("@IsArchived", option.IsArchived ? 1 : 0);
                optionCmd.Parameters.AddWithValue("@IsCustomized", option.IsCustomized ? 1 : 0);

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
                                        DoctorId = @DoctorId,
                                        SourceTemplateId = @SourceTemplateId,
                                        TemplateCode = @TemplateCode,
                                        TemplateName = @TemplateName, 
                                        IsActive = @IsActive
                                    WHERE TemplateId = @TemplateId";
            using SqlCommand templateCmd = new SqlCommand(templateQuery, conn, tran);

            templateCmd.Parameters.AddWithValue("@ClinicId", template.ClinicId.HasValue 
                                                                            ? template.ClinicId.Value 
                                                                            : DBNull.Value);
            templateCmd.Parameters.AddWithValue("@DepartmentId", template.DepartmentId.HasValue
                                                                            ? template.DepartmentId.Value
                                                                            : DBNull.Value);
            templateCmd.Parameters.AddWithValue("@DoctorId", template.DoctorId.HasValue
                                                                            ? template.DoctorId.Value
                                                                            : DBNull.Value);
            templateCmd.Parameters.AddWithValue("@SourceTemplateId", template.SourceTemplateId.HasValue
                                                                            ? template.SourceTemplateId.Value
                                                                            : DBNull.Value);
            templateCmd.Parameters.AddWithValue("@TemplateCode", template.TemplateCode);
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
                                            SET ClinicId = @ClinicId, 
                                                SectionName = @SectionName, 
                                                DisplayOrder = @DisplayOrder,
                                                IsActive = @IsActive,
                                                IsArchived = @IsArchived,
                                                Multiline = @Multiline,
                                                IsPharmacologyAdvice = @IsPharmacologyAdvice,
                                                IsVital = @IsVital,
                                                IsTestResult = @IsTestResult,
                                                SourceSectionId = @SourceSectionId,
                                                IsCustomized = @IsCustomized
                                            WHERE SectionId = @SectionId
                                            AND TemplateId = @TemplateId";
                using SqlCommand sectionCmd = new SqlCommand(sectionQuery, conn, tran);

                sectionCmd.Parameters.AddWithValue("@TemplateId", section.TemplateId);
                sectionCmd.Parameters.AddWithValue("@ClinicId", section.ClinicId.HasValue 
                                                                            ? section.ClinicId.Value 
                                                                            : DBNull.Value);
                sectionCmd.Parameters.AddWithValue("@SectionName", section.SectionName);
                sectionCmd.Parameters.AddWithValue("@DisplayOrder", section.DisplayOrder);
                sectionCmd.Parameters.AddWithValue("@SectionId", section.SectionId);
                sectionCmd.Parameters.AddWithValue("@IsActive", section.IsActive ? 1 : 0);
                sectionCmd.Parameters.AddWithValue("@IsArchived", section.IsArchived ? 1 : 0);
                sectionCmd.Parameters.AddWithValue("@Multiline", section.Multiline ? 1 : 0);
                sectionCmd.Parameters.AddWithValue("@IsPharmacologyAdvice", section.IsPharmacologyAdvice ? 1 : 0);
                sectionCmd.Parameters.AddWithValue("@IsVital", section.IsVital ? 1 : 0);
                sectionCmd.Parameters.AddWithValue("@IsTestResult", section.IsTestResult ? 1 : 0);
                sectionCmd.Parameters.AddWithValue("@SourceSectionId", section.SourceSectionId.HasValue 
                                                                            ? section.SourceSectionId.Value 
                                                                            : DBNull.Value);
                sectionCmd.Parameters.AddWithValue("@IsCustomized", section.IsCustomized ? 1 : 0);            

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
                                        SET ClinicId = @ClinicId, 
                                            FieldLabel = @FieldLabel, 
                                            FieldTypeCode = @FieldTypeCode, 
                                            FieldCode = @FieldCode, 
                                            IsActive = @IsActive,
                                            IsArchived = @IsArchived,
                                            DisplayOrder = @DisplayOrder,
                                            FieldSource = @FieldSource,
                                            MergeWith = @MergeWith,
                                            SourceFieldId = @SourceFieldId,
                                            IsCustomized = @IsCustomized
                                        WHERE FieldId = @FieldId
                                        AND TemplateId = @TemplateId
                                        AND SectionId = @SectionId;";
                using SqlCommand fieldCmd = new SqlCommand(fieldQuery, conn, tran);

                fieldCmd.Parameters.AddWithValue("@TemplateId", field.TemplateId);
                fieldCmd.Parameters.AddWithValue("@SectionId", field.SectionId);
                fieldCmd.Parameters.AddWithValue("@ClinicId", field.ClinicId.HasValue ? field.ClinicId.Value : DBNull.Value);
                fieldCmd.Parameters.AddWithValue("@FieldLabel", field.FieldLabel);
                fieldCmd.Parameters.AddWithValue("@FieldTypeCode", field.FieldTypeCode);
                fieldCmd.Parameters.AddWithValue("@FieldCode", field.FieldCode == null ? DBNull.Value : field.FieldCode);
                fieldCmd.Parameters.AddWithValue("@IsActive", field.IsActive);
                fieldCmd.Parameters.AddWithValue("@IsArchived", field.IsArchived);
                fieldCmd.Parameters.AddWithValue("@DisplayOrder", field.DisplayOrder);
                fieldCmd.Parameters.AddWithValue("@FieldSource", field.FieldSource == null ? DBNull.Value : field.FieldSource);
                fieldCmd.Parameters.AddWithValue("@MergeWith", field.MergeWith == null ? DBNull.Value : field.MergeWith);
                fieldCmd.Parameters.AddWithValue("@FieldId", field.FieldId);
                fieldCmd.Parameters.AddWithValue("@SourceFieldId", field.SourceFieldId.HasValue 
                                                                            ? field.SourceFieldId.Value 
                                                                            : DBNull.Value);
                fieldCmd.Parameters.AddWithValue("@IsCustomized", field.IsCustomized ? 1 : 0);

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
                                        SET ClinicId = @ClinicId, 
                                            OptionText = @OptionText, 
                                            IsActive = @IsActive, 
                                            IsArchived = @IsArchived,
                                            DisplayOrder = @DisplayOrder,
                                            SourceOptionId = @SourceOptionId,
                                            IsCustomized = @IsCustomized
                                        WHERE OptionId = @OptionId
                                        AND TemplateId = @TemplateId
                                        AND SectionId = @SectionId
                                        AND FieldId = @FieldId;";
                using SqlCommand optionCmd = new SqlCommand(optionQuery, conn, tran);

                optionCmd.Parameters.AddWithValue("@TemplateId", option.TemplateId);
                optionCmd.Parameters.AddWithValue("@SectionId", option.SectionId);
                optionCmd.Parameters.AddWithValue("@ClinicId", option.ClinicId.HasValue 
                                                                            ? option.ClinicId.Value 
                                                                            : DBNull.Value);
                optionCmd.Parameters.AddWithValue("@FieldId", option.FieldId);
                optionCmd.Parameters.AddWithValue("@OptionText", option.OptionText);
                optionCmd.Parameters.AddWithValue("@IsActive", option.IsActive);
                optionCmd.Parameters.AddWithValue("@IsArchived", option.IsArchived);
                optionCmd.Parameters.AddWithValue("@DisplayOrder", option.DisplayOrder);
                optionCmd.Parameters.AddWithValue("@SourceOptionId", option.SourceOptionId.HasValue 
                                                                            ? option.SourceOptionId.Value 
                                                                            : DBNull.Value);
                optionCmd.Parameters.AddWithValue("@IsCustomized", option.IsCustomized ? 1 : 0);
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
                string optionQuery = @"DELETE FROM PrescriptionTemplateFieldOptions 
                                        WHERE TemplateId = @TemplateId
                                        AND IsCustomized = 0";
                using SqlCommand optionCmd = new SqlCommand(optionQuery, conn, tran);
                optionCmd.Parameters.AddWithValue("@TemplateId", templateId);
                optionRowsAffected = optionCmd.ExecuteNonQuery();

                // start deletion from Field records
                string fieldQuery = @"DELETE FROM PrescriptionTemplateFields
                                        WHERE TemplateId = @TemplateId
                                        AND IsCustomized = 0
                                        AND NOT EXISTS
                                        (
                                            SELECT 1
                                            FROM PrescriptionTemplateFieldOptions tfo
                                            WHERE PrescriptionTemplateFields.FieldId = tfo.FieldId
                                        );";
                using SqlCommand fieldCmd = new SqlCommand(fieldQuery, conn, tran);
                fieldCmd.Parameters.AddWithValue("@TemplateId", templateId);
                fieldRowsAffected = fieldCmd.ExecuteNonQuery();

                // then delete Section records
                string sectionQuery = @"DELETE FROM PrescriptionTemplateSections
                                        WHERE TemplateId = @TemplateId
                                        AND IsCustomized = 0
                                        AND NOT EXISTS (SELECT 1
                                                        FROM PrescriptionTemplateFields tf
                                                        WHERE PrescriptionTemplateSections.SectionId = tf.SectionId)";
                using SqlCommand sectionCmd = new SqlCommand(sectionQuery, conn, tran);
                sectionCmd.Parameters.AddWithValue("@TemplateId", templateId);
                sectionRowsAffected = sectionCmd.ExecuteNonQuery();

                // and finally delete the Template
                string templateQuery = @"DELETE FROM PrescriptionTemplates
                                            WHERE TemplateId = @TemplateId
                                            AND NOT EXISTS (SELECT 1
                                                            FROM PrescriptionTemplateSections ts
                                                            WHERE PrescriptionTemplates.TemplateId = ts.TemplateId)";
                using SqlCommand templateCmd = new SqlCommand(templateQuery, conn, tran);
                templateCmd.Parameters.AddWithValue("@TemplateId", templateId);
                templateRowsAffected = templateCmd.ExecuteNonQuery();

            }

            return templateRowsAffected > 0; // || sectionRowsAffected > 0 || fieldRowsAffected > 0 || optionRowsAffected > 0;

        }

        public int? GetIdByCode(string templateCode, int? clinicId, int? departmentId, int? doctorId,
                                    SqlConnection conn, SqlTransaction tran)
        {
            int? templateId = null;

            string templateQuery = @"SELECT TemplateId 
                                    FROM PrescriptionTemplates
                                    WHERE TemplateCode = @TemplateCode";
            if (!clinicId.HasValue || clinicId.Value == 0)
                templateQuery += " AND ClinicId IS NULL";
            else
                templateQuery += " AND ClinicId = @ClinicId";

            if (!departmentId.HasValue || departmentId.Value == 0)
                templateQuery += " AND DepartmentId IS NULL";
            else
                templateQuery += " AND DepartmentId = @DepartmentId";

            if (!doctorId.HasValue || doctorId.Value == 0)
                templateQuery += " AND DoctorId IS NULL";
            else
                templateQuery += " AND DoctorId = @DoctorId";

            using SqlCommand templateCmd = new SqlCommand(templateQuery, conn);
            if (tran != null)
            {
                templateCmd.Transaction = tran;
            }

            templateCmd.Parameters.AddWithValue("@TemplateCode", templateCode);

            if (clinicId.HasValue && clinicId.Value != 0)
                templateCmd.Parameters.AddWithValue("@ClinicId", clinicId);

            if (departmentId.HasValue && departmentId.Value != 0)
                templateCmd.Parameters.AddWithValue("@DepartmentId", departmentId);

            if (doctorId.HasValue && doctorId.Value != 0)
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

        public int? CloneTemplate(string templateCode, int srcClinicId, int tgtClinicId, int tgtDepartmentId,
                                                SqlConnection conn, SqlTransaction tran)
        {
            int? templateId = GetIdByCode(templateCode, srcClinicId, 0, 0, conn, tran);

            if (!templateId.HasValue)
                return null;

            PrescriptionTemplate template = GetTemplateWithAllSections(templateId.Value, srcClinicId, conn, tran);

            if (template == null)
                return null;

            template.ClinicId = tgtClinicId;
            template.DepartmentId = tgtDepartmentId;
            template.SourceTemplateId = template.TemplateId;
            //template.TemplateId = 0;

            Add(template, conn, tran);

            foreach (var section in template.Sections)
            {
                section.SourceSectionId = section.SectionId;
                section.TemplateId = template.TemplateId;
                section.ClinicId = template.ClinicId;
                section.IsCustomized = false;
            }

            AddSections(template.Sections, conn, tran);

            foreach (var section in template.Sections)
            {
                foreach (var field in section.Fields)
                {
                    field.SourceFieldId = field.FieldId;
                    field.SectionId = section.SectionId;
                    field.TemplateId = template.TemplateId;
                    field.ClinicId = template.ClinicId;
                    field.IsCustomized = false;
                }

                AddFields(section.Fields, conn, tran);

                foreach (var field in section.Fields)
                {
                    foreach (var option in field.Options)
                    {
                        option.SourceOptionId = option.OptionId;
                        option.FieldId = field.FieldId;
                        option.SectionId = section.SectionId;
                        option.TemplateId = template.TemplateId;
                        option.ClinicId = template.ClinicId;
                        option.IsCustomized = false;
                    }

                    AddOptions(field.Options, conn, tran);
                }
            }
            return template.TemplateId;
        }

        public int? CloneTemplate(int templateId, int srcClinicId, int tgtClinicId, int tgtDepartmentId, 
                                                SqlConnection conn, SqlTransaction tran)
        {
            PrescriptionTemplate template = GetTemplateWithAllSections(templateId, srcClinicId, conn, tran);

            if (template == null)
                return null;

            template.ClinicId = tgtClinicId;
            template.DepartmentId = tgtDepartmentId;
            template.SourceTemplateId = template.TemplateId;
            template.TemplateId = 0;

            Add(template, conn, tran);

            foreach (var section in template.Sections)
            {
                section.SourceSectionId = section.SectionId;
                section.TemplateId = template.TemplateId;
                section.ClinicId = template.ClinicId;
            }

            AddSections(template.Sections, conn, tran);

            foreach (var section in template.Sections)
            {
                foreach (var field in section.Fields)
                {
                    field.SourceFieldId = field.FieldId;
                    field.SectionId = section.SectionId;
                    field.TemplateId = template.TemplateId;
                    field.ClinicId = template.ClinicId;
                }

                AddFields(section.Fields, conn, tran);

                foreach (var field in section.Fields)
                {
                    foreach (var option in field.Options)
                    {
                        option.SourceOptionId = option.OptionId;
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

        public void SaveSectionOrder(List<SectionOrderVM> sections, int? clinicId, 
                                        SqlConnection conn, SqlTransaction tran)
        {
            string query = $@"
                UPDATE PrescriptionTemplateSections
                SET DisplayOrder = @DisplayOrder,
                    IsCustomized = {CustomizedValueSql}
                WHERE TemplateId = @TemplateId
                AND SectionId = @SectionId
                AND {TemplateScopePredicate};";

            foreach (var section in sections)
            {
                using SqlCommand cmd = new SqlCommand(query, conn, tran);

                cmd.Parameters.AddWithValue("@TemplateId", section.TemplateId);
                cmd.Parameters.AddWithValue("@SectionId", section.SectionId);
                cmd.Parameters.AddWithValue("@DisplayOrder", section.DisplayOrder);
                cmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId.Value : DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }        

        public void SaveFieldOrder(List<FieldOrderVM> fields, int? clinicId, SqlConnection conn, SqlTransaction tran)
        {
            string query = $@"
                UPDATE PrescriptionTemplateFields
                SET DisplayOrder = @DisplayOrder,
                    IsCustomized = {CustomizedValueSql}
                WHERE TemplateId = @TemplateId
                AND SectionId = @SectionId
                AND FieldId = @FieldId
                AND {TemplateScopePredicate};";

            foreach (var field in fields)
            {
                using SqlCommand cmd = new SqlCommand(query, conn, tran);

                cmd.Parameters.AddWithValue("@TemplateId", field.TemplateId);
                cmd.Parameters.AddWithValue("@SectionId", field.SectionId);
                cmd.Parameters.AddWithValue("@FieldId", field.FieldId);
                cmd.Parameters.AddWithValue("@DisplayOrder", field.DisplayOrder);
                cmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId.Value : DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }        

        public void SaveOptionOrder(List<OptionOrderVM> options, int? clinicId, SqlConnection conn, SqlTransaction tran)
        {
            string query = $@"
                UPDATE PrescriptionTemplateFieldOptions
                SET DisplayOrder = @DisplayOrder,
                    IsCustomized = {CustomizedValueSql}
                WHERE TemplateId = @TemplateId
                AND FieldId = @FieldId
                AND OptionId = @OptionId
                AND {TemplateScopePredicate};";

            foreach (var option in options)
            {
                using SqlCommand cmd = new SqlCommand(query, conn, tran);

                cmd.Parameters.AddWithValue("@TemplateId", option.TemplateId);
                cmd.Parameters.AddWithValue("@FieldId", option.FieldId);
                cmd.Parameters.AddWithValue("@OptionId", option.OptionId);
                cmd.Parameters.AddWithValue("@DisplayOrder", option.DisplayOrder);
                cmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId.Value : DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }

        public bool ToggleSection(int templateId, int sectionId, int? clinicId, SqlConnection conn, SqlTransaction tran)
        {
            int rowsAffected = 0;

            string query = $@"UPDATE PrescriptionTemplateSections
                                        SET IsActive = CASE
                                                        WHEN IsActive = 1 THEN 0
                                                        ELSE 1
                                                    END,
                                        IsCustomized = {CustomizedValueSql}
                                        WHERE TemplateId = @TemplateId
                                        AND SectionId = @SectionId
                                        AND {TemplateScopePredicate};";
            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TemplateId", templateId);
            cmd.Parameters.AddWithValue("@SectionId", sectionId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId.Value : DBNull.Value);

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
                    return MapSection(sectionReader);
                }
            }

            return null;           
        }

        public bool ToggleField(int templateId, int fieldId, int? clinicId, SqlConnection conn, SqlTransaction tran)
        {
            int rowsAffected = 0;

            string query = $@"UPDATE PrescriptionTemplateFields
                                        SET IsActive = CASE
                                                        WHEN IsActive = 1 THEN 0
                                                        ELSE 1
                                                    END,
                                            IsCustomized = {CustomizedValueSql}
                                        WHERE TemplateId = @TemplateId
                                        AND FieldId = @FieldId
                                        AND {TemplateScopePredicate};";
            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TemplateId", templateId);
            cmd.Parameters.AddWithValue("@FieldId", fieldId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId.Value : DBNull.Value);

            rowsAffected = cmd.ExecuteNonQuery();

            return rowsAffected > 0;
        }
       
        public PrescriptionTemplateField GetField(int templateId, int fieldId, SqlConnection conn, SqlTransaction tran)
        {
            string fieldQuery = @"SELECT f.*,
                                        (SELECT COUNT(*)
                                        FROM PrescriptionTemplateFieldOptions o
                                        WHERE o.TemplateId = f.TemplateId
                                        AND o.FieldId = f.FieldId) AS OptionCount
                                    FROM PrescriptionTemplateFields f
                                    WHERE f.TemplateId = @TemplateId
                                    AND f.FieldId = @FieldId;";
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
                    return MapField(fieldReader);
                }
            }

            return null;
        }

        public bool ToggleOption(int templateId, int optionId, int? clinicId, SqlConnection conn, SqlTransaction tran)
        {
            int rowsAffected = 0;

            string query = $@"UPDATE PrescriptionTemplateFieldOptions
                                        SET IsActive = CASE
                                                        WHEN IsActive = 1 THEN 0
                                                        ELSE 1
                                                    END,
                                            IsCustomized = {CustomizedValueSql}
                                        WHERE TemplateId = @TemplateId
                                        AND OptionId = @OptionId
                                        AND {TemplateScopePredicate};";
            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TemplateId", templateId);
            cmd.Parameters.AddWithValue("@OptionId", optionId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId.Value : DBNull.Value);

            rowsAffected = cmd.ExecuteNonQuery();

            return rowsAffected > 0;
        }
       
        public PrescriptionTemplateFieldOption GetOption(int templateId, int optionId, SqlConnection conn, SqlTransaction tran)
        {
            string optionQuery = @"SELECT * FROM PrescriptionTemplateFieldOptions 
                                    WHERE TemplateId = @TemplateId
                                    AND OptionId = @OptionId
                                    ORDER BY DisplayOrder";
            using SqlCommand optionCmd = new SqlCommand(optionQuery, conn);
            if (tran != null)
            {
                optionCmd.Transaction = tran;
            }

            optionCmd.Parameters.AddWithValue("@TemplateId", templateId);
            optionCmd.Parameters.AddWithValue("@OptionId", optionId);

            using (var optionReader = optionCmd.ExecuteReader())
            {
                if (optionReader.Read())
                {
                    return MapOption(optionReader);
                }
            }

            return null;
        }

        public int InsertSectionBefore(int templateId, int sectionId, int? clinicId, string sectionName,
                                            int currentOrder, SqlConnection conn, SqlTransaction tran)
        {
            int updateRowsAffected = 0;

            string updateQuery = $@"UPDATE PrescriptionTemplateSections
                                SET DisplayOrder = DisplayOrder + 1,
                                    IsCustomized = {CustomizedValueSql}
                                WHERE TemplateId = @TemplateId
                                AND DisplayOrder >= @CurrentOrder
                                AND {TemplateScopePredicate};";
            using SqlCommand updateCmd = new SqlCommand(updateQuery, conn, tran);

            updateCmd.Parameters.AddWithValue("@TemplateId", templateId);
            updateCmd.Parameters.AddWithValue("@CurrentOrder", currentOrder);
            updateCmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId.Value : DBNull.Value);

            updateRowsAffected = updateCmd.ExecuteNonQuery();

            string insertQuery = $@"INSERT INTO PrescriptionTemplateSections
                                    (TemplateId, ClinicId, SourceSectionId, SectionName, DisplayOrder, IsActive, 
                                    Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsCustomized)
                                    VALUES
                                    (@TemplateId, @ClinicId, @SourceSectionId, @SectionName, @CurrentOrder, 1, 
                                    0, 0, 0, 0, {CustomizedValueSql});
                                    SELECT SCOPE_IDENTITY();";
            using SqlCommand insertCmd = new SqlCommand(insertQuery, conn, tran);

            insertCmd.Parameters.AddWithValue("@TemplateId", templateId);
            insertCmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId.Value : DBNull.Value);
            insertCmd.Parameters.AddWithValue("@SectionName", sectionName);
            insertCmd.Parameters.AddWithValue("@CurrentOrder", currentOrder);
            insertCmd.Parameters.AddWithValue("@SourceSectionId", DBNull.Value);
            int id = Convert.ToInt32(insertCmd.ExecuteScalar());

            return id;
        }

        public int InsertSectionAfter(int templateId, int sectionId, int? clinicId, string sectionName,
                                            int currentOrder, SqlConnection conn, SqlTransaction tran)
        {
            int updateRowsAffected = 0;

            string updateQuery = $@"UPDATE PrescriptionTemplateSections
                                        SET DisplayOrder = DisplayOrder + 1,
                                            IsCustomized = {CustomizedValueSql}
                                        WHERE TemplateId = @TemplateId
                                        AND DisplayOrder > @CurrentOrder
                                        AND {TemplateScopePredicate};";
            using SqlCommand updateCmd = new SqlCommand(updateQuery, conn, tran);

            updateCmd.Parameters.AddWithValue("@TemplateId", templateId);
            updateCmd.Parameters.AddWithValue("@CurrentOrder", currentOrder);
            updateCmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId.Value : DBNull.Value);

            updateRowsAffected = updateCmd.ExecuteNonQuery();

            string insertQuery = $@"INSERT INTO PrescriptionTemplateSections
                                    (TemplateId, ClinicId, SourceSectionId, SectionName, DisplayOrder, IsActive, 
                                    Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsCustomized)
                                    VALUES
                                    (@TemplateId, @ClinicId, @SourceSectionId, @SectionName, @CurrentOrder + 1, 1, 
                                    0, 0, 0, 0, {CustomizedValueSql});
                                    SELECT SCOPE_IDENTITY();";
            using SqlCommand insertCmd = new SqlCommand(insertQuery, conn, tran);

            insertCmd.Parameters.AddWithValue("@TemplateId", templateId);
            insertCmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId.Value : DBNull.Value);
            insertCmd.Parameters.AddWithValue("@SourceSectionId", DBNull.Value);
            insertCmd.Parameters.AddWithValue("@SectionName", sectionName);
            insertCmd.Parameters.AddWithValue("@CurrentOrder", currentOrder);

            int id = Convert.ToInt32(insertCmd.ExecuteScalar());

            return id;
        }

        public int AddField(int templateId, int sectionId, int? clinicId, string fieldName, string fieldType, 
                                            SqlConnection conn, SqlTransaction tran)
        {
            string insertQuery = $@"INSERT INTO PrescriptionTemplateFields
                                    (TemplateId, ClinicId, SectionId, SourceFieldId, FieldLabel, FieldTypeCode, 
                                    DisplayOrder, IsActive, IsArchived, IsCustomized)
                                    VALUES
                                    (@TemplateId, @ClinicId, @SectionId, @SourceFieldId, @FieldLabel, @FieldTypeCode, 
                                    1, 1, 0, {CustomizedValueSql});
                                    SELECT SCOPE_IDENTITY();";
            using SqlCommand insertCmd = new SqlCommand(insertQuery, conn, tran);

            insertCmd.Parameters.AddWithValue("@TemplateId", templateId);
            insertCmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId.Value : DBNull.Value);
            insertCmd.Parameters.AddWithValue("@SectionId", sectionId);
            insertCmd.Parameters.AddWithValue("@SourceFieldId", DBNull.Value);
            insertCmd.Parameters.AddWithValue("@FieldLabel", fieldName);
            insertCmd.Parameters.AddWithValue("@FieldTypeCode", fieldType);

            int id = Convert.ToInt32(insertCmd.ExecuteScalar());

            return id;
        }

        public int InsertFieldBefore(int templateId, int sectionId, int fieldId, int? clinicId, string fieldLabel,
                                            int currentOrder, SqlConnection conn, SqlTransaction tran)
        {
            int updateRowsAffected = 0;

            string updateQuery = $@"UPDATE PrescriptionTemplateFields
                                SET DisplayOrder = DisplayOrder + 1,
                                    IsCustomized = {CustomizedValueSql}
                                WHERE TemplateId = @TemplateId
                                AND SectionId = @SectionId
                                AND DisplayOrder >= @CurrentOrder
                                AND {TemplateScopePredicate};";
            using SqlCommand updateCmd = new SqlCommand(updateQuery, conn, tran);

            updateCmd.Parameters.AddWithValue("@TemplateId", templateId);
            updateCmd.Parameters.AddWithValue("@SectionId", sectionId);
            updateCmd.Parameters.AddWithValue("@CurrentOrder", currentOrder);
            updateCmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId.Value : DBNull.Value);

            updateRowsAffected = updateCmd.ExecuteNonQuery();

            string insertQuery = $@"INSERT INTO PrescriptionTemplateFields
                                    (TemplateId, ClinicId, SectionId, SourceFieldId, FieldLabel, FieldTypeCode, 
                                    DisplayOrder, IsActive, IsArchived, IsCustomized)
                                    VALUES
                                    (@TemplateId, @ClinicId, @SectionId, @SourceFieldId, @FieldLabel, @FieldTypeCode, 
                                    @CurrentOrder, 1, 0, {CustomizedValueSql});
                                    SELECT SCOPE_IDENTITY();";
            using SqlCommand insertCmd = new SqlCommand(insertQuery, conn, tran);

            insertCmd.Parameters.AddWithValue("@TemplateId", templateId);
            insertCmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId.Value : DBNull.Value);
            insertCmd.Parameters.AddWithValue("@SectionId", sectionId);
            insertCmd.Parameters.AddWithValue("@SourceFieldId", DBNull.Value);
            insertCmd.Parameters.AddWithValue("@FieldLabel", fieldLabel);
            insertCmd.Parameters.AddWithValue("@FieldTypeCode", "Text");
            insertCmd.Parameters.AddWithValue("@CurrentOrder", currentOrder);

            int id = Convert.ToInt32(insertCmd.ExecuteScalar());

            return id;
        }

        public int InsertFieldAfter(int templateId, int sectionId, int fieldId, int? clinicId, string fieldLabel,
                                            int currentOrder, SqlConnection conn, SqlTransaction tran)
        {
            int updateRowsAffected = 0;

            string updateQuery = $@"UPDATE PrescriptionTemplateFields
                                        SET DisplayOrder = DisplayOrder + 1,
                                            IsCustomized = {CustomizedValueSql}
                                        WHERE TemplateId = @TemplateId
                                        AND SectionId = @SectionId
                                        AND DisplayOrder > @CurrentOrder
                                        AND {TemplateScopePredicate};";
            using SqlCommand updateCmd = new SqlCommand(updateQuery, conn, tran);

            updateCmd.Parameters.AddWithValue("@TemplateId", templateId);
            updateCmd.Parameters.AddWithValue("@SectionId", sectionId);
            updateCmd.Parameters.AddWithValue("@CurrentOrder", currentOrder);
            updateCmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId.Value : DBNull.Value);

            updateRowsAffected = updateCmd.ExecuteNonQuery();

            string insertQuery = $@"INSERT INTO PrescriptionTemplateFields
                                    (TemplateId, ClinicId, SectionId, SourceFieldId, FieldLabel, FieldTypeCode, 
                                    DisplayOrder, IsActive, IsArchived, IsCustomized)
                                    VALUES
                                    (@TemplateId, @ClinicId, @SectionId, @SourceFieldId, @FieldLabel, @FieldTypeCode, 
                                    @CurrentOrder + 1, 1, 0, {CustomizedValueSql});
                                    SELECT SCOPE_IDENTITY();";
            using SqlCommand insertCmd = new SqlCommand(insertQuery, conn, tran);

            insertCmd.Parameters.AddWithValue("@TemplateId", templateId);
            insertCmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId.Value : DBNull.Value);
            insertCmd.Parameters.AddWithValue("@SectionId", sectionId);
            insertCmd.Parameters.AddWithValue("@SourceFieldId", DBNull.Value);
            insertCmd.Parameters.AddWithValue("@FieldLabel", fieldLabel);
            insertCmd.Parameters.AddWithValue("@FieldTypeCode", "Text");
            insertCmd.Parameters.AddWithValue("@CurrentOrder", currentOrder);

            int id = Convert.ToInt32(insertCmd.ExecuteScalar());

            return id;
        }

        public int InsertOptionBefore(int templateId, int sectionId, int fieldId, int optionId, int? clinicId,  
                                        string optionText, int currentOrder, SqlConnection conn, SqlTransaction tran)
        {
            int updateRowsAffected = 0;

            string updateQuery = $@"UPDATE PrescriptionTemplateFieldOptions
                                SET DisplayOrder = DisplayOrder + 1,
                                    IsCustomized = {CustomizedValueSql}
                                WHERE TemplateId = @TemplateId
                                AND FieldId = @FieldId
                                AND DisplayOrder >= @CurrentOrder
                                AND {TemplateScopePredicate};";
            using SqlCommand updateCmd = new SqlCommand(updateQuery, conn, tran);

            updateCmd.Parameters.AddWithValue("@TemplateId", templateId);
            updateCmd.Parameters.AddWithValue("@FieldId", fieldId);
            updateCmd.Parameters.AddWithValue("@CurrentOrder", currentOrder);
            updateCmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId.Value : DBNull.Value);

            updateRowsAffected = updateCmd.ExecuteNonQuery();

            string insertQuery = $@"INSERT INTO PrescriptionTemplateFieldOptions
                                    (TemplateId, ClinicId, SectionId, FieldId, SourceOptionId, OptionText, DisplayOrder, 
                                    IsActive, IsArchived, IsCustomized)
                                    VALUES
                                    (@TemplateId, @ClinicId, @SectionId, @FieldId, @SourceOptionId, @OptionText, @CurrentOrder,
                                    1, 0, {CustomizedValueSql});
                                    SELECT SCOPE_IDENTITY();";

            using SqlCommand insertCmd = new SqlCommand(insertQuery, conn, tran);

            insertCmd.Parameters.AddWithValue("@TemplateId", templateId);
            insertCmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId.Value : DBNull.Value);
            insertCmd.Parameters.AddWithValue("@SectionId", sectionId);
            insertCmd.Parameters.AddWithValue("@FieldId", fieldId);
            insertCmd.Parameters.AddWithValue("@SourceOptionId", DBNull.Value);
            insertCmd.Parameters.AddWithValue("@OptionText", optionText);
            insertCmd.Parameters.AddWithValue("@CurrentOrder", currentOrder);

            int id = Convert.ToInt32(insertCmd.ExecuteScalar());

            return id;
        }

        public int InsertOptionAfter(int templateId, int sectionId, int fieldId, int optionId, int? clinicId,  
                                        string optionText, int currentOrder, SqlConnection conn, SqlTransaction tran)
        {
            int updateRowsAffected = 0;

            string updateQuery = $@"UPDATE PrescriptionTemplateFieldOptions
                                        SET DisplayOrder = DisplayOrder + 1,
                                            IsCustomized = {CustomizedValueSql}
                                        WHERE TemplateId = @TemplateId
                                        AND FieldId = @FieldId
                                        AND DisplayOrder > @CurrentOrder
                                        AND {TemplateScopePredicate};";
            using SqlCommand updateCmd = new SqlCommand(updateQuery, conn, tran);

            updateCmd.Parameters.AddWithValue("@TemplateId", templateId);
            updateCmd.Parameters.AddWithValue("@FieldId", fieldId);
            updateCmd.Parameters.AddWithValue("@CurrentOrder", currentOrder);
            updateCmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId.Value : DBNull.Value);

            updateRowsAffected = updateCmd.ExecuteNonQuery();

            string insertQuery = $@"INSERT INTO PrescriptionTemplateFieldOptions
                                    (TemplateId, ClinicId, SectionId, FieldId, SourceOptionId, OptionText, 
                                    DisplayOrder, IsActive, IsArchived, IsCustomized)
                                    VALUES
                                    (@TemplateId, @ClinicId, @SectionId, @FieldId, @SourceOptionId, @OptionText, 
                                    @CurrentOrder + 1, 1, 0, {CustomizedValueSql});
                                    SELECT SCOPE_IDENTITY();";
            using SqlCommand insertCmd = new SqlCommand(insertQuery, conn, tran);

            insertCmd.Parameters.AddWithValue("@TemplateId", templateId);
            insertCmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId.Value : DBNull.Value);
            insertCmd.Parameters.AddWithValue("@SectionId", sectionId);
            insertCmd.Parameters.AddWithValue("@FieldId", fieldId);
            insertCmd.Parameters.AddWithValue("@SourceOptionId", DBNull.Value);
            insertCmd.Parameters.AddWithValue("@OptionText", optionText);
            insertCmd.Parameters.AddWithValue("@CurrentOrder", currentOrder);

            int id = Convert.ToInt32(insertCmd.ExecuteScalar());

            return id;
        }

        public bool SaveSectionName(int templateId, int sectionId, int? clinicId, string name,
                                        SqlConnection conn, SqlTransaction tran)
        {
            string query = $@"
                UPDATE PrescriptionTemplateSections
                SET SectionName = @SectionName,
                    IsCustomized = {CustomizedValueSql}
                WHERE TemplateId = @TemplateId
                AND SectionId = @SectionId
                AND {TemplateScopePredicate};";

            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TemplateId", templateId);
            cmd.Parameters.AddWithValue("@SectionId", sectionId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@SectionName", name);

            return cmd.ExecuteNonQuery() > 0;
        }

        public bool SaveFieldName(int templateId, int fieldId, int? clinicId, string name,
                                    SqlConnection conn, SqlTransaction tran)
        {
            string query = $@"
                UPDATE PrescriptionTemplateFields
                SET FieldLabel = @FieldLabel,
                    IsCustomized = {CustomizedValueSql}
                WHERE TemplateId = @TemplateId
                AND FieldId = @FieldId
                AND {TemplateScopePredicate};";

            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TemplateId", templateId);
            cmd.Parameters.AddWithValue("@FieldId", fieldId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@FieldLabel", name);

            return cmd.ExecuteNonQuery() > 0;
        }

        public bool SaveOptionName(int templateId, int optionId, int? clinicId, string name,
                                    SqlConnection conn, SqlTransaction tran)
        {
            string query = $@"
                UPDATE PrescriptionTemplateFieldOptions
                SET OptionText = @OptionText,
                    IsCustomized = {CustomizedValueSql}
                WHERE TemplateId = @TemplateId
                AND OptionId = @OptionId
                AND {TemplateScopePredicate};";

            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TemplateId", templateId);
            cmd.Parameters.AddWithValue("@OptionId", optionId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@OptionText", name);

            return cmd.ExecuteNonQuery() > 0;
        }

        public bool DeleteOptions(int templateId, int fieldId, int? clinicId, SqlConnection conn, SqlTransaction tran)
        {
            int optionRowsAffected = 0;

            string optionQuery = $@"DELETE FROM PrescriptionTemplateFieldOptions 
                                    WHERE TemplateId = @TemplateId
                                    AND FieldId = @FieldId
                                    AND {TemplateScopePredicate}";
            using SqlCommand optionCmd = new SqlCommand(optionQuery, conn, tran);

            optionCmd.Parameters.AddWithValue("@TemplateId", templateId);
            optionCmd.Parameters.AddWithValue("@FieldId", fieldId);
            optionCmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId.Value : DBNull.Value);

            optionRowsAffected = optionCmd.ExecuteNonQuery();

            return optionRowsAffected > 0;
        }

        public bool UpdateFieldType(int templateId, int fieldId, int? clinicId, string fieldTypeCode,
                                    SqlConnection conn, SqlTransaction tran)
        {
            string query = $@"
                UPDATE PrescriptionTemplateFields
                SET FieldTypeCode = @FieldTypeCode,
                    IsCustomized = {CustomizedValueSql}
                WHERE TemplateId = @TemplateId
                AND FieldId = @FieldId
                AND {TemplateScopePredicate};";

            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TemplateId", templateId);
            cmd.Parameters.AddWithValue("@FieldId", fieldId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@FieldTypeCode", fieldTypeCode);

            return cmd.ExecuteNonQuery() > 0;
        }

        public int AddOption(int templateId, int sectionId, int? clinicId, int fieldId, string optionText,  
                                bool isActive, int displayOrder, SqlConnection conn, SqlTransaction tran)
        {
            string query = $@"INSERT INTO PrescriptionTemplateFieldOptions
                                    (TemplateId, SectionId, ClinicId, FieldId, OptionText, IsActive, IsArchived, DisplayOrder,
                                    IsCustomized) 
                                    VALUES 
                                    (@TemplateId, @SectionId, @ClinicId, @FieldId, @OptionText, @IsActive, 0, @DisplayOrder, 
                                    {CustomizedValueSql});
                                        SELECT SCOPE_IDENTITY();";
            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TemplateId", templateId);
            cmd.Parameters.AddWithValue("@SectionId", sectionId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId.Value : DBNull.Value);
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
                    SourceTemplateId = reader["SourceTemplateId"] != DBNull.Value ? (int)reader["SourceTemplateId"] : null
                });
            }

            return list;
        }

        public bool SaveSectionOptions(int templateId, int sectionId, int? clinicId, bool isMultiline, bool isRx, 
                                            bool isVital, bool isTestResult, SqlConnection conn, SqlTransaction tran)
        {
            int rowsAffected = 0;

            string query = $@"UPDATE PrescriptionTemplateSections
                                        SET Multiline = @IsMultiline,
                                            IsPharmacologyAdvice = @IsRx,
                                            IsVital = @IsVital,
                                            IsTestResult = @IsTestResult,
                                            IsCustomized = {CustomizedValueSql}
                                        WHERE TemplateId = @TemplateId
                                        AND SectionId = @SectionId
                                        AND {TemplateScopePredicate};";
            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TemplateId", templateId);
            cmd.Parameters.AddWithValue("@SectionId", sectionId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@IsMultiline", isMultiline ? 1 : 0);
            cmd.Parameters.AddWithValue("@IsRx", isRx ? 1 : 0);
            cmd.Parameters.AddWithValue("@IsVital", isVital ? 1 : 0);
            cmd.Parameters.AddWithValue("@IsTestResult", isTestResult ? 1 : 0);

            rowsAffected = cmd.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public bool UpdateFieldsArchival(int templateId, int sectionId, int? clinicId, bool isArchived, 
                                            SqlConnection conn, SqlTransaction tran)
        {
            int rowsAffected = 0;
            bool existingIsArchived = !isArchived;

            string query = $@"UPDATE PrescriptionTemplateFields
                                        SET IsArchived = @IsArchived,
                                            IsCustomized = {CustomizedValueSql}
                                        WHERE TemplateId = @TemplateId
                                        AND SectionId = @SectionId
                                        AND IsArchived = @ExistingIsArchived
                                        AND {TemplateScopePredicate};";
            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TemplateId", templateId);
            cmd.Parameters.AddWithValue("@SectionId", sectionId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@IsArchived", isArchived);
            cmd.Parameters.AddWithValue("@ExistingIsArchived", existingIsArchived);

            rowsAffected = cmd.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public bool UpdateOptionsArchival(int templateId, int sectionId, int? clinicId, bool isArchived, SqlConnection conn, SqlTransaction tran)
        {
            int rowsAffected = 0;
            bool existingIsArchived = !isArchived;

            string query = $@"UPDATE PrescriptionTemplateFieldOptions
                                        SET IsArchived = @IsArchived,
                                            IsCustomized = {CustomizedValueSql}
                                        WHERE TemplateId = @TemplateId
                                        AND SectionId = @SectionId
                                        AND IsArchived = @ExistingIsArchived
                                        AND {TemplateScopePredicate};";
            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TemplateId", templateId);
            cmd.Parameters.AddWithValue("@SectionId", sectionId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@IsArchived", isArchived);
            cmd.Parameters.AddWithValue("@ExistingIsArchived", existingIsArchived);

            rowsAffected = cmd.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        public bool SaveFieldSource(int templateId, int fieldId, int? clinicId, string fieldSource, 
                                    SqlConnection conn, SqlTransaction tran)
        {
            int rowsAffected = 0;

            string query = $@"UPDATE PrescriptionTemplateFields
                                        SET FieldSource = @FieldSource,
                                            IsCustomized = {CustomizedValueSql}
                                        WHERE TemplateId = @TemplateId
                                        AND FieldId = @FieldId
                                        AND {TemplateScopePredicate};";
            using SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@TemplateId", templateId);
            cmd.Parameters.AddWithValue("@FieldId", fieldId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId.HasValue ? clinicId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@FieldSource", fieldSource);

            rowsAffected = cmd.ExecuteNonQuery();

            return rowsAffected > 0;            
        }

        public List<string> ListTemplateCodes(SqlConnection conn, SqlTransaction? tran = null)
        {
            List<string> codes = new List<string>();

            // retrieve template

            string query = @"SELECT DISTINCT (TemplateCode) AS TemplateCode
                                    FROM PrescriptionTemplates
                                    WHERE TemplateCode <> 'DEFAULT'
                                    AND ClinicId IS NULL
                                    AND DepartmentId IS NULL
                                    AND DoctorId IS NULL;";

            using SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    codes.Add(reader["TemplateCode"].ToString());
                }
            }

            return codes;
        }

        public void DeleteSyncRows(string tableName, string idColumn, int templateId, IEnumerable<int> ids,
                                    SqlConnection conn, SqlTransaction tran)
        {
            if (ids == null)
                return;

            // Table and column names are supplied only by the fixed calls above.
            string query = $@"
                DELETE FROM {tableName}
                WHERE TemplateId = @TemplateId
                  AND {idColumn} = @NodeId;";

            foreach (int id in ids.Distinct())
            {
                if (id <= 0)
                    continue;

                using SqlCommand cmd = new SqlCommand(query, conn, tran);

                cmd.Parameters.AddWithValue("@TemplateId", templateId);
                cmd.Parameters.AddWithValue("@NodeId", id);

                cmd.ExecuteNonQuery();
            }
        }

        private PrescriptionTemplate MapTemplate(SqlDataReader reader)
        {
            return new PrescriptionTemplate
            {
                TemplateId = (int)reader["TemplateId"],
                ClinicId = GetNullableInt(reader, "ClinicId"),
                DepartmentId = GetNullableInt(reader, "DepartmentId"),
                DepartmentName = GetNullableString(reader, "DepartmentName"),
                DoctorId = GetNullableInt(reader, "DoctorId"),
                DoctorName = GetNullableString(reader, "DoctorName"),
                TemplateCode = reader["TemplateCode"].ToString(),
                TemplateName = reader["TemplateName"].ToString(),
                SourceTemplateId = GetNullableInt(reader, "SourceTemplateId"),
                IsActive = (bool)reader["IsActive"]
            };
        }

        private PrescriptionTemplateSection MapSection(SqlDataReader reader)
        {
            return new PrescriptionTemplateSection
            {
                TemplateId = (int)reader["TemplateId"],
                ClinicId = GetNullableInt(reader, "ClinicId"),
                SectionId = (int)reader["SectionId"],
                SectionName = reader["SectionName"].ToString(),
                DisplayOrder = (int)reader["DisplayOrder"],
                IsActive = (bool)reader["IsActive"],
                IsArchived = (bool)reader["IsArchived"],
                Multiline = (bool)reader["Multiline"],
                IsPharmacologyAdvice = (bool)reader["IsPharmacologyAdvice"],
                IsVital = (bool)reader["IsVital"],
                IsTestResult = (bool)reader["IsTestResult"],
                SourceSectionId = GetNullableInt(reader, "SourceSectionId"),
                IsCustomized = (bool)reader["IsCustomized"]
            };
        }

        private PrescriptionTemplateField MapField(SqlDataReader reader)
        {
            return new PrescriptionTemplateField
            {
                TemplateId = (int)reader["TemplateId"],
                ClinicId = GetNullableInt(reader, "ClinicId"),
                SectionId = (int)reader["SectionId"],
                FieldId = (int)reader["FieldId"],
                FieldLabel = reader["FieldLabel"].ToString(),
                FieldTypeCode = reader["FieldTypeCode"].ToString(),
                FieldCode = reader["FieldCode"].ToString(),
                FieldSource = GetNullableString(reader, "FieldSource"),
                IsActive = (bool)reader["IsActive"],
                DisplayOrder = (int)reader["DisplayOrder"],
                IsArchived = (bool)reader["IsArchived"],
                SourceFieldId = GetNullableInt(reader, "SourceFieldId"),
                IsCustomized = (bool)reader["IsCustomized"],
                MergeWith = GetNullableString(reader, "MergeWith")
            };
        }

        private PrescriptionTemplateFieldOption MapOption(SqlDataReader reader)
        {
            return new PrescriptionTemplateFieldOption
            {
                TemplateId = (int)reader["TemplateId"],
                ClinicId = GetNullableInt(reader, "ClinicId"),
                SectionId = (int)reader["SectionId"],
                FieldId = (int)reader["FieldId"],
                OptionId = (int)reader["OptionId"],
                OptionText = reader["OptionText"].ToString(),
                IsActive = (bool)reader["IsActive"],
                IsArchived = (bool)reader["IsArchived"],
                DisplayOrder = (int)reader["DisplayOrder"],
                SourceOptionId = GetNullableInt(reader, "SourceOptionId"),
                IsCustomized = (bool)reader["IsCustomized"]
            };
        }

    }

}
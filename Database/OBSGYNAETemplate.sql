-- Creates/replaces only the global OBSGYNAE prescription template.
-- Execute with: EXEC dbo.[OBSGYNAETemplate];
CREATE OR ALTER PROCEDURE dbo.[OBSGYNAETemplate]
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;
    BEGIN TRY
        BEGIN TRANSACTION;
        DECLARE @TemplateId INT;
        DECLARE @SectionId INT;
        DECLARE @FieldId INT;

        -- Remove only this matching global template and its descendants.
        DELETE O
        FROM dbo.PrescriptionTemplateFieldOptions AS O
        INNER JOIN dbo.PrescriptionTemplates AS T ON T.TemplateId = O.TemplateId
        WHERE T.ClinicId IS NULL AND T.DepartmentId IS NULL AND T.DoctorId IS NULL AND T.TemplateCode = 'OBSGYNAE';
        DELETE F
        FROM dbo.PrescriptionTemplateFields AS F
        INNER JOIN dbo.PrescriptionTemplates AS T ON T.TemplateId = F.TemplateId
        WHERE T.ClinicId IS NULL AND T.DepartmentId IS NULL AND T.DoctorId IS NULL AND T.TemplateCode = 'OBSGYNAE';
        DELETE S
        FROM dbo.PrescriptionTemplateSections AS S
        INNER JOIN dbo.PrescriptionTemplates AS T ON T.TemplateId = S.TemplateId
        WHERE T.ClinicId IS NULL AND T.DepartmentId IS NULL AND T.DoctorId IS NULL AND T.TemplateCode = 'OBSGYNAE';
        DELETE FROM dbo.PrescriptionTemplates
        WHERE ClinicId IS NULL AND DepartmentId IS NULL AND DoctorId IS NULL AND TemplateCode = 'OBSGYNAE';

        INSERT INTO dbo.PrescriptionTemplates
        (ClinicId, DepartmentId, DoctorId, TemplateCode, TemplateName, SourceTemplateId, IsActive)
        VALUES (NULL, NULL, NULL, 'OBSGYNAE', N'Obstetrics and Gynaecology Consultation Template', NULL, 1);
        SET @TemplateId = CONVERT(INT, SCOPE_IDENTITY());

        -- Section 1: Vitals
        INSERT INTO dbo.PrescriptionTemplateSections
        (TemplateId, ClinicId, SourceSectionId, IsCustomized, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsArchived)
        VALUES (@TemplateId, NULL, NULL, 0, N'Vitals', 1, 1, 0, 0, 1, 0, 0);
        SET @SectionId = CONVERT(INT, SCOPE_IDENTITY());

        -- Section 2: Test Results
        INSERT INTO dbo.PrescriptionTemplateSections
        (TemplateId, ClinicId, SourceSectionId, IsCustomized, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsArchived)
        VALUES (@TemplateId, NULL, NULL, 0, N'Test Results', 2, 1, 0, 0, 0, 1, 0);
        SET @SectionId = CONVERT(INT, SCOPE_IDENTITY());

        -- Section 3: Chief Complaints
        INSERT INTO dbo.PrescriptionTemplateSections
        (TemplateId, ClinicId, SourceSectionId, IsCustomized, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsArchived)
        VALUES (@TemplateId, NULL, NULL, 0, N'Chief Complaints', 3, 1, 0, 0, 0, 0, 0);
        SET @SectionId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Chief Complaints', N'Textarea', N'CHIEF_COMPLAINTS', NULL, 0, 1, 1, 0, NULL);

        -- Section 4: Obstetric / Gynaecological History
        INSERT INTO dbo.PrescriptionTemplateSections
        (TemplateId, ClinicId, SourceSectionId, IsCustomized, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsArchived)
        VALUES (@TemplateId, NULL, NULL, 0, N'Obstetric / Gynaecological History', 4, 1, 0, 0, 0, 0, 0);
        SET @SectionId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'LMP / Menstrual History', N'Textarea', N'LMP___MENSTRUAL_HISTORY', NULL, 0, 1, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Obstetric History', N'Textarea', N'OBSTETRIC_HISTORY', NULL, 0, 2, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Pregnancy / Gestational Details', N'Textarea', N'PREGNANCY___GESTATIONAL_DETAILS', NULL, 0, 3, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Contraceptive History', N'Textarea', N'CONTRACEPTIVE_HISTORY', NULL, 0, 4, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Gynaecological Symptoms', N'Textarea', N'GYNAECOLOGICAL_SYMPTOMS', NULL, 0, 5, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Relevant Medical / Surgical History', N'Textarea', N'RELEVANT_MEDICAL___SURGICAL_HISTORY', NULL, 0, 6, 1, 0, NULL);

        -- Section 5: Clinical Examination
        INSERT INTO dbo.PrescriptionTemplateSections
        (TemplateId, ClinicId, SourceSectionId, IsCustomized, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsArchived)
        VALUES (@TemplateId, NULL, NULL, 0, N'Clinical Examination', 5, 1, 0, 0, 0, 0, 0);
        SET @SectionId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'General Examination', N'Textarea', N'GENERAL_EXAMINATION', NULL, 0, 1, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Abdominal Examination', N'Textarea', N'ABDOMINAL_EXAMINATION', NULL, 0, 2, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Obstetric Examination', N'Textarea', N'OBSTETRIC_EXAMINATION', NULL, 0, 3, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Pelvic Examination', N'Textarea', N'PELVIC_EXAMINATION', NULL, 0, 4, 1, 0, NULL);

        -- Section 6: Diagnosis
        INSERT INTO dbo.PrescriptionTemplateSections
        (TemplateId, ClinicId, SourceSectionId, IsCustomized, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsArchived)
        VALUES (@TemplateId, NULL, NULL, 0, N'Diagnosis', 6, 1, 0, 0, 0, 0, 0);
        SET @SectionId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Diagnosis', N'Textarea', N'DIAGNOSIS', NULL, 0, 1, 1, 0, NULL);

        -- Section 7: Medical Advice
        INSERT INTO dbo.PrescriptionTemplateSections
        (TemplateId, ClinicId, SourceSectionId, IsCustomized, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsArchived)
        VALUES (@TemplateId, NULL, NULL, 0, N'Medical Advice', 7, 1, 0, 0, 0, 0, 0);
        SET @SectionId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Medical Advice', N'Textarea', N'MEDICAL_ADVICE', NULL, 0, 1, 1, 0, NULL);

        -- Section 8: Pharmacological Advice
        INSERT INTO dbo.PrescriptionTemplateSections
        (TemplateId, ClinicId, SourceSectionId, IsCustomized, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsArchived)
        VALUES (@TemplateId, NULL, NULL, 0, N'Pharmacological Advice', 8, 1, 1, 1, 0, 0, 0);
        SET @SectionId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Medicine', N'Lookup', N'Medicine', N'Medicines', 1, 1, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Generic Name', N'Text', N'GenericName', N'Medicines', 1, 1, 1, 0, N'Medicine');

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Dose', N'Dropdown', N'Dose', NULL, 1, 2, 1, 0, NULL);
        SET @FieldId = CONVERT(INT, SCOPE_IDENTITY());
        INSERT INTO dbo.PrescriptionTemplateFieldOptions
        (FieldId, SectionId, TemplateId, ClinicId, SourceOptionId, IsCustomized, OptionText, DisplayOrder, IsActive, IsArchived)
        VALUES
        (@FieldId, @SectionId, @TemplateId, NULL, NULL, 0, N'Once Daily', 1, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, NULL, 0, N'Twice Daily', 2, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, NULL, 0, N'Thrice Daily', 3, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, NULL, 0, N'SOS', 4, 1, 0);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Timing', N'Dropdown', N'Timing', NULL, 1, 3, 1, 0, NULL);
        SET @FieldId = CONVERT(INT, SCOPE_IDENTITY());
        INSERT INTO dbo.PrescriptionTemplateFieldOptions
        (FieldId, SectionId, TemplateId, ClinicId, SourceOptionId, IsCustomized, OptionText, DisplayOrder, IsActive, IsArchived)
        VALUES
        (@FieldId, @SectionId, @TemplateId, NULL, NULL, 0, N'Before Meal', 1, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, NULL, 0, N'After Meal', 2, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, NULL, 0, N'During Meal', 3, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, NULL, 0, N'At Bedtime', 4, 1, 0);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Duration', N'Number', N'Duration', NULL, 1, 4, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Unit', N'Dropdown', N'Unit', NULL, 1, 5, 1, 0, NULL);
        SET @FieldId = CONVERT(INT, SCOPE_IDENTITY());
        INSERT INTO dbo.PrescriptionTemplateFieldOptions
        (FieldId, SectionId, TemplateId, ClinicId, SourceOptionId, IsCustomized, OptionText, DisplayOrder, IsActive, IsArchived)
        VALUES
        (@FieldId, @SectionId, @TemplateId, NULL, NULL, 0, N'Days', 1, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, NULL, 0, N'Weeks', 2, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, NULL, 0, N'Months', 3, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, NULL, 0, N'Years', 4, 1, 0);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Instructions', N'Text', N'Instruction', NULL, 0, 6, 1, 0, NULL);

        -- Section 9: Review and Followup
        INSERT INTO dbo.PrescriptionTemplateSections
        (TemplateId, ClinicId, SourceSectionId, IsCustomized, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsArchived)
        VALUES (@TemplateId, NULL, NULL, 0, N'Review and Followup', 9, 1, 0, 0, 0, 0, 0);
        SET @SectionId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Review and Followup', N'Text', N'REVIEW_AND_FOLLOWUP', NULL, 0, 1, 1, 0, NULL);

        COMMIT TRANSACTION;
        SELECT @TemplateId AS CreatedTemplateId;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
GO

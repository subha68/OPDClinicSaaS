-- Creates/replaces only the global DENTAL prescription template.
-- Execute with: EXEC dbo.[DENTALTemplate];
CREATE OR ALTER PROCEDURE dbo.[DENTALTemplate]
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
        WHERE T.ClinicId IS NULL AND T.DepartmentId IS NULL AND T.DoctorId IS NULL AND T.TemplateCode = 'DENTAL';
        DELETE F
        FROM dbo.PrescriptionTemplateFields AS F
        INNER JOIN dbo.PrescriptionTemplates AS T ON T.TemplateId = F.TemplateId
        WHERE T.ClinicId IS NULL AND T.DepartmentId IS NULL AND T.DoctorId IS NULL AND T.TemplateCode = 'DENTAL';
        DELETE S
        FROM dbo.PrescriptionTemplateSections AS S
        INNER JOIN dbo.PrescriptionTemplates AS T ON T.TemplateId = S.TemplateId
        WHERE T.ClinicId IS NULL AND T.DepartmentId IS NULL AND T.DoctorId IS NULL AND T.TemplateCode = 'DENTAL';
        DELETE FROM dbo.PrescriptionTemplates
        WHERE ClinicId IS NULL AND DepartmentId IS NULL AND DoctorId IS NULL AND TemplateCode = 'DENTAL';

        INSERT INTO dbo.PrescriptionTemplates
        (ClinicId, DepartmentId, DoctorId, TemplateCode, TemplateName, SourceTemplateId, IsActive)
        VALUES (NULL, NULL, NULL, 'DENTAL', N'Dental Consultation Template', NULL, 1);
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

        -- Section 4: Dental History
        INSERT INTO dbo.PrescriptionTemplateSections
        (TemplateId, ClinicId, SourceSectionId, IsCustomized, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsArchived)
        VALUES (@TemplateId, NULL, NULL, 0, N'Dental History', 4, 1, 0, 0, 0, 0, 0);
        SET @SectionId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Dental Pain and Duration', N'Textarea', N'DENTAL_PAIN_AND_DURATION', NULL, 0, 1, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Previous Dental Treatment', N'Textarea', N'PREVIOUS_DENTAL_TREATMENT', NULL, 0, 2, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Oral Hygiene Habits', N'Textarea', N'ORAL_HYGIENE_HABITS', NULL, 0, 3, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Medical History', N'Textarea', N'MEDICAL_HISTORY', NULL, 0, 4, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Allergy History', N'Text', N'ALLERGY_HISTORY', NULL, 0, 5, 1, 0, NULL);

        -- Section 5: Oral Examination
        INSERT INTO dbo.PrescriptionTemplateSections
        (TemplateId, ClinicId, SourceSectionId, IsCustomized, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsArchived)
        VALUES (@TemplateId, NULL, NULL, 0, N'Oral Examination', 5, 1, 0, 0, 0, 0, 0);
        SET @SectionId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Extraoral Examination', N'Textarea', N'EXTRAORAL_EXAMINATION', NULL, 0, 1, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Intraoral Examination', N'Textarea', N'INTRAORAL_EXAMINATION', NULL, 0, 2, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Dentition / Tooth Number', N'Text', N'DENTITION___TOOTH_NUMBER', NULL, 0, 3, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Periodontal Findings', N'Textarea', N'PERIODONTAL_FINDINGS', NULL, 0, 4, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Dental Charting / Findings', N'Textarea', N'DENTAL_CHARTING___FINDINGS', NULL, 0, 5, 1, 0, NULL);

        -- Section 6: Diagnosis
        INSERT INTO dbo.PrescriptionTemplateSections
        (TemplateId, ClinicId, SourceSectionId, IsCustomized, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsArchived)
        VALUES (@TemplateId, NULL, NULL, 0, N'Diagnosis', 6, 1, 0, 0, 0, 0, 0);
        SET @SectionId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Diagnosis', N'Textarea', N'DIAGNOSIS', NULL, 0, 1, 1, 0, NULL);

        -- Section 7: Treatment Plan
        INSERT INTO dbo.PrescriptionTemplateSections
        (TemplateId, ClinicId, SourceSectionId, IsCustomized, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsArchived)
        VALUES (@TemplateId, NULL, NULL, 0, N'Treatment Plan', 7, 1, 0, 0, 0, 0, 0);
        SET @SectionId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Treatment Plan', N'Textarea', N'TREATMENT_PLAN', NULL, 0, 1, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Procedure Performed', N'Textarea', N'PROCEDURE_PERFORMED', NULL, 0, 2, 1, 0, NULL);

        -- Section 8: Medical Advice
        INSERT INTO dbo.PrescriptionTemplateSections
        (TemplateId, ClinicId, SourceSectionId, IsCustomized, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsArchived)
        VALUES (@TemplateId, NULL, NULL, 0, N'Medical Advice', 8, 1, 0, 0, 0, 0, 0);
        SET @SectionId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Medical Advice', N'Textarea', N'MEDICAL_ADVICE', NULL, 0, 1, 1, 0, NULL);

        -- Section 9: Pharmacological Advice
        INSERT INTO dbo.PrescriptionTemplateSections
        (TemplateId, ClinicId, SourceSectionId, IsCustomized, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsArchived)
        VALUES (@TemplateId, NULL, NULL, 0, N'Pharmacological Advice', 9, 1, 1, 1, 0, 0, 0);
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

        -- Section 10: Review and Followup
        INSERT INTO dbo.PrescriptionTemplateSections
        (TemplateId, ClinicId, SourceSectionId, IsCustomized, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsArchived)
        VALUES (@TemplateId, NULL, NULL, 0, N'Review and Followup', 10, 1, 0, 0, 0, 0, 0);
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

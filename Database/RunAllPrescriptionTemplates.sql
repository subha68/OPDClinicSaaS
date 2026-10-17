-- ClinicsRUs global prescription templates
-- Creates/updates all 11 procedures, then executes them in one run.
-- Run in the target database in SQL Server Management Studio.
-- WARNING: Each procedure replaces its matching global template. If clinic/doctor
-- templates already reference global sections/fields/options, FK constraints may block deletion.
USE OPDClinicDB;
GO

-- ==================== DEFAULT ====================
-- Creates/replaces only the global DEFAULT prescription template.
-- Execute with: EXEC dbo.[DEFAULTTemplate];
CREATE OR ALTER PROCEDURE dbo.[DEFAULTTemplate]
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
        WHERE T.ClinicId IS NULL AND T.DepartmentId IS NULL AND T.DoctorId IS NULL AND T.TemplateCode = 'DEFAULT';
        DELETE F
        FROM dbo.PrescriptionTemplateFields AS F
        INNER JOIN dbo.PrescriptionTemplates AS T ON T.TemplateId = F.TemplateId
        WHERE T.ClinicId IS NULL AND T.DepartmentId IS NULL AND T.DoctorId IS NULL AND T.TemplateCode = 'DEFAULT';
        DELETE S
        FROM dbo.PrescriptionTemplateSections AS S
        INNER JOIN dbo.PrescriptionTemplates AS T ON T.TemplateId = S.TemplateId
        WHERE T.ClinicId IS NULL AND T.DepartmentId IS NULL AND T.DoctorId IS NULL AND T.TemplateCode = 'DEFAULT';
        DELETE FROM dbo.PrescriptionTemplates
        WHERE ClinicId IS NULL AND DepartmentId IS NULL AND DoctorId IS NULL AND TemplateCode = 'DEFAULT';

        INSERT INTO dbo.PrescriptionTemplates
        (ClinicId, DepartmentId, DoctorId, TemplateCode, TemplateName, SourceTemplateId, IsActive)
        VALUES (NULL, NULL, NULL, 'DEFAULT', N'Default Consultation Template', NULL, 1);
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

        -- Section 4: Past Illnesses
        INSERT INTO dbo.PrescriptionTemplateSections
        (TemplateId, ClinicId, SourceSectionId, IsCustomized, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsArchived)
        VALUES (@TemplateId, NULL, NULL, 0, N'Past Illnesses', 4, 1, 0, 0, 0, 0, 0);
        SET @SectionId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Past Illnesses', N'Textarea', N'PAST_ILLNESSES', NULL, 0, 1, 1, 0, NULL);

        -- Section 5: History of Present Illness
        INSERT INTO dbo.PrescriptionTemplateSections
        (TemplateId, ClinicId, SourceSectionId, IsCustomized, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsArchived)
        VALUES (@TemplateId, NULL, NULL, 0, N'History of Present Illness', 5, 1, 0, 0, 0, 0, 0);
        SET @SectionId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'History of Present Illness', N'Textarea', N'HOPI', NULL, 0, 1, 1, 0, NULL);

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

-- ==================== CARDIO ====================
-- Creates/replaces only the global CARDIO prescription template.
-- Execute with: EXEC dbo.[CARDIOTemplate];
CREATE OR ALTER PROCEDURE dbo.[CARDIOTemplate]
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
        WHERE T.ClinicId IS NULL AND T.DepartmentId IS NULL AND T.DoctorId IS NULL AND T.TemplateCode = 'CARDIO';
        DELETE F
        FROM dbo.PrescriptionTemplateFields AS F
        INNER JOIN dbo.PrescriptionTemplates AS T ON T.TemplateId = F.TemplateId
        WHERE T.ClinicId IS NULL AND T.DepartmentId IS NULL AND T.DoctorId IS NULL AND T.TemplateCode = 'CARDIO';
        DELETE S
        FROM dbo.PrescriptionTemplateSections AS S
        INNER JOIN dbo.PrescriptionTemplates AS T ON T.TemplateId = S.TemplateId
        WHERE T.ClinicId IS NULL AND T.DepartmentId IS NULL AND T.DoctorId IS NULL AND T.TemplateCode = 'CARDIO';
        DELETE FROM dbo.PrescriptionTemplates
        WHERE ClinicId IS NULL AND DepartmentId IS NULL AND DoctorId IS NULL AND TemplateCode = 'CARDIO';

        INSERT INTO dbo.PrescriptionTemplates
        (ClinicId, DepartmentId, DoctorId, TemplateCode, TemplateName, SourceTemplateId, IsActive)
        VALUES (NULL, NULL, NULL, 'CARDIO', N'Cardiology Consultation Template', NULL, 1);
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

        -- Section 4: Cardiac History
        INSERT INTO dbo.PrescriptionTemplateSections
        (TemplateId, ClinicId, SourceSectionId, IsCustomized, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsArchived)
        VALUES (@TemplateId, NULL, NULL, 0, N'Cardiac History', 4, 1, 0, 0, 0, 0, 0);
        SET @SectionId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Presenting Cardiac Symptoms', N'Textarea', N'PRESENTING_CARDIAC_SYMPTOMS', NULL, 0, 1, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Onset and Duration', N'Textarea', N'ONSET_AND_DURATION', NULL, 0, 2, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Exertional Symptoms', N'Textarea', N'EXERTIONAL_SYMPTOMS', NULL, 0, 3, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Past Cardiac History', N'Textarea', N'PAST_CARDIAC_HISTORY', NULL, 0, 4, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Family History of Cardiac Disease', N'Textarea', N'FAMILY_HISTORY_OF_CARDIAC_DISEASE', NULL, 0, 5, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Risk Factors', N'Textarea', N'RISK_FACTORS', NULL, 0, 6, 1, 0, NULL);

        -- Section 5: Cardiovascular Examination
        INSERT INTO dbo.PrescriptionTemplateSections
        (TemplateId, ClinicId, SourceSectionId, IsCustomized, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsArchived)
        VALUES (@TemplateId, NULL, NULL, 0, N'Cardiovascular Examination', 5, 1, 0, 0, 0, 0, 0);
        SET @SectionId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Pulse and Rhythm', N'Text', N'PULSE_AND_RHYTHM', NULL, 0, 1, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Blood Pressure', N'Text', N'BLOOD_PRESSURE', NULL, 0, 2, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'JVP', N'Text', N'JVP', NULL, 0, 3, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Heart Sounds and Murmurs', N'Textarea', N'HEART_SOUNDS_AND_MURMURS', NULL, 0, 4, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Peripheral Oedema', N'Text', N'PERIPHERAL_OEDEMA', NULL, 0, 5, 1, 0, NULL);

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

-- ==================== DENTAL ====================
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

-- ==================== DERMA ====================
-- Creates/replaces only the global DERMA prescription template.
-- Execute with: EXEC dbo.[DERMATemplate];
CREATE OR ALTER PROCEDURE dbo.[DERMATemplate]
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
        WHERE T.ClinicId IS NULL AND T.DepartmentId IS NULL AND T.DoctorId IS NULL AND T.TemplateCode = 'DERMA';
        DELETE F
        FROM dbo.PrescriptionTemplateFields AS F
        INNER JOIN dbo.PrescriptionTemplates AS T ON T.TemplateId = F.TemplateId
        WHERE T.ClinicId IS NULL AND T.DepartmentId IS NULL AND T.DoctorId IS NULL AND T.TemplateCode = 'DERMA';
        DELETE S
        FROM dbo.PrescriptionTemplateSections AS S
        INNER JOIN dbo.PrescriptionTemplates AS T ON T.TemplateId = S.TemplateId
        WHERE T.ClinicId IS NULL AND T.DepartmentId IS NULL AND T.DoctorId IS NULL AND T.TemplateCode = 'DERMA';
        DELETE FROM dbo.PrescriptionTemplates
        WHERE ClinicId IS NULL AND DepartmentId IS NULL AND DoctorId IS NULL AND TemplateCode = 'DERMA';

        INSERT INTO dbo.PrescriptionTemplates
        (ClinicId, DepartmentId, DoctorId, TemplateCode, TemplateName, SourceTemplateId, IsActive)
        VALUES (NULL, NULL, NULL, 'DERMA', N'Dermatology Consultation Template', NULL, 1);
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

        -- Section 4: Dermatological History
        INSERT INTO dbo.PrescriptionTemplateSections
        (TemplateId, ClinicId, SourceSectionId, IsCustomized, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsArchived)
        VALUES (@TemplateId, NULL, NULL, 0, N'Dermatological History', 4, 1, 0, 0, 0, 0, 0);
        SET @SectionId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Duration and Progression', N'Textarea', N'DURATION_AND_PROGRESSION', NULL, 0, 1, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Itching / Pain / Discharge', N'Textarea', N'ITCHING___PAIN___DISCHARGE', NULL, 0, 2, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Previous Treatment', N'Textarea', N'PREVIOUS_TREATMENT', NULL, 0, 3, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Allergy and Drug History', N'Textarea', N'ALLERGY_AND_DRUG_HISTORY', NULL, 0, 4, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Relevant Medical History', N'Textarea', N'RELEVANT_MEDICAL_HISTORY', NULL, 0, 5, 1, 0, NULL);

        -- Section 5: Skin Examination
        INSERT INTO dbo.PrescriptionTemplateSections
        (TemplateId, ClinicId, SourceSectionId, IsCustomized, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsArchived)
        VALUES (@TemplateId, NULL, NULL, 0, N'Skin Examination', 5, 1, 0, 0, 0, 0, 0);
        SET @SectionId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Site and Distribution', N'Textarea', N'SITE_AND_DISTRIBUTION', NULL, 0, 1, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Morphology of Lesions', N'Textarea', N'MORPHOLOGY_OF_LESIONS', NULL, 0, 2, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Colour / Size / Number', N'Textarea', N'COLOUR___SIZE___NUMBER', NULL, 0, 3, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Nails / Hair / Mucosa', N'Textarea', N'NAILS___HAIR___MUCOSA', NULL, 0, 4, 1, 0, NULL);

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
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Skin Care and General Advice', N'Textarea', N'MEDICAL_ADVICE', NULL, 0, 1, 1, 0, NULL);

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

-- ==================== ENT ====================
-- Creates/replaces only the global ENT prescription template.
-- Execute with: EXEC dbo.[ENTTemplate];
CREATE OR ALTER PROCEDURE dbo.[ENTTemplate]
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
        WHERE T.ClinicId IS NULL AND T.DepartmentId IS NULL AND T.DoctorId IS NULL AND T.TemplateCode = 'ENT';
        DELETE F
        FROM dbo.PrescriptionTemplateFields AS F
        INNER JOIN dbo.PrescriptionTemplates AS T ON T.TemplateId = F.TemplateId
        WHERE T.ClinicId IS NULL AND T.DepartmentId IS NULL AND T.DoctorId IS NULL AND T.TemplateCode = 'ENT';
        DELETE S
        FROM dbo.PrescriptionTemplateSections AS S
        INNER JOIN dbo.PrescriptionTemplates AS T ON T.TemplateId = S.TemplateId
        WHERE T.ClinicId IS NULL AND T.DepartmentId IS NULL AND T.DoctorId IS NULL AND T.TemplateCode = 'ENT';
        DELETE FROM dbo.PrescriptionTemplates
        WHERE ClinicId IS NULL AND DepartmentId IS NULL AND DoctorId IS NULL AND TemplateCode = 'ENT';

        INSERT INTO dbo.PrescriptionTemplates
        (ClinicId, DepartmentId, DoctorId, TemplateCode, TemplateName, SourceTemplateId, IsActive)
        VALUES (NULL, NULL, NULL, 'ENT', N'ENT Consultation Template', NULL, 1);
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

        -- Section 4: ENT History
        INSERT INTO dbo.PrescriptionTemplateSections
        (TemplateId, ClinicId, SourceSectionId, IsCustomized, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsArchived)
        VALUES (@TemplateId, NULL, NULL, 0, N'ENT History', 4, 1, 0, 0, 0, 0, 0);
        SET @SectionId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Ear Symptoms', N'Textarea', N'EAR_SYMPTOMS', NULL, 0, 1, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Nasal Symptoms', N'Textarea', N'NASAL_SYMPTOMS', NULL, 0, 2, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Throat / Voice Symptoms', N'Textarea', N'THROAT___VOICE_SYMPTOMS', NULL, 0, 3, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Duration and Progression', N'Textarea', N'DURATION_AND_PROGRESSION', NULL, 0, 4, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Relevant Medical / Allergy History', N'Textarea', N'RELEVANT_MEDICAL___ALLERGY_HISTORY', NULL, 0, 5, 1, 0, NULL);

        -- Section 5: ENT Examination
        INSERT INTO dbo.PrescriptionTemplateSections
        (TemplateId, ClinicId, SourceSectionId, IsCustomized, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsArchived)
        VALUES (@TemplateId, NULL, NULL, 0, N'ENT Examination', 5, 1, 0, 0, 0, 0, 0);
        SET @SectionId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Ear Examination', N'Textarea', N'EAR_EXAMINATION', NULL, 0, 1, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Nose and Sinus Examination', N'Textarea', N'NOSE_AND_SINUS_EXAMINATION', NULL, 0, 2, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Oral Cavity and Throat', N'Textarea', N'ORAL_CAVITY_AND_THROAT', NULL, 0, 3, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Laryngeal / Voice Findings', N'Textarea', N'LARYNGEAL___VOICE_FINDINGS', NULL, 0, 4, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Neck Examination', N'Textarea', N'NECK_EXAMINATION', NULL, 0, 5, 1, 0, NULL);

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

-- ==================== GASTRO ====================
-- Creates/replaces only the global GASTRO prescription template.
-- Execute with: EXEC dbo.[GASTROTemplate];
CREATE OR ALTER PROCEDURE dbo.[GASTROTemplate]
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
        WHERE T.ClinicId IS NULL AND T.DepartmentId IS NULL AND T.DoctorId IS NULL AND T.TemplateCode = 'GASTRO';
        DELETE F
        FROM dbo.PrescriptionTemplateFields AS F
        INNER JOIN dbo.PrescriptionTemplates AS T ON T.TemplateId = F.TemplateId
        WHERE T.ClinicId IS NULL AND T.DepartmentId IS NULL AND T.DoctorId IS NULL AND T.TemplateCode = 'GASTRO';
        DELETE S
        FROM dbo.PrescriptionTemplateSections AS S
        INNER JOIN dbo.PrescriptionTemplates AS T ON T.TemplateId = S.TemplateId
        WHERE T.ClinicId IS NULL AND T.DepartmentId IS NULL AND T.DoctorId IS NULL AND T.TemplateCode = 'GASTRO';
        DELETE FROM dbo.PrescriptionTemplates
        WHERE ClinicId IS NULL AND DepartmentId IS NULL AND DoctorId IS NULL AND TemplateCode = 'GASTRO';

        INSERT INTO dbo.PrescriptionTemplates
        (ClinicId, DepartmentId, DoctorId, TemplateCode, TemplateName, SourceTemplateId, IsActive)
        VALUES (NULL, NULL, NULL, 'GASTRO', N'Gastroenterology Consultation Template', NULL, 1);
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

        -- Section 4: Gastrointestinal History
        INSERT INTO dbo.PrescriptionTemplateSections
        (TemplateId, ClinicId, SourceSectionId, IsCustomized, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsArchived)
        VALUES (@TemplateId, NULL, NULL, 0, N'Gastrointestinal History', 4, 1, 0, 0, 0, 0, 0);
        SET @SectionId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Abdominal Pain and Duration', N'Textarea', N'ABDOMINAL_PAIN_AND_DURATION', NULL, 0, 1, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Bowel Habits', N'Textarea', N'BOWEL_HABITS', NULL, 0, 2, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Nausea / Vomiting / Reflux', N'Textarea', N'NAUSEA___VOMITING___REFLUX', NULL, 0, 3, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Appetite and Weight Change', N'Textarea', N'APPETITE_AND_WEIGHT_CHANGE', NULL, 0, 4, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Diet and Alcohol History', N'Textarea', N'DIET_AND_ALCOHOL_HISTORY', NULL, 0, 5, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Past GI / Liver History', N'Textarea', N'PAST_GI___LIVER_HISTORY', NULL, 0, 6, 1, 0, NULL);

        -- Section 5: Abdominal Examination
        INSERT INTO dbo.PrescriptionTemplateSections
        (TemplateId, ClinicId, SourceSectionId, IsCustomized, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsArchived)
        VALUES (@TemplateId, NULL, NULL, 0, N'Abdominal Examination', 5, 1, 0, 0, 0, 0, 0);
        SET @SectionId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Inspection', N'Textarea', N'INSPECTION', NULL, 0, 1, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Tenderness / Palpation', N'Textarea', N'TENDERNESS___PALPATION', NULL, 0, 2, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Organomegaly / Mass', N'Textarea', N'ORGANOMEGALY___MASS', NULL, 0, 3, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Bowel Sounds', N'Text', N'BOWEL_SOUNDS', NULL, 0, 4, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Other Findings', N'Textarea', N'OTHER_FINDINGS', NULL, 0, 5, 1, 0, NULL);

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
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Dietary and Lifestyle Advice', N'Textarea', N'MEDICAL_ADVICE', NULL, 0, 1, 1, 0, NULL);

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

-- ==================== DIABETOLOGY ====================
-- Creates/replaces only the global DIABETOLOGY prescription template.
-- Execute with: EXEC dbo.[DIABETOLOGYTemplate];
CREATE OR ALTER PROCEDURE dbo.[DIABETOLOGYTemplate]
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
        WHERE T.ClinicId IS NULL AND T.DepartmentId IS NULL AND T.DoctorId IS NULL AND T.TemplateCode = 'DIABETOLOGY';
        DELETE F
        FROM dbo.PrescriptionTemplateFields AS F
        INNER JOIN dbo.PrescriptionTemplates AS T ON T.TemplateId = F.TemplateId
        WHERE T.ClinicId IS NULL AND T.DepartmentId IS NULL AND T.DoctorId IS NULL AND T.TemplateCode = 'DIABETOLOGY';
        DELETE S
        FROM dbo.PrescriptionTemplateSections AS S
        INNER JOIN dbo.PrescriptionTemplates AS T ON T.TemplateId = S.TemplateId
        WHERE T.ClinicId IS NULL AND T.DepartmentId IS NULL AND T.DoctorId IS NULL AND T.TemplateCode = 'DIABETOLOGY';
        DELETE FROM dbo.PrescriptionTemplates
        WHERE ClinicId IS NULL AND DepartmentId IS NULL AND DoctorId IS NULL AND TemplateCode = 'DIABETOLOGY';

        INSERT INTO dbo.PrescriptionTemplates
        (ClinicId, DepartmentId, DoctorId, TemplateCode, TemplateName, SourceTemplateId, IsActive)
        VALUES (NULL, NULL, NULL, 'DIABETOLOGY', N'Diabetology and Endocrinology Consultation Template', NULL, 1);
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

        -- Section 4: Diabetes and Endocrine History
        INSERT INTO dbo.PrescriptionTemplateSections
        (TemplateId, ClinicId, SourceSectionId, IsCustomized, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsArchived)
        VALUES (@TemplateId, NULL, NULL, 0, N'Diabetes and Endocrine History', 4, 1, 0, 0, 0, 0, 0);
        SET @SectionId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Diabetes Duration and Type', N'Textarea', N'DIABETES_DURATION_AND_TYPE', NULL, 0, 1, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Current Treatment', N'Textarea', N'CURRENT_TREATMENT', NULL, 0, 2, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Home Glucose Monitoring', N'Textarea', N'HOME_GLUCOSE_MONITORING', NULL, 0, 3, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Hypoglycaemia History', N'Textarea', N'HYPOGLYCAEMIA_HISTORY', NULL, 0, 4, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Family History', N'Textarea', N'FAMILY_HISTORY', NULL, 0, 5, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Comorbidities', N'Textarea', N'COMORBIDITIES', NULL, 0, 6, 1, 0, NULL);

        -- Section 5: Diabetes / Endocrine Examination
        INSERT INTO dbo.PrescriptionTemplateSections
        (TemplateId, ClinicId, SourceSectionId, IsCustomized, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsArchived)
        VALUES (@TemplateId, NULL, NULL, 0, N'Diabetes / Endocrine Examination', 5, 1, 0, 0, 0, 0, 0);
        SET @SectionId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Weight and BMI', N'Text', N'WEIGHT_AND_BMI', NULL, 0, 1, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Foot Examination', N'Textarea', N'FOOT_EXAMINATION', NULL, 0, 2, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Peripheral Neuropathy', N'Textarea', N'PERIPHERAL_NEUROPATHY', NULL, 0, 3, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Thyroid / Endocrine Findings', N'Textarea', N'THYROID___ENDOCRINE_FINDINGS', NULL, 0, 4, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Complications Screening', N'Textarea', N'COMPLICATIONS_SCREENING', NULL, 0, 5, 1, 0, NULL);

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
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Diet, Activity and Self-monitoring Advice', N'Textarea', N'MEDICAL_ADVICE', NULL, 0, 1, 1, 0, NULL);

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

-- ==================== OBSGYNAE ====================
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

-- ==================== OPTHA ====================
-- Creates/replaces only the global OPTHA prescription template.
-- Execute with: EXEC dbo.[OPTHATemplate];
CREATE OR ALTER PROCEDURE dbo.[OPTHATemplate]
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
        WHERE T.ClinicId IS NULL AND T.DepartmentId IS NULL AND T.DoctorId IS NULL AND T.TemplateCode = 'OPTHA';
        DELETE F
        FROM dbo.PrescriptionTemplateFields AS F
        INNER JOIN dbo.PrescriptionTemplates AS T ON T.TemplateId = F.TemplateId
        WHERE T.ClinicId IS NULL AND T.DepartmentId IS NULL AND T.DoctorId IS NULL AND T.TemplateCode = 'OPTHA';
        DELETE S
        FROM dbo.PrescriptionTemplateSections AS S
        INNER JOIN dbo.PrescriptionTemplates AS T ON T.TemplateId = S.TemplateId
        WHERE T.ClinicId IS NULL AND T.DepartmentId IS NULL AND T.DoctorId IS NULL AND T.TemplateCode = 'OPTHA';
        DELETE FROM dbo.PrescriptionTemplates
        WHERE ClinicId IS NULL AND DepartmentId IS NULL AND DoctorId IS NULL AND TemplateCode = 'OPTHA';

        INSERT INTO dbo.PrescriptionTemplates
        (ClinicId, DepartmentId, DoctorId, TemplateCode, TemplateName, SourceTemplateId, IsActive)
        VALUES (NULL, NULL, NULL, 'OPTHA', N'Ophthalmology Consultation Template', NULL, 1);
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

        -- Section 4: Ophthalmic History
        INSERT INTO dbo.PrescriptionTemplateSections
        (TemplateId, ClinicId, SourceSectionId, IsCustomized, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsArchived)
        VALUES (@TemplateId, NULL, NULL, 0, N'Ophthalmic History', 4, 1, 0, 0, 0, 0, 0);
        SET @SectionId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Visual Symptoms', N'Textarea', N'VISUAL_SYMPTOMS', NULL, 0, 1, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Onset and Duration', N'Textarea', N'ONSET_AND_DURATION', NULL, 0, 2, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Ocular / Surgical History', N'Textarea', N'OCULAR___SURGICAL_HISTORY', NULL, 0, 3, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Spectacle / Contact Lens History', N'Textarea', N'SPECTACLE___CONTACT_LENS_HISTORY', NULL, 0, 4, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Relevant Medical History', N'Textarea', N'RELEVANT_MEDICAL_HISTORY', NULL, 0, 5, 1, 0, NULL);

        -- Section 5: Eye Examination
        INSERT INTO dbo.PrescriptionTemplateSections
        (TemplateId, ClinicId, SourceSectionId, IsCustomized, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsArchived)
        VALUES (@TemplateId, NULL, NULL, 0, N'Eye Examination', 5, 1, 0, 0, 0, 0, 0);
        SET @SectionId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Visual Acuity', N'Text', N'VISUAL_ACUITY', NULL, 0, 1, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Refraction', N'Text', N'REFRACTION', NULL, 0, 2, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Anterior Segment', N'Textarea', N'ANTERIOR_SEGMENT', NULL, 0, 3, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Pupils and Ocular Motility', N'Textarea', N'PUPILS_AND_OCULAR_MOTILITY', NULL, 0, 4, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Intraocular Pressure', N'Text', N'INTRAOCULAR_PRESSURE', NULL, 0, 5, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Fundus Examination', N'Textarea', N'FUNDUS_EXAMINATION', NULL, 0, 6, 1, 0, NULL);

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

-- ==================== ORTHO ====================
-- Creates/replaces only the global ORTHO prescription template.
-- Execute with: EXEC dbo.[ORTHOTemplate];
CREATE OR ALTER PROCEDURE dbo.[ORTHOTemplate]
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
        WHERE T.ClinicId IS NULL AND T.DepartmentId IS NULL AND T.DoctorId IS NULL AND T.TemplateCode = 'ORTHO';
        DELETE F
        FROM dbo.PrescriptionTemplateFields AS F
        INNER JOIN dbo.PrescriptionTemplates AS T ON T.TemplateId = F.TemplateId
        WHERE T.ClinicId IS NULL AND T.DepartmentId IS NULL AND T.DoctorId IS NULL AND T.TemplateCode = 'ORTHO';
        DELETE S
        FROM dbo.PrescriptionTemplateSections AS S
        INNER JOIN dbo.PrescriptionTemplates AS T ON T.TemplateId = S.TemplateId
        WHERE T.ClinicId IS NULL AND T.DepartmentId IS NULL AND T.DoctorId IS NULL AND T.TemplateCode = 'ORTHO';
        DELETE FROM dbo.PrescriptionTemplates
        WHERE ClinicId IS NULL AND DepartmentId IS NULL AND DoctorId IS NULL AND TemplateCode = 'ORTHO';

        INSERT INTO dbo.PrescriptionTemplates
        (ClinicId, DepartmentId, DoctorId, TemplateCode, TemplateName, SourceTemplateId, IsActive)
        VALUES (NULL, NULL, NULL, 'ORTHO', N'Orthopedics Consultation Template', NULL, 1);
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

        -- Section 4: Musculoskeletal History
        INSERT INTO dbo.PrescriptionTemplateSections
        (TemplateId, ClinicId, SourceSectionId, IsCustomized, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsArchived)
        VALUES (@TemplateId, NULL, NULL, 0, N'Musculoskeletal History', 4, 1, 0, 0, 0, 0, 0);
        SET @SectionId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Site and Duration of Pain', N'Textarea', N'SITE_AND_DURATION_OF_PAIN', NULL, 0, 1, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Injury / Mechanism', N'Textarea', N'INJURY___MECHANISM', NULL, 0, 2, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Functional Limitation', N'Textarea', N'FUNCTIONAL_LIMITATION', NULL, 0, 3, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Previous Treatment', N'Textarea', N'PREVIOUS_TREATMENT', NULL, 0, 4, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Relevant Medical History', N'Textarea', N'RELEVANT_MEDICAL_HISTORY', NULL, 0, 5, 1, 0, NULL);

        -- Section 5: Musculoskeletal Examination
        INSERT INTO dbo.PrescriptionTemplateSections
        (TemplateId, ClinicId, SourceSectionId, IsCustomized, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsArchived)
        VALUES (@TemplateId, NULL, NULL, 0, N'Musculoskeletal Examination', 5, 1, 0, 0, 0, 0, 0);
        SET @SectionId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Inspection', N'Textarea', N'INSPECTION', NULL, 0, 1, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Tenderness and Range of Motion', N'Textarea', N'TENDERNESS_AND_RANGE_OF_MOTION', NULL, 0, 2, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Deformity / Swelling', N'Textarea', N'DEFORMITY___SWELLING', NULL, 0, 3, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Neurovascular Status', N'Textarea', N'NEUROVASCULAR_STATUS', NULL, 0, 4, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Gait / Functional Assessment', N'Textarea', N'GAIT___FUNCTIONAL_ASSESSMENT', NULL, 0, 5, 1, 0, NULL);

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

-- ==================== PEDIA ====================
-- Creates/replaces only the global PEDIA prescription template.
-- Execute with: EXEC dbo.[PEDIATemplate];
CREATE OR ALTER PROCEDURE dbo.[PEDIATemplate]
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
        WHERE T.ClinicId IS NULL AND T.DepartmentId IS NULL AND T.DoctorId IS NULL AND T.TemplateCode = 'PEDIA';
        DELETE F
        FROM dbo.PrescriptionTemplateFields AS F
        INNER JOIN dbo.PrescriptionTemplates AS T ON T.TemplateId = F.TemplateId
        WHERE T.ClinicId IS NULL AND T.DepartmentId IS NULL AND T.DoctorId IS NULL AND T.TemplateCode = 'PEDIA';
        DELETE S
        FROM dbo.PrescriptionTemplateSections AS S
        INNER JOIN dbo.PrescriptionTemplates AS T ON T.TemplateId = S.TemplateId
        WHERE T.ClinicId IS NULL AND T.DepartmentId IS NULL AND T.DoctorId IS NULL AND T.TemplateCode = 'PEDIA';
        DELETE FROM dbo.PrescriptionTemplates
        WHERE ClinicId IS NULL AND DepartmentId IS NULL AND DoctorId IS NULL AND TemplateCode = 'PEDIA';

        INSERT INTO dbo.PrescriptionTemplates
        (ClinicId, DepartmentId, DoctorId, TemplateCode, TemplateName, SourceTemplateId, IsActive)
        VALUES (NULL, NULL, NULL, 'PEDIA', N'Pediatrics Consultation Template', NULL, 1);
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

        -- Section 4: Birth and Development History
        INSERT INTO dbo.PrescriptionTemplateSections
        (TemplateId, ClinicId, SourceSectionId, IsCustomized, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsArchived)
        VALUES (@TemplateId, NULL, NULL, 0, N'Birth and Development History', 4, 1, 0, 0, 0, 0, 0);
        SET @SectionId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Birth History', N'Textarea', N'BIRTH_HISTORY', NULL, 0, 1, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Developmental Milestones', N'Textarea', N'DEVELOPMENTAL_MILESTONES', NULL, 0, 2, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Immunization History', N'Textarea', N'IMMUNIZATION_HISTORY', NULL, 0, 3, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Feeding and Nutrition', N'Textarea', N'FEEDING_AND_NUTRITION', NULL, 0, 4, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Family History', N'Textarea', N'FAMILY_HISTORY', NULL, 0, 5, 1, 0, NULL);

        -- Section 5: Paediatric Examination
        INSERT INTO dbo.PrescriptionTemplateSections
        (TemplateId, ClinicId, SourceSectionId, IsCustomized, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice, IsVital, IsTestResult, IsArchived)
        VALUES (@TemplateId, NULL, NULL, 0, N'Paediatric Examination', 5, 1, 0, 0, 0, 0, 0);
        SET @SectionId = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'General Appearance', N'Textarea', N'GENERAL_APPEARANCE', NULL, 0, 1, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Growth Parameters', N'Text', N'GROWTH_PARAMETERS', NULL, 0, 2, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Systemic Examination', N'Textarea', N'SYSTEMIC_EXAMINATION', NULL, 0, 3, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Developmental Assessment', N'Textarea', N'DEVELOPMENTAL_ASSESSMENT', NULL, 0, 4, 1, 0, NULL);

        INSERT INTO dbo.PrescriptionTemplateFields
        (SectionId, TemplateId, ClinicId, SourceFieldId, IsCustomized, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder, IsActive, IsArchived, MergeWith)
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Hydration / Nutrition Status', N'Textarea', N'HYDRATION___NUTRITION_STATUS', NULL, 0, 5, 1, 0, NULL);

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
        VALUES (@SectionId, @TemplateId, NULL, NULL, 0, N'Parent / Caregiver Advice', N'Textarea', N'MEDICAL_ADVICE', NULL, 0, 1, 1, 0, NULL);

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

-- ==================== EXECUTE ALL TEMPLATES ====================
EXEC dbo.[DEFAULTTemplate];
EXEC dbo.[CARDIOTemplate];
EXEC dbo.[DENTALTemplate];
EXEC dbo.[DERMATemplate];
EXEC dbo.[ENTTemplate];
EXEC dbo.[GASTROTemplate];
EXEC dbo.[DIABETOLOGYTemplate];
EXEC dbo.[OBSGYNAETemplate];
EXEC dbo.[OPTHATemplate];
EXEC dbo.[ORTHOTemplate];
EXEC dbo.[PEDIATemplate];
GO

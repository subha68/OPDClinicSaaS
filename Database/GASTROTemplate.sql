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

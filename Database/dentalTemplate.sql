-- EXEC dbo.GenerateDefaultDentalTemplate;

CREATE OR ALTER PROCEDURE dbo.GenerateDefaultDentalTemplate
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY

        BEGIN TRANSACTION;

        DECLARE @TemplateId INT;
        DECLARE @SectionId INT;
        DECLARE @FieldId INT;

        /* ============================================================
           TEMPLATE
        ============================================================ */

        INSERT INTO PrescriptionTemplates
        (
            ClinicId, DepartmentId, DoctorId, TemplateCode, TemplateName, IsActive
        )
        VALUES
        (
            NULL, NULL, NULL, 'DENTAL', 'Dental Template', 1
        );

        SET @TemplateId = SCOPE_IDENTITY();


        /* ============================================================
           1. CHIEF COMPLAINTS
        ============================================================ */

        INSERT INTO PrescriptionTemplateSections
        (
            TemplateId, ClinicId, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice
        )
        VALUES
        (
            @TemplateId, NULL, 'Chief Complaints', 1, 1, 0, 0
        );

        SET @SectionId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFields
        (
            SectionId, TemplateId, ClinicId, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder,
            IsActive, IsArchived, MergeWith
        )
        VALUES
        (
            @SectionId, @TemplateId, NULL, 'Symptoms', 'CheckboxGroup', 'SYMPTOMS', NULL, 0, 1, 1, 0, NULL
        );

        SET @FieldId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFieldOptions
        (
            FieldId, SectionId, TemplateId, ClinicId, OptionText, IsRequired, DisplayOrder, IsActive, IsArchived
        )
        VALUES
        (@FieldId, @SectionId, @TemplateId, NULL, 'Tooth Pain',             0, 1, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Sensitivity',            0, 2, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Swelling',               0, 3, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Bleeding Gums',          0, 4, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Bad Breath',             0, 5, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Loose Tooth',             0, 6, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Difficulty Chewing',      0, 7, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Jaw Pain',                0, 8, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Difficulty Opening Mouth', 0, 9, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Ulcer / Oral Lesion',     0, 10, 1, 0);


        /* ============================================================
           2. DENTAL HISTORY
        ============================================================ */

        INSERT INTO PrescriptionTemplateSections
        (
            TemplateId, ClinicId, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice
        )
        VALUES
        (
            @TemplateId, NULL, 'Dental History', 2, 1, 0, 0
        );

        SET @SectionId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFields
        (
            SectionId, TemplateId, ClinicId, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder,
            IsActive, IsArchived, MergeWith
        )
        VALUES
        (
            @SectionId, @TemplateId, NULL, 'Dental History', 'TextArea', 'DENTAL_HISTORY', NULL, 0, 1, 1, 0, NULL
        );


        /* ============================================================
           3. ORAL EXAMINATION
        ============================================================ */

        INSERT INTO PrescriptionTemplateSections
        (
            TemplateId, ClinicId, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice
        )
        VALUES
        (
            @TemplateId, NULL, 'Oral Examination', 3, 1, 0, 0
        );

        SET @SectionId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFields
        (
            SectionId, TemplateId, ClinicId, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder,
            IsActive, IsArchived, MergeWith
        )
        VALUES
        (@SectionId, @TemplateId, NULL, 'Teeth', 'TextArea', 'TEETH_EXAMINATION', NULL, 0, 1, 1, 0, NULL),
        (@SectionId, @TemplateId, NULL, 'Gingiva', 'TextArea', 'GINGIVA_EXAMINATION', NULL, 0, 2, 1, 0, NULL),
        (@SectionId, @TemplateId, NULL, 'Oral Mucosa', 'TextArea', 'ORAL_MUCOSA_EXAMINATION', NULL, 0, 3, 1, 0, NULL),
        (@SectionId, @TemplateId, NULL, 'Tongue', 'TextArea', 'TONGUE_EXAMINATION', NULL, 0, 4, 1, 0, NULL),
        (@SectionId, @TemplateId, NULL, 'Palate', 'TextArea', 'PALATE_EXAMINATION', NULL, 0, 5, 1, 0, NULL),
        (@SectionId, @TemplateId, NULL, 'Occlusion', 'TextArea', 'OCCLUSION_EXAMINATION', NULL, 0, 6, 1, 0, NULL);

        /* ============================================================
           4. PERIODONTAL EXAMINATION
        ============================================================ */

        INSERT INTO PrescriptionTemplateSections
        (
            TemplateId, ClinicId, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice
        )
        VALUES
        (
            @TemplateId, NULL, 'Periodontal Examination', 4, 1, 0, 0
        );

        SET @SectionId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFields
        (
            SectionId, TemplateId, ClinicId, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder,
            IsActive, IsArchived, MergeWith
        )
        VALUES
        (@SectionId, @TemplateId, NULL, 'Gingival Condition', 'TextArea', 'GINGIVAL_CONDITION', NULL, 0, 1, 1, 0, NULL),
        (@SectionId, @TemplateId, NULL, 'Periodontal Pockets', 'TextArea', 'PERIODONTAL_POCKETS', NULL, 0, 2, 1, 0, NULL),
        (@SectionId, @TemplateId, NULL, 'Mobility', 'TextArea', 'TOOTH_MOBILITY', NULL, 0, 3, 1, 0, NULL),
        (@SectionId, @TemplateId, NULL, 'Calculus / Plaque', 'TextArea', 'CALCULUS_PLAQUE', NULL, 0, 4, 1, 0, NULL);

        /* ============================================================
           5. DIAGNOSIS
        ============================================================ */

        INSERT INTO PrescriptionTemplateSections
        (
            TemplateId, ClinicId, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice
        )
        VALUES
        (
            @TemplateId, NULL, 'Diagnosis', 5, 1, 0, 0
        );

        SET @SectionId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFields
        (
            SectionId, TemplateId, ClinicId, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder,
            IsActive, IsArchived, MergeWith
        )
        VALUES
        (
            @SectionId, @TemplateId, NULL, 'Diagnosis', 'TextArea', 'DIAGNOSIS', NULL, 1, 1, 1, 0, NULL
        );


        /* ============================================================
           6. PROCEDURE / TREATMENT
        ============================================================ */

        INSERT INTO PrescriptionTemplateSections
        (
            TemplateId, ClinicId, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice
        )
        VALUES
        (
            @TemplateId, NULL, 'Procedure / Treatment', 6, 1, 0, 0
        );

        SET @SectionId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFields
        (
            SectionId, TemplateId, ClinicId, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder,
            IsActive, IsArchived, MergeWith
        )
        VALUES
        (
            @SectionId, @TemplateId, NULL, 'Procedure / Treatment', 'CheckboxGroup', 'PROCEDURE_TREATMENT', NULL, 0, 1, 1, 0, NULL
        );

        SET @FieldId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFieldOptions
        (
            FieldId, SectionId, TemplateId, ClinicId, OptionText, IsRequired, DisplayOrder, IsActive, IsArchived
        )
        VALUES
        (@FieldId, @SectionId, @TemplateId, NULL, 'Scaling',                    0, 1, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Root Planing',               0, 2, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Dental Filling',              0, 3, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Root Canal Treatment',       0, 4, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Tooth Extraction',            0, 5, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Crown',                       0, 6, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Bridge',                      0, 7, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Denture',                     0, 8, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Implant Evaluation',          0, 9, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Orthodontic Evaluation',      0, 10, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Oral Hygiene Instruction',    0, 11, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Review / Observation',        0, 12, 1, 0);


        /* ============================================================
           7. ADVICE
        ============================================================ */

        INSERT INTO PrescriptionTemplateSections
        (
            TemplateId, ClinicId, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice
        )
        VALUES
        (
            @TemplateId, NULL, 'Advice', 7, 1, 0, 0
        );

        SET @SectionId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFields
        (
            SectionId, TemplateId, ClinicId, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder,
            IsActive, IsArchived, MergeWith
        )
        VALUES
        (
            @SectionId, @TemplateId, NULL, 'Dental Advice', 'CheckboxGroup', 'DENTAL_ADVICE', NULL, 0, 1, 1, 0, NULL
        );

        SET @FieldId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFieldOptions
        (
            FieldId, SectionId, TemplateId, ClinicId, OptionText, IsRequired, DisplayOrder, IsActive, IsArchived
        )
        VALUES
        (@FieldId, @SectionId, @TemplateId, NULL, 'Brush Twice Daily',              0, 1, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Use Fluoride Toothpaste',         0, 2, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Use Dental Floss',                0, 3, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Use Interdental Brush',            0, 4, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Avoid Tobacco',                    0, 5, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Avoid Excessive Sugar',            0, 6, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Salt Water Rinse',                 0, 7, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Maintain Oral Hygiene',            0, 8, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Regular Dental Check-up',          0, 9, 1, 0);


        /* ============================================================
           8. PHARMACOLOGICAL ADVICE
           
           IMPORTANT:
           All six fields are included:
           Medicine
           Dose
           Timing
           Duration
           Unit
           Instructions
        ============================================================ */

        INSERT INTO PrescriptionTemplateSections
        (
            TemplateId, ClinicId, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice
        )
        VALUES
        (
            @TemplateId, NULL, 'Pharmacological Advice', 8, 1, 1, 1
        );

        SET @SectionId = SCOPE_IDENTITY();


        /* ------------------------------------------------------------
        Medicine
        ------------------------------------------------------------ */

        INSERT INTO PrescriptionTemplateFields
        (
            SectionId, TemplateId, ClinicId, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder,
            IsActive, IsArchived, MergeWith
        )
        VALUES
        (
            @SectionId, @TemplateId, NULL, 'Medicine', 'Lookup', 'Medicine', 'Medicines', 1, 1,
            1, 0, NULL
        );


        /* ------------------------------------------------------------
        Dose
        ------------------------------------------------------------ */

        INSERT INTO PrescriptionTemplateFields
        (
            SectionId, TemplateId, ClinicId, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder,
            IsActive, IsArchived, MergeWith
        )
        VALUES
        (
            @SectionId, @TemplateId, NULL, 'Dose', 'Dropdown', 'Dose', NULL, 1, 2, 
            1, 0, NULL
        );

        SET @FieldId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFieldOptions
        (
            FieldId, SectionId, TemplateId, ClinicId, OptionText, IsRequired, DisplayOrder, IsActive, IsArchived
        )
        VALUES
        (@FieldId, @SectionId, @TemplateId, NULL, 'Once Daily',   0, 1, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Twice Daily',  0, 2, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Thrice Daily', 0, 3, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'SOS',          0, 4, 1, 0);


        /* ------------------------------------------------------------
        Timing
        ------------------------------------------------------------ */

        INSERT INTO PrescriptionTemplateFields
        (
            SectionId, TemplateId, ClinicId, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder,
            IsActive, IsArchived, MergeWith
        )
        VALUES
        (
            @SectionId, @TemplateId, NULL, 'Timing', 'Dropdown', 'Timing', NULL, 1, 3,
            1, 0, NULL
        );

        SET @FieldId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFieldOptions
        (
            FieldId, SectionId, TemplateId, ClinicId, OptionText, IsRequired, DisplayOrder, IsActive, IsArchived
        )
        VALUES
        (@FieldId, @SectionId, @TemplateId, NULL, 'Before Meal', 0, 1, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'After Meal',  0, 2, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'During Meal', 0, 3, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'At Bedtime',  0, 4, 1, 0);


        /* ------------------------------------------------------------
        Duration
        ------------------------------------------------------------ */

        INSERT INTO PrescriptionTemplateFields
        (
            SectionId, TemplateId, ClinicId, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder,
            IsActive, IsArchived, MergeWith
        )
        VALUES
        (
            @SectionId, @TemplateId, NULL, 'Duration', 'Number', 'Duration', NULL, 1, 4,
            1, 0, NULL
        );


        /* ------------------------------------------------------------
        Unit
        ------------------------------------------------------------ */

        INSERT INTO PrescriptionTemplateFields
        (
            SectionId, TemplateId, ClinicId, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder,
            IsActive, IsArchived, MergeWith
        )
        VALUES
        (
            @SectionId, @TemplateId, NULL, 'Unit', 'Dropdown', 'Unit', NULL, 1, 5,
            1, 0, NULL
        );

        SET @FieldId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFieldOptions
        (
            FieldId, SectionId, TemplateId, ClinicId, OptionText, IsRequired, DisplayOrder, IsActive, IsArchived
        )
        VALUES
        (@FieldId, @SectionId, @TemplateId, NULL, 'Days',   0, 1, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Weeks',  0, 2, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Months', 0, 3, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Years',  0, 4, 1, 0);


        /* ------------------------------------------------------------
        Instructions
        ------------------------------------------------------------ */

        INSERT INTO PrescriptionTemplateFields
        (
            SectionId, TemplateId, ClinicId, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder,
            IsActive, IsArchived, MergeWith
        )
        VALUES
        (
            @SectionId, @TemplateId, NULL, 'Instructions', 'Text', 'Instruction', NULL, 0, 6, 
            1, 0, NULL
        );


        /* ============================================================
           9. FOLLOW UP
        ============================================================ */

        INSERT INTO PrescriptionTemplateSections
        (
            TemplateId, ClinicId, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice
        )
        VALUES
        (
            @TemplateId, NULL, 'Follow Up', 9, 1, 0, 0
        );

        SET @SectionId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFields
        (
            SectionId, TemplateId, ClinicId, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder,
            IsActive, IsArchived, MergeWith
        )
        VALUES
        (@SectionId, @TemplateId, NULL, 'Review After', 'Text', 'REVIEW_AFTER', NULL, 0, 1, 1, 0, NULL),
        (@SectionId, @TemplateId, NULL, 'Next Visit Date', 'Date', 'NEXT_VISIT_DATE', NULL, 0, 2, 1, 0, NULL);


        /* ============================================================
           COMMIT
        ============================================================ */

        COMMIT TRANSACTION;

    END TRY

    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();

        RAISERROR
        (
            @ErrorMessage,
            @ErrorSeverity,
            @ErrorState
        );

    END CATCH
END
GO
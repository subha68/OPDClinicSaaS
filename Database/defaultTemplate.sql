-- EXEC dbo.GenerateDefaultTemplate;

CREATE OR ALTER PROCEDURE dbo.GenerateDefaultTemplate
AS
BEGIN
    BEGIN TRANSACTION;

    BEGIN TRY

        DECLARE @ClinicId INT = 0;
        DECLARE @TemplateId INT;
        DECLARE @SectionId INT;
        DECLARE @FieldId INT;

        /* ============================================================
        1. CREATE DEFAULT CONSULTATION TEMPLATE
        ============================================================ */

        INSERT INTO PrescriptionTemplates
        (
            ClinicId, DepartmentId, DoctorId, TemplateCode, TemplateName, IsActive
        )
        VALUES
        (
            NULL, NULL, NULL, 'DEFAULT', 'Default Consultation Template', 1
        );

        SET @TemplateId = SCOPE_IDENTITY();


        /* ============================================================
        2. CHIEF COMPLAINTS
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
            @SectionId, @TemplateId, NULL, 'Chief Complaints', 'Textarea', 'CHIEF_COMPLAINTS', NULL, 0, 1,
            1, 0, NULL
        );


        /* ============================================================
        3. PAST ILLNESSES
        ============================================================ */

        INSERT INTO PrescriptionTemplateSections
        (
            TemplateId, ClinicId, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice
        )
        VALUES
        (
            @TemplateId, NULL, 'Past Illnesses', 2, 1, 0, 0
        );

        SET @SectionId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFields
        (
            SectionId, TemplateId, ClinicId, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder,
            IsActive, IsArchived, MergeWith
        )
        VALUES
        (
            @SectionId, @TemplateId, NULL, 'Past Illnesses', 'Textarea', 'PAST_ILLNESSES', NULL, 0, 1,
            1, 0, NULL
        );


        /* ============================================================
        4. HISTORY OF PRESENT ILLNESS
        ============================================================ */

        INSERT INTO PrescriptionTemplateSections
        (
            TemplateId, ClinicId, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice
        )
        VALUES
        (
            @TemplateId, NULL, 'History of Present Illness', 3, 1, 0, 0
        );

        SET @SectionId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFields
        (
            SectionId, TemplateId, ClinicId, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder,
            IsActive, IsArchived, MergeWith
        )
        VALUES
        (
            @SectionId, @TemplateId, NULL, 'History of Present Illness', 'Textarea', 'HOPI', NULL, 0, 1,
            1, 0, NULL
        );


        /* ============================================================
        5. DIAGNOSIS
        ============================================================ */

        INSERT INTO PrescriptionTemplateSections
        (
            TemplateId, ClinicId, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice
        )
        VALUES
        (
            @TemplateId, NULL, 'Diagnosis', 4, 1, 0, 0
        );

        SET @SectionId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFields
        (
            SectionId, TemplateId, ClinicId, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder,
            IsActive, IsArchived, MergeWith
        )
        VALUES
        (
            @SectionId, @TemplateId, NULL, 'Diagnosis', 'Textarea', 'DIAGNOSIS', NULL, 0, 1,
            1, 0, NULL
        );


        /* ============================================================
        6. MEDICAL ADVICE
        ============================================================ */

        INSERT INTO PrescriptionTemplateSections
        (
            TemplateId, ClinicId, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice
        )
        VALUES
        (
            @TemplateId, NULL, 'Medical Advice', 5, 1, 0, 0
        );

        SET @SectionId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFields
        (
            SectionId, TemplateId, ClinicId, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder,
            IsActive, IsArchived, MergeWith
        )
        VALUES
        (
            @SectionId, @TemplateId, NULL, 'Medical Advice', 'Textarea', 'MEDICAL_ADVICE', NULL, 0, 1,
            1, 0, NULL
        );


        /* ============================================================
        7. PHARMACOLOGICAL ADVICE
        ============================================================ */

        INSERT INTO PrescriptionTemplateSections
        (
            TemplateId, ClinicId, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice
        )
        VALUES
        (
            @TemplateId, NULL, 'Pharmacological Advice', 6, 1, 1, 1
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
        8. REVIEW AND FOLLOWUP
        ============================================================ */

        INSERT INTO PrescriptionTemplateSections
        (
            TemplateId, ClinicId, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice
        )
        VALUES
        (
            @TemplateId, NULL, 'Review and Followup', 7, 1, 0, 0
        );

        SET @SectionId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFields
        (
            SectionId, TemplateId, ClinicId, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder,
            IsActive, IsArchived, MergeWith
        )
        VALUES
        (
            @SectionId, @TemplateId, NULL, 'Review and Followup', 'Text', 'REVIEW_AND_FOLLOWUP', NULL, 0, 1,
            1, 0, NULL
        );


        /* ============================================================
        COMMIT
        ============================================================ */

        COMMIT TRANSACTION;

        SELECT @TemplateId AS CreatedTemplateId;

    END TRY
    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;

    END CATCH;

END
GO

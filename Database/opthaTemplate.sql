-- EXEC dbo.GenerateDefaultOphthalmologyTemplate;

CREATE OR ALTER PROCEDURE dbo.GenerateDefaultOphthalmologyTemplate
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @TemplateId INT;
        DECLARE @SectionId INT;
        DECLARE @FieldId INT;

        ----------------------------------------------------
        -- TEMPLATE
        ----------------------------------------------------
        INSERT INTO PrescriptionTemplates
        (
            ClinicId, DepartmentId, DoctorId, TemplateCode, TemplateName
        )
        VALUES
        (
            NULL, NULL, NULL, 'OPTHALMOLOGY', 'Ophthalmology Template'
        );

        SET @TemplateId = SCOPE_IDENTITY();

        ----------------------------------------------------
        -- SECTION : CHIEF COMPLAINTS
        ----------------------------------------------------
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
        (@FieldId, @SectionId, @TemplateId, NULL, 'Diminished Vision', 0, 1, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Redness', 0, 2, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Watering', 0, 3, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Eye Pain', 0, 4, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Itching', 0, 5, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Photophobia', 0, 6, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Discharge', 0, 7, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Foreign Body Sensation', 0, 8, 1, 0);

        ----------------------------------------------------
        -- SECTION : VISUAL ACUITY
        ----------------------------------------------------
        INSERT INTO PrescriptionTemplateSections
        (
            TemplateId, ClinicId, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice
        )
        VALUES
        (
            @TemplateId, NULL, 'Visual Acuity', 2, 1, 0, 0
        );

        SET @SectionId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFields
        (
            SectionId, TemplateId, ClinicId, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder,
            IsActive, IsArchived, MergeWith
        )
        VALUES
        (@SectionId, @TemplateId, NULL, 'Distance Vision OD', 'Text', 'DISTANCE_VISION_OD', NULL, 0, 1, 1, 0, NULL),
        (@SectionId, @TemplateId, NULL, 'Distance Vision OS', 'Text', 'DISTANCE_VISION_OS', NULL, 0, 2, 1, 0, NULL),
        (@SectionId, @TemplateId, NULL, 'Near Vision OD', 'Text', 'NEAR_VISION_OD', NULL, 0, 3, 1, 0, NULL),
        (@SectionId, @TemplateId, NULL, 'Near Vision OS', 'Text', 'NEAR_VISION_OS', NULL, 0, 4, 1, 0, NULL);

        ----------------------------------------------------
        -- SECTION : REFRACTION
        ----------------------------------------------------
        INSERT INTO PrescriptionTemplateSections
        (
            TemplateId, ClinicId, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice
        )
        VALUES
        (
            @TemplateId, NULL, 'Refraction', 3, 1, 0, 0
        );

        SET @SectionId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFields
        (
            SectionId, TemplateId, ClinicId, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder,
            IsActive, IsArchived, MergeWith
        )
        VALUES
        (@SectionId, @TemplateId, NULL, 'OD Sphere', 'Text', 'OD_SPHERE', NULL, 0, 1, 1, 0, NULL),
        (@SectionId, @TemplateId, NULL, 'OD Cylinder', 'Text', 'OD_CYLINDER', NULL, 0, 2, 1, 0, NULL),
        (@SectionId, @TemplateId, NULL, 'OD Axis', 'NUMBER', 'OD_AXIS', NULL, 0, 3, 1, 0, NULL),
        (@SectionId, @TemplateId, NULL, 'OS Sphere', 'Text', 'OS_SPHERE', NULL, 0, 4, 1, 0, NULL),
        (@SectionId, @TemplateId, NULL, 'OS Cylinder', 'Text', 'OS_CYLINDER', NULL, 0, 5, 1, 0, NULL),
        (@SectionId, @TemplateId, NULL, 'OS Axis', 'Text', 'OS_AXIS', NULL, 0, 6, 1, 0, NULL);

        ----------------------------------------------------
        -- SECTION : INTRA OCULAR PRESSURE
        ----------------------------------------------------
        INSERT INTO PrescriptionTemplateSections
        (
            TemplateId, ClinicId, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice
        )
        VALUES
        (
            @TemplateId, NULL, 'Intra Ocular Pressure', 4, 1, 0, 0
        );

        SET @SectionId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFields
        (
            SectionId, TemplateId, ClinicId, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder,
            IsActive, IsArchived, MergeWith
        )
        VALUES
        (@SectionId, @TemplateId, NULL, 'IOP OD (mmHg)', 'Number', 'IOP_OD', NULL, 0, 1, 1, 0, NULL),
        (@SectionId, @TemplateId, NULL, 'IOP OS (mmHg)', 'Number', 'IOP_OS', NULL, 0, 2, 1, 0, NULL);

        ----------------------------------------------------
        -- SECTION : ANTERIOR SEGMENT EXAMINATION
        ----------------------------------------------------
        INSERT INTO PrescriptionTemplateSections
        (
            TemplateId, ClinicId, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice
        )
        VALUES
        (
            @TemplateId, NULL, 'Anterior Segment Examination', 5, 1, 0, 0
        );

        SET @SectionId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFields
        (
            SectionId, TemplateId, ClinicId, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder,
            IsActive, IsArchived, MergeWith
        )
        VALUES
        (@SectionId, @TemplateId, NULL, 'Lids & Adnexa', 'TextArea', 'LIDS_ADNEXA', NULL, 0, 1, 1, 0, NULL),
        (@SectionId, @TemplateId, NULL, 'Conjunctiva', 'TextArea', 'CONJUNCTIVA', NULL, 0, 2, 1, 0, NULL),
        (@SectionId, @TemplateId, NULL, 'Cornea', 'TextArea', 'CORNEA', NULL, 0, 3, 1, 0, NULL),
        (@SectionId, @TemplateId, NULL, 'Anterior Chamber', 'TextArea', 'ANTERIOR_CHAMBER', NULL, 0, 4, 1, 0, NULL),
        (@SectionId, @TemplateId, NULL, 'Iris', 'TextArea', 'IRIS', NULL, 0, 5, 1, 0, NULL),
        (@SectionId, @TemplateId, NULL, 'Lens', 'TextArea', 'LENS', NULL, 0, 6, 1, 0, NULL);

        ----------------------------------------------------
        -- SECTION : POSTERIOR SEGMENT EXAMINATION
        ----------------------------------------------------
        INSERT INTO PrescriptionTemplateSections
        (
            TemplateId, ClinicId, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice
        )
        VALUES
        (
            @TemplateId, NULL, 'Posterior Segment Examination', 6, 1, 0, 0
        );

        SET @SectionId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFields
        (
            SectionId, TemplateId, ClinicId, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder,
            IsActive, IsArchived, MergeWith
        )
        VALUES
        (@SectionId, @TemplateId, NULL, 'Optic Disc', 'TextArea', 'OPTIC_DISC', NULL, 0, 1, 1, 0, NULL),
        (@SectionId, @TemplateId, NULL, 'Macula', 'TextArea', 'MACULA', NULL, 0, 2, 1, 0, NULL),
        (@SectionId, @TemplateId, NULL, 'Retinal Vessels', 'TextArea', 'RETINA_VESSELS', NULL, 0, 3, 1, 0, NULL),
        (@SectionId, @TemplateId, NULL, 'Peripheral Retina', 'TextArea', 'PERIPHERAL_RETINA', NULL, 0, 4, 1, 0, NULL);

        ----------------------------------------------------
        -- SECTION : DIAGNOSIS
        ----------------------------------------------------
        INSERT INTO PrescriptionTemplateSections
        (
            TemplateId, ClinicId, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice
        )
        VALUES
        (
            @TemplateId, NULL, 'Diagnosis', 7, 1, 0, 0
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

        ----------------------------------------------------
        -- SECTION : ADVICE
        ----------------------------------------------------
        INSERT INTO PrescriptionTemplateSections
        (
            TemplateId, ClinicId, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice
        )
        VALUES
        (
            @TemplateId, NULL, 'Advice', 8, 1, 0, 0
        );

        SET @SectionId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFields
        (
            SectionId, TemplateId, ClinicId, FieldLabel, FieldTypeCode, FieldCode, FieldSource, IsRequired, DisplayOrder,
            IsActive, IsArchived, MergeWith
        )
        VALUES
        (
            @SectionId, @TemplateId, NULL, 'Advice', 'CheckboxGroup', 'ADVICE', NULL, 0, 1, 1, 0, NULL
        );

        SET @FieldId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFieldOptions
        (
            FieldId, SectionId, TemplateId, ClinicId, OptionText, IsRequired, DisplayOrder, IsActive, IsArchived
        )
        VALUES
        (@FieldId, @SectionId, @TemplateId, NULL, 'Spectacles Advised', 0, 1, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Continue Existing Glasses', 0, 2, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Artificial Tears', 0, 3, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Warm Compress', 0, 4, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Diabetic Eye Evaluation', 0, 5, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Glaucoma Workup', 0, 6, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Cataract Surgery Evaluation', 0, 7, 1, 0),
        (@FieldId, @SectionId, @TemplateId, NULL, 'Retina Evaluation', 0, 8, 1, 0);

        /* ============================================================
        7. PHARMACOLOGICAL ADVICE
        ============================================================ */

        INSERT INTO PrescriptionTemplateSections
        (
            TemplateId, ClinicId, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice
        )
        VALUES
        (
            @TemplateId, NULL, 'Pharmacological Advice', 9, 1, 1, 1
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
            @SectionId, @TemplateId, NULL, 'Medicine', 'Lookup', 'Medicine', 'Medicines', 1, 1, 1, 0, NULL
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
            @SectionId, @TemplateId, NULL, 'Dose', 'Dropdown', 'Dose', NULL, 1, 2, 1, 0, NULL
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
            @SectionId, @TemplateId, NULL, 'Timing', 'Dropdown', 'Timing', NULL, 1, 3, 1, 0, NULL
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
            @SectionId, @TemplateId, NULL, 'Duration', 'Number', 'Duration', NULL, 1, 4, 1, 0, NULL
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
            @SectionId, @TemplateId, NULL, 'Unit', 'Dropdown', 'Unit', NULL, 1, 5, 1, 0, NULL
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
            @SectionId, @TemplateId, NULL, 'Instructions', 'Text', 'Instruction', NULL, 0, 6, 1, 0, NULL
        );

        ----------------------------------------------------
        -- SECTION : FOLLOW UP
        ----------------------------------------------------
        INSERT INTO PrescriptionTemplateSections
        (
            TemplateId, ClinicId, SectionName, DisplayOrder, IsActive, Multiline, IsPharmacologyAdvice
        )
        VALUES
        (
            @TemplateId, NULL, 'Follow Up', 10, 1, 0, 0
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

        ----------------------------------------------------
        -- COMMIT TRANSACTION
        ----------------------------------------------------
        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();

        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);

    END CATCH
END
GO        
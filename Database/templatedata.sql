-- EXEC dbo.GenerateDefaultOphthalmologyTemplate @ClinicId = 7;

CREATE OR ALTER PROCEDURE dbo.GenerateDefaultOphthalmologyTemplate
    @ClinicId INT
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
            ClinicId,
            DepartmentId,
            DoctorId,
            TemplateName
        )
        VALUES
        (
            @ClinicId,
            5,
            NULL,
            'Default Ophthalmology Template'
        );

        SET @TemplateId = SCOPE_IDENTITY();

        ----------------------------------------------------
        -- SECTION : CHIEF COMPLAINTS
        ----------------------------------------------------
        INSERT INTO PrescriptionTemplateSections
        (
            TemplateId,
            ClinicId,
            SectionName,
            DisplayOrder
        )
        VALUES
        (
            @TemplateId,
            @ClinicId,
            'Chief Complaints',
            1
        );

        SET @SectionId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFields
        (
            SectionId,
            TemplateId,
            ClinicId,
            FieldLabel,
            FieldTypeCode,
            IsRequired,
            DisplayOrder
        )
        VALUES
        (
            @SectionId,
            @TemplateId,
            @ClinicId,
            'Symptoms',
            'CheckboxGroup',
            0,
            1
        );

        SET @FieldId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFieldOptions
        (
            FieldId,
            SectionId,
            TemplateId,
            ClinicId,
            OptionText,
            IsRequired,
            DisplayOrder
        )
        VALUES
        (@FieldId, @SectionId, @TemplateId, @ClinicId, 'Diminished Vision', 0, 1),
        (@FieldId, @SectionId, @TemplateId, @ClinicId, 'Redness', 0, 2),
        (@FieldId, @SectionId, @TemplateId, @ClinicId, 'Watering', 0, 3),
        (@FieldId, @SectionId, @TemplateId, @ClinicId, 'Eye Pain', 0, 4),
        (@FieldId, @SectionId, @TemplateId, @ClinicId, 'Itching', 0, 5),
        (@FieldId, @SectionId, @TemplateId, @ClinicId, 'Photophobia', 0, 6),
        (@FieldId, @SectionId, @TemplateId, @ClinicId, 'Discharge', 0, 7),
        (@FieldId, @SectionId, @TemplateId, @ClinicId, 'Foreign Body Sensation', 0, 8);

        ----------------------------------------------------
        -- SECTION : VISUAL ACUITY
        ----------------------------------------------------
        INSERT INTO PrescriptionTemplateSections
        (
            TemplateId,
            ClinicId,
            SectionName,
            DisplayOrder
        )
        VALUES
        (
            @TemplateId,
            @ClinicId,
            'Visual Acuity',
            2
        );

        SET @SectionId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFields
        (
            SectionId,
            TemplateId,
            ClinicId,
            FieldLabel,
            FieldTypeCode,
            IsRequired,
            DisplayOrder
        )
        VALUES
        (@SectionId, @TemplateId, @ClinicId, 'Distance Vision OD', 'Text', 0, 1),
        (@SectionId, @TemplateId, @ClinicId, 'Distance Vision OS', 'Text', 0, 2),
        (@SectionId, @TemplateId, @ClinicId, 'Near Vision OD', 'Text', 0, 3),
        (@SectionId, @TemplateId, @ClinicId, 'Near Vision OS', 'Text', 0, 4);

        ----------------------------------------------------
        -- SECTION : REFRACTION
        ----------------------------------------------------
        INSERT INTO PrescriptionTemplateSections
        (
            TemplateId,
            ClinicId,
            SectionName,
            DisplayOrder
        )
        VALUES
        (
            @TemplateId,
            @ClinicId,
            'Refraction',
            3
        );

        SET @SectionId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFields
        (
            SectionId,
            TemplateId,
            ClinicId,
            FieldLabel,
            FieldTypeCode,
            IsRequired,
            DisplayOrder
        )
        VALUES
        (@SectionId, @TemplateId, @ClinicId, 'OD Sphere', 'Text', 0, 1),
        (@SectionId, @TemplateId, @ClinicId, 'OD Cylinder', 'Text', 0, 2),
        (@SectionId, @TemplateId, @ClinicId, 'OD Axis', 'Text', 0, 3),
        (@SectionId, @TemplateId, @ClinicId, 'OS Sphere', 'Text', 0, 4),
        (@SectionId, @TemplateId, @ClinicId, 'OS Cylinder', 'Text', 0, 5),
        (@SectionId, @TemplateId, @ClinicId, 'OS Axis', 'Text', 0, 6);

        ----------------------------------------------------
        -- SECTION : INTRA OCULAR PRESSURE
        ----------------------------------------------------
        INSERT INTO PrescriptionTemplateSections
        (
            TemplateId,
            ClinicId,
            SectionName,
            DisplayOrder
        )
        VALUES
        (
            @TemplateId,
            @ClinicId,
            'Intra Ocular Pressure',
            4
        );

        SET @SectionId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFields
        (
            SectionId,
            TemplateId,
            ClinicId,
            FieldLabel,
            FieldTypeCode,
            IsRequired,
            DisplayOrder
        )
        VALUES
        (@SectionId, @TemplateId, @ClinicId, 'IOP OD (mmHg)', 'Number', 0, 1),
        (@SectionId, @TemplateId, @ClinicId, 'IOP OS (mmHg)', 'Number', 0, 2);

        ----------------------------------------------------
        -- SECTION : ANTERIOR SEGMENT EXAMINATION
        ----------------------------------------------------
        INSERT INTO PrescriptionTemplateSections
        (
            TemplateId,
            ClinicId,
            SectionName,
            DisplayOrder
        )
        VALUES
        (
            @TemplateId,
            @ClinicId,
            'Anterior Segment Examination',
            5
        );

        SET @SectionId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFields
        (
            SectionId,
            TemplateId,
            ClinicId,
            FieldLabel,
            FieldTypeCode,
            IsRequired,
            DisplayOrder
        )
        VALUES
        (@SectionId, @TemplateId, @ClinicId, 'Lids & Adnexa', 'TextArea', 0, 1),
        (@SectionId, @TemplateId, @ClinicId, 'Conjunctiva', 'TextArea', 0, 2),
        (@SectionId, @TemplateId, @ClinicId, 'Cornea', 'TextArea', 0, 3),
        (@SectionId, @TemplateId, @ClinicId, 'Anterior Chamber', 'TextArea', 0, 4),
        (@SectionId, @TemplateId, @ClinicId, 'Iris', 'TextArea', 0, 5),
        (@SectionId, @TemplateId, @ClinicId, 'Lens', 'TextArea', 0, 6);

        ----------------------------------------------------
        -- SECTION : POSTERIOR SEGMENT EXAMINATION
        ----------------------------------------------------
        INSERT INTO PrescriptionTemplateSections
        (
            TemplateId,
            ClinicId,
            SectionName,
            DisplayOrder
        )
        VALUES
        (
            @TemplateId,
            @ClinicId,
            'Posterior Segment Examination',
            6
        );

        SET @SectionId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFields
        (
            SectionId,
            TemplateId,
            ClinicId,
            FieldLabel,
            FieldTypeCode,
            IsRequired,
            DisplayOrder
        )
        VALUES
        (@SectionId, @TemplateId, @ClinicId, 'Optic Disc', 'TextArea', 0, 1),
        (@SectionId, @TemplateId, @ClinicId, 'Macula', 'TextArea', 0, 2),
        (@SectionId, @TemplateId, @ClinicId, 'Retinal Vessels', 'TextArea', 0, 3),
        (@SectionId, @TemplateId, @ClinicId, 'Peripheral Retina', 'TextArea', 0, 4);

        ----------------------------------------------------
        -- SECTION : DIAGNOSIS
        ----------------------------------------------------
        INSERT INTO PrescriptionTemplateSections
        (
            TemplateId,
            ClinicId,
            SectionName,
            DisplayOrder
        )
        VALUES
        (
            @TemplateId,
            @ClinicId,
            'Diagnosis',
            7
        );

        SET @SectionId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFields
        (
            SectionId,
            TemplateId,
            ClinicId,
            FieldLabel,
            FieldTypeCode,
            IsRequired,
            DisplayOrder
        )
        VALUES
        (
            @SectionId,
            @TemplateId,
            @ClinicId,
            'Diagnosis',
            'TextArea',
            1,
            1
        );

        ----------------------------------------------------
        -- SECTION : ADVICE
        ----------------------------------------------------
        INSERT INTO PrescriptionTemplateSections
        (
            TemplateId,
            ClinicId,
            SectionName,
            DisplayOrder
        )
        VALUES
        (
            @TemplateId,
            @ClinicId,
            'Advice',
            8
        );

        SET @SectionId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFields
        (
            SectionId,
            TemplateId,
            ClinicId,
            FieldLabel,
            FieldTypeCode,
            IsRequired,
            DisplayOrder
        )
        VALUES
        (
            @SectionId,
            @TemplateId,
            @ClinicId,
            'Advice',
            'CheckboxGroup',
            0,
            1
        );

        SET @FieldId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFieldOptions
        (
            FieldId,
            SectionId,
            TemplateId,
            ClinicId,
            OptionText,
            IsRequired,
            DisplayOrder
        )
        VALUES
        (@FieldId, @SectionId, @TemplateId, @ClinicId, 'Spectacles Advised', 0, 1),
        (@FieldId, @SectionId, @TemplateId, @ClinicId, 'Continue Existing Glasses', 0, 2),
        (@FieldId, @SectionId, @TemplateId, @ClinicId, 'Artificial Tears', 0, 3),
        (@FieldId, @SectionId, @TemplateId, @ClinicId, 'Warm Compress', 0, 4),
        (@FieldId, @SectionId, @TemplateId, @ClinicId, 'Diabetic Eye Evaluation', 0, 5),
        (@FieldId, @SectionId, @TemplateId, @ClinicId, 'Glaucoma Workup', 0, 6),
        (@FieldId, @SectionId, @TemplateId, @ClinicId, 'Cataract Surgery Evaluation', 0, 7),
        (@FieldId, @SectionId, @TemplateId, @ClinicId, 'Retina Evaluation', 0, 8);

        ----------------------------------------------------
        -- SECTION : FOLLOW UP
        ----------------------------------------------------
        INSERT INTO PrescriptionTemplateSections
        (
            TemplateId,
            ClinicId,
            SectionName,
            DisplayOrder
        )
        VALUES
        (
            @TemplateId,
            @ClinicId,
            'Follow Up',
            9
        );

        SET @SectionId = SCOPE_IDENTITY();

        INSERT INTO PrescriptionTemplateFields
        (
            SectionId,
            TemplateId,
            ClinicId,
            FieldLabel,
            FieldTypeCode,
            IsRequired,
            DisplayOrder
        )
        VALUES
        (
            @SectionId,
            @TemplateId,
            @ClinicId,
            'Review After',
            'Text',
            0,
            1
        ),
        (
            @SectionId,
            @TemplateId,
            @ClinicId,
            'Next Visit Date',
            'Date',
            0,
            2
        );

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
INSERT INTO PrescriptionTemplates
(
ClinicId,
DepartmentId,
TemplateName,
IsActive
)
VALUES
(
1,
5,
'Default Ophthalmology Template',
1
);

DECLARE @TemplateId INT = SCOPE_IDENTITY();

-- SECTION : CHIEF COMPLAINTS

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
1,
'Chief Complaints',
1
);

DECLARE @ChiefComplaintSectionId INT = SCOPE_IDENTITY();

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
@ChiefComplaintSectionId,
@TemplateId,
1,
'Symptoms',
'CheckboxGroup',
0,
1
);

DECLARE @SymptomsFieldId INT = SCOPE_IDENTITY();

INSERT INTO PrescriptionTemplateFieldOptions
(
FieldId,
SectionId,
TemplateId,
ClinicId,
OptionText,
OptionValue,
IsRequired,
DisplayOrder
)
VALUES
(@SymptomsFieldId,@ChiefComplaintSectionId,@TemplateId,1,'Diminished Vision','Diminished Vision',0,1),
(@SymptomsFieldId,@ChiefComplaintSectionId,@TemplateId,1,'Redness','Redness',0,2),
(@SymptomsFieldId,@ChiefComplaintSectionId,@TemplateId,1,'Watering','Watering',0,3),
(@SymptomsFieldId,@ChiefComplaintSectionId,@TemplateId,1,'Eye Pain','Eye Pain',0,4),
(@SymptomsFieldId,@ChiefComplaintSectionId,@TemplateId,1,'Itching','Itching',0,5),
(@SymptomsFieldId,@ChiefComplaintSectionId,@TemplateId,1,'Photophobia','Photophobia',0,6),
(@SymptomsFieldId,@ChiefComplaintSectionId,@TemplateId,1,'Discharge','Discharge',0,7),
(@SymptomsFieldId,@ChiefComplaintSectionId,@TemplateId,1,'Foreign Body Sensation','Foreign Body Sensation',0,8);

-- SECTION : VISUAL ACUITY

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
1,
'Visual Acuity',
2
);

DECLARE @VisualAcuitySectionId INT = SCOPE_IDENTITY();

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
(@VisualAcuitySectionId,@TemplateId,1,'Distance Vision OD','Text',0,1),
(@VisualAcuitySectionId,@TemplateId,1,'Distance Vision OS','Text',0,2),
(@VisualAcuitySectionId,@TemplateId,1,'Near Vision OD','Text',0,3),
(@VisualAcuitySectionId,@TemplateId,1,'Near Vision OS','Text',0,4);

-- SECTION : REFRACTION

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
1,
'Refraction',
3
);

DECLARE @RefractionSectionId INT = SCOPE_IDENTITY();

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
(@RefractionSectionId,@TemplateId,1,'OD Sphere','Text',0,1),
(@RefractionSectionId,@TemplateId,1,'OD Cylinder','Text',0,2),
(@RefractionSectionId,@TemplateId,1,'OD Axis','Text',0,3),
(@RefractionSectionId,@TemplateId,1,'OS Sphere','Text',0,4),
(@RefractionSectionId,@TemplateId,1,'OS Cylinder','Text',0,5),
(@RefractionSectionId,@TemplateId,1,'OS Axis','Text',0,6);

-- SECTION : INTRA OCULAR PRESSURE

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
1,
'Intra Ocular Pressure',
4
);

DECLARE @IOPSectionId INT = SCOPE_IDENTITY();

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
(@IOPSectionId,@TemplateId,1,'IOP OD (mmHg)','Number',0,1),
(@IOPSectionId,@TemplateId,1,'IOP OS (mmHg)','Number',0,2);

-- SECTION : ANTERIOR SEGMENT

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
1,
'Anterior Segment Examination',
5
);

DECLARE @AnteriorSectionId INT = SCOPE_IDENTITY();

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
(@AnteriorSectionId,@TemplateId,1,'Lids & Adnexa','TextArea',0,1),
(@AnteriorSectionId,@TemplateId,1,'Conjunctiva','TextArea',0,2),
(@AnteriorSectionId,@TemplateId,1,'Cornea','TextArea',0,3),
(@AnteriorSectionId,@TemplateId,1,'Anterior Chamber','TextArea',0,4),
(@AnteriorSectionId,@TemplateId,1,'Iris','TextArea',0,5),
(@AnteriorSectionId,@TemplateId,1,'Lens','TextArea',0,6);

-- SECTION : POSTERIOR SEGMENT

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
1,
'Posterior Segment Examination',
6
);

DECLARE @PosteriorSectionId INT = SCOPE_IDENTITY();

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
(@PosteriorSectionId,@TemplateId,1,'Optic Disc','TextArea',0,1),
(@PosteriorSectionId,@TemplateId,1,'Macula','TextArea',0,2),
(@PosteriorSectionId,@TemplateId,1,'Retinal Vessels','TextArea',0,3),
(@PosteriorSectionId,@TemplateId,1,'Peripheral Retina','TextArea',0,4);

-- SECTION : DIAGNOSIS

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
1,
'Diagnosis',
7
);

DECLARE @DiagnosisSectionId INT = SCOPE_IDENTITY();

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
@DiagnosisSectionId,
@TemplateId,
1,
'Diagnosis',
'TextArea',
1,
1
);

-- SECTION : ADVICE

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
1,
'Advice',
8
);

DECLARE @AdviceSectionId INT = SCOPE_IDENTITY();

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
@AdviceSectionId,
@TemplateId,
1,
'Advice',
'CheckboxGroup',
0,
1
);

DECLARE @AdviceFieldId INT = SCOPE_IDENTITY();

INSERT INTO PrescriptionTemplateFieldOptions
(
FieldId,
SectionId,
TemplateId,
ClinicId,
OptionText,
OptionValue,
IsRequired,
DisplayOrder
)
VALUES
(@AdviceFieldId,@AdviceSectionId,@TemplateId,1,'Spectacles Advised','Spectacles Advised',0,1),
(@AdviceFieldId,@AdviceSectionId,@TemplateId,1,'Continue Existing Glasses','Continue Existing Glasses',0,2),
(@AdviceFieldId,@AdviceSectionId,@TemplateId,1,'Artificial Tears','Artificial Tears',0,3),
(@AdviceFieldId,@AdviceSectionId,@TemplateId,1,'Warm Compress','Warm Compress',0,4),
(@AdviceFieldId,@AdviceSectionId,@TemplateId,1,'Diabetic Eye Evaluation','Diabetic Eye Evaluation',0,5),
(@AdviceFieldId,@AdviceSectionId,@TemplateId,1,'Glaucoma Workup','Glaucoma Workup',0,6),
(@AdviceFieldId,@AdviceSectionId,@TemplateId,1,'Cataract Surgery Evaluation','Cataract Surgery Evaluation',0,7),
(@AdviceFieldId,@AdviceSectionId,@TemplateId,1,'Retina Evaluation','Retina Evaluation',0,8);

-- SECTION : FOLLOW UP

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
1,
'Follow Up',
9
);

DECLARE @FollowUpSectionId INT = SCOPE_IDENTITY();

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
(@FollowUpSectionId,@TemplateId,1,'Review After','Text',0,1),
(@FollowUpSectionId,@TemplateId,1,'Next Visit Date','Date',0,2);
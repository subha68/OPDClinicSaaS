INSERT INTO FieldTypes VALUES
('Text'),
('TextArea'),
('Number'),
('Date'),
('CheckboxGroup'),
('Dropdown'),
('RadioGroup'),
('MultiSelect'),
('Lookup'),
('Hidden');

INSERT INTO Roles VALUES
(1,'SuperAdmin'),
(2,'ClinicAdmin'),
(3,'ClinicUser'),
(4,'Doctor'),
(5,'BillingUser'),
(6, 'PharmacyUser'),
(7, 'Nurse');

-- Clinics
INSERT INTO Clinics (ClinicCode, Name) VALUES
('CLN001','City Care'),
('CLN002','Health Plus'),
('CLN000','Library');

-- Users
INSERT INTO Users (RoleId, Username, PasswordHash)
VALUES (1,'superadmin','admin123');

INSERT INTO Users (ClinicId, RoleId, Username, PasswordHash)
VALUES
(1,2,'clinicadmin','admin123'),
(1,3,'user1','user123'),
(1,4,'doctor1','doc123'),
(1,5,'billing1','bill123'),
(1,6,'doctor2','doc123'),
(1,7,'doctor3','doc123');

-- Departments
INSERT INTO Departments (ClinicId, Name)
VALUES (1,'General'), (1,'Cardiology'), (1,'ENT'), (1, 'Orthopaedics'), (1, 'Ophthalmology');

INSERT INTO Departments (ClinicId, Name)
SELECT ClinicId, 'General' FROM Clinics WHERE Name = 'Library' UNION ALL
SELECT ClinicId, 'Cardiology' FROM Clinics WHERE Name = 'Library' UNION ALL
SELECT ClinicId, 'ENT' FROM Clinics WHERE Name = 'Library' UNION ALL
SELECT ClinicId, 'Orthopaedics' FROM Clinics WHERE Name = 'Library' UNION ALL
SELECT ClinicId, 'Ophthalmology' FROM Clinics WHERE Name = 'Library';

-- Doctors
INSERT INTO Doctors (ClinicId, DepartmentId, FirstName, LastName, UserId, RegistrationNo, Qualification)
VALUES
(1,1,'Dr. Amit','Sharma',4,'REG001','MD'),
(1,2,'Dr. Roy','Kumar',6,'REG002','MBBS');

-- Patients
INSERT INTO Patients (ClinicId, FirstName, LastName, Phone, DOB)
VALUES
(1,'Rahul','Das','9999999991','1990-01-01'),
(1,'Sita','Roy','9999999992','1985-05-05');

-- Features
INSERT INTO Features (Name, Code) VALUES
('Manage Departments', 'DEPT_VIEW'),
('Manage Doctors', 'DOC_VIEW'),
('Manage Users', 'USER_VIEW'),
('Manage Roles', 'ROLE_VIEW'),
('Manage Clinics', 'CLINIC_VIEW'),
('Doctor Schedule', 'DOCTOR_SCHEDULE'),
('Appointment Booking', 'APPOINTMENT_BOOKING'),
('Consultation Management', 'CONSULTATION_MANAGEMENT'),
('Billing Management', 'BILLING_MANAGEMENT'),
('Patient History', 'PATIENT_HISTORY'),
('Billing Charges', 'BILLING_CHARGES'),
('Medicine Management', 'MEDICINE_MANAGEMENT'),
('Prescription Templates', 'PRESCRIPTION_TEMPLATE'),
('Vital Definition', 'VITAL_DEFINITION'),
('Vital', 'VITAL'),
('Test Definition', 'TEST_DEFINITION'),
('Test Result', 'TEST_RESULT');

-- Role Mapping

-- SuperAdmin
INSERT INTO RoleFeatures (RoleId, FeatureId)
SELECT 1, FeatureId FROM Features WHERE Code IN ('CLINIC_VIEW', 'USER_VIEW', 'ROLE_VIEW', 'VITAL_DEFINITION', 'TEST_DEFINITION');

-- ClinicAdmin
INSERT INTO RoleFeatures (RoleId, FeatureId)
SELECT 2, FeatureId FROM Features WHERE Code IN ('DEPT_VIEW','DOC_VIEW','USER_VIEW','DOCTOR_SCHEDULE', 'PRESCRIPTION_TEMPLATE', 
                                                    'VITAL_DEFINITION', 'TEST_DEFINITION');
-- ClinicUser
INSERT INTO RoleFeatures (RoleId, FeatureId)
SELECT 3, FeatureId FROM Features WHERE Code IN ('APPOINTMENT_BOOKING', 'PATIENT_HISTORY', 'VITAL', 'TEST_DEFINITION',
                                                'TEST_RESULT');

-- Doctor
INSERT INTO RoleFeatures (RoleId, FeatureId)
SELECT 4, FeatureId FROM Features WHERE Code IN ('CONSULTATION_MANAGEMENT', 'PATIENT_HISTORY', 'VITAL', 'TEST_RESULT');

-- BillingUser
INSERT INTO RoleFeatures (RoleId, FeatureId)
SELECT 5, FeatureId FROM Features WHERE Code IN ('BILLING_CHARGES', 'PATIENT_HISTORY', 'BILLING_MANAGEMENT'),

-- PharmacyUser
INSERT INTO RoleFeatures (RoleId, FeatureId)
SELECT 6, FeatureId FROM Features WHERE Code IN ('MEDICINE_MANAGEMENT');

-- Nurse
INSERT INTO RoleFeatures (RoleId, FeatureId)
SELECT 7, FeatureId FROM Features WHERE Code IN ('APPOINTMENT_BOOKING', 'PATIENT_HISTORY', 'BILLING_CHARGES', 'BILLING_MANAGEMENT');


-- Menus
INSERT INTO Menus (Title, Controller, Action, Icon, FeatureCode, DisplayOrder)
VALUES
('Clinics','Clinic','Index','fa-hospital','CLINIC_VIEW',1),
('Roles','Role','Index','fa-user-shield','ROLE_VIEW',2),
('Users','User','Index','fa-users','USER_VIEW',3),
('Departments','Department','Index','fa-building','DEPT_VIEW',4),
('Doctors','Doctor','Index','fa-user-md','DOC_VIEW',5),
('Doctor Schedule','DoctorSchedule','Index','fa-calendar','DOCTOR_SCHEDULE',6),
('Patients', 'Patient', 'Index', 'fa fa-users', 'PATIENT_HISTORY', 7),
('Appointment','AppointmentBooking','Index','fa-calendar-check','APPOINTMENT_BOOKING',8),
('Consultation','Consultation','Index','fa-stethoscope','CONSULTATION_MANAGEMENT',9),
('Billing','Billing','Index','fa-file-invoice-dollar','BILLING_MANAGEMENT', 10),
('Billing Charges', 'BillingCharge', 'Index', 'fa fa-money-bill', 'BILLING_CHARGES', 11),
('Medicines', 'Medicine', 'Index', 'fa fa-pills', 'MEDICINE_MANAGEMENT', 12),
('Prescription Templates', 'PrescriptionTemplate', 'Index', 'fa-file-medical', 'PRESCRIPTION_TEMPLATE', 13),
('Vital Definition', 'VitalDefinition', 'Index', 'fa-heartbeat', 'VITAL_DEFINITION', 14),
('Vitals', null, null, 'fa-heartbeat', 'VITAL', 15),
('Test Definition', 'TestDefinition', 'Index', 'fa-flask', 'TEST_DEFINITION', 16),
('Test Results', NULL, NULL, 'fa-flask', 'TEST_RESULT', 16);

-- Mandatory line
INSERT INTO BillingCharges
(ClinicId, ChargeCode, ChargeName, Description, ServiceType, Rate, IsActive)
VALUES
(0, 'CONSULT', 'Consultation', 'Doctor consultation fee', 'Consultation', 0.00, 1);

INSERT INTO BillNoGenerators
(ClinicId, BillNoPrefix, BillNoFY, BillNo)
VALUES
(1, 'B', 2026, 100);

-- Change ClinicId before running below script
SET IDENTITY_INSERT AccountGroups ON;

INSERT INTO AccountGroups (AccountGroupId, ClinicId, GroupName, ParentGroupId, Nature)
VALUES
(1, 1, 'Assets', NULL, 'Asset'),
(2, 1, 'Current Assets', 1, 'Asset'),
(3, 1, 'Fixed Assets', 1, 'Asset'),

(4, 1, 'Liabilities', NULL, 'Liability'),
(5, 1, 'Current Liabilities', 4, 'Liability'),

(6, 1, 'Equity', NULL, 'Equity'),

(7, 1, 'Income', NULL, 'Income'),
(8, 1, 'Operating Income', 7, 'Income'),
(9, 1, 'Other Income', 7, 'Income'),

(10, 1, 'Expenses', NULL, 'Expense'),
(11, 1, 'Operating Expenses', 10, 'Expense'),
(12, 1, 'Administrative Expenses', 10, 'Expense'),

(13, 1, 'Contra Revenue', NULL, 'Contra Revenue');

SET IDENTITY_INSERT AccountGroups OFF;

SET IDENTITY_INSERT LedgerAccounts ON;

INSERT INTO LedgerAccounts (LedgerAccountId, ClinicId, AccountGroupId, AccountCode, AccountName, IsActive)
VALUES

-- CURRENT ASSETS

(201, 1, 2, 'CA001', 'Cash In Hand', 1),
(202, 1, 2, 'CA002', 'Bank Account', 1),
(203, 1, 2, 'CA003', 'UPI Clearing Account', 1),
(204, 1, 2, 'CA004', 'Patient Receivable', 1),
(205, 1, 2, 'CA005', 'Insurance Receivable', 1),
(206, 1, 2, 'CA006', 'Pharmacy Inventory', 1),
(207, 1, 2, 'CA007', 'Medical Consumable Inventory', 1),
(208, 1, 2, 'CA008', 'Advance To Suppliers', 1),

-- FIXED ASSETS

(301, 1, 3, 'FA001', 'Medical Equipment', 1),
(302, 1, 3, 'FA002', 'Furniture And Fixtures', 1),
(303, 1, 3, 'FA003', 'Computers And IT Equipment', 1),

-- CURRENT LIABILITIES

(501, 1, 5, 'CL001', 'Supplier Payable', 1),
(502, 1, 5, 'CL002', 'GST Payable', 1),
(503, 1, 5, 'CL003', 'Salary Payable', 1),
(504, 1, 5, 'CL004', 'Professional Fees Payable', 1),
(505, 1, 5, 'CL005', 'Output GST Payable', 1),

-- EQUITY

(601, 1, 6, 'EQ001', 'Capital Account', 1),
(602, 1, 6, 'EQ002', 'Retained Earnings', 1),

-- OPERATING INCOME

(801, 1, 8, 'INC001', 'Consultation Revenue', 1),
(802, 1, 8, 'INC002', 'Lab Revenue', 1),
(803, 1, 8, 'INC003', 'Procedure Revenue', 1),
(804, 1, 8, 'INC004', 'Pharmacy Revenue', 1),
(805, 1, 8, 'INC005', 'Registration Revenue', 1),
(806, 1, 8, 'INC006', 'Vaccination Revenue', 1),
(807, 1, 8, 'INC007', 'Radiology Revenue', 1),

-- OTHER INCOME

(901, 1, 9, 'OIN001', 'Interest Income', 1),
(902, 1, 9, 'OIN002', 'Miscellaneous Income', 1),

-- OPERATING EXPENSES

(1101, 1, 11, 'EXP001', 'Medicine Consumption', 1),
(1102, 1, 11, 'EXP002', 'Medical Consumables Expense', 1),
(1103, 1, 11, 'EXP003', 'Laboratory Expense', 1),
(1104, 1, 11, 'EXP004', 'Doctor Consultation Expense', 1),
(1105, 1, 11, 'EXP005', 'Nursing Expense', 1),

-- COST OF GOODS SOLD
(1150, 1, 11, 'COGS001', 'Cost Of Goods Sold', 1),

-- ADMINISTRATIVE EXPENSES

(1201, 1, 12, 'AEXP001', 'Salary Expense', 1),
(1202, 1, 12, 'AEXP002', 'Rent Expense', 1),
(1203, 1, 12, 'AEXP003', 'Electricity Expense', 1),
(1204, 1, 12, 'AEXP004', 'Internet Expense', 1),
(1205, 1, 12, 'AEXP005', 'Telephone Expense', 1),
(1206, 1, 12, 'AEXP006', 'Software Subscription Expense', 1),
(1207, 1, 12, 'AEXP007', 'Repairs And Maintenance', 1),
(1208, 1, 12, 'AEXP008', 'Printing And Stationery', 1),
(1209, 1, 12, 'AEXP009', 'Marketing Expense', 1),
(1210, 1, 12, 'AEXP010', 'Miscellaneous Expense', 1),

-- CONTRA REVENUE

(1301, 1, 13, 'CNRP001', 'Discount Allowed', 1);


SET IDENTITY_INSERT LedgerAccounts OFF;

INSERT INTO Medicines
(
    MedicineCode,
    MedicineName,
    GenericName,
    HSNCode,
    Strength,
    DosageForm,
    UnitOfMeasure,
    GSTPercent,
    Manufacturer,
    ScheduleType,
    ReorderLevel,
    IsActive
)
VALUES

-- Fever / Pain

('PCM500','Paracetamol 500','Paracetamol','30049099','500 mg','Tablet','Tab',12.00,'Sun Pharma','OTC',50,1),
('PCM650','Paracetamol 650','Paracetamol','30049099','650 mg','Tablet','Tab',12.00,'Mankind Pharma','OTC',50,1),
('IBU400','Ibuprofen','Ibuprofen','30049099','400 mg','Tablet','Tab',12.00,'Abbott','OTC',40,1),
('ACE100','Aceclofenac','Aceclofenac','30049099','100 mg','Tablet','Tab',12.00,'Alkem','H',40,1),
('DIC50','Diclofenac','Diclofenac','30049099','50 mg','Tablet','Tab',12.00,'Sun Pharma','H',40,1),
('ETO60','Etoricoxib','Etoricoxib','30049099','60 mg','Tablet','Tab',12.00,'Torrent','H',30,1),

-- Antibiotics

('AZI250','Azithromycin 250','Azithromycin','30049099','250 mg','Tablet','Tab',12.00,'Cipla','H1',30,1),
('AZI500','Azithromycin 500','Azithromycin','30049099','500 mg','Tablet','Tab',12.00,'Cipla','H1',30,1),
('AMX500','Amoxicillin 500','Amoxicillin','30049099','500 mg','Capsule','Cap',12.00,'Alkem','H1',30,1),
('AMXCLV625','Amoxyclav 625','Amoxicillin + Clavulanic Acid','30049099','625 mg','Tablet','Tab',12.00,'Abbott','H1',30,1),
('CFX200','Cefixime 200','Cefixime','30049099','200 mg','Tablet','Tab',12.00,'Lupin','H1',25,1),
('DOX100','Doxycycline','Doxycycline','30049099','100 mg','Capsule','Cap',12.00,'Dr Reddys','H1',25,1),

-- Gastro

('PAN40','Pantoprazole','Pantoprazole','30049099','40 mg','Tablet','Tab',12.00,'Alkem','H',50,1),
('PANDSR','Pantop DSR','Pantoprazole + Domperidone','30049099','40 mg','Capsule','Cap',12.00,'Alkem','H',50,1),
('RAB20','Rabeprazole','Rabeprazole','30049099','20 mg','Tablet','Tab',12.00,'Dr Reddys','H',40,1),
('OMZ20','Omeprazole','Omeprazole','30049099','20 mg','Capsule','Cap',12.00,'Cipla','H',40,1),
('ESO40','Esomeprazole','Esomeprazole','30049099','40 mg','Capsule','Cap',12.00,'Sun Pharma','H',40,1),
('OND4','Ondansetron','Ondansetron','30049099','4 mg','Tablet','Tab',12.00,'Cipla','H',30,1),

-- Allergy / Respiratory

('CET10','Cetirizine','Cetirizine','30049099','10 mg','Tablet','Tab',12.00,'Cipla','OTC',50,1),
('LEV5','Levocetirizine','Levocetirizine','30049099','5 mg','Tablet','Tab',12.00,'Mankind Pharma','H',50,1),
('MON10','Montelukast','Montelukast','30049099','10 mg','Tablet','Tab',12.00,'Sun Pharma','H',40,1),
('MONLC','Montelukast LC','Montelukast + Levocetirizine','30049099','10/5 mg','Tablet','Tab',12.00,'Cipla','H',40,1),
('AMB30','Ambroxol','Ambroxol','30049099','30 mg','Tablet','Tab',12.00,'Lupin','H',30,1),

-- Diabetes

('MET500','Metformin 500','Metformin','30049099','500 mg','Tablet','Tab',12.00,'Sun Pharma','H',50,1),
('MET1000','Metformin 1000','Metformin','30049099','1000 mg','Tablet','Tab',12.00,'Sun Pharma','H',50,1),
('GLM1','Glimepiride 1','Glimepiride','30049099','1 mg','Tablet','Tab',12.00,'Torrent','H',40,1),
('GLM2','Glimepiride 2','Glimepiride','30049099','2 mg','Tablet','Tab',12.00,'Torrent','H',40,1),
('VGL50','Voglibose','Voglibose','30049099','0.2 mg','Tablet','Tab',12.00,'Zydus','H',30,1),

-- Hypertension

('TEL40','Telmisartan 40','Telmisartan','30049099','40 mg','Tablet','Tab',12.00,'Torrent','H',40,1),
('TEL80','Telmisartan 80','Telmisartan','30049099','80 mg','Tablet','Tab',12.00,'Torrent','H',40,1),
('AML5','Amlodipine 5','Amlodipine','30049099','5 mg','Tablet','Tab',12.00,'Dr Reddys','H',40,1),
('AML10','Amlodipine 10','Amlodipine','30049099','10 mg','Tablet','Tab',12.00,'Dr Reddys','H',40,1),
('LOS50','Losartan 50','Losartan','30049099','50 mg','Tablet','Tab',12.00,'Cipla','H',40,1),

-- Cardiology

('ATV10','Atorvastatin 10','Atorvastatin','30049099','10 mg','Tablet','Tab',12.00,'Sun Pharma','H',30,1),
('ATV20','Atorvastatin 20','Atorvastatin','30049099','20 mg','Tablet','Tab',12.00,'Sun Pharma','H',30,1),
('ROS10','Rosuvastatin 10','Rosuvastatin','30049099','10 mg','Tablet','Tab',12.00,'Cipla','H',30,1),
('ROS20','Rosuvastatin 20','Rosuvastatin','30049099','20 mg','Tablet','Tab',12.00,'Cipla','H',30,1),
('ECOS75','Ecosprin 75','Aspirin','30049099','75 mg','Tablet','Tab',12.00,'USV','H',30,1),

-- Vitamins

('VITC500','Vitamin C','Ascorbic Acid','30049099','500 mg','Tablet','Tab',12.00,'Zydus','OTC',50,1),
('B12TAB','Methylcobalamin','Methylcobalamin','30049099','1500 mcg','Tablet','Tab',12.00,'Lupin','H',50,1),
('CAL500','Calcium','Calcium Carbonate','30049099','500 mg','Tablet','Tab',12.00,'Cipla','OTC',50,1),
('D3K60','Vitamin D3','Cholecalciferol','30049099','60000 IU','Capsule','Cap',12.00,'Mankind Pharma','OTC',30,1),

-- Syrups

('PCMSYP','Paracetamol Syrup','Paracetamol','30049099','60 ml','Syrup','Bottle',12.00,'Cipla','OTC',20,1),
('AMBSYP','Ambroxol Syrup','Ambroxol','30049099','100 ml','Syrup','Bottle',12.00,'Lupin','H',20,1),
('COFSYP','Cough Syrup','Dextromethorphan','30049099','100 ml','Syrup','Bottle',12.00,'Dr Reddys','H',20,1),

-- Injections

('INJPCM','Paracetamol Injection','Paracetamol','30049099','100 ml','Injection','Vial',12.00,'Sun Pharma','H',20,1),
('INJCFT','Ceftriaxone Injection','Ceftriaxone','30049099','1 gm','Injection','Vial',12.00,'Alkem','H1',20,1),
('INJPAN','Pantoprazole Injection','Pantoprazole','30049099','40 mg','Injection','Vial',12.00,'Alkem','H',20,1),

-- Eye / Ear Drops

('MOXI','Moxifloxacin Eye Drop','Moxifloxacin','30049099','5 ml','Drops','Bottle',12.00,'Cipla','H',20,1),
('CMC','Carboxymethylcellulose','CMC','30049099','10 ml','Drops','Bottle',12.00,'Sun Pharma','OTC',20,1);

-- Vital Definitions

INSERT INTO VitalDefinitions
(ClinicId, VitalLabel, VitalCode, VitalType, DataType, VitalUnit, IsDerived, DeriveFormula, DisplayComposition,
    IsStandard, IsActive)
VALUES
(NULL, 'Height', 'HEIGHT', 'Item', 'Decimal', 'cm', 0, NULL, NULL, 1, 1),
(NULL, 'Weight', 'WEIGHT', 'Item', 'Decimal', 'kg', 0, NULL, NULL, 1, 1),
(NULL, 'Systolic Blood Pressure', 'SBP', 'CompositionItem', 'Number', 'mmHg', 0, NULL, NULL, 1, 1),
(NULL, 'Diastolic Blood Pressure', 'DBP', 'CompositionItem', 'Number', 'mmHg', 0, NULL, NULL, 1, 1),
(NULL, 'Pulse', 'PULSE', 'Item', 'Number', 'bpm', 0, NULL, NULL, 1, 1),
(NULL, 'SpO2', 'SPO2', 'Item', 'Number', '%', 0, NULL, NULL, 1, 1),
(NULL, 'Respiratory Rate', 'RESP_RATE', 'Item', 'Number', '/min', 0, NULL, NULL, 1, 1),
(NULL, 'Temperature', 'TEMPERATURE', 'Item', 'Decimal', '°F', 0, NULL, NULL, 1, 1),
(NULL, 'Pain Score', 'PAIN_SCORE', 'Item', 'Number', '/10', 0, NULL, NULL, 1, 1),
(NULL, 'Blood Glucose', 'BLOOD_GLUCOSE', 'Item', 'Decimal', 'mg/dL', 0, NULL, NULL, 1, 1),
(NULL, 'Blood Pressure', 'BP', 'Composition', 'Text', 'mmHg', 0, NULL, 'SBP / DBP', 1, 1),
(NULL, 'Body Mass Index', 'BMI', 'Derived', 'Decimal', 'kg/m²', 1, 'WEIGHT / ((HEIGHT / 100) * (HEIGHT / 100))', NULL, 1, 1),
(NULL, 'Mean Arterial Pressure', 'MAP', 'Derived', 'Decimal', 'mmHg', 1, '(SBP + (2 * DBP)) / 3', NULL, 1, 1),
(NULL, 'Body Surface Area', 'BSA', 'Derived', 'Decimal', 'm²', 1, 'SQRT((HEIGHT * WEIGHT) / 3600)', NULL, 1, 1);

/* ============================================================
   TEST PANELS
   Global definitions: ClinicId = NULL
   ============================================================ */

INSERT INTO TestPanels
(ClinicId, Code, Name, Description, DepartmentName, IsActive, DisplayOrder)
VALUES
(NULL, 'CBC', 'Complete Blood Count', 'Complete blood count including red blood cells, white blood cells, hemoglobin and platelets.', NULL, 1, 1),
(NULL, 'LFT', 'Liver Function Test', 'Assessment of liver enzymes, bilirubin, proteins and related liver function parameters.', NULL, 1, 2),
(NULL, 'RFT', 'Renal Function Test', 'Assessment of kidney function including urea, creatinine and electrolytes.', NULL, 1, 3),
(NULL, 'LIPID', 'Lipid Profile', 'Assessment of total cholesterol, triglycerides, HDL, LDL and related lipid parameters.', NULL, 1, 4),
(NULL, 'TFT', 'Thyroid Function Test', 'Assessment of thyroid function using TSH, T3 and T4 measurements.', NULL, 1, 5),
(NULL, 'DIABETES', 'Diabetes Profile', 'Assessment of blood glucose control including fasting glucose and HbA1c.', NULL, 1, 6),
(NULL, 'ELECTROLYTES', 'Serum Electrolytes', 'Measurement of major serum electrolytes including sodium, potassium and chloride.', NULL, 1, 7),
(NULL, 'URINE_ROUTINE', 'Urine Routine Examination', 'Routine physical, chemical and microscopic examination of urine.', NULL, 1, 8),
(NULL, 'IRON', 'Iron Profile', 'Assessment of iron status including serum iron, TIBC, transferrin saturation and ferritin.', NULL, 1, 9),
(NULL, 'VITAMIN', 'Vitamin Profile', 'Assessment of commonly measured vitamin levels including vitamin B12 and vitamin D.', NULL, 1, 10);

/* ============================================================
   TEST PARAMETERS
   Global definitions: ClinicId = NULL
   ============================================================ */

INSERT INTO TestParameters
(ClinicId, Code, Name, Description, DataType, Unit, NormalRange, NormalMin, NormalMax, TestMethod, IsActive)
VALUES

/* ============================================================
   CBC
   ============================================================ */

(NULL, 'HB',
 'Hemoglobin',
 'Concentration of hemoglobin in blood.',
 'Numeric',
 'g/dL',
 'Male: 13-17; Female: 12-15',
 NULL,
 NULL,
 'Photometric / Cyanmethemoglobin',
 1),
(NULL, 'RBC',
 'Red Blood Cell Count',
 'Number of red blood cells in blood.',
 'Numeric',
 'million/µL',
 'Male: 4.5-5.9; Female: 4.1-5.1',
 NULL,
 NULL,
 'Automated hematology analyzer',
 1),
(NULL, 'WBC',
 'White Blood Cell Count',
 'Total white blood cell count.',
 'Numeric',
 '10^3/µL',
 '4.0-11.0',
 4.0000,
 11.0000,
 'Automated hematology analyzer',
 1),
(NULL, 'PLT',
 'Platelet Count',
 'Number of platelets in blood.',
 'Numeric',
 '10^3/µL',
 '150-450',
 150.0000,
 450.0000,
 'Automated hematology analyzer',
 1),
(NULL, 'HCT',
 'Hematocrit',
 'Percentage of blood volume occupied by red blood cells.',
 'Numeric',
 '%',
 'Male: 40-52; Female: 36-48',
 NULL,
 NULL,
 'Calculated / Automated hematology analyzer',
 1),
(NULL, 'MCV',
 'Mean Corpuscular Volume',
 'Average volume of red blood cells.',
 'Numeric',
 'fL',
 '80-100',
 80.0000,
 100.0000,
 'Calculated by hematology analyzer',
 1),
(NULL, 'MCH',
 'Mean Corpuscular Hemoglobin',
 'Average amount of hemoglobin per red blood cell.',
 'Numeric',
 'pg',
 '27-33',
 27.0000,
 33.0000,
 'Calculated by hematology analyzer',
 1),
(NULL, 'MCHC',
 'Mean Corpuscular Hemoglobin Concentration',
 'Average hemoglobin concentration within red blood cells.',
 'Numeric',
 'g/dL',
 '32-36',
 32.0000,
 36.0000,
 'Calculated by hematology analyzer',
 1),
(NULL, 'RDW',
 'Red Cell Distribution Width',
 'Measure of variation in red blood cell size.',
 'Numeric',
 '%',
 '11.5-14.5',
 11.5000,
 14.5000,
 'Automated hematology analyzer',
 1),
(NULL, 'NEUT',
 'Neutrophils',
 'Percentage of white blood cells that are neutrophils.',
 'Numeric',
 '%',
 '40-75',
 40.0000,
 75.0000,
 'Automated differential count',
 1),
(NULL, 'LYMPH',
 'Lymphocytes',
 'Percentage of white blood cells that are lymphocytes.',
 'Numeric',
 '%',
 '20-45',
 20.0000,
 45.0000,
 'Automated differential count',
 1),
(NULL, 'EOS',
 'Eosinophils',
 'Percentage of white blood cells that are eosinophils.',
 'Numeric',
 '%',
 '1-6',
 1.0000,
 6.0000,
 'Automated differential count',
 1),

/* ============================================================
   LIVER FUNCTION
   ============================================================ */

(NULL, 'BIL_T',
 'Total Bilirubin',
 'Total serum bilirubin concentration.',
 'Numeric',
 'mg/dL',
 '0.3-1.2',
 0.3000,
 1.2000,
 'Diazo / Colorimetric',
 1),
(NULL, 'BIL_D',
 'Direct Bilirubin',
 'Conjugated/direct serum bilirubin concentration.',
 'Numeric',
 'mg/dL',
 '0.0-0.3',
 0.0000,
 0.3000,
 'Diazo / Colorimetric',
 1),
(NULL, 'BIL_I',
 'Indirect Bilirubin',
 'Unconjugated/indirect serum bilirubin concentration.',
 'Numeric',
 'mg/dL',
 '0.2-0.9',
 0.2000,
 0.9000,
 'Calculated',
 1),
(NULL, 'ALT',
 'Alanine Aminotransferase',
 'Liver enzyme used in assessment of hepatocellular injury.',
 'Numeric',
 'U/L',
 '7-56',
 7.0000,
 56.0000,
 'IFCC / Kinetic',
 1),
(NULL, 'AST',
 'Aspartate Aminotransferase',
 'Enzyme used in assessment of liver and other tissue injury.',
 'Numeric',
 'U/L',
 '10-40',
 10.0000,
 40.0000,
 'IFCC / Kinetic',
 1),
(NULL, 'ALP',
 'Alkaline Phosphatase',
 'Enzyme associated with liver, bile ducts and bone.',
 'Numeric',
 'U/L',
 '44-147',
 44.0000,
 147.0000,
 'IFCC / Colorimetric',
 1),
(NULL, 'GGT',
 'Gamma Glutamyl Transferase',
 'Enzyme useful in assessment of hepatobiliary disease.',
 'Numeric',
 'U/L',
 '9-48',
 9.0000,
 48.0000,
 'IFCC / Kinetic',
 1),
(NULL, 'TP',
 'Total Protein',
 'Total concentration of proteins in serum.',
 'Numeric',
 'g/dL',
 '6.0-8.3',
 6.0000,
 8.3000,
 'Biuret',
 1),
(NULL, 'ALB',
 'Albumin',
 'Major serum protein produced by the liver.',
 'Numeric',
 'g/dL',
 '3.5-5.0',
 3.5000,
 5.0000,
 'BCG / Colorimetric',
 1),

/* ============================================================
   RENAL FUNCTION
   ============================================================ */

(NULL, 'UREA',
 'Blood Urea',
 'Serum blood urea concentration.',
 'Numeric',
 'mg/dL',
 '15-45',
 15.0000,
 45.0000,
 'Urease / GLDH',
 1),
(NULL, 'CREAT',
 'Creatinine',
 'Serum creatinine concentration used to assess kidney function.',
 'Numeric',
 'mg/dL',
 'Male: 0.74-1.35; Female: 0.59-1.04',
 NULL,
 NULL,
 'Jaffe / Enzymatic',
 1),
(NULL, 'URIC',
 'Uric Acid',
 'Serum uric acid concentration.',
 'Numeric',
 'mg/dL',
 'Male: 3.4-7.0; Female: 2.4-6.0',
 NULL,
 NULL,
 'Uricase / Colorimetric',
 1),
(NULL, 'EGFR',
 'Estimated Glomerular Filtration Rate',
 'Estimated kidney filtration rate calculated from serum creatinine and demographic factors.',
 'Numeric',
 'mL/min/1.73m²',
 '>= 90',
 90.0000,
 NULL,
 'Calculated',
 1),

/* ============================================================
   LIPID PROFILE
   ============================================================ */

(NULL, 'CHOL',
 'Total Cholesterol',
 'Total cholesterol concentration in serum.',
 'Numeric',
 'mg/dL',
 '< 200',
 NULL,
 200.0000,
 'Enzymatic Colorimetric',
 1),
(NULL, 'HDL',
 'HDL Cholesterol',
 'High-density lipoprotein cholesterol.',
 'Numeric',
 'mg/dL',
 '>= 40',
 40.0000,
 NULL,
 'Enzymatic Colorimetric',
 1),
(NULL, 'LDL',
 'LDL Cholesterol',
 'Low-density lipoprotein cholesterol.',
 'Numeric',
 'mg/dL',
 '< 100',
 NULL,
 100.0000,
 'Direct / Calculated',
 1),
(NULL, 'TG',
 'Triglycerides',
 'Serum triglyceride concentration.',
 'Numeric',
 'mg/dL',
 '< 150',
 NULL,
 150.0000,
 'GPO-PAP / Enzymatic',
 1),
(NULL, 'VLDL',
 'VLDL Cholesterol',
 'Very-low-density lipoprotein cholesterol.',
 'Numeric',
 'mg/dL',
 '5-40',
 5.0000,
 40.0000,
 'Calculated',
 1),

/* ============================================================
   THYROID
   ============================================================ */

(NULL, 'TSH',
 'Thyroid Stimulating Hormone',
 'Hormone produced by the pituitary gland that regulates thyroid function.',
 'Numeric',
 'µIU/mL',
 '0.4-4.0',
 0.4000,
 4.0000,
 'CLIA / ECLIA',
 1),
(NULL, 'FT3',
 'Free Triiodothyronine',
 'Unbound active form of the thyroid hormone T3.',
 'Numeric',
 'pg/mL',
 '2.0-4.4',
 2.0000,
 4.4000,
 'CLIA / ECLIA',
 1),
(NULL, 'FT4',
 'Free Thyroxine',
 'Unbound thyroxine available for biological activity.',
 'Numeric',
 'ng/dL',
 '0.8-1.8',
 0.8000,
 1.8000,
 'CLIA / ECLIA',
 1),

/* ============================================================
   DIABETES
   ============================================================ */

(NULL, 'FBS',
 'Fasting Blood Glucose',
 'Blood glucose measured after an appropriate fasting period.',
 'Numeric',
 'mg/dL',
 '70-99',
 70.0000,
 99.0000,
 'Hexokinase / Glucose Oxidase',
 1),
(NULL, 'PPBS',
 'Postprandial Blood Glucose',
 'Blood glucose measured after a meal.',
 'Numeric',
 'mg/dL',
 '< 140',
 NULL,
 140.0000,
 'Hexokinase / Glucose Oxidase',
 1),
(NULL, 'HBA1C',
 'Glycated Hemoglobin',
 'Percentage of hemoglobin glycated over approximately the previous 2-3 months.',
 'Numeric',
 '%',
 '< 5.7',
 NULL,
 5.7000,
 'HPLC / Immunoturbidimetric',
 1),

/* ============================================================
   ELECTROLYTES
   ============================================================ */

(NULL, 'NA',
 'Sodium',
 'Serum sodium concentration.',
 'Numeric',
 'mmol/L',
 '135-145',
 135.0000,
 145.0000,
 'Ion Selective Electrode',
 1),
(NULL, 'K',
 'Potassium',
 'Serum potassium concentration.',
 'Numeric',
 'mmol/L',
 '3.5-5.1',
 3.5000,
 5.1000,
 'Ion Selective Electrode',
 1),
(NULL, 'CL',
 'Chloride',
 'Serum chloride concentration.',
 'Numeric',
 'mmol/L',
 '98-107',
 98.0000,
 107.0000,
 'Ion Selective Electrode',
 1),
(NULL, 'CA',
 'Calcium',
 'Total serum calcium concentration.',
 'Numeric',
 'mg/dL',
 '8.5-10.5',
 8.5000,
 10.5000,
 'Colorimetric',
 1),

/* ============================================================
   IRON PROFILE
   ============================================================ */

(NULL, 'IRON',
 'Serum Iron',
 'Concentration of circulating iron in serum.',
 'Numeric',
 'µg/dL',
 'Male: 65-175; Female: 50-170',
 NULL,
 NULL,
 'Ferrozine / Colorimetric',
 1),
(NULL, 'TIBC',
 'Total Iron Binding Capacity',
 'Measurement of the blood capacity to bind iron.',
 'Numeric',
 'µg/dL',
 '250-450',
 250.0000,
 450.0000,
 'Colorimetric',
 1),
(NULL, 'FERRITIN',
 'Ferritin',
 'Serum ferritin concentration reflecting body iron stores.',
 'Numeric',
 'ng/mL',
 'Male: 24-336; Female: 11-307',
 NULL,
 NULL,
 'CLIA / ECLIA',
 1),

/* ============================================================
   VITAMINS
   ============================================================ */

(NULL, 'B12',
 'Vitamin B12',
 'Serum vitamin B12 concentration.',
 'Numeric',
 'pg/mL',
 '200-900',
 200.0000,
 900.0000,
 'CLIA / ECLIA',
 1),
(NULL, 'VITD',
 'Vitamin D - 25 Hydroxy',
 'Serum 25-hydroxy vitamin D concentration.',
 'Numeric',
 'ng/mL',
 '30-100',
 30.0000,
 100.0000,
 'CLIA / ECLIA',
 1);

/* ============================================================
   CBC
   ============================================================ */

INSERT INTO TestPanelParameters
(
    ClinicId,
    TestPanelId,
    TestParameterId,
    DisplayOrder,
    IsActive
)
SELECT
    NULL,
    p.TestPanelId,
    tp.TestParameterId,
    v.DisplayOrder,
    1
FROM TestPanels p
CROSS APPLY
(
    VALUES
    ('HB',   1),
    ('RBC',  2),
    ('WBC',  3),
    ('PLT',  4),
    ('HCT',  5),
    ('MCV',  6),
    ('MCH',  7),
    ('MCHC', 8),
    ('RDW',  9),
    ('NEUT', 10),
    ('LYMPH', 11),
    ('EOS', 12)
) v(Code, DisplayOrder)
INNER JOIN TestParameters tp
    ON tp.Code = v.Code
   AND tp.ClinicId IS NULL
WHERE p.Code = 'CBC'
  AND p.ClinicId IS NULL;


/* ============================================================
   LFT
   ============================================================ */

INSERT INTO TestPanelParameters
(
    ClinicId,
    TestPanelId,
    TestParameterId,
    DisplayOrder,
    IsActive
)
SELECT
    NULL,
    p.TestPanelId,
    tp.TestParameterId,
    v.DisplayOrder,
    1
FROM TestPanels p
CROSS APPLY
(
    VALUES
    ('BIL_T', 1),
    ('BIL_D', 2),
    ('BIL_I', 3),
    ('ALT',   4),
    ('AST',   5),
    ('ALP',   6),
    ('GGT',   7),
    ('TP',    8),
    ('ALB',   9)
) v(Code, DisplayOrder)
INNER JOIN TestParameters tp
    ON tp.Code = v.Code
   AND tp.ClinicId IS NULL
WHERE p.Code = 'LFT'
  AND p.ClinicId IS NULL;


/* ============================================================
   RFT
   ============================================================ */

INSERT INTO TestPanelParameters
(
    ClinicId,
    TestPanelId,
    TestParameterId,
    DisplayOrder,
    IsActive
)
SELECT
    NULL,
    p.TestPanelId,
    tp.TestParameterId,
    v.DisplayOrder,
    1
FROM TestPanels p
CROSS APPLY
(
    VALUES
    ('UREA',  1),
    ('CREAT', 2),
    ('URIC',  3),
    ('EGFR',  4),
    ('NA',    5),
    ('K',     6),
    ('CL',    7),
    ('CA',    8)
) v(Code, DisplayOrder)
INNER JOIN TestParameters tp
    ON tp.Code = v.Code
   AND tp.ClinicId IS NULL
WHERE p.Code = 'RFT'
  AND p.ClinicId IS NULL;


/* ============================================================
   LIPID PROFILE
   ============================================================ */

INSERT INTO TestPanelParameters
(
    ClinicId,
    TestPanelId,
    TestParameterId,
    DisplayOrder,
    IsActive
)
SELECT
    NULL,
    p.TestPanelId,
    tp.TestParameterId,
    v.DisplayOrder,
    1
FROM TestPanels p
CROSS APPLY
(
    VALUES
    ('CHOL', 1),
    ('HDL',  2),
    ('LDL',  3),
    ('TG',   4),
    ('VLDL', 5)
) v(Code, DisplayOrder)
INNER JOIN TestParameters tp
    ON tp.Code = v.Code
   AND tp.ClinicId IS NULL
WHERE p.Code = 'LIPID'
  AND p.ClinicId IS NULL;


/* ============================================================
   THYROID PROFILE
   ============================================================ */

INSERT INTO TestPanelParameters
(
    ClinicId,
    TestPanelId,
    TestParameterId,
    DisplayOrder,
    IsActive
)
SELECT
    NULL,
    p.TestPanelId,
    tp.TestParameterId,
    v.DisplayOrder,
    1
FROM TestPanels p
CROSS APPLY
(
    VALUES
    ('TSH',  1),
    ('FT3',  2),
    ('FT4',  3)
) v(Code, DisplayOrder)
INNER JOIN TestParameters tp
    ON tp.Code = v.Code
   AND tp.ClinicId IS NULL
WHERE p.Code = 'TFT'
  AND p.ClinicId IS NULL;


/* ============================================================
   DIABETES PROFILE
   ============================================================ */

INSERT INTO TestPanelParameters
(
    ClinicId,
    TestPanelId,
    TestParameterId,
    DisplayOrder,
    IsActive
)
SELECT
    NULL,
    p.TestPanelId,
    tp.TestParameterId,
    v.DisplayOrder,
    1
FROM TestPanels p
CROSS APPLY
(
    VALUES
    ('FBS',   1),
    ('PPBS',  2),
    ('HBA1C', 3)
) v(Code, DisplayOrder)
INNER JOIN TestParameters tp
    ON tp.Code = v.Code
   AND tp.ClinicId IS NULL
WHERE p.Code = 'DIABETES'
  AND p.ClinicId IS NULL;


/* ============================================================
   ELECTROLYTES
   ============================================================ */

INSERT INTO TestPanelParameters
(
    ClinicId,
    TestPanelId,
    TestParameterId,
    DisplayOrder,
    IsActive
)
SELECT
    NULL,
    p.TestPanelId,
    tp.TestParameterId,
    v.DisplayOrder,
    1
FROM TestPanels p
CROSS APPLY
(
    VALUES
    ('NA', 1),
    ('K',  2),
    ('CL', 3),
    ('CA', 4)
) v(Code, DisplayOrder)
INNER JOIN TestParameters tp
    ON tp.Code = v.Code
   AND tp.ClinicId IS NULL
WHERE p.Code = 'ELECTROLYTES'
  AND p.ClinicId IS NULL;


/* ============================================================
   IRON PROFILE
   ============================================================ */

INSERT INTO TestPanelParameters
(
    ClinicId,
    TestPanelId,
    TestParameterId,
    DisplayOrder,
    IsActive
)
SELECT
    NULL,
    p.TestPanelId,
    tp.TestParameterId,
    v.DisplayOrder,
    1
FROM TestPanels p
CROSS APPLY
(
    VALUES
    ('IRON',    1),
    ('TIBC',    2),
    ('FERRITIN', 3)
) v(Code, DisplayOrder)
INNER JOIN TestParameters tp
    ON tp.Code = v.Code
   AND tp.ClinicId IS NULL
WHERE p.Code = 'IRON'
  AND p.ClinicId IS NULL;


/* ============================================================
   VITAMIN PROFILE
   ============================================================ */

INSERT INTO TestPanelParameters
(
    ClinicId,
    TestPanelId,
    TestParameterId,
    DisplayOrder,
    IsActive
)
SELECT
    NULL,
    p.TestPanelId,
    tp.TestParameterId,
    v.DisplayOrder,
    1
FROM TestPanels p
CROSS APPLY
(
    VALUES
    ('B12',  1),
    ('VITD', 2)
) v(Code, DisplayOrder)
INNER JOIN TestParameters tp
    ON tp.Code = v.Code
   AND tp.ClinicId IS NULL
WHERE p.Code = 'VITAMIN'
  AND p.ClinicId IS NULL;


/* ============================================================
   URINE ROUTINE
   ============================================================
   These parameters are intentionally omitted from the initial
   parameter seed above. See note below.
   ============================================================ */

INSERT INTO Roles VALUES
(1,'SuperAdmin'),
(2,'ClinicAdmin'),
(3,'ClinicUser'),
(4,'Doctor'),
(5,'BillingUser'),
(6, 'PharmacyUser');

-- Clinics
INSERT INTO Clinics (ClinicCode, Name) VALUES
('CLN001','City Care'),
('CLN002','Health Plus');

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
VALUES (1,'General'), (1,'Cardiology'), (1,'ENT');

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
('Medicine Management', 'MEDICINE_MANAGEMENT');

-- Role Mapping
INSERT INTO RoleFeatures (RoleId, FeatureId)
SELECT 1, FeatureId FROM Features WHERE Code IN ('CLINIC_VIEW', 'USER_VIEW', 'ROLE_VIEW');

INSERT INTO RoleFeatures (RoleId, FeatureId)
SELECT 2, FeatureId FROM Features WHERE Code IN ('DEPT_VIEW','DOC_VIEW','USER_VIEW','DOCTOR_SCHEDULE');

INSERT INTO RoleFeatures (RoleId, FeatureId)
SELECT 3, FeatureId FROM Features WHERE Code IN ('APPOINTMENT_BOOKING', 'PATIENT_HISTORY');

INSERT INTO RoleFeatures (RoleId, FeatureId)
SELECT 4, FeatureId FROM Features WHERE Code IN ('CONSULTATION_MANAGEMENT', 'PATIENT_HISTORY');

INSERT INTO RoleFeatures (RoleId, FeatureId)
SELECT 5, FeatureId FROM Features WHERE Code IN ('BILLING_CHARGES', 'PATIENT_HISTORY', 'BILLING_MANAGEMENT');

INSERT INTO RoleFeatures (RoleId, FeatureId)
SELECT 6, FeatureId FROM Features WHERE Code IN ('MEDICINE_MANAGEMENT');


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
('Medicines', 'Medicine', 'Index', 'fa fa-pills', 'MEDICINE_MANAGEMENT', 12);

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


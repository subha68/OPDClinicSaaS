INSERT INTO Roles VALUES
(1,'SuperAdmin'),
(2,'ClinicAdmin'),
(3,'ClinicUser'),
(4,'Doctor'),
(5,'BillingUser');

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
('Patient Search', 'PATIENT_SEARCH'),
('Billing Charges', 'BILLING_CHARGES');

-- Role Mapping
INSERT INTO RoleFeatures (RoleId, FeatureId)
SELECT 1, FeatureId FROM Features WHERE Code IN ('CLINIC_VIEW', 'USER_VIEW', 'ROLE_VIEW');

INSERT INTO RoleFeatures (RoleId, FeatureId)
SELECT 2, FeatureId FROM Features WHERE Code IN ('DEPT_VIEW','DOC_VIEW','USER_VIEW','DOCTOR_SCHEDULE');

INSERT INTO RoleFeatures (RoleId, FeatureId)
SELECT 3, FeatureId FROM Features WHERE Code IN ('APPOINTMENT_BOOKING', 'PATIENT_SEARCH');

INSERT INTO RoleFeatures (RoleId, FeatureId)
SELECT 4, FeatureId FROM Features WHERE Code IN ('CONSULTATION_MANAGEMENT', 'PATIENT_SEARCH');

INSERT INTO RoleFeatures (RoleId, FeatureId)
SELECT 5, FeatureId FROM Features WHERE Code IN ('BILLING_CHARGES', 'PATIENT_SEARCH');

-- Menus
INSERT INTO Menus (Title, Controller, Action, Icon, FeatureCode, DisplayOrder)
VALUES
('Users','User','Index','fa-users','USER_VIEW',1),
('Roles','Role','Index','fa-user-shield','ROLE_VIEW',2),
('Departments','Department','Index','fa-building','DEPT_VIEW',3),
('Doctors','Doctor','Index','fa-user-md','DOC_VIEW',4),
('Clinics','Clinic','Index','fa-hospital','CLINIC_VIEW',5),
('Doctor Schedule','DoctorSchedule','Index','fa-calendar','DOCTOR_SCHEDULE',6),
('Appointment Booking','AppointmentBooking','Index','fa-calendar-check','APPOINTMENT_BOOKING',7),
('Consultation','Consultation','Index','fa-stethoscope','CONSULTATION_MANAGEMENT',8),
('Patient Billing','Billing','Index','fa-file-invoice-dollar','BILLING_MANAGEMENT',9),
('Patient Search', 'Patient', 'Search', 'fa fa-user-injured', 'PATIENT_SEARCH', 10),
('Billing Charges', 'BillingCharge', 'Index', 'fa fa-money-bill', 'BILLING_CHARGES', 11);


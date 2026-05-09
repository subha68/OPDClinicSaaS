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
(1,5,'billing1','bill123');

-- Departments
INSERT INTO Departments (ClinicId, Name)
VALUES (1,'General'), (1,'Cardiology'), (1,'ENT');

-- Doctors
INSERT INTO Doctors (ClinicId, DepartmentId, FirstName)
VALUES
(1,1,'Dr. Amit'),
(1,2,'Dr. Roy');

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
('Manage Clinics', 'CLINIC_VIEW'),
('Doctor Schedule', 'DOCTOR_SCHEDULE');

-- Role Mapping
INSERT INTO RoleFeatures (RoleId, FeatureId)
SELECT 1, FeatureId FROM Features WHERE Code IN ('CLINIC_VIEW');

INSERT INTO RoleFeatures (RoleId, FeatureId)
SELECT 2, FeatureId FROM Features WHERE Code IN ('DEPT_VIEW','DOC_VIEW','USER_VIEW','DOCTOR_SCHEDULE');

-- Menus
INSERT INTO Menus (Title, Controller, Action, Icon, FeatureCode, DisplayOrder)
VALUES
('Users','User','Index','fa-users','USER_VIEW',1),
('Departments','Department','Index','fa-building','DEPT_VIEW',2),
('Doctors','Doctor','Index','fa-user-md','DOC_VIEW',3),
('Clinics','Clinic','Index','fa-hospital','CLINIC_VIEW',4),
('Doctor Schedule','DoctorSchedule','Index','fa-calendar','DOCTOR_SCHEDULE',5);


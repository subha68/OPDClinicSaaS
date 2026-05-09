CREATE DATABASE OPDClinicDB;
GO

USE OPDClinicDB;
GO

-- Clinics
CREATE TABLE Clinics (
    ClinicId INT IDENTITY(1,1) PRIMARY KEY,
    ClinicCode NVARCHAR(20) UNIQUE NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    LogoPath NVARCHAR(500),
    Address NVARCHAR(500),
    Phone NVARCHAR(20),
    Email NVARCHAR(100),
    CreatedAt DATETIME DEFAULT GETDATE()
);

-- Roles
CREATE TABLE Roles (
    RoleId INT PRIMARY KEY,
    RoleName NVARCHAR(50) UNIQUE NOT NULL
);

-- Users
CREATE TABLE Users (
    UserId INT IDENTITY PRIMARY KEY,
    ClinicId INT NULL,
    RoleId INT NOT NULL,
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    Phone NVARCHAR(20),
    Email NVARCHAR(100),
    Username NVARCHAR(50) UNIQUE,
    PasswordHash NVARCHAR(500),
    IsFirstLogin BIT DEFAULT 1,
    IsActive BIT DEFAULT 1,
    CreatedAt DATETIME DEFAULT GETDATE(),

    FOREIGN KEY (ClinicId) REFERENCES Clinics(ClinicId),
    FOREIGN KEY (RoleId) REFERENCES Roles(RoleId)
);

-- Departments
CREATE TABLE Departments (
    DepartmentId INT IDENTITY PRIMARY KEY,
    ClinicId INT NOT NULL,
    Name NVARCHAR(100),
    Description NVARCHAR(300),

    FOREIGN KEY (ClinicId) REFERENCES Clinics(ClinicId)
);

-- Doctors
CREATE TABLE Doctors (
    DoctorId INT IDENTITY PRIMARY KEY,
    ClinicId INT,
    DepartmentId INT,
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    RegistrationNo NVARCHAR(100),
    Qualification NVARCHAR(200),

    FOREIGN KEY (ClinicId) REFERENCES Clinics(ClinicId),
    FOREIGN KEY (DepartmentId) REFERENCES Departments(DepartmentId)
);

CREATE TABLE Patients (
    PatientId INT IDENTITY PRIMARY KEY,
    ClinicId INT,
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    Phone NVARCHAR(20),
    DOB DATE,
    Gender NVARCHAR(10),
    Address NVARCHAR(300),

    UNIQUE (FirstName, LastName, Phone, DOB),

    FOREIGN KEY (ClinicId) REFERENCES Clinics(ClinicId)
);

CREATE TABLE Appointments (
    AppointmentId INT IDENTITY PRIMARY KEY,
    ClinicId INT,
    DoctorId INT,
    PatientId INT,
    AppointmentDate DATE,
    SlotTime TIME,
    Status NVARCHAR(20),

    FOREIGN KEY (ClinicId) REFERENCES Clinics(ClinicId),
    FOREIGN KEY (DoctorId) REFERENCES Doctors(DoctorId),
    FOREIGN KEY (PatientId) REFERENCES Patients(PatientId)
);

CREATE TABLE Vitals (
    VitalId INT IDENTITY PRIMARY KEY,
    AppointmentId INT,
    BP NVARCHAR(20),
    Height FLOAT,
    Weight FLOAT,
    SPO2 INT,
    Pulse INT,
    BMI AS (Weight / (Height * Height)) PERSISTED,  // Could not be created

    FOREIGN KEY (AppointmentId) REFERENCES Appointments(AppointmentId)
);

CREATE TABLE MedicalHistories (
    HistoryId INT IDENTITY PRIMARY KEY,
    PatientId INT,
    Details NVARCHAR(MAX),

    FOREIGN KEY (PatientId) REFERENCES Patients(PatientId)
);

CREATE TABLE Medicines (
    MedicineId INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(200)
);

CREATE TABLE Consultations (
    ConsultationId INT IDENTITY PRIMARY KEY,
    AppointmentId INT,
    Diagnosis NVARCHAR(MAX),
    Notes NVARCHAR(MAX),

    FOREIGN KEY (AppointmentId) REFERENCES Appointments(AppointmentId)
);

CREATE TABLE Bills (
    BillId INT IDENTITY PRIMARY KEY,
    AppointmentId INT,
    TotalAmount DECIMAL(10,2),
    PaidAmount DECIMAL(10,2),
    Status NVARCHAR(20),

    FOREIGN KEY (AppointmentId) REFERENCES Appointments(AppointmentId)
);

CREATE TABLE Payments (
    PaymentId INT IDENTITY PRIMARY KEY,
    BillId INT,
    Amount DECIMAL(10,2),
    Mode NVARCHAR(50),
    ReferenceNo NVARCHAR(100),
    CreatedAt DATETIME DEFAULT GETDATE(),

    FOREIGN KEY (BillId) REFERENCES Bills(BillId)
);

CREATE TABLE Refunds (
    RefundId INT IDENTITY PRIMARY KEY,
    BillId INT,
    Amount DECIMAL(10,2),
    Reason NVARCHAR(200),

    FOREIGN KEY (BillId) REFERENCES Bills(BillId)
);

-- Features
CREATE TABLE Features (
    FeatureId INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(100),
    Code NVARCHAR(100) UNIQUE
);

-- RoleFeature Mapping
CREATE TABLE RoleFeatures (
    Id INT IDENTITY PRIMARY KEY,
    RoleId INT,
    FeatureId INT,
    FOREIGN KEY (RoleId) REFERENCES Roles(RoleId),
    FOREIGN KEY (FeatureId) REFERENCES Features(FeatureId)
);

-- Menu (DB-driven)
CREATE TABLE Menus (
    MenuId INT IDENTITY PRIMARY KEY,
    Title NVARCHAR(100),
    Controller NVARCHAR(50),
    Action NVARCHAR(50),
    Icon NVARCHAR(50),
    FeatureCode NVARCHAR(100),
    ParentId INT NULL,
    DisplayOrder INT
);

CREATE TABLE DoctorSchedules (
    ScheduleId INT IDENTITY(1,1) PRIMARY KEY,
    DoctorId INT NOT NULL,
    ClinicId INT NOT NULL,

    DayOfWeek INT NOT NULL,

    StartTime TIME NOT NULL,
    EndTime TIME NOT NULL,

    SlotDurationMinutes INT NOT NULL DEFAULT 15,
    MaxPatientsPerSlot INT NOT NULL DEFAULT 1,

    IsActive BIT NOT NULL DEFAULT 1,

    CreatedOn DATETIME NOT NULL DEFAULT GETDATE(),

    FOREIGN KEY (DoctorId) REFERENCES Doctors(DoctorId),
    FOREIGN KEY (ClinicId) REFERENCES Clinics(ClinicId)
);

CREATE TABLE DoctorScheduleException
(
    ExceptionId INT IDENTITY(1,1) PRIMARY KEY,

    DoctorId INT NOT NULL,
    ClinicId INT NOT NULL,

    ExceptionDate DATE NOT NULL,

    StartTime TIME NULL,
    EndTime TIME NULL,

    Reason NVARCHAR(200),

    IsFullDay BIT NOT NULL DEFAULT 1,

    FOREIGN KEY (DoctorId) REFERENCES Doctors(DoctorId),
    FOREIGN KEY (ClinicId) REFERENCES Clinics(ClinicId)
);

CREATE TABLE AppointmentSlot
(
    SlotId INT IDENTITY(1,1) PRIMARY KEY,

    DoctorId INT NOT NULL,
    ClinicId INT NOT NULL,

    SlotDate DATE NOT NULL,

    StartDateTime DATETIME NOT NULL,
    EndDateTime DATETIME NOT NULL,

    Capacity INT NOT NULL DEFAULT 1,
    BookedCount INT NOT NULL DEFAULT 0,

    IsBlocked BIT NOT NULL DEFAULT 0,

    CreatedOn DATETIME NOT NULL DEFAULT GETDATE(),

    FOREIGN KEY (DoctorId) REFERENCES Doctors(DoctorId),
    FOREIGN KEY (ClinicId) REFERENCES Clinics(ClinicId)
);


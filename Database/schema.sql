CREATE DATABASE OPDClinicDB;
GO

USE OPDClinicDB;
GO

-- Clinics
CREATE TABLE Clinics (
    ClinicId INT IDENTITY PRIMARY KEY,
    ClinicCode VARCHAR(20) UNIQUE,
    Name VARCHAR(100),
    Logo VARBINARY(MAX),
    Address VARCHAR(255),
    ContactNumber VARCHAR(15)
);

-- Roles
CREATE TABLE Roles (
    RoleId INT PRIMARY KEY,
    RoleName VARCHAR(50)
);

INSERT INTO Roles VALUES
(1,'SuperAdmin'),
(2,'ClinicAdmin'),
(3,'ClinicUser'),
(4,'Doctor'),
(5,'BillingUser');

-- Users
CREATE TABLE Users (
    UserId INT IDENTITY PRIMARY KEY,
    ClinicId INT NULL,
    RoleId INT,
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
    Email VARCHAR(100),
    ContactNumber VARCHAR(15),
    Username VARCHAR(50) UNIQUE,
    PasswordHash VARCHAR(255),
    IsFirstLogin BIT DEFAULT 1,
    FOREIGN KEY (ClinicId) REFERENCES Clinics(ClinicId),
    FOREIGN KEY (RoleId) REFERENCES Roles(RoleId)
);

-- Departments
CREATE TABLE Departments (
    DepartmentId INT IDENTITY PRIMARY KEY,
    ClinicId INT,
    Name VARCHAR(100),
    Description VARCHAR(255)
);

-- Doctors
CREATE TABLE Doctors (
    DoctorId INT IDENTITY PRIMARY KEY,
    ClinicId INT,
    DepartmentId INT,
    FirstName VARCHAR(50),
    LastName VARCHAR(50),
    Qualification VARCHAR(100),
    Specialisation VARCHAR(100),
    RegistrationNumber VARCHAR(50),
    Email VARCHAR(100),
    ContactNumber VARCHAR(15),
    FOREIGN KEY (DepartmentId) REFERENCES Departments(DepartmentId)
);
CREATE DATABASE OPDClinicDB;
GO

USE OPDClinicDB;
GO

-- Field Types
CREATE TABLE FieldTypes
(
    FieldTypeCode VARCHAR(30) PRIMARY KEY
);

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
    Salutation NVARCHAR(15),
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    RegistrationNo NVARCHAR(100),
    Qualification NVARCHAR(200),
    UserId INT NULL

    FOREIGN KEY (ClinicId) REFERENCES Clinics(ClinicId),
    FOREIGN KEY (DepartmentId) REFERENCES Departments(DepartmentId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);

ALTER TABLE Doctors
ADD CONSTRAINT UQ_Doctor_User UNIQUE(UserId);

CREATE TABLE PrescriptionTemplates
(
    TemplateId INT IDENTITY PRIMARY KEY,
    ClinicId INT,
    DepartmentId INT,
    DoctorId INT,
    TemplateName VARCHAR(100) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1

    FOREIGN KEY (ClinicId) REFERENCES Clinics(ClinicId)
);

CREATE TABLE PrescriptionTemplateSections
(
    SectionId INT IDENTITY PRIMARY KEY,
    TemplateId INT NOT NULL,
    ClinicId INT,
    SectionName VARCHAR(100) NOT NULL,
    DisplayOrder INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1

    FOREIGN KEY (ClinicId) REFERENCES Clinics(ClinicId),
    FOREIGN KEY (TemplateId) REFERENCES PrescriptionTemplates(TemplateId)
);

CREATE TABLE PrescriptionTemplateFields
(
    FieldId INT IDENTITY PRIMARY KEY,
    SectionId INT NOT NULL,
    TemplateId INT NOT NULL,
    ClinicId INT,
    FieldLabel VARCHAR(100) NOT NULL,
    FieldTypeCode VARCHAR(30) NOT NULL,
    IsRequired BIT NOT NULL DEFAULT 0,
    DisplayOrder INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1

    FOREIGN KEY (ClinicId) REFERENCES Clinics(ClinicId),
    FOREIGN KEY (SectionId) REFERENCES PrescriptionTemplateSections(SectionId),
    FOREIGN KEY (TemplateId) REFERENCES PrescriptionTemplates(TemplateId),
    FOREIGN KEY (FieldTypeCode) REFERENCES FieldTypes(FieldTypeCode)
);

CREATE TABLE PrescriptionTemplateFieldOptions
(
    OptionId INT IDENTITY PRIMARY KEY,
    FieldId INT NOT NULL,
    SectionId INT NOT NULL,
    TemplateId int not null,
    ClinicId INT,
    OptionText VARCHAR(100) NOT NULL,
    IsRequired BIT NOT NULL DEFAULT 0,
    DisplayOrder INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1

    FOREIGN KEY (ClinicId) REFERENCES Clinics(ClinicId),
    FOREIGN KEY (SectionId) REFERENCES PrescriptionTemplateSections(SectionId),
    FOREIGN KEY (TemplateId) REFERENCES PrescriptionTemplates(TemplateId),
    FOREIGN KEY (FieldId) REFERENCES PrescriptionTemplateFields(FieldId)
);

CREATE TABLE Patients (
    PatientId INT IDENTITY PRIMARY KEY,
    ClinicId INT,
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    Phone NVARCHAR(20),
    DOB DATE,
    Gender NVARCHAR(10),
    Address1 NVARCHAR(300),
    Address2 NVARCHAR(300),
    City NVARCHAR(50),
    State NVARCHAR(50),
    PinCode NVARCHAR(10),
    IdType NVARCHAR(15),
    IdNumber NVARCHAR(15),
    EmergencyContactName NVARCHAR(50),
    EmergencyRelationship NVARCHAR(50),
    EmergencyPhone NVARCHAR(50),    

    UNIQUE (FirstName, LastName, Phone, DOB),

    FOREIGN KEY (ClinicId) REFERENCES Clinics(ClinicId)
);

CREATE TABLE Visits (
    VisitId INT IDENTITY PRIMARY KEY,
    VisitDate DATE NOT NULL,
    VisitType NVARCHAR(10) NOT NULL,
    ClinicId INT NOT NULL,
    PatientId INT NOT NULL,

    FOREIGN KEY (PatientId) REFERENCES Patients(PatientId),
    FOREIGN KEY (ClinicId) REFERENCES Clinics(ClinicId)
);

CREATE TABLE Appointments (
    AppointmentId INT IDENTITY PRIMARY KEY,
    ClinicId INT NOT NULL,
    DoctorId INT NOT NULL,
    PatientId INT NOT NULL,
    VisitId INT NOT NULL,
    AppointmentDate DATE,
    SlotStartTime TIME,
    SlotEndTime TIME,
    Status NVARCHAR(20),
    CancellationReason NVARCHAR(250)

    FOREIGN KEY (ClinicId) REFERENCES Clinics(ClinicId),
    FOREIGN KEY (DoctorId) REFERENCES Doctors(DoctorId),
    FOREIGN KEY (PatientId) REFERENCES Patients(PatientId),
    FOREIGN KEY (VisitId) REFERENCES Visits(VisitId)   
);

CREATE TABLE Vitals (
    VitalId INT IDENTITY PRIMARY KEY,
    VisitId INT NOT NULL,
    BP NVARCHAR(20),
    Height FLOAT,
    Weight FLOAT,
    SPO2 INT,
    Pulse INT,
    BMI AS (Weight / (Height * Height)) PERSISTED,  // Could not be created

    FOREIGN KEY (VisitId) REFERENCES Visits(VisitId)
);

CREATE TABLE MedicalHistories (
    HistoryId INT IDENTITY PRIMARY KEY,
    PatientId INT,
    Details NVARCHAR(MAX),

    FOREIGN KEY (PatientId) REFERENCES Patients(PatientId)
);

CREATE TABLE Consultations (
    ConsultationId INT IDENTITY PRIMARY KEY,
    AppointmentId INT,
    Symptoms NVARCHAR(MAX),
    Diagnosis NVARCHAR(MAX),
    Notes NVARCHAR(MAX),

    FOREIGN KEY (AppointmentId) REFERENCES Appointments(AppointmentId)
);

CREATE TABLE BillingCharges (
    ChargeId INT IDENTITY PRIMARY KEY,
    ClinicId INT NOT NULL,
    ChargeCode NVARCHAR(20),
    ChargeName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(250),
    ServiceType NVARCHAR(50) NOT NULL,
    Rate DECIMAL(18,2) NOT NULL,
    IsActive BIT DEFAULT 1
);

CREATE TABLE Bills (
    BillId INT IDENTITY PRIMARY KEY,
    BillNo NVARCHAR(20) NOT NULL,
    BillDate DATE NOT NULL,
    VisitId INT NOT NULL,
    ClinicId INT NOT NULL,
    BillType NVARCHAR(20),
    GrossAmount DECIMAL(10,2),
    DiscountAmount DECIMAL(10,2),
    TaxAmount DECIMAL(10,2),
    NetAmount DECIMAL(10,2),
    PaidAmount DECIMAL(10,2),
    PaymentStatus NVARCHAR(20),
    Remarks NVARCHAR(100)

    FOREIGN KEY (VisitId) REFERENCES Visits(VisitId),
    FOREIGN KEY (ClinicId) REFERENCES Clinics(ClinicId)
);

CREATE TABLE BillLines (
    LineId INT IDENTITY PRIMARY KEY,
    LineNo INT NOT NULL,
    BillId INT NOT NULL,
    ServiceType NVARCHAR(50),
    ReferenceId INT,
    ReferenceCode NVARCHAR(50),
    ReferenceName NVARCHAR(100),
    Description NVARCHAR(200),
    Quantity INT,
    Rate DECIMAL(18,2),
    GrossAmount DECIMAL(18,2),
    DiscountPercent DECIMAL(5,2),
    DiscountAmount DECIMAL(18,2),
    NetAmount DECIMAL(18,2),

    FOREIGN KEY (BillId) REFERENCES Bills(BillId),
    FOREIGN KEY (ChargeId) REFERENCES BillingCharges(ChargeId)
);

CREATE TABLE Payments (
    PaymentId INT IDENTITY PRIMARY KEY,
    BillId INT NOT NULL,
    ClinicId INT NOT NULL,
    PaymentDate DATE NOT NULL,
    Amount DECIMAL(10,2),
    Mode NVARCHAR(50),
    ReferenceNo NVARCHAR(100),
    CreatedAt DATETIME DEFAULT GETDATE(),

    FOREIGN KEY (BillId) REFERENCES Bills(BillId),
    FOREIGN KEY (ClinicId) REFERENCES Clinics(ClinicId)
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

CREATE TABLE PharmacologyAdvices
(
    PharmacologyId INT IDENTITY PRIMARY KEY,
    ConsultationId INT NOT NULL,
    MedicineId INT,
    MedicineName NVARCHAR(200),
    Dosage NVARCHAR(100),
    Frequency NVARCHAR(100),
    Duration NVARCHAR(100),
    Instructions NVARCHAR(500),

    FOREIGN KEY (ConsultationId) REFERENCES Consultations(ConsultationId)
    FOREIGN KEY (MedicineId) REFERENCES Medicines(MedicineId)
);

CREATE TABLE Medicines
(
    MedicineId INT IDENTITY(1,1) PRIMARY KEY,
    MedicineCode NVARCHAR(20) NOT NULL,
    MedicineName NVARCHAR(200) NOT NULL,
    GenericName NVARCHAR(200) NULL,
    HSNCode NVARCHAR(20),
    Strength NVARCHAR(50) NULL,
    DosageForm NVARCHAR(50) NULL,      -- Tablet, Capsule, Syrup, Injection
    UnitOfMeasure NVARCHAR(20) NULL,   -- Tab, Cap, Bottle, Vial
    ReorderLevel INT NOT NULL DEFAULT 0,
    GSTPercent DECIMAL(5,2) NULL,
    Manufacturer NVARCHAR(200) NULL,
    ScheduleType NVARCHAR(20) NULL,    -- OTC (Over-the-counter), H (Prescription Drug), H1 (Restricted Antibiotics), X (Controlled Drugs)
    CreatedFromPrescription BIT,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NULL
);

CREATE TABLE MedicineBatches
(
    BatchId INT IDENTITY PRIMARY KEY,
    MedicineId INT,
    BatchNo NVARCHAR(50),
    ExpiryDate DATE,
    PurchaseRate DECIMAL(18,2),
    SaleRate DECIMAL(18,2),
    CurrentQty INT

    FOREIGN KEY (MedicineId) REFERENCES Medicines(MedicineId)
);

CREATE TABLE Suppliers
(
    SupplierId INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(50),
    GSTNo NVARCHAR(25)
);

CREATE TABLE PurchaseOrders
(
    PurchaseOrderId INT IDENTITY PRIMARY KEY,
    SupplierId INT,
    PODate DATE,
    Status NVARCHAR(25)

    FOREIGN KEY (SupplierId) REFERENCES Suppliers(SupplierId)
);

CREATE TABLE POLines
(
    POLineId INT IDENTITY PRIMARY KEY,
    POLineNo INT,
    PurchaseOrderId INT,
    MedicineId INT,
    Quantity INT,
    Rate DECIMAL(10,2)

    FOREIGN KEY (PurchaseOrderId) REFERENCES PurchaseOrders(PurchaseOrderId),
    FOREIGN KEY (MedicineId) REFERENCES Medicines(MedicineId)    
);

CREATE TABLE GoodsReceipts
(
    GRNId INT IDENTITY PRIMARY KEY,
    SupplierId INT,
    PurchaseOrderId INT,
    GRNDate DATE,
    InvoiceNo NVARCHAR(20)

    FOREIGN KEY (SupplierId) REFERENCES Suppliers(SupplierId),
    FOREIGN KEY (PurchaseOrderId) REFERENCES PurchaseOrders(PurchaseOrderId)    
);

CREATE TABLE GoodsReceiptLines
(
    GRNLineId INT IDENTITY PRIMARY KEY,
    GRNLineNo INT,
    MedicineId INT,
    BatchNo NVARCHAR(50),
    ExpiryDate DATE,
    Quantity INT,
    PurchaseRate DECIMAL(10,2),
    SaleRate DECIMAL(10,2)

    FOREIGN KEY (MedicineId) REFERENCES Medicines(MedicineId)    
);

CREATE TABLE PharmacyBills
(
    PharmacyBillId INT IDENTITY PRIMARY KEY,
    PharmacyBillNo NVARCHAR(30) NOT NULL,
    PatientId INT,
    PharmacologyId INT,
    BillDate DATE,
    GrossAmount DECIMAL(10,2) NOT NULL DEFAULT 0,
    Discount DECIMAL(10,2) NOT NULL DEFAULT 0,
    Tax DECIMAL(10,2) NOT NULL DEFAULT 0,
    CGST DECIMAL(10,2) NOT NULL DEFAULT 0,
    SGST DECIMAL(10,2) NOT NULL DEFAULT 0,
    IGST DECIMAL(10,2) NOT NULL DEFAULT 0,
    NetAmount DECIMAL(10,2)  NOT NULL DEFAULT 0,
    Remarks NVARCHAR(100)

    FOREIGN KEY (PatientId) REFERENCES Patients(PatientId),
    FOREIGN KEY (PharmacologyId) REFERENCES PharmacologyAdvices(PharmacologyId)
);

CREATE TABLE PharmacyBillLines
(
    PBLineId INT IDENTITY PRIMARY KEY,
    PharmacyBillId INT NOT NULL,
    PBLineNo INT,
    MedicineId INT,
    BatchId INT,
    Quantity INT,
    Rate DECIMAL(10,2),
    GrossAmount DECIMAL(10,2),
    DiscountPercent DECIMAL(10,2),
    DiscountAmount DECIMAL(10,2),
    TaxPercent DECIMAL(10,2),
    TaxAmount DECIMAL(10,2),
    NetAmount DECIMAL(10,2)

    FOREIGN KEY (MedicineId) REFERENCES Medicines(MedicineId),   
    FOREIGN KEY (BatchId) REFERENCES MedicineBatches(BatchId),
    FOREIGN KEY (PharmacyBillId) REFERENCES PharmacyBills(PharmacyBillId)
);

CREATE TABLE StockLedgers
(
    StockLedgerId INT IDENTITY PRIMARY KEY,
    MedicineId INT,
    BatchId INT,
    TransactionDate DATE,
    TransactionType NVARCHAR(20), -- GRN, SALE, ADJUSTMENT, RETURN
    ReferenceType NVARCHAR(20),
    ReferenceId INT,
    QuantityIn INT,
    QuantityOut INT,
    BalanceQty INT

    FOREIGN KEY (MedicineId) REFERENCES Medicines(MedicineId),
    FOREIGN KEY (BatchId) REFERENCES MedicineBatches(BatchId)
);

CREATE TABLE AccountGroups
(
    AccountGroupId INT IDENTITY(1,1) PRIMARY KEY,
    ClinicId INT NOT NULL,
    GroupName VARCHAR(100) NOT NULL,
    ParentGroupId INT NULL,
    Nature VARCHAR(20) NOT NULL

    FOREIGN KEY (ClinicId) REFERENCES Clinics(ClinicId)
);

CREATE TABLE LedgerAccounts
(
    LedgerAccountId INT IDENTITY(1,1) PRIMARY KEY,
    ClinicId INT NOT NULL,
    AccountGroupId INT NOT NULL,
    AccountCode VARCHAR(20),
    AccountName VARCHAR(200) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,

    FOREIGN KEY(AccountGroupId) REFERENCES AccountGroups(AccountGroupId),
    FOREIGN KEY (ClinicId) REFERENCES Clinics(ClinicId)
);

CREATE TABLE JournalHeaders
(
    JournalHeaderId INT IDENTITY(1,1) PRIMARY KEY,
    ClinicId INT NOT NULL,
    JournalDate DATETIME NOT NULL,
    VoucherType VARCHAR(20) NOT NULL,
    ReferenceType VARCHAR(50),
    ReferenceId INT,
    Narration VARCHAR(500),
    CreatedOn DATETIME NOT NULL DEFAULT GETDATE()

    FOREIGN KEY (ClinicId) REFERENCES Clinics(ClinicId)
);

CREATE TABLE JournalLines
(
    JournalLineId INT IDENTITY(1,1) PRIMARY KEY,
    ClinicId INT NOT NULL,
    JournalHeaderId INT NOT NULL,
    LedgerAccountId INT NOT NULL,
    DebitAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
    CreditAmount DECIMAL(18,2) NOT NULL DEFAULT 0,

    FOREIGN KEY(JournalHeaderId) REFERENCES JournalHeaders(JournalHeaderId),
    FOREIGN KEY(LedgerAccountId) REFERENCES LedgerAccounts(LedgerAccountId),
    FOREIGN KEY (ClinicId) REFERENCES Clinics(ClinicId)
);



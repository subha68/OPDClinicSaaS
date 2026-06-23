namespace Helpers
{
    public static class LedgerAccountCode
    {
        // CURRENT ASSETS
        public const string CashInHand = "CA001";
        public const string BankAccount = "CA002";
        public const string UPIClearingAccount = "CA003";
        public const string PatientReceivable = "CA004";
        public const string InsuranceReceivable = "CA005";
        public const string PharmacyInventory = "CA006";
        public const string MedicalConsumableInventory = "CA007";
        public const string AdvanceToSuppliers = "CA008";

        // FIXED ASSETS
        public const string MedicalEquipment = "FA001";
        public const string FurnitureAndFixtures = "FA002";
        public const string ComputersAndITEquipment = "FA003";

        // CURRENT LIABILITIES
        public const string SupplierPayable = "CL001";
        public const string GSTPayable = "CL002";
        public const string SalaryPayable = "CL003";
        public const string ProfessionalFeesPayable = "CL004";
        public const string OutputGSTPayable = "CL005";

        // EQUITY
        public const string CapitalAccount = "EQ001";
        public const string RetainedEarnings = "EQ002";

        // OPERATING INCOME
        public const string ConsultationRevenue = "INC001";
        public const string LabRevenue = "INC002";
        public const string ProcedureRevenue = "INC003";
        public const string PharmacyRevenue = "INC004";
        public const string RegistrationRevenue = "INC005";
        public const string VaccinationRevenue = "INC006";
        public const string RadiologyRevenue = "INC007";

        // OTHER INCOME
        public const string InterestIncome = "OIN001";
        public const string MiscellaneousIncome = "OIN002";

        // OPERATING EXPENSES
        public const string MedicineConsumption = "EXP001";
        public const string MedicalConsumablesExpense = "EXP002";
        public const string LaboratoryExpense = "EXP003";
        public const string DoctorConsultationExpense = "EXP004";
        public const string NursingExpense = "EXP005";

        // ADMINISTRATIVE EXPENSES
        public const string SalaryExpense = "AEXP001";
        public const string RentExpense = "AEXP002";
        public const string ElectricityExpense = "AEXP003";
        public const string InternetExpense = "AEXP004";
        public const string TelephoneExpense = "AEXP005";
        public const string SoftwareSubscriptionExpense = "AEXP006";
        public const string RepairsAndMaintenance = "AEXP007";
        public const string PrintingAndStationery = "AEXP008";
        public const string MarketingExpense = "AEXP009";
        public const string MiscellaneousExpense = "AEXP010";

        // COST OF GOODS SOLD
        public const string CostOfGoodsSold = "COGS001";

        // CONTRA REVENUE
        public const string DiscountAllowed = "CNRP001";
    }
}
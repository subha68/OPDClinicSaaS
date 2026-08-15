using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;

namespace DAL
{
    public class MedicineDAL : IDAL<Medicine>
    {
        private readonly DbHelper _db;

        public MedicineDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(Medicine medicine, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"INSERT INTO Medicines 
                            (MedicineCode, MedicineName, GenericName, HSNCode, Strength, DosageForm, 
                            UnitOfMeasure, ReorderLevel, GSTPercent, Manufacturer, ScheduleType, CreatedFromPrescription, IsActive)
                            VALUES (@MedicineCode, @MedicineName, @GenericName, @HSNCode, @Strength, @DosageForm, 
                            @UnitOfMeasure, @ReorderLevel, @GSTPercent, @Manufacturer, @ScheduleType, @CreatedFromPrescription, @IsActive);
                            SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@MedicineCode", medicine.MedicineCode);
            cmd.Parameters.AddWithValue("@MedicineName", medicine.MedicineName);
            cmd.Parameters.AddWithValue("@GenericName", medicine.GenericName);
            cmd.Parameters.AddWithValue("@HSNCode", medicine.HSNCode);
            cmd.Parameters.AddWithValue("@Strength", medicine.Strength);
            cmd.Parameters.AddWithValue("@DosageForm", medicine.DosageForm);
            cmd.Parameters.AddWithValue("@UnitOfMeasure", medicine.UnitOfMeasure);
            cmd.Parameters.AddWithValue("@ReorderLevel", medicine.ReorderLevel);
            cmd.Parameters.AddWithValue("@GSTPercent", medicine.GSTPercent);
            cmd.Parameters.AddWithValue("@Manufacturer", medicine.Manufacturer);
            cmd.Parameters.AddWithValue("@ScheduleType", medicine.ScheduleType);
            cmd.Parameters.AddWithValue("@CreatedFromPrescription", medicine.CreatedFromPrescription);
            cmd.Parameters.AddWithValue("@IsActive", medicine.IsActive);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool Update(Medicine medicine, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"UPDATE Medicines
                             SET MedicineCode = @MedicineCode, 
                                MedicineName = @MedicineName, 
                                GenericName = @GenericName, 
                                HSNCode = @HSNCode, 
                                Strength = @Strength,
                                DosageForm = @DosageForm, 
                                UnitOfMeasure = @UnitOfMeasure, 
                                ReorderLevel = @ReorderLevel,
                                GSTPercent = @GSTPercent, 
                                Manufacturer = @Manufacturer, 
                                ScheduleType = @ScheduleType,
                                CreatedFromPrescription = @CreatedFromPrescription, 
                                IsActive = @IsActive
                             WHERE MedicineId = @id";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@MedicineCode", medicine.MedicineCode);
            cmd.Parameters.AddWithValue("@MedicineName", medicine.MedicineName);
            cmd.Parameters.AddWithValue("@GenericName", medicine.GenericName);
            cmd.Parameters.AddWithValue("@HSNCode", medicine.HSNCode);
            cmd.Parameters.AddWithValue("@Strength", medicine.Strength);
            cmd.Parameters.AddWithValue("@DosageForm", medicine.DosageForm);
            cmd.Parameters.AddWithValue("@UnitOfMeasure", medicine.UnitOfMeasure);
            cmd.Parameters.AddWithValue("@ReorderLevel", medicine.ReorderLevel);
            cmd.Parameters.AddWithValue("@GSTPercent", medicine.GSTPercent);
            cmd.Parameters.AddWithValue("@Manufacturer", medicine.Manufacturer);
            cmd.Parameters.AddWithValue("@ScheduleType", medicine.ScheduleType);
            cmd.Parameters.AddWithValue("@CreatedFromPrescription", medicine.CreatedFromPrescription);
            cmd.Parameters.AddWithValue("@IsActive", medicine.IsActive);
            cmd.Parameters.AddWithValue("@id", medicine.MedicineId);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public bool Delete(int medicineId, int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"DELETE FROM Medicines WHERE MedicineId = @id";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@id", medicineId);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public List<Medicine> GetAll(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<Medicine>();

            var cmd = new SqlCommand("SELECT * FROM Medicines", conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Medicine
                {
                    MedicineId = (int)reader["MedicineId"],
                    MedicineCode = reader["MedicineCode"].ToString(),
                    MedicineName = reader["MedicineName"].ToString(),
                    GenericName = reader["GenericName"].ToString(),
                    HSNCode = reader["HSNCode"].ToString(),
                    Strength = reader["Strength"].ToString(),
                    DosageForm = reader["DosageForm"].ToString(),
                    UnitOfMeasure = reader["UnitOfMeasure"].ToString(),
                    ReorderLevel = Convert.ToInt32(reader["ReorderLevel"]),
                    GSTPercent = Convert.ToDecimal(reader["GSTPercent"]),
                    Manufacturer = reader["Manufacturer"].ToString(),
                    ScheduleType = reader["ScheduleType"].ToString(),
                    CreatedFromPrescription = Convert.ToBoolean(reader["CreatedFromPrescription"]),
                    IsActive = Convert.ToBoolean(reader["IsActive"])
                });
            }

            return list;
        }

        public List<Medicine> GetAll(string term, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<Medicine>();

            string query = @"SELECT TOP 10
                                MedicineId,
                                MedicineName
                            FROM Medicines
                            WHERE MedicineName LIKE '%' + @term + '%'
                            ORDER BY MedicineName";

            var cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@term", term);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Medicine
                {
                    MedicineId = (int)reader["MedicineId"],
                    MedicineName = reader["MedicineName"].ToString()
                });
            }

            return list;
        }

        public Medicine Get(int medicineId, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            Medicine medicine = null;

            var cmd = new SqlCommand("SELECT * FROM Medicines WHERE MedicineId = @id", conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@id", medicineId);
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                medicine = new Medicine
                {
                    MedicineId = (int)reader["MedicineId"],
                    MedicineCode = reader["MedicineCode"].ToString(),
                    MedicineName = reader["MedicineName"].ToString(),
                    GenericName = reader["GenericName"].ToString(),
                    HSNCode = reader["HSNCode"].ToString(),
                    Strength = reader["Strength"].ToString(),
                    DosageForm = reader["DosageForm"].ToString(),
                    UnitOfMeasure = reader["UnitOfMeasure"].ToString(),
                    ReorderLevel = Convert.ToInt32(reader["ReorderLevel"]),
                    GSTPercent = Convert.ToDecimal(reader["GSTPercent"]),
                    Manufacturer = reader["Manufacturer"].ToString(),
                    ScheduleType = reader["ScheduleType"].ToString(),
                    CreatedFromPrescription = Convert.ToBoolean(reader["CreatedFromPrescription"]),
                    IsActive = Convert.ToBoolean(reader["IsActive"])
                };
            }

            return medicine;
        }

        public Medicine GetByName(string name, SqlConnection conn, SqlTransaction? tran = null)
        {
            Medicine medicine = null;

            var cmd = new SqlCommand("SELECT * FROM Medicines WHERE MedicineName = @MedicineName", conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@MedicineName", name);
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                medicine = new Medicine
                {
                    MedicineId = (int)reader["MedicineId"],
                    MedicineCode = reader["MedicineCode"].ToString(),
                    MedicineName = reader["MedicineName"].ToString(),
                    GenericName = reader["GenericName"].ToString(),
                    HSNCode = reader["HSNCode"].ToString(),
                    Strength = reader["Strength"].ToString(),
                    DosageForm = reader["DosageForm"].ToString(),
                    UnitOfMeasure = reader["UnitOfMeasure"].ToString(),
                    ReorderLevel = Convert.ToInt32(reader["ReorderLevel"]),
                    GSTPercent = Convert.ToDecimal(reader["GSTPercent"]),
                    Manufacturer = reader["Manufacturer"].ToString(),
                    ScheduleType = reader["ScheduleType"].ToString(),
                    CreatedFromPrescription = Convert.ToBoolean(reader["CreatedFromPrescription"]),
                    IsActive = Convert.ToBoolean(reader["IsActive"])
                };
            }

            return medicine;
        }

    }
}
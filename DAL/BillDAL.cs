using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;
using ViewModels;

namespace DAL
{
    public class BillDAL : BaseDAL, IDAL<Bill>
    {
        private readonly DbHelper _db;

        public BillDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(Bill bill, SqlConnection conn, SqlTransaction tran)
        {
            int billId = 0;

            string query = @"INSERT INTO Bills 
                                (BillNo, PatientId, VisitId, AppointmentId, ClinicId, BillDate, GrossAmount, DiscountAmount, 
                                NetAmount, PaidAmount, Status, CreatedBy) 
                                VALUES (@BillNo, @patientId, @visitId, @AppointmentId, @ClinicId, @billDate, @grossAmount, @discountAmount,
                                @netAmount, @paidAmount, @Status, @CreatedBy);
                            SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@BillNo", bill.BillNo);
            cmd.Parameters.AddWithValue("@patientId", bill.PatientId);
            cmd.Parameters.AddWithValue("@visitId", bill.VisitId);
            cmd.Parameters.AddWithValue("@AppointmentId", bill.AppointmentId.HasValue ? bill.AppointmentId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@billDate", bill.BillDate);
            cmd.Parameters.AddWithValue("@grossAmount", bill.GrossAmount);
            cmd.Parameters.AddWithValue("@discountAmount", bill.DiscountAmount);
            cmd.Parameters.AddWithValue("@netAmount", bill.NetAmount);
            cmd.Parameters.AddWithValue("@paidAmount", 0); // Assuming new bills are unpaid
            cmd.Parameters.AddWithValue("@ClinicId", bill.ClinicId);
            cmd.Parameters.AddWithValue("@Status", bill.PaymentStatus);
            cmd.Parameters.AddWithValue("@CreatedBy", bill.CreatedBy);

            billId = Convert.ToInt32(cmd.ExecuteScalar());

            return billId;
        }

        public bool Update(Bill bill, SqlConnection conn, SqlTransaction tran)
        {
            // bills once created cannot be updated
            throw new NotSupportedException("Bills cannot be modified.");
        }

        public bool UpdatePayment(int billId, decimal totalPaid, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"UPDATE Bills
                                SET PaidAmount = @paidAmount
                                WHERE BillId = @billId";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@paidAmount", totalPaid);
            cmd.Parameters.AddWithValue("@billId", billId);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }


        public bool Delete(int id, int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            // bills once created cannot be deleted
            throw new NotSupportedException("Bills cannot be deleted.");

        }

        public Bill Get(int id, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = @"SELECT b.*,
                                    v.VisitDate,
                                    a.DoctorId 
                            FROM Bills b 
                            JOIN Visits v 
                                ON b.VisitId = v.VisitId
                            LEFT JOIN Appointments a
                                ON b.AppointmentId = a.AppointmentId
                            WHERE b.BillId = @id
                            AND b.ClinicId = @ClinicId";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using var reader = cmd.ExecuteReader();

            var bill = new Bill();

            if (reader.Read())
            {
                return new Bill
                {
                    BillId = (int)reader["BillId"],
                    BillNo = reader["BillNo"].ToString(),
                    ClinicId = (int)reader["ClinicId"],
                    PatientId = (int)reader["PatientId"],
                    VisitId = (int)reader["VisitId"],
                    AppointmentId = reader["AppointmentId"] != DBNull.Value ? (int)reader["AppointmentId"] : null,
                    DoctorId = reader["DoctorId"] != DBNull.Value ? (int)reader["DoctorId"] : null,
                    VisitDate = (DateTime)reader["VisitDate"],
                    BillDate = (DateTime)reader["BillDate"],
                    GrossAmount = (decimal)reader["GrossAmount"],
                    DiscountAmount = (decimal)reader["DiscountAmount"],
                    TotalPaidAmount = (decimal)reader["PaidAmount"]
                };
            }

            return null;
        }

        public List<Bill> GetByAppointment(int appointmentId, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<Bill>();

            string query = @"
                SELECT
                    b.*,
                    v.VisitDate,
                    a.DoctorId
                FROM Bills b
                JOIN Visits v
                    ON b.VisitId = v.VisitId
                JOIN Appointments a
                    ON b.AppointmentId = a.AppointmentId
                WHERE b.AppointmentId = @AppointmentId
                AND b.ClinicId = @ClinicId
                ORDER BY b.BillDate DESC, b.BillId DESC";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Bill
                {
                    BillId = (int)reader["BillId"],
                    BillNo = reader["BillNo"].ToString(),
                    PatientId = (int)reader["PatientId"],
                    VisitId = (int)reader["VisitId"],

                    AppointmentId =
                        reader["AppointmentId"] != DBNull.Value
                            ? (int)reader["AppointmentId"]
                            : null,

                    DoctorId =
                        reader["DoctorId"] != DBNull.Value
                            ? (int)reader["DoctorId"]
                            : null,

                    VisitDate = (DateTime)reader["VisitDate"],
                    BillDate = (DateTime)reader["BillDate"],
                    GrossAmount = (decimal)reader["GrossAmount"],
                    DiscountAmount = (decimal)reader["DiscountAmount"],
                    TotalPaidAmount = (decimal)reader["PaidAmount"]
                });
            }

            return list;
        }

        public List<Bill> GetByVisit(int visitId, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<Bill>();

            string query = @"
                SELECT
                    b.*,
                    v.VisitDate,
                    a.DoctorId
                FROM Bills b
                JOIN Visits v
                    ON b.VisitId = v.VisitId
                JOIN Appointments a
                    ON b.AppointmentId = a.AppointmentId
                WHERE b.VisitId = @VisitId
                AND b.ClinicId = @ClinicId
                ORDER BY b.BillDate DESC, b.BillId DESC";

            using SqlCommand cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@AppointmentId", visitId);
            cmd.Parameters.AddWithValue("@VisitId", visitId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Bill
                {
                    BillId = (int)reader["BillId"],
                    BillNo = reader["BillNo"].ToString(),
                    PatientId = (int)reader["PatientId"],
                    VisitId = (int)reader["VisitId"],

                    AppointmentId =
                        reader["AppointmentId"] != DBNull.Value
                            ? (int)reader["AppointmentId"]
                            : null,

                    DoctorId =
                        reader["DoctorId"] != DBNull.Value
                            ? (int)reader["DoctorId"]
                            : null,

                    VisitDate = (DateTime)reader["VisitDate"],
                    BillDate = (DateTime)reader["BillDate"],
                    GrossAmount = (decimal)reader["GrossAmount"],
                    DiscountAmount = (decimal)reader["DiscountAmount"],
                    TotalPaidAmount = (decimal)reader["PaidAmount"]
                });
            }

            return list;            
        }

        public int GetAppointmentBillCount(int appointmentId, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            const string query = @"
                SELECT COUNT(*)
                FROM Bills
                WHERE AppointmentId = @AppointmentId
                AND ClinicId = @ClinicId";

            using var cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public int GetVisitBillCount(int visitId, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            const string query = @"
                SELECT COUNT(*)
                FROM Bills
                WHERE VisitId = @VisitId
                AND ClinicId = @ClinicId";

            using var cmd = new SqlCommand(query, conn);

            if (tran != null)
                cmd.Transaction = tran;

            cmd.Parameters.AddWithValue("@VisitId", visitId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public List<Bill> GetAll(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<Bill>();

            string query = @"SELECT b.*, v.VisitDate,
                                    a.DoctorId 
                                FROM Bills b 
                                JOIN Visits v 
                                    ON b.VisitId = v.VisitId
                                LEFT JOIN Appointments a
                                    ON b.AppointmentId = a.AppointmentId
                                WHERE b.ClinicId = @ClinicId";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Bill
                {
                    BillId = (int)reader["BillId"],
                    BillNo = reader["BillNo"].ToString(),
                    PatientId = (int)reader["PatientId"],
                    VisitId = (int)reader["VisitId"],
                    AppointmentId = reader["AppointmentId"] != DBNull.Value ? (int)reader["AppointmentId"] : null,
                    DoctorId = reader["DoctorId"] != DBNull.Value ? (int)reader["DoctorId"] : null,
                    VisitDate = (DateTime)reader["VisitDate"],
                    BillDate = (DateTime)reader["BillDate"],
                    GrossAmount = (decimal)reader["GrossAmount"],
                    DiscountAmount = (decimal)reader["DiscountAmount"],
                    TotalPaidAmount = (decimal)reader["PaidAmount"]
                });
            }

            return list;

        }

        public List<Bill> GetAllByClinic(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<Bill>();

            string query = @"SELECT b.*, v.VisitDate, p.FirstName + ' ' + p.LastName as PatientName,
                                        a.DoctorId 
                                FROM Bills b 
                                JOIN Visits v 
                                    ON b.VisitId = v.VisitId
                                JOIN Patients p
                                    ON v.PatientId = p.PatientId 
                                LEFT JOIN Appointments a
                                    ON b.AppointmentId = a.AppointmentId
                                WHERE b.ClinicId = @clinicId
                                ORDER BY b.BillDate DESC";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@clinicId", clinicId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Bill
                {
                    BillId = (int)reader["BillId"],
                    BillNo = reader["BillNo"].ToString(),
                    VisitId = (int)reader["VisitId"],
                    AppointmentId = reader["AppointmentId"] != DBNull.Value ? (int)reader["AppointmentId"] : null,
                    DoctorId = reader["DoctorId"] != DBNull.Value ? (int)reader["DoctorId"] : null,
                    VisitDate = (DateTime)reader["VisitDate"],
                    PatientId = (int)reader["PatientId"],
                    PatientName = reader["PatientName"].ToString(),
                    BillDate = (DateTime)reader["BillDate"],
                    GrossAmount = (decimal)reader["GrossAmount"],
                    DiscountAmount = (decimal)reader["DiscountAmount"],
                    TotalPaidAmount = (decimal)reader["PaidAmount"]
                });
            }

            return list;

        }

        public List<Bill> ListByPatient(int clinicId, int patientId, SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<Bill>();

            string query = @"SELECT b.*, p.FirstName + ' ' + p.LastName as PatientName, v.VisitDate,
                                        a.DoctorId 
                                FROM Bills b
                                JOIN Visits v 
                                    ON b.VisitId = v.VisitId
                                JOIN Patients p 
                                    ON v.PatientId = p.PatientId
                                LEFT JOIN Appointments a
                                    ON b.AppointmentId = a.AppointmentId
                                WHERE p.PatientId = @patientId
                                AND b.ClinicId = @clinicId
                                ORDER BY b.BillDate DESC";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@patientId", patientId);
            cmd.Parameters.AddWithValue("@clinicId", clinicId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Bill
                {
                    BillId = (int)reader["BillId"],
                    BillNo = reader["BillNo"].ToString(),
                    PatientId = (int)reader["PatientId"],
                    VisitId = (int)reader["VisitId"],
                    AppointmentId = reader["AppointmentId"] != DBNull.Value ? (int)reader["AppointmentId"] : null,
                    DoctorId = reader["DoctorId"] != DBNull.Value ? (int)reader["DoctorId"] : null,
                    VisitDate = (DateTime)reader["VisitDate"],
                    BillDate = (DateTime)reader["BillDate"],
                    PatientName = reader["PatientName"].ToString(),
                    GrossAmount = (decimal)reader["GrossAmount"],
                    DiscountAmount = (decimal)reader["DiscountAmount"],
                    TotalPaidAmount = (decimal)reader["PaidAmount"]
                });
            }

            return list;
        }

        public List<Bill> ListRefundableBillsByPatient(int clinicId, int patientId, int? selectedBillId,
                                                            SqlConnection conn, SqlTransaction? tran = null)
        {
            var list = new List<Bill>();

            string query;

            if (selectedBillId.HasValue && selectedBillId.Value > 0)
            {
                query = @"SELECT b.*, p.FirstName + ' ' + p.LastName as PatientName, v.VisitDate, a.DoctorId 
                                FROM Bills b
                                JOIN Visits v 
                                    ON b.VisitId = v.VisitId
                                JOIN Patients p 
                                    ON v.PatientId = p.PatientId
                                LEFT JOIN Appointments a
                                    ON b.AppointmentId = a.AppointmentId
                                WHERE p.PatientId = @patientId
                                AND b.ClinicId = @clinicId
                                AND EXISTS (SELECT 1
                                            FROM BillLines bl
                                            WHERE bl.BillId = b.BillId
                                            AND (bl.AvailableQuantity > 0
                                            OR bl.BillId = @SelectedBillId))
                                ORDER BY b.BillDate DESC";
            }
            else
            {
                query = @"SELECT b.*, p.FirstName + ' ' + p.LastName as PatientName, v.VisitDate, a.DoctorId 
                                FROM Bills b
                                JOIN Visits v 
                                    ON b.VisitId = v.VisitId
                                JOIN Patients p 
                                    ON v.PatientId = p.PatientId
                                LEFT JOIN Appointments a
                                    ON b.AppointmentId = a.AppointmentId
                                WHERE p.PatientId = @patientId
                                AND b.ClinicId = @clinicId
                                AND EXISTS (SELECT 1
                                            FROM BillLines bl
                                            WHERE bl.BillId = b.BillId
                                            AND bl.AvailableQuantity > 0)
                                ORDER BY b.BillDate DESC";                
            }

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@patientId", patientId);
            cmd.Parameters.AddWithValue("@clinicId", clinicId);
            if (selectedBillId.HasValue && selectedBillId.Value > 0)
            {
                cmd.Parameters.AddWithValue("@SelectedBillId", selectedBillId.Value);
            }

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new Bill
                {
                    BillId = (int)reader["BillId"],
                    BillNo = reader["BillNo"].ToString(),
                    PatientId = (int)reader["PatientId"],
                    VisitId = (int)reader["VisitId"],
                    AppointmentId = reader["AppointmentId"] != DBNull.Value ? (int)reader["AppointmentId"] : null,
                    DoctorId = reader["DoctorId"] != DBNull.Value ? (int)reader["DoctorId"] : null,
                    VisitDate = (DateTime)reader["VisitDate"],
                    BillDate = (DateTime)reader["BillDate"],
                    PatientName = reader["PatientName"].ToString(),
                    GrossAmount = (decimal)reader["GrossAmount"],
                    DiscountAmount = (decimal)reader["DiscountAmount"],
                    TotalPaidAmount = (decimal)reader["PaidAmount"]
                });
            }

            return list;
        }



    }

}
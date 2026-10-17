using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;
using ViewModels;

namespace DAL
{
    public class DoctorDAL : BaseDAL, IDAL<Doctor>
    {
        private readonly DbHelper _db;

        public DoctorDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(Doctor model, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"INSERT INTO Doctors 
                            (ClinicId, DepartmentId, Salutation, FirstName, LastName, RegistrationNo, Qualification, Fee, UserId)
                            VALUES
                            (@ClinicId, @DepartmentId, @Salutation, @FirstName, @LastName, @RegNo, @Qualification, @Fee, @UserId);
                            SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@ClinicId", model.ClinicId);
            cmd.Parameters.AddWithValue("@DepartmentId", model.DepartmentId);
            cmd.Parameters.AddWithValue("@Salutation", model.Salutation);
            cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
            cmd.Parameters.AddWithValue("@LastName", model.LastName ?? "");
            cmd.Parameters.AddWithValue("@RegNo", model.RegistrationNo ?? "");
            cmd.Parameters.AddWithValue("@Qualification", model.Qualification ?? "");
            cmd.Parameters.AddWithValue("@Fee", model.Fee);
            cmd.Parameters.AddWithValue("@UserId", model.UserId);
    
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool Update(Doctor model, SqlConnection conn, SqlTransaction tran)
        {
            int rowsAffected_doctor = 0;

            string query = @"UPDATE Doctors
                            SET DepartmentId = @DepartmentId,
                                Salutation = @Salutation,
                                FirstName = @FirstName,
                                LastName = @LastName,
                                RegistrationNo = @RegNo,
                                Qualification = @Qualification,
                                Fee = @Fee
                            WHERE DoctorId = @DoctorId
                            AND ClinicId = @ClinicId;";

            SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@DepartmentId", model.DepartmentId);
            cmd.Parameters.AddWithValue("@Salutation", model.Salutation);
            cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
            cmd.Parameters.AddWithValue("@LastName", model.LastName ?? "");
            cmd.Parameters.AddWithValue("@RegNo", model.RegistrationNo ?? "");
            cmd.Parameters.AddWithValue("@Qualification", model.Qualification ?? "");
            cmd.Parameters.AddWithValue("@Fee", model.Fee);
            cmd.Parameters.AddWithValue("@DoctorId", model.DoctorId);
            cmd.Parameters.AddWithValue("@ClinicId", model.ClinicId);

            rowsAffected_doctor = cmd.ExecuteNonQuery();

            return rowsAffected_doctor > 0;
        }

        public bool Delete(int id, int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            int rowsAffected_doctor = 0;
            var doctor = Get(id, clinicId, conn, tran);
            
            if (doctor == null)
                throw new InvalidOperationException($"Doctor details could not be retrieved for doctor {id}.");

            int userId = doctor.UserId;

            string query = @"DELETE FROM Doctors 
                                WHERE DoctorId = @DoctorId
                                AND ClinicId = @ClinicId;";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@DoctorId", id);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            rowsAffected_doctor = cmd.ExecuteNonQuery();

            int rowsAffected_user = 0;

            string query2 = @"DELETE FROM Users 
                                WHERE UserId = @UserId
                                AND ClinicId = @ClinicId;";

            SqlCommand cmd2 = new SqlCommand(query2, conn, tran);
            cmd2.Parameters.AddWithValue("@UserId", userId);
            cmd2.Parameters.AddWithValue("@ClinicId", clinicId);

            rowsAffected_user = cmd2.ExecuteNonQuery();

            return rowsAffected_doctor > 0 && rowsAffected_user > 0;
        }

        public List<Doctor> GetAll(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<Doctor> list = new List<Doctor>();

            string query = @"SELECT d.*, d.UserId as DoctorUserId,
                            u.*, 
                            dept.DepartmentCode, dept.Name AS DepartmentName, 
                            c.Name AS ClinicName,
                            t.TemplateId as PrescriptionTemplateId,
                            t.TemplateName as PrescriptionTemplateName,
                            t.SourceTemplateId as SourceTemplateId 
                            FROM Doctors d
                            INNER JOIN Departments dept 
                                ON d.DepartmentId = dept.DepartmentId
                            INNER JOIN Clinics c 
                                ON d.ClinicId = c.ClinicId
                            INNER JOIN Users u 
                                ON d.UserId = u.UserId
                            LEFT JOIN PrescriptionTemplates t 
                                ON (d.DoctorId = t.DoctorId
                                AND d.ClinicId = t.ClinicId)
                            WHERE d.ClinicId = @ClinicId;";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(mapDoctor(dr));
            }

            return list;
        }

        public Doctor Get(int doctorId, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = @"SELECT d.*, d.UserId as DoctorUserId, 
                            u.*, 
                            dept.DepartmentCode, dept.Name AS DepartmentName, 
                            c.Name AS ClinicName,
                            t.TemplateId as PrescriptionTemplateId,
                            t.TemplateName as PrescriptionTemplateName,
                            t.SourceTemplateId as SourceTemplateId 
                            FROM Doctors d
                            INNER JOIN Departments dept 
                                ON d.DepartmentId = dept.DepartmentId
                            INNER JOIN Clinics c 
                                ON d.ClinicId = c.ClinicId
                            INNER JOIN Users u 
                                ON d.UserId = u.UserId
                            LEFT JOIN PrescriptionTemplates t 
                                ON (d.DoctorId = t.DoctorId
                                AND d.ClinicId = t.ClinicId)
                            WHERE d.DoctorId = @DoctorId
                            AND d.ClinicId = @ClinicId";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@DoctorId", doctorId);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                return mapDoctor(dr);
            }

            return null; // Not found
        }

        public List<Doctor> GetDoctorsByClinic(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<Doctor> list = new List<Doctor>();

            string query = @"SELECT d.*, d.UserId as DoctorUserId, 
                            u.*, 
                            dept.DepartmentCode, dept.Name AS DepartmentName, 
                            c.Name AS ClinicName,
                            t.TemplateId as PrescriptionTemplateId,
                            t.TemplateName as PrescriptionTemplateName,
                            t.SourceTemplateId as SourceTemplateId 
                            FROM Doctors d
                            INNER JOIN Departments dept 
                                ON d.DepartmentId = dept.DepartmentId
                            INNER JOIN Clinics c 
                                ON d.ClinicId = c.ClinicId
                            INNER JOIN Users u 
                                ON d.UserId = u.UserId                          
                            LEFT JOIN PrescriptionTemplates t 
                                ON (d.DoctorId = t.DoctorId
                                AND d.ClinicId = t.ClinicId)
                            WHERE d.ClinicId = @ClinicId";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(mapDoctor(dr));
            }

            return list;
        }

        public List<Doctor> GetDoctorsByDepartment(int clinicId, int departmentId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<Doctor> list = new List<Doctor>();

            string query = @"SELECT d.*, d.UserId as DoctorUserId, 
                            u.*, 
                            dept.DepartmentCode, dept.Name AS DepartmentName, 
                            c.Name AS ClinicName,
                            t.TemplateId as PrescriptionTemplateId,
                            t.TemplateName as PrescriptionTemplateName,
                            t.SourceTemplateId as SourceTemplateId 
                            FROM Doctors d
                            INNER JOIN Departments dept ON d.DepartmentId = dept.DepartmentId
                            INNER JOIN Clinics c 
                                ON d.ClinicId = c.ClinicId
                            INNER JOIN Users u 
                                ON d.UserId = u.UserId  
                            LEFT JOIN PrescriptionTemplates t 
                                ON (d.DoctorId = t.DoctorId
                                AND d.ClinicId = t.ClinicId)
                            WHERE d.ClinicId = @ClinicId 
                            AND d.DepartmentId = @DepartmentId";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);
            cmd.Parameters.AddWithValue("@DepartmentId", departmentId);

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(mapDoctor(dr));
            }

            return list;
        }

        public int GetDoctorIdByUserId(int userId, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = "SELECT DoctorId FROM Doctors WHERE UserId=@UserId";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@UserId", userId);

            object result = cmd.ExecuteScalar();

            return result != null ? Convert.ToInt32(result) : 0;
        }

        public int GetUserId(int doctorId, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = "SELECT UserId FROM Doctors WHERE DoctorId=@DoctorId";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@DoctorId", doctorId);

            object result = cmd.ExecuteScalar();

            return result != null ? Convert.ToInt32(result) : 0;
        }

        private Doctor mapDoctor(SqlDataReader reader)
        {
            return new Doctor
            {
                DoctorId = Convert.ToInt32(reader["DoctorId"]),
                ClinicId = Convert.ToInt32(reader["ClinicId"]),
                UserId = Convert.ToInt32(reader["DoctorUserId"]),
                DepartmentId = Convert.ToInt32(reader["DepartmentId"]),
                Salutation = reader["Salutation"].ToString(),
                FirstName = reader["FirstName"].ToString(),
                LastName = reader["LastName"].ToString(),
                RegistrationNo = reader["RegistrationNo"].ToString(),
                Qualification = reader["Qualification"].ToString(),
                DepartmentCode = GetNullableString(reader, "DepartmentCode"),
                DepartmentName = GetNullableString(reader, "DepartmentName"),
                ClinicName = GetNullableString(reader, "ClinicName"),
                Username = reader["Username"].ToString(),
                Password = reader["PasswordHash"].ToString(),
                Email = GetNullableString(reader, "Email"),
                Phone = GetNullableString(reader, "Phone"),
                Fee = Convert.ToDecimal(reader["Fee"]),
                PrescriptionTemplateId = GetNullableInt(reader, "PrescriptionTemplateId"),
                PrescriptionTemplateName = GetNullableString(reader, "PrescriptionTemplateName"),
                SourceTemplateId = GetNullableInt(reader, "SourceTemplateId")                                                                 
            };
        }

    }
}
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;
using ViewModels;

namespace DAL
{
    public class DoctorDAL : IDAL<Doctor>
    {
        private readonly DbHelper _db;

        public DoctorDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(Doctor model, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"INSERT INTO Doctors 
                            (ClinicId, DepartmentId, Salutation, FirstName, LastName, RegistrationNo, Qualification, UserId)
                            VALUES
                            (@ClinicId, @DepartmentId, @Salutation, @FirstName, @LastName, @RegNo, @Qualification, @UserId);
                            SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@ClinicId", model.ClinicId);
            cmd.Parameters.AddWithValue("@DepartmentId", model.DepartmentId);
            cmd.Parameters.AddWithValue("@Salutation", model.Salutation);
            cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
            cmd.Parameters.AddWithValue("@LastName", model.LastName ?? "");
            cmd.Parameters.AddWithValue("@RegNo", model.RegistrationNo ?? "");
            cmd.Parameters.AddWithValue("@Qualification", model.Qualification ?? "");
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
                                Qualification = @Qualification
                            WHERE DoctorId = @DoctorId";

            SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@DepartmentId", model.DepartmentId);
            cmd.Parameters.AddWithValue("@Salutation", model.Salutation);
            cmd.Parameters.AddWithValue("@FirstName", model.FirstName);
            cmd.Parameters.AddWithValue("@LastName", model.LastName ?? "");
            cmd.Parameters.AddWithValue("@RegNo", model.RegistrationNo ?? "");
            cmd.Parameters.AddWithValue("@Qualification", model.Qualification ?? "");
            cmd.Parameters.AddWithValue("@DoctorId", model.DoctorId);

            rowsAffected_doctor = cmd.ExecuteNonQuery();

            // Also update the associated User record's name fields
 /*           int rowsAffected_user   = 0;

            string query2 = @"UPDATE Users
                            SET FirstName = @FirstName,
                                LastName = @LastName,
                                Username = @Username,
                                PasswordHash = @PasswordHash,
                                Email = @Email,
                                Phone = @Phone
                            WHERE UserId = @UserId";

            SqlCommand cmd2 = new SqlCommand(query2, conn, tran);
            cmd2.Parameters.AddWithValue("@FirstName", model.FirstName);
            cmd2.Parameters.AddWithValue("@LastName", model.LastName ?? "");
            cmd2.Parameters.AddWithValue("@Username", model.Username);
            cmd2.Parameters.AddWithValue("@PasswordHash", model.Password);
            cmd2.Parameters.AddWithValue("@UserId", model.UserId);
            cmd2.Parameters.AddWithValue("@Email", model.Email ?? "");
            cmd2.Parameters.AddWithValue("@Phone", model.Phone ?? "");

            rowsAffected_user = cmd2.ExecuteNonQuery(); */

            return rowsAffected_doctor > 0 /*&& rowsAffected_user > 0 */;
        }

        public bool Delete(int id, int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            int rowsAffected_doctor = 0;
            int userId = Get(id, clinicId, conn, tran).UserId;

            string query = @"DELETE FROM Doctors WHERE DoctorId = @DoctorId";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@DoctorId", id);

            rowsAffected_doctor = cmd.ExecuteNonQuery();

            int rowsAffected_user = 0;

            string query2 = @"DELETE FROM Users WHERE UserId = @UserId";

            SqlCommand cmd2 = new SqlCommand(query2, conn, tran);
            cmd2.Parameters.AddWithValue("@UserId", userId);

            rowsAffected_user = cmd2.ExecuteNonQuery();

            return rowsAffected_doctor > 0 && rowsAffected_user > 0;
        }

        public List<Doctor> GetAll(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<Doctor> list = new List<Doctor>();

            string query = @"SELECT d.*, u.*, dept.Name AS DepartmentName, c.Name AS ClinicName
                            FROM Doctors d
                            INNER JOIN Departments dept ON d.DepartmentId = dept.DepartmentId
                            INNER JOIN Clinics c ON d.ClinicId = c.ClinicId
                            INNER JOIN Users u ON d.UserId = u.UserId";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            using SqlDataReader dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new Doctor
                {
                    DoctorId = Convert.ToInt32(dr["DoctorId"]),
                    ClinicId = Convert.ToInt32(dr["ClinicId"]),
                    UserId = Convert.ToInt32(dr["UserId"]),
                    DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
                    Salutation = dr["Salutation"].ToString(),
                    FirstName = dr["FirstName"].ToString(),
                    LastName = dr["LastName"].ToString(),
                    RegistrationNo = dr["RegistrationNo"].ToString(),
                    Qualification = dr["Qualification"].ToString(),
                    DepartmentName = dr["DepartmentName"].ToString(),
                    ClinicName = dr["ClinicName"].ToString(),
                    Username = dr["Username"].ToString(),
                    Password = dr["PasswordHash"].ToString(),
                    Email = dr["Email"].ToString(),
                    Phone = dr["Phone"].ToString()
                });
            }

            return list;
        }

        public Doctor Get(int doctorId, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = @"SELECT d.*, d.UserId as DoctorUserId, u.*, dept.Name AS DepartmentName, c.Name AS ClinicName
                            FROM Doctors d
                            INNER JOIN Departments dept ON d.DepartmentId = dept.DepartmentId
                            INNER JOIN Clinics c ON d.ClinicId = c.ClinicId
                            INNER JOIN Users u ON d.UserId = u.UserId
                            WHERE d.DoctorId = @DoctorId";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@DoctorId", doctorId);

            using SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                return new Doctor
                {
                    DoctorId = Convert.ToInt32(dr["DoctorId"]),
                    ClinicId = Convert.ToInt32(dr["ClinicId"]),
                    UserId = Convert.ToInt32(dr["DoctorUserId"]),
                    DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
                    Salutation = dr["Salutation"].ToString(),
                    FirstName = dr["FirstName"].ToString(),
                    LastName = dr["LastName"].ToString(),
                    RegistrationNo = dr["RegistrationNo"].ToString(),
                    Qualification = dr["Qualification"].ToString(),
                    DepartmentName = dr["DepartmentName"].ToString(),
                    ClinicName = dr["ClinicName"].ToString(),
                    Username = dr["Username"].ToString(),
                    Password = dr["PasswordHash"].ToString(),
                    Email = dr["Email"].ToString(),
                    Phone = dr["Phone"].ToString()
                };
            }

            return null; // Not found
        }

        public List<Doctor> GetDoctorsByClinic(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<Doctor> list = new List<Doctor>();

            string query = @"SELECT d.*, d.UserId as DoctorUserId, u.*, dept.Name AS DepartmentName, c.Name AS ClinicName
                            FROM Doctors d
                            INNER JOIN Departments dept ON d.DepartmentId = dept.DepartmentId
                            INNER JOIN Clinics c ON d.ClinicId = c.ClinicId
                            INNER JOIN Users u ON d.UserId = u.UserId
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
                list.Add(new Doctor
                {
                    DoctorId = Convert.ToInt32(dr["DoctorId"]),
                    ClinicId = Convert.ToInt32(dr["ClinicId"]),
                    UserId = Convert.ToInt32(dr["DoctorUserId"]),
                    Salutation = dr["Salutation"].ToString(),
                    FirstName = dr["FirstName"].ToString(),
                    LastName = dr["LastName"].ToString(),
                    DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
                    DepartmentName = dr["DepartmentName"].ToString(),
                    ClinicName = dr["ClinicName"].ToString(),
                    Username = dr["Username"].ToString(),
                    Password = dr["PasswordHash"].ToString(),
                    Email = dr["Email"].ToString(),
                    Phone = dr["Phone"].ToString()
                });
            }

            return list;
        }

        public List<Doctor> GetDoctorsByDepartment(int clinicId, int departmentId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<Doctor> list = new List<Doctor>();

            string query = @"SELECT d.*, d.UserId as DoctorUserId, u.*, dept.Name AS DepartmentName, c.Name AS ClinicName
                            FROM Doctors d
                            INNER JOIN Departments dept ON d.DepartmentId = dept.DepartmentId
                            INNER JOIN Clinics c ON d.ClinicId = c.ClinicId
                            INNER JOIN Users u ON d.UserId = u.UserId
                            WHERE d.ClinicId = @ClinicId AND d.DepartmentId = @DepartmentId";

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
                list.Add(new Doctor
                {
                    DoctorId = Convert.ToInt32(dr["DoctorId"]),
                    ClinicId = Convert.ToInt32(dr["ClinicId"]),
                    Salutation = dr["Salutation"].ToString(),
                    FirstName = dr["FirstName"].ToString(),
                    LastName = dr["LastName"].ToString(),
                    DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
                    DepartmentName = dr["DepartmentName"].ToString(),
                    ClinicName = dr["ClinicName"].ToString(),
                    Username = dr["Username"].ToString(),
                    Password = dr["PasswordHash"].ToString(),
                    Email = dr["Email"].ToString(),
                    Phone = dr["Phone"].ToString()
                });
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

    }
}
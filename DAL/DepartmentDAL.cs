using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;
using Helpers;

namespace DAL
{
    public class DepartmentDAL : BaseDAL, IDAL<Department>
    {
        private readonly DbHelper _db;

        public DepartmentDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(Department model, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"INSERT INTO Departments 
                            (DepartmentCode, ClinicId, Name, Description)
                            VALUES 
                            (@DepartmentCode, @ClinicId, @Name, @Description);
                            SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@DepartmentCode", model.DepartmentCode);
            cmd.Parameters.AddWithValue("@ClinicId", model.ClinicId);
            cmd.Parameters.AddWithValue("@Name", model.Name);
            cmd.Parameters.AddWithValue("@Description", model.Description ?? "");

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool Update(Department model, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"UPDATE Departments
                             SET DepartmentCode = @DepartmentCode,
                                 Name = @Name,
                                 Description = @Description
                             WHERE DepartmentId = @DepartmentId
                             AND ClinicId = @ClinicId;";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@DepartmentCode", model.DepartmentCode);
            cmd.Parameters.AddWithValue("@Name", model.Name);
            cmd.Parameters.AddWithValue("@Description", model.Description ?? "");
            cmd.Parameters.AddWithValue("@DepartmentId", model.DepartmentId);
            cmd.Parameters.AddWithValue("@ClinicId", model.ClinicId);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public bool Delete(int id, int clinicId, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"DELETE FROM Departments 
                                WHERE DepartmentId = @DepartmentId
                                AND ClinicId = @ClinicId;";

            SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@DepartmentId", id);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public List<Department> GetAll(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<Department> list = new List<Department>();

            string query = @"SELECT d.*, 
                                    c.Name as ClinicName,
                                    t.TemplateId as PrescriptionTemplateId,
                                    t.TemplateName as PrescriptionTemplateName,
                                    t.SourceTemplateId as SourceTemplateId 
                                FROM Departments d 
                                LEFT JOIN Clinics c 
                                    ON d.ClinicId = c.ClinicId 
                                LEFT JOIN PrescriptionTemplates t 
                                    ON (d.DepartmentId = t.DepartmentId
                                    AND d.ClinicId = t.ClinicId
                                    AND t.DoctorId IS NULL) 
                            WHERE d.ClinicId = @ClinicId";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using SqlDataReader dept = cmd.ExecuteReader();

            while (dept.Read())
            {
                list.Add(mapDepartment(dept));
            }

            return list;
        }

        public Department Get(int id, int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = @"SELECT d.*, 
                                    c.Name as ClinicName,
                                    t.TemplateId as PrescriptionTemplateId, 
                                    t.TemplateName as PrescriptionTemplateName,
                                    t.SourceTemplateId as SourceTemplateId 
                            FROM Departments d 
                            LEFT JOIN Clinics c
                                ON d.ClinicId = c.ClinicId 
                            LEFT JOIN PrescriptionTemplates t 
                                ON (d.DepartmentId = t.DepartmentId
                                AND d.ClinicId = t.ClinicId
                                AND t.DoctorId IS NULL) 
                            WHERE d.DepartmentId = @DepartmentId
                            AND d.ClinicId = @ClinicId";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@DepartmentId", id);
            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using SqlDataReader dept = cmd.ExecuteReader();

            if (dept.Read())
            {
                return mapDepartment(dept);
            }

            return null; // Not found
        }

        public List<Department> GetDepartmentsByClinic(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<Department> list = new List<Department>();

            string query = @"SELECT d.*, 
                                    c.Name as ClinicName,
                                    t.TemplateId as PrescriptionTemplateId,
                                    t.TemplateName as PrescriptionTemplateName,
                                    t.SourceTemplateId as SourceTemplateId
                            FROM Departments d 
                            LEFT JOIN Clinics c 
                                ON d.ClinicId = c.ClinicId 
                            LEFT JOIN PrescriptionTemplates t 
                                ON (d.DepartmentId = t.DepartmentId
                                AND d.ClinicId = t.ClinicId
                                AND t.DoctorId IS NULL) 
                            WHERE d.ClinicId = @ClinicId";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            using SqlDataReader dept = cmd.ExecuteReader();

            while (dept.Read())
            {
                list.Add(mapDepartment(dept));
            }

            return list;
        }

        public int CountByCode(int clinicId, int? departmentId, string departmentCode, SqlConnection conn, SqlTransaction? tran = null)
        {
            int count = 0;

            string query = "";

            if (departmentId.HasValue)
            {
                query = @"SELECT COUNT(*) AS DepartmentCodeCount
                            FROM Departments 
                            WHERE ClinicId = @ClinicId
                            AND DepartmentCode = @DepartmentCode
                            AND DepartmentId <> @DepartmentId";         
            }
            else
            {
                query = @"SELECT COUNT(*) AS DepartmentCodeCount
                            FROM Departments 
                            WHERE ClinicId = @ClinicId
                            AND DepartmentCode = @DepartmentCode";
            }
            
            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);
            cmd.Parameters.AddWithValue("@DepartmentCode", departmentCode);

            if (departmentId.HasValue)
                cmd.Parameters.AddWithValue("@DepartmentId", departmentId.Value);

            using SqlDataReader dept = cmd.ExecuteReader();

            if (dept.Read())
            {
                count = Convert.ToInt32(dept["DepartmentCodeCount"]);
            }

            return count;
        }

        private Department mapDepartment(SqlDataReader reader)
        {
            return new Department
            {
                DepartmentId = Convert.ToInt32(reader["DepartmentId"]),
                ClinicId = Convert.ToInt32(reader["ClinicId"]),
                DepartmentCode = reader["DepartmentCode"].ToString(),
                Name = reader["Name"].ToString(),
                Description = reader["Description"].ToString(),
                ClinicName = reader["ClinicName"].ToString(),
                PrescriptionTemplateId = GetNullableInt(reader, "PrescriptionTemplateId"),
                PrescriptionTemplateName = GetNullableString(reader, "PrescriptionTemplateName"),
                SourceTemplateId = GetNullableInt(reader, "SourceTemplateId")                                                                 
            };
        }
    }
}
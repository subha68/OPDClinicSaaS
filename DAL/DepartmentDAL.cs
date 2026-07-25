using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;
using Helpers;

namespace DAL
{
    public class DepartmentDAL : IDAL<Department>
    {
        private readonly DbHelper _db;

        public DepartmentDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(Department model, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"INSERT INTO Departments (ClinicId, Name, Description)
                                VALUES (@ClinicId, @Name, @Description);
                                SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@ClinicId", model.ClinicId);
            cmd.Parameters.AddWithValue("@Name", model.Name);
            cmd.Parameters.AddWithValue("@Description", model.Description ?? "");

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool Update(Department model, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"UPDATE Departments
                             SET Name = @Name,
                                 Description = @Description
                             WHERE DepartmentId = @DepartmentId";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@Name", model.Name);
            cmd.Parameters.AddWithValue("@Description", model.Description ?? "");
            cmd.Parameters.AddWithValue("@DepartmentId", model.DepartmentId);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public bool Delete(int id, SqlConnection conn, SqlTransaction tran)
        {
            string query = "DELETE FROM Departments WHERE DepartmentId = @DepartmentId";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@DepartmentId", id);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public List<Department> GetAll(SqlConnection conn, SqlTransaction? tran = null)
        {
            List<Department> list = new List<Department>();

            string query = @"SELECT d.*, 
                                    c.Name as ClinicName,
                                    t.TemplateId as PrescriptionTemplateId,
                                    t.TemplateName as PrescriptionTemplateName 
                                FROM Departments d 
                                LEFT JOIN Clinics c 
                                    ON d.ClinicId = c.ClinicId 
                                LEFT JOIN PrescriptionTemplates t 
                                    ON (d.DepartmentId = t.DepartmentId
                                    AND d.ClinicId = t.ClinicId) 
                            WHERE d.DepartmentId = @DepartmentId";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            using SqlDataReader dept = cmd.ExecuteReader();

            while (dept.Read())
            {
                list.Add(new Department
                {
                    DepartmentId = Convert.ToInt32(dept["DepartmentId"]),
                    ClinicId = Convert.ToInt32(dept["ClinicId"]),
                    Name = dept["Name"].ToString(),
                    Description = dept["Description"].ToString(),
                    ClinicName = dept["ClinicName"].ToString(),
                    PrescriptionTemplateId = DbConvert.ToInt(dept["PrescriptionTemplateId"]),
                    PrescriptionTemplateName = DbConvert.ToString(dept["PrescriptionTemplateName"])
                });
            }

            return list;
        }

        public Department Get(int id, SqlConnection conn, SqlTransaction? tran = null)
        {
            string query = @"SELECT d.*, 
                                    c.Name as ClinicName,
                                    t.TemplateId as PrescriptionTemplateId, 
                                    t.TemplateName as PrescriptionTemplateName 
                            FROM Departments d 
                            LEFT JOIN Clinics c
                                ON d.ClinicId = c.ClinicId 
                            LEFT JOIN PrescriptionTemplates t 
                                ON (d.DepartmentId = t.DepartmentId
                                AND d.ClinicId = t.ClinicId) 
                            WHERE d.DepartmentId = @DepartmentId";

            SqlCommand cmd = new SqlCommand(query, conn);
            if (tran != null)
            {
                cmd.Transaction = tran;
            }

            cmd.Parameters.AddWithValue("@DepartmentId", id);

            using SqlDataReader dept = cmd.ExecuteReader();

            if (dept.Read())
            {
                return new Department
                {
                    DepartmentId = Convert.ToInt32(dept["DepartmentId"]),
                    ClinicId = Convert.ToInt32(dept["ClinicId"]),
                    Name = dept["Name"].ToString(),
                    Description = dept["Description"].ToString(),
                    ClinicName = dept["ClinicName"].ToString(),
                    PrescriptionTemplateId = DbConvert.ToInt(dept["PrescriptionTemplateId"]),
                    PrescriptionTemplateName = DbConvert.ToString(dept["PrescriptionTemplateName"])
                };
            }

            return null; // Not found
        }

        public List<Department> GetDepartmentsByClinic(int clinicId, SqlConnection conn, SqlTransaction? tran = null)
        {
            List<Department> list = new List<Department>();

            string query = @"SELECT d.*, 
                                    c.Name as ClinicName,
                                    t.TemplateId as PrescriptionTemplateId,
                                    t.TemplateName as PrescriptionTemplateName
                            FROM Departments d 
                            LEFT JOIN Clinics c 
                                ON d.ClinicId = c.ClinicId 
                            LEFT JOIN PrescriptionTemplates t 
                                ON (d.DepartmentId = t.DepartmentId
                                AND d.ClinicId = t.ClinicId) 
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
                list.Add(new Department
                {
                    DepartmentId = Convert.ToInt32(dept["DepartmentId"]),
                    ClinicId = Convert.ToInt32(dept["ClinicId"]),
                    Name = dept["Name"].ToString(),
                    Description = dept["Description"].ToString(),
                    ClinicName = dept["ClinicName"].ToString(),
                    PrescriptionTemplateId = DbConvert.ToInt(dept["PrescriptionTemplateId"]),
                    PrescriptionTemplateName = DbConvert.ToString(dept["PrescriptionTemplateName"])
                });
            }

            return list;
        }

    }
}
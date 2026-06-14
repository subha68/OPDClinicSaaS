using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models;
using ViewModels;

namespace DAL
{
    public class PatientDAL : IDAL<Patient>
    {
        private readonly DbHelper _db;

        public PatientDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(Patient patient, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"INSERT INTO Patients (FirstName, LastName, Phone, DOB, Gender, ClinicId,
                                                Address1, Address2, City, State, PinCode, IdType, IdNumber,
                                                EmergencyContactName, EmergencyRelationship, EmergencyPhone)
                                VALUES (@first, @last, @phone, @dob, @gender, @clinicId,
                                        @Address1, @Address2, @City, @State, @PinCode, @IdType, @IdNumber,
                                                @EmergencyContactName, @EmergencyRelationship, @EmergencyPhone);
                            SELECT SCOPE_IDENTITY();";

            SqlCommand cmd = new SqlCommand(query, conn, tran);

            cmd.Parameters.AddWithValue("@first", string.IsNullOrWhiteSpace(patient.FirstName) ? DBNull.Value : (object)patient.FirstName);
            cmd.Parameters.AddWithValue("@last", string.IsNullOrWhiteSpace(patient.LastName) ? DBNull.Value : (object)patient.LastName);
            cmd.Parameters.AddWithValue("@phone", string.IsNullOrWhiteSpace(patient.Phone) ? DBNull.Value : (object)patient.Phone);
            cmd.Parameters.AddWithValue("@dob", patient.DOB == DateTime.MinValue ? DBNull.Value : patient.DOB);
            cmd.Parameters.AddWithValue("@gender", string.IsNullOrWhiteSpace(patient.Gender) ? DBNull.Value : (object)patient.Gender);
            cmd.Parameters.AddWithValue("@address1", string.IsNullOrWhiteSpace(patient.Address1) ? DBNull.Value : (object)patient.Address1);
            cmd.Parameters.AddWithValue("@address2", string.IsNullOrWhiteSpace(patient.Address2) ? DBNull.Value : (object)patient.Address2);
            cmd.Parameters.AddWithValue("@City", string.IsNullOrWhiteSpace(patient.City) ? DBNull.Value : (object)patient.City);
            cmd.Parameters.AddWithValue("@State", string.IsNullOrWhiteSpace(patient.State) ? DBNull.Value : (object)patient.State);
            cmd.Parameters.AddWithValue("@PinCode", string.IsNullOrWhiteSpace(patient.PinCode) ? DBNull.Value : (object)patient.PinCode);
            cmd.Parameters.AddWithValue("@IdType", string.IsNullOrWhiteSpace(patient.IdType) ? DBNull.Value : (object)patient.IdType);
            cmd.Parameters.AddWithValue("@IdNumber", string.IsNullOrWhiteSpace(patient.IdNumber) ? DBNull.Value : (object)patient.IdNumber);
            cmd.Parameters.AddWithValue("@EmergencyContactName", string.IsNullOrWhiteSpace(patient.EmergencyContactName) ? DBNull.Value : (object)patient.EmergencyContactName);
            cmd.Parameters.AddWithValue("@EmergencyRelationship", string.IsNullOrWhiteSpace(patient.EmergencyRelationship) ? DBNull.Value : (object)patient.EmergencyRelationship);
            cmd.Parameters.AddWithValue("@EmergencyPhone", string.IsNullOrWhiteSpace(patient.EmergencyPhone) ? DBNull.Value : (object)patient.EmergencyPhone);

            cmd.Parameters.AddWithValue("@clinicId", patient.ClinicId);

            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public bool Update(Patient patient, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"UPDATE Patients
                                SET FirstName = @first,
                                    LastName = @last,
                                    Phone = @phone,
                                    DOB = @dob,
                                    Gender = @gender,
                                    Address1 = @address1,
                                    Address2 = @Address2, 
                                    City = @City, 
                                    State = @State, 
                                    PinCode = @PinCode, 
                                    IdType = @IdType, 
                                    IdNumber = @IdNumber,
                                    EmergencyContactName = @EmergencyContactName, 
                                    EmergencyRelationship = @EmergencyRelationship, 
                                    EmergencyPhone = @EmergencyPhone
                                WHERE PatientId = @id";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@first", string.IsNullOrWhiteSpace(patient.FirstName) ? DBNull.Value : (object)patient.FirstName);
            cmd.Parameters.AddWithValue("@last", string.IsNullOrWhiteSpace(patient.LastName) ? DBNull.Value : (object)patient.LastName);
            cmd.Parameters.AddWithValue("@phone", string.IsNullOrWhiteSpace(patient.Phone) ? DBNull.Value : (object)patient.Phone);
            cmd.Parameters.AddWithValue("@dob", patient.DOB == DateTime.MinValue ? DBNull.Value : patient.DOB);
            cmd.Parameters.AddWithValue("@gender", string.IsNullOrWhiteSpace(patient.Gender) ? DBNull.Value : (object)patient.Gender);
            cmd.Parameters.AddWithValue("@address1", string.IsNullOrWhiteSpace(patient.Address1) ? DBNull.Value : (object)patient.Address1);
            cmd.Parameters.AddWithValue("@address2", string.IsNullOrWhiteSpace(patient.Address2) ? DBNull.Value : (object)patient.Address2);
            cmd.Parameters.AddWithValue("@City", string.IsNullOrWhiteSpace(patient.City) ? DBNull.Value : (object)patient.City);
            cmd.Parameters.AddWithValue("@State", string.IsNullOrWhiteSpace(patient.State) ? DBNull.Value : (object)patient.State);
            cmd.Parameters.AddWithValue("@PinCode", string.IsNullOrWhiteSpace(patient.PinCode) ? DBNull.Value : (object)patient.PinCode);
            cmd.Parameters.AddWithValue("@IdType", string.IsNullOrWhiteSpace(patient.IdType) ? DBNull.Value : (object)patient.IdType);
            cmd.Parameters.AddWithValue("@IdNumber", string.IsNullOrWhiteSpace(patient.IdNumber) ? DBNull.Value : (object)patient.IdNumber);
            cmd.Parameters.AddWithValue("@EmergencyContactName", string.IsNullOrWhiteSpace(patient.EmergencyContactName) ? DBNull.Value : (object)patient.EmergencyContactName);
            cmd.Parameters.AddWithValue("@EmergencyRelationship", string.IsNullOrWhiteSpace(patient.EmergencyRelationship) ? DBNull.Value : (object)patient.EmergencyRelationship);
            cmd.Parameters.AddWithValue("@EmergencyPhone", string.IsNullOrWhiteSpace(patient.EmergencyPhone) ? DBNull.Value : (object)patient.EmergencyPhone);
            cmd.Parameters.AddWithValue("@id", patient.PatientId);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public bool Delete(int id, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"DELETE FROM Patients WHERE PatientId = @id";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@id", id);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public List<Patient> GetAll()
        {
            List<Patient> patients = new List<Patient>();

            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"SELECT * FROM Patients";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    patients.Add(new Patient
                    {
                        PatientId = Convert.ToInt32(dr["PatientId"]),
                        FirstName = dr["FirstName"]?.ToString() ?? "",
                        LastName = dr["LastName"]?.ToString() ?? "",
                        Phone = dr["Phone"]?.ToString() ?? "",
                        DOB = dr["DOB"] == DBNull.Value ? null : Convert.ToDateTime(dr["DOB"]),
                        Gender = dr["Gender"]?.ToString() ?? "",
                        Address1 = dr["Address1"]?.ToString() ?? "",
                        Address2 = dr["Address2"]?.ToString() ?? "",
                        City = dr["City"]?.ToString() ?? "",
                        State = dr["State"]?.ToString() ?? "",
                        PinCode = dr["PinCode"]?.ToString() ?? "",
                        IdType = dr["IdType"]?.ToString() ?? "",
                        IdNumber = dr["IdNumber"]?.ToString() ?? "",
                        EmergencyContactName = dr["EmergencyContactName"]?.ToString() ?? "",
                        EmergencyRelationship = dr["EmergencyRelationship"]?.ToString() ?? "",
                        EmergencyPhone = dr["EmergencyPhone"]?.ToString() ?? "",
                        ClinicId = Convert.ToInt32(dr["ClinicId"])
                    });
                }
            }

            return patients;
        }

        public Patient Get(int id)
        {
            using (SqlConnection con = _db.GetConnection())
            {
                string query = @"SELECT * FROM Patients WHERE PatientId = @id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);

                con.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    return new Patient
                    {
                        PatientId = Convert.ToInt32(dr["PatientId"]),
                        FirstName = dr["FirstName"]?.ToString() ?? "",
                        LastName = dr["LastName"]?.ToString() ?? "",
                        Phone = dr["Phone"]?.ToString() ?? "",
                        DOB = dr["DOB"] == DBNull.Value ? null : Convert.ToDateTime(dr["DOB"]),
                        Gender = dr["Gender"]?.ToString() ?? "",
                        Address1 = dr["Address1"]?.ToString() ?? "",
                        Address2 = dr["Address2"]?.ToString() ?? "",
                        City = dr["City"]?.ToString() ?? "",
                        State = dr["State"]?.ToString() ?? "",
                        PinCode = dr["PinCode"]?.ToString() ?? "",
                        IdType = dr["IdType"]?.ToString() ?? "",
                        IdNumber = dr["IdNumber"]?.ToString() ?? "",
                        EmergencyContactName = dr["EmergencyContactName"]?.ToString() ?? "",
                        EmergencyRelationship = dr["EmergencyRelationship"]?.ToString() ?? "",
                        EmergencyPhone = dr["EmergencyPhone"]?.ToString() ?? "",
                        ClinicId = Convert.ToInt32(dr["ClinicId"])
                    };
                }
            }

            return null;
        }

        public List<Patient> Search(PatientSearchVM vm)
        {
            var patients = new List<Patient>();

            SqlConnection con = _db.GetConnection();

            string sql = @"
                SELECT p.*, v.LastVisitDate
                FROM Patients p
                LEFT OUTER JOIN (SELECT PatientId, MAX(VisitDate) AS LastVisitDate FROM Visits GROUP BY PatientId) v
                    ON p.PatientId = v.PatientId
                WHERE
                   (p.PatientId LIKE '%' + @searchString + '%')
                OR (p.Phone LIKE '%' + @searchString + '%')
                OR (p.FirstName + ' ' + p.LastName LIKE '%' + @searchString + '%')
                AND p.ClinicId = @ClinicId
                ORDER BY p.PatientId DESC";

            using var cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@searchString", vm.SearchString);
            cmd.Parameters.AddWithValue("@ClinicId", vm.ClinicId);

            con.Open();

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                patients.Add(new Patient
                {
                    PatientId = Convert.ToInt32(reader["PatientId"]),
                    FirstName = reader["FirstName"]?.ToString() ?? "",
                    LastName = reader["LastName"]?.ToString() ?? "",
                    Phone = reader["Phone"]?.ToString() ?? "",
                    DOB = reader["DOB"] == DBNull.Value ? null : Convert.ToDateTime(reader["DOB"]),
                    Gender = reader["Gender"]?.ToString() ?? "",
                    Address1 = reader["Address1"]?.ToString() ?? "",
                    Address2 = reader["Address2"]?.ToString() ?? "",
                    City = reader["City"]?.ToString() ?? "",
                    State = reader["State"]?.ToString() ?? "",
                    PinCode = reader["PinCode"]?.ToString() ?? "",
                    IdType = reader["IdType"]?.ToString() ?? "",
                    IdNumber = reader["IdNumber"]?.ToString() ?? "",
                    EmergencyContactName = reader["EmergencyContactName"]?.ToString() ?? "",
                    EmergencyRelationship = reader["EmergencyRelationship"]?.ToString() ?? "",
                    EmergencyPhone = reader["EmergencyPhone"]?.ToString() ?? "",
                    ClinicId = Convert.ToInt32(reader["ClinicId"]),
                    LastVisitDate = reader["LastVisitDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["LastVisitDate"])
                });
            }

            return patients;
        }

        public List<Patient> ListByClinic(int clinicId)
        {
            var patients = new List<Patient>();

            SqlConnection con = _db.GetConnection();

            string sql = @"
                SELECT p.*, v.LastVisitDate
                FROM Patients p
                LEFT OUTER JOIN (SELECT PatientId, MAX(VisitDate) AS LastVisitDate FROM Visits GROUP BY PatientId) v
                    ON p.PatientId = v.PatientId
                WHERE ClinicId = @ClinicId
                ORDER BY p.PatientId DESC";

            using var cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@ClinicId", clinicId);

            con.Open();

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                patients.Add(new Patient
                {
                    PatientId = Convert.ToInt32(reader["PatientId"]),
                    FirstName = reader["FirstName"]?.ToString() ?? "",
                    LastName = reader["LastName"]?.ToString() ?? "",
                    DOB = reader["DOB"] == DBNull.Value ? null : Convert.ToDateTime(reader["DOB"]),
                    Gender = reader["Gender"]?.ToString() ?? "",
                    Address1 = reader["Address1"]?.ToString() ?? "",
                    Address2 = reader["Address2"]?.ToString() ?? "",
                    City = reader["City"]?.ToString() ?? "",
                    State = reader["State"]?.ToString() ?? "",
                    PinCode = reader["PinCode"]?.ToString() ?? "",
                    IdType = reader["IdType"]?.ToString() ?? "",
                    IdNumber = reader["IdNumber"]?.ToString() ?? "",
                    EmergencyContactName = reader["EmergencyContactName"]?.ToString() ?? "",
                    EmergencyRelationship = reader["EmergencyRelationship"]?.ToString() ?? "",
                    EmergencyPhone = reader["EmergencyPhone"]?.ToString() ?? "",
                    Phone = reader["Phone"]?.ToString() ?? "",
                    ClinicId = Convert.ToInt32(reader["ClinicId"]),
                    LastVisitDate = reader["LastVisitDate"] == DBNull.Value ? null : Convert.ToDateTime(reader["LastVisitDate"])
                });
            }

            return patients;
        }

    }
}


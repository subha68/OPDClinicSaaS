using Models;
using ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace DAL
{
    public class ConsultationDAL : IDAL<ConsultationVM>
    {
        private readonly DbHelper _db;

        public ConsultationDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(ConsultationVM vm)
        {
            int consultationId = 0;

            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"
                    INSERT INTO Consultations
                    (
                        AppointmentId,
                        Symptoms,
                        Diagnosis,
                        Notes
                    )
                    VALUES
                    (
                        @AppointmentId,
                        @Symptoms,
                        @Diagnosis,
                        @Notes
                    );
                                       
                    SELECT SCOPE_IDENTITY();";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@AppointmentId", vm.AppointmentId);
                cmd.Parameters.AddWithValue("@Symptoms", vm.Symptoms ?? "");
                cmd.Parameters.AddWithValue("@Diagnosis", vm.Diagnosis ?? "");
                cmd.Parameters.AddWithValue("@Notes", vm.Notes ?? "");

                conn.Open();

                //cmd.ExecuteNonQuery();
                consultationId = Convert.ToInt32(cmd.ExecuteScalar());
            }

            return consultationId;
        }

        public bool Update(ConsultationVM vm)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"
                    UPDATE Consultations
                    SET AppointmentId = @AppointmentId,
                        Symptoms = @Symptoms,
                        Diagnosis = @Diagnosis,
                        Notes = @Notes
                    WHERE ConsultationId = @ConsultationId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Symptoms", vm.Symptoms ?? "");
                cmd.Parameters.AddWithValue("@Diagnosis", vm.Diagnosis ?? "");
                cmd.Parameters.AddWithValue("@Notes", vm.Notes ?? "");
                cmd.Parameters.AddWithValue("@ConsultationId", vm.ConsultationId);
                cmd.Parameters.AddWithValue("@AppointmentId", vm.AppointmentId);

                conn.Open();

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool Delete(int id)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"DELETE FROM Consultations WHERE ConsultationId = @id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();

                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public List<ConsultationVM> GetAll()
        {
            List<ConsultationVM> consultations = new List<ConsultationVM>();

            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"SELECT ConsultationId, AppointmentId, Symptoms, Diagnosis, Notes FROM Consultations";

                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        consultations.Add(new ConsultationVM
                        {
                            ConsultationId = Convert.ToInt32(dr["ConsultationId"]),
                            AppointmentId = Convert.ToInt32(dr["AppointmentId"]),
                            Symptoms = dr["Symptoms"].ToString(),
                            Diagnosis = dr["Diagnosis"].ToString(),
                            Notes = dr["Notes"].ToString()
                        });
                    }
                }
            }

            return consultations;
        }

        public ConsultationVM Get(int consultationId)
        {
            ConsultationVM consultation = null;

            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"SELECT ConsultationId, AppointmentId, Symptoms, Diagnosis, Notes FROM Consultations WHERE ConsultationId = @id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", consultationId);

                conn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        consultation = new ConsultationVM
                        {
                            ConsultationId = Convert.ToInt32(dr["ConsultationId"]),
                            AppointmentId = Convert.ToInt32(dr["AppointmentId"]),
                            Symptoms = dr["Symptoms"].ToString(),
                            Diagnosis = dr["Diagnosis"].ToString(),
                            Notes = dr["Notes"].ToString()
                        };
                    }
                }
            }

            return consultation;
        }

        public ConsultationVM GetConsultationVM(int appointmentId)
        {
            ConsultationVM vm = null;

            using var con = _db.GetConnection();

            string sql = @"
                SELECT
                    a.AppointmentId,
                    a.AppointmentDate,
                    a.SlotTime,
                    p.PatientId,
                    p.FirstName + ' ' + p.LastName as PatientName,
                    DATEDIFF(YEAR, p.DOB, GETDATE())
                        -
                        CASE
                            WHEN DATEADD(YEAR,
                                        DATEDIFF(YEAR, p.DOB, GETDATE()),
                                        p.DOB) > GETDATE()
                            THEN 1
                            ELSE 0
                        END AS Age,
                    d.DoctorId,
                    d.FirstName + ' ' + d.LastName as DoctorName
                FROM Appointments a
                INNER JOIN Patients p
                    ON a.PatientId = p.PatientId
                INNER JOIN Doctors d
                    ON a.DoctorId = d.DoctorId
                WHERE a.AppointmentId = @AppointmentId";

            using var cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);

            con.Open();

            using var dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                vm = new ConsultationVM
                {
                    AppointmentId = Convert.ToInt32(dr["AppointmentId"]),
                    //AppointmentDate = Convert.ToDateTime(dr["AppointmentDate"]),
                    //SlotTime = Convert.ToDateTime(dr["SlotTime"]),
                    //PatientId = Convert.ToInt32(dr["PatientId"]),
                    PatientName = dr["PatientName"].ToString(),
                    //Age = Convert.ToInt32(dr["Age"]),
                    //DoctorId = Convert.ToInt32(dr["DoctorId"]),
                    //DoctorName = dr["DoctorName"].ToString()
                };
            }

            return vm;
        }

        public PrescriptionVM GetPrescription(int appointmentId)
        {
            PrescriptionVM vm = new();

            using SqlConnection conn = _db.GetConnection();

            conn.Open();

            // consultation + doctor + patient + clinic
            string sql = @"
            SELECT
                d.FirstName + ' ' + d.LastName AS DoctorName,
                d.Qualification,
                d.RegistrationNo,
                c.Name ClinicName,
                c.Address ClinicAddress,
                p.PatientId,
                p.FirstName + ' ' + p.LastName AS PatientName,
                p.Phone AS PatientPhoneNo,
                DATEDIFF(YEAR, p.DOB, GETDATE())
                    - CASE
                            WHEN DATEADD(YEAR,
                                        DATEDIFF(YEAR, p.DOB, GETDATE()),
                                        p.DOB) > GETDATE()
                            THEN 1
                            ELSE 0
                        END AS Age,
                p.Gender,
                a.AppointmentDate,
                a.SlotTime,
                con.Symptoms,
                con.Diagnosis,
                con.Notes
            FROM Appointments a
            JOIN Consultations con ON a.AppointmentId=con.AppointmentId
            JOIN Patients p ON a.PatientId=p.PatientId
            JOIN Doctors d ON a.DoctorId=d.DoctorId
            JOIN Clinics c ON a.ClinicId=c.ClinicId
            WHERE a.AppointmentId=@AppointmentId";

            SqlCommand cmd = new(sql, conn);
            cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);

            using var rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                vm.DoctorName = rdr["DoctorName"].ToString();
                vm.Qualification = rdr["Qualification"].ToString();
                vm.RegistrationNumber = rdr["RegistrationNo"].ToString();

                vm.ClinicName = rdr["ClinicName"].ToString();
                vm.ClinicAddress = rdr["ClinicAddress"].ToString();

                vm.PatientId = Convert.ToInt32(rdr["PatientId"]);
                vm.PatientName = rdr["PatientName"].ToString();
                vm.Age = Convert.ToInt32(rdr["Age"]);
                vm.Gender = rdr["Gender"].ToString();
                vm.PatientPhoneNo = rdr["PatientPhoneNo"].ToString();

                vm.VisitDateTime = Convert.ToDateTime(rdr["AppointmentDate"]).Add((TimeSpan)rdr["SlotTime"]);

                vm.Symptoms = rdr["Symptoms"].ToString();
                vm.Diagnosis = rdr["Diagnosis"].ToString();
                vm.Notes = rdr["Notes"].ToString();
                vm.AppointmentId = Convert.ToInt32(appointmentId);
            }

            rdr.Close();

            vm.Medicines = new List<PharmacologyAdviceVM>();

            string medSql = @"
                SELECT MedicineName,
                        Dosage,
                        Frequency,
                        Duration,
                        Instructions
                FROM PharmacologyAdvices
                WHERE AppointmentId=@AppointmentId
                ORDER BY PharmacologyId";

            SqlCommand medCmd = new(medSql, conn);
            medCmd.Parameters.AddWithValue("@AppointmentId", appointmentId);

            using var medRdr = medCmd.ExecuteReader();

            while (medRdr.Read())
            {
                vm.Medicines.Add(new PharmacologyAdviceVM
                {
                    MedicineName = medRdr["MedicineName"].ToString(),
                    Dosage = medRdr["Dosage"].ToString(),
                    Frequency = medRdr["Frequency"].ToString(),
                    Duration = medRdr["Duration"].ToString(),
                    Instructions = medRdr["Instructions"].ToString()
                });
            }

            return vm;
        }
    }

}


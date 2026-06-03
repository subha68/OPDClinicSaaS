using Models;
using ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace DAL
{
    public class ConsultationDAL : IDAL<Consultation>
    {
        private readonly DbHelper _db;

        public ConsultationDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public int Add(Consultation model, SqlConnection conn, SqlTransaction tran)
        {
            int consultationId = 0;

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

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@AppointmentId", model.AppointmentId);
            cmd.Parameters.AddWithValue("@Symptoms", model.Symptoms ?? "");
            cmd.Parameters.AddWithValue("@Diagnosis", model.Diagnosis ?? "");
            cmd.Parameters.AddWithValue("@Notes", model.Notes ?? "");

            consultationId = Convert.ToInt32(cmd.ExecuteScalar());

            return consultationId;
        }

        public bool Update(Consultation model, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                UPDATE Consultations
                SET AppointmentId = @AppointmentId,
                    Symptoms = @Symptoms,
                    Diagnosis = @Diagnosis,
                    Notes = @Notes
                WHERE ConsultationId = @ConsultationId";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@Symptoms", model.Symptoms ?? "");
            cmd.Parameters.AddWithValue("@Diagnosis", model.Diagnosis ?? "");
            cmd.Parameters.AddWithValue("@Notes", model.Notes ?? "");
            cmd.Parameters.AddWithValue("@ConsultationId", model.ConsultationId);
            cmd.Parameters.AddWithValue("@AppointmentId", model.AppointmentId);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public bool Delete(int id, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"DELETE FROM Consultations WHERE ConsultationId = @id";

            SqlCommand cmd = new SqlCommand(query, conn, tran);
            cmd.Parameters.AddWithValue("@id", id);

            int rowsAffected = cmd.ExecuteNonQuery();
            return rowsAffected > 0;
        }

        public List<Consultation> GetAll()
        {
            List<Consultation> consultations = new List<Consultation>();

            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"SELECT ConsultationId, AppointmentId, Symptoms, Diagnosis, Notes FROM Consultations";

                SqlCommand cmd = new SqlCommand(query, conn);

                conn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        consultations.Add(new Consultation
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

        public Consultation Get(int id)
        {
            Consultation consultation = null;

            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"SELECT ConsultationId, AppointmentId, Symptoms, Diagnosis, Notes FROM Consultations WHERE ConsultationId = @id";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        consultation = new Consultation
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

        public Consultation GetConsultation(int appointmentId)
        {
            Consultation consultation = null;

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
                consultation = new Consultation
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

            return consultation;
        }

        public List<Consultation> GetConsultationsByPatientId(int patientId)
        {
            List<Consultation> consultations = new List<Consultation>();

            using var con = _db.GetConnection();

            string sql = @"SELECT c.ConsultationId, c.AppointmentId, c.Symptoms, c.Diagnosis, c.Notes,
                                a.AppointmentDate, a.SlotTime,
                                p.PatientId,
                                p.FirstName + ' ' + p.LastName as PatientName,
                                d.DoctorId,
                                d.FirstName + ' ' + d.LastName as DoctorName
                            FROM Consultations c
                            INNER JOIN Appointments a ON c.AppointmentId = a.AppointmentId
                            INNER JOIN Patients p ON a.PatientId = p.PatientId
                            INNER JOIN Doctors d ON a.DoctorId = d.DoctorId
                            WHERE p.PatientId = @PatientId";

            using var cmd = new SqlCommand(sql, con);

            cmd.Parameters.AddWithValue("@PatientId", patientId);

            con.Open();

            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                consultations.Add(new Consultation
                {
                    AppointmentId = Convert.ToInt32(dr["AppointmentId"]),
                    //AppointmentDate = Convert.ToDateTime(dr["AppointmentDate"]),
                    //SlotTime = Convert.ToDateTime(dr["SlotTime"]),
                    PatientId = Convert.ToInt32(dr["PatientId"]),
                    PatientName = dr["PatientName"].ToString(),
                    //Age = Convert.ToInt32(dr["Age"]),
                    DoctorId = Convert.ToInt32(dr["DoctorId"]),
                    DoctorName = dr["DoctorName"].ToString(),
                    Symptoms = dr["Symptoms"].ToString(),
                    Diagnosis = dr["Diagnosis"].ToString(),
                    Notes = dr["Notes"].ToString()
                });
            }

            return consultations;
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

            vm.Medicines = new List<PharmacologyAdvice>();

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
                vm.Medicines.Add(new PharmacologyAdvice
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

        public List<PrescriptionVM> GetPrescriptionsByPatientId(int patientId)
        {
            var prescriptions = new List<PrescriptionVM>();

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
                con.Notes,
                con.ConsultationId,
                a.AppointmentId
            FROM Appointments a
            JOIN Consultations con ON a.AppointmentId=con.AppointmentId
            JOIN Patients p ON a.PatientId=p.PatientId
            JOIN Doctors d ON a.DoctorId=d.DoctorId
            JOIN Clinics c ON a.ClinicId=c.ClinicId
            WHERE a.AppointmentId=@PatientId";

            using (SqlCommand cmd = new(sql, conn))
            {
                cmd.Parameters.AddWithValue("@PatientId", patientId);

                using var rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    var vm = new PrescriptionVM();

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
                    vm.AppointmentId = Convert.ToInt32(rdr["AppointmentId"]);

                    int conId = Convert.ToInt32(rdr["ConsultationId"]);

                    string medSql = @"
                        SELECT MedicineName,
                                Dosage,
                                Frequency,
                                Duration,
                                Instructions
                        FROM PharmacologyAdvices
                        WHERE ConsultationId=@ConsultationId
                        ORDER BY PharmacologyId";

                    SqlCommand medCmd = new(medSql, conn);
                    medCmd.Parameters.AddWithValue("@ConsultationId", conId);

                    using var medRdr = medCmd.ExecuteReader();

                    while (medRdr.Read())
                    {
                        vm.Medicines.Add(new PharmacologyAdvice
                        {
                            MedicineName = medRdr["MedicineName"].ToString(),
                            Dosage = medRdr["Dosage"].ToString(),
                            Frequency = medRdr["Frequency"].ToString(),
                            Duration = medRdr["Duration"].ToString(),
                            Instructions = medRdr["Instructions"].ToString()
                        });
                    }
                }
            }

            return prescriptions;
        }
    }

}


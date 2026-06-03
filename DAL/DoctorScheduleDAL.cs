using ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;

namespace DAL
{
    public class DoctorScheduleDAL : IDAL<DoctorScheduleVM>
    {
        private readonly DbHelper _db;

        public DoctorScheduleDAL(IConfiguration configuration)
        {
            _db = new DbHelper(configuration);
        }

        public List<DoctorScheduleVM> GetAllDoctorSchedulesByClinic(int clinicId)
        {
            List<DoctorScheduleVM> schedules = new List<DoctorScheduleVM>();

            using (SqlConnection con = _db.GetConnection())
            {
                string query = @"
                    SELECT
                        ScheduleId,
                        Doctors.DoctorId,
                        Doctors.FirstName AS DoctorFirstName,
                        Doctors.LastName AS DoctorLastName,
                        Clinics.Name AS ClinicName,
                        Clinics.ClinicId,
                        DayOfWeek,
                        StartTime,
                        EndTime,
                        SlotDurationMinutes,
                        MaxPatientsPerSlot,
                        IsActive,
                        CreatedOn
                    FROM DoctorSchedules
                    INNER JOIN Doctors ON DoctorSchedules.DoctorId = Doctors.DoctorId
                    INNER JOIN Clinics ON DoctorSchedules.ClinicId = Clinics.ClinicId
                    WHERE DoctorSchedules.ClinicId = @ClinicId
                    ORDER BY DoctorId, DayOfWeek";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ClinicId", clinicId);

                con.Open();
                SqlDataReader sched = cmd.ExecuteReader();

                while (sched.Read())
                {
                    schedules.Add(MapReader(sched));
                }
            }

            return schedules;
        }

        public List<DoctorScheduleVM> GetByDoctor(int doctorId)
        {
            List<DoctorScheduleVM> schedules = new List<DoctorScheduleVM>();

            using (SqlConnection con = _db.GetConnection())
            {
                string query = @"
                    SELECT
                        ScheduleId,
                        Doctors.DoctorId,
                        Doctors.FirstName AS DoctorFirstName,
                        Doctors.LastName AS DoctorLastName,
                        Clinics.ClinicId,
                        Clinics.Name AS ClinicName,
                        DayOfWeek,
                        StartTime,
                        EndTime,
                        SlotDurationMinutes,
                        MaxPatientsPerSlot,
                        IsActive,
                        CreatedOn
                    FROM DoctorSchedules
                    INNER JOIN Doctors ON DoctorSchedules.DoctorId = Doctors.DoctorId
                    INNER JOIN Clinics ON DoctorSchedules.ClinicId = Clinics.ClinicId
                    WHERE DoctorSchedules.DoctorId = @DoctorId
                    ORDER BY DayOfWeek";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DoctorId", doctorId);

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        schedules.Add(MapReader(reader));
                    }
                }
            }

            return schedules;
        }

        public int Add(DoctorScheduleVM model, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                INSERT INTO DoctorSchedules
                (
                    DoctorId,
                    ClinicId,
                    DayOfWeek,
                    StartTime,
                    EndTime,
                    SlotDurationMinutes,
                    MaxPatientsPerSlot,
                    IsActive,
                    CreatedOn
                )
                VALUES
                (
                    @DoctorId,
                    @ClinicId,
                    @DayOfWeek,
                    @StartTime,
                    @EndTime,
                    @SlotDurationMinutes,
                    @MaxPatientsPerSlot,
                    @IsActive,
                    GETDATE()
                );

                SELECT SCOPE_IDENTITY();
            ";

            using (SqlCommand cmd = new SqlCommand(query, conn, tran))
            {
                cmd.Parameters.AddWithValue("@DoctorId", model.DoctorId);
                cmd.Parameters.AddWithValue("@ClinicId", model.ClinicId);
                cmd.Parameters.AddWithValue("@DayOfWeek", model.DayOfWeek);
                cmd.Parameters.AddWithValue("@StartTime", model.StartTime);
                cmd.Parameters.AddWithValue("@EndTime", model.EndTime);
                cmd.Parameters.AddWithValue("@SlotDurationMinutes", model.SlotDurationMinutes);
                cmd.Parameters.AddWithValue("@MaxPatientsPerSlot", model.MaxPatientsPerSlot);
                cmd.Parameters.AddWithValue("@IsActive", model.IsActive);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public bool Update(DoctorScheduleVM model, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                UPDATE DoctorSchedules
                SET
                    DoctorId = @DoctorId,
                    ClinicId = @ClinicId,
                    DayOfWeek = @DayOfWeek,
                    StartTime = @StartTime,
                    EndTime = @EndTime,
                    SlotDurationMinutes = @SlotDurationMinutes,
                    MaxPatientsPerSlot = @MaxPatientsPerSlot,
                    IsActive = @IsActive
                WHERE ScheduleId = @ScheduleId";

            using (SqlCommand cmd = new SqlCommand(query, conn, tran))
            {
                cmd.Parameters.AddWithValue("@ScheduleId", model.ScheduleId);
                cmd.Parameters.AddWithValue("@DoctorId", model.DoctorId);
                cmd.Parameters.AddWithValue("@ClinicId", model.ClinicId);
                cmd.Parameters.AddWithValue("@DayOfWeek", model.DayOfWeek);
                cmd.Parameters.AddWithValue("@StartTime", model.StartTime);
                cmd.Parameters.AddWithValue("@EndTime", model.EndTime);
                cmd.Parameters.AddWithValue("@SlotDurationMinutes", model.SlotDurationMinutes);
                cmd.Parameters.AddWithValue("@MaxPatientsPerSlot", model.MaxPatientsPerSlot);
                cmd.Parameters.AddWithValue("@IsActive", model.IsActive);

                int rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }

        public bool Delete(int scheduleId, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                DELETE FROM DoctorSchedules
                    WHERE ScheduleId = @ScheduleId";

            using (SqlCommand cmd = new SqlCommand(query, conn, tran))
            {
                cmd.Parameters.AddWithValue("@ScheduleId", scheduleId);

                int rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }

        public List<DoctorScheduleVM> GetAll()
        {
            // This method is not used in our current design, but we need to implement it to satisfy the IDAL interface.
            // In a real application, we might want to throw a NotImplementedException or return an empty list instead.
            return new List<DoctorScheduleVM>();
        }

        /// Checks whether another active schedule overlaps
        /// with the provided schedule timings.

        public bool ExistsOverlap(
            int doctorId,
            int dayOfWeek,
            TimeSpan startTime,
            TimeSpan endTime,
            int? excludeScheduleId = null)
        {
            using (SqlConnection con = _db.GetConnection())
            {
                string query = @"
                    SELECT COUNT(*)
                    FROM DoctorSchedules
                    WHERE DoctorId = @DoctorId
                    AND DayOfWeek = @DayOfWeek
                    AND IsActive = 1
                    AND (
                        @StartTime < EndTime
                        AND @EndTime > StartTime
                    )";

                // Exclude current schedule during update
                if (excludeScheduleId.HasValue)
                {
                    query += " AND ScheduleId <> @ExcludeScheduleId";
                }

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@DoctorId", doctorId);
                    cmd.Parameters.AddWithValue("@DayOfWeek", dayOfWeek);
                    cmd.Parameters.AddWithValue("@StartTime", startTime);
                    cmd.Parameters.AddWithValue("@EndTime", endTime);

                    if (excludeScheduleId.HasValue)
                    {
                        cmd.Parameters.AddWithValue(
                            "@ExcludeScheduleId",
                            excludeScheduleId.Value);
                    }

                    con.Open();

                    int count = Convert.ToInt32(cmd.ExecuteScalar());

                    return count > 0;
                }
            }
        }

        public DoctorScheduleVM Get(int scheduleId)
        {
            using (SqlConnection con = _db.GetConnection())
            {
                string query = @"
                    SELECT
                        ScheduleId,
                        Doctors.DoctorId,
                        Doctors.FirstName AS DoctorFirstName,
                        Doctors.LastName AS DoctorLastName,
                        Clinics.ClinicId,
                        Clinics.Name AS ClinicName,
                        DayOfWeek,
                        StartTime,
                        EndTime,
                        SlotDurationMinutes,
                        MaxPatientsPerSlot,
                        IsActive,
                        CreatedOn
                    FROM DoctorSchedules
                    INNER JOIN Doctors ON DoctorSchedules.DoctorId = Doctors.DoctorId
                    INNER JOIN Clinics ON DoctorSchedules.ClinicId = Clinics.ClinicId
                    WHERE ScheduleId = @ScheduleId";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ScheduleId", scheduleId);

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return MapReader(reader);
                    }
                }
            }

            return null; // Not found
        }
        
        private DoctorScheduleVM MapReader(SqlDataReader reader)
        {
            return new DoctorScheduleVM
            {
                ScheduleId = Convert.ToInt32(reader["ScheduleId"]),
                DoctorId = Convert.ToInt32(reader["DoctorId"]),
                ClinicId = Convert.ToInt32(reader["ClinicId"]),
                DayOfWeek = Convert.ToInt32(reader["DayOfWeek"]),
                DoctorFirstName = reader["DoctorFirstName"].ToString(),
                DoctorLastName = reader["DoctorLastName"].ToString(),
                ClinicName = reader["ClinicName"].ToString(),

                StartTime = (TimeSpan)reader["StartTime"],
                EndTime = (TimeSpan)reader["EndTime"],

                SlotDurationMinutes = Convert.ToInt32(reader["SlotDurationMinutes"]),
                MaxPatientsPerSlot = Convert.ToInt32(reader["MaxPatientsPerSlot"]),

                IsActive = Convert.ToBoolean(reader["IsActive"]),

                CreatedOn = Convert.ToDateTime(reader["CreatedOn"])
            };
        }

    }
}
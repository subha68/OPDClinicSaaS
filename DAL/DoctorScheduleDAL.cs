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
                    schedules.Add(MapScheduleReader(sched));
                }
            }

            return schedules;
        }

        public DoctorScheduleVM GetDoctorSchedule(int doctorId)
        {
            DoctorScheduleVM vm = null;

            using (SqlConnection con = _db.GetConnection())
            {
                string query = @"
                    SELECT
                        ds.ScheduleId,
                        ds.DoctorId,
                        d.FirstName AS DoctorFirstName,
                        d.LastName AS DoctorLastName,
                        ds.ClinicId,
                        c.Name AS ClinicName,
                        ds.IsActive,
                        ds.CreatedOn,
                        ds.DayOfWeek,
                        ds.StartTime,
                        ds.EndTime,
                        ds.SlotDurationMinutes,
                        ds.MaxPatientsPerSlot
                    FROM DoctorSchedules ds
                    INNER JOIN Doctors d
                        ON ds.DoctorId = d.DoctorId
                    INNER JOIN Clinics c
                        ON ds.ClinicId = c.ClinicId
                    WHERE ds.DoctorId = @DoctorId
                    ORDER BY
                        ds.DayOfWeek,
                        ds.StartTime";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.Add("@DoctorId", SqlDbType.Int).Value = doctorId;

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Create header once
                        if (vm == null)
                        {
                            vm = MapScheduleReader(reader);
                        }

                        vm.Shifts.Add(MapShiftReader(reader));
                    }
                }
            }

            return vm;
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
                cmd.Parameters.AddWithValue("@DayOfWeek", model.Shifts[0].DayOfWeek);
                cmd.Parameters.AddWithValue("@StartTime", model.Shifts[0].StartTime);
                cmd.Parameters.AddWithValue("@EndTime", model.Shifts[0].EndTime);
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
                cmd.Parameters.AddWithValue("@DayOfWeek", model.Shifts[0].DayOfWeek);
                cmd.Parameters.AddWithValue("@StartTime", model.Shifts[0].StartTime);
                cmd.Parameters.AddWithValue("@EndTime", model.Shifts[0].EndTime);
                cmd.Parameters.AddWithValue("@SlotDurationMinutes", model.SlotDurationMinutes);
                cmd.Parameters.AddWithValue("@MaxPatientsPerSlot", model.MaxPatientsPerSlot);
                cmd.Parameters.AddWithValue("@IsActive", model.IsActive);

                int rowsAffected = cmd.ExecuteNonQuery();

                return rowsAffected > 0;
            }
        }

        public bool SaveDoctorSchedule(int doctorId, DoctorScheduleVM schedule, SqlConnection conn, SqlTransaction tran)
        {
            int deleteRowsAffected = 0;
            int insertRowsAffected = 0;

            // Delete existing schedule
            string deleteQuery = @"
                DELETE FROM DoctorSchedules
                WHERE DoctorId = @DoctorId";

            using (SqlCommand cmd = new SqlCommand(deleteQuery, conn, tran))
            {
                cmd.Parameters.Add("@DoctorId", SqlDbType.Int).Value = doctorId;
                deleteRowsAffected = cmd.ExecuteNonQuery();
            }

            // Insert new shifts
            string insertQuery = @"
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
                    @CreatedOn
                )";

            foreach (var shift in schedule.Shifts)
            {
                using (SqlCommand cmd = new SqlCommand(insertQuery, conn, tran))
                {
                    cmd.Parameters.Add("@DoctorId", SqlDbType.Int).Value = schedule.DoctorId;
                    cmd.Parameters.Add("@ClinicId", SqlDbType.Int).Value = schedule.ClinicId;
                    cmd.Parameters.Add("@DayOfWeek", SqlDbType.Int).Value = shift.DayOfWeek;
                    cmd.Parameters.Add("@StartTime", SqlDbType.Time).Value = shift.StartTime;
                    cmd.Parameters.Add("@EndTime", SqlDbType.Time).Value = shift.EndTime;
                    cmd.Parameters.Add("@SlotDurationMinutes", SqlDbType.Int).Value = schedule.SlotDurationMinutes;
                    cmd.Parameters.Add("@MaxPatientsPerSlot", SqlDbType.Int).Value = schedule.MaxPatientsPerSlot;
                    cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = schedule.IsActive;
                    cmd.Parameters.Add("@CreatedOn", SqlDbType.DateTime).Value = schedule.CreatedOn;

                    insertRowsAffected += cmd.ExecuteNonQuery();
                }
            }

            return deleteRowsAffected > 0 && insertRowsAffected > 0;
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

        public bool DeleteDoctorSchedule(int doctorId, SqlConnection conn, SqlTransaction tran)
        {
            string query = @"
                DELETE FROM DoctorSchedules
                    WHERE DoctorId = @DoctorId";

            using (SqlCommand cmd = new SqlCommand(query, conn, tran))
            {
                cmd.Parameters.AddWithValue("@DoctorId", doctorId);

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
            DoctorScheduleVM vm = null;

            using (SqlConnection con = _db.GetConnection())
            {
                string query = @"
                    SELECT
                        ds.ScheduleId,
                        ds.DoctorId,
                        d.FirstName AS DoctorFirstName,
                        d.LastName AS DoctorLastName,
                        ds.ClinicId,
                        c.Name AS ClinicName,
                        ds.IsActive,
                        ds.CreatedOn,
                        ds.DayOfWeek,
                        ds.StartTime,
                        ds.EndTime,
                        ds.SlotDurationMinutes,
                        ds.MaxPatientsPerSlot

                    FROM DoctorSchedules ds
                    INNER JOIN Doctors d
                        ON ds.DoctorId = d.DoctorId
                    INNER JOIN Clinics c
                        ON ds.ClinicId = c.ClinicId

                    WHERE ds.ScheduleId = @ScheduleId

                    ORDER BY
                        ds.DayOfWeek,
                        ds.StartTime";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.Add("@ScheduleId", SqlDbType.Int).Value = scheduleId;

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Create header once
                        if (vm == null)
                        {
                            vm = MapScheduleReader(reader);
                        }

                        vm.Shifts.Add(MapShiftReader(reader));
                    }
                }
            }

            return vm;
        }

        public DoctorScheduleVM GetSchedulesByDay(int clinicId, int doctorId, int dayOfWeek)
        {
            DoctorScheduleVM vm = new DoctorScheduleVM();

            using (SqlConnection con = _db.GetConnection())
            {
                string query = @"
                    SELECT
                        ds.ScheduleId,
                        ds.DoctorId,
                        d.FirstName AS DoctorFirstName,
                        d.LastName AS DoctorLastName,
                        ds.ClinicId,
                        c.Name AS ClinicName,
                        ds.IsActive,
                        ds.CreatedOn,
                        ds.DayOfWeek,
                        ds.StartTime,
                        ds.EndTime,
                        ds.SlotDurationMinutes,
                        ds.MaxPatientsPerSlot

                    FROM DoctorSchedules ds
                    INNER JOIN Doctors d
                        ON ds.DoctorId = d.DoctorId
                    INNER JOIN Clinics c
                        ON ds.ClinicId = c.ClinicId

                    WHERE ds.ClinicId = @ClinicId
                    AND ds.DoctorId = @DoctorId
                    AND ds.DayOfWeek = @DayOfWeek

                    ORDER BY
                        ds.DayOfWeek,
                        ds.StartTime";

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.Add("@ClinicId", SqlDbType.Int).Value = clinicId;
                cmd.Parameters.Add("@DoctorId", SqlDbType.Int).Value = doctorId;
                cmd.Parameters.Add("@DayOfWeek", SqlDbType.Int).Value = dayOfWeek;

                con.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Create header once
                        vm = MapScheduleReader(reader);
                        
                        vm.Shifts.Add(MapShiftReader(reader));
                    }
                }
            }

            return vm;
        }
        
        private DoctorScheduleVM MapScheduleReader(SqlDataReader reader)
        {
            return new DoctorScheduleVM
            {
                ScheduleId = Convert.ToInt32(reader["ScheduleId"]),
                DoctorId = Convert.ToInt32(reader["DoctorId"]),
                ClinicId = Convert.ToInt32(reader["ClinicId"]),
                DoctorFirstName = reader["DoctorFirstName"].ToString(),
                DoctorLastName = reader["DoctorLastName"].ToString(),
                ClinicName = reader["ClinicName"].ToString(),
                SlotDurationMinutes = Convert.ToInt32(reader["SlotDurationMinutes"]),
                MaxPatientsPerSlot = Convert.ToInt32(reader["MaxPatientsPerSlot"]),
                IsActive = Convert.ToBoolean(reader["IsActive"]),
                CreatedOn = Convert.ToDateTime(reader["CreatedOn"])
            };
        }

        private DoctorShiftVM MapShiftReader(SqlDataReader reader)
        {
            return new DoctorShiftVM
            {
                DoctorId = Convert.ToInt32(reader["DoctorId"]),
                ClinicId = Convert.ToInt32(reader["ClinicId"]),
                DayOfWeek = (DayOfWeek)reader["DayOfWeek"],
                StartTime = (TimeSpan)reader["StartTime"],
                EndTime = (TimeSpan)reader["EndTime"]
            };
        }
    }
}
using Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace DAL
{
    public class AppointmentSlotDAL : IDAL<AppointmentSlot>
    {
        private readonly DbHelper _db;

        public AppointmentSlotDAL(IConfiguration config)
        {
            _db = new DbHelper(config);
        }

        public List<AppointmentSlot> GetAll()
        {
            List<AppointmentSlot> slots =
                new List<AppointmentSlot>();

            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"
                    SELECT
                        SlotId,
                        DoctorId,
                        ClinicId,
                        SlotDate,
                        StartDateTime,
                        EndDateTime,
                        Capacity,
                        BookedCount,
                        IsBlocked,
                        CreatedOn
                    FROM AppointmentSlot
                    ORDER BY SlotDate, StartDateTime";

                using (SqlCommand cmd =
                    new SqlCommand(query, conn))
                {
                    conn.Open();

                    using (SqlDataReader reader =
                        cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            slots.Add(MapReader(reader));
                        }
                    }
                }
            }

            return slots;
        }

        public AppointmentSlot Get(int slotId)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"
                    SELECT
                        SlotId,
                        DoctorId,
                        ClinicId,
                        SlotDate,
                        StartDateTime,
                        EndDateTime,
                        Capacity,
                        BookedCount,
                        IsBlocked,
                        CreatedOn
                    FROM AppointmentSlot
                    WHERE SlotId = @SlotId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@SlotId",
                        slotId);

                    conn.Open();

                    using (SqlDataReader reader =
                        cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return MapReader(reader);
                        }
                    }
                }

                return null; // Not found
            }
        }

        public List<AppointmentSlot> GetByDoctor(int doctorId)
        {
            List<AppointmentSlot> slots =
                new List<AppointmentSlot>();

            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"
                    SELECT
                        SlotId,
                        DoctorId,
                        ClinicId,
                        SlotDate,
                        StartDateTime,
                        EndDateTime,
                        Capacity,
                        BookedCount,
                        IsBlocked,
                        CreatedOn
                    FROM AppointmentSlot
                    WHERE DoctorId = @DoctorId
                    ORDER BY SlotDate, StartDateTime";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DoctorId", doctorId);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            slots.Add(MapReader(reader));
                        }
                    }
                }
            }

            return slots;
        }

        public List<AppointmentSlot> GetByDate(int doctorId, DateTime slotDate)
        {
            List<AppointmentSlot> slots = new List<AppointmentSlot>();

            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"
                    SELECT
                        SlotId,
                        DoctorId,
                        ClinicId,
                        SlotDate,
                        StartDateTime,
                        EndDateTime,
                        Capacity,
                        BookedCount,
                        IsBlocked,
                        CreatedOn
                    FROM AppointmentSlot
                    WHERE DoctorId = @DoctorId
                    AND CAST(SlotDate AS DATE) = @SlotDate
                    ORDER BY StartDateTime";

                using (SqlCommand cmd =
                    new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@DoctorId",
                        doctorId);

                    cmd.Parameters.AddWithValue(
                        "@SlotDate",
                        slotDate.Date);

                    conn.Open();

                    using (SqlDataReader reader =
                        cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            slots.Add(MapReader(reader));
                        }
                    }
                }
            }

            return slots;
        }

        public int Add(AppointmentSlot model)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"
                    INSERT INTO AppointmentSlot
                    (
                        DoctorId,
                        ClinicId,
                        SlotDate,
                        StartDateTime,
                        EndDateTime,
                        Capacity,
                        BookedCount,
                        IsBlocked,
                        CreatedOn
                    )
                    VALUES
                    (
                        @DoctorId,
                        @ClinicId,
                        @SlotDate,
                        @StartDateTime,
                        @EndDateTime,
                        @Capacity,
                        @BookedCount,
                        @IsBlocked,
                        GETDATE()
                    );

                    SELECT SCOPE_IDENTITY();
                ";

                using (SqlCommand cmd =
                    new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@DoctorId",
                        model.DoctorId);

                    cmd.Parameters.AddWithValue(
                        "@ClinicId",
                        model.ClinicId);

                    cmd.Parameters.AddWithValue(
                        "@SlotDate",
                        model.SlotDate.Date);

                    cmd.Parameters.AddWithValue(
                        "@StartDateTime",
                        model.StartDateTime);

                    cmd.Parameters.AddWithValue(
                        "@EndDateTime",
                        model.EndDateTime);

                    cmd.Parameters.AddWithValue(
                        "@Capacity",
                        model.Capacity);

                    cmd.Parameters.AddWithValue(
                        "@BookedCount",
                        model.BookedCount);

                    cmd.Parameters.AddWithValue(
                        "@IsBlocked",
                        model.IsBlocked);

                    conn.Open();

                    return Convert.ToInt32(
                        cmd.ExecuteScalar());
                }
            }
        }

        public bool Update(AppointmentSlot model)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"
                    UPDATE AppointmentSlot
                    SET
                        Capacity = @Capacity,
                        BookedCount = @BookedCount,
                        IsBlocked = @IsBlocked
                    WHERE SlotId = @SlotId";

                using (SqlCommand cmd =
                    new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@SlotId",
                        model.SlotId);

                    cmd.Parameters.AddWithValue(
                        "@Capacity",
                        model.Capacity);

                    cmd.Parameters.AddWithValue(
                        "@BookedCount",
                        model.BookedCount);

                    cmd.Parameters.AddWithValue(
                        "@IsBlocked",
                        model.IsBlocked);

                    conn.Open();

                    int rowsAffected = cmd.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }

        public bool Delete(int slotId)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"
                    DELETE FROM AppointmentSlot
                    WHERE SlotId = @SlotId";

                using (SqlCommand cmd =
                    new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@SlotId",
                        slotId);

                    conn.Open();

                    int rowsAffected =
                        cmd.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }

        /// Prevent duplicate slot generation.

        public bool ExistsSlot(
            int doctorId,
            DateTime startDateTime,
            DateTime endDateTime)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"
                    SELECT COUNT(*)
                    FROM AppointmentSlot
                    WHERE DoctorId = @DoctorId
                    AND StartDateTime = @StartDateTime
                    AND EndDateTime = @EndDateTime";

                using (SqlCommand cmd =
                    new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@DoctorId",
                        doctorId);

                    cmd.Parameters.AddWithValue(
                        "@StartDateTime",
                        startDateTime);

                    cmd.Parameters.AddWithValue(
                        "@EndDateTime",
                        endDateTime);

                    conn.Open();

                    int count = Convert.ToInt32(
                        cmd.ExecuteScalar());

                    return count > 0;
                }
            }
        }

        public bool IsSlotAvailable(int slotId)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"
                    SELECT COUNT(*)
                    FROM AppointmentSlot
                    WHERE SlotId = @SlotId
                    AND IsBlocked = 0
                    AND BookedCount < Capacity";

                using (SqlCommand cmd =
                    new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@SlotId",
                        slotId);

                    conn.Open();

                    int count = Convert.ToInt32(
                        cmd.ExecuteScalar());

                    return count > 0;
                }
            }
        }

        public bool IncrementBookedCount(int slotId)
        {
            using (SqlConnection conn = _db.GetConnection())
            {
                string query = @"
                    UPDATE AppointmentSlot
                    SET BookedCount = BookedCount + 1
                    WHERE SlotId = @SlotId
                    AND BookedCount < Capacity";

                using (SqlCommand cmd =
                    new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@SlotId",
                        slotId);

                    conn.Open();

                    int rowsAffected =
                        cmd.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
        }

        private AppointmentSlot MapReader(
            SqlDataReader reader)
        {
            return new AppointmentSlot
            {
                SlotId = Convert.ToInt32(reader["SlotId"]),

                DoctorId = Convert.ToInt32(reader["DoctorId"]),

                ClinicId = Convert.ToInt32(reader["ClinicId"]),

                SlotDate = Convert.ToDateTime(reader["SlotDate"]),

                StartDateTime = Convert.ToDateTime(reader["StartDateTime"]),

                EndDateTime = Convert.ToDateTime(reader["EndDateTime"]),

                Capacity = Convert.ToInt32(reader["Capacity"]),

                BookedCount = Convert.ToInt32(reader["BookedCount"]),

                IsBlocked = Convert.ToBoolean(reader["IsBlocked"]),

                CreatedOn = Convert.ToDateTime(reader["CreatedOn"])
            };
        }

    }
}
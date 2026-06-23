using ViewModels;
using Models;
using DAL;
using System;
using System.Collections.Generic;

namespace BLL
{
    public class DoctorScheduleService : BaseService<DoctorScheduleVM>
    {
        private readonly DoctorScheduleDAL _scheduleDAL;

        public DoctorScheduleService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(config, httpContextAccessor)
        {
            _scheduleDAL = new DoctorScheduleDAL(config);
            _dal = _scheduleDAL;
        }

        public override List<DoctorScheduleVM> List()
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _scheduleDAL.GetAllDoctorSchedulesByClinic(ClinicId, conn);
            }
            catch
            {
                throw;
            }
        }

        public DoctorScheduleVM GetDoctorSchedule(int doctorId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _scheduleDAL.GetDoctorSchedule(doctorId, conn);
            }
            catch
            {
                throw;
            }
        }

        /// Validates and saves doctor schedule.
        /// Handles both Insert and Update.

        public bool SaveDoctorSchedule(DoctorScheduleVM model)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                bool result = _scheduleDAL.SaveDoctorSchedule(model.DoctorId, model, conn, tran);

                tran.Commit();
                return result;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

    /*    public void ValidateSchedule(DoctorScheduleVM model)
        {
            if (model == null)
            {
                throw new Exception("Schedule model cannot be null.");
            }

            if (model.DoctorId <= 0)
            {
                throw new Exception("Please select doctor.");
            }

            if (model.ClinicId <= 0)
            {
                throw new Exception("Invalid clinic.");
            }

            if (model.DayOfWeek < 0 || model.DayOfWeek > 6)
            {
                throw new Exception("Invalid day of week.");
            }

            if (model.StartTime >= model.EndTime)
            {
                throw new Exception(
                    "End time must be greater than start time.");
            }

            if (model.SlotDurationMinutes <= 0)
            {
                throw new Exception(
                    "Slot duration must be greater than zero.");
            }

            if (model.SlotDurationMinutes > 240)
            {
                throw new Exception(
                    "Slot duration is too large.");
            }

            if (model.MaxPatientsPerSlot <= 0)
            {
                throw new Exception(
                    "Max patients per slot must be greater than zero.");
            }

            // Optional advanced validation
            TimeSpan totalDuration = model.EndTime - model.StartTime;

            if (totalDuration.TotalMinutes < model.SlotDurationMinutes)
            {
                throw new Exception(
                    "Slot duration cannot exceed schedule duration.");
            }
        } */

        public override bool Remove(int scheduleId)
        {
            if (scheduleId <= 0)
            {
                throw new Exception("Invalid schedule id.");
            }

            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();
            try
            {
                bool deleted = _dal.Delete(scheduleId, conn, tran);

                if (!deleted)
                {
                    throw new Exception("Unable to delete schedule.");
                }

                tran.Commit();
                return deleted;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        /// Generates appointment slots
        /// between given date range.

    /*    public void GenerateSlots(int doctorId, DateTime fromDate, DateTime toDate)
        {
            if (doctorId <= 0)
            {
                throw new Exception("Invalid doctor.");
            }

            if (fromDate.Date > toDate.Date)
            {
                throw new Exception(
                    "From date cannot be greater than To date.");
            }

            List<DoctorScheduleVM> schedules = _scheduleDAL.GetDoctorSchedule(doctorId);

            if (schedules == null || schedules.Count == 0)
            {
                throw new Exception(
                    "No active schedules found for doctor.");
            }

            DateTime currentDate = fromDate.Date;

            while (currentDate <= toDate.Date)
            {
                int currentDay = (int)currentDate.DayOfWeek;

                foreach (DoctorScheduleVM schedule in schedules)
                {
                    if (!schedule.IsActive)
                        continue;

                    if (schedule.DayOfWeek != currentDay)
                        continue;

                    GenerateSlotsForDay(schedule, currentDate);
                }

                currentDate = currentDate.AddDays(1);
            }
        }

        private void GenerateSlotsForDay(DoctorScheduleVM schedule, DateTime slotDate)
        {
            DateTime startDateTime = slotDate.Date + schedule.StartTime;

            DateTime endDateTime = slotDate.Date + schedule.EndTime;

            DateTime currentSlotStart = startDateTime;

            while (currentSlotStart < endDateTime)
            {
                DateTime currentSlotEnd = currentSlotStart.AddMinutes(schedule.SlotDurationMinutes);

                // Prevent overflow beyond schedule end
                if (currentSlotEnd > endDateTime)
                    break;

                using var conn = _db.GetConnection();
                conn.Open();
                using var tran = conn.BeginTransaction();
                try
                {
                    bool slotExists = _slotDAL.ExistsSlot(schedule.DoctorId, currentSlotStart, currentSlotEnd, conn, tran);

                    if (!slotExists)
                    {
                        AppointmentSlot slot =
                            new AppointmentSlot
                            {
                                DoctorId = schedule.DoctorId,
                                ClinicId = schedule.ClinicId,

                                SlotDate = slotDate.Date,

                                StartDateTime = currentSlotStart,
                                EndDateTime = currentSlotEnd,

                                Capacity =
                                    schedule.MaxPatientsPerSlot,

                                BookedCount = 0,

                                IsBlocked = false,

                                CreatedOn = DateTime.Now
                            };

                        int id = _slotDAL.Add(slot, conn, tran);
                        tran.Commit();
                    }
                }
                catch
                {
                    tran.Rollback();
                    throw;
                }

                currentSlotStart = currentSlotEnd;
            }
        } */
    }
}
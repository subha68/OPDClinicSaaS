using Models;
using ViewModels;
using DAL;
using System;
using System.Collections.Generic;

namespace BLL
{
    /// <summary>
    /// Responsible for generating
    /// appointment slots from doctor schedules.
    /// </summary>
    public class AppointmentSlotService
    {
        private readonly DoctorScheduleDAL _scheduleDAL;
        private readonly AppointmentSlotDAL _slotDAL;

        public AppointmentSlotService(IConfiguration config)
        {
            _scheduleDAL = new DoctorScheduleDAL(config);
            _slotDAL = new AppointmentSlotDAL(config);
        }

        #region Generate Slots

        /// <summary>
        /// Generates appointment slots
        /// for a doctor within a date range.
        /// </summary>
        public void GenerateSlots(int doctorId, DateTime fromDate, DateTime toDate)
        {
            ValidateGenerationRequest(doctorId, fromDate, toDate);

            List<DoctorScheduleVM> schedules = _scheduleDAL.GetByDoctor(doctorId);

            if (schedules == null || schedules.Count == 0)
            {
                throw new Exception("No schedules found for doctor.");
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

                    GenerateSlotsForSchedule(schedule, currentDate);
                }

                currentDate = currentDate.AddDays(1);
            }
        }

        #endregion

        #region Generate Slots For Schedule

        /// <summary>
        /// Generates slots for a single schedule/day.
        /// </summary>
        private void GenerateSlotsForSchedule(DoctorScheduleVM schedule, DateTime slotDate)
        {
            DateTime scheduleStart = slotDate.Date + schedule.StartTime;
            DateTime scheduleEnd = slotDate.Date + schedule.EndTime;
            DateTime currentSlotStart = scheduleStart;

            while (currentSlotStart < scheduleEnd)
            {
                DateTime currentSlotEnd =
                    currentSlotStart.AddMinutes(
                        schedule.SlotDurationMinutes);

                // Prevent slot overflow
                if (currentSlotEnd > scheduleEnd)
                    break;

                bool alreadyExists =
                    _slotDAL.ExistsSlot(
                        schedule.DoctorId,
                        currentSlotStart,
                        currentSlotEnd);

                // Prevent duplicate slot creation
                if (!alreadyExists)
                {
                    AppointmentSlot slot = BuildSlot(schedule, slotDate, currentSlotStart, currentSlotEnd);
                    _slotDAL.Insert(slot);
                }

                currentSlotStart = currentSlotEnd;
            }
        }

        #endregion

        #region Build Slot

        /// <summary>
        /// Creates AppointmentSlot object.
        /// </summary>
        private AppointmentSlot BuildSlot(DoctorScheduleVM schedule, DateTime slotDate, DateTime slotStart, DateTime slotEnd)
        {
            return new AppointmentSlot
            {
                DoctorId = schedule.DoctorId,
                ClinicId = schedule.ClinicId,
                SlotDate = slotDate.Date,
                StartDateTime = slotStart,
                EndDateTime = slotEnd,
                Capacity = schedule.MaxPatientsPerSlot,
                BookedCount = 0,
                IsBlocked = false,
                CreatedOn = DateTime.Now
            };
        }

        #endregion

        #region Validation

        /// <summary>
        /// Validates slot generation request.
        /// </summary>
        private void ValidateGenerationRequest(
            int doctorId,
            DateTime fromDate,
            DateTime toDate)
        {
            if (doctorId <= 0)
            {
                throw new Exception(
                    "Invalid doctor.");
            }

            if (fromDate.Date > toDate.Date)
            {
                throw new Exception(
                    "From date cannot be greater than To date.");
            }

            // Optional protection
            int totalDays =
                (toDate.Date - fromDate.Date).Days;

            if (totalDays > 365)
            {
                throw new Exception(
                    "Cannot generate slots for more than 365 days.");
            }
        }

        #endregion

        #region Delete Future Slots

        /// <summary>
        /// Optional helper method.
        /// Clears future slots before regeneration.
        /// Useful after schedule update.
        /// </summary>
        public void DeleteFutureSlots(
            int doctorId,
            DateTime fromDate)
        {
            // This can be implemented later
            // if regeneration support is needed.
        }

        #endregion

        #region Regenerate Slots

        /// <summary>
        /// Optional advanced feature.
        /// </summary>
        public void RegenerateSlots(
            int doctorId,
            DateTime fromDate,
            DateTime toDate)
        {
            // Future implementation:
            // 1. Delete future slots
            // 2. Recreate fresh slots

            GenerateSlots(
                doctorId,
                fromDate,
                toDate);
        }

        #endregion

        public List<AppointmentSlot> GetAvailableSlots(int doctorId)
        {
            return _slotDAL.GetByDoctor(doctorId);

        }

    }
}
using Models;
using ViewModels;
using DAL;
using System;
using System.Collections.Generic;

namespace BLL
{
    public class AppointmentSlotService : BaseService<AppointmentSlot>
    {
        private readonly DoctorScheduleDAL _scheduleDAL;
        private readonly AppointmentSlotDAL _slotDAL;

        public AppointmentSlotService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                        : base(httpContextAccessor)
        {
            _scheduleDAL = new DoctorScheduleDAL(config);
            _slotDAL = new AppointmentSlotDAL(config);
            _dal = _slotDAL;
        }

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

        private void GenerateSlotsForSchedule(DoctorScheduleVM schedule, DateTime slotDate)
        {
            DateTime scheduleStart = slotDate.Date + schedule.StartTime;
            DateTime scheduleEnd = slotDate.Date + schedule.EndTime;
            DateTime currentSlotStart = scheduleStart;

            while (currentSlotStart < scheduleEnd)
            {
                DateTime currentSlotEnd =
                    currentSlotStart.AddMinutes(schedule.SlotDurationMinutes);

                // Prevent slot overflow
                if (currentSlotEnd > scheduleEnd)
                    break;

                bool alreadyExists =
                    _slotDAL.ExistsSlot(schedule.DoctorId, currentSlotStart, currentSlotEnd);

                // Prevent duplicate slot creation
                if (!alreadyExists)
                {
                    AppointmentSlot slot = BuildSlot(schedule, slotDate, currentSlotStart, currentSlotEnd);
                    _dal.Add(slot);
                }

                currentSlotStart = currentSlotEnd;
            }
        }

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

        private void ValidateGenerationRequest(int doctorId, DateTime fromDate, DateTime toDate)
        {
            if (doctorId <= 0)
            {
                throw new Exception("Invalid doctor.");
            }

            if (fromDate.Date > toDate.Date)
            {
                throw new Exception("From date cannot be greater than To date.");
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

        /// Optional helper method.
        /// Clears future slots before regeneration.
        /// Useful after schedule update.

        public void DeleteFutureSlots(int doctorId, DateTime fromDate)
        {
            // This can be implemented later
            // if regeneration support is needed.
        }

        public void RegenerateSlots(int doctorId, DateTime fromDate, DateTime toDate)
        {
            // Future implementation:
            // 1. Delete future slots
            // 2. Recreate fresh slots

            GenerateSlots(doctorId, fromDate, toDate);
        }

        public List<AppointmentSlot> GetAvailableSlots(int doctorId)
        {
            return _slotDAL.GetByDoctor(doctorId);
        }

        public bool IncrementBookedCount(int slotId)
        {
            return _slotDAL.IncrementBookedCount(slotId);
        }

    }
}
using ViewModels;
using Models;
using DAL;
using System;
using System.Collections.Generic;

namespace BLL
{
    public class DoctorScheduleService
    {
        private readonly DoctorScheduleDAL _scheduleDAL;
        private readonly AppointmentSlotDAL _slotDAL;

        public DoctorScheduleService(IConfiguration config)
        {
            _scheduleDAL = new DoctorScheduleDAL(config);
            _slotDAL = new AppointmentSlotDAL(config);
        }

        #region Get Methods

        public List<DoctorScheduleVM> GetByClinic(int clinicId)
        {
            return _scheduleDAL.GetAllDoctorSchedulesByClinic(clinicId);
        }

        public List<DoctorScheduleVM> GetByDoctor(int doctorId)
        {
            return _scheduleDAL.GetByDoctor(doctorId);
        }

        #endregion

        #region Save Schedule

        /// <summary>
        /// Validates and saves doctor schedule.
        /// Handles both Insert and Update.
        /// </summary>
        public void SaveSchedule(DoctorScheduleVM model)
        {
            ValidateSchedule(model);

            bool overlapExists = _scheduleDAL.ExistsOverlap(
                model.DoctorId,
                model.DayOfWeek,
                model.StartTime,
                model.EndTime,
                model.ScheduleId > 0 ? model.ScheduleId : null);

            if (overlapExists)
            {
                throw new Exception(
                    "Schedule overlap detected for this doctor.");
            }

            if (model.ScheduleId > 0)
            {
                bool updated = _scheduleDAL.Update(model);

                if (!updated)
                {
                    throw new Exception("Unable to update schedule.");
                }
            }
            else
            {
                int scheduleId = _scheduleDAL.Insert(model);

                if (scheduleId <= 0)
                {
                    throw new Exception("Unable to save schedule.");
                }

                model.ScheduleId = scheduleId;
            }
        }

        #endregion

        #region Validate Schedule

        /// <summary>
        /// Validates schedule business rules.
        /// </summary>
        public void ValidateSchedule(DoctorScheduleVM model)
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
            TimeSpan totalDuration =
                model.EndTime - model.StartTime;

            if (totalDuration.TotalMinutes <
                model.SlotDurationMinutes)
            {
                throw new Exception(
                    "Slot duration cannot exceed schedule duration.");
            }
        }

        #endregion

        #region Delete Schedule

        public void DeleteSchedule(int scheduleId)
        {
            if (scheduleId <= 0)
            {
                throw new Exception("Invalid schedule id.");
            }

            bool deleted = _scheduleDAL.Delete(scheduleId);

            if (!deleted)
            {
                throw new Exception("Unable to delete schedule.");
            }
        }

        #endregion

        #region Generate Slots

        /// <summary>
        /// Generates appointment slots
        /// between given date range.
        /// </summary>
        public void GenerateSlots(
            int doctorId,
            DateTime fromDate,
            DateTime toDate)
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

            List<DoctorScheduleVM> schedules =
                _scheduleDAL.GetByDoctor(doctorId);

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

        #endregion

        #region Generate Slots For Day

        private void GenerateSlotsForDay(
            DoctorScheduleVM schedule,
            DateTime slotDate)
        {
            DateTime startDateTime =
                slotDate.Date + schedule.StartTime;

            DateTime endDateTime =
                slotDate.Date + schedule.EndTime;

            DateTime currentSlotStart = startDateTime;

            while (currentSlotStart < endDateTime)
            {
                DateTime currentSlotEnd =
                    currentSlotStart.AddMinutes(
                        schedule.SlotDurationMinutes);

                // Prevent overflow beyond schedule end
                if (currentSlotEnd > endDateTime)
                    break;

                bool slotExists =
                    _slotDAL.ExistsSlot(
                        schedule.DoctorId,
                        currentSlotStart,
                        currentSlotEnd);

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

                    _slotDAL.Insert(slot);
                }

                currentSlotStart =
                    currentSlotEnd;
            }
        }

        #endregion
    }
}
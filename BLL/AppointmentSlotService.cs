using Models;
using ViewModels;
using DAL;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace BLL
{
    public class AppointmentSlotService : BaseService<AppointmentSlot>
    {
        private readonly DoctorScheduleDAL _scheduleDAL;
        private readonly AppointmentSlotDAL _slotDAL;
        private readonly AppointmentDAL _appointmentDAL;
        private readonly DbHelper _db;


        public AppointmentSlotService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                        : base(config, httpContextAccessor)
        {
            _scheduleDAL = new DoctorScheduleDAL(config);
            _slotDAL = new AppointmentSlotDAL(config);
            _appointmentDAL = new AppointmentDAL(config);
            _dal = _slotDAL;
            _db = new DbHelper(config);
        }

        public void GenerateSlots(int doctorId, DateTime fromDate, DateTime toDate)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                ValidateGenerationRequest(doctorId, fromDate, toDate);

                DoctorScheduleVM schedule = _scheduleDAL.GetDoctorSchedule(doctorId);

                List<DoctorShiftVM> shifts = null;
                
                shifts = schedule.Shifts;

                if (shifts == null || shifts.Count == 0)
                {
                    throw new Exception("No schedules found for doctor.");
                }

                DateTime currentDate = fromDate.Date;

                while (currentDate <= toDate.Date)
                {
                    int currentDay = (int)currentDate.DayOfWeek;

                    foreach (DoctorShiftVM shift in shifts)
                    {
                        if (!schedule.IsActive)
                            continue;

                        if (shift.DayOfWeek != (DayOfWeek)currentDay)
                            continue;

                        GenerateSlotsForSchedule(schedule, currentDate, conn, tran);
                    }

                    currentDate = currentDate.AddDays(1);
                }

                tran.Commit();
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        private void GenerateSlotsForSchedule(DoctorScheduleVM schedule, DateTime slotDate, 
                                                SqlConnection conn, SqlTransaction tran)
        {
            DateTime scheduleStart = slotDate.Date + schedule.Shifts[0].StartTime;
            DateTime scheduleEnd = slotDate.Date + schedule.Shifts[0].EndTime;
            DateTime currentSlotStart = scheduleStart;

            while (currentSlotStart < scheduleEnd)
            {
                DateTime currentSlotEnd = currentSlotStart.AddMinutes(schedule.SlotDurationMinutes);

                // Prevent slot overflow
                if (currentSlotEnd > scheduleEnd)
                    break;

                bool alreadyExists = _slotDAL.ExistsSlot(schedule.DoctorId, currentSlotStart, currentSlotEnd, conn, tran);

                // Prevent duplicate slot creation
                if (!alreadyExists)
                {
                    AppointmentSlot slot = BuildSlot(schedule, slotDate, currentSlotStart, currentSlotEnd);
                    _dal.Add(slot, conn, tran);
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
                throw new Exception("Cannot generate slots for more than 365 days.");
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

        public List<AppointmentSlotVM> GetSlots(int doctorId, DateTime appointmentDate)
        {
            int dayOfWeek = (int)appointmentDate.DayOfWeek;
            int clinicId = ClinicId;

            DoctorScheduleVM schedule = _scheduleDAL.GetSchedulesByDay(clinicId, doctorId, dayOfWeek);
            List<Appointment> appointments = _appointmentDAL.GetAppointmentsByDate(clinicId, doctorId, appointmentDate);

            List<AppointmentSlotVM> slots = new();

            foreach(var slot in schedule.Shifts)
            {
                TimeSpan start = slot.StartTime;
                TimeSpan end = slot.EndTime;

                while(start < end)
                {
                    string slotTime = start.ToString(@"hh\:mm");

                    var booked =
                        appointments
                            .FirstOrDefault(x =>
                                x.SlotStartTime
                                .ToString(@"hh\:mm")
                                == slotTime);

                    slots.Add(
                        new AppointmentSlotVM
                        {
                            SlotTime = slotTime,
                            IsBooked = booked != null,
                            AppointmentId = booked?.AppointmentId,
                            PatientId = booked?.PatientId,
                            PatientName = booked?.PatientName
                        });

                    start = start.Add(TimeSpan.FromMinutes(15));
                }

            }

            return slots;            
        }

        public bool IncrementBookedCount(int slotId)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                bool result = _slotDAL.IncrementBookedCount(slotId, conn, tran);
                tran.Commit();
                return result;
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

    }
}
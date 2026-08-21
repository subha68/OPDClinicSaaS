using Models;
using ViewModels;
using DAL;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;

namespace BLL
{
    public class AppointmentService : BaseService<Appointment>
    {
        private readonly AppointmentDAL _appointmentDAL;
        private readonly VisitDAL _visitDAL;
        private readonly DoctorScheduleDAL _scheduleDAL;

        public AppointmentService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(config, httpContextAccessor)
        {
            _appointmentDAL = new AppointmentDAL(config);
            _visitDAL = new VisitDAL(config);
            _scheduleDAL = new DoctorScheduleDAL(config);
            _dal = _appointmentDAL;
        }

        public List<Appointment> GetTodaysAppointments(int doctorId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _appointmentDAL.GetTodaysAppointments(doctorId, conn);
            }
            catch
            {
                throw;
            }           
        }

        public List<Appointment> ListByPatient(int patientId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _appointmentDAL.ListByPatient(patientId, conn);
            }
            catch
            {
                throw;
            }
        }
        
        public void UpdateStatus(int appointmentId, string status, string cancellationReason = null)
        {
            using var conn = _db.GetConnection();
            conn.Open();
            using var tran = conn.BeginTransaction();

            try
            {
                _appointmentDAL.UpdateStatus(appointmentId, status, cancellationReason, conn, tran);
                tran.Commit();
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }

        public List<AppointmentSlotVM> GetSlots(int doctorId, DateTime appointmentDate)
        {
            int dayOfWeek = (int)appointmentDate.DayOfWeek;
            int clinicId = ClinicId;

            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                DoctorScheduleVM schedule = _scheduleDAL.GetSchedulesByDay(clinicId, doctorId, dayOfWeek, conn);
                List<Appointment> appointments = _appointmentDAL.GetAppointmentsByDate(clinicId, doctorId, appointmentDate, conn);

                List<AppointmentSlotVM> slots = new();

                foreach(var slot in schedule.Shifts)
                {
                    TimeSpan start = slot.StartTime;
                    TimeSpan end = slot.EndTime;

                    while(start < end)
                    {
                        string slotTime = start.ToString(@"hh\:mm");

                        var slotAppointments = appointments
                                            .Where(x =>
                                                x.SlotStartTime.ToString(@"hh\:mm") == slotTime)
                                            .ToList();

                        var activeAppointment = slotAppointments.FirstOrDefault(x =>
                                                    x.Status != "Cancelled");

                        var cancelledAppointments = slotAppointments
                                                        .Where(x => x.Status == "Cancelled")
                                                        .ToList();

                        bool isBooked = activeAppointment != null;

                        slots.Add(
                            new AppointmentSlotVM
                            {
                                SlotTime = slotTime,
                                IsBooked = isBooked,
                                AppointmentId = activeAppointment?.AppointmentId,
                                PatientId = activeAppointment?.PatientId,
                                PatientName = activeAppointment?.PatientName,
                                AppointmentStatus = activeAppointment?.Status ?? "Available",
                                BillingStatus = activeAppointment?.BillingStatus ?? "Unbilled",
                                PaymentStatus = activeAppointment?.PaymentStatus ?? "",

                                CancelledAppointments = cancelledAppointments
                            });

                        start = start.Add(TimeSpan.FromMinutes(15));
                    }

                }

                return slots;
            }
            catch
            {
                throw;
            }

        }

        public AppointmentBookingResult BookAppointment(AppointmentCreateVM vm)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            using var tran = conn.BeginTransaction(System.Data.IsolationLevel.Serializable);

            try
            {
                Visit visit = _visitDAL.GetByPatientAndDate(vm.PatientId, vm.AppointmentDate, conn, tran);

                if (visit == null)
                {
                    visit = new Visit
                    {
                        VisitDate = vm.AppointmentDate,
                        VisitType = vm.VisitType,
                        ClinicId = ClinicId,
                        PatientId = vm.PatientId
                    };

                    vm.VisitId = _visitDAL.Add(visit, conn, tran);
                }
                else
                {
                    vm.VisitId = visit.VisitId;
                }

                int tokenNumber = _appointmentDAL.GetNextTokenNumber(ClinicId, vm.AppointmentDate.Date, conn, tran);
                int queueNumber = _appointmentDAL.GetNextQueueNumber(ClinicId, vm.DoctorId, vm.AppointmentDate.Date, conn, tran);

                var appointment = new Appointment
                {
                    DoctorId = vm.DoctorId,
                    ClinicId = ClinicId,
                    PatientId = vm.PatientId,
                    VisitId = vm.VisitId,

                    TokenNumber = tokenNumber,
                    QueueNumber = queueNumber,

                    AppointmentDate = vm.AppointmentDate,
                    SlotTime = TimeSpan.Parse(vm.SlotTime),
                    Status = "Booked"
                };

                int appointmentId = _dal.Add(appointment, conn, tran);

                tran.Commit();

                return new AppointmentBookingResult
                {
                    AppointmentId = appointmentId,
                    TokenNumber = tokenNumber,
                    QueueNumber = queueNumber
                };
            }
            catch
            {
                tran.Rollback();
                throw;
            }
        }
    }
}
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
        private readonly PatientDAL _patientDAL;

        public AppointmentService(IConfiguration config, IHttpContextAccessor httpContextAccessor) 
                                : base(config, httpContextAccessor)
        {
            _appointmentDAL = new AppointmentDAL(config);
            _visitDAL = new VisitDAL(config);
            _scheduleDAL = new DoctorScheduleDAL(config);
            _patientDAL = new PatientDAL(config);
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

        public List<Appointment> ListByPatientAndDate(int patientId, DateTime appointmentDate)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            try
            {
                return _appointmentDAL.GetByPatientAndDate(patientId, appointmentDate, ClinicId, conn);
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

        public List<Appointment> ListActiveByPatientAndDate(int patientId, DateTime appointmentDate)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            var appointments = _appointmentDAL.GetByPatientAndDate(patientId, appointmentDate, ClinicId, conn);

            return appointments
                .Where(x => x.Status != "Cancelled")
                .ToList();
        }

        
        public void UpdateStatus(int appointmentId, string status, string? cancellationReason = null)
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
                        var appointmentStatus = activeAppointment?.Status ?? "Available";
                        var billingStatus = activeAppointment?.BillingStatus ?? "Unbilled";
                        var paymentStatus = activeAppointment?.PaymentStatus ?? "";

                        slots.Add(
                            new AppointmentSlotVM
                            {
                                SlotTime = slotTime,
                                IsBooked = isBooked,
                                AppointmentId = activeAppointment?.AppointmentId,
                                PatientId = activeAppointment?.PatientId,
                                PatientName = activeAppointment?.PatientName,
                                AppointmentStatus = appointmentStatus,
                                BillingStatus = billingStatus,
                                PaymentStatus = paymentStatus,
                              //  SlotStatus = StatusMap(appointmentStatus, billingStatus, paymentStatus),

                                TokenNumber = activeAppointment?.TokenNumber,
                                QueueNumber = activeAppointment?.QueueNumber,

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

                Patient patient = _patientDAL.Get(vm.PatientId, ClinicId, conn, tran);

                if (patient != null)
                {
                    PatientId = patient.PatientId;
                    PatientDisplayName = patient.FullName;
                    PatientGender = patient.Gender;
                    PatientAge = patient.Age;
                    PatientPhone = patient.Phone;
                }                

                tran.Commit();

                return new AppointmentBookingResult
                {
                    success = true,
                    AppointmentId = appointmentId,
                    TokenNumber = tokenNumber,
                    QueueNumber = queueNumber,
                    Patient = patient
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
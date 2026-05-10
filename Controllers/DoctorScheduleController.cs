using Microsoft.Extensions.Configuration;
using BLL;
using DAL;
using Models;
using ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace Controllers
{
    //[AuthorizeFeature("DOCTOR_SCHEDULE")]
    public class DoctorScheduleController : Controller
    {
        private readonly SessionHelper _session;
        private readonly DoctorScheduleService _scheduleService;
        private readonly AppointmentSlotService _slotService;
        private readonly DoctorService _doctor;

        public DoctorScheduleController(IConfiguration config, SessionHelper session)
        {
            _scheduleService = new DoctorScheduleService(config);
            _slotService = new AppointmentSlotService(config);
            _doctor = new DoctorService(config);
            _session = session;
        }

        #region Index

        /// <summary>
        /// Displays all schedules.
        /// </summary>
        public IActionResult Index(int? doctorId)
        {
            List<DoctorScheduleVM> schedules;

            if (doctorId.HasValue && doctorId.Value > 0)
            {
                schedules = _scheduleService.GetByDoctor(doctorId.Value);
            }
            else
            {
                int clinicId = Convert.ToInt32(_session.ClinicId);
                schedules = _scheduleService.GetByClinic(clinicId);
            }

            return View(schedules);
        }

        #endregion

        #region Create GET

        /// <summary>
        /// Loads create schedule screen.
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            int clinicId = Convert.ToInt32(_session.ClinicId);
            LoadDoctors(clinicId);

            DoctorScheduleVM model =
                new DoctorScheduleVM
                {
                    IsActive = true,
                    SlotDurationMinutes = 15,
                    MaxPatientsPerSlot = 1
                };

            return View(model);
        }

        #endregion

        #region Create POST / Save

        /// <summary>
        /// Saves doctor schedule.
        /// Handles both insert and update.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(DoctorScheduleVM model)
        {
            try
            {
                model.ClinicId = Convert.ToInt32(_session.ClinicId);
                LoadDoctors();

                if (!ModelState.IsValid)
                {
                    return View("Create", model);
                }

                // Example:
                // model.ClinicId = LoggedInClinicId;

                _scheduleService.SaveSchedule(model);

                TempData["SuccessMessage"] = "Doctor schedule saved successfully.";

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                LoadDoctors();
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Create", model);
            }
        }

        #endregion

        #region Generate Slots

        /// <summary>
        /// Generates slots for doctor.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GenerateSlots(int doctorId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                _slotService.GenerateSlots(doctorId, fromDate, toDate);

                TempData["SuccessMessage"] = "Appointment slots generated successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Loads doctor dropdown.
        /// </summary>
        private void LoadDoctors(int ClinicId = 0)
        {
            var doctors = _doctor.GetDoctors(ClinicId) ?? new List<DoctorVM>();

            ViewBag.Doctors =
                new SelectList(
                    doctors,
                    "DoctorId",
                    "FirstName",
                    "LastName");
        }

        #endregion
    }
}
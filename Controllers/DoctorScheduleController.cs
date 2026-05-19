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
        private readonly DoctorScheduleService _schedule;
        private readonly AppointmentSlotService _slot;
        private readonly DoctorService _doctor;

        public DoctorScheduleController(IConfiguration config, SessionHelper session)
        {
            _schedule = new DoctorScheduleService(config);
            _slot = new AppointmentSlotService(config);
            _doctor = new DoctorService(config);
            _session = session;
        }

        public IActionResult Index(int? doctorId)
        {
            List<DoctorScheduleVM> schedules;

            if (doctorId.HasValue && doctorId.Value > 0)
            {
                schedules = _schedule.GetByDoctor(doctorId.Value);
            }
            else
            {
                _schedule.clinicId = Convert.ToInt32(_session.ClinicId);
                schedules = _schedule.List();
            }

            return View(schedules);
        }

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

                _schedule.Add(model);

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GenerateSlots(int doctorId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                _slot.GenerateSlots(doctorId, fromDate, toDate);

                TempData["SuccessMessage"] = "Appointment slots generated successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        private void LoadDoctors(int ClinicId = 0)
        {
            _doctor.clinicId = ClinicId;
            var doctors = _doctor.List() ?? new List<DoctorVM>();

            ViewBag.Doctors =
                new SelectList(
                    doctors,
                    "DoctorId",
                    "FirstName",
                    "LastName");
        }

    }
}
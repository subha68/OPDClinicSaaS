using Microsoft.Extensions.Configuration;
using BLL;
using DAL;
using Models;
using ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using Helpers;

namespace Controllers
{
    //[AuthorizeFeature("DOCTOR_SCHEDULE")]
    public class DoctorScheduleController : Controller
    {
        private readonly DoctorScheduleService _schedule;
        private readonly AppointmentSlotService _slot;
        private readonly DoctorService _doctor;

        public DoctorScheduleController(DoctorScheduleService schedule, 
                                        AppointmentSlotService slot, 
                                        DoctorService doctor)
        {
            _schedule = schedule;
            _slot = slot;
            _doctor = doctor;
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
                schedules = _schedule.List();
            }

            return View(schedules);
        }

        [HttpGet]
        public IActionResult Create()
        {
            LoadDoctors();

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
                model.ClinicId = _schedule.ClinicId;
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

        private void LoadDoctors()
        {
            var doctors = _doctor.List() ?? new List<Doctor>();

            ViewBag.Doctors =
                new SelectList(
                    doctors,
                    "DoctorId",
                    "FirstName",
                    "LastName");
        }

    }
}
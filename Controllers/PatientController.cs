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
    public class PatientController : Controller
    {
        private readonly SessionHelper _session;
        private readonly PatientService _patient;
        private readonly AppointmentService _appointment;
        private readonly ConsultationService _consultation;

        public PatientController(IConfiguration config, SessionHelper session)
        {
            _patient = new PatientService(config);
            _session = session;
        }

        [HttpGet]
        public JsonResult SearchByMobile(string mobile)
        {
            var patient = _patient.GetByMobile(mobile);

            if (patient == null)
                return Json(null);

            return Json(new
            {
                patientId = patient.PatientId,
                name = patient.FirstName + " " + patient.LastName,
                mobile = patient.Phone
            });
        }

        [HttpGet]
        public IActionResult Search()
        {
            return View(new PatientSearchVM());
        }

        [HttpPost]
        public IActionResult Search(PatientSearchVM vm)
        {
            int clinicId = _session.ClinicId;

            vm.Results = _patient.Search(
                                vm.PatientId,
                                vm.Phone,
                                vm.Name,
                                clinicId);

            return View(vm);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new Patient();

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(Patient model)
        {
            if (model == null)
            {
                return Content("Model is NULL");
            }

            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var err in errors)
                    {
                        Console.WriteLine($"Field: {key} → Error: {err.ErrorMessage}");
                    }
                }

                return View(model);
            }

            _patient.Add(model);

            return RedirectToAction("Search");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var patient = _patient.Get(id);
            if (patient == null)
            {
                return NotFound();
            }
            return View("Create", patient);
        }

        [HttpPost]
        public IActionResult Edit(Patient patient)
        {
            if (patient == null)
            {
                return Content("Model is NULL");
            }

            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var err in errors)
                    {
                        Console.WriteLine($"Field: {key} → Error: {err.ErrorMessage}");
                    }
                }

                return View(patient);
            }

            _patient.Update(patient);

            return RedirectToAction("Search");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _patient.Delete(id);
            return RedirectToAction("Search");
        }

        public IActionResult History(int patientId)
        {
            var patient = _patient.Get(patientId);

            if (patient == null)
                return NotFound();

            var vm = new PatientHistoryVM
            {
                Patient = patient,
                Appointments = _appointment.GetByPatientId(patientId),
                Consultations = _consultation.GetConsultationsByPatientId(patientId),
                Prescriptions = _consultation.GetPrescriptionsByPatientId(patientId)
            };

            return View(vm);
        }
      

    }

}
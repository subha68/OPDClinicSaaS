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
    public class PatientController : Controller
    {
        private readonly PatientService _patient;
        private readonly AppointmentService _appointment;
        private readonly ConsultationService _consultation;

        public PatientController(PatientService patient, 
                                    AppointmentService appointment, 
                                    ConsultationService consultation)
        {
            _patient = patient;
            _appointment = appointment;
            _consultation = consultation;
        }

        public ActionResult Index(string? SearchString)
        {
            var vm = new PatientSearchVM();
            vm.SearchString = SearchString ?? "";

            vm.Results = _patient.Search(vm);

            return View(vm);
        }

        [HttpGet]
        public IActionResult Search(string? returnTo, int? departmentId, int? doctorId, DateTime? appointmentDateTime)
        {
            var vm = new PatientSearchVM
            {
                ClinicId = _patient.ClinicId,
                ReturnTo = returnTo,
                DepartmentId = departmentId,
                DoctorId = doctorId,
                AppointmentDateTime = appointmentDateTime
            };

            return View(vm);
        }

        [HttpPost]
        public IActionResult Search(PatientSearchVM vm)
        {
            vm.ClinicId = _patient.ClinicId;
            vm.Results = _patient.Search(vm);

            if(vm.Results.Count == 1)
            {
                return RedirectToAction(
                    "SelectPatient",
                    new
                    {
                        patientId = vm.Results[0].PatientId,
                        returnTo = vm.ReturnTo,
                        departmentId = vm.DepartmentId,
                        doctorId = vm.DoctorId,
                        appointmentDateTime = vm.AppointmentDateTime,
                        LastVisitDate = vm.LastVisitDate
                    });
            }

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

            model.ClinicId = _patient.ClinicId;

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

            _patient.Create(model);

            return RedirectToAction("Index");
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

            patient.ClinicId = _patient.ClinicId;

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

                return View("Create", patient);
            }

            _patient.Modify(patient);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _patient.Remove(id);
            return RedirectToAction("Index");
        }

        public IActionResult History(int id, int page = 1)
        {
            var vm = _patient.GetHistory(id, page);
            
            return View(vm);
        }
      
        public IActionResult SelectPatient(
            string patientId,
            string returnTo,
            int? departmentId,
            int? doctorId,
            DateTime? appointmentDateTime,
            int? visitId)
        {
            switch (returnTo)
            {
                case "AppointmentBooking":

                    return RedirectToAction(
                        "Book",
                        "AppointmentBooking",
                        new
                        {
                            departmentId,
                            doctorId,
                            appointmentDateTime,
                            patientId
                        });

            /*    case "Consultation":

                    return RedirectToAction(
                        "PatientVisits",
                        "Consultation",
                        new
                        {
                            patientId
                        }); */

                case "Billing":

                    return RedirectToAction(
                        "SearchByPatient",
                        "Billing",
                        new
                        {
                            patientId
                        });

                case "PatientHistory":

                    return RedirectToAction(
                        "History",
                        "Patient",
                        new
                        {
                            patientId
                        });

                default:
                    return RedirectToAction("Index", "Patient");
            }
        }

        [HttpGet]
        public JsonResult SearchPatient(string term)
        {
            var patients = _patient.Search(term)
                                        .Take(10)
                                        .Select(x => new
                                        {
                                            id = x.PatientId,
                                            text = "ID: " + x.PatientId + " - " + x.FirstName + " " + x.LastName + 
                                                            " (" + x.Phone + ")"
                                        })
                                        .ToList();

            return Json(patients);
        }
    }

}
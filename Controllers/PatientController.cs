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
            var vm = new PatientSearchVM
            {
                ClinicId = _patient.ClinicId,
                SearchString = SearchString?.Trim() ?? ""
            };

            // Do not retrieve the complete patient population
            // when the page is opened without a search term.
            if (!string.IsNullOrWhiteSpace(vm.SearchString))
            {
                vm.Results = _patient.Search(vm);
            }

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
                        appointmentDateTime = vm.AppointmentDateTime
                    });
            }

            return View(vm);
        }
       
        [HttpGet]
        public IActionResult Create(string? searchString, string? returnTo, int? departmentId, int? doctorId,
                                    DateTime? appointmentDateTime)
        {
            var model = new Patient();

            // Pre-populate the phone number when the failed
            // search looks like a phone number.
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                searchString = searchString.Trim();

                if (searchString.All(char.IsDigit))
                {
                    model.Phone = searchString;
                }
            }

            // Preserve the originating workflow.
            ViewBag.ReturnTo = returnTo;
            ViewBag.DepartmentId = departmentId;
            ViewBag.DoctorId = doctorId;
            ViewBag.AppointmentDateTime = appointmentDateTime;
            ViewBag.SearchString = searchString;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Patient model, string? returnTo, int? departmentId, int? doctorId,
                                    DateTime? appointmentDateTime)
        {
            if (model == null)
            {
                return Content("Model is NULL");
            }

            model.ClinicId = _patient.ClinicId;

            if (!ModelState.IsValid)
            {
                // Preserve workflow context when returning to the form.
                ViewBag.ReturnTo = returnTo;
                ViewBag.DepartmentId = departmentId;
                ViewBag.DoctorId = doctorId;
                ViewBag.AppointmentDateTime = appointmentDateTime;

                return View(model);
            }

            // PatientService.Create() also places the newly
            // created patient into the global patient cache.
            _patient.Create(model);

            return ReturnToWorkflow(returnTo, model.PatientId, departmentId, doctorId, appointmentDateTime);
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

        [HttpGet]
        public JsonResult SearchPatient(string? term)
        {
            string searchTerm = term?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Json(new List<object>());
            }

            var patients = _patient.Search(searchTerm)
                .Select(x => new
                {
                    id = x.PatientId,
                    text = "ID: " + x.PatientId
                        + " - " + x.FirstName + " " + x.LastName
                        + " (" + x.Phone + ")",

                    patientName = x.FirstName + " " + x.LastName,
                    phone = x.Phone,
                    age = x.Age,
                    gender = x.Gender,
                    lastVisitDate = x.LastVisitDate
                })
                .ToList();

            return Json(patients);
        }

        [HttpGet]
        public IActionResult GetSelectedPatient()
        {
            PatientSelectionVM p = _patient.GetCache();

            if (p == null || p.PatientId <= 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Please select a patient first."
                });
            }

            return Json(new
            {
                success = true,
                patient = new
                {
                    id = p.PatientId,
                    patientId = p.PatientId,
                    patientName = p.FullName,
                    fullName = p.FullName,
                    gender = p.Gender,
                    age = p.Age,
                    phone = p.Phone
                }
            });

        }

        [HttpPost]
        public IActionResult SetSelectedPatient([FromBody] PatientSelectionVM patient)
        {
            if (patient == null || patient.PatientId <= 0)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid patient."
                });
            }

        //    var patient = _patient.Get(patientId);

            var response = _patient.SetCache(patient);

            return Json(new
            {
                success = true,
                patient = new
                {
                    id = patient.PatientId,
                    patientName = patient.FullName,
                    gender = patient.Gender,
                    age = patient.Age,
                    phone = patient.Phone
                }
            });
        }

        [HttpGet]
        public IActionResult SelectPatient(int patientId, string? returnTo, int? departmentId, int? doctorId,
                                            DateTime? appointmentDateTime, DateTime? lastVisitDate)
        {
            var patient = _patient.Get(patientId);

            if (patient == null)
            {
                return NotFound();
            }

            if (patient.ClinicId != _patient.ClinicId)
            {
                return Forbid();
            }

            var selection = new PatientSelectionVM(patient)
            {
                LastVisitDate = lastVisitDate
            };

            _patient.SetCache(selection);

            return ReturnToWorkflow(returnTo, patient.PatientId, departmentId, doctorId, appointmentDateTime);
        }

        private IActionResult ReturnToWorkflow(string? returnTo, int patientId, int? departmentId, int? doctorId,
                                                DateTime? appointmentDateTime)
        {
            switch (returnTo?.Trim())
            {
                case "AppointmentBooking":

                    if (!departmentId.HasValue ||
                        !doctorId.HasValue ||
                        !appointmentDateTime.HasValue)
                    {
                        return RedirectToAction(
                            "Index",
                            "AppointmentBooking");
                    }

                    return RedirectToAction(
                        "Book",
                        "AppointmentBooking",
                        new
                        {
                            departmentId = departmentId.Value,
                            doctorId = doctorId.Value,
                            appointmentDateTime = appointmentDateTime.Value,
                            patientId = patientId
                        });

                case "Billing":

                    return RedirectToAction(
                        "SearchByPatient",
                        "Billing",
                        new
                        {
                            patientId = patientId
                        });

                default:

                    return RedirectToAction(nameof(Index));
            }
        }
    
    }

}
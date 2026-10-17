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
    public class MedicineController : Controller
    {
        private readonly MedicineService _medicine;

        public MedicineController(MedicineService medicine)
        {
            _medicine = medicine;
        }

        public IActionResult Index()
        {
            List<Medicine> medicines = _medicine.List();
            return View(medicines);
        }

        public IActionResult Create()
        {
            return View(new Medicine());
        }

        [HttpPost]
        public IActionResult Create(Medicine medicine)
        {
            if (ModelState.IsValid)
            {
                _medicine.Create(medicine);
                return RedirectToAction("Index");
            }

            return View(medicine);
        }

        public IActionResult Edit(int id)
        {
            var medicine = _medicine.Get(id);
            if (medicine == null)
            {
                return NotFound();
            }
            return View("Create", medicine);
        }

        [HttpPost]
        public IActionResult Edit(Medicine medicine)
        {
            if (medicine == null)
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

                return View("Create", medicine);
            }

            _medicine.Modify(medicine);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _medicine.Remove(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult SearchMedicine(string term)
        {
            var medicines =
                _medicine.Search(term)
                        .Take(10)
                        .Select(x => new
                        {
                            id = x.MedicineId,
                            label = x.MedicineName,
                            value = x.MedicineName,
                            dosageForm = x.DosageForm,
                            scheduleType = x.ScheduleType,
                            genericName = x.GenericName
                        });

            return Json(medicines);
        }

    }

}
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
        private PatientService _service;

        public PatientController(IConfiguration config, SessionHelper session)
        {
            _service = new PatientService(config);
            _session = session;
        }

        [HttpGet]
        public JsonResult SearchByMobile(string mobile)
        {
            var patient = _service.GetByMobile(mobile);

            if (patient == null)
                return Json(null);

            return Json(new
            {
                patientId = patient.PatientId,
                name = patient.FirstName + " " + patient.LastName,
                mobile = patient.Phone
            });
        }

    }

}
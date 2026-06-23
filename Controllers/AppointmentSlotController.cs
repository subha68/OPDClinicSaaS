using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using DAL;
using BLL;
using Helpers;

namespace Controllers   
{
    public class AppointmentSlotController : Controller
    {
        private readonly AppointmentSlotService _slot;

        public AppointmentSlotController(AppointmentSlotService slot)
        {
            _slot = slot;
        }

/*        public IActionResult Index(int doctorId)
        {
            var slots = _slot.GetAvailableSlots(doctorId);

            return View(slots);
        } */
    }
}
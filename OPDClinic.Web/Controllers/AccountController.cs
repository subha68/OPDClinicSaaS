using System.Web.Mvc;
using OPDClinic.Web.DAL;
using OPDClinic.Web.Models;

namespace OPDClinic.Web.Controllers
{
    public class AccountController : Controller
    {
        UserDAL userDAL = new UserDAL();

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            var user = userDAL.ValidateUser(username, password);

            if (user != null)
            {
                Session["UserId"] = user.UserId;
                Session["RoleId"] = user.RoleId;
                Session["ClinicId"] = user.ClinicId;

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid credentials";
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
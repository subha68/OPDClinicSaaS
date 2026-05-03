using System.Web.Mvc;

namespace OPDClinic.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }
    }
}
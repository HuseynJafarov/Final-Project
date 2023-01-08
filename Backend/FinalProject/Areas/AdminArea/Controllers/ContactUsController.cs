using Microsoft.AspNetCore.Mvc;

namespace FinalProject.Areas.AdminArea.Controllers
{
    public class ContactUsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

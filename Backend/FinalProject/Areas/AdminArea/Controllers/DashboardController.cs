using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace FinalProject.Areas.AdminArea.Controllers
{
    public class DashboardController : Controller
    {
        [Area("AdminArea")]
        //[Authorize(Roles = "SuperAdmin,Admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}

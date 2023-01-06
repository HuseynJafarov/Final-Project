using FinalProject.Data;
using FinalProject.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class AboutTopController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AboutTopController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async  Task<IActionResult> Index()
        {
            AboutTop aboutTop = await _context.AboutTops.Where(m => !m.IsDeleted).FirstOrDefaultAsync();
            ViewBag.count = await _context.AboutBottoms.Where(m => !m.IsDeleted).CountAsync();
            return View(aboutTop); ;
        }

        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id is null) return BadRequest();

                AboutTop aboutTop = await _context.AboutTops.FirstOrDefaultAsync(m => m.Id == id);

                if (aboutTop is null) return NotFound();

                return View(aboutTop);

            }
            catch (Exception ex)
            {

                ViewBag.Message = ex.Message;
                return View();
            }

        }

        public IActionResult Create() => View();

    }
}

using FinalProject.Data;
using FinalProject.Helpers.Enums;
using FinalProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class TellUsController : Controller
    {
        private readonly AppDbContext _context;
        public TellUsController(AppDbContext context)
        {
            _context= context;
        }
        public async Task<IActionResult> Index()
        {
            List<TellUs> tellUs = await _context.TellUs.Where(m=> !m.IsDeleted).ToListAsync();

            return View(tellUs);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            TellUs tellUs = await _context.TellUs.FirstOrDefaultAsync(m => m.Id == id);

            tellUs.IsDeleted = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

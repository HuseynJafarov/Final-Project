using FinalProject.Data;
using FinalProject.Models;
using FinalProject.Services;
using FinalProject.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Controllers
{
    public class AboutController : Controller
    {
        private readonly AppDbContext _context;
        public AboutController(AppDbContext context)
        {
            _context = context;
            
        }
        public async Task<IActionResult> Index()
        {
            AboutBottom aboutBottom = await _context.AboutBottoms.Where(m => !m.IsDeleted).FirstOrDefaultAsync();
            AboutTop aboutTop = await _context.AboutTops.Where(m => !m.IsDeleted).FirstOrDefaultAsync();
            IEnumerable<AboutLi> aboutLi = await _context.AboutLis.Where(m => !m.IsDeleted).ToListAsync();

            AboutVM model = new AboutVM
            {
                AboutBottom= aboutBottom,
                AboutTop= aboutTop,
                AboutLi= aboutLi
            };

            return View(model);
        }
    }
}

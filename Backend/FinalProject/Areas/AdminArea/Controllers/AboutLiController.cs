using FinalProject.Data;
using FinalProject.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class AboutLiController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AboutLiController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<AboutLi> li = await _context.AboutLis
              .Where(m => !m.IsDeleted).ToListAsync();
            ViewBag.count = await _context.AboutLis.Where(m => !m.IsDeleted).CountAsync();


            return View(li);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AboutLi aboutLi)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }


                bool isExist = await _context.AboutLis.AnyAsync(m => m.Text.Trim() == aboutLi.Text.Trim());

                if (isExist)
                {
                    ModelState.AddModelError("Description", "About Text already exist");
                    return View();
                }

                await _context.AboutLis.AddAsync(aboutLi);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {

                ViewBag.Message = ex.Message;
                return View();
            }

        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return BadRequest();

            AboutLi aboutLi = await _context.AboutLis.FindAsync(id);

            if (aboutLi == null) return NotFound();

            return View(aboutLi);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            AboutLi aboutLi = await _context.AboutLis.FirstOrDefaultAsync(m => m.Id == id);

            aboutLi.IsDeleted = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id is null) return BadRequest();

                AboutLi aboutLi = await _context.AboutLis.FirstOrDefaultAsync(m => m.Id == id);

                if (aboutLi is null) return NotFound();

                return View(aboutLi);

            }
            catch (Exception ex)
            {

                ViewBag.Message = ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AboutLi aboutLi)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(aboutLi);
                }

                AboutLi dbAboutLi = await _context.AboutLis.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

                if (dbAboutLi is null) return NotFound();

                if (dbAboutLi.Text.ToLower().Trim() == aboutLi.Text.ToLower().Trim())
                {
                    return RedirectToAction(nameof(Index));
                }

                // dbCategory.Name = category.Name;

                _context.AboutLis.Update(aboutLi);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {

                ViewBag.Message = ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetStatus(int id)
        {
            List<AboutLi> dbModel = await _context.AboutLis.Where(m => m.IsActive).ToListAsync();

            if (dbModel.Count < 1)
            {
                AboutLi model = await _context.AboutLis.FirstOrDefaultAsync(m => m.Id == id);

                if (model is null) return NotFound();

                if (model.IsActive)
                {
                    model.IsActive = false;
                }
                else
                {
                    model.IsActive = true;
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                AboutLi model = await _context.AboutLis.FirstOrDefaultAsync(m => m.Id == id);
                if (model.IsActive)
                {
                    model.IsActive = false;
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }


        }

    }
}

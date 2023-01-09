using FinalProject.Data;
using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    public class FooterCategoryController : Controller
    {
        private readonly AppDbContext _context;

        public FooterCategoryController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<FooterCategory> categories = await _context.FooterCategories.Where(m => !m.IsDeleted).AsNoTracking().OrderByDescending(m => m.Id).ToListAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create() => View();
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FooterCategory category)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }


                bool isExist = await _context.FooterCategories.AnyAsync(m => m.Name.Trim() == category.Name.Trim());

                if (isExist)
                {
                    ModelState.AddModelError("Name", "Category already exist");
                    return View();
                }

                await _context.FooterCategories.AddAsync(category);

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
        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return BadRequest();

            FooterCategory category = await _context.FooterCategories.FindAsync(id);

            if (category == null) return NotFound();

            return View(category);
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            FooterCategory category = await _context.FooterCategories.FirstOrDefaultAsync(m => m.Id == id);

            category.IsDeleted = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id is null) return BadRequest();

                FooterCategory category = await _context.FooterCategories.FirstOrDefaultAsync(m => m.Id == id);

                if (category is null) return NotFound();

                return View(category);

            }
            catch (Exception ex)
            {

                ViewBag.Message = ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FooterCategory category)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(category);
                }

                FooterCategory dbCategory = await _context.FooterCategories.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

                if (dbCategory is null) return NotFound();

                if (dbCategory.Name.ToLower().Trim() == category.Name.ToLower().Trim())
                {
                    return RedirectToAction(nameof(Index));
                }

                // dbCategory.Name = category.Name;

                _context.FooterCategories.Update(category);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {

                ViewBag.Message = ex.Message;
                return View();
            }
        }
    }
}

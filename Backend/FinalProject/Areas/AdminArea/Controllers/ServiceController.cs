using FinalProject.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using FinalProject.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using FinalProject.Helpers.Enums;
using Microsoft.AspNetCore.Authorization;

namespace FinalProject.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class ServiceController : Controller
    {
        private readonly AppDbContext _context;
        public ServiceController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<Service> services = await _context.Services
              .Where(m => !m.IsDeleted).ToListAsync();
            return View(services);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Service services)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }


                bool isExist = await _context.Services.AnyAsync(m => m.Title.Trim() == services.Title.Trim()
                && m.Description.Trim() == services.Description.Trim() && m.Icon.Trim() == services.Icon.Trim());


                if (isExist)
                {
                    ModelState.AddModelError("Description", "About Text already exist");
                    return View();
                }

                await _context.Services.AddAsync(services);

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

            Service services = await _context.Services.FindAsync(id);

            if (services == null) return NotFound();

            return View(services);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            Service services = await _context.Services.FirstOrDefaultAsync(m => m.Id == id);

            services.IsDeleted = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id is null) return BadRequest();

                Service services = await _context.Services.FirstOrDefaultAsync(m => m.Id == id);

                if (services is null) return NotFound();

                return View(services);

            }
            catch (Exception ex)
            {

                ViewBag.Message = ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Service services)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(services);
                }

                Service dbServices = await _context.Services.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

                if (dbServices is null) return NotFound();

                if (dbServices.Title.ToLower().Trim() == services.Title.ToLower().Trim() &&
                    dbServices.Description.ToLower().Trim() == services.Description.ToLower().Trim() &&
                    dbServices.Icon.ToLower().Trim() == services.Icon.ToLower().Trim())
                {
                    return RedirectToAction(nameof(Index));
                }


                _context.Services.Update(services);

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
            List<Service> dbModel = await _context.Services.Where(m => m.IsActive).ToListAsync();

            if (dbModel.Count < 10)
            {
                Service model = await _context.Services.FirstOrDefaultAsync(m => m.Id == id);

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
                Service model = await _context.Services.FirstOrDefaultAsync(m => m.Id == id);
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

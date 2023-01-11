using FinalProject.Data;
using FinalProject.Helpers.Enums;
using FinalProject.Models;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class ContactInfoController : Controller
    {
        private readonly AppDbContext _context;
        public ContactInfoController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<ContactInfo> info = await _context.ContactInfos
              .Where(m => !m.IsDeleted).ToListAsync();
            return View(info);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContactInfo info)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }


                bool isExist = await _context.ContactInfos.AnyAsync(m => m.Title.Trim() == info.Title.Trim()
                && m.Description.Trim() == info.Description.Trim() && m.Icon.Trim() == info.Icon.Trim());


                if (isExist)
                {
                    ModelState.AddModelError("Description", "About Text already exist");
                    return View();
                }

                await _context.ContactInfos.AddAsync(info);

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

            ContactInfo info = await _context.ContactInfos.FindAsync(id);

            if (info == null) return NotFound();

            return View(info);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            ContactInfo info = await _context.ContactInfos.FirstOrDefaultAsync(m => m.Id == id);

            info.IsDeleted = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id is null) return BadRequest();

                ContactInfo info = await _context.ContactInfos.FirstOrDefaultAsync(m => m.Id == id);

                if (info is null) return NotFound();

                return View(info);

            }
            catch (Exception ex)
            {

                ViewBag.Message = ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ContactInfo info)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(info);
                }

                ContactInfo dbInfo = await _context.ContactInfos.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

                if (dbInfo is null) return NotFound();

                if (dbInfo.Title.ToLower().Trim() == dbInfo.Title.ToLower().Trim() && 
                    dbInfo.Description.ToLower().Trim() == info.Description.ToLower().Trim() &&
                    dbInfo.Icon.ToLower().Trim() == info.Icon.ToLower().Trim())
                {
                    return RedirectToAction(nameof(Index));
                }


                _context.ContactInfos.Update(info);

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
            List<ContactInfo> dbModel = await _context.ContactInfos.Where(m => m.IsActive).ToListAsync();

            if (dbModel.Count < 10)
            {
                ContactInfo model = await _context.ContactInfos.FirstOrDefaultAsync(m => m.Id == id);

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
                ContactInfo model = await _context.ContactInfos.FirstOrDefaultAsync(m => m.Id == id);
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

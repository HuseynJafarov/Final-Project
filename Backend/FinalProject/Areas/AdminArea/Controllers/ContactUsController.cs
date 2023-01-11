using FinalProject.Data;
using FinalProject.Helpers;
using FinalProject.Models;
using FinalProject.ViewModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using FinalProject.Helpers.Enums;
using Microsoft.AspNetCore.Authorization;

namespace FinalProject.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class ContactUsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ContactUsController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<ContactUs> contact = await _context.ContactUs.Where(m => !m.IsDeleted).ToListAsync();
            return View(contact);
        }

        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id is null) return BadRequest();

                ContactUs contact = await _context.ContactUs.FirstOrDefaultAsync(m => m.Id == id);

                if (contact is null) return NotFound();

                return View(contact);

            }
            catch (Exception ex)
            {

                ViewBag.Message = ex.Message;
                return View();
            }

        }

        public IActionResult Create() => View();


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ContactUsCreateVM createVM)
        {
            if (!ModelState.IsValid) return View(createVM);

            if (createVM.Photo != null)
            {
                if (!createVM.Photo.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Photo", "Please choose correct image type");
                    return View(createVM);
                }

                if (!createVM.Photo.CheckFileSize(500))
                {
                    ModelState.AddModelError("Photo", "Please choose correct image size");
                    return View(createVM);
                }

                string fileName = Guid.NewGuid().ToString() + "_" + createVM.Photo.FileName;

                string pathh = Helper.GetFilePath(_env.WebRootPath, "assets/images/subpage/contact", fileName);

                await Helper.SaveFile(pathh, createVM.Photo);

                ContactUs contact = new ContactUs
                {
                    Image = fileName,
                    Description = createVM.Description,
                    Title = createVM.Title,
                };

                await _context.ContactUs.AddAsync(contact);

            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id is null) return BadRequest();

                ContactUs contact = await _context.ContactUs.FirstOrDefaultAsync(m => m.Id == id);

                if (contact is null) return NotFound();

                return View(contact);

            }
            catch (Exception ex)
            {

                ViewBag.Message = ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ContactUs contact)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(contact);
                }

                if (contact.Photo != null)
                {
                    if (!contact.Photo.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photo", "Please choose correct image type");
                        return View();
                    }

                    if (!contact.Photo.CheckFileSize(200))
                    {
                        ModelState.AddModelError("Photo", "Please choose correct image size");
                        return View();
                    }

                    string fileName = Guid.NewGuid().ToString() + "_" + contact.Photo.FileName;

                    ContactUs dbContact = await _context.ContactUs.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

                    if (dbContact is null) return NotFound();

                    if (dbContact.Title.Trim().ToLower() == contact.Title.Trim().ToLower()
                        && dbContact.Description.Trim().ToLower() == contact.Description.Trim().ToLower()
                        && dbContact.Photo == contact.Photo)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    string path = Helper.GetFilePath(_env.WebRootPath, "assets/images/subpage/contact", fileName);

                    using (FileStream stream = new FileStream(path, FileMode.Create))
                    {
                        await contact.Photo.CopyToAsync(stream);
                    }

                    contact.Image = fileName;

                    _context.ContactUs.Update(contact);

                    await _context.SaveChangesAsync();

                    string dbPath = Helper.GetFilePath(_env.WebRootPath, "assets/images/subpage/contact", dbContact.Image);

                    Helper.DeleteFile(dbPath);
                }

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
        public async Task<IActionResult> Delete(int id)
        {
            ContactUs contact = await _context.ContactUs
               .Where(m => !m.IsDeleted && m.Id == id)
               .FirstOrDefaultAsync();

            if (contact == null) return NotFound();

            contact.IsDeleted = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetStatus(int id)
        {
            List<ContactUs> dbModel = await _context.ContactUs.Where(m => m.IsActive).ToListAsync();

            if (dbModel.Count < 10)
            {
                ContactUs model = await _context.ContactUs.FirstOrDefaultAsync(m => m.Id == id);

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
                ContactUs model = await _context.ContactUs.FirstOrDefaultAsync(m => m.Id == id);
                if (model.IsActive)
                {
                    model.IsActive = false;
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }


        }

        private async Task<List<ContactUs>> GetByIdToLIstAsync(int id)
        {
            return await _context.ContactUs.Where(m => !m.IsDeleted && m.Id == id).ToListAsync();
        }

        private async Task<ContactUs> GetByIdAsync(int id)
        {
            return await _context.ContactUs.Where(m => !m.IsDeleted && m.Id == id).FirstOrDefaultAsync();
        }
    }
}

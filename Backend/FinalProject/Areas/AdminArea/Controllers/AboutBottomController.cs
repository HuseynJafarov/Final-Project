using FinalProject.Data;
using FinalProject.Helpers;
using FinalProject.Helpers.Enums;
using FinalProject.Models;
using FinalProject.ViewModel.AboutBottomViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class AboutBottomController : Controller
    {
        
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AboutBottomController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<AboutBottom> aboutBottoms = await _context.AboutBottoms.Where(m => !m.IsDeleted).ToListAsync();
            return View(aboutBottoms);
        }

        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id is null) return BadRequest();

                AboutBottom aboutBottom = await _context.AboutBottoms.FirstOrDefaultAsync(m => m.Id == id);

                if (aboutBottom is null) return NotFound();

                return View(aboutBottom);

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
        public async Task<ActionResult> Create(AboutBottomCreateVM createVM)
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

                string pathh = Helper.GetFilePath(_env.WebRootPath, "assets/images/subpage/about", fileName);

                await Helper.SaveFile(pathh, createVM.Photo);

                AboutBottom bottom = new AboutBottom
                {
                    Image = fileName,
                    Description = createVM.Description,
                    Title= createVM.Title,
                };

                await _context.AboutBottoms.AddAsync(bottom);

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

                AboutBottom aboutBottom = await _context.AboutBottoms.FirstOrDefaultAsync(m => m.Id == id);

                if (aboutBottom is null) return NotFound();

                return View(aboutBottom);

            }
            catch (Exception ex)
            {

                ViewBag.Message = ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AboutBottom aboutBottom)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(aboutBottom);
                }

                if (aboutBottom.Photo != null)
                {
                    if (!aboutBottom.Photo.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photo", "Please choose correct image type");
                        return View();
                    }

                    if (!aboutBottom.Photo.CheckFileSize(200))
                    {
                        ModelState.AddModelError("Photo", "Please choose correct image size");
                        return View();
                    }

                    string fileName = Guid.NewGuid().ToString() + "_" + aboutBottom.Photo.FileName;

                    AboutBottom dbBottom = await _context.AboutBottoms.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

                    if (dbBottom is null) return NotFound();

                    if (dbBottom.Title.Trim().ToLower() == aboutBottom.Title.Trim().ToLower()
                        && dbBottom.Description.Trim().ToLower() == aboutBottom.Description.Trim().ToLower()
                        && dbBottom.Photo == aboutBottom.Photo)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    string path = Helper.GetFilePath(_env.WebRootPath, "assets/images/subpage/about", fileName);

                    using (FileStream stream = new FileStream(path, FileMode.Create))
                    {
                        await aboutBottom.Photo.CopyToAsync(stream);
                    }

                    aboutBottom.Image = fileName;

                    _context.AboutBottoms.Update(aboutBottom);

                    await _context.SaveChangesAsync();

                    string dbPath = Helper.GetFilePath(_env.WebRootPath, "assets/images/subpage/about", dbBottom.Image);

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
            AboutBottom bottom = await _context.AboutBottoms
               .Where(m => !m.IsDeleted && m.Id == id)
               .FirstOrDefaultAsync();

            if (bottom == null) return NotFound();

            bottom.IsDeleted = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetStatus(int id)
        {
            List<AboutBottom> dbBottom = await _context.AboutBottoms.Where(m => m.IsActive).ToListAsync();

            if (dbBottom.Count < 10)
            {
                AboutBottom bottom = await _context.AboutBottoms.FirstOrDefaultAsync(m => m.Id == id);

                if (bottom is null) return NotFound();

                if (bottom.IsActive)
                {
                    bottom.IsActive = false;
                }
                else
                {
                    bottom.IsActive = true;
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            else
            {
                AboutBottom bottom = await _context.AboutBottoms.FirstOrDefaultAsync(m => m.Id == id);
                if (bottom.IsActive)
                {
                    bottom.IsActive = false;
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }


        }

        private async Task<List<AboutBottom>> GetByIdToLIstAsync(int id)
        {
            return await _context.AboutBottoms.Where(m => !m.IsDeleted && m.Id == id).ToListAsync();
        }

        private async Task<AboutBottom> GetByIdAsync(int id)
        {
            return await _context.AboutBottoms.Where(m => !m.IsDeleted && m.Id == id).FirstOrDefaultAsync();
        }


    }
}

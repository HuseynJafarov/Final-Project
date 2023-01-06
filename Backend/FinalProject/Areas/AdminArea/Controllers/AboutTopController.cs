using FinalProject.Data;
using FinalProject.Helpers;
using FinalProject.Models;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(AboutTop createVM)
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

                AboutTop top = new AboutTop
                {
                    Image = fileName,
                    Description = createVM.Description,
                    Title = createVM.Title,
                };

                await _context.AboutTops.AddAsync(top);

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AboutTop aboutTop)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(aboutTop);
                }

                if (aboutTop.Photo != null)
                {
                    if (!aboutTop.Photo.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photo", "Please choose correct image type");
                        return View();
                    }

                    if (!aboutTop.Photo.CheckFileSize(200))
                    {
                        ModelState.AddModelError("Photo", "Please choose correct image size");
                        return View();
                    }

                    string fileName = Guid.NewGuid().ToString() + "_" + aboutTop.Photo.FileName;

                    AboutTop dbTop = await _context.AboutTops.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

                    if (dbTop is null) return NotFound();

                    if (dbTop.Title.Trim().ToLower() == aboutTop.Title.Trim().ToLower()&&
                        dbTop.SubTitle.Trim().ToLower() == aboutTop.SubTitle.Trim().ToLower()&&
                        dbTop.EndDescription.Trim().ToLower() == aboutTop.EndDescription.Trim().ToLower()&&
                        dbTop.Button.Trim().ToLower() == aboutTop.Button.Trim().ToLower()
                        && dbTop.Description.Trim().ToLower() == aboutTop.Description.Trim().ToLower()
                        && dbTop.Photo == aboutTop.Photo)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    string path = Helper.GetFilePath(_env.WebRootPath, "assets/images/subpage/about", fileName);

                    using (FileStream stream = new FileStream(path, FileMode.Create))
                    {
                        await aboutTop.Photo.CopyToAsync(stream);
                    }

                    aboutTop.Image = fileName;

                    _context.AboutTops.Update(aboutTop);

                    await _context.SaveChangesAsync();

                    string dbPath = Helper.GetFilePath(_env.WebRootPath, "assets/images/subpage/about", dbTop.Image);

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
            AboutTop top = await _context.AboutTops
               .Where(m => !m.IsDeleted && m.Id == id)
               .FirstOrDefaultAsync();

            if (top == null) return NotFound();

            top.IsDeleted = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        private async Task<List<AboutTop>> GetByIdToLIstAsync(int id)
        {
            return await _context.AboutTops.Where(m => !m.IsDeleted && m.Id == id).ToListAsync();
        }

        private async Task<AboutTop> GetByIdAsync(int id)
        {
            return await _context.AboutTops.Where(m => !m.IsDeleted && m.Id == id).FirstOrDefaultAsync();
        }

    }
}

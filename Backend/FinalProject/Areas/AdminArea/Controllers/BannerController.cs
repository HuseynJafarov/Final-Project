using FinalProject.Data;
using FinalProject.Helpers;
using FinalProject.Models;
using FinalProject.ViewModel;
using FinalProject.ViewModel.AboutBottomViewModels;
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
    public class BannerController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BannerController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Banner> banner = await _context.Banners.Where(m => !m.IsDeleted).ToListAsync();
            return View(banner);
        }

        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id is null) return BadRequest();

                Banner banner = await _context.Banners.FirstOrDefaultAsync(m => m.Id == id);

                if (banner is null) return NotFound();

                return View(banner);

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
        public async Task<ActionResult> Create(BannerCreateVM createVM)
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

                string pathh = Helper.GetFilePath(_env.WebRootPath, "assets/images/demos/demo1/banner", fileName);

                await Helper.SaveFile(pathh, createVM.Photo);

                Banner banner = new Banner
                {
                    Image = fileName,
                    Description = createVM.Description,
                    Title = createVM.Title,
                };

                await _context.Banners.AddAsync(banner);

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

                Banner banner = await _context.Banners.FirstOrDefaultAsync(m => m.Id == id);

                if (banner is null) return NotFound();

                return View(banner);

            }
            catch (Exception ex)
            {

                ViewBag.Message = ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Banner banner)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(banner);
                }

                if (banner.Photo != null)
                {
                    if (!banner.Photo.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photo", "Please choose correct image type");
                        return View();
                    }

                    if (!banner.Photo.CheckFileSize(200))
                    {
                        ModelState.AddModelError("Photo", "Please choose correct image size");
                        return View();
                    }

                    string fileName = Guid.NewGuid().ToString() + "_" + banner.Photo.FileName;

                    Banner dbBanner = await _context.Banners.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

                    if (dbBanner is null) return NotFound();

                    if (dbBanner.Title.Trim().ToLower() == banner.Title.Trim().ToLower()
                        && dbBanner.Description.Trim().ToLower() == banner.Description.Trim().ToLower()
                        && dbBanner.Photo == banner.Photo)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    string path = Helper.GetFilePath(_env.WebRootPath, "assets/images/demos/demo1/banner", fileName);

                    using (FileStream stream = new FileStream(path, FileMode.Create))
                    {
                        await banner.Photo.CopyToAsync(stream);
                    }

                    banner.Image = fileName;

                    _context.Banners.Update(banner);

                    await _context.SaveChangesAsync();

                    string dbPath = Helper.GetFilePath(_env.WebRootPath, "assets/images/demos/demo1/banner", dbBanner.Image);

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
            Banner banner = await _context.Banners
               .Where(m => !m.IsDeleted && m.Id == id)
               .FirstOrDefaultAsync();

            if (banner == null) return NotFound();

            banner.IsDeleted = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetStatus(int id)
        {
            List<Banner> dbModel = await _context.Banners.Where(m => m.IsActive).ToListAsync();

            if (dbModel.Count < 10)
            {
                Banner model = await _context.Banners.FirstOrDefaultAsync(m => m.Id == id);

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
                Banner model = await _context.Banners.FirstOrDefaultAsync(m => m.Id == id);
                if (model.IsActive)
                {
                    model.IsActive = false;
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }


        }

        private async Task<List<Banner>> GetByIdToLIstAsync(int id)
        {
            return await _context.Banners.Where(m => !m.IsDeleted && m.Id == id).ToListAsync();
        }

        private async Task<Banner> GetByIdAsync(int id)
        {
            return await _context.Banners.Where(m => !m.IsDeleted && m.Id == id).FirstOrDefaultAsync();
        }
    }
}

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
    public class ProductInfoDetailController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductInfoDetailController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<ProductInfoDetail> productInfos = await _context.ProductInfoDetails.Where(m => !m.IsDeleted).ToListAsync();
            return View(productInfos);
        }

        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id is null) return BadRequest();

                ProductInfoDetail productInfos = await _context.ProductInfoDetails.FirstOrDefaultAsync(m => m.Id == id);

                if (productInfos is null) return NotFound();

                return View(productInfos);

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
        public async Task<ActionResult> Create(ProductInfoDetailCreateVM createVM)
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

                ProductInfoDetail productInfos = new ProductInfoDetail
                {
                    Image = fileName,
                    Description = createVM.Description,
                    Title = createVM.Title,
                };

                await _context.ProductInfoDetails.AddAsync(productInfos);

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

                ProductInfoDetail productInfos = await _context.ProductInfoDetails.FirstOrDefaultAsync(m => m.Id == id);

                if (productInfos is null) return NotFound();

                return View(productInfos);

            }
            catch (Exception ex)
            {

                ViewBag.Message = ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductInfoDetail productInfos)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(productInfos);
                }

                if (productInfos.Photo != null)
                {
                    if (!productInfos.Photo.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photo", "Please choose correct image type");
                        return View();
                    }

                    if (!productInfos.Photo.CheckFileSize(200))
                    {
                        ModelState.AddModelError("Photo", "Please choose correct image size");
                        return View();
                    }

                    string fileName = Guid.NewGuid().ToString() + "_" + productInfos.Photo.FileName;

                    ProductInfoDetail dbProductInfos = await _context.ProductInfoDetails.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

                    if (dbProductInfos is null) return NotFound();

                    if (dbProductInfos.Title.Trim().ToLower() == productInfos.Title.Trim().ToLower()
                        && dbProductInfos.Description.Trim().ToLower() == productInfos.Description.Trim().ToLower()
                        && dbProductInfos.Photo == productInfos.Photo)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    string path = Helper.GetFilePath(_env.WebRootPath, "assets/images/demos/demo1/banner", fileName);

                    using (FileStream stream = new FileStream(path, FileMode.Create))
                    {
                        await productInfos.Photo.CopyToAsync(stream);
                    }

                    productInfos.Image = fileName;

                    _context.ProductInfoDetails.Update(productInfos);

                    await _context.SaveChangesAsync();

                    string dbPath = Helper.GetFilePath(_env.WebRootPath, "assets/images/demos/demo1/banner", dbProductInfos.Image);

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
            ProductInfoDetail productInfos = await _context.ProductInfoDetails
               .Where(m => !m.IsDeleted && m.Id == id)
               .FirstOrDefaultAsync();

            if (productInfos == null) return NotFound();

            productInfos.IsDeleted = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetStatus(int id)
        {
            List<ProductInfoDetail> dbModel = await _context.ProductInfoDetails.Where(m => m.IsActive).ToListAsync();

            if (dbModel.Count < 10)
            {
                ProductInfoDetail model = await _context.ProductInfoDetails.FirstOrDefaultAsync(m => m.Id == id);

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
                ProductInfoDetail model = await _context.ProductInfoDetails.FirstOrDefaultAsync(m => m.Id == id);
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

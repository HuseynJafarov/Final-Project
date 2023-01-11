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
    public class ProductDetailController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductDetailController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<ProductDetail> details = await _context.ProductDetails.Where(m => !m.IsDeleted).ToListAsync();
            return View(details);
        }

        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id is null) return BadRequest();

                ProductDetail detail = await _context.ProductDetails.FirstOrDefaultAsync(m => m.Id == id);

                if (detail is null) return NotFound();

                return View(detail);

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
        public async Task<ActionResult> Create(ProductDetailCreateVM createVM)
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

                string pathh = Helper.GetFilePath(_env.WebRootPath, "assets/images/products/product-single/banner", fileName);

                await Helper.SaveFile(pathh, createVM.Photo);

                ProductDetail detail = new ProductDetail
                {
                    Image = fileName,
                    Description = createVM.Description,
                    Title = createVM.Title,
                    HeaderDescription= createVM.HeaderDescription,
                    HeaderTitle= createVM.HeaderTitle,
                };

                await _context.ProductDetails.AddAsync(detail);

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

                ProductDetail detail = await _context.ProductDetails.FirstOrDefaultAsync(m => m.Id == id);

                if (detail is null) return NotFound();

                return View(detail);

            }
            catch (Exception ex)
            {

                ViewBag.Message = ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductDetail detail)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(detail);
                }

                if (detail.Photo != null)
                {
                    if (!detail.Photo.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photo", "Please choose correct image type");
                        return View();
                    }

                    if (!detail.Photo.CheckFileSize(200))
                    {
                        ModelState.AddModelError("Photo", "Please choose correct image size");
                        return View();
                    }

                    string fileName = Guid.NewGuid().ToString() + "_" + detail.Photo.FileName;

                    ProductDetail dbDetail = await _context.ProductDetails.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

                    if (dbDetail is null) return NotFound();

                    if (dbDetail.Title.Trim().ToLower() == detail.Title.Trim().ToLower()
                        && dbDetail.Description.Trim().ToLower() == detail.Description.Trim().ToLower()
                        && dbDetail.Photo == detail.Photo && dbDetail.HeaderDescription.Trim().ToLower() == detail.HeaderDescription.Trim().ToLower()
                        && dbDetail.HeaderTitle.Trim().ToLower() == detail.HeaderTitle.Trim().ToLower())
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    string path = Helper.GetFilePath(_env.WebRootPath, "assets/images/products/product-single/banner", fileName);

                    using (FileStream stream = new FileStream(path, FileMode.Create))
                    {
                        await detail.Photo.CopyToAsync(stream);
                    }

                    detail.Image = fileName;

                    _context.ProductDetails.Update(detail);

                    await _context.SaveChangesAsync();

                    string dbPath = Helper.GetFilePath(_env.WebRootPath, "assets/images/products/product-single/banner", dbDetail.Image);

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
            ProductDetail detail = await _context.ProductDetails
               .Where(m => !m.IsDeleted && m.Id == id)
               .FirstOrDefaultAsync();

            if (detail == null) return NotFound();

            detail.IsDeleted = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}

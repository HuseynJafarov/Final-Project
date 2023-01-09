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
    public class ProductInfoController : Controller
    {
        private readonly AppDbContext _context;
        public ProductInfoController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            List<ProductInfo> info = await _context.ProductInfos
              .Where(m => !m.IsDeleted).ToListAsync();
            return View(info);
        }

        [HttpGet]
        public IActionResult Create() => View();
      

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductInfo info)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }


                bool isExist = await _context.ProductInfos.AnyAsync(m => m.Title.Trim() == info.Title.Trim()
                && m.Description.Trim() == info.Description.Trim() && m.Icon.Trim() == info.Icon.Trim());


                if (isExist)
                {
                    ModelState.AddModelError("Description", "About Text already exist");
                    return View();
                }

                await _context.ProductInfos.AddAsync(info);

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

            ProductInfo info = await _context.ProductInfos.FindAsync(id);

            if (info == null) return NotFound();

            return View(info);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            ProductInfo info = await _context.ProductInfos.FirstOrDefaultAsync(m => m.Id == id);

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

                ProductInfo info = await _context.ProductInfos.FirstOrDefaultAsync(m => m.Id == id);

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
        public async Task<IActionResult> Edit(int id, ProductInfo info)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(info);
                }

                ProductInfo dbInfo = await _context.ProductInfos.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

                if (dbInfo is null) return NotFound();

                if (dbInfo.Title.ToLower().Trim() == dbInfo.Title.ToLower().Trim() &&
                    dbInfo.Description.ToLower().Trim() == info.Description.ToLower().Trim() &&
                    dbInfo.Icon.ToLower().Trim() == info.Icon.ToLower().Trim())
                {
                    return RedirectToAction(nameof(Index));
                }


                _context.ProductInfos.Update(info);

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
            List<ProductInfo> dbModel = await _context.ProductInfos.Where(m => m.IsActive).ToListAsync();

            if (dbModel.Count < 10)
            {
                ProductInfo model = await _context.ProductInfos.FirstOrDefaultAsync(m => m.Id == id);

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
                ProductInfo model = await _context.ProductInfos.FirstOrDefaultAsync(m => m.Id == id);
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

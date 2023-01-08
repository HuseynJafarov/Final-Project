﻿using FinalProject.Data;
using FinalProject.Helpers.Enums;
using FinalProject.Helpers;
using FinalProject.Models;
using FinalProject.ViewModel;
using FinalProject.ViewModels.ProductViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using FinalProject.ViewModels.ProductViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FinalProject.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]

    public class ShopController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public ShopController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            List<Product> products = await _context.Products
                .Where(m => !m.IsDeleted)
                .Include(m => m.ProductImages)
                .Include(m => m.Category)
                .ToListAsync();
            List<Category> categories = await _context.Categories.Where(m => m.IsDeleted == false).Include(m => m.Products).ToListAsync();


            ShopVM shopVM = new ShopVM
            {
                Categories = categories,
                Products = products,
            };

            return View(shopVM);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.categories = await GetCategoriesAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateVM product)
        {
            ViewBag.categories = await GetCategoriesAsync();

            if (!ModelState.IsValid)
            {
                return View(product);
            }

            foreach (var photo in product.Photos)
            {
                if (!photo.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Photo", "Please choose correct image type");
                    return View(product);
                }


                if (!photo.CheckFileSize(500))
                {
                    ModelState.AddModelError("Photo", "Please choose correct image size");
                    return View(product);
                }

            }

            List<ProductImage> images = new List<ProductImage>();

            foreach (var photo in product.Photos)
            {
                string fileName = Guid.NewGuid().ToString() + "_" + photo.FileName;

                string path = Helper.GetFilePath(_env.WebRootPath, "assets/images/products", fileName);

                await Helper.SaveFile(path, photo);


                ProductImage image = new ProductImage
                {
                    Image = fileName,
                };

                images.Add(image);
            }

            images.FirstOrDefault().IsMain = true;

            decimal convertedPrice = StringToDecimal(product.Price);

            Product newProduct = new Product
            {
                Name = product.Name,
                Description = product.Description,
                Price = convertedPrice,
                Discount = product.Discount,
                CategoryId = product.CategoryId,
                ProductImages = images
            };

            await _context.ProductImages.AddRangeAsync(images);
            await _context.Products.AddAsync(newProduct);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            Product product = await _context.Products
                .Where(m => !m.IsDeleted && m.Id == id)
                .Include(m => m.ProductImages)
                .FirstOrDefaultAsync();

            if (product == null) return NotFound();

            foreach (var item in product.ProductImages)
            {
                string path = Helper.GetFilePath(_env.WebRootPath, "assets/images/products", item.Image);
                Helper.DeleteFile(path);
                item.IsDeleted = true;
            }

            product.IsDeleted = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

        }


        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id is null) return BadRequest();

            ViewBag.categories = await GetCategoriesAsync();

            Product dbProduct = await GetByIdAsync((int)id);

            return View(new ProductUpdateVM
            {
                Id = dbProduct.Id,
                Name = dbProduct.Name,
                Description= dbProduct.Description,
                Price = dbProduct.Price.ToString("0.#####").Replace(",", "."),
                Discount = dbProduct.Discount,
                CategoryId = dbProduct.CategoryId,
                Images = dbProduct.ProductImages,
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductUpdateVM updatedProduct)
        {
            ViewBag.categories = await GetCategoriesAsync();

            if (!ModelState.IsValid) return View(updatedProduct);

            Product dbProduct = await GetByIdAsync(id);

            if (updatedProduct.Photos != null)
            {

                foreach (var photo in updatedProduct.Photos)
                {
                    if (!photo.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photo", "Please choose correct image type");
                        return View(updatedProduct);
                    }


                    if (!photo.CheckFileSize(500))
                    {
                        ModelState.AddModelError("Photo", "Please choose correct image size");
                        return View(updatedProduct);
                    }

                }

                foreach (var item in dbProduct.ProductImages)
                {
                    string path = Helper.GetFilePath(_env.WebRootPath, "assets/images/products", item.Image);
                    Helper.DeleteFile(path);
                }


                List<ProductImage> images = new List<ProductImage>();

                foreach (var photo in updatedProduct.Photos)
                {

                    string fileName = Guid.NewGuid().ToString() + "_" + photo.FileName;

                    string path = Helper.GetFilePath(_env.WebRootPath, "assets/images/products", fileName);

                    await Helper.SaveFile(path, photo);


                    ProductImage image = new ProductImage
                    {
                        Image = fileName,
                    };

                    images.Add(image);

                }

                images.FirstOrDefault().IsMain = true;

                dbProduct.ProductImages = images;

            }

            decimal convertedPrice = StringToDecimal(updatedProduct.Price);

            dbProduct.Name = updatedProduct.Name;
            dbProduct.Description = updatedProduct.Description;
            dbProduct.Price = convertedPrice;
            dbProduct.Discount = updatedProduct.Discount;
            dbProduct.CategoryId = updatedProduct.CategoryId;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id is null) return BadRequest();

            ViewBag.categories = await GetCategoriesAsync();

            Product dbProduct = await GetByIdAsync((int)id);

            return View(new ProductUpdateVM
            {
                Id = dbProduct.Id,
                Name = dbProduct.Name,
                Description = dbProduct.Description,
                Price = dbProduct.Price.ToString("0.#####").Replace(",", "."),
                Discount = dbProduct.Discount,
                CategoryId = dbProduct.CategoryId,
                CategoryName = dbProduct.Category.Name,
                Images = dbProduct.ProductImages
            });
        }

        private decimal StringToDecimal(string str)
        {
            return decimal.Parse(str.Replace(".", ","));
        }

        private async Task<SelectList> GetCategoriesAsync()
        {
            IEnumerable<Category> categories = await _context.Categories.Where(m => !m.IsDeleted).ToListAsync();
            return new SelectList(categories, "Id", "Name");
        }

        private async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products
                                 .Where(m => !m.IsDeleted && m.Id == id)
                                 .Include(m => m.Category)
                                 .Include(m => m.ProductImages)
                                 .FirstOrDefaultAsync();
        }

    }
}

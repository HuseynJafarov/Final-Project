using FinalProject.Data;
using FinalProject.Models;
using FinalProject.Services;
using FinalProject.ViewModel;
using FinalProject.ViewModel.WishlistViewModel;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly LayoutService _layoutService;
        public HomeController(AppDbContext context, LayoutService layoutService)
        {
            _context = context;
            _layoutService = layoutService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.count = await _context.Products.Where(m => !m.IsDeleted).CountAsync();

            Dictionary<string, string> settingDatas = await _layoutService.GetDatasFromSetting();

            int take = int.Parse(settingDatas["HomeTakeProduct"]);
            int takeBlog = int.Parse(settingDatas["HomeTakeBlog"]);

            IEnumerable<SliderDetail> sliderDetails = await _context.SliderDetails.Where(m => !m.IsDeleted).ToListAsync();
            IEnumerable<Service> services = await _context.Services.Where(m => !m.IsDeleted).ToListAsync();
            IEnumerable<Category> categories = await _context.Categories.Where(m => !m.IsDeleted)
                    .Include(m=> m.Products).ToListAsync();
            IEnumerable<Product> products = await _context.Products
                    .Where(m => m.IsDeleted == false)
                    .Include(m => m.Category)
                    .Include(m => m.ProductImages).Take(take).ToListAsync();
            IEnumerable<ProductInfo> productInfos = await _context.ProductInfos.Where(m => !m.IsDeleted).Take(take).ToListAsync();
            IEnumerable<Banner> banners = await _context.Banners.Where(m => !m.IsDeleted).ToListAsync();
            IEnumerable<Blog> blogs =await _context.Blogs.Where(m => !m.IsDeleted).Take(takeBlog).ToListAsync();
            IEnumerable<ProductInfoDetail> productInfoDetail = await _context.ProductInfoDetails.Where(m => !m.IsDeleted).ToListAsync();

            HomeVM model = new HomeVM
            {
                SliderDetails= sliderDetails,
                Services= services,
                Categories= categories,
                Products= products,
                ProductInfos= productInfos,
                Banners= banners,
                Blogs= blogs,
                ProductInfoDetail= productInfoDetail
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> AddBasket(int? id)
        {
            if (id is null) return BadRequest();

            var dbProduct = await GetProductById(id);

            if (dbProduct == null) return NotFound();

            List<BasketVM> basket = GetBasket();

            UpdateBasket(basket, dbProduct.Id);

            Response.Cookies.Append("basket", JsonConvert.SerializeObject(basket));

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddWish(int? id)
        {
            if (id is null) return BadRequest();

            var dbProduct = await GetProductById(id);

            if (dbProduct == null) return NotFound();

            List<WishVM> wish = GetWish();

            UpdateWish(wish, dbProduct.Id);

            Response.Cookies.Append("wish", JsonConvert.SerializeObject(wish));

            return RedirectToAction("Index");
        }

       
        public IActionResult Search(string search)
        {
            List<Product> searchName = _context.Products.Where(s => s.Name.Trim().Contains(search.Trim())).Include(m => m.ProductImages).ToList();
            return PartialView("_Search", searchName);
        }

        public async Task<IActionResult> LoadMore(int skip)
        {
            IEnumerable<Product> products = await _context.Products.Where(m => !m.IsDeleted).Include(m => m.Category).Include(m => m.ProductImages).Skip(skip).Take(4).ToListAsync();

            HomeVM model = new HomeVM
            {
                Products= products,
            };

            return PartialView("_ProductPartial", model);
        }

        private void UpdateBasket(List<BasketVM> basket, int id)
        {
            BasketVM existProduct = basket.FirstOrDefault(m => m.Id == id);

            if (existProduct == null)
            {
                basket.Add(new BasketVM
                {
                    Id = id,
                    Count = 1
                });
            }
            else
            {
                existProduct.Count++;
            }
        }

        private void UpdateWish(List<WishVM> wish, int id)
        {
            WishVM existProduct = wish.FirstOrDefault(m => m.Id == id);

            if (existProduct == null)
            {
                wish.Add(new WishVM
                {
                    Id = id,
                    Count = 1
                });
            }
            else
            {
                existProduct.Count++;
            }
        }

        private async Task<Product> GetProductById(int? id)
        {
            return await _context.Products.FindAsync(id);
        }

        private List<WishVM> GetWish()
        {
            List<WishVM> wish;

            if (Request.Cookies["wish"] != null)
            {
                wish = JsonConvert.DeserializeObject<List<WishVM>>(Request.Cookies["wish"]);
            }
            else
            {
                wish = new List<WishVM>();
            }

            return wish;
        }

        private List<BasketVM> GetBasket()
        {
            List<BasketVM> basket;

            if (Request.Cookies["basket"] != null)
            {
                basket = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);
            }
            else
            {
                basket = new List<BasketVM>();
            }

            return basket;
        }


    }
}

using FinalProject.Data;
using FinalProject.Helpers;
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
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDbContext _context;
        private readonly LayoutService _layoutService;

        public ShopController(AppDbContext context, LayoutService layoutService)
        {
            _context = context;
            _layoutService = layoutService;
        }
        public async Task<IActionResult> Index(int page = 1)
        {


            Dictionary<string, string> settingDatas = await _layoutService.GetDatasFromSetting();

            int take = int.Parse(settingDatas["ProductTake"]);

            List<Product> products = await _context.Products.Where(m => !m.IsDeleted)
                .Include(m => m.Category).Include(m => m.ProductImages)
                .Skip((page * take) - take).Take(take).OrderBy(m => m.Id).ToListAsync();
            IEnumerable<Category> categories = await _context.Categories.Where(m => !m.IsDeleted)
                                .Include(m => m.Products).ToListAsync();
            int count = await GetPageCount(take);

            List<ShopVM> shopList = new List<ShopVM>();

            ShopVM model = new ShopVM
            {
                Products = products.ToList(),
                Categories = categories.ToList()

            };


            shopList.Add(model);

            Paginate<ShopVM> result = new Paginate<ShopVM>(shopList, page, count);

            return View(result);
        }


     

        private async Task<int> GetPageCount(int take)
        {
            int productCount = await _context.Products.Where(m => !m.IsDeleted).CountAsync();

            return (int)Math.Ceiling((decimal)productCount / take);
        }
    }
}

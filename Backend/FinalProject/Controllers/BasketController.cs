using FinalProject.Data;
using FinalProject.ViewModels.BasketViewModel;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using FinalProject.Models;
using Microsoft.AspNetCore.Identity;
using Org.BouncyCastle.Bcpg;

namespace FinalProject.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;

        public BasketController(AppDbContext context)
        {
            _context = context;


        }


        public async Task<IActionResult> Index()
        {

            if (Request.Cookies["basket"] != null)
            {
                List<BasketVM> basketItems = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);
                List<BasketDetailVM> basketDetail = new List<BasketDetailVM>();

                foreach (var item in basketItems)
                {
                    var product = await _context.Products
                        .Where(m => m.Id == item.Id && m.IsDeleted == false)
                        .Include(m => m.ProductImages).FirstOrDefaultAsync();

                    BasketDetailVM newBasket = new BasketDetailVM
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Image = product.ProductImages.Where(m => m.IsMain).FirstOrDefault().Image,
                        Price = product.Price,
                        Discount= product.Discount,
                        Count = item.Count,
                        Total = product.Price * item.Count
                    };

                    basketDetail.Add(newBasket);

                }
                return View(basketDetail);
            }
            else
            {
                List<BasketDetailVM> basketDetail = new List<BasketDetailVM>();
                return View(basketDetail);
            }
        }

        //public async Task<IActionResult> Down(int id)
        //{
        //    Product product = _context.Products.Where(m => !m.IsDeleted).FirstOrDefault();
        //    BasketVM basketItems = new BasketVM
        //    {
        //        Id = product.Id,

        //    };
        //    basketItems.Count++;
        //    return
        //}
        //public async Task<IActionResult> Up(int id)
        //{
        //    return
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            List<BasketVM> basketItems = JsonConvert.DeserializeObject<List<BasketVM>>(Request.Cookies["basket"]);
            foreach (var item in basketItems)
            {
                if (item.Id == id)
                {
                    basketItems.Remove(item);
                    Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketItems));
                    return RedirectToAction("Index", "Basket");
                }
            }
            return RedirectToAction("Index", "Basket");

        }
    }
}

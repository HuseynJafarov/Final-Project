using FinalProject.Data;
using FinalProject.ViewModels.BasketViewModel;
using FinalProject.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinalProject.ViewModel.WishlistViewModel;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers
{
    public class WishlistController : Controller
    {
        private readonly AppDbContext _context;

        public WishlistController(AppDbContext context)
        {
            _context = context;


        }


        public async Task<IActionResult> Index()
        {

            if (Request.Cookies["wish"] != null)
            {
                List<WishVM> wishItems = JsonConvert.DeserializeObject<List<WishVM>>(Request.Cookies["wish"]);
                List<WishDetailVM> wishDetail = new List<WishDetailVM>();

                foreach (var item in wishItems)
                {
                    var product = await _context.Products
                        .Where(m => m.Id == item.Id && m.IsDeleted == false)
                        .Include(m => m.ProductImages).FirstOrDefaultAsync();

                    WishDetailVM newWish = new WishDetailVM
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Image = product.ProductImages.Where(m => m.IsMain).FirstOrDefault().Image,
                        Price = product.Price,
                        Discount = product.Discount,
                        Count = item.Count,
                        Total = product.Price * item.Count
                    };

                    wishDetail.Add(newWish);

                }
                return View(wishDetail);
            }
            else
            {
                List<WishDetailVM> wishDetail = new List<WishDetailVM>();
                return View(wishDetail);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            List<WishVM> wishItems = JsonConvert.DeserializeObject<List<WishVM>>(Request.Cookies["wish"]);
            foreach (var item in wishItems)
            {
                if (item.Id == id)
                {
                    wishItems.Remove(item);
                    Response.Cookies.Append("wish", JsonConvert.SerializeObject(wishItems));
                    return RedirectToAction("Index", "Wishlist");
                }
            }
            return RedirectToAction("Index", "Wishlist");

        }
    }
}

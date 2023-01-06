using FinalProject.Data;
using FinalProject.Models;
using FinalProject.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Controllers
{
    public class ProductDetailController : Controller
    {
        private readonly AppDbContext _context;
        public ProductDetailController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? id)
        {
            List<Product> products = await _context.Products.Where(m => !m.IsDeleted && m.Id == id)
                .Include(m => m.ProductImages).ToListAsync();

            IEnumerable<ProductDetail> productDetails = await _context.ProductDetails.Where(m => !m.IsDeleted)
                .ToListAsync();

            IEnumerable<Product> listProduct = await _context.Products.Where(m => !m.IsDeleted)
                .Include(m => m.ProductImages).ToListAsync();

            ProductDetailVM model = new ProductDetailVM
            {
                Product=products,
                Details= productDetails,
                ListProduct=listProduct,
            };
            return View(model);
        }
    }
}

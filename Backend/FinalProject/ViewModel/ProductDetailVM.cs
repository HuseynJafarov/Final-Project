using FinalProject.Models;
using System.Collections;
using System.Collections.Generic;

namespace FinalProject.ViewModel
{
    public class ProductDetailVM
    {
        public List<Product> Product { get; set; }
        public IEnumerable<Product> ListProduct { get; set; }
        public IEnumerable<ProductDetail> Details { get; set; }
    }
}

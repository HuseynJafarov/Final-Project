using FinalProject.Models;
using System.Collections.Generic;

namespace FinalProject.ViewModel
{
    public class ShopVM
    {
        public ICollection<Category> Categories { get; set; }
        public List<Product> Products { get; set; }
    }
}

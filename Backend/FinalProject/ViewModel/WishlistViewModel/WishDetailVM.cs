using FinalProject.Models;
using System.Collections;
using System.Collections.Generic;

namespace FinalProject.ViewModel.WishlistViewModel
{
    public class WishDetailVM
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public int Count { get; set; }
    }
}

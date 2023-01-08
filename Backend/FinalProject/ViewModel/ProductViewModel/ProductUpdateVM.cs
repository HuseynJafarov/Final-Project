using FinalProject.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.ViewModels.ProductViewModels
{
    public class ProductUpdateVM
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Price { get; set; }
        public int Discount { get; set; }
        [Required]
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public ICollection<ProductImage> Images { get; set; }
        public List<IFormFile> Photos { get; set; }
    }
}

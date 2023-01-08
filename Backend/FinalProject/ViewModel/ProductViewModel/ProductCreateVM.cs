using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace FinalProject.ViewModels.ProductViewModel
{
    public class ProductCreateVM
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Price { get; set; }
        public int Discount { get; set; }
        [Required]
        public string Description { get; set; }
        public int CategoryId { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public List<IFormFile> Photos { get; set; }
    }
}

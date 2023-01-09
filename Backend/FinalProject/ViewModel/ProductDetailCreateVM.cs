using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.ViewModel
{
    public class ProductDetailCreateVM
    {
        public string Image { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string HeaderTitle { get; set; }
        [Required]
        public string HeaderDescription { get; set; }
        [Required]
        [NotMapped]
        public IFormFile Photo { get; set; }
    }
}

using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class ProductInfoDetail:BaseEntity
    {
        public bool IsActive { get; set; } = false;
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        [NotMapped]
        public IFormFile Photo { get; set; }
    }
}

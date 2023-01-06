using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class AboutBottom:BaseEntity
    {
        public string Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        [NotMapped]
        public IFormFile Photo { get; set; }
    }
}

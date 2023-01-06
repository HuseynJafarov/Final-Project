using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class AboutTop:BaseEntity
    {
        public string Image { get; set; }
        public string SubTitle { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string EndDescription { get; set; }
        public string Button { get; set; }
        [NotMapped]
        public IFormFile Photo { get; set; }
    }
}

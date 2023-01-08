using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public class SliderDetail: BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string SingImage { get; set; }
        [NotMapped]
        public IFormFile Photo { get; set; }
       
    }
}

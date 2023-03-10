using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.ViewModel
{
    public class AboutTopCreateVM
    {
        public string Image { get; set; }
        [Required]
        public string SubTitle { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string EndDescription { get; set; }
        [Required]
        public string Button { get; set; }
        [Required]
        [NotMapped]
        public IFormFile Photo { get; set; }
    }
}

using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.ViewModel
{
    public class SliderDetailVM
    {
        public string Image { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required(ErrorMessage = "Can't be empty")]
        public List<IFormFile> Photo { get; set; }
    }
}

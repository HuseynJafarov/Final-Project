using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.ViewModel.AboutBottomViewModels
{
    public class AboutBottomCreateVM
    {

        public string Image { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        [NotMapped]
        public IFormFile Photo { get; set; }
    }
}

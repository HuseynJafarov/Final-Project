using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.Models
{
    public class Category:BaseEntity
    {

        [Required(ErrorMessage = "Name can't be empty")]
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; }

    }
}

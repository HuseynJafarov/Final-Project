using System.ComponentModel.DataAnnotations;

namespace FinalProject.Models
{
    public class Subscribe:BaseEntity
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}

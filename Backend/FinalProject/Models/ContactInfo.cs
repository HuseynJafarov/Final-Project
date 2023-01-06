using System.Security.Permissions;

namespace FinalProject.Models
{
    public class ContactInfo:BaseEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
    }
}

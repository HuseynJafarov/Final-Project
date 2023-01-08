using System.Security.Permissions;

namespace FinalProject.Models
{
    public class ContactInfo:BaseEntity
    {
        public bool IsActive { get; set; } = false;
        public string Title { get; set; }
        public string Description { get; set; }
        public string Icon { get; set; }
    }
}

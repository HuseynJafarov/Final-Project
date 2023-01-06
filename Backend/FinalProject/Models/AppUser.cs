using Microsoft.AspNetCore.Identity;

namespace FinalProject.Models
{
    public class AppUser: IdentityUser
    {
        public string FullName { get; set; }
        public int Age { get; set; }
        public bool IsActive { get; set; } = false;
    }
}

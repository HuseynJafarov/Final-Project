namespace FinalProject.Models
{
    public class ContactUs:BaseEntity
    {
        public bool IsActive { get; set; } = false;
        public string Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}

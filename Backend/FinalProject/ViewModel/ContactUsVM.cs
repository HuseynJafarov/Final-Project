using FinalProject.Models;
using System.Collections.Generic;

namespace FinalProject.ViewModel
{
    public class ContactUsVM
    {
        public IEnumerable<ContactInfo> ContactInfo { get; set; }
        public ContactUs ContactUs { get; set; }
        public TellUs TellUs { get; set; }
    }
}

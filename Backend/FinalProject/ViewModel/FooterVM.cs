using FinalProject.Models;
using System.Collections;
using System.Collections.Generic;

namespace FinalProject.ViewModel
{
    public class FooterVM
    {
        public string PandaLogo { get; set; }
        public string PaymentImage { get; set; }
        public string SiteName { get; set; }
        public IEnumerable<FooterCategory> FooterCategories { get; set; }
        public IEnumerable<Social> Socials { get; set; }
        public IEnumerable<ContactInfo> ContactInfos { get; set; }
        public IEnumerable<Service> Service { get; set; }

    }
}

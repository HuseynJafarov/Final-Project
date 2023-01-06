using FinalProject.Models;
using System.Collections;
using System.Collections.Generic;

namespace FinalProject.ViewModel
{
    public class HeaderVM
    {
        public IEnumerable<ContactInfo> ContactInfo { get; set; }
        public string PandaLogo { get; set; }
        public IEnumerable<Social> Socials { get; set; }
    }
}

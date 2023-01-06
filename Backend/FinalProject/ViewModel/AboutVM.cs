using FinalProject.Models;
using System.Collections.Generic;

namespace FinalProject.ViewModel
{
    public class AboutVM
    {
        public AboutBottom AboutBottom { get; set; }
        public IEnumerable<AboutLi> AboutLi { get; set; }
        public AboutTop AboutTop { get; set; }
    }
}

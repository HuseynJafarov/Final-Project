using FinalProject.Models;
using System.Collections;
using System.Collections.Generic;

namespace FinalProject.ViewModel
{
    public class HomeVM
    {
        public IEnumerable<SliderDetail> SliderDetails { get; set; }
        public IEnumerable<Service> Services { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<ProductInfo> ProductInfos { get; set; }
        public IEnumerable<Banner> Banners { get; set; }
        public IEnumerable<Blog> Blogs { get; set; }
        public IEnumerable<ProductInfoDetail>ProductInfoDetail { get; set; }
    }
}

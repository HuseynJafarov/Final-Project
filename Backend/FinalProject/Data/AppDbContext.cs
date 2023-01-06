using FinalProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Data
{
    public class AppDbContext: IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<SliderDetail> SliderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ProductInfo> ProductInfos { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<ContactInfo> ContactInfos { get; set; }
        public DbSet<ContactUs> ContactUs { get; set; }
        public DbSet<FooterCategory> FooterCategories { get; set; }
        
        public DbSet<Social> Socials { get; set; }
        
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<TellUs> TellUs { get; set; }
        public DbSet<Subscribe> Subscribes { get; set; }
        public DbSet<AboutTop> AboutTops { get; set; }
        public DbSet<AboutBottom> AboutBottoms { get; set; }
        public DbSet<AboutLi> AboutLis { get; set; }
        public DbSet<ProductDetail> ProductDetails { get; set; }
        public DbSet<ProductInfoDetail> ProductInfoDetails { get; set; }





        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}

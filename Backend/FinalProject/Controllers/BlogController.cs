using FinalProject.Data;
using FinalProject.Helpers;
using FinalProject.Models;
using FinalProject.Services;
using FinalProject.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Controllers
{
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;
        private readonly LayoutService _layoutService;

        public BlogController(AppDbContext context, LayoutService layoutService)
        {
            _context = context;
            _layoutService = layoutService;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            Dictionary<string, string> settingDatas = await _layoutService.GetDatasFromSetting();

            int take = int.Parse(settingDatas["HomeTakeBlog"]);
            List<Blog> blog = await _context.Blogs.Where(m => !m.IsDeleted)
                .Skip((page * take) - take).Take(take).OrderBy(m => m.Id).ToListAsync();
            int count = await GetPageCount(take);

            List<BlogVM> blogList = new List<BlogVM>();

            BlogVM model = new BlogVM
            {
                Blogs = blog,
                
            };

            blogList.Add(model);

            Paginate<BlogVM> result = new Paginate<BlogVM>(blogList, page, count);

            return View(result);

        }

        private async Task<int> GetPageCount(int take)
        {
            int blogCount = await _context.Blogs.Where(m => !m.IsDeleted).CountAsync();

            return (int)Math.Ceiling((decimal)blogCount / take);
        }
    }
}

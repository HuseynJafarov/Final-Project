using FinalProject.Data;
using FinalProject.Helpers;
using FinalProject.Helpers.Enums;
using FinalProject.Models;
using FinalProject.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Areas.AdminArea.Controllers
{
    [Area("AdminArea")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class BlogController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BlogController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<Blog> blogs = await _context.Blogs.Where(m => !m.IsDeleted).ToListAsync();
            return View(blogs);
        }

        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id is null) return BadRequest();

                Blog blog = await _context.Blogs.FirstOrDefaultAsync(m => m.Id == id);

                if (blog is null) return NotFound();

                return View(blog);

            }
            catch (Exception ex)
            {

                ViewBag.Message = ex.Message;
                return View();
            }

        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(BlogCreateVM createVM)
        {
            if (!ModelState.IsValid) return View(createVM);

            if (createVM.Photo != null)
            {
                if (!createVM.Photo.CheckFileType("image/"))
                {
                    ModelState.AddModelError("Photo", "Please choose correct image type");
                    return View(createVM);
                }

                if (!createVM.Photo.CheckFileSize(500))
                {
                    ModelState.AddModelError("Photo", "Please choose correct image size");
                    return View(createVM);
                }

                string fileName = Guid.NewGuid().ToString() + "_" + createVM.Photo.FileName;

                string pathh = Helper.GetFilePath(_env.WebRootPath, "assets/images/blog", fileName);

                await Helper.SaveFile(pathh, createVM.Photo);

                Blog blog = new Blog
                {
                    Image = fileName,
                    Description = createVM.Description,
                    Title = createVM.Title,
                };

                await _context.Blogs.AddAsync(blog);

            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id is null) return BadRequest();

                Blog blog = await _context.Blogs.FirstOrDefaultAsync(m => m.Id == id);

                if (blog is null) return NotFound();

                return View(blog);

            }
            catch (Exception ex)
            {

                ViewBag.Message = ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Blog blog)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(blog);
                }

                if (blog.Photo != null)
                {
                    if (!blog.Photo.CheckFileType("image/"))
                    {
                        ModelState.AddModelError("Photo", "Please choose correct image type");
                        return View();
                    }

                    if (!blog.Photo.CheckFileSize(200))
                    {
                        ModelState.AddModelError("Photo", "Please choose correct image size");
                        return View();
                    }

                    string fileName = Guid.NewGuid().ToString() + "_" + blog.Photo.FileName;

                    Blog dbBlog = await _context.Blogs.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);

                    if (dbBlog is null) return NotFound();

                    if (dbBlog.Title.Trim().ToLower() == blog.Title.Trim().ToLower()
                        && dbBlog.Description.Trim().ToLower() == blog.Description.Trim().ToLower()
                        && dbBlog.Photo == blog.Photo)
                    {
                        return RedirectToAction(nameof(Index));
                    }

                    string path = Helper.GetFilePath(_env.WebRootPath, "assets/images/blog", fileName);

                    using (FileStream stream = new FileStream(path, FileMode.Create))
                    {
                        await blog.Photo.CopyToAsync(stream);
                    }

                    blog.Image = fileName;

                    _context.Blogs.Update(blog);

                    await _context.SaveChangesAsync();

                    string dbPath = Helper.GetFilePath(_env.WebRootPath, "assets/images/blog", dbBlog.Image);

                    Helper.DeleteFile(dbPath);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            Blog blog = await _context.Blogs
               .Where(m => !m.IsDeleted && m.Id == id)
               .FirstOrDefaultAsync();

            if (blog == null) return NotFound();

            blog.IsDeleted = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private async Task<List<Blog>> GetByIdToLIstAsync(int id)
        {
            return await _context.Blogs.Where(m => !m.IsDeleted && m.Id == id).ToListAsync();
        }

        private async Task<Blog> GetByIdAsync(int id)
        {
            return await _context.Blogs.Where(m => !m.IsDeleted && m.Id == id).FirstOrDefaultAsync();
        }
    }
}

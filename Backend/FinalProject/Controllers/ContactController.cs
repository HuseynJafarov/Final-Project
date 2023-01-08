using FinalProject.Data;
using FinalProject.Models;
using FinalProject.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Controllers
{
    public class ContactController : Controller
    {
        private readonly AppDbContext _context;
        public ContactController(AppDbContext context)
        {
            _context = context;

        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<ContactInfo> contactInfo = await _context.ContactInfos.Where(m => !m.IsDeleted && m.IsActive).ToListAsync();
            ContactUs contactUs = await _context.ContactUs.Where(m => !m.IsDeleted && m.IsActive).FirstOrDefaultAsync();

            ContactUsVM model = new ContactUsVM
            {
                ContactUs= contactUs,
                ContactInfo= contactInfo,
                TellUs=  new TellUs()
            };

            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(TellUs tellUs)
        {
            try
            {

                if (!ModelState.IsValid)
                {
                    return RedirectToAction(nameof(Index));
                }


                bool isExist = await _context.TellUs.AnyAsync(m => m.Name.Trim() == tellUs.Name.Trim() &&
                m.Email.Trim() == tellUs.Email.Trim() &&
                m.Message.Trim() == tellUs.Message.Trim() &&
                m.Subject.Trim() == tellUs.Subject.Trim() &&
                m.Message == tellUs.Message);

                if (isExist)
                {
                    ModelState.AddModelError("Name", "Subject already exist");
                    return View();
                }


                await _context.TellUs.AddAsync(tellUs);

                await _context.SaveChangesAsync();


                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return View();
            }


        }
    }
}


using FinalProject.Data;
using FinalProject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.Services
{
    public class LayoutService
    {
        private readonly AppDbContext _context;

        public LayoutService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<string, string>> GetDatasFromSetting()
        {
            return await _context.Settings.ToDictionaryAsync(m => m.Key, m => m.Value);
        }

        public async Task<IEnumerable<Social>> GEtDatasFromSocial()
        {
            return await _context.Socials.Where(m => !m.IsDeleted).ToListAsync();
        }

        public async Task<IEnumerable<Service>> GEtDatasFromService()
        {
            return await _context.Services.Where(m => !m.IsDeleted).ToListAsync();
        }

        public async Task<IEnumerable<FooterCategory>> GetDatasFromFooterCategory()
        {
            return await _context.FooterCategories.Where(m => !m.IsDeleted).ToListAsync();
        }
        public async Task<IEnumerable<ContactInfo>> GetDatasFromContactInfo()
        {
            return await _context.ContactInfos.Where(m => !m.IsDeleted).ToListAsync();
        }
    }
}

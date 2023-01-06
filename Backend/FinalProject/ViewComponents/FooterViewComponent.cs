using FinalProject.Data;
using FinalProject.Models;
using FinalProject.Services;
using FinalProject.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinalProject.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly LayoutService _layoutService;
        private readonly AppDbContext _context;

        public FooterViewComponent(LayoutService layoutService, AppDbContext context)
        {
            _layoutService = layoutService;
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            Dictionary<string, string> settingDatas = await _layoutService.GetDatasFromSetting();

            IEnumerable<FooterCategory> footerCategories = await _layoutService.GetDatasFromFooterCategory();

            IEnumerable<ContactInfo> contactInfos = await _layoutService.GetDatasFromContactInfo();

            IEnumerable<Social> socials = await _layoutService.GEtDatasFromSocial();

            IEnumerable<Service> service = await _layoutService.GEtDatasFromService();



            FooterVM model = new FooterVM
            {
                FooterCategories = footerCategories,
                ContactInfos = contactInfos,
                Socials = socials,
                Service = service,
                PandaLogo = settingDatas["PandaLogo"],
                PaymentImage = settingDatas["Payment"],
                SiteName = settingDatas["SiteName"]
            };

            return await Task.FromResult(View(model));
        }

       
    }
}

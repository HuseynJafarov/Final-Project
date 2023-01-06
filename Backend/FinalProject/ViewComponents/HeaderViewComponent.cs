using FinalProject.Data;
using FinalProject.Models;
using FinalProject.Services;
using FinalProject.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FinalProject.ViewComponents
{
    public class HeaderViewComponent:ViewComponent
    {
        private readonly LayoutService _layoutService;
        private readonly AppDbContext _context;

        public HeaderViewComponent(LayoutService layoutService, AppDbContext context)
        {
            _layoutService = layoutService;
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            Dictionary<string, string> settingDatas = await _layoutService.GetDatasFromSetting();

            IEnumerable<ContactInfo> contactInfos = await _layoutService.GetDatasFromContactInfo();

            IEnumerable<Social> socials = await _layoutService.GEtDatasFromSocial();


            HeaderVM model = new HeaderVM
            {
                PandaLogo = settingDatas["PandaLogo"],
                ContactInfo= contactInfos,
                Socials= socials,
            };


            return await Task.FromResult(View(model));

        }

    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiniETBIS.Models;
using MiniETBIS.Services;

namespace MiniETBIS.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly ICompanyService _companyService;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(IDashboardService dashboardService,
            ICompanyService companyService,
            UserManager<ApplicationUser> userManager)
        {
            _dashboardService = dashboardService;
            _companyService = companyService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                var dto = await _dashboardService.GetAdminDashboardAsync();
                return View("AdminDashboard", dto);
            }
            else
            {
                var userId = _userManager.GetUserId(User)!;
                var company = await _companyService.GetByUserIdAsync(userId);
                if (company == null) return RedirectToAction("Create", "Company");

                var dto = await _dashboardService.GetCompanyDashboardAsync(company.Id);
                return View("CompanyDashboard", dto);
            }
        }
    }
}

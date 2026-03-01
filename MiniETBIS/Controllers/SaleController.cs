using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MiniETBIS.Models;
using MiniETBIS.Models.DTOs;
using MiniETBIS.Services;

namespace MiniETBIS.Controllers
{
    [Authorize(Roles = "Firma")]
    public class SaleController : Controller
    {
        private readonly ISaleService _saleService;
        private readonly IProductService _productService;
        private readonly ICompanyService _companyService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuditLogService _auditLog;

        public SaleController(ISaleService saleService,
            IProductService productService,
            ICompanyService companyService,
            UserManager<ApplicationUser> userManager,
            IAuditLogService auditLog)
        {
            _saleService = saleService;
            _productService = productService;
            _companyService = companyService;
            _userManager = userManager;
            _auditLog = auditLog;
        }

        private async Task<CompanyDto?> GetMyCompanyAsync()
        {
            var userId = _userManager.GetUserId(User)!;
            return await _companyService.GetByUserIdAsync(userId);
        }

        public async Task<IActionResult> Index(int page = 1, DateTime? startDate = null, DateTime? endDate = null)
        {
            var company = await GetMyCompanyAsync();
            if (company == null) return RedirectToAction("Create", "Company");

            var result = await _saleService.GetByCompanyPagedAsync(company.Id, page, 10, startDate, endDate);
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var company = await GetMyCompanyAsync();
            if (company == null) return RedirectToAction("Create", "Company");

            var products = await _productService.GetByCompanyAsync(company.Id);
            ViewBag.Products = new SelectList(products, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSaleDto dto)
        {
            var company = await GetMyCompanyAsync();
            if (company == null) return RedirectToAction("Create", "Company");

            if (!ModelState.IsValid)
            {
                var products = await _productService.GetByCompanyAsync(company.Id);
                ViewBag.Products = new SelectList(products, "Id", "Name");
                return View(dto);
            }

            var sale = await _saleService.CreateAsync(dto, company.Id);
            if (sale == null)
            {
                ModelState.AddModelError(string.Empty, "Satýþ oluþturulamadý. Ürünü kontrol ediniz.");
                var products = await _productService.GetByCompanyAsync(company.Id);
                ViewBag.Products = new SelectList(products, "Id", "Name");
                return View(dto);
            }

            var userId = _userManager.GetUserId(User)!;
            await _auditLog.LogAsync(userId, $"Satýþ ekledi: Ürün ID={dto.ProductId}, Adet={dto.Quantity}", HttpContext.Connection.RemoteIpAddress?.ToString());
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var company = await GetMyCompanyAsync();
            if (company == null) return Forbid();

            await _saleService.DeleteAsync(id, company.Id);
            var userId = _userManager.GetUserId(User)!;
            await _auditLog.LogAsync(userId, $"Satýþ sildi: ID={id}", HttpContext.Connection.RemoteIpAddress?.ToString());
            return RedirectToAction(nameof(Index));
        }
    }
}

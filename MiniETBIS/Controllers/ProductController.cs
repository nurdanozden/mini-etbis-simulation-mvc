using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiniETBIS.Models;
using MiniETBIS.Models.DTOs;
using MiniETBIS.Services;

namespace MiniETBIS.Controllers
{
    [Authorize(Roles = "Firma")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICompanyService _companyService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuditLogService _auditLog;

        public ProductController(IProductService productService,
            ICompanyService companyService,
            UserManager<ApplicationUser> userManager,
            IAuditLogService auditLog)
        {
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

        public async Task<IActionResult> Index(int page = 1)
        {
            var company = await GetMyCompanyAsync();
            if (company == null) return RedirectToAction("Create", "Company");

            var result = await _productService.GetByCompanyPagedAsync(company.Id, page, 10);
            ViewBag.CompanyId = company.Id;
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var company = await GetMyCompanyAsync();
            if (company == null) return RedirectToAction("Create", "Company");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var company = await GetMyCompanyAsync();
            if (company == null) return RedirectToAction("Create", "Company");

            var product = await _productService.CreateAsync(dto, company.Id);
            var userId = _userManager.GetUserId(User)!;
            await _auditLog.LogAsync(userId, $"Ürün ekledi: {dto.Name}", HttpContext.Connection.RemoteIpAddress?.ToString());
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var company = await GetMyCompanyAsync();
            if (company == null) return Forbid();

            var product = await _productService.GetByIdAsync(id);
            if (product == null || product.CompanyId != company.Id) return NotFound();

            return View(new EditProductDto { Id = product.Id, Name = product.Name, Category = product.Category, Price = product.Price });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProductDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var company = await GetMyCompanyAsync();
            if (company == null) return Forbid();

            var success = await _productService.UpdateAsync(dto, company.Id);
            if (!success) return NotFound();

            var userId = _userManager.GetUserId(User)!;
            await _auditLog.LogAsync(userId, $"Ürün güncelledi: ID={dto.Id}", HttpContext.Connection.RemoteIpAddress?.ToString());
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var company = await GetMyCompanyAsync();
            if (company == null) return Forbid();

            var success = await _productService.DeleteAsync(id, company.Id);
            if (!success) return NotFound();

            var userId = _userManager.GetUserId(User)!;
            await _auditLog.LogAsync(userId, $"Ürün sildi: ID={id}", HttpContext.Connection.RemoteIpAddress?.ToString());
            return RedirectToAction(nameof(Index));
        }
    }
}

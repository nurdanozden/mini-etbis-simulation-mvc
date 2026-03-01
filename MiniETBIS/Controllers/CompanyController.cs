using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MiniETBIS.Models;
using MiniETBIS.Models.DTOs;
using MiniETBIS.Services;

namespace MiniETBIS.Controllers
{
    [Authorize]
    public class CompanyController : Controller
    {
        private readonly ICompanyService _companyService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuditLogService _auditLog;

        public CompanyController(ICompanyService companyService,
            UserManager<ApplicationUser> userManager,
            IAuditLogService auditLog)
        {
            _companyService = companyService;
            _userManager = userManager;
            _auditLog = auditLog;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(int page = 1, string? sector = null, string? city = null)
        {
            var result = await _companyService.GetAllPagedAsync(page, 10, sector, city);
            ViewBag.Sectors = await _companyService.GetDistinctSectorsAsync();
            ViewBag.Cities = await _companyService.GetDistinctCitiesAsync();
            ViewBag.SelectedSector = sector;
            ViewBag.SelectedCity = city;
            return View(result);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int id)
        {
            var company = await _companyService.GetByIdAsync(id);
            if (company == null) return NotFound();
            return View(company);
        }

        [Authorize(Roles = "Firma")]
        public async Task<IActionResult> MyCompany()
        {
            var userId = _userManager.GetUserId(User)!;
            var company = await _companyService.GetByUserIdAsync(userId);
            if (company == null) return RedirectToAction(nameof(Create));
            return View(company);
        }

        [Authorize(Roles = "Firma")]
        [HttpGet]
        public IActionResult Create() => View();

        [Authorize(Roles = "Firma")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCompanyDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var userId = _userManager.GetUserId(User)!;
            var existing = await _companyService.GetByUserIdAsync(userId);
            if (existing != null)
            {
                ModelState.AddModelError(string.Empty, "Zaten bir firmanýz var.");
                return View(dto);
            }

            var result = await _companyService.CreateAsync(dto, userId);
            if (result == null)
            {
                ModelState.AddModelError(nameof(dto.TaxNumber), "Bu vergi numarasý zaten kayýtlý.");
                return View(dto);
            }

            await _auditLog.LogAsync(userId, $"Firma oluþturdu: {dto.Name}", HttpContext.Connection.RemoteIpAddress?.ToString());
            return RedirectToAction(nameof(MyCompany));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AdminCreate() => View();

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdminCreate(AdminCreateCompanyDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            if (await _companyService.TaxNumberExistsAsync(dto.TaxNumber))
            {
                ModelState.AddModelError(nameof(dto.TaxNumber), "Bu vergi numarasý zaten kayýtlý.");
                return View(dto);
            }

            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError(nameof(dto.Email), "Bu e-posta adresi zaten kullanýlýyor.");
                return View(dto);
            }

            var user = new ApplicationUser { UserName = dto.Email, Email = dto.Email, EmailConfirmed = true };
            var identityResult = await _userManager.CreateAsync(user, dto.Password);
            if (!identityResult.Succeeded)
            {
                foreach (var error in identityResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(dto);
            }

            await _userManager.AddToRoleAsync(user, "Firma");

            var createDto = new CreateCompanyDto
            {
                Name = dto.Name,
                TaxNumber = dto.TaxNumber,
                City = dto.City,
                Sector = dto.Sector
            };
            var company = await _companyService.CreateAsync(createDto, user.Id);

            var adminUserId = _userManager.GetUserId(User)!;
            await _auditLog.LogAsync(adminUserId, $"Admin firma oluþturdu: {dto.Name} ({dto.Email})", HttpContext.Connection.RemoteIpAddress?.ToString());

            TempData["Success"] = $"'{dto.Name}' firmasý baþarýyla oluþturuldu!";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Firma")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var company = await _companyService.GetByIdAsync(id);
            if (company == null) return NotFound();

            var userId = _userManager.GetUserId(User)!;
            var myCompany = await _companyService.GetByUserIdAsync(userId);
            if (myCompany?.Id != id) return Forbid();

            return View(new EditCompanyDto { Id = company.Id, Name = company.Name, City = company.City, Sector = company.Sector });
        }

        [Authorize(Roles = "Firma")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditCompanyDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var userId = _userManager.GetUserId(User)!;
            var success = await _companyService.UpdateAsync(dto, userId);
            if (!success) return Forbid();

            await _auditLog.LogAsync(userId, $"Firma güncelledi: ID={dto.Id}", HttpContext.Connection.RemoteIpAddress?.ToString());
            return RedirectToAction(nameof(MyCompany));
        }
    }
}

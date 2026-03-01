using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniETBIS.Services;

namespace MiniETBIS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AuditLogController : Controller
    {
        private readonly IAuditLogService _auditLogService;

        public AuditLogController(IAuditLogService auditLogService)
        {
            _auditLogService = auditLogService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var logs = await _auditLogService.GetLogsAsync(page, 20);
            ViewBag.Page = page;
            return View(logs);
        }
    }
}

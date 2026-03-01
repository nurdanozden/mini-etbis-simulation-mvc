using MiniETBIS.Models;

namespace MiniETBIS.Services
{
    public interface IAuditLogService
    {
        Task LogAsync(string userId, string action, string? ipAddress = null);
        Task<IEnumerable<AuditLog>> GetLogsAsync(int page, int pageSize);
    }
}

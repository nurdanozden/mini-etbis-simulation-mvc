using MiniETBIS.Models.DTOs;

namespace MiniETBIS.Services
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetAdminDashboardAsync();
        Task<DashboardDto> GetCompanyDashboardAsync(int companyId);
    }
}

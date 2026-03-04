using MiniETBIS.Models;
using MiniETBIS.Models.DTOs;

namespace MiniETBIS.Services
{
    public interface ISaleService
    {
        Task<PagedResult<SaleDto>> GetByCompanyPagedAsync(int companyId, int page, int pageSize, DateTime? startDate = null, DateTime? endDate = null);
        Task<SaleDto?> GetByIdAsync(int id);
        Task<Sale?> CreateAsync(CreateSaleDto dto, int companyId);
        Task<bool> DeleteAsync(int id, int companyId);
    }
}

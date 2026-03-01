using MiniETBIS.Models;
using MiniETBIS.Models.DTOs;

namespace MiniETBIS.Services
{
    public interface ICompanyService
    {
        Task<PagedResult<CompanyDto>> GetAllPagedAsync(int page, int pageSize, string? sectorFilter = null, string? cityFilter = null);
        Task<CompanyDto?> GetByIdAsync(int id);
        Task<CompanyDto?> GetByUserIdAsync(string userId);
        Task<Company?> CreateAsync(CreateCompanyDto dto, string userId);
        Task<bool> UpdateAsync(EditCompanyDto dto, string userId);
        Task<bool> DeleteAsync(int id, string userId);
        Task<bool> TaxNumberExistsAsync(string taxNumber);
        Task<List<string>> GetDistinctSectorsAsync();
        Task<List<string>> GetDistinctCitiesAsync();
    }
}

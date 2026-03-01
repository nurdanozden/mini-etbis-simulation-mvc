using MiniETBIS.Models;
using MiniETBIS.Models.DTOs;

namespace MiniETBIS.Services
{
    public interface IProductService
    {
        Task<PagedResult<ProductDto>> GetByCompanyPagedAsync(int companyId, int page, int pageSize);
        Task<IEnumerable<ProductDto>> GetByCompanyAsync(int companyId);
        Task<ProductDto?> GetByIdAsync(int id);
        Task<Product?> CreateAsync(CreateProductDto dto, int companyId);
        Task<bool> UpdateAsync(EditProductDto dto, int companyId);
        Task<bool> DeleteAsync(int id, int companyId);
    }
}

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MiniETBIS.Data;
using MiniETBIS.Models;
using MiniETBIS.Models.DTOs;

namespace MiniETBIS.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ProductService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResult<ProductDto>> GetByCompanyPagedAsync(int companyId, int page, int pageSize)
        {
            var query = _context.Products.AsNoTracking().Where(p => p.CompanyId == companyId);
            var total = await query.CountAsync();
            var items = await query
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Category = p.Category,
                    Price = p.Price,
                    CompanyId = p.CompanyId
                })
                .ToListAsync();

            return new PagedResult<ProductDto>
            {
                Items = items,
                TotalCount = total,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<ProductDto>> GetByCompanyAsync(int companyId)
        {
            return await _context.Products.AsNoTracking()
                .Where(p => p.CompanyId == companyId)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Category = p.Category,
                    Price = p.Price,
                    CompanyId = p.CompanyId
                })
                .ToListAsync();
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var product = await _context.Products.AsNoTracking()
                .Include(p => p.Company)
                .FirstOrDefaultAsync(p => p.Id == id);
            return product == null ? null : _mapper.Map<ProductDto>(product);
        }

        public async Task<Product?> CreateAsync(CreateProductDto dto, int companyId)
        {
            var product = _mapper.Map<Product>(dto);
            product.CompanyId = companyId;
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> UpdateAsync(EditProductDto dto, int companyId)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == dto.Id && p.CompanyId == companyId);
            if (product == null) return false;

            product.Name = dto.Name;
            product.Category = dto.Category;
            product.Price = dto.Price;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, int companyId)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.CompanyId == companyId);
            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

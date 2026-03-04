using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MiniETBIS.Data;
using MiniETBIS.Models;
using MiniETBIS.Models.DTOs;

namespace MiniETBIS.Services
{
    public class SaleService : ISaleService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public SaleService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResult<SaleDto>> GetByCompanyPagedAsync(int companyId, int page, int pageSize, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Sales.AsNoTracking()
                .Include(s => s.Product)
                .Where(s => s.Product!.CompanyId == companyId);

            if (startDate.HasValue)
            {
                var utcStart = DateTime.SpecifyKind(startDate.Value.Date, DateTimeKind.Utc);
                query = query.Where(s => s.SaleDate >= utcStart);
            }
            if (endDate.HasValue)
            {
                var utcEnd = DateTime.SpecifyKind(endDate.Value.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc);
                query = query.Where(s => s.SaleDate <= utcEnd);
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(s => s.SaleDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new SaleDto
                {
                    Id = s.Id,
                    ProductId = s.ProductId,
                    ProductName = s.Product!.Name,
                    Quantity = s.Quantity,
                    TotalAmount = s.TotalAmount,
                    SaleDate = s.SaleDate,
                    City = s.City
                })
                .ToListAsync();

            return new PagedResult<SaleDto>
            {
                Items = items,
                TotalCount = total,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<SaleDto?> GetByIdAsync(int id)
        {
            var sale = await _context.Sales.AsNoTracking()
                .Include(s => s.Product)
                .FirstOrDefaultAsync(s => s.Id == id);
            return sale == null ? null : _mapper.Map<SaleDto>(sale);
        }

        public async Task<Sale?> CreateAsync(CreateSaleDto dto, int companyId)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == dto.ProductId && p.CompanyId == companyId);
            if (product == null) return null;

            var sale = new Sale
            {
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                TotalAmount = product.Price * dto.Quantity,
                SaleDate = DateTime.UtcNow,
                City = dto.City
            };

            _context.Sales.Add(sale);
            await _context.SaveChangesAsync();
            return sale;
        }

        public async Task<bool> DeleteAsync(int id, int companyId)
        {
            var sale = await _context.Sales
                .Include(s => s.Product)
                .FirstOrDefaultAsync(s => s.Id == id && s.Product!.CompanyId == companyId);
            if (sale == null) return false;

            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

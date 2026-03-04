using Microsoft.EntityFrameworkCore;
using MiniETBIS.Data;
using MiniETBIS.Models.DTOs;

namespace MiniETBIS.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardDto> GetAdminDashboardAsync()
        {
            var dto = new DashboardDto
            {
                TotalCompanies = await _context.Companies.CountAsync(),
                TotalProducts = await _context.Products.CountAsync(),
                TotalSales = await _context.Sales.CountAsync(),
                TotalRevenue = await _context.Sales.SumAsync(s => (decimal?)s.TotalAmount) ?? 0,
            };

            dto.AverageOrderAmount = dto.TotalSales > 0 ? dto.TotalRevenue / dto.TotalSales : 0;

            dto.MonthlySales = await _context.Sales
                .AsNoTracking()
                .GroupBy(s => new { s.SaleDate.Year, s.SaleDate.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new MonthlySalesDto
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    TotalAmount = g.Sum(s => s.TotalAmount)
                })
                .ToListAsync();

            dto.SalesByCity = await _context.Sales
                .AsNoTracking()
                .GroupBy(s => s.City)
                .Select(g => new CityDistributionDto
                {
                    City = g.Key,
                    TotalAmount = g.Sum(s => s.TotalAmount)
                })
                .OrderByDescending(x => x.TotalAmount)
                .Take(10)
                .ToListAsync();

            dto.CompaniesBySector = await _context.Companies
                .AsNoTracking()
                .GroupBy(c => c.Sector)
                .Select(g => new SectorDistributionDto
                {
                    Sector = g.Key,
                    CompanyCount = g.Count()
                })
                .ToListAsync();

            dto.TopProducts = await _context.Sales
                .AsNoTracking()
                .Include(s => s.Product)
                .GroupBy(s => new { s.ProductId, s.Product!.Name })
                .Select(g => new TopProductDto
                {
                    ProductName = g.Key.Name,
                    TotalQuantity = g.Sum(s => s.Quantity),
                    TotalAmount = g.Sum(s => s.TotalAmount)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(10)
                .ToListAsync();

            return dto;
        }

        public async Task<DashboardDto> GetCompanyDashboardAsync(int companyId)
        {
            var productIds = await _context.Products
                .AsNoTracking()
                .Where(p => p.CompanyId == companyId)
                .Select(p => p.Id)
                .ToListAsync();

            var dto = new DashboardDto
            {
                TotalProducts = productIds.Count,
                TotalSales = await _context.Sales.CountAsync(s => productIds.Contains(s.ProductId)),
                TotalRevenue = await _context.Sales
                    .Where(s => productIds.Contains(s.ProductId))
                    .SumAsync(s => (decimal?)s.TotalAmount) ?? 0,
            };

            dto.AverageOrderAmount = dto.TotalSales > 0 ? dto.TotalRevenue / dto.TotalSales : 0;

            dto.MonthlySales = await _context.Sales
                .AsNoTracking()
                .Where(s => productIds.Contains(s.ProductId))
                .GroupBy(s => new { s.SaleDate.Year, s.SaleDate.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new MonthlySalesDto
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    TotalAmount = g.Sum(s => s.TotalAmount)
                })
                .ToListAsync();

            dto.SalesByCity = await _context.Sales
                .AsNoTracking()
                .Where(s => productIds.Contains(s.ProductId))
                .GroupBy(s => s.City)
                .Select(g => new CityDistributionDto
                {
                    City = g.Key,
                    TotalAmount = g.Sum(s => s.TotalAmount)
                })
                .OrderByDescending(x => x.TotalAmount)
                .ToListAsync();

            dto.TopProducts = await _context.Sales
                .AsNoTracking()
                .Include(s => s.Product)
                .Where(s => productIds.Contains(s.ProductId))
                .GroupBy(s => new { s.ProductId, s.Product!.Name })
                .Select(g => new TopProductDto
                {
                    ProductName = g.Key.Name,
                    TotalQuantity = g.Sum(s => s.Quantity),
                    TotalAmount = g.Sum(s => s.TotalAmount)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(10)
                .ToListAsync();

            return dto;
        }
    }
}

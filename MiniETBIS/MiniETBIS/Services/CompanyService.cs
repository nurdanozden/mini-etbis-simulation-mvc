using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MiniETBIS.Data;
using MiniETBIS.Models;
using MiniETBIS.Models.DTOs;

namespace MiniETBIS.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CompanyService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResult<CompanyDto>> GetAllPagedAsync(int page, int pageSize, string? sectorFilter = null, string? cityFilter = null)
        {
            var query = _context.Companies.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(sectorFilter))
                query = query.Where(c => c.Sector == sectorFilter);
            if (!string.IsNullOrWhiteSpace(cityFilter))
                query = query.Where(c => c.City == cityFilter);

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(c => c.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CompanyDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    TaxNumber = c.TaxNumber,
                    City = c.City,
                    Sector = c.Sector,
                    CreatedDate = c.CreatedDate
                })
                .ToListAsync();

            return new PagedResult<CompanyDto>
            {
                Items = items,
                TotalCount = total,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<CompanyDto?> GetByIdAsync(int id)
        {
            var company = await _context.Companies.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
            return company == null ? null : _mapper.Map<CompanyDto>(company);
        }

        public async Task<CompanyDto?> GetByUserIdAsync(string userId)
        {
            var company = await _context.Companies.AsNoTracking()
                .FirstOrDefaultAsync(c => c.UserId == userId);
            return company == null ? null : _mapper.Map<CompanyDto>(company);
        }

        public async Task<Company?> CreateAsync(CreateCompanyDto dto, string userId)
        {
            if (await TaxNumberExistsAsync(dto.TaxNumber))
                return null;

            var company = _mapper.Map<Company>(dto);
            company.UserId = userId;
            company.CreatedDate = DateTime.UtcNow;

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
            return company;
        }

        public async Task<bool> UpdateAsync(EditCompanyDto dto, string userId)
        {
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == dto.Id && c.UserId == userId);
            if (company == null) return false;

            company.Name = dto.Name;
            company.City = dto.City;
            company.Sector = dto.Sector;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id, string userId)
        {
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
            if (company == null) return false;

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TaxNumberExistsAsync(string taxNumber)
            => await _context.Companies.AnyAsync(c => c.TaxNumber == taxNumber);

        public async Task<List<string>> GetDistinctSectorsAsync()
            => await _context.Companies.AsNoTracking()
                .Select(c => c.Sector).Distinct().OrderBy(s => s).ToListAsync();

        public async Task<List<string>> GetDistinctCitiesAsync()
            => await _context.Companies.AsNoTracking()
                .Select(c => c.City).Distinct().OrderBy(s => s).ToListAsync();
    }
}

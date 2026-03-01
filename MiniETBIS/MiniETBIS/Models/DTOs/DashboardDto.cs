namespace MiniETBIS.Models.DTOs
{
    public class DashboardDto
    {
        public int TotalCompanies { get; set; }
        public int TotalProducts { get; set; }
        public int TotalSales { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderAmount { get; set; }

        public List<MonthlySalesDto> MonthlySales { get; set; } = new();
        public List<CityDistributionDto> SalesByCity { get; set; } = new();
        public List<SectorDistributionDto> CompaniesBySector { get; set; } = new();
        public List<TopProductDto> TopProducts { get; set; } = new();
    }

    public class MonthlySalesDto
    {
        public string Month { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }

    public class CityDistributionDto
    {
        public string City { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }

    public class SectorDistributionDto
    {
        public string Sector { get; set; } = string.Empty;
        public int CompanyCount { get; set; }
    }

    public class TopProductDto
    {
        public string ProductName { get; set; } = string.Empty;
        public int TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }
}

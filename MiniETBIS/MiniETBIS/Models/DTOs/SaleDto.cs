using System.ComponentModel.DataAnnotations;

namespace MiniETBIS.Models.DTOs
{
    public class SaleDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime SaleDate { get; set; }
        public string City { get; set; } = string.Empty;
    }

    public class CreateSaleDto
    {
        [Required(ErrorMessage = "Ürün seçimi zorunludur.")]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Satýþ miktarý en az 1 olmalýdýr.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Þehir zorunludur.")]
        public string City { get; set; } = string.Empty;
    }
}

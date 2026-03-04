using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniETBIS.Models
{
    public class Sale
    {
        public int Id { get; set; }

        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Satýþ miktarý negatif olamaz.")]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public DateTime SaleDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;
    }
}

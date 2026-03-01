using System.ComponentModel.DataAnnotations;

namespace MiniETBIS.Models.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
    }

    public class CreateProductDto
    {
        [Required(ErrorMessage = "Ürün adý zorunludur.")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kategori zorunludur.")]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalýdýr.")]
        public decimal Price { get; set; }
    }

    public class EditProductDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ürün adý zorunludur.")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kategori zorunludur.")]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalýdýr.")]
        public decimal Price { get; set; }
    }
}

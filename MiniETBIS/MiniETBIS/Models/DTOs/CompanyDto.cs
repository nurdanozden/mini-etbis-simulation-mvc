using System.ComponentModel.DataAnnotations;

namespace MiniETBIS.Models.DTOs
{
    public class CompanyDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TaxNumber { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Sector { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }

    public class CreateCompanyDto
    {
        [Required(ErrorMessage = "Firma adý zorunludur.")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vergi numarasý zorunludur.")]
        [StringLength(11, MinimumLength = 10, ErrorMessage = "Vergi numarasý 10-11 karakter olmalýdýr.")]
        public string TaxNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Þehir zorunludur.")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sektör zorunludur.")]
        public string Sector { get; set; } = string.Empty;
    }

    public class EditCompanyDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Firma adý zorunludur.")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Þehir zorunludur.")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sektör zorunludur.")]
        public string Sector { get; set; } = string.Empty;
    }

    public class AdminCreateCompanyDto
    {
        [Required(ErrorMessage = "Firma adý zorunludur.")]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vergi numarasý zorunludur.")]
        [StringLength(11, MinimumLength = 10, ErrorMessage = "Vergi numarasý 10-11 karakter olmalýdýr.")]
        public string TaxNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Þehir zorunludur.")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sektör zorunludur.")]
        public string Sector { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta giriniz.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Þifre zorunludur.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Þifre en az 6 karakter olmalýdýr.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}

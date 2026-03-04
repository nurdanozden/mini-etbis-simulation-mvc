using FluentValidation;
using MiniETBIS.Models.DTOs;

namespace MiniETBIS.Validators
{
    public class CreateCompanyValidator : AbstractValidator<CreateCompanyDto>
    {
        public CreateCompanyValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Firma adý zorunludur.").MaximumLength(200);
            RuleFor(x => x.TaxNumber)
                .NotEmpty().WithMessage("Vergi numarasý zorunludur.")
                .Length(10, 11).WithMessage("Vergi numarasý 10-11 karakter olmalýdýr.")
                .Matches("^[0-9]+$").WithMessage("Vergi numarasý sadece rakamlardan oluþmalýdýr.");
            RuleFor(x => x.City).NotEmpty().WithMessage("Þehir zorunludur.");
            RuleFor(x => x.Sector).NotEmpty().WithMessage("Sektör zorunludur.");
        }
    }

    public class CreateProductValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Ürün adý zorunludur.").MaximumLength(200);
            RuleFor(x => x.Category).NotEmpty().WithMessage("Kategori zorunludur.");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalýdýr.");
        }
    }

    public class CreateSaleValidator : AbstractValidator<CreateSaleDto>
    {
        public CreateSaleValidator()
        {
            RuleFor(x => x.ProductId).GreaterThan(0).WithMessage("Ürün seçimi zorunludur.");
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Satýþ miktarý en az 1 olmalýdýr.");
            RuleFor(x => x.City).NotEmpty().WithMessage("Þehir zorunludur.");
        }
    }
}

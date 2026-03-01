using AutoMapper;
using MiniETBIS.Models;
using MiniETBIS.Models.DTOs;

namespace MiniETBIS.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Company, CompanyDto>().ReverseMap();
            CreateMap<CreateCompanyDto, Company>();
            CreateMap<EditCompanyDto, Company>();

            CreateMap<Product, ProductDto>()
                .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.Company != null ? s.Company.Name : string.Empty))
                .ReverseMap();
            CreateMap<CreateProductDto, Product>();
            CreateMap<EditProductDto, Product>();

            CreateMap<Sale, SaleDto>()
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product != null ? s.Product.Name : string.Empty))
                .ReverseMap();
            CreateMap<CreateSaleDto, Sale>();
        }
    }
}

namespace API.Helpers;

using AutoMapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<User, UserModel>().ReverseMap();
        CreateMap<User, UserModel>();

        CreateMap<Category, CategoryModel>().ReverseMap();
        CreateMap<Category, CategoryModel>();

        CreateMap<Product, ProductModel>().ReverseMap();
        CreateMap<Product, ProductModel>();

        CreateMap<Product, ProductExportModel>().ReverseMap();
        CreateMap<Product, ProductExportModel>();
    }
}

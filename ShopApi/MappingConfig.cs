using AutoMapper;
using ShopApi.Model;
using ShopApi.Model.Dto;

namespace ShopApi;

public class MappingConfig: Profile
{
    public MappingConfig()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.ProductCategories.Select(pc => new SimpleCategoryDto { Id = pc.Category.Id, Type = pc.Category.Type })));

        CreateMap<ProductDto, Product>();
        CreateMap<Wishlist, WishlistDto>().ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.ProductWishlists.Select(pc => pc.Product)));
        CreateMap<WishlistDto, Wishlist>();
        CreateMap<Category, CategoryDto>().ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.ProductCategories.Select(pc => pc.Product)));
        CreateMap<OrderProducts, OrderProductDto>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product.Price))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.Total));

        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
        
            .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.OrderProducts));

        CreateMap<ApplicationUser, ApplicationUserDto>()
            .ForMember(dest => dest.Orders, opt => opt.MapFrom(src => src.Orders));

      
        CreateMap<Product, ProductCopy>().ReverseMap();
        CreateMap<ApplicationUser, ApplicationUserDto>()
            .ForMember(dest => dest.Orders, opt => opt.MapFrom(src => src.Orders)); 

       
        
        CreateMap<CreateProductDto, Product>().ReverseMap();
        CreateMap<CreateCategoryDto, Category>().ReverseMap();
        CreateMap<CreateOrderDto, Order>().ReverseMap();
        CreateMap<UpdateProductDto, Product>().ReverseMap();
        CreateMap<UpdateOrderDto, Order>().ReverseMap();
        CreateMap<UpdateCategoryDto, Category>().ReverseMap();
        CreateMap<ProductCategory, ProductCategoryDto>().ReverseMap();
        CreateMap<ApplicationUser, UserDto>().ReverseMap();
    }
}
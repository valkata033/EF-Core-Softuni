using AutoMapper;
using ProductShop.DTOs.Categories;
using ProductShop.DTOs.CategoryProducts;
using ProductShop.DTOs.Products;
using ProductShop.DTOs.User;
using ProductShop.Models;
using System.Linq;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            this.CreateMap<ImportUserDto, User>();
            this.CreateMap<ImportProductsDto, Product>();
            this.CreateMap<ImportCategoriesDto, Category>();
            this.CreateMap<ImportCategoriesProductsDto, CategoryProduct>();

            this.CreateMap<Product, ExportProductsInRangeDto>()
                .ForMember(d => d.SellerFullName , 
                mo => mo.MapFrom(s => $"{s.Seller.FirstName} {s.Seller.LastName}"));

            this.CreateMap<Product, ExportUsersWithSoldProductsDto>()
                .ForMember(d => d.FirstName, mo => mo.MapFrom(s => s.Buyer.FirstName))
                .ForMember(d => d.LastName, mo => mo.MapFrom(s => s.Buyer.LastName));

            this.CreateMap<User, ExportUsersWithSoldProductsDto>()
                .ForMember(d => d.SoldProducts, mo => mo.MapFrom(s => s.ProductsSold
                .Where(p => p.BuyerId.HasValue)));

            this.CreateMap<Category, ExportCategoriesDto>()
                .ForMember(d => d.ProductsCount, mo => mo.MapFrom(s => s.CategoryProducts.Count))
                .ForMember(d => d.AveragePrice, mo => mo.MapFrom(s => s.CategoryProducts.));

        }
    }
}

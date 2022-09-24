using AutoMapper;
using AutoMapper.QueryableExtensions;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DTOs.Categories;
using ProductShop.DTOs.CategoryProducts;
using ProductShop.DTOs.Products;
using ProductShop.DTOs.User;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            Mapper.Initialize(cnf => cnf.AddProfile(typeof(ProductShopProfile)));
            ProductShopContext dbContext = new ProductShopContext();

            //string inputJson = File.ReadAllText("../../../Datasets/categories-products.json");

            //dbContext.Database.EnsureDeleted();
            //dbContext.Database.EnsureCreated();
            //Console.WriteLine("Succeeded");

            string json = GetCategoriesByProductsCount(dbContext);
            File.WriteAllText("../../../Datasets/categories-by-products.json", json);
        }

        //Problem 01
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            ImportUserDto[] userDto = JsonConvert.DeserializeObject<ImportUserDto[]>(inputJson);

            ICollection<User> users = new List<User>();

            foreach (var uDto in userDto)
            {
                if (!IsValid(uDto))
                {
                    continue;
                }
                User user = Mapper.Map<User>(uDto);
                users.Add(user);
            }

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }

        //Problem 02
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            ImportProductsDto[] productsDto = JsonConvert.DeserializeObject<ImportProductsDto[]>(inputJson);

            ICollection<Product> products = new List<Product>();

            foreach (var pDto in productsDto)
            {
                if (!IsValid(pDto))
                {
                    continue;
                }
                Product product = Mapper.Map<Product>(pDto);
                products.Add(product);
            }

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        //Problem 03
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            ImportCategoriesDto[] categoriesDtos = JsonConvert.DeserializeObject<ImportCategoriesDto[]>(inputJson);

            ICollection<Category> categories = new List<Category>();

            foreach (var cDto in categoriesDtos)
            {
                if (!IsValid(cDto))
                {
                    continue;
                }
                Category category = Mapper.Map<Category>(cDto);
                categories.Add(category);
            }

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }
        
        //Problem 04
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            ImportCategoriesProductsDto[] categoriesProductsDtos 
                = JsonConvert.DeserializeObject<ImportCategoriesProductsDto[]>(inputJson);

            ICollection<CategoryProduct> categoryProducts = new List<CategoryProduct>();

            foreach (var cpDto in categoriesProductsDtos)
            {
                if (!IsValid(cpDto))
                {
                    continue;
                }
                CategoryProduct categoryProduct = Mapper.Map<CategoryProduct>(cpDto);
                categoryProducts.Add(categoryProduct);
            }

            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count}";
        }

        //Problem 05
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context
                .Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .ProjectTo<ExportProductsInRangeDto>()
                .ToArray();

            string json = JsonConvert.SerializeObject(products, Formatting.Indented);
            return json;
        }

        //Problem 06
        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context
                .Users
                .Where(u => u.ProductsSold.Any(p => p.BuyerId.HasValue))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .ProjectTo<ExportUsersWithSoldProductsDto>()
                .ToArray();

            string json = JsonConvert.SerializeObject(users, Formatting.Indented);
            return json;
        }

        //Problem 07
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {



        }


        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult);
            return isValid;
        }
    }
}
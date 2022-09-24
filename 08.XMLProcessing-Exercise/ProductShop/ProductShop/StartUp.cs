using ProductShop.Data;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        public static object Products { get; private set; }

        public static void Main(string[] args)
        {
            ProductShopContext dbContext = new ProductShopContext();
            string xml = File.ReadAllText("../../../Datasets/categories-products.xml");

            string result = ImportCategoryProducts(dbContext, xml);
            Console.WriteLine(result);

            //dbContext.Database.EnsureDeleted();
            //dbContext.Database.EnsureCreated();
            //Console.WriteLine("Ensured!");
        }

        //Problem 01
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            ImportUsersDto[] usersDtos = Deserialize<ImportUsersDto[]>(inputXml, "Users");

            User[] users = usersDtos
                .Select(x => new User
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Age = x.Age
                })
                .ToArray(); ;

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Length}";
        }

        //Problem 02
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            ImportProductsDto[] productsDtos = Deserialize<ImportProductsDto[]>(inputXml, "Products");

            Product[] products = productsDtos
                .Select(x => new Product
                {
                    Name = x.Name,
                    Price = x.Price,
                    SellerId = x.SellerId,
                    BuyerId = x.BuyerId
                })
                .ToArray();

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Length}";
        }

        //Problem 03
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            ImportCategoriesDto[] categoriesDtos = Deserialize<ImportCategoriesDto[]>(inputXml, "Categories");

            ICollection<Category> categories = new List<Category>();
            foreach (var cDto in categoriesDtos)
            {
                if (cDto.Name == null)
                {
                    continue;
                }

                Category category = new Category
                {
                    Name = cDto.Name
                };

                categories.Add(category);
            }

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }

        //Problem 04
        //Do not work!!!
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            ImportCategoryProductsDto[] cpDtos = Deserialize<ImportCategoryProductsDto[]>(inputXml, "CategoryProducts");

            ICollection<CategoryProduct> categoryProducts = new List<CategoryProduct>();
            foreach (var cpDto in cpDtos)
            {

                if (!context.CategoryProducts.Any(x => x.CategoryId == cpDto.CategoryId
                                                    && x.ProductId == cpDto.ProductId))
                {
                    continue;
                }

                CategoryProduct categoryProduct = new CategoryProduct
                {
                    ProductId = cpDto.ProductId,
                    CategoryId = cpDto.CategoryId
                };

                categoryProducts.Add(categoryProduct);
            }

            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count}";
        }


        //Helper methods
        private static T Deserialize<T>(string inputXml, string rootName)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), xmlRoot);

            using StringReader reader = new StringReader(inputXml);
            T dtos = (T)xmlSerializer.Deserialize(reader);

            return dtos;
        }

        private static string Serializer<T>(T dto, string rootName)
        {
            StringBuilder sb = new StringBuilder();

            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), xmlRoot);

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using StringWriter writer = new StringWriter(sb);
            xmlSerializer.Serialize(writer, dto, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}
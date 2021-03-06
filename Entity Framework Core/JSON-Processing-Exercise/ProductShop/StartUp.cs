using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.Dtos.Input;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {

        private static IMapper mapper;
        public static void Main(string[] args)
        {
            var context = new ProductShopContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            //var usersAsJson = File.ReadAllText("../../../Datasets/users.json");
            //var productsAsJson = File.ReadAllText("../../../Datasets/products.json");

            Console.WriteLine(GetUsersWithProducts(context));
        }

        //Query 2. Import Users
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            IEnumerable<UserInputDto> users = JsonConvert.DeserializeObject<IEnumerable<UserInputDto>>(inputJson);

            InitializeMapper();//our method for mapping

            var mappedUsers = mapper.Map<IEnumerable<User>>(users); 


            //IEnumerable<User> mappedUsers =  users.Select(x=>x.MapToDomainUser()).ToList(); if we use the static mapping

            context.Users.AddRange(mappedUsers);

            context.SaveChanges();

            return $"Successfully imported {mappedUsers.Count()}";
        }

        //Query 3. Import Products
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            IEnumerable<ProductInputDto> products = JsonConvert.DeserializeObject<IEnumerable<ProductInputDto>>(inputJson);

            InitializeMapper();

            var mappedProducts = mapper.Map<IEnumerable<Product>>(products);
            context.Products.AddRange(mappedProducts);
            context.SaveChanges();

           return $"Successfully imported {mappedProducts.Count()}";
        }

        //Query 4. Import Categories
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            //clear nulls
            IEnumerable<CategoryInputDto> categories = JsonConvert.DeserializeObject<IEnumerable<CategoryInputDto>>(inputJson)
                .Where(x=>!string.IsNullOrEmpty(x.Name));

            InitializeMapper();

            var mappedCategories = mapper.Map<IEnumerable<Category>>(categories);
            context.Categories.AddRange(mappedCategories);
            context.SaveChanges();

            return $"Successfully imported {mappedCategories.Count()}";
        }
        //Query 5. Import Categories and Products
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            IEnumerable<CategoryAndProductInputDto> categoriesProducts = JsonConvert.DeserializeObject<IEnumerable<CategoryAndProductInputDto>>(inputJson);

            InitializeMapper();

            var mappedCategoriesProducts = mapper.Map<IEnumerable<CategoryProduct>>(categoriesProducts);
            context.CategoryProducts.AddRange(mappedCategoriesProducts);
            context.SaveChanges();

            return $"Successfully imported {mappedCategoriesProducts.Count()}";
        }

        //Export 5. Products in Range
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .OrderBy(x => x.Price)
                .Select(x => new
                {
                    Name = x.Name,
                    Price = x.Price,
                    Seller = $"{x.Seller.FirstName} {x.Seller.LastName}",
                })
                .ToArray();

            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver= contractResolver
            };

            string productsAsJson = JsonConvert.SerializeObject(products,jsonSettings);

             return productsAsJson;
        }

        //Query 6. Export Successfully Sold Products
        public static string GetSoldProducts(ProductShopContext context)
        {
            var soldProducts = context
                .Users
                .Include(p => p.ProductsSold)
                .Where(x => x.ProductsSold.Any(y => y.Buyer != null))
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Select(x => new
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    SoldProducts = x.ProductsSold
                    .Select(p => new
                    {
                        Name = p.Name,
                        Price = p.Price,
                        BuyerFirstName = p.Buyer.FirstName,
                        BuyerLastName = p.Buyer.LastName,
                    })
                    .ToList()
                })
                .ToList();

            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = contractResolver
            };

            string soldProductsAsJson = JsonConvert.SerializeObject(soldProducts, jsonSettings);

            return soldProductsAsJson;
        }

        //Export 7. Categories by Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context
                .Categories
                .OrderByDescending(c=>c.CategoryProducts.Count)
                .Select(x => new
                {
                    Category = x.Name,
                    ProductsCount = x.CategoryProducts.Count,
                    AveragePrice = $"{x.CategoryProducts.Sum(p => p.Product.Price) / x.CategoryProducts.Count:f2}",
                    TotalRevenue = $"{x.CategoryProducts.Sum(p => p.Product.Price):f2}"
                }).ToList();

            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = contractResolver
            };

            string categoriesByProducts = JsonConvert.SerializeObject(categories, jsonSettings);
              return categoriesByProducts;
        }

        //Query 7. Export Users and Products
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var result = context.Users
                .Where(x => x.ProductsSold.Any(b => b.Buyer != null))
                .OrderByDescending(d => d.ProductsSold.Count)
                .Select(x => new
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Age = x.Age,


                    SoldProducts = x.ProductsSold.Select(f => new
                    {
                        Count = x.ProductsSold.Count(),

                        Products = x.ProductsSold.Select(p => new
                        {
                            Name = p.Name,
                            Price = p.Price
                        })
                    })
                }).ToList();

            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = contractResolver,
                NullValueHandling = NullValueHandling.Ignore//handle nulls
            };

            var resultWithCount = new
            {
                usersCount = result.Count,
                result
            };

            string usersWithProducts = JsonConvert.SerializeObject(resultWithCount, jsonSettings);
            return usersWithProducts;
        }


        private static void InitializeMapper()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });

           mapper = new Mapper(mapperConfiguration);
        }

    }
    //public static class UserMappings
    //{
    //    public static User MapToDomainUser(this UserInputDto userDto)
    //    {
    //        return new User
    //        {
    //            Age = userDto.Age,
    //            FirstName = userDto.FirstName,
    //            LastName = userDto.LastName
    //        };
    //    }
    //}
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Newtonsoft.Json;
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

            var usersAsJson = File.ReadAllText("../../../Datasets/users.json");
            var productsAsJson = File.ReadAllText("../../../Datasets/products.json");
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
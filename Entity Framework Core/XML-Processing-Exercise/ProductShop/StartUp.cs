using ProductShop.Data;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            ProductShopContext db = new ProductShopContext();

            //Imports
            string inputXml = File.ReadAllText("./Datasets/categories.xml");
            string result = ImportCategories(db, inputXml);
            Console.WriteLine(result);
        }


        //Query 1. Import Users
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            XmlRootAttribute xmlRootAttribute = new XmlRootAttribute("Users");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportUserDto[]), xmlRootAttribute);

            StringReader stringReader = new StringReader(inputXml);

            ImportUserDto[] dtos = (ImportUserDto[])
                xmlSerializer.Deserialize(stringReader);

            ICollection<User> users = new HashSet<User>();

            foreach (var userDto in dtos)
            {
                User u = new User()
                {
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    Age = userDto.Age
                };
                users.Add(u);
            }

            context.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }


        //Query 2. Import Products
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = GenerateSerializer("Products", typeof(ImportProductDto[]));

             StringReader stringReader = new StringReader(inputXml);

            ImportProductDto[] imporProducttDtos = (ImportProductDto[])xmlSerializer.Deserialize(stringReader);

            ICollection<Product> products = new HashSet<Product>();

            foreach (var productDto in imporProducttDtos)
            {
                Product p = new Product()
                {
                    Name=productDto.Name,
                    Price=productDto.Price,
                    SellerId=productDto.SellerId,
                    BuyerId=productDto.BuyerId
                };
                products.Add(p);
            }

            context.AddRange(products);
            context.SaveChanges();



            return $"Successfully imported {products.Count}";
        }


        //Query 3. Import Categories
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = GenerateSerializer("Categories", typeof(ImportCategoryDto[]));

            StringReader stringReader = new StringReader(inputXml);

            ImportCategoryDto[] imporCategoryDtos = (ImportCategoryDto[])xmlSerializer.Deserialize(stringReader);

            ICollection<Category> categories = new HashSet<Category>();

            foreach (var categoryDto in imporCategoryDtos)
            {
               // var categoryExist = imporCategoryDtos.Where(x => context.Categories.Any(ct => ct.Name == null));

                if (categoryDto == null)
                {
                    continue;
                }

                Category c = new Category()
                {
                    Name = categoryDto.Name
                };
                categories.Add(c);
            }

            context.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }

        //Query 4. Import Categories and Products
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {

        }

        private static XmlSerializer GenerateSerializer(string rootName, Type dtoType)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);

            XmlSerializer xmlSerializer = new XmlSerializer(dtoType, xmlRoot);

            return xmlSerializer;
        }

    }
}
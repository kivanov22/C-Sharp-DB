using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            ProductShopContext db = new ProductShopContext();

            //Imports
            //string inputXml = File.ReadAllText("./Datasets/categories-products.xml");
            //string result = ImportCategoryProducts(db, inputXml);
            //Console.WriteLine(result);

            //Exports
            string result = GetCategoriesByProductsCount(db);
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
            XmlSerializer xmlSerializer = GenerateSerializer("CategoryProducts", typeof(ImportCategoryProductDto[]));

            StringReader stringReader = new StringReader(inputXml);

            ImportCategoryProductDto[] imporCategoryDtos = (ImportCategoryProductDto[])xmlSerializer.Deserialize(stringReader);
               

            ICollection<CategoryProduct> categoriesProducts = new HashSet<CategoryProduct>();

            

            
            foreach (var categoryDto in imporCategoryDtos)
            {//??0 imports
                if (context.Categories.Any(cat=>cat.Id !=categoryDto.CategoryId) ||
                   context.Products.Any(p => p.Id != categoryDto.ProductId))
                {
                    continue;
                }

                CategoryProduct c = new CategoryProduct()
                {
                  CategoryId=categoryDto.CategoryId,
                  ProductId=categoryDto.ProductId,
                };
                categoriesProducts.Add(c);
            }

            context.AddRange(categoriesProducts);
            context.SaveChanges();



            return $"Successfully imported {categoriesProducts.Count}";
        }

        //Query 5. Products In Range
        public static string GetProductsInRange(ProductShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            StringWriter stringWriter = new StringWriter(sb);

            XmlSerializer xmlSerializer = GenerateSerializer("Products", typeof(ExportProductInRangeDto[]));

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, String.Empty);

            var productDto = context.Products
               .Where(x => x.Price >= 500 && x.Price <= 1000)
               .Select(p => new ExportProductInRangeDto
               {
                   Name = p.Name,
                   Price = p.Price,
                   Buyer = p.Buyer.FirstName + " " +p.Buyer.LastName
               })
               .OrderBy(p => p.Price)
               .Take(10)
               .ToArray();

            xmlSerializer.Serialize(stringWriter, productDto, namespaces);

            return sb.ToString().Trim();
        }


        //Query 6. Sold Products
        public static string GetSoldProducts(ProductShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            StringWriter stringWriter = new StringWriter(sb);

            XmlSerializer xmlSerializer = GenerateSerializer("Users", typeof(SoldProduct[]));

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, String.Empty);

            var productDto = context.Users
                .Where(u => u.ProductsSold.Any())
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FirstName)
                .Select(s => new SoldProduct
                {
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    SoldProducts = s.ProductsSold.Select(ps => new ProductDto
                    {
                        Name = ps.Name,
                        Price = ps.Price
                    }).ToArray()
                })
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Take(5)
                .ToArray();
               


            xmlSerializer.Serialize(stringWriter, productDto, namespaces);

            return sb.ToString().Trim();
        }

        //Query 7. Categories By Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            StringWriter stringWriter = new StringWriter(sb);

            XmlSerializer xmlSerializer = GenerateSerializer("Categories", typeof(ExportCategoryByProductCountDto[]));

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, String.Empty);

            var productDto = context.Categories
                .Select(c => new ExportCategoryByProductCountDto
                {
                    Name = c.Name,
                    Count=c.CategoryProducts.Count,
                    AveragePrice=c.CategoryProducts.Average(x=>x.Product.Price),
                    TotalRevenue = c.CategoryProducts.Sum(x=>x.Product.Price)

                })
                .OrderByDescending(p=>p.Count)
                .ThenBy(t=>t.TotalRevenue)
                .ToArray();



            xmlSerializer.Serialize(stringWriter, productDto, namespaces);

            return sb.ToString().Trim();
        }

        private static XmlSerializer GenerateSerializer(string rootName, Type dtoType)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);

            XmlSerializer xmlSerializer = new XmlSerializer(dtoType, xmlRoot);

            return xmlSerializer;
        }

    }
}
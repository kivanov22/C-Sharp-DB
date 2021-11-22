using ProductShop.Data;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            ProductShopContext db = new ProductShopContext();

            //Imports
            string inputXml = File.ReadAllText("./Datasets/users.xml");
            string result = ImportUsers(db, inputXml);
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
                    FirstName=userDto.FirstName,
                    LastName=userDto.LastName,
                    Age=userDto.Age
                };
                users.Add(u);
            }

            context.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }


        private static XmlSerializer GenerateSerializer(string rootName, Type dtoType)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);

            XmlSerializer xmlSerializer = new XmlSerializer(dtoType, xmlRoot);

            return xmlSerializer;
        }

    }
}
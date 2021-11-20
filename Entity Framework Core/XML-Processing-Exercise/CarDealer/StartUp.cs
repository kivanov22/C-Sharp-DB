using CarDealer.Data;
using CarDealer.DTO.ImportDto;
using CarDealer.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            CarDealerContext db = new CarDealerContext();


            //ResetDb(db);

            //      or ../../../
            string inputXml = File.ReadAllText("./Datasets/suppliers.xml");
            string result = ImportSuppliers(db, inputXml);
            Console.WriteLine(result);
        }

        //Query 9. Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            XmlRootAttribute xmlRootAttribute = new XmlRootAttribute("Suppliers");
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportSupplierDto[]), xmlRootAttribute);

            using StringReader stringReader = new StringReader(inputXml);

            //cast to dto[]
            ImportSupplierDto[] dtos = (ImportSupplierDto[])
                xmlSerializer.Deserialize(stringReader);

            ICollection<Supplier> suppliers = new HashSet<Supplier>();
            foreach (var supplierDto in dtos)
            {
                Supplier s = new Supplier()
                {
                    Name = supplierDto.Name,
                    IsImporter = bool.Parse(supplierDto.IsImporter)
                };
                suppliers.Add(s);
            }

            context.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}";
        }


        private static void ResetDb(CarDealerContext dbContext)
        {

            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();


            Console.WriteLine("Db created success !");
        }
    }
}
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
            string inputXml = File.ReadAllText("./Datasets/parts.xml");
            string result = ImportParts(db, inputXml);
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


        //Query 10. Import Parts
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute("Parts");

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportPartDto[]), xmlRoot);

            using StringReader stringReader = new StringReader(inputXml);

            ImportPartDto[] partDtos = (ImportPartDto[])xmlSerializer.Deserialize(stringReader);

            ICollection<Part> parts = new HashSet<Part>();

            foreach (var partDto in partDtos)
            {
                //check for supplier if it has id
                Supplier supplier = context.Suppliers.Find(partDto.SupplierId);


                //if not we continue
                if (supplier==null)
                {
                    continue;
                }

                //create a new part and use the supplier if it exists
                Part p = new Part()
                {
                    Name = partDto.Name,
                    Price = decimal.Parse(partDto.Price),
                    Quantity = partDto.Quantity,
                    Supplier = supplier
                };

                parts.Add(p);
            }

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}";
        }


        private static void ResetDb(CarDealerContext dbContext)
        {

            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();


            Console.WriteLine("Db created success !");
        }
    }
}
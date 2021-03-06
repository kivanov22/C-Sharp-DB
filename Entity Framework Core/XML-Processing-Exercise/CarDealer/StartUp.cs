using CarDealer.Data;
using CarDealer.DTO.ExportDto;
using CarDealer.DTO.ImportDto;
using CarDealer.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

            //Imports
            //string inputXml = File.ReadAllText("./Datasets/sales.xml");
            // string result = ImportSales(db, inputXml);
            // Console.WriteLine(result);

            //Exports
            string result = GetSalesWithAppliedDiscount(db);
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
                if (supplier == null)
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


        //Query 11. Import Cars
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = GenerateSerializer("Cars", typeof(ImportCarDto[]));

            using StringReader stringReader = new StringReader(inputXml);

            ImportCarDto[] importCarDtos = (ImportCarDto[])xmlSerializer.Deserialize(stringReader);

            ICollection<Car> cars = new HashSet<Car>();

            foreach (var carDto in importCarDtos)
            {

                Car car = new Car
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TravelledDistance = carDto.TraveledDistance,
                };

                ICollection<PartCar> currentCarParts = new HashSet<PartCar>();

                //check for unique id-s
                foreach (int partId in carDto.Parts.Select(p => p.Id).Distinct())
                {
                    Part part = context
                        .Parts
                        .Find(partId);

                    if (part == null)
                    {
                        continue;
                    }
                    PartCar partCar = new PartCar()
                    {
                        Car = car,
                        Part = part
                    };
                    car.PartCars = currentCarParts;
                    currentCarParts.Add(partCar);

                }

                cars.Add(car);
            }
            context.Cars.AddRange(cars);
            context.SaveChanges();


            return $"Successfully imported {cars.Count}";
        }

        //Query 12. Import Customers
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = GenerateSerializer("Customers", typeof(ImportCustomerDto[]));
            using StringReader stringReader = new StringReader(inputXml);

            ImportCustomerDto[] customerDto = (ImportCustomerDto[])xmlSerializer.Deserialize(stringReader);

            ICollection<Customer> customers = new HashSet<Customer>();

            foreach (var cus in customerDto)
            {
                Customer c = new Customer()
                {
                    Name = cus.Name,
                    BirthDate = cus.BirthDate,
                    IsYoungDriver = cus.IsYoungDriver
                };

                customers.Add(c);
            }

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}";
        }


        //Query 13. Import Sales
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = GenerateSerializer("Sales", typeof(ImportSaleDto[]));
            using StringReader stringReader = new StringReader(inputXml);

            ImportSaleDto[] salesDto = (ImportSaleDto[])xmlSerializer.Deserialize(stringReader);

            var sales = salesDto
                .Where(sd => context.Cars.Any(c => c.Id == sd.CarId))
                .Select(x => new Sale()
                {
                    CarId = x.CarId,
                    CustomerId = x.CustomerId,
                    Discount = x.Discount
                })
                .ToList();


            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}";
        }

        //Query 14. Cars With Distance
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            StringBuilder sb = new StringBuilder();

            using StringWriter stringWriter = new StringWriter(sb);

            XmlSerializer xmlSerializer = GenerateSerializer("cars", typeof(ExportCarWithDistanceDto[]));

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, String.Empty);

            ExportCarWithDistanceDto[] carsDtos = context.Cars
                .Where(c => c.TravelledDistance > 2000000)
                .OrderBy(x => x.Make)
                .ThenBy(x => x.Model)
                .Take(10)
                .Select(c => new ExportCarWithDistanceDto()
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance.ToString()//its long so we give it ToString
                })
                .ToArray();

            xmlSerializer.Serialize(stringWriter, carsDtos, namespaces);

            return sb.ToString().Trim();
        }

        //Query 15. Cars from make BMW
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            StringBuilder sb = new StringBuilder();

            using StringWriter stringWriter = new StringWriter(sb);

            XmlSerializer xmlSerializer = GenerateSerializer("cars", typeof(ExportCarFromMakeDto[]));

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, String.Empty);

            ExportCarFromMakeDto[] carDtos = context.Cars
                .Where(m => m.Make == "BMW")
                .OrderBy(a => a.Model)
                .ThenByDescending(d => d.TravelledDistance)
                .Select(x => new ExportCarFromMakeDto()
                {
                    Id = x.Id,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance.ToString()
                })
                .ToArray();

            xmlSerializer.Serialize(stringWriter, carDtos, namespaces);

            return sb.ToString().Trim();
        }

        //Query 16. Local Suppliers
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            StringBuilder sb = new StringBuilder();

            using StringWriter stringWriter = new StringWriter(sb);

            XmlSerializer xmlSerializer = GenerateSerializer("suppliers", typeof(ExportSupplierDto[]));

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, String.Empty);

            ExportSupplierDto[] supplierDtos = context.Suppliers
                .Where(m => m.IsImporter == false)
                .Select(x => new ExportSupplierDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    PartsCount = x.Parts.Count
                })
                .ToArray();

            xmlSerializer.Serialize(stringWriter, supplierDtos, namespaces);

            return sb.ToString().Trim();
        }

        //Query 17. Cars with Their List of Parts
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            StringBuilder sb = new StringBuilder();

            using StringWriter stringWriter = new StringWriter(sb);

            XmlSerializer xmlSerializer = GenerateSerializer("cars", typeof(ExportCarWithListOfPartDto[]));

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, String.Empty);

            ExportCarWithListOfPartDto[] carPartsDtos = context.Cars
                .Select(x => new ExportCarWithListOfPartDto
                {
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance,
                    Parts = x.PartCars.Select(p => new ExportPartDto
                    {
                        Name = p.Part.Name,
                        Price = p.Part.Price
                    })
                    .OrderByDescending(p => p.Price)
                    .ToArray()
                })
                .OrderByDescending(d => d.TravelledDistance)
                .ThenBy(m => m.Model)
                .Take(5)
                .ToArray();

            xmlSerializer.Serialize(stringWriter, carPartsDtos, namespaces);

            return sb.ToString().Trim();
        }


        //Query 18. Total Sales by Customer
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            StringBuilder sb = new StringBuilder();

            using StringWriter stringWriter = new StringWriter(sb);

            XmlSerializer xmlSerializer = GenerateSerializer("customers", typeof(ExportTotalSaleByCustomerDto[]));

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, String.Empty);

            ExportTotalSaleByCustomerDto[] salesCustomerDto = context.Customers
                .Where(c => c.Sales.Any())
                .Select(x => new ExportTotalSaleByCustomerDto
                {
                    FullName = x.Name,
                    BoughtCars = x.Sales.Count,
                    SpentMoney = x.Sales.SelectMany(s => s.Car.PartCars).Sum(c => c.Part.Price)
                })
                .OrderByDescending(d => d.SpentMoney)
                .ToArray();

            xmlSerializer.Serialize(stringWriter, salesCustomerDto, namespaces);

            return sb.ToString().Trim();
        }


        //Query 19. Sales with Applied Discount
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            StringBuilder sb = new StringBuilder();

            using StringWriter stringWriter = new StringWriter(sb);

            XmlSerializer xmlSerializer = GenerateSerializer("sales", typeof(ExportSaleWithAppliedDiscountDto[]));

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(String.Empty, String.Empty);

            ExportSaleWithAppliedDiscountDto[] salesCustomerDto = context.Sales
                .Select(x => new ExportSaleWithAppliedDiscountDto
                {
                    Car = new CarInfo()
                    {
                        Make = x.Car.Make,
                        Model = x.Car.Model,
                        TravelledDistance = x.Car.TravelledDistance,
                    },
                    Discount = x.Discount,
                    CustomerName = x.Customer.Name,
                    Price = x.Car.PartCars.Sum(p => p.Part.Price),
                    PriceWithDiscount = (x.Car.PartCars.Sum(s => s.Part.Price) - (x.Car.PartCars.Sum(p => p.Part.Price) * x.Discount / 100))
                })
                .ToArray();

            xmlSerializer.Serialize(stringWriter, salesCustomerDto, namespaces);

            return sb.ToString().Trim();
        }

        private static XmlSerializer GenerateSerializer(string rootName, Type dtoType)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);

            XmlSerializer xmlSerializer = new XmlSerializer(dtoType, xmlRoot);

            return xmlSerializer;
        }

        private static void ResetDb(CarDealerContext dbContext)
        {

            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();


            Console.WriteLine("Db created success !");
        }
    }
}
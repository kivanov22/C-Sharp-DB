namespace CarDealer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using AutoMapper;
    using CarDealer.Data;
    using CarDealer.DTO;
    using CarDealer.Models;
    using Newtonsoft.Json;

    public class StartUp
    {
        private static IMapper mapper;

        public static void Main(string[] args)
        {
            CarDealerContext context = new CarDealerContext();
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();


            //var customersJson = File.ReadAllText("Datasets/customers.json");
            //Console.WriteLine(ImportCustomers(context, customersJson));

            Console.WriteLine(GetCarsFromMakeToyota(context));
            //Console.WriteLine(ImportSuppliers(context));
        }

        //Query 8. Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            IEnumerable<SupplierDto> suppliers = JsonConvert.DeserializeObject<IEnumerable<SupplierDto>>(inputJson);

            InitializeMapper();

            var mappedSuppliers = mapper.Map<IEnumerable<Supplier>>(suppliers);

            context.AddRange(mappedSuppliers);
            context.SaveChanges();

            return $"Successfully imported {mappedSuppliers.Count()}.";
        }

        //Query 9. Import Parts
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var supplierId = context.Suppliers.Select(x => x.Id).ToArray();

            var parts = JsonConvert.DeserializeObject<IEnumerable<Part>>(inputJson)
                .Where(s => supplierId.Contains(s.SupplierId))
                .ToList();


            //IEnumerable<PartDto> parts = JsonConvert.DeserializeObject<IEnumerable<PartDto>>(inputJson);//??check

            //InitializeMapper();

            //var mappedParts = mapper.Map<IEnumerable<Part>>(parts).Where(p => p.Id != p.Supplier.);

            context.AddRange(parts);
            context.SaveChanges();


            return $"Successfully imported {parts.Count()}.";
        }

        //Query 10. Import Cars
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            IEnumerable<CarDto> input = JsonConvert.DeserializeObject<IEnumerable<CarDto>>(inputJson);

            List<Car> cars = new List<Car>();

            foreach (var currentCar in input)
            {
                Car car = new Car()
                {
                    Make = currentCar.Make,
                    Model = currentCar.Model,
                    TravelledDistance = currentCar.TravelledDistance
                };

                foreach (var partId in currentCar.PartsId.Distinct())
                {
                    car.PartCars.Add(new PartCar()
                    {
                        Car = car,
                        PartId=partId
                    });
                }
                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}.";
        }


        //Query 11. Import Customers
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<IEnumerable<Customer>>(inputJson);

            context.Customers.AddRange(customers);
            context.SaveChanges();

           return $"Successfully imported {customers.Count()}.";
        }

        //Query 12. Import Sales
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sales = JsonConvert.DeserializeObject<IEnumerable<Sale>>(inputJson);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count()}.";
        }


        //Query 13. Export Ordered Customers
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .Select(c => new
                {
                    Name = c.Name,
                    BirthDate = c.BirthDate,
                    IsYoungDriver=c.IsYoungDriver
                })
                .OrderBy(b=>b.BirthDate)
                .ThenBy(y=>y.IsYoungDriver)
                .ToList();

            var jsonSettings = new JsonSerializerSettings()
            {
                DateFormatString="dd/MM/yyyy",
                Formatting=Formatting.Indented
            };

            var json = JsonConvert.SerializeObject(customers, jsonSettings);

            return json;
        }

        //Export 15.Cars from Make Toyota
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var carModels = context.Cars
                .Where(m=>m.Make=="Toyota")
                .Select(c => new
                {
                    Id = c.Id,
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .OrderBy(m => m.Model)
                .ThenByDescending(d => d.TravelledDistance)
                .ToList();

            var jsonSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            };

            var json = JsonConvert.SerializeObject(carModels, jsonSettings);

            return json;
        }

        //Query 14. Export Local Suppliers
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                    PartsCount = x.Parts.Count
                })
                .ToList();

            var jsonSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented
            };

            var json = JsonConvert.SerializeObject(suppliers, jsonSettings);

            return json;
        }

        private static void InitializeMapper()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });

            mapper = new Mapper(mapperConfiguration);
        }
    }
}
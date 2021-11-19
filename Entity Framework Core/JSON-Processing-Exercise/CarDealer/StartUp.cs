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
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

         
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
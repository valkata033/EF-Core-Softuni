using CarDealer.Data;
using CarDealer.Dtos.Export;
using CarDealer.Dtos.Import;
using CarDealer.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static object XmlSerializedrNameSpaces { get; private set; }

        public static void Main(string[] args)
        {
            CarDealerContext dbContext = new CarDealerContext();
            //string xml = File.ReadAllText("../../../Datasets/sales.xml");

            string result = GetSalesWithAppliedDiscount(dbContext);
            Console.WriteLine(result);

            //dbContext.Database.EnsureDeleted();
            //dbContext.Database.EnsureCreated();

            //Console.WriteLine("Ensured!");
        }

        //Import Data
        //Problem 09
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            ImportSuppliersDto[] supplierDtos = 
                Deserialize<ImportSuppliersDto[]>(inputXml, "Suppliers");

            Supplier[] suppliers = supplierDtos
                .Select(s => new Supplier
                {
                    Name = s.Name,
                    IsImporter = s.IsImporter
                })
                .ToArray();

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Length}";
        }

        //Problem 10
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            ImportPartsDto[] partsDtos = Deserialize<ImportPartsDto[]>(inputXml, "Parts");

            ICollection<Part> parts = new List<Part>();

            foreach (ImportPartsDto pDto in partsDtos)
            {
                if (!context.Suppliers.Any(s => s.Id == pDto.SupplierId))
                {
                    continue;
                }

                Part part = new Part
                {
                    Name = pDto.Name,
                    Price = pDto.Price,
                    Quantity = pDto.Quantity,
                    SupplierId = pDto.SupplierId
                };

                parts.Add(part);
            }

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}";
        }

        //Problem 11
        public static string ImportCars(CarDealerContext context, string inputXml) 
        {
            ImportCarsDto[] carsDtos = Deserialize<ImportCarsDto[]>(inputXml, "Cars");

            ICollection<Car> cars = new List<Car>();
            foreach (ImportCarsDto cDto in carsDtos)
            {
                Car car = new Car
                {
                    Make = cDto.Make,
                    Model = cDto.Model,
                    TravelledDistance = cDto.TraveledDistance
                };

                ICollection<PartCar> carParts = new List<PartCar>();
                foreach (int partId in cDto.Parts.Select(p => p.Id).Distinct())
                {
                    if (!context.Parts.Any(p => p.Id == partId))
                    {
                        continue;
                    }

                    carParts.Add(new PartCar
                    {
                        Car = car,
                        PartId = partId
                    });
                }

                car.PartCars = carParts;
                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        //Problem 12
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            ImportCustomersDto[] customersDtos = 
                Deserialize<ImportCustomersDto[]>(inputXml, "Customers");

            ICollection<Customer> customers = new List<Customer>();
            foreach (ImportCustomersDto cDto in customersDtos)
            {
                Customer customer = new Customer
                {
                    Name = cDto.Name,
                    BirthDate = DateTime.Parse(cDto.BirthDate),
                    IsYoungDriver = cDto.IsYoungDriver
                };

                customers.Add(customer);
            }
            context.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}";
        }

        //Problem 13
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            ImportSalesDto[] salesDtos = Deserialize<ImportSalesDto[]>(inputXml, "Sales");

            ICollection<Sale> sales = new List<Sale>();
            foreach (ImportSalesDto sDto in salesDtos)
            {
                if (!context.Cars.Any(c => c.Id == sDto.CarId))
                {
                    continue;
                }

                Sale sale = new Sale
                {
                    CarId = sDto.CarId,
                    CustomerId = sDto.CustomerId,
                    Discount = sDto.Discount
                };

                sales.Add(sale);
            }

            context.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}"; 
        }

        //ExportData
        //Problem 14
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            ExportCarsWithDistanceDto[] carsDtos = context.Cars
                .Where(x => x.TravelledDistance > 2000000)
                .OrderBy(x => x.Make)
                .ThenBy(x => x.Model)
                .Take(10)
                .Select(x => new ExportCarsWithDistanceDto 
                {
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                })
                .ToArray();

            return Serializer<ExportCarsWithDistanceDto[]>(carsDtos, "cars");
        }

        //Problem 15
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            ExportCarsFromMakeBMWDto[] carsDtos = context.Cars
                .Where(x => x.Make == "BMW")
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .Select(x => new ExportCarsFromMakeBMWDto
                {
                    Id = x.Id,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                })
                .ToArray();

            return Serializer<ExportCarsFromMakeBMWDto[]>(carsDtos, "cars");
        }

        //Problem 16
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            ExportLocalSuppliersDto[] suppliersDtos = context.Suppliers
                .Where(x => !x.IsImporter)
                .Select(x => new ExportLocalSuppliersDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    PartsCount = x.Parts.Count
                })
                .ToArray();

            return Serializer<ExportLocalSuppliersDto[]>(suppliersDtos, "suppliers");
        }

        //Problem 17
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            ExportCarsWithTheirListOfPartsDto[] carsDtos = context
                .Cars
                .Select(c => new ExportCarsWithTheirListOfPartsDto
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance,
                    Parts = c.PartCars
                        .Select(cp => new ExportPartsOfCarDto
                        {
                            Name = cp.Part.Name,
                            Price = cp.Part.Price
                        })
                        .OrderByDescending(x => x.Price)
                        .ToArray()
                })
                .OrderByDescending(x => x.TravelledDistance)
                .ThenBy(x => x.Model)
                .Take(5)
                .ToArray();

            return Serializer<ExportCarsWithTheirListOfPartsDto[]>(carsDtos, "cars");
        }

        //Problem 18
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            ExportTotalSalesByCustomerDto[] customersDtos = context
                .Customers
                .Where(x => x.Sales.Count > 0)
                .Select(x => new ExportTotalSalesByCustomerDto
                {
                    FullName = x.Name,
                    BoughtCars = x.Sales.Count(),
                    SpentMoney = x.Sales.Sum(c => c.Discount)
                })
                .OrderByDescending(x => x.SpentMoney)
                .ToArray();

            return Serializer<ExportTotalSalesByCustomerDto[]>(customersDtos, "customers");
        }

        //Problem 19
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            ExportSalesWithAppliedDiscountDto[] salesDtos = context.Sales
                .Select(x => new ExportSalesWithAppliedDiscountDto
                {
                    Car = new ExportCarsDto
                    {
                        Make = x.Car.Make,
                        Model = x.Car.Model,
                        TravelledDistance = x.Car.TravelledDistance
                    },
                    CustomerName = x.Customer.Name,
                    Discount = x.Discount,
                    Price = x.Car.PartCars.Sum(cp => cp.Part.Price),
                    PriceWithDiscount = x.Car.PartCars.Sum(cp => cp.Part.Price) 
                                - (x.Car.PartCars.Sum(cp => cp.Part.Price) * (x.Discount / 100))
                })
                .ToArray();

            return Serializer<ExportSalesWithAppliedDiscountDto[]>(salesDtos, "sales");
        }


        //Helper methods
        private static T Deserialize<T>(string inputXml, string rootName)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), xmlRoot);

            using StringReader reader = new StringReader(inputXml);
            T dtos = (T)xmlSerializer.Deserialize(reader);

            return dtos;
        }

        private static string Serializer<T>(T dto, string rootName)
        {
            StringBuilder sb = new StringBuilder();

            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), xmlRoot);

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using StringWriter writer = new StringWriter(sb);
            xmlSerializer.Serialize(writer, dto, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}
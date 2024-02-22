using Bogus;
using Bogus.Extensions.Romania;
using InventoryManagementSystem.Model;
using InventoryManagementSystem.Services;
using System.Text.Json;
using System.Windows.Media;

namespace InventoryManagementSystem.AppConfiguration;


public static class Configuration
{


    public static string? AdminPassword { get; set; } = "1122";

    public static string OrderChequesDirectoryPath { get; set; } = "D://";

    public static double CurrencyRate { get; set; } = 12430;


    // Fontsize
    // FontFamily



    public static List<Product> GenerateFakeProducts(int numberOfProducts)
    {
        Randomizer.Seed = new Random(8675309);

        Faker<Product> faker;




        faker = new Faker<Product>()
       .RuleFor(p => p.Name, f => f.Commerce.ProductName())
       .RuleFor(p => p.Code, f => f.Commerce.Ean13())
       .RuleFor(p => p.RealPrice, f => f.Random.Number(100, 100000))
       .RuleFor(p => p.Price, f => f.Random.Number(100, 100000))
       .RuleFor(p => p.Quantity, f => f.Random.Number(1, 500))
       // .RuleFor(p => p.USDPrice, f => f.Random.Number(100, 100000))
       // .RuleFor(p => p.USDPriceForCustomer, f => f.Random.Number(100, 100000))
       .RuleFor(p => p.StoredAt, f => f.Date.Recent())
       //.RuleFor(p => p.QuantitySold, f => f.Random.Number(0, 50))
       .RuleFor(p => p.CompanyId, f => f.Random.Number(1, 10)) // Assuming you have 10 companies
       .RuleFor(p => p.CarTypeId, f => f.Random.Number(1, 11)) // Assuming you have 5 car types
       .RuleFor(p => p.CountryId, f => f.Random.Number(1, 8)) // Assuming you have 20 countries
       .RuleFor(p => p.SetTypeId, f => f.Random.Number(1, 2)); // Assuming you have 3 set types






        return faker.Generate(numberOfProducts);
    }


    public static List<Company> GenerateFakeCompanies(int numberOfCompanies)
    {
        var faker = new Faker<Company>()
            .RuleFor(c => c.Name, f => f.Company.CompanyName());

        return faker.Generate(numberOfCompanies);
    }

    public static List<Country> GenerateFakeCountries(int numberOfCountries)
    {
        var faker = new Faker<Country>()
            .RuleFor(c => c.Name, f => f.Address.Country());

        return faker.Generate(numberOfCountries);
    }

    public static List<CarType> GenerateFakeCars(int numberOfCountries)
    {
        var faker = new Faker<CarType>()
            .RuleFor(c => c.Name, f => f.Vehicle.Model());

        return faker.Generate(numberOfCountries);
    }

    public static List<SetType> GenerateFakeSetTypes()
    {
        return new List<SetType> { new() { Id = 1, Name = "Dona" }, new() { Id = 2, Name = "Komplekt" } };
    }




}

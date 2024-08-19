using Bogus;
using InventoryManagementSystem.Model;

namespace InventoryManagementSystem.Shared
{
    public static class Shared
    {
        public static bool IsCartEmpty { get; set; } = true;


        public static void SeedFakeProducts(int count)
        {
            var db = new AppDbContext();
            var products = GenerateFakeProducts(count);
            db.Products.AddRange(products);
            db.SaveChanges();
        }

       
        public static List<Product> GenerateFakeProducts(int count)
        {
            var fakeProductGenerator = new Faker<Product>()
                // .RuleFor(p => p.Id, f => f.IndexFaker + 1)
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Code, f => f.Random.AlphaNumeric(6))

                .RuleFor(p => p.Barcode, f => f.Random.AlphaNumeric(13))

               // .RuleFor(p => p.RealPrice, f => f.Random.Number(1, 100))
              //  .RuleFor(p => p.Price, f => f.Random.Number(100, 10000))
                .RuleFor(p => p.Quantity, f => f.Random.Number(100, 10000))
                .RuleFor(p => p.USDPrice, f => f.Random.Number(1, 100))
                .RuleFor(p => p.USDPriceForCustomer, f => f.Random.Number(1, 100))
                .RuleFor(p => p.StoredAt, f => f.Date.Past())
                .RuleFor(p => p.QuantitySold, f => f.Random.Number(0, 200))
                .RuleFor(p => p.CompanyId, f => f.Random.Number(1, 6)+2) // Update with your actual CompanyId generation logic
                .RuleFor(p => p.CarTypeId, f => f.Random.Number(1, 6)+2) // Update with your actual CarTypeId generation logic
                .RuleFor(p => p.CountryId, f => f.Random.Number(1, 6)+2) // Update with your actual CountryId generation logic
                .RuleFor(p => p.SetTypeId, f => f.Random.Number(1, 3)+1);// Update with your actual SetTypeId generation logic
              //  .RuleFor(p => p.OrderDetails, f => Enumerable.Empty<OrderDetail>().ToList()); // Empty list for OrderDetails

            return fakeProductGenerator.Generate(count);
        }
    }
}

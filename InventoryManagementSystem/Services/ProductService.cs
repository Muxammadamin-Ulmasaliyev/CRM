using ControlzEx.Standard;
using InventoryManagementSystem.Model;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Services
{
    public class ProductService
    {
        private readonly AppDbContext dbContext;

        public ProductService(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }



        public void AddProducts(List<Product> products)
        {
            products.RemoveAt(0);
            dbContext.Products.AddRange(products);
            dbContext.SaveChanges();
        }

        public void AddProduct(Product product)
        {
            dbContext.Products.Add(product);
            dbContext.SaveChanges();
        }

        public double CalculateNetWorth()
        {
            double sum = 0, sumUsd = 0;
            sum = (double)dbContext.Products.Take(5).Sum(p => p.Quantity * p.RealPrice);
            sumUsd = (double)dbContext.Products.Take(5).Sum(p => p.Quantity * p.USDPrice * Properties.Settings.Default.CurrencyRate);

            return sum + sumUsd;
        }

        public Product GetProduct(int id)
        {
            return dbContext.Products.Find(id);
        }

        public void UpdateQuantity(Product productToUpdate)
        {
            var existingProduct = dbContext.Products.Find(productToUpdate.Id);
            if (existingProduct != null)
            {
                dbContext.Entry(existingProduct).CurrentValues.SetValues(productToUpdate);
                dbContext.SaveChanges();
            }

        }

        public void Update(Product productToUpdate)
        {
            dbContext.Products.Update(productToUpdate);
            dbContext.SaveChanges();
        }

        public void Delete(Product product)
        {
            dbContext.Products.Remove(product);
            dbContext.SaveChanges();
        }

        public List<Product> GetAll()
        {
            return dbContext.Products.Include(p => p.Company).Include(p => p.Country).Include(p => p.CarType).Include(p => p.SetType).ToList();
        }

        public List<Product> GetTopSoldProducts(int numberOfProducts)
        {
            return dbContext.Products
                    .Include(p => p.Company)
                    .Include(p => p.Country)
                    .Include(p => p.CarType)
                    .OrderByDescending(p => p.QuantitySold)
                    .Take(numberOfProducts)
                    .ToList();

        }

        public bool IsProductCodeExists(string productCode, string barcode, out Product? product)
        {
            product = dbContext.Products.FirstOrDefault(p => p.Code == productCode || p.Barcode == barcode);
            if (product != null)
            {
                return true;
            }
            return false;
        }

       

        public Product GetProductByBarcode(string barcode)
        {
            return dbContext.Products.Include(p => p.Company).Include(p => p.Country).Include(p => p.CarType).Include(p => p.SetType).FirstOrDefault(p => p.Barcode == barcode);

        }
    }
}

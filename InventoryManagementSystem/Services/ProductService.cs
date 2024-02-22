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

        public bool IsProductCodeExists(string productCode, out Product? product)
        {
            product = dbContext.Products.FirstOrDefault(p => p.Code == productCode);
            if (product != null)
            {
                return true;
            }
            return false;
        }

        public List<Product> GetProductsByCategories(List<Country> selectedCountries)
        {
            var selectedCountryIds = selectedCountries.Select(c => c.Id).ToArray();

            List<Product> products = new();   

            products.AddRange(dbContext.Products.Where(p => selectedCountryIds.Contains(p.Id)));

            return products;


        }
    }
}

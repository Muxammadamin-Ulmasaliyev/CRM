using InventoryManagementSystem.Model;

namespace InventoryManagementSystem.Services
{
    public class CountryService
    {
        private readonly AppDbContext dbContext;

        public CountryService(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public bool IsCountryExists(string countryName)
        {
            return dbContext.Countries.Any(c => c.Name.ToLower() == countryName.ToLower());
        }

        public void AddCountry(string countryName)
        {
            dbContext.Countries.Add(new Country { Name = countryName });
            dbContext.SaveChanges();
        }

        public void Delete(Country country)
        {
            dbContext.Countries.Remove(country);
            dbContext.SaveChanges();
        }

        public void Update(Country editedCountry)
        {
            dbContext.Countries.Update(editedCountry);
            dbContext.SaveChanges();
        }

        public List<Country>? GetAll()
        {
            return dbContext.Countries.ToList();
        }

        
    }
}

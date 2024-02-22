using InventoryManagementSystem.Model;

namespace InventoryManagementSystem.Services
{
    public class CompanyService
    {
        private readonly AppDbContext dbContext;

        public CompanyService(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public bool IsCompanyExists(string companyName)
        {
            return dbContext.Companies.Any(c => c.Name.ToLower() == companyName.ToLower());
        }

        public void AddCompany(string companyName)
        {
            dbContext.Companies.Add(new Company { Name = companyName });
            dbContext.SaveChanges();
        }

        public void Delete(Company company)
        {
            dbContext.Companies.Remove(company);
            dbContext.SaveChanges();
        }

        public void Update(Company editedCompany)
        {
            dbContext.Companies.Update(editedCompany);
            dbContext.SaveChanges();
        }

        public List<Company>? GetAll()
        {
            return dbContext.Companies.ToList();
        }
    }
}

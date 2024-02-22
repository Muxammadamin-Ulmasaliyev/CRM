using InventoryManagementSystem.Model;

namespace InventoryManagementSystem.Services
{
    public class CustomerService
    {
        private readonly AppDbContext dbContext;


        public CustomerService(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void Update(Customer customerToUpdate)
        {
            dbContext.Customers.Update(customerToUpdate);
            dbContext.SaveChanges();
        }

        public List<Customer> GetAll()
        {
            return dbContext.Customers.ToList();

        }

        public void Delete(Customer customer)
        {
            dbContext.Customers.Remove(customer);
            dbContext.SaveChanges(); 
        }

        public void IncrementOrdersCountOfCustomer(Customer newCustomer)
        {
            var customer = dbContext.Products.Find(newCustomer.Id);
            if (customer != null)
            {
                dbContext.Entry(customer).CurrentValues.SetValues(newCustomer);
                dbContext.SaveChanges();
            }
        }
    }
}

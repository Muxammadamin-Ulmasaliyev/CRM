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

        public void Add(Customer customer)
        {
            dbContext.Customers.Add(customer);
            dbContext.SaveChanges();
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

       /* public double CalculateDebtAmount()
        {
            return dbContext.Customers.Where(c => c.Debt < 0).Sum(c => Math.Abs(c.Debt));
        }*/

        public double CalculateDebtAmountOfCustomers()
        {
           // return dbContext.Customers.Where(c => c.Debt > 0).Sum(c => c.Debt);
            return dbContext.Customers.Sum(c => c.Debt);
        }

        public int GetCustomersCount()
        {
            return dbContext.Customers.Count();
        }

        public List<Customer> GetPageOfCustomers(int startIndex, int pageSize)
        {
            return dbContext.Customers.Skip(startIndex).Take(pageSize).ToList();
        }
    }
}

using InventoryManagementSystem.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace InventoryManagementSystem.Services
{
    public class OrderService
    {
        private readonly AppDbContext dbContext;

        public OrderService(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        public EntityEntry<Order> AddOrder(Order order)
        {
            var orderEntity = dbContext.Orders.Add(order);
            dbContext.SaveChanges();
            return orderEntity;
        }

        public List<Order> GetOrdersByCustomerId(int customerId)
        {
            return dbContext.Orders.Where(o => o.CustomerId == customerId).ToList();
        }

        public Order GetOrderById(int selectedOrderId)
        {
            return dbContext.Orders.Include(o => o.Customer).Include(o => o.OrderDetails).FirstOrDefault(o => o.Id == selectedOrderId);
        }








        // #################################################################################################################################
        public Dictionary<int, double> GetMonthlySales()
        {
            double sum = 0;
            var salesInYear = new Dictionary<int, double>();
            // var salesInYear = new List<double>();
            var monthlySalesGrouping = dbContext.Orders.GroupBy(o => o.OrderDate.Month).ToList();
            foreach (var salesInMonth in monthlySalesGrouping)
            {
                foreach (var order in salesInMonth)
                {
                    sum += order.TotalAmount;
                }
                salesInYear.Add(salesInMonth.Key, sum);
                //      salesInYear.Add(sum);
                sum = 0;
            }


            // check everymonth sales
            for (int i = 1; i <= 12; i++)
            {
                if (!salesInYear.Keys.Contains(i))
                {
                    salesInYear.Add(i, 0);
                }
            }


            //salesInYear = (Dictionary<int, double>)salesInYear.OrderBy(sy => sy.Key);
            return salesInYear.OrderBy(pair => pair.Key).ToDictionary(pair => pair.Key, pair => pair.Value);

        }

        public List<TopCustomerDto> GetTopCustomers(int numberOfTopCustomers)
        {
            var customers = dbContext.Customers.Include(c => c.Orders).ToList();
            var topCustomers = customers.OrderByDescending(c => c.Orders.Select(o => o.TotalAmount).Sum()).Take(numberOfTopCustomers).ToList();
            var topCustomersDto = topCustomers.Select(c => new TopCustomerDto
            {
                Name = c.Name,  // Replace with the actual property name of the customer's name
                NoOfOrders = c.Orders.Count(),  // Assuming 'Orders' is a collection property in Customer
                TotalAmountOfOrders = c.Orders.Sum(o => o.TotalAmount)  // Assuming 'TotalAmount' is a property in Order
            }).ToList();

            return topCustomersDto;

        }

        // #################################################################################################################################

    }
}

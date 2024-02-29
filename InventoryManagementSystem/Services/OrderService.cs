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
            /* return dbContext.Orders
                     .Include(o => o.Customer)

                     .Include(o => o.OrderDetails)
                     .ThenInclude(od => od.Product)
                     .ThenInclude(p => p.CarType)

                     .Include(o => o.OrderDetails)
                     .ThenInclude(od => od.Product)
                     .ThenInclude(p => p.Country)

                     .Include(o => o.OrderDetails)
                     .ThenInclude(od => od.Product)
                     .ThenInclude(p => p.Company)


                     .First(o => o.Id == selectedOrderId);*/

            return dbContext.Orders.Include(o => o.Customer).Include(o => o.OrderDetails).First(o => o.Id == selectedOrderId);
        }
    }
}

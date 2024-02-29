using InventoryManagementSystem.Model;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Services
{
    public class OrderDetailService
    {
        private readonly AppDbContext dbContext;

        public OrderDetailService(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        public void AddOrderDetail(OrderDetail orderDetail)
        {
            dbContext.OrderDetails.Add(orderDetail);
            dbContext.SaveChanges();
        }

        public bool ProductExists(int productId)
        {
            return dbContext.OrderDetails.Any(od => od.ProductId == productId);
        }
        public List<OrderDetail> GetOrderDetailsByOrderId(int orderId)
        {
            /*return dbContext.OrderDetails
                         .Include(o => o.Product)
                         .ThenInclude(p => p.Company)
                         .Include(o => o.Product)
                         .ThenInclude(p => p.CarType)
                         .Include(o => o.Product)
                         .ThenInclude(p => p.Country)
                         .Where(o => o.OrderId == orderId)
                         .ToList();*/

            return dbContext.OrderDetails
                        .Where(o => o.OrderId == orderId)
                        .ToList();
        }


    }
}

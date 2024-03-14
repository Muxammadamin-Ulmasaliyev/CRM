using InventoryManagementSystem.Model;

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

        public bool IsProductSoldAtLeastOneTime(int productId)
        {
            return dbContext.OrderDetails.Any(od => od.ProductId == productId);
        }
        public List<OrderDetail> GetAllOrderDetailsByOrderId(int orderId)
        {
            return dbContext.OrderDetails
                        .Where(o => o.OrderId == orderId)
                        .ToList();
        }

        //////////////////////////////////////////////////////////////////////////////////////////
        public double CalculateAverageIncomePercentage()
        {
            if (dbContext.OrderDetails.Count() > 0)
            {
                return dbContext.OrderDetails.Select(od => (od.Price / od.RealPrice) - 1).Average();
            }
            return 0;
        }


    }
}

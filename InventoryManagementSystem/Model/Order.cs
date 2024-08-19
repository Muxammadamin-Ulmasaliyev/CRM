using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Model
{
    public class Order
    {

        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public double TotalAmount { get; set; } = 0;
        public double TotalPaidAmount { get; set; } = 0;


        public List<OrderDetail> OrderDetails { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}

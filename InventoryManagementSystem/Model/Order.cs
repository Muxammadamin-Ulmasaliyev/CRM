using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Model
{
    public class Order
    {
        [Key]

        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Today;
        public double TotalAmount { get; set; } = 0;


        public List<OrderDetail> OrderDetails { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}

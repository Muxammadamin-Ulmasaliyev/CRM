using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Model
{
    public class OrderDetail
    {
        [Key]

        public int Id { get; set; }
        public double SubTotal { get; set; } = 0;
        public int Quantity { get; set; }

        public double Price { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }


    }
}

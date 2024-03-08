using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Model
{
    public class Customer
    {
        [Key]

        public int Id { get; set; }
        public string Name { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }

        public double Debt { get; set; } = 0;
        public int TotalOrdersCount { get; set; } = 0;
        public List<Order> Orders { get; set; }


    }
}

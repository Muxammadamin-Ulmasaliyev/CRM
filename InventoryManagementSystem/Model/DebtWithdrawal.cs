using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Model
{
    public class DebtWithdrawal
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.Today;

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}

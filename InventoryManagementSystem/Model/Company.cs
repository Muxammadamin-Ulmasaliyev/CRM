using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Model
{
    public class Company
    {
       

        public int Id { get; set; }
        public string Name { get; set; }

        public List<Product> Products { get; set; }
    }
}

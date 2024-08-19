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
        public double RealPrice { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string ProductName { get; set; }
        public string ProductCarType { get; set; }
        public string ProductCompany { get; set; }
        public string ProductCountry { get; set; }
        public string ProductSetType { get; set; }

        public string QuantityAndSetType
        {
            get { return $" {Quantity}\n{ProductSetType}"; }
        }

		public string CompanyAndCountry
		{
			get { return $"{ProductCompany}\n{ProductCountry}"; }
		}

	}
}

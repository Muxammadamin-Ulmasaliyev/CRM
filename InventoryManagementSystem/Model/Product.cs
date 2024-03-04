using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Model
{
    public class Product
    {
        [Key]

        public int Id { get; set; }
        public string Name { get; set; }
        public string? Code { get; set; }
        public string? Barcode { get; set; }
        public double? RealPrice { get; set; } // tannarx
        public double? Price { get; set; } // sotich narx
        public int Quantity { get; set; }

        public double? USDPrice { get; set; } // $ tanrnarx
        public double? USDPriceForCustomer { get; set; } // $ narx

        public DateTime StoredAt { get; set; } = DateTime.Now;
        public int QuantitySold { get; set; } = 0; // eng ko`p sotilgan productni aniqlash uchun

        public int CompanyId { get; set; }
        public Company Company { get; set; }

        public int CarTypeId { get; set; }
        public CarType CarType { get; set; }

        public int CountryId { get; set; }
        public Country Country { get; set; }

        public int SetTypeId { get; set; }
        public SetType SetType { get; set; }


        public List<OrderDetail> OrderDetails { get; set; }

    }
}

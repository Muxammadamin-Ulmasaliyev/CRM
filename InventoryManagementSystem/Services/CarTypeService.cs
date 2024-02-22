using InventoryManagementSystem.Model;

namespace InventoryManagementSystem.Services
{
    public class CarTypeService
    {
        private readonly AppDbContext dbContext;

        public CarTypeService(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public bool IsCarTypeExists(string carTypeName)
        {
            return dbContext.CarTypes.Any(c => c.Name.ToLower() == carTypeName.ToLower());
        }

        public void AddCarType(string carTypeName)
        {
            dbContext.CarTypes.Add(new CarType { Name = carTypeName });
            dbContext.SaveChanges();
        }

        public void Delete(CarType carType)
        {
            dbContext.CarTypes.Remove(carType);
            dbContext.SaveChanges();
        }

        public void Update(CarType editedCarType)
        {
            dbContext.CarTypes.Update(editedCarType);
            dbContext.SaveChanges();
        }

        public List<CarType>? GetAll()
        {
            return dbContext.CarTypes.ToList(); 
        }
    }
}

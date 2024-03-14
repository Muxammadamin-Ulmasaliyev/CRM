using InventoryManagementSystem.Model;

namespace InventoryManagementSystem.Services
{
    public class DebtWithdrawalService
    {
        private readonly AppDbContext dbContext;
        public DebtWithdrawalService(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public List<DebtWithdrawal> GetAllByCustomerId(int customerId)
        {
            return dbContext.DebtWithdrawals.Where(dw => dw.CustomerId == customerId).ToList();
        }

        public void Add(DebtWithdrawal debtWithdrawal)
        {
            dbContext.Add(debtWithdrawal);
            dbContext.SaveChanges();
        }
    }
}

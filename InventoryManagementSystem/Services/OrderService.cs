using InventoryManagementSystem.Model;
using MethodTimer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections;
using System.Linq;

namespace InventoryManagementSystem.Services
{
	public class OrderService
	{
		private readonly AppDbContext dbContext;

		public OrderService(AppDbContext dbContext)
		{
			this.dbContext = dbContext;
		}


		public EntityEntry<Order> AddOrder(Order order)
		{
			var orderEntity = dbContext.Orders.Add(order);
			dbContext.SaveChanges();
			return orderEntity;
		}



		// #################################################################################################################################


		public List<Order> GetOrdersByCustomerId(int customerId)
		{
			var debtWithrawals = dbContext.DebtWithdrawals.Where(dw => dw.CustomerId == customerId).ToList();

			var debtWithrawalsAsOrders = debtWithrawals.Select(dw => new Order()
			{
				OrderDate = dw.Date,
				TotalPaidAmount = dw.Amount
			}).ToList();

			var orders = dbContext.Orders.Where(o => o.CustomerId == customerId).ToList();

			orders.AddRange(debtWithrawalsAsOrders);

			orders = orders.OrderByDescending(o => o.OrderDate).ToList();

			return orders;
			//return dbContext.Orders.Where(o => o.CustomerId == customerId).OrderByDescending(o => o.OrderDate).ToList();
		}


		public Order GetOrderById(int selectedOrderId)
		{
			return dbContext.Orders.Include(o => o.Customer).Include(o => o.OrderDetails).FirstOrDefault(o => o.Id == selectedOrderId);
		}








		// #################################################################################################################################


		public Dictionary<int, double> GetMonthlySalesOfYear(int year)
		{
			double sum = 0;
			var salesInYear = new Dictionary<int, double>();
			// var salesInYear = new List<double>();
			var monthlySalesGrouping = dbContext.Orders.Where(o => o.OrderDate.Year == year).GroupBy(o => o.OrderDate.Month).ToList();
			foreach (var salesInMonth in monthlySalesGrouping)
			{
				foreach (var order in salesInMonth)
				{
					sum += order.TotalAmount;
				}
				salesInYear.Add(salesInMonth.Key, sum);
				//      salesInYear.Add(sum);
				sum = 0;
			}


			// check everymonth sales
			for (int i = 1; i <= 12; i++)
			{
				if (!salesInYear.Keys.Contains(i))
				{
					salesInYear.Add(i, 0);
				}
			}


			//salesInYear = (Dictionary<int, double>)salesInYear.OrderBy(sy => sy.Key);
			return salesInYear.OrderBy(pair => pair.Key).ToDictionary(pair => pair.Key, pair => pair.Value);

		}

		public List<TopCustomerDto> GetTopCustomers(int numberOfTopCustomers)
		{
			var customers = dbContext.Customers.Include(c => c.Orders).ToList();
			var topCustomers = customers.OrderByDescending(c => c.Orders.Select(o => o.TotalAmount).Sum()).Take(numberOfTopCustomers).ToList();

			var topCustomersDto = topCustomers.Select(c => new TopCustomerDto
			{
				Name = c.Name,
				NoOfOrders = c.Orders.Count(),
				TotalAmountOfOrders = c.Orders.Sum(o => o.TotalAmount)
			}).ToList();

			return topCustomersDto;

		}


		// #################################################################################################################################

		public double CalculateTodaysSaleAmount()
		{
			return dbContext.Orders.Where(o => o.OrderDate.Date == DateTime.Today).Sum(o => o.TotalAmount);
		}

		public double CalculateTodaysCustomersTotalPaidAmount()
		{
			return dbContext.Orders.Where(o => o.OrderDate == DateTime.Today).Sum(o => o.TotalPaidAmount);
		}

		public List<int> GetDistinctYears()
		{
			return dbContext.Orders.Select(o => o.OrderDate.Year).Distinct().ToList();
		}


		// #################################################################################################################################

		public List<DailySale> GetDailyOrdersSum(int year, int monthIndex)
		{
			var dailySales = dbContext.Orders.Where(o => o.OrderDate.Year == year && o.OrderDate.Month == monthIndex)
			   .GroupBy(o => o.OrderDate.Day)
			   .Select(group => new DailySale
			   {
				   Day = group.Key,
				   Month = monthIndex,
				   Year = year,
				   SalesTotalAmount = group.Sum(o => o.TotalAmount)
			   })
			   .ToList();

			return dailySales;
		}

		[Time]
		public List<Order> GetOrdersOfDay(int year, int month, int day)
		{
			return dbContext.Orders.Include(o => o.Customer).Where(o => o.OrderDate.Date == new DateTime(year, month, day)).ToList();
		}


		// #################################################################################################################################

	}
}

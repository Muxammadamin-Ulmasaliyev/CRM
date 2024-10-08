﻿using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Net;

namespace InventoryManagementSystem.Model
{
    public class AppDbContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CarType> CarTypes { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<SetType> SetTypes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<DebtWithdrawal> DebtWithdrawals { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
			string dbPath = Path.Combine(AppContext.BaseDirectory, "baza.db");
			optionsBuilder.UseSqlite($"Data Source={dbPath}");

			// Configure your SQL Server connection string here
			// optionsBuilder.UseSqlServer("server=(localDb)\\MSSQLLocalDB;database=Baza;Integrated Security=true;");
			// optionsBuilder.UseSqlServer("server=(localDb)\\MSSQLLocalDB;AttachDbFilename=" + System.IO.Path.GetFullPath("Baza.mdf") + ";Integrated Security=true;");


		}
    }
}

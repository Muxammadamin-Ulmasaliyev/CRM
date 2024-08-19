using InventoryManagementSystem.Model;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace InventoryManagementSystem
{
    public partial class App : Application
    {
        public static AppDbContext AppDbContext { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
            {
            base.OnStartup(e);


           /* AppDbContext = new AppDbContext();
            AppDbContext.Database.EnsureCreated();*/

			SQLitePCL.Batteries.Init();


			AppDomain.CurrentDomain.UnhandledException += GlobalExceptionHandler;

			using (var context = new AppDbContext())
			{
				context.Database.Migrate();
			}
		}

        private void GlobalExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = (Exception)e.ExceptionObject;

            // Show error message in a MessageBox
            MessageBox.Show($"An unhandled exception occurred: {exception.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            // Optionally log the exception or perform other actions

        }
        protected override void OnExit(ExitEventArgs e)
        {


            base.OnExit(e);
			using (var context = new AppDbContext())
			{
				context.Database.CloseConnection();
			}
		}
    }

}

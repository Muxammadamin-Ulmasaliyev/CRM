using InventoryManagementSystem.Model;
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


            AppDbContext = new AppDbContext();
            AppDbContext.Database.EnsureCreated();


            AppDomain.CurrentDomain.UnhandledException += GlobalExceptionHandler;
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

            AppDbContext.Dispose();
        }
    }

}

using InventoryManagementSystem.Model;
using System.Windows;

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
        }


        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            AppDbContext.Dispose();
        }
    }

}

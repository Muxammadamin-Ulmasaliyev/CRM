using InventoryManagementSystem.Model;
using InventoryManagementSystem.Services;
using InventoryManagementSystem.View;
using System.Drawing;
using System.Windows.Controls;

namespace InventoryManagementSystem.Pages
{
    public partial class AdminPage : Page
    {

        private readonly ProductService _productService;

        public AdminPage()
        {
            _productService = new(new AppDbContext());
            InitializeComponent();
        }


        private void btnExportToExcel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ChequeDocumentXlsx.ExportToExcel(_productService.GetAll(), $"{DateTime.Now.ToString("dd-MM-yyyy")}producst.xlsx");
        }

        private void btnChangePassword_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var changePasswordWindow = new ChangePasswordWindow();
            changePasswordWindow.ShowDialog();
        }
    }
}

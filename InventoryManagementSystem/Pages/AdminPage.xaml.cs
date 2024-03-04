using InventoryManagementSystem.Model;
using InventoryManagementSystem.Services;
using InventoryManagementSystem.View;
using LiveCharts.Wpf;
using LiveCharts;
using System.Windows.Controls;
using System.Windows.Media;
using System.Globalization;
using Notification.Wpf;

namespace InventoryManagementSystem.Pages
{
    public partial class AdminPage : Page
    {

        private readonly ProductService _productService;
        private readonly OrderDetailService _orderDetailService;
        private readonly OrderService _orderService;
        private readonly CustomerService _customerService;
        private NotificationManager _notificationManager;

        public AdminPage()
        {
            _notificationManager = new();
            _productService = new(new AppDbContext());
            _orderService = new(new AppDbContext());
            _orderDetailService = new(new AppDbContext());
            _customerService = new(new AppDbContext());
            InitializeComponent();
            SetupUserCustomizationsSettings();
            InitializeChart();

            PopulateTopCustomersDataGrid();
            PopulateTopSoldProductsDataGrid();
            PopulateNetWorthTxt();
            PopulateIncomePercentageTxt();
            PopulateDebtAmountOfCustomersTxt();
        }

        private void PopulateNetWorthTxt()
        {
            txtNetWorth.Text = _productService.CalculateNetWorth().ToString("C0", new CultureInfo("uz-UZ"));
        }
        private void PopulateIncomePercentageTxt()
        {
            txtIncomePercentage.Text = _orderDetailService.CalculateAverageIncomePercentage().ToString("P2");
        }
        private void PopulateDebtAmountOfCustomersTxt()
        {
            txtCustomersDebtAmount.Text = _customerService.CalculateDebtAmountOfCustomers().ToString("C0", new CultureInfo("uz-UZ"));
        }
        private void PopulateTopCustomersDataGrid()
        {
            topCustomersDataGrid.ItemsSource = _orderService.GetTopCustomers(Properties.Settings.Default.NumberOfTopCustomers);

        }

        private void PopulateTopSoldProductsDataGrid()
        {
            topSoldProductsDataGrid.ItemsSource = _productService.GetTopSoldProducts(Properties.Settings.Default.NumberOfTopProducts);
        }
        private Dictionary<int, double> GetMonthlySales()
        {
            return _orderService.GetMonthlySales();
        }
        private void InitializeChart()
        {
            var sales = GetMonthlySales();
            // Create a LineSeries
            var series = new LineSeries
            {
                Title = "Жами ойлик савдо",
                Values = new ChartValues<double>(sales.Values),

            };
            var labels = new[] { "Январь ", "Февраль ", "Март ", "Апрель ", "Май", "Июнь ", "Июль ", "Август ", "Сентябрь ", "Октябрь ", "Ноябрь ", "Декабрь" };
            // Add the series to the chart

            monthlySalesChart.Series.Add(series);

            // Configure axes if needed
            monthlySalesChart.AxisX.Add(new Axis
            {
                Title = "Ой",
                Labels = labels,
                Foreground = Brushes.Black,
                FontSize = 14,
                

            });
            monthlySalesChart.AxisY.Add(new Axis
            {
                Title = "Савдо суммаси",
                LabelFormatter = value => value.ToString("C0", CultureInfo.GetCultureInfo("uz-UZ")),
                Foreground = Brushes.Black,
                FontSize = 14

            });
        }
        private void SetupUserCustomizationsSettings()
        {
            this.FontFamily = new FontFamily(Properties.Settings.Default.AppFontFamily);
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






        private void btnSaveNumberOfTopCustomers_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!int.TryParse(tbNumberOfTopCustomers.Text, out int numberOfCustomers))
            {
                _notificationManager.Show("Хатолик !", "Сон киритинг !", NotificationType.Error);
                return;
            }
            if (numberOfCustomers > 11)
            {
                _notificationManager.Show("Хатолик !", "Кичикрок сон киритинг !", NotificationType.Error);
                return;
            }
            Properties.Settings.Default.NumberOfTopCustomers = numberOfCustomers;
            Properties.Settings.Default.Save();
            PopulateTopCustomersDataGrid();
            tbNumberOfTopCustomers.Visibility = System.Windows.Visibility.Hidden;
            btnSaveNumberOfTopCustomers.Visibility = System.Windows.Visibility.Hidden;
            btnEditNumberOfTopCustomers.IsEnabled = true;
            _notificationManager.Show("Муваффакият !", "", NotificationType.Success);
            tbNumberOfTopCustomers.Text = string.Empty;

        }

        private void btnEditNumberOfTopCustomers_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            tbNumberOfTopCustomers.Visibility = System.Windows.Visibility.Visible;
            btnSaveNumberOfTopCustomers.Visibility = System.Windows.Visibility.Visible;
            btnEditNumberOfTopCustomers.IsEnabled = false;
        }



        private void btnSaveNumberOfTopProducts_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!int.TryParse(tbNumberOfTopProducts.Text, out int numberOfProducts))
            {
                _notificationManager.Show("Хатолик !", "Сон киритинг !", NotificationType.Error);
                return;
            }
            if (numberOfProducts > 31)
            {
                _notificationManager.Show("Хатолик !", "Кичикрок сон киритинг !", NotificationType.Error);
                return;
            }

            Properties.Settings.Default.NumberOfTopProducts = numberOfProducts;
            Properties.Settings.Default.Save();
            PopulateTopSoldProductsDataGrid();
            tbNumberOfTopProducts.Visibility = System.Windows.Visibility.Hidden;
            btnSaveNumberOfTopProducts.Visibility = System.Windows.Visibility.Hidden;
            btnEditNumberOfTopProducts.IsEnabled = true;
            _notificationManager.Show("Муваффакият !", "", NotificationType.Success);
            tbNumberOfTopProducts.Text = string.Empty;


        }
        private void btnEditNumberOfTopProducts_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            tbNumberOfTopProducts.Visibility = System.Windows.Visibility.Visible;
            btnSaveNumberOfTopProducts.Visibility = System.Windows.Visibility.Visible;
            btnEditNumberOfTopProducts.IsEnabled = false;

        }
    }
}

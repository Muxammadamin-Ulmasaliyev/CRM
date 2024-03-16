using InventoryManagementSystem.Model;
using InventoryManagementSystem.Services;
using InventoryManagementSystem.View;
using LiveCharts.Wpf;
using LiveCharts;
using System.Windows.Controls;
using System.Windows.Media;
using System.Globalization;
using Notification.Wpf;
using System.Windows;
using MethodTimer;
using Ookii.Dialogs.Wpf;

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

            InitializeChart(DateTime.Today.Year);

            PopulateTopCustomersDataGrid();
            PopulateTopSoldProductsDataGrid();
            PopulateLeastProductsDataGrid();
            PopulateNetWorthTxt();
            PopulateIncomePercentageTxt();
            PopulateDebtAmountOfCustomersTxt();
            PopulateTodaysSaleAmountTxt();
            PopulateYearsComboBox();

            KeyDown += btnSave_KeyDown;


            chboxIsPasswordRequired.IsChecked = Properties.Settings.Default.IsPasswordRequired;


            monthlySalesChart.AnimationsSpeed = TimeSpan.FromMicroseconds(200);


        }
        private void PopulateYearsComboBox()
        {
            cbYears.ItemsSource = _orderService.GetDistinctYears();
            cbYears.SelectedIndex = 0;
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

        private void PopulateTodaysSaleAmountTxt()
        {
            txtTodaysSaleAmount.Text = _orderService.CalculateTodaysSaleAmount().ToString("C0", new CultureInfo("uz-UZ"));
        }

        private void PopulateTopCustomersDataGrid()
        {
            topCustomersDataGrid.ItemsSource = _orderService.GetTopCustomers(Properties.Settings.Default.NumberOfTopCustomers);

        }


        private void PopulateTopSoldProductsDataGrid()
        {
            topSoldProductsDataGrid.ItemsSource = _productService.GetTopSoldProducts(Properties.Settings.Default.NumberOfTopProducts);
        }

        private void PopulateLeastProductsDataGrid()
        {
            leastAmountProductsDataGrid.ItemsSource = _productService.GetLeastProducts(Properties.Settings.Default.NumberOfLeastProducts);
        }

        private void cbYears_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedYear = (int)cbYears.SelectedItem;
            RefreshChart(selectedYear);
        }

        private void RefreshChart(int selectedYear)
        {
            // Clear existing series before adding a new one
            monthlySalesChart.Series.Clear();
            monthlySalesChart.AxisX.Clear();
            monthlySalesChart.AxisY.Clear();

            // Initialize chart with the selected year
            InitializeChart(selectedYear);
        }
        private Dictionary<int, double> GetMonthlySalesOfYear(int year)
        {
            return _orderService.GetMonthlySalesOfYear(year);
        }
        private void InitializeChart(int selectedYear)
        {
            var sales = GetMonthlySalesOfYear(selectedYear);
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


        private void StartLoading()
        {
            loadingPanel.Visibility = System.Windows.Visibility.Visible;
        }

        private void StopLoading()
        {
            loadingPanel.Visibility = System.Windows.Visibility.Collapsed;
        }


        private async void btnExportToExcel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var folderBrowserDialog = new VistaFolderBrowserDialog();
            bool? result = folderBrowserDialog.ShowDialog();
            if (result == false)
            {
                return;
            }

            string selectedDirectory = folderBrowserDialog.SelectedPath;


            StartLoading();

            await Task.Run(() =>
            {
                ChequeDocumentXlsx.ExportToExcel(_productService.GetAll(), $"{selectedDirectory}/{DateTime.Now.ToString("dd-MM-yyyy")}producst.xlsx");
            });

            StopLoading();
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

            Properties.Settings.Default.NumberOfTopCustomers = numberOfCustomers;
            Properties.Settings.Default.Save();
            PopulateTopCustomersDataGrid();
            tbNumberOfTopCustomers.Visibility = System.Windows.Visibility.Hidden;
            btnSaveNumberOfTopCustomers.Visibility = System.Windows.Visibility.Hidden;
            btnEditNumberOfTopCustomers.IsEnabled = true;
            //   _notificationManager.Show("Муваффакият !", "", NotificationType.Success);
            tbNumberOfTopCustomers.Text = string.Empty;

        }

        private void btnEditNumberOfTopCustomers_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            tbNumberOfTopCustomers.Visibility = System.Windows.Visibility.Visible;
            tbNumberOfTopCustomers.Focus();

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


            Properties.Settings.Default.NumberOfTopProducts = numberOfProducts;
            Properties.Settings.Default.Save();
            PopulateTopSoldProductsDataGrid();
            tbNumberOfTopProducts.Visibility = System.Windows.Visibility.Hidden;
            btnSaveNumberOfTopProducts.Visibility = System.Windows.Visibility.Hidden;
            btnEditNumberOfTopProducts.IsEnabled = true;
            // _notificationManager.Show("Муваффакият !", "", NotificationType.Success);
            tbNumberOfTopProducts.Text = string.Empty;


        }
        private void btnEditNumberOfTopProducts_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            tbNumberOfTopProducts.Visibility = System.Windows.Visibility.Visible;
            tbNumberOfTopProducts.Focus();
            btnSaveNumberOfTopProducts.Visibility = System.Windows.Visibility.Visible;
            btnEditNumberOfTopProducts.IsEnabled = false;

        }

        private void btnNetWorthInfo_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _notificationManager.Show("", "Бу сумма - базадаги барча товарларни таннархларини йигиндиси", NotificationType.Information);

        }

        private void btnCustomersDebtAmountInfo_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _notificationManager.Show("", "Биздан карзи бор мижозларни жами карзлари йигиндиси", NotificationType.Information);

        }

        private void btnAverageIncomePercentageInfo_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _notificationManager.Show("", "Хар бир сотилган товарни устига куйилган фойдани уртача киймати", NotificationType.Information);

        }

        private void btnTodaysSaleInfo_Click(object sender, RoutedEventArgs e)
        {
            _notificationManager.Show("", "Бугунги жами савдо суммаси", NotificationType.Information);

        }

        private void btnSaveNumberOfLeastProducts_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(tbNumberOfLeastProducts.Text, out int numberOfLeastProducts))
            {
                _notificationManager.Show("Хатолик !", "Сон киритинг !", NotificationType.Error);
                return;
            }


            Properties.Settings.Default.NumberOfLeastProducts = numberOfLeastProducts;
            Properties.Settings.Default.Save();
            PopulateLeastProductsDataGrid();
            tbNumberOfLeastProducts.Visibility = System.Windows.Visibility.Hidden;
            btnSaveNumberOfLeastProducts.Visibility = System.Windows.Visibility.Hidden;
            btnEditNumberOfLeastProducts.IsEnabled = true;
            //   _notificationManager.Show("Муваффакият !", "", NotificationType.Success);
            tbNumberOfLeastProducts.Text = string.Empty;
        }

        private void btnEditNumberOfLeastProducts_Click(object sender, RoutedEventArgs e)
        {
            tbNumberOfLeastProducts.Visibility = System.Windows.Visibility.Visible;
            tbNumberOfLeastProducts.Focus();

            btnSaveNumberOfLeastProducts.Visibility = System.Windows.Visibility.Visible;
            btnEditNumberOfLeastProducts.IsEnabled = false;
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
            e.Handled = true;
        }


        private void btnSave_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {

                if (tbNumberOfTopProducts.IsFocused)
                {
                    btnSaveNumberOfTopProducts_Click(sender, e);
                    return;
                }
                if (tbNumberOfTopCustomers.IsFocused)
                {
                    btnSaveNumberOfTopCustomers_Click(sender, e);

                    return;
                }
                if (tbNumberOfLeastProducts.IsFocused)
                {
                    btnSaveNumberOfLeastProducts_Click(sender, e);

                    return;
                }
            }

        }

        private void monthlySalesChart_DataClick(object sender, ChartPoint chartPoint)
        {
            var month = chartPoint.Key + 1;
            var year = (int)cbYears.SelectedItem;


            var daysCount = DateTime.DaysInMonth(year, month);

            var dailySales = _orderService.GetDailyOrdersSum(year, month);

            for (int i = 1; i <= daysCount; i++)
            {
                if (!dailySales.Any(ds => ds.Day == i))
                {
                    dailySales.Add(new DailySale() { Day = i, Month = month, Year = year, SalesTotalAmount = 0 });
                }
            }

            dailySales = dailySales.OrderBy(ds => ds.Day).ToList();

            DailySalesOverviewWindow dailySalesOverviewWindow = new DailySalesOverviewWindow(dailySales, month, year);
            this.Opacity = 0.4;
            dailySalesOverviewWindow.ShowDialog();
            this.Opacity = 1;


        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.IsPasswordRequired = (bool)chboxIsPasswordRequired.IsChecked;
            Properties.Settings.Default.Save();
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.IsPasswordRequired = (bool)chboxIsPasswordRequired.IsChecked;
            Properties.Settings.Default.Save();
        }
    }
}

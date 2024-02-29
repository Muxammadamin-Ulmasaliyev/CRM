using Notification.Wpf;
using Ookii.Dialogs.Wpf;
using System.Windows.Controls;

namespace InventoryManagementSystem.Pages
{
    public partial class SettingsPage : Page
    {
        private NotificationManager _notificationManager;
        public SettingsPage()
        {
            _notificationManager = new();
            InitializeComponent();
            PopulateTextBoxes();
        }

        private void SaveChanges()
        {
            Properties.Settings.Default.Save();
        }
        private void PopulateTextBoxes()
        {
            tbCurrencyRate.Text = Properties.Settings.Default.CurrencyRate.ToString();
            tbProductsPerPage.Text = Properties.Settings.Default.ProductsPerPage.ToString();
            tbCustomersPerPage.Text = Properties.Settings.Default.CustomersPerPage.ToString();
            tbOrdersPerPage.Text = Properties.Settings.Default.OrdersPerPage.ToString();
            tbOrderDetailsPerPage.Text = Properties.Settings.Default.OrderDetailsPerPage.ToString();
            tbProductDataGridFontSize.Text = Properties.Settings.Default.ProductsDataGridFontSize.ToString();
            tbOrderDetailsDataGridFontSize.Text = Properties.Settings.Default.OrderHistoryDetailsDataGridFontSize.ToString();
            tbOrderDataGridFontSize.Text = Properties.Settings.Default.OrderHistoryDataGridFontSize.ToString();
            tbCustomerDataGridFontSize.Text = Properties.Settings.Default.CustomersDataGridFontSize.ToString();
            tbCartDataGridFontSize.Text = Properties.Settings.Default.CartDataGridFontSize.ToString();
            tbChequesDirectory.Text = Properties.Settings.Default.OrderChequesDirectoryPath;
        }

       
        private void btnsaveCurrencyRate_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (double.TryParse(tbCurrencyRate.Text, out var currencyRate))
            {
                Properties.Settings.Default.CurrencyRate = currencyRate;
                _notificationManager.Show("Success", "Muvaffaqiyat", NotificationType.Success);
                tbCurrencyRate.IsEnabled = false;
                btnEditCurrencyRate.IsEnabled = true;
                btnsaveCurrencyRate.IsEnabled = false;
                SaveChanges();
            }
            else
            {
                tbCurrencyRate.Text = Properties.Settings.Default.CurrencyRate.ToString();
                tbCurrencyRate.Focus();
                _notificationManager.Show("Error", "Son kiriting", NotificationType.Error);
            }
        }

        private void btnEditCurrencyRate_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            tbCurrencyRate.IsEnabled = true;
            btnsaveCurrencyRate.IsEnabled = true;
            btnEditCurrencyRate.IsEnabled = false;
        }

        private void btnEditChequesDirectory_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var folderBrowserDialog = new VistaFolderBrowserDialog();
            bool? result = folderBrowserDialog.ShowDialog();
            if (result == false)
            {
                return;
            }

            string selectedDirectory = folderBrowserDialog.SelectedPath;
            tbChequesDirectory.Text = selectedDirectory;
            Properties.Settings.Default.OrderChequesDirectoryPath = selectedDirectory;

            SaveChanges();

        }

        private void btnEditProductsPerPage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            tbProductsPerPage.IsEnabled = true;
            btnSaveProductsPerPage.IsEnabled = true;
            btnEditProductsPerPage.IsEnabled = false;
        }

        private void btnSaveProductsPerPage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (int.TryParse(tbProductsPerPage.Text, out var productsPerPage))
            {

                Properties.Settings.Default.ProductsPerPage = productsPerPage;

                _notificationManager.Show("Success", "Muvaffaqiyat", NotificationType.Success);
                tbProductsPerPage.IsEnabled = false;
                btnEditProductsPerPage.IsEnabled = true;
                btnSaveProductsPerPage.IsEnabled = false;

                SaveChanges();

            }
            else
            {
                tbProductsPerPage.Text = Properties.Settings.Default.ProductsPerPage.ToString();
                tbProductsPerPage.Focus();
                _notificationManager.Show("Error", "Son kiriting", NotificationType.Error);
            }
        }

        private void btnEditCustomersPerPage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            tbCustomersPerPage.IsEnabled = true;
            btnSaveCustomersPerPage.IsEnabled = true;
            btnEditCustomersPerPage.IsEnabled = false;
        }

        private void btnSaveCustomersPerPage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (int.TryParse(tbCustomersPerPage.Text, out var customersPerPage))
            {

                Properties.Settings.Default.CustomersPerPage = customersPerPage;

                _notificationManager.Show("Success", "Muvaffaqiyat", NotificationType.Success);
                tbCustomersPerPage.IsEnabled = false;
                btnEditCustomersPerPage.IsEnabled = true;
                btnSaveCustomersPerPage.IsEnabled = false;

                SaveChanges();

            }
            else
            {
                tbCustomersPerPage.Text = Properties.Settings.Default.CustomersPerPage.ToString();
                tbCustomersPerPage.Focus();
                _notificationManager.Show("Error", "Son kiriting", NotificationType.Error);
            }
        }

        private void btnEditOrdersPerPage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            tbOrdersPerPage.IsEnabled = true;
            btnSaveOrdersPerPage.IsEnabled = true;
            btnEditOrdersPerPage.IsEnabled = false;
        }

        private void btnSaveOrdersPerPage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (int.TryParse(tbOrdersPerPage.Text, out var ordersPerPage))
            {

                Properties.Settings.Default.OrdersPerPage = ordersPerPage;

                _notificationManager.Show("Success", "Muvaffaqiyat", NotificationType.Success);
                tbOrdersPerPage.IsEnabled = false;
                btnEditOrdersPerPage.IsEnabled = true;
                btnSaveOrdersPerPage.IsEnabled = false;

                SaveChanges();

            }
            else
            {
                tbOrdersPerPage.Text = Properties.Settings.Default.OrdersPerPage.ToString();
                tbOrdersPerPage.Focus();
                _notificationManager.Show("Error", "Son kiriting", NotificationType.Error);
            }
        }

       

        private void btnSaveOrderDetailsPerPage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (int.TryParse(tbOrderDetailsPerPage.Text, out var orderDetailsPerPage))
            {

                Properties.Settings.Default.OrderDetailsPerPage = orderDetailsPerPage;

                _notificationManager.Show("Success", "Muvaffaqiyat", NotificationType.Success);
                tbOrderDetailsPerPage.IsEnabled = false;
                btnEditOrderDetailsPerPage.IsEnabled = true;
                btnSaveOrderDetailsPerPage.IsEnabled = false;

                SaveChanges();

            }
            else
            {
                tbOrderDetailsPerPage.Text = Properties.Settings.Default.OrderDetailsPerPage.ToString();
                tbOrderDetailsPerPage.Focus();
                _notificationManager.Show("Error", "Son kiriting", NotificationType.Error);
            }
        }

        private void btnEditOrderDetailsPerPage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            tbOrderDetailsPerPage.IsEnabled = true;
            btnSaveOrderDetailsPerPage.IsEnabled = true;
            btnEditOrderDetailsPerPage.IsEnabled = false;
        }

        private void btnEditProductDataGridFontSize_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            tbProductDataGridFontSize.IsEnabled = true;
            btnSaveProductDataGridFontSize.IsEnabled = true;
            btnEditProductDataGridFontSize.IsEnabled = false;
        }

        private void btnSaveProductDataGridFontSize_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (double.TryParse(tbProductDataGridFontSize.Text, out var productsDataGridFontSize))
            {

                Properties.Settings.Default.ProductsDataGridFontSize = productsDataGridFontSize;

                _notificationManager.Show("Success", "Muvaffaqiyat", NotificationType.Success);
                tbProductDataGridFontSize.IsEnabled = false;
                btnEditProductDataGridFontSize.IsEnabled = true;
                btnSaveProductDataGridFontSize.IsEnabled = false;

                SaveChanges();

            }
            else
            {
                tbProductDataGridFontSize.Text = Properties.Settings.Default.ProductsDataGridFontSize.ToString();
                tbProductDataGridFontSize.Focus();
                _notificationManager.Show("Error", "Son kiriting", NotificationType.Error);
            }
        }

        private void btnSaveCustomerDataGridFontSize_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (double.TryParse(tbCustomerDataGridFontSize.Text, out var customersDataGridFontSize))
            {

                Properties.Settings.Default.CustomersDataGridFontSize = customersDataGridFontSize;

                _notificationManager.Show("Success", "Muvaffaqiyat", NotificationType.Success);
                tbCustomerDataGridFontSize.IsEnabled = false;
                btnEditCustomerDataGridFontSize.IsEnabled = true;
                btnSaveCustomerDataGridFontSize.IsEnabled = false;

                SaveChanges();

            }
            else
            {
                tbCustomerDataGridFontSize.Text = Properties.Settings.Default.CustomersDataGridFontSize.ToString();
                tbCustomerDataGridFontSize.Focus();
                _notificationManager.Show("Error", "Son kiriting", NotificationType.Error);
            }
        }

        private void btnEditCustomerDataGridFontSize_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            tbCustomerDataGridFontSize.IsEnabled = true;
            btnSaveCustomerDataGridFontSize.IsEnabled = true;
            btnEditCustomerDataGridFontSize.IsEnabled = false;
        }

        private void btnEditOrderDataGridFontSize_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            tbOrderDataGridFontSize.IsEnabled = true;
            btnSaveOrderDataGridFontSize.IsEnabled = true;
            btnEditOrderDataGridFontSize.IsEnabled = false;
        }

        private void btnSaveOrderDataGridFontSize_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (double.TryParse(tbOrderDataGridFontSize.Text, out var ordersDataGridFontSize))
            {

                Properties.Settings.Default.OrderHistoryDataGridFontSize = ordersDataGridFontSize;

                _notificationManager.Show("Success", "Muvaffaqiyat", NotificationType.Success);
                tbOrderDataGridFontSize.IsEnabled = false;
                btnEditOrderDataGridFontSize.IsEnabled = true;
                btnSaveOrderDataGridFontSize.IsEnabled = false;

                SaveChanges();

            }
            else
            {
                tbOrderDataGridFontSize.Text = Properties.Settings.Default.OrderHistoryDataGridFontSize.ToString();
                tbOrderDataGridFontSize.Focus();
                _notificationManager.Show("Error", "Son kiriting", NotificationType.Error);
            }
        }

        private void btnEditOrderDetailsDataGridFontSize_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            tbOrderDetailsDataGridFontSize.IsEnabled = true;
            btnSaveOrderDetailsDataGridFontSize.IsEnabled = true;
            btnEditOrderDetailsDataGridFontSize.IsEnabled = false;
        }

        private void btnSaveOrderDetailsDataGridFontSize_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (double.TryParse(tbOrderDetailsDataGridFontSize.Text, out var orderDetailsDataGridFontSize))
            {

                Properties.Settings.Default.OrderHistoryDetailsDataGridFontSize = orderDetailsDataGridFontSize;

                _notificationManager.Show("Success", "Muvaffaqiyat", NotificationType.Success);
                tbOrderDetailsDataGridFontSize.IsEnabled = false;
                btnEditOrderDetailsDataGridFontSize.IsEnabled = true;
                btnSaveOrderDetailsDataGridFontSize.IsEnabled = false;

                SaveChanges();

            }
            else
            {
                tbOrderDetailsDataGridFontSize.Text = Properties.Settings.Default.OrderHistoryDetailsDataGridFontSize.ToString();
                tbOrderDetailsDataGridFontSize.Focus();
                _notificationManager.Show("Error", "Son kiriting", NotificationType.Error);
            }
        }

        private void btnEditCartDataGridFontSize_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            tbCartDataGridFontSize.IsEnabled = true;
            btnSaveCartDataGridFontSize.IsEnabled = true;
            btnEditCartDataGridFontSize.IsEnabled = false;
        }

        private void btnSaveCartDataGridFontSize_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (double.TryParse(tbCartDataGridFontSize.Text, out var cartDataGridFontSize))
            {

                Properties.Settings.Default.CartDataGridFontSize = cartDataGridFontSize;

                _notificationManager.Show("Success", "Muvaffaqiyat", NotificationType.Success);
                tbCartDataGridFontSize.IsEnabled = false;
                btnEditCartDataGridFontSize.IsEnabled = true;
                btnSaveCartDataGridFontSize.IsEnabled = false;

                SaveChanges();

            }
            else
            {
                tbCartDataGridFontSize.Text = Properties.Settings.Default.CartDataGridFontSize.ToString();
                tbCartDataGridFontSize.Focus();
                _notificationManager.Show("Error", "Son kiriting", NotificationType.Error);
            }
        }

    }
}

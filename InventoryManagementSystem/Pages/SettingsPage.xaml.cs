using MahApps.Metro.Controls;
using Notification.Wpf;
using Ookii.Dialogs.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InventoryManagementSystem.Pages
{
    public partial class SettingsPage : Page
    {
        private NotificationManager _notificationManager;
        private FontFamily _fontFamily;
        public SettingsPage()
        {
            _fontFamily = new FontFamily(Properties.Settings.Default.AppFontFamily);
            _notificationManager = new();
            InitializeComponent();
            SetupUserCustomizationsSettings();
            PopulateTextBoxes();

            PopulateFontFamilyComboBoxes();

            KeyDown += btnSave_KeyDown;
        }

        private void SetupUserCustomizationsSettings()
        {
            this.FontFamily = new FontFamily(Properties.Settings.Default.AppFontFamily);
        }
        private void PopulateFontFamilyComboBoxes()
        {

            cbFontFamilies.ItemsSource = Fonts.SystemFontFamilies;
            cbFontFamilies.SelectedItem = _fontFamily;

        }
        private void cbFontFamilies_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedFont = cbFontFamilies.SelectedItem as FontFamily;
            if (selectedFont != null)
            {
                Properties.Settings.Default.AppFontFamily = selectedFont.Source;
                Properties.Settings.Default.Save();
            }
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
            tbCategoriesFontSize.Text = Properties.Settings.Default.CategoriesDataGridFontSize.ToString();
            tbChequesDirectory.Text = Properties.Settings.Default.OrderChequesDirectoryPath;
        }


        private void btnsaveCurrencyRate_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (double.TryParse(tbCurrencyRate.Text, out var currencyRate))
            {
                Properties.Settings.Default.CurrencyRate = currencyRate;
                _notificationManager.Show("Муваффакият", "", NotificationType.Success);
                tbCurrencyRate.IsEnabled = false;
                btnEditCurrencyRate.IsEnabled = true;

                btnsaveCurrencyRate.Visibility = Visibility.Hidden;
                btnsaveCurrencyRate.IsEnabled = false;
                SaveChanges();
            }
            else
            {
                tbCurrencyRate.Text = Properties.Settings.Default.CurrencyRate.ToString();
                tbCurrencyRate.Focus();
                _notificationManager.Show("Хатолик", "Сон киритинг", NotificationType.Error);
            }
        }

        private void btnEditCurrencyRate_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            tbCurrencyRate.IsEnabled = true;
            tbCurrencyRate.Focus();

            btnsaveCurrencyRate.IsEnabled = true;
            btnsaveCurrencyRate.Visibility = Visibility.Visible;
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
            tbProductsPerPage.Focus();

            btnSaveProductsPerPage.IsEnabled = true;
            btnSaveProductsPerPage.Visibility = Visibility.Visible;

            btnEditProductsPerPage.IsEnabled = false;
        }

        private void btnSaveProductsPerPage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (int.TryParse(tbProductsPerPage.Text, out var productsPerPage))
            {

                Properties.Settings.Default.ProductsPerPage = productsPerPage;

                _notificationManager.Show("Муваффакият", "", NotificationType.Success);
                tbProductsPerPage.IsEnabled = false;
                btnEditProductsPerPage.IsEnabled = true;

                btnSaveProductsPerPage.Visibility = Visibility.Hidden;
                btnSaveProductsPerPage.IsEnabled = false;

                SaveChanges();

            }
            else
            {
                tbProductsPerPage.Text = Properties.Settings.Default.ProductsPerPage.ToString();
                tbProductsPerPage.Focus();
                _notificationManager.Show("Хатолик", "Сон киритинг", NotificationType.Error);
            }
        }

        private void btnEditCustomersPerPage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            tbCustomersPerPage.IsEnabled = true;
            tbCustomersPerPage.Focus();
            btnSaveCustomersPerPage.IsEnabled = true;
            btnSaveCustomersPerPage.Visibility = Visibility.Visible;

            btnEditCustomersPerPage.IsEnabled = false;
        }

        private void btnSaveCustomersPerPage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (int.TryParse(tbCustomersPerPage.Text, out var customersPerPage))
            {

                Properties.Settings.Default.CustomersPerPage = customersPerPage;

                _notificationManager.Show("Муваффакият", "", NotificationType.Success);
                tbCustomersPerPage.IsEnabled = false;
                btnEditCustomersPerPage.IsEnabled = true;
                btnSaveCustomersPerPage.Visibility = Visibility.Hidden;

                btnSaveCustomersPerPage.IsEnabled = false;

                SaveChanges();

            }
            else
            {
                tbCustomersPerPage.Text = Properties.Settings.Default.CustomersPerPage.ToString();
                tbCustomersPerPage.Focus();
                _notificationManager.Show("Хатолик", "Сон киритинг", NotificationType.Error);
            }
        }

        private void btnEditOrdersPerPage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            tbOrdersPerPage.IsEnabled = true;
            tbOrdersPerPage.Focus();
            btnSaveOrdersPerPage.IsEnabled = true;
            btnSaveOrdersPerPage.Visibility = Visibility.Visible;

            btnEditOrdersPerPage.IsEnabled = false;
        }

        private void btnSaveOrdersPerPage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (int.TryParse(tbOrdersPerPage.Text, out var ordersPerPage))
            {

                Properties.Settings.Default.OrdersPerPage = ordersPerPage;

                _notificationManager.Show("Муваффакият", "", NotificationType.Success);
                tbOrdersPerPage.IsEnabled = false;
                btnEditOrdersPerPage.IsEnabled = true;
                btnSaveOrdersPerPage.IsEnabled = false;
                btnSaveOrdersPerPage.Visibility = Visibility.Hidden;


                SaveChanges();

            }
            else
            {
                tbOrdersPerPage.Text = Properties.Settings.Default.OrdersPerPage.ToString();
                tbOrdersPerPage.Focus();
                _notificationManager.Show("Хатолик", "Сон киритинг", NotificationType.Error);
            }
        }



        private void btnSaveOrderDetailsPerPage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (int.TryParse(tbOrderDetailsPerPage.Text, out var orderDetailsPerPage))
            {

                Properties.Settings.Default.OrderDetailsPerPage = orderDetailsPerPage;

                _notificationManager.Show("Муваффакият", "", NotificationType.Success);
                tbOrderDetailsPerPage.IsEnabled = false;
                btnEditOrderDetailsPerPage.IsEnabled = true;
                btnSaveOrderDetailsPerPage.IsEnabled = false;
                btnSaveOrderDetailsPerPage.Visibility = Visibility.Hidden;


                SaveChanges();

            }
            else
            {
                tbOrderDetailsPerPage.Text = Properties.Settings.Default.OrderDetailsPerPage.ToString();
                tbOrderDetailsPerPage.Focus();
                _notificationManager.Show("Хатолик", "Сон киритинг", NotificationType.Error);
            }
        }

        private void btnEditOrderDetailsPerPage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            tbOrderDetailsPerPage.IsEnabled = true;
            tbOrderDetailsPerPage.Focus();
            btnSaveOrderDetailsPerPage.IsEnabled = true;
            btnSaveOrderDetailsPerPage.Visibility = Visibility.Visible;

            btnEditOrderDetailsPerPage.IsEnabled = false;
        }

        private void btnEditProductDataGridFontSize_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            tbProductDataGridFontSize.IsEnabled = true;
            tbProductDataGridFontSize.Focus();
            btnSaveProductDataGridFontSize.IsEnabled = true;
            btnSaveProductDataGridFontSize.Visibility = Visibility.Visible;

            btnEditProductDataGridFontSize.IsEnabled = false;
        }

        private void btnSaveProductDataGridFontSize_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (double.TryParse(tbProductDataGridFontSize.Text, out var productsDataGridFontSize))
            {

                Properties.Settings.Default.ProductsDataGridFontSize = productsDataGridFontSize;

                _notificationManager.Show("Муваффакият", "", NotificationType.Success);
                tbProductDataGridFontSize.IsEnabled = false;
                btnEditProductDataGridFontSize.IsEnabled = true;
                btnSaveProductDataGridFontSize.IsEnabled = false;
                btnSaveProductDataGridFontSize.Visibility = Visibility.Hidden;

                SaveChanges();

            }
            else
            {
                tbProductDataGridFontSize.Text = Properties.Settings.Default.ProductsDataGridFontSize.ToString();
                tbProductDataGridFontSize.Focus();
                _notificationManager.Show("Хатолик", "Сон киритинг", NotificationType.Error);
            }
        }

        private void btnSaveCustomerDataGridFontSize_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (double.TryParse(tbCustomerDataGridFontSize.Text, out var customersDataGridFontSize))
            {

                Properties.Settings.Default.CustomersDataGridFontSize = customersDataGridFontSize;

                _notificationManager.Show("Муваффакият", "", NotificationType.Success);
                tbCustomerDataGridFontSize.IsEnabled = false;
                btnEditCustomerDataGridFontSize.IsEnabled = true;
                btnSaveCustomerDataGridFontSize.IsEnabled = false;
                btnSaveCustomerDataGridFontSize.Visibility = Visibility.Hidden;

                SaveChanges();

            }
            else
            {
                tbCustomerDataGridFontSize.Text = Properties.Settings.Default.CustomersDataGridFontSize.ToString();
                tbCustomerDataGridFontSize.Focus();
                _notificationManager.Show("Error", "Сон киритинг", NotificationType.Error);
            }
        }

        private void btnEditCustomerDataGridFontSize_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            tbCustomerDataGridFontSize.IsEnabled = true;
            tbCustomerDataGridFontSize.Focus();
            btnSaveCustomerDataGridFontSize.IsEnabled = true;
            btnSaveCustomerDataGridFontSize.Visibility = Visibility.Visible;

            btnEditCustomerDataGridFontSize.IsEnabled = false;
        }

        private void btnEditOrderDataGridFontSize_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            tbOrderDataGridFontSize.IsEnabled = true;
            tbOrderDataGridFontSize.Focus();
            btnSaveOrderDataGridFontSize.IsEnabled = true;
            btnSaveOrderDataGridFontSize.Visibility = Visibility.Visible;

            btnEditOrderDataGridFontSize.IsEnabled = false;
        }

        private void btnSaveOrderDataGridFontSize_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (double.TryParse(tbOrderDataGridFontSize.Text, out var ordersDataGridFontSize))
            {

                Properties.Settings.Default.OrderHistoryDataGridFontSize = ordersDataGridFontSize;

                _notificationManager.Show("Муваффакият", "", NotificationType.Success);
                tbOrderDataGridFontSize.IsEnabled = false;
                btnEditOrderDataGridFontSize.IsEnabled = true;
                btnSaveOrderDataGridFontSize.IsEnabled = false;
                btnSaveOrderDataGridFontSize.Visibility = Visibility.Hidden;

                SaveChanges();

            }
            else
            {
                tbOrderDataGridFontSize.Text = Properties.Settings.Default.OrderHistoryDataGridFontSize.ToString();
                tbOrderDataGridFontSize.Focus();
                _notificationManager.Show("Хатолик", "Сон киритинг", NotificationType.Error);
            }
        }

        private void btnEditOrderDetailsDataGridFontSize_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            tbOrderDetailsDataGridFontSize.IsEnabled = true;
            btnEditOrderDetailsDataGridFontSize.Focus();
            btnSaveOrderDetailsDataGridFontSize.IsEnabled = true;
            btnSaveOrderDetailsDataGridFontSize.Visibility = Visibility.Visible;

            btnEditOrderDetailsDataGridFontSize.IsEnabled = false;
        }

        private void btnSaveOrderDetailsDataGridFontSize_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (double.TryParse(tbOrderDetailsDataGridFontSize.Text, out var orderDetailsDataGridFontSize))
            {

                Properties.Settings.Default.OrderHistoryDetailsDataGridFontSize = orderDetailsDataGridFontSize;

                _notificationManager.Show("Муваффакият", "", NotificationType.Success);
                tbOrderDetailsDataGridFontSize.IsEnabled = false;
                btnEditOrderDetailsDataGridFontSize.IsEnabled = true;

                btnSaveOrderDetailsDataGridFontSize.IsEnabled = false;
                btnSaveOrderDetailsDataGridFontSize.Visibility = Visibility.Hidden;

                SaveChanges();

            }
            else
            {
                tbOrderDetailsDataGridFontSize.Text = Properties.Settings.Default.OrderHistoryDetailsDataGridFontSize.ToString();
                tbOrderDetailsDataGridFontSize.Focus();
                _notificationManager.Show("Хатолик", "Сон киритинг", NotificationType.Error);
            }
        }

        private void btnEditCartDataGridFontSize_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            tbCartDataGridFontSize.IsEnabled = true;
            tbCartDataGridFontSize.Focus();
            btnSaveCartDataGridFontSize.IsEnabled = true;
            btnSaveCartDataGridFontSize.Visibility = Visibility.Visible;

            btnEditCartDataGridFontSize.IsEnabled = false;
        }

        private void btnSaveCartDataGridFontSize_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (double.TryParse(tbCartDataGridFontSize.Text, out var cartDataGridFontSize))
            {

                Properties.Settings.Default.CartDataGridFontSize = cartDataGridFontSize;

                _notificationManager.Show("Муваффакият", "", NotificationType.Success);
                tbCartDataGridFontSize.IsEnabled = false;
                btnEditCartDataGridFontSize.IsEnabled = true;
                btnSaveCartDataGridFontSize.IsEnabled = false;
                btnSaveCartDataGridFontSize.Visibility = Visibility.Hidden;


                SaveChanges();

            }
            else
            {
                tbCartDataGridFontSize.Text = Properties.Settings.Default.CartDataGridFontSize.ToString();
                tbCartDataGridFontSize.Focus();
                _notificationManager.Show("Хатолик", "Сон киритинг", NotificationType.Error);
            }
        }

        private void btnEditCategoriesFontSize_Click(object sender, RoutedEventArgs e)
        {
            tbCategoriesFontSize.IsEnabled = true;
            tbCategoriesFontSize.Focus();
            btnSaveCategoriesFontSize.IsEnabled = true;
            btnSaveCategoriesFontSize.Visibility = Visibility.Visible;

            btnEditCategoriesFontSize.IsEnabled = false;
        }

        private void btnSaveCategoriesFontSize_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(tbCategoriesFontSize.Text, out var categoriesFontSize))
            {

                Properties.Settings.Default.CategoriesDataGridFontSize = categoriesFontSize;

                _notificationManager.Show("Муваффакият", "", NotificationType.Success);
                tbCategoriesFontSize.IsEnabled = false;
                btnEditCategoriesFontSize.IsEnabled = true;
                btnSaveCategoriesFontSize.IsEnabled = false;
                btnSaveCategoriesFontSize.Visibility = Visibility.Hidden;

                SaveChanges();

            }
            else
            {
                tbCategoriesFontSize.Text = Properties.Settings.Default.CategoriesDataGridFontSize.ToString();
                tbCategoriesFontSize.Focus();
                _notificationManager.Show("Хатолик", "Сон киритинг", NotificationType.Error);
            }
        }


        private void btnSave_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (tbCurrencyRate.IsFocused)
                {
                    btnsaveCurrencyRate_Click(sender, e);
                    return;
                }


                if (tbProductsPerPage.IsFocused)
                {
                    btnSaveProductsPerPage_Click(sender, e);
                    return;
                }
                if (tbCustomersPerPage.IsFocused)
                {
                    btnSaveCustomersPerPage_Click(sender, e);

                    return;
                }
                if (tbOrdersPerPage.IsFocused)
                {
                    btnSaveOrdersPerPage_Click(sender, e);

                    return;
                }
                if (tbOrderDetailsPerPage.IsFocused)
                {
                    btnSaveOrderDetailsPerPage_Click(sender, e);

                    return;
                }


                if (tbProductDataGridFontSize.IsFocused)
                {
                    btnSaveProductDataGridFontSize_Click(sender, e);

                    return;
                }
                if (tbCustomerDataGridFontSize.IsFocused)
                {
                    btnSaveCustomerDataGridFontSize_Click(sender, e);

                    return;
                }
                if (tbOrderDataGridFontSize.IsFocused)
                {
                    btnSaveOrderDataGridFontSize_Click(sender, e);

                    return;
                }
                if (tbOrderDetailsDataGridFontSize.IsFocused)
                {
                    btnSaveOrderDetailsDataGridFontSize_Click(sender, e);

                    return;
                }
                if (tbCartDataGridFontSize.IsFocused)
                {
                    btnSaveCartDataGridFontSize_Click(sender, e);

                    return;
                }
                if (tbCategoriesFontSize.IsFocused)
                {
                    btnSaveCategoriesFontSize_Click(sender, e);

                    return;
                }
            }
        }
    }
}

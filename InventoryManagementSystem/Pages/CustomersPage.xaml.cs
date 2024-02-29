using InventoryManagementSystem.Model;
using InventoryManagementSystem.Services;
using InventoryManagementSystem.View;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Notification.Wpf;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace InventoryManagementSystem.Pages
{
    public partial class CustomersPage : Page
    {
        private ObservableCollection<Customer> customers;
        private ObservableCollection<Customer> filteredCustomers;
        private NotificationManager notificationManager;
        private CustomerService _customerService;
        private int _pageSize = Properties.Settings.Default.CustomersPerPage;
        private int _currentPage = 1;
        private System.Timers.Timer debounceTimer;

        public CustomersPage()
        {
            SetupTimerSettings();
            _customerService = new(new AppDbContext());
            InitializeComponent();
            SetupUserCustomizationsSettings();
            PopulateDataGrid();
            LoadPage();
            PopulateNumberOfCustomersTxt();
            notificationManager = new();
            txtNumberOfCustomersInDb.Text = $"Bazada mavjud jami mijozlar soni : {customers.Count}";

        }
        private void SetupUserCustomizationsSettings()
        {
            customerDataGrid.FontSize = Properties.Settings.Default.CustomersDataGridFontSize;
        }
        private void SetupTimerSettings()
        {
            debounceTimer = new();
            debounceTimer.Interval = 1000;
            debounceTimer.AutoReset = false;
            debounceTimer.Elapsed += DebounceTimer_Elapsed;
        }
        private void PopulateNumberOfCustomersTxt()
        {
            txtNumberOfCustomers.Text = $"Жадвалдаги Mijozlar сони : {customerDataGrid.Items.Count}";
        }
        private void DebounceTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    string searchText = searchBar.Text;
                    Search();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void PopulateDataGrid()
        {
            customers = new ObservableCollection<Customer>(_customerService.GetAll());
            PopulateNumberOfCustomersTxt();
        }
        private bool IsFilteringDisabled()
        {
            return string.IsNullOrWhiteSpace(searchBar.Text);
        }
        private void CheckPaginationButtonStates()
        {
            if (_currentPage == 1)
            {
                btnPrevious.IsEnabled = false;
            }
            else
            {
                btnPrevious.IsEnabled = true;
            }

            if (IsFilteringDisabled())
            {
                if (_currentPage == Math.Ceiling((decimal)customers.Count / _pageSize))
                {
                    btnNext.IsEnabled = false;
                }
                else
                {
                    btnNext.IsEnabled = true;
                }
            }
            else
            {

                if (_currentPage == Math.Ceiling((decimal)filteredCustomers.Count / _pageSize))
                {
                    btnNext.IsEnabled = false;
                }
                else
                {
                    btnNext.IsEnabled = true;
                }
            }
        }
        private void LoadPage()
        {
            if (IsFilteringDisabled())
            {

                int startIndex = (_currentPage - 1) * _pageSize;
                int endIndex = Math.Min(startIndex + _pageSize - 1, customers.Count - 1);

                var currentPageData = new ObservableCollection<Customer>();

                for (int i = startIndex; i <= endIndex; i++)
                {
                    currentPageData.Add(customers[i]);
                }

                customerDataGrid.ItemsSource = currentPageData;

                currentPageText.Text = $"Сахифа {_currentPage} / {Math.Ceiling((decimal)customers.Count / _pageSize)}";
                PopulateNumberOfCustomersTxt();

                CheckPaginationButtonStates();
            }
            else
            {
                int startIndex = (_currentPage - 1) * _pageSize;
                int endIndex = Math.Min(startIndex + _pageSize - 1, filteredCustomers.Count - 1);

                var currentPageData = new ObservableCollection<Customer>();

                for (int i = startIndex; i <= endIndex; i++)
                {
                    currentPageData.Add(filteredCustomers[i]);
                }

                customerDataGrid.ItemsSource = currentPageData;

                currentPageText.Text = $"Сахифа {_currentPage} / {Math.Ceiling((decimal)filteredCustomers.Count / _pageSize)}";
                PopulateNumberOfCustomersTxt();

                CheckPaginationButtonStates();
            }
        }
        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                LoadPage();
                CheckPaginationButtonStates();
            }
        }
        private void ResetFilters()
        {
            searchBar.Text = string.Empty;

        }
        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            int totalPages;
            if (IsFilteringDisabled())
            {
                totalPages = (int)Math.Ceiling((double)customers.Count / _pageSize);
            }
            else
            {
                totalPages = (int)Math.Ceiling((double)filteredCustomers.Count / _pageSize);
            }
            if (_currentPage < totalPages)
            {
                _currentPage++;
                LoadPage();
                CheckPaginationButtonStates();

            }
        }
        private void btnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            var addCustomerWindow = new AddCustomerWindow();
            addCustomerWindow.AddCustomerButtonClicked += AddCustomerWindow_AddCustomerButtonClicked;
            this.Opacity = 0.4;
            addCustomerWindow.ShowDialog();
            this.Opacity = 1;
        }

        private void AddCustomerWindow_AddCustomerButtonClicked(object? sender, EventArgs e)
        {
            PopulateDataGrid();
            LoadPage();
        }


        private void btnDeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (button.DataContext is Customer customer)
                {
                    var choice = MessageBox.Show($"Are you sure to delete customer : {customer.Name} ", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                    if (choice == MessageBoxResult.Yes)
                    {

                        _customerService.Delete(customer);

                        notificationManager.Show("Success", "Customer Deleted successfully", NotificationType.Success);

                        PopulateDataGrid();
                        LoadPage();
                    }
                }

            }
        }



        private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Stop the timer (if it's running)
            debounceTimer.Stop();

            // Start the timer again
            debounceTimer.Start();
        }
        private void Search()
        {
            _currentPage = 1;
            if (IsFilteringDisabled())
            {
                LoadPage();
                txtNumberOfCustomersInDb.Text = $"Bazada mavjud jami mijozlar soni : {customers.Count}";

            }
            else
            {
                customerDataGrid.ItemsSource = customers;
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(customerDataGrid.ItemsSource);
                view.Filter = SearchFilter;
                filteredCustomers = new ObservableCollection<Customer>(view.Cast<Customer>().ToList());
                LoadPage();
                txtNumberOfCustomersInDb.Text = $"Bazada mavjud jami mijozlar soni : {filteredCustomers.Count}";
            }



        }

        private bool SearchFilter(object item)
        {
            bool isMatching = true;
            if (IsFilteringDisabled())
                return true;

            Customer customer = item as Customer;

            if (customer == null)
                return false;

            if (!string.IsNullOrWhiteSpace(searchBar.Text))
            {
                isMatching = customer.Name.Contains(searchBar.Text, StringComparison.OrdinalIgnoreCase);
                isMatching = isMatching || customer.Address.Contains(searchBar.Text, StringComparison.OrdinalIgnoreCase);
            }

            return isMatching;
        }

        private void btnClearSearchBar_Click(object sender, RoutedEventArgs e)
        {
            searchBar.Clear();
            LoadPage();
            txtNumberOfCustomersInDb.Text = $"Bazada mavjud jami mijozlar soni : {customers.Count}";
        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = (Customer)(sender as Button).DataContext;

            OrdersHistoryPage ordersHistoryPage = new OrdersHistoryPage(customer);
            NavigationService?.Navigate(ordersHistoryPage);

        }

        private void customerDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var editedCustomer = e.Row.Item as Customer;

            string propertyName = e.Column.SortMemberPath;

            var editedValue = (e.EditingElement as TextBox).Text;
            switch (propertyName)
            {
                case "Name":
                    if (string.IsNullOrWhiteSpace(editedValue.ToString()))
                    {
                        notificationManager.Show("Error", "Bu bosh joy bo`lishi mumknimas.", NotificationType.Error);
                        (e.EditingElement as TextBox).Text = editedCustomer.Name.ToString();
                        return;
                    }
                    else
                    {
                        editedCustomer.Name = editedValue.ToString();
                    }
                    break;
                case "Address":
                    // Address is optional property
                    editedCustomer.Address = editedValue.ToString();
                    break;
                case "Phone":
                    if (string.IsNullOrWhiteSpace(editedValue.ToString()))
                    {
                        notificationManager.Show("Error", "Bu bosh joy bo`lishi mumknimas.", NotificationType.Error);
                        (e.EditingElement as TextBox).Text = editedCustomer.Phone.ToString();
                        return;
                    }
                    else
                    {
                        editedCustomer.Phone = editedValue.ToString();
                    }
                    break;

                default:
                    break;
            }

            _customerService.Update(editedCustomer);
            notificationManager.Show("Success", $"{propertyName} changed successfully", NotificationType.Success);
        }

        private void customerDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.DataContext is Customer customer)
            {
                if (customer.Debt > 0)
                {
                    e.Row.Foreground = Brushes.Red;
                }
                if (customer.Debt < 0)
                {
                    e.Row.Foreground = Brushes.Green;
                }
            }
        }

        private void btnPayDebt_Click(object sender, RoutedEventArgs e)
        {
            IncomeInputWindow incomeInputWindow = new IncomeInputWindow();
            this.Opacity = 0.4;
            incomeInputWindow.ShowDialog();
            this.Opacity = 1;

            if (!incomeInputWindow.IsResultSuccessful())
            {
                return;
            }

            Customer customer = (Customer)(sender as Button).DataContext;
            customer.Debt -= incomeInputWindow.GetTotalPaidAmount();
            _customerService.Update(customer);
            notificationManager.Show("Success", "Debt changed successfully", NotificationType.Success);
            LoadPage();
        }
    }
}

using InventoryManagementSystem.Model;
using InventoryManagementSystem.Services;
using InventoryManagementSystem.View;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Notification.Wpf;
using NPOI.SS.Formula.Functions;
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
        private DebtWithdrawalService _debtWithdrawalService;
        private int _pageSize = Properties.Settings.Default.CustomersPerPage;
        private int _currentPage = 1;
        private int _customersCountInDb;
        private System.Timers.Timer debounceTimer;

        public CustomersPage()
        {
            SetupTimerSettings();
            _customerService = new(new AppDbContext());
            _debtWithdrawalService = new(new AppDbContext());
            _customersCountInDb = _customerService.GetCustomersCount();
            InitializeComponent();
            SetupUserCustomizationsSettings();
            LoadPage();
            PopulateNumberOfCustomersTxt();
            notificationManager = new();
            txtNumberOfCustomersInDb.Text = $"Базада мавжуд жами мижозлар сони : {_customersCountInDb}";

        }
        private void SetupUserCustomizationsSettings()
        {
            this.FontFamily = new FontFamily(Properties.Settings.Default.AppFontFamily);

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
            txtNumberOfCustomers.Text = $"Жадвалдаги мижозлар сони : {customerDataGrid.Items.Count}";
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
                MessageBox.Show(ex.ToString(), "Хатолик !", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private bool IsFilteringDisabled()
        {
            return string.IsNullOrWhiteSpace(searchBar.Text);
        }
        private void CheckPaginationButtonStates()
        {
            btnPrevious.IsEnabled = _currentPage != 1;

            int totalPages = IsFilteringDisabled()
                ? (int)Math.Ceiling((decimal)_customersCountInDb / _pageSize)
                : (int)Math.Ceiling((decimal)filteredCustomers.Count / _pageSize);

            btnNext.IsEnabled = _currentPage != totalPages;


        }
        private void LoadPage()
        {
            if (IsFilteringDisabled())
            {
                int startIndex = (_currentPage - 1) * _pageSize;
                var currentPageData = new ObservableCollection<Customer>(_customerService.GetPageOfCustomers(startIndex, _pageSize));
                customerDataGrid.ItemsSource = currentPageData;
                currentPageText.Text = $"Сахифа {_currentPage} / {Math.Ceiling((decimal)_customersCountInDb / _pageSize)}";

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

            }
            PopulateNumberOfCustomersTxt();

            CheckPaginationButtonStates();
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

            totalPages = IsFilteringDisabled()
                            ? (int)Math.Ceiling((double)_customersCountInDb / _pageSize)
                            : (int)Math.Ceiling((double)filteredCustomers.Count / _pageSize);

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
            LoadPage();

            _customersCountInDb++;
            txtNumberOfCustomersInDb.Text = $"Базада мавжуд жами продуктлар сони : {_customersCountInDb}";
            if (IsFilteringDisabled())
            {
                txtNumberOfCustomersInDb.Text = $"Базада мавжуд жами продуктлар сони : {_customersCountInDb}";

                currentPageText.Text = $"Сахифа {_currentPage} / {Math.Ceiling((decimal)_customersCountInDb / _pageSize)}";
            }
            else
            {
                currentPageText.Text = $"Сахифа {_currentPage} / {Math.Ceiling((decimal)filteredCustomers.Count / _pageSize)}";

            }
            CheckPaginationButtonStates();

        }


        private void btnDeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (button.DataContext is Customer customer)
                {
                    var choice = MessageBox.Show($"{customer.Name} исмли мижозни учириб ташламокчимисиз ?", "Огохлантириш !", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                    if (choice == MessageBoxResult.Yes)
                    {

                        if (customer.TotalOrdersCount > 0)
                        {
                            MessageBox.Show($"{customer.Name} : мижоз аввал товар сотиб олгани учун, учириб ташлаш мумкин емас !", "Хатолик !", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        _customerService.Delete(customer);

                        _customersCountInDb--;
                        if (IsFilteringDisabled())
                        {
                            txtNumberOfCustomersInDb.Text = $"Базада мавжуд жами продуктлар сони : {_customersCountInDb}";

                        }
                        else
                        {
                            txtNumberOfCustomersInDb.Text = $"Базада мавжуд жами продуктлар сони : {filteredCustomers.Count}";

                        }

                        notificationManager.Show("Муваффакият !", "Мижоз учирилди !", NotificationType.Success);

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
                txtNumberOfCustomersInDb.Text = $"Базада мавжуд жами мижозлар сони : {_customersCountInDb}";

            }
            else
            {
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(_customerService.GetAll());
                view.Filter = SearchFilter;
                filteredCustomers = new ObservableCollection<Customer>(view.Cast<Customer>().ToList());
                LoadPage();
                txtNumberOfCustomersInDb.Text = $"Базада мавжуд жами мижозлар сони : {filteredCustomers.Count}";
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
            txtNumberOfCustomersInDb.Text = $"Базада мавжуд жами мижозлар сони : {_customersCountInDb}";
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
                        notificationManager.Show("Хатолик !", "ФИО киритинг !", NotificationType.Error);
                        (e.EditingElement as TextBox).Text = editedCustomer.Name.ToString();
                        return;
                    }
                    else
                    {
                        editedCustomer.Name = editedValue.ToString();
                        notificationManager.Show("Муваффакият !", "Мижоз исми янгиланди", NotificationType.Success);

                    }
                    break;
                case "Address":
                    // Address is optional property
                    editedCustomer.Address = editedValue.ToString();
                    notificationManager.Show("Муваффакият !", "Мижоз аддреси янгиланди", NotificationType.Success);

                    break;
                case "Phone":


                    editedCustomer.Phone = editedValue.ToString();
                    notificationManager.Show("Муваффакият !", "Мижоз телефон номери янгиланди", NotificationType.Success);

                    break;

                default:
                    break;
            }

            _customerService.Update(editedCustomer);
        }

        private void customerDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.DataContext is Customer customer)
            {
                if (customer.Debt > 0)
                {
                    e.Row.Foreground = Brushes.Red;
                }
               /* if (customer.Debt < 0)
                {
                    e.Row.Foreground = Brushes.Green;
                }*/
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

            _debtWithdrawalService.Add(new DebtWithdrawal()
            {
                Amount = incomeInputWindow.GetTotalPaidAmount(),
                CustomerId = customer.Id
            });

            _customerService.Update(customer);
            notificationManager.Show("Муваффакият !", "Карз туланди", NotificationType.Success);
            LoadPage();
        }

       
    }
}

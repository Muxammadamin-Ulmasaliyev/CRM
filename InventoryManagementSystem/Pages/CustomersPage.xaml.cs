using InventoryManagementSystem.Model;
using InventoryManagementSystem.Services;
using InventoryManagementSystem.View;
using Notification.Wpf;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace InventoryManagementSystem.Pages
{
    public partial class CustomersPage : Page
    {
        private ObservableCollection<Customer> customers;
        private NotificationManager notificationManager;
        private readonly CustomerService _customerService;
        public CustomersPage()
        {
            _customerService = new(new AppDbContext());
            InitializeComponent();
            PopulateDataGrid();
            notificationManager = new();
        }
        private void PopulateDataGrid()
        {
            customers = new ObservableCollection<Customer>(_customerService.GetAll());
            customerDataGrid.ItemsSource = customers;
            txtNumberOfCustomers.Text = $"Jadvaldagi Mijozlar soni : {customerDataGrid.Items.Count}";
        }


        private void btnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            var addCustomerWindow = new AddCustomerWindow();
            addCustomerWindow.AddCustomerButtonClicked += AddCustomerWindow_AddCustomerButtonClicked;
            addCustomerWindow.Show();
        }

        private void AddCustomerWindow_AddCustomerButtonClicked(object? sender, EventArgs e)
        {
            PopulateDataGrid();
        }

        private void btnDeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (button.DataContext is Customer customer)
                {
                    var choice = MessageBox.Show($"Are you sure to delete customer : {customer.Name} ", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (choice == MessageBoxResult.Yes)
                    {

                        _customerService.Delete(customer);

                        notificationManager.Show("Success", "Customer Deleted successfully", NotificationType.Success);
                    }
                }
            }
            PopulateDataGrid();
        }

        private void btnEditCustomer_Click(object sender, RoutedEventArgs e)
        {
            Customer customerToEdit = (Customer)(sender as Button).DataContext;
            var editCustomerWindow = new EditCustomerWindow(customerToEdit);
            editCustomerWindow.ShowDialog();
            PopulateDataGrid();
        }

        private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            Search();
        }
        private void Search()
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(customerDataGrid.ItemsSource);
            view.Filter = SearchFilter;
            txtNumberOfCustomers.Text = $"Jadvaldagi Mijozlar soni : {view.Cast<object>().Count()}";

        }

        private bool SearchFilter(object item)
        {
            bool isMatching = true;
            if (string.IsNullOrWhiteSpace(searchBar.Text))
                return true;

            Customer customer = item as Customer;

            if (customer == null)
                return false;

            if (!string.IsNullOrWhiteSpace(searchBar.Text))
            {
                isMatching = customer.Name.Contains(searchBar.Text, StringComparison.OrdinalIgnoreCase);
                isMatching = isMatching || customer.Address.Contains(searchBar.Text, StringComparison.OrdinalIgnoreCase);
                isMatching = isMatching || customer.Phone.Contains(searchBar.Text, StringComparison.OrdinalIgnoreCase);
            }

            return isMatching;
        }

        private void btnClearSearchBar_Click(object sender, RoutedEventArgs e)
        {
            searchBar.Clear();
        }

        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            Customer customer = (Customer)(sender as Button).DataContext;

            OrdersHistoryPage ordersHistoryPage = new OrdersHistoryPage(customer);
            NavigationService?.Navigate(ordersHistoryPage);

        }
    }
}

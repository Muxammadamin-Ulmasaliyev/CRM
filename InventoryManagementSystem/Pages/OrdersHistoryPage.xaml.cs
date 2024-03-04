using InventoryManagementSystem.Model;
using InventoryManagementSystem.Services;
using Microsoft.EntityFrameworkCore;
using Notification.Wpf;
using NPOI.SS.Formula.Functions;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace InventoryManagementSystem.Pages
{
    public partial class OrdersHistoryPage : Page
    {
        private ObservableCollection<Order> orders;
        private ObservableCollection<OrderDetail> orderDetails;
        private Customer customer;
        private NotificationManager notificationManager;


        private readonly OrderService _orderService;
        private readonly OrderDetailService _orderDetailService;

        private int _pageSizeOrderDetails = Properties.Settings.Default.OrderDetailsPerPage;
        private int _pageSizeOrders = Properties.Settings.Default.OrdersPerPage;
        private int _currentPage = 1;
        private int _currentPage2 = 1;

        public OrdersHistoryPage(Customer customer)
        {
            _orderService = new(new AppDbContext());
            _orderDetailService = new(new AppDbContext());
            this.customer = customer;
            InitializeComponent();
            SetupUserCustomizationsSettings();
            PopulateOrdersDataGrid();
            LoadPageOrders();
            notificationManager = new();
            txtCustomerName.Text = customer.Name;
        }
        private void SetupUserCustomizationsSettings()
        {
            this.FontFamily = new FontFamily(Properties.Settings.Default.AppFontFamily);

            ordersDataGrid.FontSize = Properties.Settings.Default.OrderHistoryDataGridFontSize;
            orderDetailsDataGrid.FontSize = Properties.Settings.Default.OrderHistoryDetailsDataGridFontSize;
        }
        private void PopulateOrdersDataGrid()
        {
            orders = new ObservableCollection<Order>(_orderService.GetOrdersByCustomerId(customer.Id));
            ordersDataGrid.ItemsSource = orders;
        }

        private void PopulateOrderDetailsDataGrid(int orderId)
        {
            orderDetails = new ObservableCollection<OrderDetail>(_orderDetailService.GetOrderDetailsByOrderId(orderId));
            orderDetailsDataGrid.ItemsSource = orderDetails;

        }

        private void btnShowDetails_Click(object sender, RoutedEventArgs e)
        {
            Order order = (Order)(sender as Button).DataContext;
            PopulateOrderDetailsDataGrid(order.Id);
            txtOrderDetailsTitle.Text = $"Списка заказ - {order.Id}";
            LoadPageOrderDetails();

        }

        private void btnPrintPdfCheque_Click(object sender, RoutedEventArgs e)
        {
            Order selectedOrder = (Order)(sender as Button).DataContext;


            Order order = _orderService.GetOrderById(selectedOrder.Id);

            GeneretePDFCheque(order);

            notificationManager.Show("Муваффакият !", "Чек яратилди !", NotificationType.Success, onClick: () => OpenChequesFolder());


        }
        private void OpenChequesFolder()
        {

            var folderPath = Properties.Settings.Default.OrderChequesDirectoryPath;
            // Check if the folder exists before attempting to open it
            if (System.IO.Directory.Exists(folderPath))
            {
                try
                {
                    // Start the process with specific information
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "explorer.exe",  // Explorer is used to open folders
                        Arguments = folderPath,     // Path to the folder
                        UseShellExecute = true      // Use the shell to execute (open with default application)
                    };

                    Process.Start(psi);
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that may occur
                    MessageBox.Show($"Papkani ochishda xatolik : {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void GeneretePDFCheque(Order order)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var filePath = $"{Properties.Settings.Default.OrderChequesDirectoryPath}/{order.Customer.Name} {order.OrderDate.ToString("dd-MM-yyyy")} {order.Id}.pdf";

            var model = order;
            var document = new ChequeDocument(model);
            document.GeneratePdf(filePath);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private void scrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
                e.Handled = true;
            }
        }

        private void LoadPageOrders()
        {
            int startIndex = (_currentPage - 1) * _pageSizeOrders;
            int endIndex = Math.Min(startIndex + _pageSizeOrders - 1, orders.Count - 1);

            var currentPageData = new ObservableCollection<Order>();

            for (int i = startIndex; i <= endIndex; i++)
            {
                currentPageData.Add(orders[i]);
            }

            ordersDataGrid.ItemsSource = currentPageData;

            currentPageText.Text = $"Сахифа {_currentPage} / {Math.Ceiling((decimal)orders.Count / _pageSizeOrders)}";
            CheckPaginationButtonStatesOrders();
        }

        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                LoadPageOrders();
                CheckPaginationButtonStatesOrders();

            }
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            int totalPages = (int)Math.Ceiling((double)orders.Count / _pageSizeOrders);
            if (_currentPage < totalPages)
            {
                _currentPage++;
                LoadPageOrders();
                CheckPaginationButtonStatesOrders();

            }
        }

        private void CheckPaginationButtonStatesOrders()
        {
            // Orders
            if (_currentPage == 1)
            {
                btnPreviousOrderPage.IsEnabled = false;
            }
            else
            {
                btnPreviousOrderPage.IsEnabled = true;
            }
            if (_currentPage == Math.Ceiling((decimal)orders.Count / _pageSizeOrders))
            {
                btnNextOrderPage.IsEnabled = false;
            }
            else
            {
                btnNextOrderPage.IsEnabled = true;
            }
            
        }
        private void CheckPaginationButtonStatesOrderDetails()
        {
            if (_currentPage2 == 1)
            {
                btnPreviousOrderDetailsPage.IsEnabled = false;
            }
            else
            {
                btnPreviousOrderDetailsPage.IsEnabled = true;
            }
            if (_currentPage2 == Math.Ceiling((decimal)orderDetails.Count / _pageSizeOrderDetails))
            {
                btnNextOrderDetailsPage.IsEnabled = false;
            }
            else
            {
                btnNextOrderDetailsPage.IsEnabled = true;
            }
        }
        private void LoadPageOrderDetails()
        {
            int startIndex = (_currentPage2 - 1) * _pageSizeOrderDetails;
            int endIndex = Math.Min(startIndex + _pageSizeOrderDetails - 1, orderDetails.Count - 1);

            var currentPageData = new ObservableCollection<OrderDetail>();

            for (int i = startIndex; i <= endIndex; i++)
            {
                currentPageData.Add(orderDetails[i]);
            }

            orderDetailsDataGrid.ItemsSource = currentPageData;

            currentPageText2.Text = $"Сахифа {_currentPage2} / {Math.Ceiling((decimal)orderDetails.Count / _pageSizeOrderDetails)}";
            CheckPaginationButtonStatesOrderDetails();

        }

        private void PreviousPage_Click2(object sender, RoutedEventArgs e)
        {
            if (_currentPage2 > 1)
            {
                _currentPage2--;
                LoadPageOrderDetails();
                CheckPaginationButtonStatesOrderDetails();

            }
        }

        private void NextPage_Click2(object sender, RoutedEventArgs e)
        {
            int totalPages = (int)Math.Ceiling((double)orderDetails.Count / _pageSizeOrderDetails);
            if (_currentPage2 < totalPages)
            {
                _currentPage2++;
                LoadPageOrderDetails();
                CheckPaginationButtonStatesOrderDetails();

            }
        }

        private void ordersDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            if (e.Row.DataContext is Order order)
            {
                if (order.TotalAmount > order.TotalPaidAmount)
                {
                    e.Row.Foreground = Brushes.Red;
                }
                if (order.TotalAmount < order.TotalPaidAmount)
                {
                    e.Row.Foreground = Brushes.Green;
                }
            }
        }
    }
}

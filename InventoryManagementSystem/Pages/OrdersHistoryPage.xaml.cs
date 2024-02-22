using InventoryManagementSystem.Model;
using InventoryManagementSystem.Services;
using Microsoft.EntityFrameworkCore;
using Notification.Wpf;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

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
        public OrdersHistoryPage(Customer customer)
        {
            _orderService = new(new AppDbContext());
            _orderDetailService = new(new AppDbContext());
            this.customer = customer;
            InitializeComponent();
            PopulateOrdersDataGrid();
            notificationManager = new();
            txtCustomerName.Text = customer.Name;
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
            txtOrderDetailsTitle.Text = $"OrderDetails -> Id : {order.Id}";
        }

        private void btnPrintPdfCheque_Click(object sender, RoutedEventArgs e)
        {
            Order selectedOrder = (Order)(sender as Button).DataContext;


            Order order = _orderService.GetOrderById(selectedOrder.Id);

            GeneretePDFCheque(order);

            notificationManager.Show("Success", "Cheque created successfully", NotificationType.Success);


        }

        private void GeneretePDFCheque(Order order)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var filePath = $"D://{order.Customer.Name} {order.OrderDate.ToString("dd-MM-yyyy")}.pdf";

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
    }
}

using InventoryManagementSystem.Model;
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
        public OrdersHistoryPage(Customer customer)
        {
            this.customer = customer;
            InitializeComponent();
            PopulateOrdersDataGrid();
            notificationManager = new();
            txtCustomerName.Text = customer.Name;
        }
        private void PopulateOrdersDataGrid()
        {
            orders = new ObservableCollection<Order>(GetOrdersByCustomerId());
            ordersDataGrid.ItemsSource = orders;
        }

        private void PopulateOrderDetailsDataGrid(int orderId)
        {
            orderDetails = new ObservableCollection<OrderDetail>(GetOrderDetailsByOrderId(orderId));
            orderDetailsDataGrid.ItemsSource = orderDetails;

        }

        private List<Order> GetOrdersByCustomerId()
        {
            using (var dbContext = new AppDbContext())
            {
                return dbContext.Orders.Where(c => c.CustomerId == customer.Id).ToList();
            }
        }
        private List<OrderDetail> GetOrderDetailsByOrderId(int orderId)
        {
            using (var dbContext = new AppDbContext())
            {
                return dbContext.OrderDetails
                         .Include(o => o.Product)
                         .ThenInclude(p => p.Company)
                         .Include(o => o.Product)
                         .ThenInclude(p => p.CarType)
                         .Include(o => o.Product)
                         .ThenInclude(p => p.Country)
                    .Where(o => o.OrderId == orderId).ToList();
            }
        }


        private void btnShowDetails_Click(object sender, RoutedEventArgs e)
        {
            Order order = (Order)(sender as Button).DataContext;
            PopulateOrderDetailsDataGrid(order.Id);
            txtOrderDetailsTitle.Text = $"OrderDetails -> Id : {order.Id}";
        }

        private void btnPrintPdfCheque_Click(object sender, RoutedEventArgs e)
        {
            Order orderFromTable = (Order)(sender as Button).DataContext;

            using (var dbContext = new AppDbContext())
            {

                var order = dbContext.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                    .ThenInclude(p => p.CarType)

                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                    .ThenInclude(p => p.Country)

                    .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                    .ThenInclude(p => p.Company)


                    .First(o => o.Id == orderFromTable.Id);



                QuestPDF.Settings.License = LicenseType.Community;
                var filePath = "D://invoiceFromHistory.pdf";

                var model = order;
                var document = new ChequeDocument(model);
                document.GeneratePdf(filePath);

                notificationManager.Show("Success", "Cheque created successfully", NotificationType.Success);

            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }
    }
}

using InventoryManagementSystem.Model;
using InventoryManagementSystem.Services;
using Notification.Wpf;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using System.Collections.ObjectModel;
using System.Windows;

namespace InventoryManagementSystem.View
{
    public partial class CurrentOrderDetails : Window
    {
        private Order currentOrder;
        public ObservableCollection<OrderDetail> orderDetails;
        private NotificationManager notificationManager;
        public event EventHandler OrderSavedButtonClicked;
        private readonly OrderDetailService _orderDetailService;
        private readonly OrderService _orderService;
        private readonly ProductService _productService;


        public CurrentOrderDetails(Order currentOrder, Customer currentCustomer)
        {
            notificationManager = new();
            _orderDetailService = new(new AppDbContext());
            _orderService = new(new AppDbContext());
            _productService = new(new AppDbContext());

            this.currentOrder = currentOrder;
            InitializeComponent();
            PopulateDataGrid();
            CanSaveOrder();
            txtCustomerName.Text = $"Mijoz : {(currentCustomer != null ? currentCustomer.Name : "-")}";
            ShowTotalSums();
        }

        private void ShowTotalSums()
        {
            txtOrderTotalSumUzs.Text = $"Jami Sum : {(currentOrder.TotalAmount)}";
        }
        private void CalculateOrderTotals()
        {
            currentOrder.TotalAmount = currentOrder.OrderDetails.Sum(od => od.SubTotal);
        }
        private void CanSaveOrder()
        {
            if (orderDetails.Count > 0)
            {
                btnSaveOrder.IsEnabled = true;
            }
            else
            {
                btnSaveOrder.IsEnabled = false;
            }
        }
        private void PopulateDataGrid()
        {
            orderDetails = new ObservableCollection<OrderDetail>(currentOrder.OrderDetails);
            orderDetailsDataGrid.ItemsSource = orderDetails;
        }
        private void btnDeleteOrderDetail_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = orderDetailsDataGrid.SelectedItem as OrderDetail;
            currentOrder.OrderDetails.Remove(selectedItem);
            orderDetails.Remove(selectedItem);
            CanSaveOrder();
            CalculateOrderTotals();
            ShowTotalSums();

        }

        private void btnSaveOrder_Click(object sender, RoutedEventArgs e)
        {
            var order = new Order()
            {
                OrderDate = DateTime.Today,
                TotalAmount = currentOrder.TotalAmount,
                CustomerId = currentOrder.CustomerId
            };

            var newOrder = _orderService.AddOrder(order);

            foreach (var orderDetail in orderDetails)
            {
                var orderDetailToAdd = new OrderDetail()
                {
                    SubTotal = orderDetail.SubTotal,
                    Quantity = orderDetail.Quantity,
                    OrderId = newOrder.Entity.Id,
                    ProductId = orderDetail.Product.Id,
                    Price = orderDetail.Price,
                };

                _orderDetailService.AddOrderDetail(orderDetailToAdd);
            }
            notificationManager.Show("Success", "Order saved successfully", NotificationType.Success);

            Close();
            WithdrawalSoldProducts();
            OrderSaved();

            currentOrder.Id = newOrder.Entity.Id;

            GeneretePDFCheque(currentOrder);

            orderDetails.Clear();
            currentOrder = null;

        }
        private void GeneretePDFCheque(Order order)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var filePath = "D://invoice.pdf";
            var model = order;
            var document = new ChequeDocument(model);
            document.GeneratePdf(filePath);

            //Process.Start("explorer.exe", filePath);

        }

        // REFACTOR -------------------------------------------------------------
        private void WithdrawalSoldProducts()
        {

            foreach (var item in currentOrder.OrderDetails)
            {
                var productToUpdate = _productService.GetProduct(item.Product.Id);
                productToUpdate.Quantity -= item.Quantity;
                _productService.UpdateQuantity(productToUpdate);
            }
        }

        protected virtual void OrderSaved()
        {
            OrderSavedButtonClicked?.Invoke(this, EventArgs.Empty);
        }




    }
}

using InventoryManagementSystem.Model;
using InventoryManagementSystem.Services;
using Notification.Wpf;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace InventoryManagementSystem.Pages
{
    public partial class CartPage : Page
    {
        private Order currentOrder;
        private Customer currentCustomer;

        public ObservableCollection<OrderDetail> orderDetails;
        private NotificationManager notificationManager;
        public event EventHandler OrderSavedButtonClicked;
        private readonly OrderDetailService _orderDetailService;
        private readonly OrderService _orderService;
        private readonly ProductService _productService;
        private readonly CustomerService _customerService;

        public CartPage(Order currentOrder)
        {
            _orderDetailService = new(new AppDbContext());
            _orderService = new(new AppDbContext());
            _productService = new(new AppDbContext());
            _customerService = new(new AppDbContext());

            notificationManager = new();
            this.currentOrder = currentOrder;
            InitializeComponent();
            PopulateDataGrid();
            PopulateCustomersComboBox();
            CanSaveOrder();
            ShowTotalSums();
        }

        private void DisplayCustomerName()
        {

            txtCustomerName.Text = $"Mijoz : {(currentCustomer != null ? currentCustomer.Name : "-")}";
        }
        private void PopulateCustomersComboBox()
        {
            cbCurrentCustomer.ItemsSource = _customerService.GetAll();
        }


        private void ShowTotalSums()
        {
            txtOrderTotalSumUzs.Text = $"Jami Sum : {currentOrder.TotalAmount.ToString("c0", new CultureInfo("uz-UZ"))}";
        }
        private void CalculateOrderTotals()
        {
            currentOrder.TotalAmount = currentOrder.OrderDetails.Sum(od => od.SubTotal);
        }
        private void CanSaveOrder()
        {
            if (orderDetails.Count > 0 && currentOrder.CustomerId != 0)
            {
                btnSaveOrder.IsEnabled = true;
            }
            else
            {
                btnSaveOrder.IsEnabled = false;
            }
        }
        private void IncrementOrdersCountOfCustomer(int customerId)
        {
            currentCustomer.TotalOrdersCount++;
            _customerService.Update(currentCustomer);
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


            WithdrawalSoldProducts();
            OrderSaved();
            IncrementOrdersCountOfCustomer(currentCustomer.Id);
            currentOrder.Id = newOrder.Entity.Id;

            GeneretePDFCheque(currentOrder);

            txtOrderTotalSumUzs.Text = string.Empty;
            orderDetails.Clear();
            currentOrder = null;

        }
        private void GeneretePDFCheque(Order order)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var filePath = $"D://{order.Customer.Name} {order.OrderDate.ToString("dd-MM-yyyy")}.pdf";
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

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();

        }

        private void orderDetailsDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var editedOrderDetail = e.Row.Item as OrderDetail;

            string propertyName = e.Column.SortMemberPath;

            var editedValue = (e.EditingElement as TextBox).Text;

            switch (propertyName)
            {
                case "Price":
                    if (double.TryParse(editedValue.ToString(), out double price))
                    {
                        var orderDetail = currentOrder.OrderDetails.First(x => x.Id == editedOrderDetail.Id);
                        orderDetail.Price = price;

                        //editedOrderDetail.Price = price;
                    }
                    else
                    {

                        notificationManager.Show("Error", "Insert number pls", NotificationType.Error);
                        (e.EditingElement as TextBox).Text = editedOrderDetail.Quantity.ToString();
                        return;
                    }
                    break;
                case "Quantity":
                    if (int.TryParse(editedValue.ToString(), out int quantity))
                    {


                        var orderDetail = currentOrder.OrderDetails.First(x => x.Id == editedOrderDetail.Id);

                        var productId = orderDetail.Product.Id;

                        if (!IsSufficientQuantityInDb(productId, quantity))
                        {
                            notificationManager.Show("Error", "No sufficient amount in store", NotificationType.Error);
                            (e.EditingElement as TextBox).Text = editedOrderDetail.Quantity.ToString();

                            return;
                        }

                        orderDetail.Quantity = quantity;

                        notificationManager.Show("Success", "Edited successfully", NotificationType.Success);


                        //editedOrderDetail.Quantity = quantity;
                    }
                    else
                    {

                        notificationManager.Show("Error", "Insert number pls", NotificationType.Error);
                        (e.EditingElement as TextBox).Text = editedOrderDetail.Quantity.ToString();
                        return;
                    }
                    break;
            }

            CalculateSubTotals();
            CalculateOrderTotals();
            CanSaveOrder();
            ShowTotalSums();
            PopulateDataGrid();
        }

        private bool IsSufficientQuantityInDb(int productId, int quantity)
        {
            var productFromDb = _productService.GetProduct(productId);
            if (productFromDb.Quantity >= quantity)
            {
                return true;
            }
            return false;
        }

        private void CalculateSubTotals()
        {
            currentOrder.OrderDetails.ForEach(x => x.SubTotal = x.Price * x.Quantity);
        }

        private void cbCurrentCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currentCustomer = (Customer)cbCurrentCustomer.SelectedItem;
            currentOrder.Customer = currentCustomer;
            currentOrder.CustomerId = currentCustomer.Id;
            DisplayCustomerName();
            CanSaveOrder();
        }
    }
}

using ControlzEx.Standard;
using InventoryManagementSystem.Model;
using InventoryManagementSystem.Services;
using InventoryManagementSystem.View;
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
            SetupUserCustomizationsSettings();
            PopulateDataGrid();
            PopulateCustomersComboBox();
            CanSaveOrder();
            ShowTotalSums();
        }

        private void SetupUserCustomizationsSettings()
        {
            orderDetailsDataGrid.FontSize = Properties.Settings.Default.CartDataGridFontSize;
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
        private void UpdateOrdersCountAndDebtAmountOfCustomer(int customerId)
        {
            currentCustomer.TotalOrdersCount++;
            currentCustomer.Debt += (currentOrder.TotalAmount - currentOrder.TotalPaidAmount);
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
        /*//////////////////////////////////////////////////////////////////////////////////////*/
        private void btnSaveOrder_Click(object sender, RoutedEventArgs e)
        {
            var incomeInputWindow = new IncomeInputWindow();
            incomeInputWindow.ShowDialog();

            if(!incomeInputWindow.IsResultSuccessful())
            {
                return;
            }

            currentOrder.TotalPaidAmount = incomeInputWindow.GetTotalPaidAmount();

            var orderDetailsLocal = new List<OrderDetail>();
            var order = new Order()
            {
                OrderDate = DateTime.Today,
                TotalAmount = currentOrder.TotalAmount,
                CustomerId = currentOrder.CustomerId,
                TotalPaidAmount = currentOrder.TotalPaidAmount
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
                    ProductName = orderDetail.ProductName,
                    ProductCarType = orderDetail.ProductCarType,
                    ProductCompany = orderDetail.ProductCompany,
                    ProductCountry = orderDetail.ProductCountry,
                    ProductSetType = orderDetail.ProductSetType,
                    Price = orderDetail.Price,
                };

                orderDetailsLocal.Add(orderDetailToAdd);
                _orderDetailService.AddOrderDetail(orderDetailToAdd);
            }
            notificationManager.Show("Success", "Order saved successfully", NotificationType.Success);


            WithdrawalSoldProducts();
            OrderSaved();
            UpdateOrdersCountAndDebtAmountOfCustomer(currentCustomer.Id);
            currentOrder.Id = newOrder.Entity.Id;
            currentOrder.OrderDetails = orderDetailsLocal;
            GeneretePDFCheque(currentOrder);

            txtOrderTotalSumUzs.Text = string.Empty;
            orderDetails.Clear();
            currentOrder = null;

        }
        /*//////////////////////////////////////////////////////////////////////////////////////*/

        private void GeneretePDFCheque(Order order)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            var filePath = $"{Properties.Settings.Default.OrderChequesDirectoryPath}/{order.Customer.Name} {order.OrderDate.ToString("dd-MM-yyyy")} {order.Id}.pdf";
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
                    if (double.TryParse(StringHelper.TrimAllWhiteSpaces(editedValue), out double price))
                    {
                        var orderDetail = currentOrder.OrderDetails.First(x => x.ProductId == editedOrderDetail.ProductId);
                        orderDetail.Price = price;
                        notificationManager.Show("Success", "Edited successfully", NotificationType.Success);

                    }
                    else
                    {
                        notificationManager.Show("Error", "Insert number pls", NotificationType.Error);
                        (e.EditingElement as TextBox).Text = editedOrderDetail.Price.ToString();
                        return;
                    }
                    break;
                case "Quantity":
                    if (int.TryParse(StringHelper.TrimAllWhiteSpaces(editedValue), out int quantity))
                    {
                        var orderDetail = currentOrder.OrderDetails.First(x => x.ProductId == editedOrderDetail.ProductId);

                        var productId = orderDetail.Product.Id;

                        if (!IsSufficientQuantityInDb(productId, quantity))
                        {
                            notificationManager.Show("Error", "No sufficient amount in store", NotificationType.Error);
                            (e.EditingElement as TextBox).Text = editedOrderDetail.Quantity.ToString();

                            return;
                        }

                        orderDetail.Quantity = quantity;

                        notificationManager.Show("Success", "Edited successfully", NotificationType.Success);
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
            CanSaveOrder();
        }

        private void btnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            var addCustomerWindow = new AddCustomerWindow();
            this.Opacity = 0.4;
            addCustomerWindow.ShowDialog();
            this.Opacity = 1;

            PopulateCustomersComboBox();

        }
        private void productDataGrid_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {

            if (e.Column.Header.Equals("Price"))
            {
                if (e.EditingElement is TextBox textBox)
                {
                    string textWithoutSum = StringHelper.RemoveSumSignFromPrice(textBox.Text);
                    textBox.Text = textWithoutSum;

                    textBox.CaretIndex = textWithoutSum.Length;
                }
                return;
            }

        }
    }
}

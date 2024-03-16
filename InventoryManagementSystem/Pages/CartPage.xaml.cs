using InventoryManagementSystem.Model;
using InventoryManagementSystem.Services;
using InventoryManagementSystem.View;
using MethodTimer;
using Notification.Wpf;
using NPOI.DDF;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using FontFamily = System.Windows.Media.FontFamily;
using MessageBox = System.Windows.MessageBox;
using TextBox = System.Windows.Controls.TextBox;

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
            tbBarcode.Focus();
            KeyDown += btnSeachWithBarcode_KeyDown;
            IsCartEmpty();

            ShowCurrentCustomer();

        }

        private void ShowCurrentCustomer()
        {
            int index = -1;
            for (int i = 0; i < cbCurrentCustomer.Items.Count; i++)
            {
                var item = cbCurrentCustomer.Items[i] as Customer;
                if (item.Id == currentOrder.CustomerId)
                {
                    index = i;
                    break;
                }
            }

            // Set the SelectedIndex if the item is found
            if (index != -1)
            {
                cbCurrentCustomer.SelectedIndex = index;
            }
            else
            {
                cbCurrentCustomer.SelectedIndex = 0;

            }
        }

        private void IsCartEmpty()
        {
            if (currentOrder.OrderDetails.Count > 0)
            {
                Shared.Shared.IsCartEmpty = false;
            }
            else
            {
                Shared.Shared.IsCartEmpty = true;

            }
        }
        private void btnSeachWithBarcode_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

            if (tbBarcode.IsFocused)
            {
                if (e.Key == System.Windows.Input.Key.Enter)
                {
                    AddProductToCartAfterScanning(tbBarcode.Text);
                    tbBarcode.Text = string.Empty;
                    tbBarcode.Focus();
                }
            }
        }


        [Time]
        private void AddProductToCartAfterScanning(string barcode)
        {
            OrderDetail? listedOrderDetail = orderDetails.FirstOrDefault(od => od.Product.Barcode == barcode);
            if (listedOrderDetail != null)
            {
                if (listedOrderDetail.Product.Quantity > listedOrderDetail.Quantity)
                {
                    listedOrderDetail.Quantity++;
                    PopulateDataGrid();
                }
                else
                {

                    notificationManager.Show("Хатолик!", "Бу товардан колмаган !", NotificationType.Error);
                    return;
                }
            }
            else
            {
                var product = _productService.GetProductByBarcode(barcode);
                if (product == null)
                {
                    notificationManager.Show("Хатолик!", "Товар топилмади !", NotificationType.Error);
                    return;
                }
                var orderDetail = new OrderDetail
                {
                    ProductId = product.Id,
                    Product = product,
                    Quantity = 1,
                    SubTotal = (double)(product.Price > 0 ?
                                (1 * (product.Price)) :
                                (1 * product.USDPriceForCustomer * Properties.Settings.Default.CurrencyRate)),
                    Price = Math.Truncate((double)(product.Price > 0 ?
                                (product.Price) :
                                (product.USDPriceForCustomer * Properties.Settings.Default.CurrencyRate))),
                    RealPrice = Math.Truncate((double)(product.RealPrice > 0 ?
                                (product.RealPrice) :
                                (product.USDPrice * Properties.Settings.Default.CurrencyRate))),

                    ProductName = product.Name,
                    ProductCarType = product.CarType.Name,
                    ProductCompany = product.Company.Name,
                    ProductCountry = product.Country.Name,
                    ProductSetType = product.SetType.Name,

                };

                orderDetails.Add(orderDetail);
                currentOrder.OrderDetails.Add(orderDetail);
            }

            CalculateSubTotals();
            CalculateOrderTotals();
            ShowTotalSums();
            tbBarcode.Text = string.Empty;
            tbBarcode.Focus();

            CanSaveOrder();
            IsCartEmpty();


        }





        private void SetupUserCustomizationsSettings()
        {
            orderDetailsDataGrid.FontSize = Properties.Settings.Default.CartDataGridFontSize;
            this.FontFamily = new FontFamily(Properties.Settings.Default.AppFontFamily);
        }
        private void PopulateCustomersComboBox()
        {
            cbCurrentCustomer.ItemsSource = _customerService.GetAll();
        }

        private void ShowTotalSums()
        {
            var uzCulture = new CultureInfo("uz-UZ");
            uzCulture.NumberFormat.CurrencySymbol = "сум";
            txtOrderTotalSumUzs.Text = $"Жами сумма : {currentOrder.TotalAmount.ToString("c0", uzCulture)}";
            tbBarcode.Focus();

        }
        private void CalculateOrderTotals()
        {
            currentOrder.TotalAmount = currentOrder.OrderDetails.Sum(od => od.SubTotal);
            tbBarcode.Focus();

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
            tbBarcode.Focus();

        }
        private void UpdateOrdersCountAndDebtAmountOfCustomer(int customerId)
        {
            currentCustomer.TotalOrdersCount++;
            currentCustomer.Debt += (currentOrder.TotalAmount - currentOrder.TotalPaidAmount);
            _customerService.Update(currentCustomer);
            tbBarcode.Focus();

        }
        private void PopulateDataGrid()
        {
            try
            {

                orderDetails = new ObservableCollection<OrderDetail>(currentOrder.OrderDetails);
                orderDetailsDataGrid.ItemsSource = orderDetails;
            }
            catch (Exception ex)
            {

            }
        }
        private void btnDeleteOrderDetail_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = orderDetailsDataGrid.SelectedItem as OrderDetail;
            currentOrder.OrderDetails.Remove(selectedItem);
            orderDetails.Remove(selectedItem);
            CanSaveOrder();
            CalculateOrderTotals();
            ShowTotalSums();
            IsCartEmpty();
            tbBarcode.Focus();

        }
        private void btnSaveOrder_Click(object sender, RoutedEventArgs e)
        {
            var incomeInputWindow = new IncomeInputWindow();
            incomeInputWindow.ShowDialog();

            if (!incomeInputWindow.IsResultSuccessful())
            {
                tbBarcode.Focus();
                return;
            }

            currentOrder.TotalPaidAmount = incomeInputWindow.GetTotalPaidAmount();

            var order = new Order()
            {
                OrderDate = DateTime.Now,
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
                    RealPrice = orderDetail.RealPrice,
                };

                _orderDetailService.AddOrderDetail(orderDetailToAdd);
            }


            WithdrawalSoldProducts();
            OrderSaved();
            UpdateOrdersCountAndDebtAmountOfCustomer(currentCustomer.Id);
            currentOrder.Id = newOrder.Entity.Id;
            // currentOrder.OrderDetails = orderDetailsLocal;

            GeneretePDFCheque(currentOrder);
            PrintReceiptCheque(currentOrder);
            notificationManager.Show("Муваффакият !", "Товарлар сотилди ва чек яратилди !", NotificationType.Success, onClick: () => OpenChequesFolder());

            txtOrderTotalSumUzs.Text = string.Empty;
            orderDetails.Clear();
            currentOrder = null;

            tbBarcode.Focus();

        }

        private void PrintReceiptCheque(Order order)
        {

            PrintOverviewWindow printOverviewWindow = new PrintOverviewWindow(order);
            printOverviewWindow.Show();
            printOverviewWindow.Visibility = Visibility.Collapsed;
            try
            {
                printOverviewWindow.IsEnabled = false;
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintVisual(printOverviewWindow.print, "Чек");
                }
            }
            finally
            {
                printOverviewWindow.IsEnabled = true;
            }

            printOverviewWindow.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (currentOrder != null)
            {
                PrintOverviewWindow printOverviewWindow = new PrintOverviewWindow(currentOrder);
                printOverviewWindow.ShowDialog();
            }
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
                    MessageBox.Show($"Error : {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

            //Process.Start("explorer.exe", filePath);

        }

        // REFACTOR -------------------------------------------------------------
        private void WithdrawalSoldProducts()
        {

            foreach (var item in currentOrder.OrderDetails)
            {
                var productToUpdate = _productService.GetProduct(item.Product.Id);
                productToUpdate.Quantity -= item.Quantity;
                productToUpdate.QuantitySold += item.Quantity;
                _productService.UpdateQuantity(productToUpdate);
            }
            tbBarcode.Focus();

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

                    }
                    else
                    {
                        notificationManager.Show("Хатолик !", "Сон киритинг !", NotificationType.Error);
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
                            notificationManager.Show("Хатолик !", $"Бу товардан {quantity} та дан кам колган, кичикрок сон киритинг !", NotificationType.Error);
                            (e.EditingElement as TextBox).Text = editedOrderDetail.Quantity.ToString();

                            return;
                        }

                        orderDetail.Quantity = quantity;

                    }
                    else
                    {

                        notificationManager.Show("Хатолик !", "Сон киритинг !", NotificationType.Error);
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
            tbBarcode.Focus();

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
            // ShowCurrentCustomer();
            tbBarcode.Focus();


        }

        private void btnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            var addCustomerWindow = new AddCustomerWindow();
            this.Opacity = 0.4;
            addCustomerWindow.ShowDialog();
            this.Opacity = 1;

            PopulateCustomersComboBox();
            tbBarcode.Focus();

        }
        private void productDataGrid_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {

            if (e.Column.Header.Equals("Нарх"))
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



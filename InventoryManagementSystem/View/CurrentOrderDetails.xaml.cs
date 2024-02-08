using InventoryManagementSystem.Model;
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


        public CurrentOrderDetails( Order currentOrder, Customer currentCustomer)
        {
            notificationManager = new();
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
            using (var dbContext = new AppDbContext())
            {
                var newOrder = dbContext.Orders.Add(new Order()
                {
                    OrderDate = DateTime.Today,
                    TotalAmount = currentOrder.TotalAmount,
                    CustomerId = currentOrder.CustomerId
                });
                dbContext.SaveChanges();
                foreach (var orderDetail in orderDetails)
                {
                    dbContext.OrderDetails.Add(new OrderDetail()
                    {
                        SubTotal = orderDetail.SubTotal,
                        Quantity = orderDetail.Quantity,
                        OrderId = newOrder.Entity.Id,
                        ProductId = orderDetail.Product.Id,
                        Price = orderDetail.Price,
                    });
                }
                dbContext.SaveChanges();
                notificationManager.Show("Success", "Order saved successfully", NotificationType.Success);

                Close();
                WithdrawalSoldProducts();
                OrderSaved();





                QuestPDF.Settings.License = LicenseType.Community;
                var filePath = "D://invoice.pdf";
                var model = currentOrder;
                var document = new ChequeDocument(model);
                document.GeneratePdf(filePath);

                orderDetails.Clear();
                currentOrder = null;

                //Process.Start("explorer.exe", filePath);

            }

        }

        private void WithdrawalSoldProducts()
        {
            using (var dbContext = new AppDbContext())
            {
                foreach (var item in currentOrder.OrderDetails)
                {
                    var productToUpdate = dbContext.Products.Find(item.Product.Id);
                    productToUpdate.Quantity -= item.Quantity;

                    dbContext.Products.Update(productToUpdate);

                }
                dbContext.SaveChanges();
            }
        }

        protected virtual void OrderSaved()
        {
            OrderSavedButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        private void GeneretePDFCheque()
        {
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;


            QuestPDF.Fluent.Document.Create(container =>
            {

            })
                .ShowInPreviewer();
        }


    }
}

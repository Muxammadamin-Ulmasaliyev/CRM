using InventoryManagementSystem.AppConfiguration;
using InventoryManagementSystem.Model;
using InventoryManagementSystem.View;
using Microsoft.EntityFrameworkCore;
using Notification.Wpf;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace InventoryManagementSystem.Pages
{
    public partial class HomePage : Page
    {
        private NotificationManager notificationManager;
        private ObservableCollection<Product> products;
        private Order? currentOrder;
        private Customer selectedCustomer;
        public HomePage()
        {
            InitializeComponent();
            InitializeNewOrder();
            PopulateDataGrid();
            PopulateDataGridComboboxes();
            PopulateCustomersComboBox();
            PopulateCurrencyRate();
            notificationManager = new();
        }

        private void PopulateCurrencyRate()
        {
            txtCurrencyRate.Text = $"1 $ = {AppConfiguration.Configuration.CurrencyRate} sum";
        }
        private void InitializeNewOrder()
        {
            currentOrder = new Order
            {
                OrderDetails = new List<OrderDetail>()
            };
        }
        private void PopulateCustomersComboBox()
        {
            using (var dbContext = new AppDbContext())
            {
                cbCurrentCustomer.ItemsSource = dbContext.Customers.ToList();
            }
        }
        private void PopulateDataGridComboboxes()
        {
            using (var dbContext = new AppDbContext())
            {
                cbCarType.ItemsSource = dbContext.CarTypes.ToList();
                cbCompany.ItemsSource = dbContext.Companies.ToList();
                cbCountry.ItemsSource = dbContext.Countries.ToList();
                cbSetType.ItemsSource = dbContext.SetTypes.ToList();
            }
        }
        private void PopulateDataGrid()
        {

            products = new ObservableCollection<Product>(GetProductsFromDb());
            productDataGrid.ItemsSource = products;

            txtNumberOfProducts.Text = $"Jadvaldagi produktlar soni : {productDataGrid.Items.Count}";

        }
        private List<Product> GetProductsFromDb()
        {
            using (var dbContext = new AppDbContext())
            {
                return dbContext.Products.Include(p => p.Company).Include(p => p.Country).Include(p => p.CarType).Include(p => p.SetType).ToList();
            }
        }
        private void Search()
        {
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(productDataGrid.ItemsSource);
            view.Filter = SearchFilter;

            txtNumberOfProducts.Text = $"Jadvaldagi produktlar soni : {view.Cast<object>().Count()}";


        }
        private bool SearchFilter(object item)
        {
            bool isMatching = true;
            if (
                string.IsNullOrWhiteSpace(searchBar.Text) &&
                cbCarType.SelectedItem == null &&
                cbCompany.SelectedItem == null &&
                cbCountry.SelectedItem == null &&
                cbSetType.SelectedItem == null
               )
                return true;

            Product product = item as Product;

            if (product == null)
                return false;

            if (!string.IsNullOrWhiteSpace(searchBar.Text))
            {
                isMatching = product.Name.Contains(searchBar.Text, StringComparison.OrdinalIgnoreCase);
                isMatching = isMatching || product.Code.Contains(searchBar.Text, StringComparison.OrdinalIgnoreCase);
            }
            if (cbCarType.SelectedItem != null) isMatching = isMatching && product.CarTypeId == (cbCarType.SelectedItem as CarType).Id;
            if (cbCompany.SelectedItem != null) isMatching = isMatching && product.CompanyId == (cbCompany.SelectedItem as Company).Id;
            if (cbCountry.SelectedItem != null) isMatching = isMatching && product.CountryId == (cbCountry.SelectedItem as Country).Id;
            if (cbSetType.SelectedItem != null) isMatching = isMatching && product.SetTypeId == (cbSetType.SelectedItem as SetType).Id;

            return isMatching;




        }
        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            var addProductWindow = new AddProductWindow();
            addProductWindow.AddProductButtonClicked += AddProductWindow_AddProductButtonClicked;
            addProductWindow.Show();
        }
        private void AddProductWindow_AddProductButtonClicked(object? sender, EventArgs e)
        {
            PopulateDataGrid();
            PopulateDataGridComboboxes();
        }

        private void searchBar_TextChanged_OR_ComboBox_SelectionChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            Search();
        }

        private void searchBar_TextChanged_OR_ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            Search();
        }

        private void btnAddCarType_Click(object sender, RoutedEventArgs e)
        {
            var addCarTypeWindow = new AddCarTypeWindow();
            addCarTypeWindow.AddCarTypeButtonClicked += Window_AddSomeCategoryButtonClicked;
            addCarTypeWindow.Show();
        }

        private void Window_AddSomeCategoryButtonClicked(object? sender, EventArgs e)
        {
            PopulateDataGridComboboxes();
        }

        private void btnAddCompany_Click(object sender, RoutedEventArgs e)
        {
            var addCompanyWindow = new AddCompanyWindow();
            addCompanyWindow.AddCompanyButtonClicked += Window_AddSomeCategoryButtonClicked;
            addCompanyWindow.Show();
        }

        private void btnAddCountry_Click(object sender, RoutedEventArgs e)
        {
            var addCountryWindow = new AddCountryWindow();
            addCountryWindow.AddCountryButtonClicked += Window_AddSomeCategoryButtonClicked;
            addCountryWindow.Show();
        }


        private void btnResetFilter_Click(object sender, RoutedEventArgs e)
        {
            searchBar.Text = string.Empty;
            cbCompany.SelectedItem = null;
            cbCountry.SelectedItem = null;
            cbSetType.SelectedItem = null;
            cbCarType.SelectedItem = null;
        }



        private void btnEditProduct_Click(object sender, RoutedEventArgs e)
        {
            var productToUpdate = (Product)(sender as Button).DataContext;
            var editProductWindow = new EditProductWindow(productToUpdate);
            editProductWindow.ShowDialog();
            PopulateDataGrid();
        }

        private void btnClearSearchBar_Click(object sender, RoutedEventArgs e)
        {
            searchBar.Clear();
        }

        private void btnRefreshDb_Click(object sender, RoutedEventArgs e)
        {
            PopulateDataGridComboboxes();
            PopulateDataGrid();
        }

        private void btnDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            Product product = (Product)productDataGrid.SelectedItem;
            var choice = MessageBox.Show($"Are you sure to delete product : {product.Name} ", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (choice == MessageBoxResult.Yes)
            {
                using (var dbContext = new AppDbContext())
                {
                    dbContext.Products.Remove(product);
                    dbContext.SaveChanges();
                }
                notificationManager.Show("Success", "product Deleted successfully", NotificationType.Success, areaName: "notificationArea");

            }
            PopulateDataGrid();
        }

        private void btnAddToCart_Click(object sender, RoutedEventArgs e)
        {

            if (cbCurrentCustomer.SelectedItem != null)
            {
                Product selectedProduct = (Product)productDataGrid.SelectedItem;

                if (selectedProduct != null)
                {
                    var quantityWindow = new SetQuantityWindow();
                    quantityWindow.ShowDialog();

                    var quantity = quantityWindow.GetQuantity();
                    if (quantity <= 0)
                    {
                        return;
                    }
                    if (quantity > selectedProduct.Quantity)
                    {
                        notificationManager.Show("Error", "No sufficient quantity in store", NotificationType.Error, "notificationArea");
                        return;
                    }

                    var orderDetail = new OrderDetail
                    {
                        ProductId = selectedProduct.Id,
                        Product = selectedProduct,
                        Quantity = quantity,
                        SubTotal = (double)(selectedProduct.Price > 0 ?
                                    (quantity * (selectedProduct.Price)) :
                                    (quantity * selectedProduct.USDPriceForCustomer * Configuration.CurrencyRate)),
                        Price = (double)(selectedProduct.Price > 0 ?
                                    (selectedProduct.Price) :
                                    (selectedProduct.USDPriceForCustomer * Configuration.CurrencyRate))
                    };

                    currentOrder.OrderDetails.Add(orderDetail);
                    CalculateOrderTotals();
                    notificationManager.Show("Success", "Item added to cart successfully", NotificationType.Success, "notificationArea");
                }
            }
            else
            {
                MessageBox.Show("Mijozni tanlang!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

        }
        private void CalculateOrderTotals()
        {
            currentOrder.TotalAmount = currentOrder.OrderDetails.Sum(od => od.SubTotal);
        }



        private void btnViewCart_Click(object sender, RoutedEventArgs e)
        {
            var currentOrderDetails = new CurrentOrderDetails(currentOrder, (Customer)cbCurrentCustomer.SelectedItem);
            currentOrderDetails.OrderSavedButtonClicked += CurrentOrderDetails_OrderSaved;
            currentOrderDetails.ShowDialog();

        }



        private void CurrentOrderDetails_OrderSaved(object sender, EventArgs e)
        {
            InitializeNewOrder();
            IncrementOrdersCountOfCustomer(currentOrder.CustomerId);
            PopulateDataGrid();
        }



        private void IncrementOrdersCountOfCustomer(int customerId)
        {
            using (var dbContext = new AppDbContext())
            {
                selectedCustomer.TotalOrdersCount++;
                dbContext.Customers.Update(selectedCustomer);
                dbContext.SaveChanges();
            }
        }

        private void cbCurrentCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedCustomer = (Customer)cbCurrentCustomer.SelectedItem;
            currentOrder.Customer = selectedCustomer;
            currentOrder.CustomerId = selectedCustomer.Id;
        }

        //  ************************************************************************************************************
        private void productDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var editedProduct = e.Row.Item as Product;

            if (editedProduct != null)
            {
                var editedValue = (e.EditingElement as TextBox).Text;
                if (!string.IsNullOrWhiteSpace(editedValue))
                {
                    if (int.TryParse(editedValue, out var quantity))
                    {

                        editedProduct.Quantity = quantity;
                        using (var dbContext = new AppDbContext())
                        {
                            dbContext.Products.Update(editedProduct);
                            dbContext.SaveChanges();

                        }
                        notificationManager.Show("Success", "Quantity changed successfully", NotificationType.Success);

                    }
                    else
                    {
                        notificationManager.Show("Error", "Insert number pls", NotificationType.Error);
                        (e.EditingElement as TextBox).Text = editedProduct.Quantity.ToString();
                    }


                }
                else
                {
                    notificationManager.Show("Error", "quantity be null", NotificationType.Error);
                    (e.EditingElement as TextBox).Text = editedProduct.Quantity.ToString();


                }
            }

        }

        private void scrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}

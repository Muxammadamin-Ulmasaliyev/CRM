using InventoryManagementSystem.AppConfiguration;
using InventoryManagementSystem.Model;
using InventoryManagementSystem.Services;
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
        private readonly ProductService _productService;
        private readonly CustomerService _customerService;
        public HomePage()
        {
            _productService = new(new AppDbContext());
            _customerService = new(new AppDbContext());
            InitializeComponent();
            InitializeNewOrder();
            PopulateDataGrid();
            PopulateDataGridComboboxes();
           
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



            //_productService.AddProducts(AppConfiguration.Configuration.GenerateFakeProducts(100));

            products = new ObservableCollection<Product>(_productService.GetAll());

            productDataGrid.ItemsSource = products;

            txtNumberOfProducts.Text = $"Jadvaldagi produktlar soni : {productDataGrid.Items.Count}";

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

                _productService.Delete(product);

                notificationManager.Show("Success", "product Deleted successfully", NotificationType.Success, areaName: "notificationArea");

            }
            PopulateDataGrid();
        }

        private void btnAddToCart_Click(object sender, RoutedEventArgs e)
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
        private void CalculateOrderTotals()
        {
            currentOrder.TotalAmount = currentOrder.OrderDetails.Sum(od => od.SubTotal);
        }


       



        private void btnViewCart_Click(object sender, RoutedEventArgs e)
        {


            var cartPage = new CartPage(currentOrder);
            NavigationService?.Navigate(cartPage);
            cartPage.OrderSavedButtonClicked += CurrentOrderDetails_OrderSaved;


        }



        private void CurrentOrderDetails_OrderSaved(object sender, EventArgs e)
        {
            InitializeNewOrder();
           
            PopulateDataGrid();
        }



        

       

        private void productDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var editedProduct = e.Row.Item as Product;

            string propertyName = e.Column.SortMemberPath;

            var editedValue = (e.EditingElement as TextBox).Text;

            switch (propertyName)
            {
                case "Name":
                    editedProduct.Name = editedValue.ToString();
                    break;
                case "Code":
                    editedProduct.Code = editedValue.ToString();

                    break;
                case "Quantity":
                    if (int.TryParse(editedValue.ToString(), out int quantity))
                    {
                        editedProduct.Quantity = quantity;
                    }
                    else
                    {

                        notificationManager.Show("Error", "Insert number pls", NotificationType.Error);
                        (e.EditingElement as TextBox).Text = editedProduct.Quantity.ToString();
                        return;
                    }
                    break;
                case "RealPrice":
                    if (editedProduct.RealPrice == null || editedProduct.Price == null)
                    {
                        notificationManager.Show("Error", "Bu mahsulot $ sotiladi.", NotificationType.Error);
                        (e.EditingElement as TextBox).Text = null;
                        return;

                    }
                    if (double.TryParse(editedValue.ToString(), out double realPrice))
                    {
                        editedProduct.RealPrice = realPrice;
                    }
                    else
                    {
                        notificationManager.Show("Error", "Insert number pls", NotificationType.Error);
                        (e.EditingElement as TextBox).Text = editedProduct.RealPrice.ToString();
                        return;
                    }
                    break;
                case "Price":
                    if (editedProduct.RealPrice == null || editedProduct.Price == null)
                    {
                        notificationManager.Show("Error", "Bu mahsulot $ sotiladi.", NotificationType.Error);
                        (e.EditingElement as TextBox).Text = null;
                        return;

                    }
                    if (double.TryParse(editedValue.ToString(), out double price))
                    {
                        editedProduct.Price = price;
                    }
                    else
                    {
                        notificationManager.Show("Error", "Insert number pls", NotificationType.Error);
                        (e.EditingElement as TextBox).Text = editedProduct.Price.ToString();
                        return;


                    }
                    break;
                case "USDPrice":
                    if (editedProduct.USDPrice == null || editedProduct.USDPriceForCustomer == null)
                    {
                        notificationManager.Show("Error", "Bu mahsulot so`mda sotiladi.", NotificationType.Error);
                        (e.EditingElement as TextBox).Text = null;
                        return;

                    }
                    if (double.TryParse(editedValue.ToString(), out double usdPrice))
                    {
                        editedProduct.USDPrice = usdPrice;
                    }
                    else
                    {
                        notificationManager.Show("Error", "Insert number pls", NotificationType.Error);
                        (e.EditingElement as TextBox).Text = editedProduct.USDPrice.ToString();
                        return;

                    }
                    break;
                case "USDPriceForCustomer":
                    if (editedProduct.USDPrice == null || editedProduct.USDPriceForCustomer == null)
                    {
                        notificationManager.Show("Error", "Bu mahsulot so`mda sotiladi.", NotificationType.Error);
                        (e.EditingElement as TextBox).Text = null;
                        return;

                    }
                    if (double.TryParse(editedValue.ToString(), out double usdPriceForCustomer))
                    {
                        editedProduct.USDPriceForCustomer = usdPriceForCustomer;
                    }
                    else
                    {
                        notificationManager.Show("Error", "Insert number pls", NotificationType.Error);
                        (e.EditingElement as TextBox).Text = editedProduct.USDPriceForCustomer.ToString();
                        return;

                    }
                    break;

                default:
                    break;
            }

            _productService.Update(editedProduct);
            notificationManager.Show("Success", "Quantity changed successfully", NotificationType.Success);



        }

        private void scrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }
}

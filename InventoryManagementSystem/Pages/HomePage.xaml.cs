using InventoryManagementSystem.Model;
using InventoryManagementSystem.Services;
using InventoryManagementSystem.View;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Notification.Wpf;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace InventoryManagementSystem.Pages
{
    public partial class HomePage : Page
    {
        private NotificationManager notificationManager;
        private ObservableCollection<Product> products;
        private ObservableCollection<Product> filteredProducts;
        private Order? currentOrder;
        private Customer selectedCustomer;
        private ProductService _productService;
        private readonly CustomerService _customerService;
        private readonly OrderDetailService _orderDetailService;
        private int _pageSize = Properties.Settings.Default.ProductsPerPage;
        private int _currentPage = 1;

        private System.Timers.Timer debounceTimer;

        public HomePage()
        {
            InitializeComponent();
            SetupUserCustomizationsSettings();
            SetupTimerSettings();
            _productService = new(new AppDbContext());
            _customerService = new(new AppDbContext());
            _orderDetailService = new(new AppDbContext());
            InitializeNewOrder();
            PopulateDataGrid();
            PopulateDataGridComboboxes();
            PopulateCurrencyRate();
            LoadPage();
            PopulateNumberOfProductsTxt();
            notificationManager = new();
            txtNumberOfProductsInDb.Text = $"Bazada mavjud jami produktlar soni : {products.Count}";
            KeyDown += btnSaveCurrencyRate_KeyDown;

        }

        private void SetupUserCustomizationsSettings()
        {
            productDataGrid.FontSize = Properties.Settings.Default.ProductsDataGridFontSize;
        }

        private void SetupTimerSettings()
        {
            debounceTimer = new();
            debounceTimer.Interval = 1000;
            debounceTimer.AutoReset = false;
            debounceTimer.Elapsed += DebounceTimer_Elapsed;
        }
        private void DebounceTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Dispatcher.Invoke(() =>
                {
                    string searchText = searchBar.Text;
                    Search();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CheckPaginationButtonStates()
        {
            if (_currentPage == 1)
            {
                btnPrevious.IsEnabled = false;
            }
            else
            {
                btnPrevious.IsEnabled = true;
            }
            if (!IsFilteringDisabled())
            {
                if (_currentPage == Math.Ceiling((decimal)filteredProducts.Count / _pageSize))
                {
                    btnNext.IsEnabled = false;
                }
                else
                {
                    btnNext.IsEnabled = true;
                }
            }
            else
            {

                if (_currentPage == Math.Ceiling((decimal)products.Count / _pageSize))
                {
                    btnNext.IsEnabled = false;
                }
                else
                {
                    btnNext.IsEnabled = true;
                }
            }
        }
        private void LoadPage()
        {
            if (!IsFilteringDisabled())
            {
                int startIndex = (_currentPage - 1) * _pageSize;
                int endIndex = Math.Min(startIndex + _pageSize - 1, filteredProducts.Count - 1);

                var currentPageData = new ObservableCollection<Product>();

                for (int i = startIndex; i <= endIndex; i++)
                {
                    currentPageData.Add(filteredProducts[i]);
                }

                productDataGrid.ItemsSource = currentPageData;

                currentPageText.Text = $"Сахифа {_currentPage} / {Math.Ceiling((decimal)filteredProducts.Count / _pageSize)}";
                PopulateNumberOfProductsTxt();

                CheckPaginationButtonStates();
            }
            else
            {
                int startIndex = (_currentPage - 1) * _pageSize;
                int endIndex = Math.Min(startIndex + _pageSize - 1, products.Count - 1);

                var currentPageData = new ObservableCollection<Product>();

                for (int i = startIndex; i <= endIndex; i++)
                {
                    currentPageData.Add(products[i]);
                }

                productDataGrid.ItemsSource = currentPageData;

                currentPageText.Text = $"Сахифа {_currentPage} / {Math.Ceiling((decimal)products.Count / _pageSize)}";
                PopulateNumberOfProductsTxt();

                CheckPaginationButtonStates();
            }
        }
        private void PreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                LoadPage();
                CheckPaginationButtonStates();
            }
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            int totalPages;
            if (IsFilteringDisabled())
            {
                totalPages = (int)Math.Ceiling((double)products.Count / _pageSize);
            }
            else
            {
                totalPages = (int)Math.Ceiling((double)filteredProducts.Count / _pageSize);
            }
            if (_currentPage < totalPages)
            {
                _currentPage++;
                LoadPage();
                CheckPaginationButtonStates();

            }


        }


        private void PopulateCurrencyRate()
        {
            txtCurrencyRate.Text = $"Курс : 1 $ = {Properties.Settings.Default.CurrencyRate} сум";
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

        private void PopulateNumberOfProductsTxt()
        {
            txtNumberOfProducts.Text = $"Жадвалдаги продуктлар сони : {productDataGrid.Items.Count}";
        }

        private void PopulateDataGrid()
        {

            products = new ObservableCollection<Product>(_productService.GetAll());

            PopulateNumberOfProductsTxt();
        }

        private bool IsFilteringDisabled()
        {
            return
                string.IsNullOrWhiteSpace(searchBar.Text) &&
                cbCarType.SelectedItem == null &&
                cbCompany.SelectedItem == null &&
                cbCountry.SelectedItem == null &&
                cbSetType.SelectedItem == null;
        }
        private void Search()
        {
            _currentPage = 1;
            if (IsFilteringDisabled())
            {
                LoadPage();
                txtNumberOfProductsInDb.Text = $"Bazada mavjud jami produktlar soni : {products.Count}";

            }
            else
            {
                productDataGrid.ItemsSource = products;
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(productDataGrid.ItemsSource);
                view.Filter = SearchFilter;
                filteredProducts = new ObservableCollection<Product>(view.Cast<Product>().ToList());
                LoadPage();
                txtNumberOfProductsInDb.Text = $"Bazada mavjud jami produktlar soni : {filteredProducts.Count}";
            }


        }

        

        /******************************** OPTIMIZE ******************************************************************************/
        private bool SearchFilter(object item)
        {
            bool isMatching = true;
            if (IsFilteringDisabled())
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
        /******************************** OPTIMIZE ******************************************************************************/

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
            LoadPage();

        }

        private void searchBarTextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Stop the timer (if it's running)
            debounceTimer.Stop();

            // Start the timer again
            debounceTimer.Start();
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
            ResetFilters();
            LoadPage();
            txtNumberOfProductsInDb.Text = $"Bazada mavjud jami produktlar soni : {products.Count}";

        }

        private void ResetFilters()
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


        private void btnDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            Product product = (Product)productDataGrid.SelectedItem;
            var choice = MessageBox.Show($"Are you sure to delete product : {product.Name} ", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
            if (choice == MessageBoxResult.Yes)
            {
                if (_orderDetailService.ProductExists(product.Id))
                {
                    MessageBox.Show($"{product.Name} bu produkt avval sotilganligi uchun, o`chirb tashlash mumkin emas", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _productService.Delete(product);

                notificationManager.Show("Success", "product Deleted successfully", NotificationType.Success, areaName: "notificationArea");

                PopulateDataGrid();
                LoadPage();
            }
        }

        private void btnAddToCart_Click(object sender, RoutedEventArgs e)
        {

            Product selectedProduct = (Product)productDataGrid.SelectedItem;

            if (selectedProduct != null)
            {
                var quantityWindow = new SetQuantityWindow();
                this.Opacity = 0.4;
                quantityWindow.ShowDialog();
                this.Opacity = 1;

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
                                (quantity * selectedProduct.USDPriceForCustomer * Properties.Settings.Default.CurrencyRate)),
                    Price = (double)(selectedProduct.Price > 0 ?
                                (selectedProduct.Price) :
                                (selectedProduct.USDPriceForCustomer * Properties.Settings.Default.CurrencyRate)),
                    ProductName = selectedProduct.Name,
                    ProductCarType = selectedProduct.CarType.Name,
                    ProductCompany = selectedProduct.Company.Name,
                    ProductCountry = selectedProduct.Country.Name,
                    ProductSetType = selectedProduct.SetType.Name,

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
            _productService = new(new AppDbContext());
            InitializeNewOrder();

            PopulateDataGrid();
            LoadPage();

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
                    if (int.TryParse(StringHelper.TrimAllWhiteSpaces(editedValue), out int quantity))
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
                    if (double.TryParse(StringHelper.TrimAllWhiteSpaces(editedValue), out double realPrice))
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
                    if (double.TryParse(StringHelper.TrimAllWhiteSpaces(editedValue), out double price))
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
                    if (double.TryParse(StringHelper.TrimAllWhiteSpaces(editedValue), out double usdPrice))
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
                    if (double.TryParse(StringHelper.TrimAllWhiteSpaces(editedValue), out double usdPriceForCustomer))
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
            notificationManager.Show("Success", $"{propertyName} changed successfully", NotificationType.Success);



        }

        private void scrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void btnEditCurrency_Click(object sender, RoutedEventArgs e)
        {
            spCurrency.Visibility = Visibility.Visible;
            txtCurrencyRate.Visibility = Visibility.Collapsed;
            btnEditCurrency.IsEnabled = false;
        }


       

        private void productDataGrid_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            if (e.Column.Header.Equals("$ tannarx") || e.Column.Header.Equals("$ sotish"))
            {
                // Remove the "$" sign temporarily for editing
                if (e.EditingElement is TextBox textBox)
                {
                    // Remove the "$" sign for editing
                    string textWithoutDollar = StringHelper.RemoveDollarSignFromPrice(textBox.Text); 
                    textBox.Text = textWithoutDollar;

                    // Set the cursor at the end of the text
                    textBox.CaretIndex = textWithoutDollar.Length;
                }
                return;
            }
            if (e.Column.Header.Equals("Tannarx") || e.Column.Header.Equals("Sotish narx"))
            {
                if (e.EditingElement is TextBox textBox)
                {
                    // Remove the "$" sign for editing
                   
                    string textWithoutSum = StringHelper.RemoveSumSignFromPrice(textBox.Text);
                    textBox.Text = textWithoutSum;

                    // Set the cursor at the end of the text
                    textBox.CaretIndex = textWithoutSum.Length;
                }
                return;
            }

        }

        private void btnSaveCurrencyRate_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrWhiteSpace(tbCurrencyRate.Text))
            {
                notificationManager.Show("Error", "son kiriting", NotificationType.Error);

                return;
            }
            if(!double.TryParse(StringHelper.TrimAllWhiteSpaces(tbCurrencyRate.Text), out double currencyRate))
            {
                notificationManager.Show("Error", "son kiriting", NotificationType.Error);
                return;
            }
            else
            {
                Properties.Settings.Default.CurrencyRate = currencyRate;
                Properties.Settings.Default.Save();

            }
            PopulateCurrencyRate();
            spCurrency.Visibility = Visibility.Collapsed;
            txtCurrencyRate.Visibility = Visibility.Visible;
            btnEditCurrency.IsEnabled = true;
            notificationManager.Show("Success", " changed successfully", NotificationType.Success);


        }

        private void btnSaveCurrencyRate_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (tbCurrencyRate.IsFocused)
                {
                    btnSaveCurrencyRate_Click((object)sender, e);
                }
            }
        }
    }
}

using InventoryManagementSystem.Model;
using InventoryManagementSystem.Services;
using InventoryManagementSystem.View;
using MethodTimer;
using Notification.Wpf;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;
using TextBox = System.Windows.Controls.TextBox;

namespace InventoryManagementSystem.Pages
{
    public partial class HomePage : Page
    {
        private NotificationManager notificationManager;
        private ObservableCollection<Product> filteredProducts;
        private Order? currentOrder;
        private ProductService _productService;
        private readonly OrderDetailService _orderDetailService;


        private int _productsCountInDb;
        private int _pageSize = Properties.Settings.Default.ProductsPerPage;
        private int _currentPage = 1;

        private System.Timers.Timer debounceTimer;

        public HomePage()
        {
            // Shared.Shared.SeedFakeProducts();
            InitializeComponent();
            SetupUserCustomizationsSettings();
            SetupTimerSettings();
            _productService = new(new AppDbContext());
            _orderDetailService = new(new AppDbContext());
            _productsCountInDb = _productService.GetProductsCount();
            InitializeNewOrder();
            PopulateDataGridComboboxes();
            PopulateCurrencyRate();
            LoadPage();
            PopulateNumberOfProductsTxt();
            CheckPaginationButtonStates();
            notificationManager = new();
            txtNumberOfProductsInDb.Text = $"Базада мавжуд жами продуктлар сони : {_productsCountInDb}";
            KeyDown += btnSaveCurrencyRate_KeyDown;

            checkBoxShowRealPrices.IsChecked = true;

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

        private void SetupUserCustomizationsSettings()
        {
            productDataGrid.FontSize = Properties.Settings.Default.ProductsDataGridFontSize;
            this.FontFamily = new System.Windows.Media.FontFamily(Properties.Settings.Default.AppFontFamily);
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
            btnPrevious.IsEnabled = _currentPage > 1;

            btnNext.IsEnabled = IsFilteringDisabled()
                ? _currentPage < Math.Ceiling((decimal)_productsCountInDb / _pageSize)
                : _currentPage < Math.Ceiling((decimal)filteredProducts.Count / _pageSize);

        }

        [Time]
        private void LoadPage()
        {
            if (IsFilteringDisabled())
            {
                int startIndex = (_currentPage - 1) * _pageSize;
                var currentPageData = new ObservableCollection<Product>(_productService.GetPageOfProducts(startIndex, _pageSize));
                productDataGrid.ItemsSource = currentPageData;
                currentPageText.Text = $"Сахифа {_currentPage} / {Math.Ceiling((decimal)_productsCountInDb / _pageSize)}";

            }
            else
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
            }
            PopulateNumberOfProductsTxt();
            CheckPaginationButtonStates();
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

            totalPages = IsFilteringDisabled()
                             ? (int)Math.Ceiling((double)_productsCountInDb / _pageSize)
                             : (int)Math.Ceiling((double)filteredProducts.Count / _pageSize);

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

            IsCartEmpty();

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


        /******************************** OPTIMIZE ******************************************************************************/

        private bool IsFilteringDisabled()
        {
            return
                string.IsNullOrWhiteSpace(searchBar.Text) &&
                cbCarType.SelectedItem == null &&
                cbCompany.SelectedItem == null &&
                cbCountry.SelectedItem == null &&
                cbSetType.SelectedItem == null;
        }

        [Time]
        private void Search()
        {
            _currentPage = 1;
            if (IsFilteringDisabled())
            {
                LoadPage();
                txtNumberOfProductsInDb.Text = $"Базада мавжуд жами продуктлар сони : {_productsCountInDb}";
            }
            else
            {
                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(_productService.GetAll());
                view.Filter = SearchFilter;
                filteredProducts = new ObservableCollection<Product>(view.Cast<Product>().ToList());
                LoadPage();
                txtNumberOfProductsInDb.Text = $"Базада мавжуд жами продуктлар сони : {filteredProducts.Count}";
            }
        }
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
            addProductWindow.ShowDialog();
        }
        private void AddProductWindow_AddProductButtonClicked(object? sender, EventArgs e)
        {
            LoadPage();
            _productsCountInDb++;

            if (IsFilteringDisabled())
            {

                txtNumberOfProductsInDb.Text = $"Базада мавжуд жами продуктлар сони : {_productsCountInDb}";
                currentPageText.Text = $"Сахифа {_currentPage} / {Math.Ceiling((decimal)_productsCountInDb / _pageSize)}";
            }
            else
            {
                currentPageText.Text = $"Сахифа {_currentPage} / {Math.Ceiling((decimal)filteredProducts.Count / _pageSize)}";
            }
            CheckPaginationButtonStates();


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

        #region Categories
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
        private void btnAddSetType_Click(object sender, RoutedEventArgs e)
        {
            var addSetTypeWindow = new AddSetTypeWindow();
            addSetTypeWindow.AddSetTypeButtonClicked += Window_AddSomeCategoryButtonClicked;
            addSetTypeWindow.Show();
        }
        private void btnAddCountry_Click(object sender, RoutedEventArgs e)
        {
            var addCountryWindow = new AddCountryWindow();
            addCountryWindow.AddCountryButtonClicked += Window_AddSomeCategoryButtonClicked;
            addCountryWindow.Show();
        }
        #endregion

        private void btnResetFilter_Click(object sender, RoutedEventArgs e)
        {
            ResetFilters();
            LoadPage();
            txtNumberOfProductsInDb.Text = $"Базада мавжуд жами продуктлар сони : {_productsCountInDb}";

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
            var choice = MessageBox.Show($"{product.Name} : продуктни учириб ташламокчимисиз ? ", "Огохлантириш !", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
            if (choice == MessageBoxResult.Yes)
            {
                if (_orderDetailService.IsProductSoldAtLeastOneTime(product.Id))
                {
                    MessageBox.Show($"{product.Name} продукт аввл сотилганлиги учун, учириб ташлаш мумкин емас !", "Хатолик !", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                _productService.Delete(product);
                _productsCountInDb--;

                if (IsFilteringDisabled())
                {
                    txtNumberOfProductsInDb.Text = $"Базада мавжуд жами продуктлар сони : {_productsCountInDb}";

                }
                else
                {
                    txtNumberOfProductsInDb.Text = $"Базада мавжуд жами продуктлар сони : {filteredProducts.Count}";

                }

                notificationManager.Show("Муваффакият !", "Продукт учириб ташланди !", NotificationType.Success, areaName: "notificationArea");

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
                    notificationManager.Show("Хатолик !", $"Бу товардан {quantity} та дан кам колган, кичикрок сон киритинг !", NotificationType.Error, "notificationArea");
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
                    RealPrice = (double)(selectedProduct.RealPrice > 0 ?
                                (selectedProduct.RealPrice) :
                                (selectedProduct.USDPrice * Properties.Settings.Default.CurrencyRate)),
                    ProductName = selectedProduct.Name,
                    ProductCarType = selectedProduct.CarType.Name,
                    ProductCompany = selectedProduct.Company.Name,
                    ProductCountry = selectedProduct.Country.Name,
                    ProductSetType = selectedProduct.SetType.Name,

                };

                currentOrder.OrderDetails.Add(orderDetail);
                CalculateOrderTotals();
                notificationManager.Show("Муваффакият !", "Товар корзинага кушилди !", NotificationType.Success, "notificationArea");
                IsCartEmpty();

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

            LoadPage();
            IsCartEmpty();


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
                case "Barcode":
                    editedProduct.Barcode = editedValue.ToString();
                    break;
                case "Quantity":
                    if (int.TryParse(StringHelper.TrimAllWhiteSpaces(editedValue), out int quantity))
                    {
                        editedProduct.Quantity = quantity;
                    }
                    else
                    {

                        notificationManager.Show("Хатолик !", "Сон киритинг !", NotificationType.Error);
                        (e.EditingElement as TextBox).Text = editedProduct.Quantity.ToString();
                        return;
                    }
                    break;
                case "RealPrice":
                    if (editedProduct.RealPrice == null || editedProduct.Price == null)
                    {
                        notificationManager.Show("Хатолик !", "Бу махсулот $ да", NotificationType.Error);
                        (e.EditingElement as TextBox).Text = null;
                        return;

                    }
                    if (double.TryParse(StringHelper.TrimAllWhiteSpaces(editedValue), out double realPrice))
                    {
                        editedProduct.RealPrice = realPrice;
                    }
                    else
                    {
                        notificationManager.Show("Хатолик !", "Сон киритинг !", NotificationType.Error);
                        (e.EditingElement as TextBox).Text = editedProduct.RealPrice.ToString();
                        return;
                    }
                    break;
                case "Price":
                    if (editedProduct.RealPrice == null || editedProduct.Price == null)
                    {
                        notificationManager.Show("Error", "Бу махсулот $ да", NotificationType.Error);
                        (e.EditingElement as TextBox).Text = null;
                        return;

                    }
                    if (double.TryParse(StringHelper.TrimAllWhiteSpaces(editedValue), out double price))
                    {
                        editedProduct.Price = price;
                    }
                    else
                    {
                        notificationManager.Show("Хатолик !", "Сон киритинг !", NotificationType.Error);
                        (e.EditingElement as TextBox).Text = editedProduct.Price.ToString();
                        return;


                    }
                    break;
                case "USDPrice":
                    if (editedProduct.USDPrice == null || editedProduct.USDPriceForCustomer == null)
                    {
                        notificationManager.Show("Error", "Бу махсулот сумда", NotificationType.Error);
                        (e.EditingElement as TextBox).Text = null;
                        return;

                    }
                    if (double.TryParse(StringHelper.TrimAllWhiteSpaces(editedValue), out double usdPrice))
                    {
                        editedProduct.USDPrice = usdPrice;
                    }
                    else
                    {
                        notificationManager.Show("Хатолик !", "Сон киритинг !", NotificationType.Error);
                        (e.EditingElement as TextBox).Text = editedProduct.USDPrice.ToString();
                        return;

                    }
                    break;
                case "USDPriceForCustomer":
                    if (editedProduct.USDPrice == null || editedProduct.USDPriceForCustomer == null)
                    {
                        notificationManager.Show("Error", "Бу махсулот сумда", NotificationType.Error);
                        (e.EditingElement as TextBox).Text = null;
                        return;

                    }
                    if (double.TryParse(StringHelper.TrimAllWhiteSpaces(editedValue), out double usdPriceForCustomer))
                    {
                        editedProduct.USDPriceForCustomer = usdPriceForCustomer;
                    }
                    else
                    {
                        notificationManager.Show("Хатолик !", "Сон киритинг !", NotificationType.Error);
                        (e.EditingElement as TextBox).Text = editedProduct.USDPriceForCustomer.ToString();
                        return;
                    }
                    break;

                default:
                    break;
            }

            _productService.Update(editedProduct);
            notificationManager.Show("Муваффакият !", $"Товар янгиланди!", NotificationType.Success);



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
            tbCurrencyRate.Focus();

            btnEditCurrency.IsEnabled = false;
        }




        private void productDataGrid_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            if (e.Column.Header.Equals("$ таннарх") || e.Column.Header.Equals("$ сотиш"))
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
            if (e.Column.Header.Equals("Таннарх") || e.Column.Header.Equals("Сотиш нарх"))
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
            if (string.IsNullOrWhiteSpace(tbCurrencyRate.Text))
            {
                notificationManager.Show("Хатолик !", "Сон киритинг !", NotificationType.Error);

                return;
            }
            if (!double.TryParse(StringHelper.TrimAllWhiteSpaces(tbCurrencyRate.Text), out double currencyRate))
            {
                notificationManager.Show("Хатолик !", "Сон киритинг !", NotificationType.Error);
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

            notificationManager.Show("Муваффакият !", "Курс янгиланди", NotificationType.Success);

            ChangeOrderDetailsPricesAccordingToCurrency();



        }

        private void ChangeOrderDetailsPricesAccordingToCurrency()
        {
            foreach (var orderDetail in currentOrder.OrderDetails)
            {
                orderDetail.Price = (double)(orderDetail.Product.USDPriceForCustomer * Properties.Settings.Default.CurrencyRate);
                orderDetail.SubTotal = orderDetail.Price * orderDetail.Quantity;
            }

            CalculateOrderTotals();
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

        private void currentPageNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (tbcurrentPageNumber.IsFocused)
            {
                if (e.Key == Key.Enter)
                {
                    if (int.TryParse(tbcurrentPageNumber.Text, out var pageNumber))
                    {
                        if (IsFilteringDisabled())
                        {

                            if (pageNumber > 0 && pageNumber <= Math.Ceiling((decimal)_productsCountInDb / _pageSize) + 1)
                            {
                                _currentPage = pageNumber;
                                LoadPage();
                            }
                            else
                            {
                                notificationManager.Show("Хатолик !", "Бу сахифа мавжуд эмас", NotificationType.Error);

                            }
                            tbcurrentPageNumber.Text = string.Empty;


                        }
                        else
                        {
                            if (pageNumber > 0 && pageNumber <= Math.Ceiling((decimal)filteredProducts.Count / _pageSize))
                            {
                                _currentPage = pageNumber;
                                LoadPage();
                            }
                            else
                            {
                                notificationManager.Show("Хатолик !", "Бу сахифа мавжуд эмас", NotificationType.Error);

                            }
                            tbcurrentPageNumber.Text = string.Empty;


                        }

                    }

                }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ToggleColumnVisibility(true);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ToggleColumnVisibility(false);
        }
        private void ToggleColumnVisibility(bool isVisible)
        {
            var column = productDataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "Таннарх");
            var column2 = productDataGrid.Columns.FirstOrDefault(c => c.Header.ToString() == "$ таннарх");

            if (column != null)
            {
                column.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            }

            if (column2 != null)
            {
                column2.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}

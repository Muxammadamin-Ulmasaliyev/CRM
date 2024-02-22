using InventoryManagementSystem.Model;
using InventoryManagementSystem.Services;
using InventoryManagementSystem.UserControls;
using Notification.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace InventoryManagementSystem.View
{
    public partial class AddProductWindow : Window
    {
        private NotificationManager notificationManager;
        public event EventHandler AddProductButtonClicked;
        private readonly ProductService _productService;

        public AddProductWindow()
        {
            notificationManager = new();
            _productService = new(new AppDbContext());
            InitializeComponent();
            rbUzs.IsChecked = true;
            PopulateComboboxes();
        }
        private void PopulateComboboxes()
        {
            using (var dbContext = new AppDbContext())
            {
                cbCarType.ItemsSource = dbContext.CarTypes.ToList();
                cbCompany.ItemsSource = dbContext.Companies.ToList();
                cbCountry.ItemsSource = dbContext.Countries.ToList();
                cbSetType.ItemsSource = dbContext.SetTypes.ToList();
            }
        }

        protected virtual void ProductAdded()
        {
            AddProductButtonClicked?.Invoke(this, EventArgs.Empty);
        }


        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {

            if (IsValidTextBoxesAndComboBoxes() && IsValidNumericInputFields())
            {
                var productToAdd = new Product()
                {
                    Name = tbName.txtInput.Text,
                    Code = tbCode.txtInput.Text,
                    RealPrice = double.TryParse(tbRealPrice.txtInput.Text, out var realPrice) ? realPrice : null,
                    Price = double.TryParse(tbPrice.txtInput.Text, out var price) ? price : null,
                    USDPrice = double.TryParse(tbUsdPrice.txtInput.Text, out var UsdPrice) ? UsdPrice : null,
                    USDPriceForCustomer = double.TryParse(tbUsdPrice2.txtInput.Text, out var UsdPriceForCustomer) ? UsdPriceForCustomer : null,
                    Quantity = int.Parse(tbQuantity.txtInput.Text),
                    CountryId = (cbCountry.SelectedItem as Country).Id,
                    CarTypeId = (cbCarType.SelectedItem as CarType).Id,
                    CompanyId = (cbCompany.SelectedItem as Company).Id,
                    SetTypeId = (cbSetType.SelectedItem as SetType).Id
                };



                if (_productService.IsProductCodeExists(productToAdd.Code, out var productFromDb))
                {
                    productFromDb.Quantity += productToAdd.Quantity;
                    _productService.UpdateQuantity(productFromDb);

                    notificationManager.Show("info", "Product already exists, quantity incremented", NotificationType.Information);
                }
                else
                {
                    _productService.AddProduct(productToAdd);
                    notificationManager.Show("Success", "Product added successfully", NotificationType.Success);
                }

                ClearTextBoxesAndComboBoxes();
                ProductAdded();
            }
        }

        private void DisplayErrorTextBox(string errorMessage, ClearableTextBox textBox, TextBlock textBlock, Thickness thickness)
        {
            textBox.txtInput.BorderThickness = thickness;
            textBox.txtInput.BorderBrush = Brushes.Red;
            textBlock.Text = errorMessage;
        }

        private void DisplayErrorComboBox(string errorMessage, TextBlock textBlock)
        {
            textBlock.Text = errorMessage;

        }
        private void DisplaySuccessComboBox(TextBlock textBlock)
        {
            textBlock.Text = string.Empty;

        }

        private void DisplaySuccessTextBox(ClearableTextBox textBox, TextBlock textBlock, Thickness thickness)
        {
            textBox.txtInput.BorderThickness = thickness;
            textBox.txtInput.BorderBrush = Brushes.Green;
            textBlock.Text = string.Empty;

        }
        private bool IsValidTextBoxesAndComboBoxes()
        {
            bool result = true;
            var thickness = new Thickness(2);

            if (string.IsNullOrWhiteSpace(tbName.txtInput.Text))
            {
                DisplayErrorTextBox(errorMessage: "* Mahsulot nomini kiriting! *", tbName, txtErrorName, thickness);
                result = result && false;
            }
            else
            {
                DisplaySuccessTextBox(tbName, txtErrorName, thickness);
                result = result && true;
            }

            if (rbUzs.IsChecked == true)
            {


                if (string.IsNullOrWhiteSpace(tbRealPrice.txtInput.Text))
                {
                    DisplayErrorTextBox(errorMessage: "* Mahsulot tannarxini kiriting! *", tbRealPrice, txtErrorRealPrice, thickness);
                    result = result && false;
                }
                else
                {
                    DisplaySuccessTextBox(tbRealPrice, txtErrorRealPrice, thickness);
                    result = result && true;
                }


                if (string.IsNullOrWhiteSpace(tbPrice.txtInput.Text))
                {
                    DisplayErrorTextBox(errorMessage: "* Mahsulot narxini kiriting! *", tbPrice, txtErrorPrice, thickness);
                    result = result && false;
                }
                else
                {
                    DisplaySuccessTextBox(tbPrice, txtErrorPrice, thickness);
                    result = result && true;
                }
            }
            if (rbUsd.IsChecked == true)
            {

                if (string.IsNullOrWhiteSpace(tbUsdPrice.txtInput.Text))
                {
                    DisplayErrorTextBox(errorMessage: "* Mahsulot $ tannarxini kiriting! *", tbUsdPrice, txtErrorUsdPrice, thickness);
                    result = result && false;
                }
                else
                {
                    DisplaySuccessTextBox(tbUsdPrice, txtErrorUsdPrice, thickness);
                    result = result && true;
                }

                if (string.IsNullOrWhiteSpace(tbUsdPrice2.txtInput.Text))
                {
                    DisplayErrorTextBox(errorMessage: "* Mahsulot $ narxini kiriting! *", tbUsdPrice2, txtErrorUsdPrice2, thickness);
                    result = result && false;
                }
                else
                {
                    DisplaySuccessTextBox(tbUsdPrice2, txtErrorUsdPrice2, thickness);
                    result = result && true;
                }
            }

            if (string.IsNullOrWhiteSpace(tbQuantity.txtInput.Text))
            {
                DisplayErrorTextBox(errorMessage: "* Miqdorni kiriting! *", tbQuantity, txtErrorQuantity, thickness);
                result = result && false;
            }
            else
            {
                DisplaySuccessTextBox(tbQuantity, txtErrorQuantity, thickness);
                result = result && true;
            }
            if (cbCountry.SelectedItem == null)
            {
                DisplayErrorComboBox(errorMessage: "* Davlatni tanlang!! *", txtErrorCountry);
                result = result && false;
            }
            else
            {
                DisplaySuccessComboBox(txtErrorCountry);
                result = result && true;

            }

            if (cbCarType.SelectedItem == null)
            {
                DisplayErrorComboBox(errorMessage: "* Mashinani tanlang!! *", txtErrorCarType);
                result = result && false;

            }
            else
            {
                DisplaySuccessComboBox(txtErrorCarType);
                result = result && true;

            }

            if (cbCompany.SelectedItem == null)
            {
                DisplayErrorComboBox(errorMessage: "* Zavodni tanlang!! *", txtErrorCompany);
                result = result && false;

            }
            else
            {
                DisplaySuccessComboBox(txtErrorCompany);
                result = result && true;
            }

            if (cbSetType.SelectedItem == null)
            {
                DisplayErrorComboBox(errorMessage: "*  O`ramni  tanlang!! *", txtErrorSetType);
                result = result && false;

            }
            else
            {
                DisplaySuccessComboBox(txtErrorSetType);
                result = result && true;
            }
            return result;

        }

        private void btnAddCountry_Click(object sender, RoutedEventArgs e)
        {
            var addCountryWindow = new AddCountryWindow();
            this.Opacity = 0.4;
            addCountryWindow.ShowDialog();
            this.Opacity = 1.0;
            PopulateComboboxes();
        }

        private void btnAddCompany_Click(object sender, RoutedEventArgs e)
        {
            var addCompanyWindow = new AddCompanyWindow();
            this.Opacity = 0.4;
            addCompanyWindow.ShowDialog();
            this.Opacity = 1.0;
            PopulateComboboxes();
        }

        private void btnAddCarType_Click(object sender, RoutedEventArgs e)
        {
            var addCarTypeWindow = new AddCarTypeWindow();
            this.Opacity = 0.4;
            addCarTypeWindow.ShowDialog();
            this.Opacity = 1.0;
            PopulateComboboxes();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ClearTextBoxesAndComboBoxes()
        {
            tbName.txtInput.Clear();
            tbCode.txtInput.Clear();
            tbPrice.txtInput.Clear();
            tbRealPrice.txtInput.Clear();
            tbQuantity.txtInput.Clear();
            tbUsdPrice.txtInput.Clear();
            tbUsdPrice2.txtInput.Clear();

            cbCarType.SelectedIndex = -1;
            cbCompany.SelectedIndex = -1;
            cbSetType.SelectedIndex = -1;
            cbCountry.SelectedIndex = -1;
        }


        private bool IsValidNumericInputFields()
        {
            var result = true;
            var thickness = new Thickness(2);


            if (rbUzs.IsChecked == true)
            {

                if (!double.TryParse(tbRealPrice.txtInput.Text, out _))
                {
                    DisplayErrorTextBox(errorMessage: "* Son kiriting! *", tbRealPrice, txtErrorRealPrice, thickness);
                    result = result && false;
                }
                if (!double.TryParse(tbPrice.txtInput.Text, out _))
                {
                    DisplayErrorTextBox(errorMessage: "* Son kiriting! *", tbPrice, txtErrorPrice, thickness);
                    result = result && false;

                }
            }
            if (!int.TryParse(tbQuantity.txtInput.Text, out _))
            {
                DisplayErrorTextBox(errorMessage: "* Son kiriting! *", tbQuantity, txtErrorQuantity, thickness);
                result = result && false;

            }
            if (rbUsd.IsChecked == true)
            {

                if (!double.TryParse(tbUsdPrice.txtInput.Text, out _))
                {
                    DisplayErrorTextBox(errorMessage: "* Son kiriting! *", tbUsdPrice, txtErrorUsdPrice, thickness);
                    result = result && false;

                }
                if (!double.TryParse(tbUsdPrice2.txtInput.Text, out _))
                {
                    DisplayErrorTextBox(errorMessage: "* Son kiriting! *", tbUsdPrice2, txtErrorUsdPrice2, thickness);
                    result = result && false;

                }
            }
            return result;

        }



        private void textBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ClearableTextBox textBox = sender as ClearableTextBox;
            var textBoxName = textBox.Name;

            textBox.txtInput.BorderThickness = new Thickness(2);
            textBox.txtInput.BorderBrush = Brushes.Green;

            switch (textBoxName)
            {
                case "tbName": txtErrorName.Text = string.Empty; break;
                case "tbRealPrice": txtErrorRealPrice.Text = string.Empty; break;
                case "tbPrice": txtErrorPrice.Text = string.Empty; break;
                case "tbQuantity": txtErrorQuantity.Text = string.Empty; break;
                case "tbUsdPrice": txtErrorQuantity.Text = string.Empty; break;
                case "tbUsdPrice2": txtErrorQuantity.Text = string.Empty; break;
                default: break;
            }
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            string comboBoxName = comboBox.Name;
            switch (comboBoxName)
            {
                case "cbCountry": txtErrorCountry.Text = string.Empty; break;
                case "cbCarType": txtErrorCarType.Text = string.Empty; break;
                case "cbCompany": txtErrorCompany.Text = string.Empty; break;
                case "cbSetType": txtErrorSetType.Text = string.Empty; break;
                default: break;
            }
        }



        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (rbUzs.IsChecked == true)
            {
                tbUsdPrice.Visibility = Visibility.Collapsed;
                tbUsdPrice2.Visibility = Visibility.Collapsed;
                txtErrorUsdPrice.Visibility = Visibility.Collapsed;
                txtErrorUsdPrice2.Visibility = Visibility.Collapsed;
            }
            if (rbUzs.IsChecked == false)
            {
                tbUsdPrice.Visibility = Visibility.Visible;
                tbUsdPrice2.Visibility = Visibility.Visible;
                txtErrorUsdPrice.Visibility = Visibility.Visible;
                txtErrorUsdPrice2.Visibility = Visibility.Visible;
            }

            if (rbUsd.IsChecked == true)
            {
                tbRealPrice.Visibility = Visibility.Collapsed;
                tbPrice.Visibility = Visibility.Collapsed;
                txtErrorRealPrice.Visibility = Visibility.Collapsed;
                txtErrorPrice.Visibility = Visibility.Collapsed;
            }
            if (rbUsd.IsChecked == false)
            {
                tbRealPrice.Visibility = Visibility.Visible;
                tbPrice.Visibility = Visibility.Visible;
                txtErrorRealPrice.Visibility = Visibility.Visible;
                txtErrorPrice.Visibility = Visibility.Visible;
            }
        }
    }
}


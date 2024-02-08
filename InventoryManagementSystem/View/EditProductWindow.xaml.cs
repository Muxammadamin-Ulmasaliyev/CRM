using InventoryManagementSystem.Model;
using InventoryManagementSystem.UserControls;
using Notification.Wpf;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace InventoryManagementSystem.View
{
    public partial class EditProductWindow : Window
    {
        private Product _productToUpdate;
        private NotificationManager notificationManager;

        public EditProductWindow(Product productToUpdate)
        {
            notificationManager = new ();    
            this._productToUpdate = productToUpdate;
            InitializeComponent();
            PopulateInfo(productToUpdate);
        }
        private void PopulateInfo(Product productToUpdate)
        {
            if (productToUpdate.USDPrice == null)
            {
                txtPlaceholderUsdPrice.Visibility = Visibility.Collapsed;
                txtPlaceholderUsdPriceForCustomer.Visibility = Visibility.Collapsed;
                tbUsdPrice.Visibility = Visibility.Collapsed;
                tbUsdPrice2.Visibility = Visibility.Collapsed;
            }
            if(productToUpdate.RealPrice == null)
            {
                txtPlaceholderRealPrice.Visibility = Visibility.Collapsed;
                txtPlaceholderPrice.Visibility = Visibility.Collapsed;
                tbRealPrice.Visibility = Visibility.Collapsed;
                tbPrice.Visibility = Visibility.Collapsed;
            }

            PopulateComboboxes();
            tbName.txtInput.Text = productToUpdate.Name;
            tbCode.txtInput.Text = productToUpdate.Code;
            tbRealPrice.txtInput.Text = productToUpdate.RealPrice.ToString();
            tbPrice.txtInput.Text = productToUpdate.Price.ToString();
            tbUsdPrice.txtInput.Text = productToUpdate.USDPrice.ToString();
            tbUsdPrice2.txtInput.Text = productToUpdate.USDPriceForCustomer.ToString();
            tbQuantity.txtInput.Text = productToUpdate.Quantity.ToString();

            cbCountry.SelectedItem = productToUpdate.Country;
            cbCompany.SelectedItem = productToUpdate.Company;
            cbCarType.SelectedItem = productToUpdate.CarType;
            cbSetType.SelectedItem = productToUpdate.SetType;

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
        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (IsValidTextBoxesAndComboBoxes() && IsValidNumericInputFields())
            {

                var productToUpdate = new Product()
                {
                    Id = _productToUpdate.Id,
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

                using (var dbContext = new AppDbContext())
                {
                    dbContext.Products.Update(productToUpdate);
                    dbContext.SaveChanges();
                }
                notificationManager.Show("Success", "Product muvaffaqiyatli yangilandi !", NotificationType.Success);

                Close();
            }
        }

        private bool IsValidTextBoxesAndComboBoxes()
        {
            bool result = true;
            var thickness = new Thickness(2);

            if (string.IsNullOrWhiteSpace(tbName.txtInput.Text))
            {
                tbName.txtInput.BorderThickness = thickness;
                tbName.txtInput.BorderBrush = Brushes.Red;
                txtErrorName.Text = "* Mahsulot nomini kiriting! *";
                result = result && false;
            }
            else
            {
                tbName.txtInput.BorderThickness = thickness;
                tbName.txtInput.BorderBrush = Brushes.Green;
                txtErrorName.Text = string.Empty;
                result = result && true;

            }

            /*if (string.IsNullOrWhiteSpace(tbRealPrice.txtInput.Text))
            {
                tbRealPrice.txtInput.BorderThickness = thickness;
                tbRealPrice.txtInput.BorderBrush = Brushes.Red;
                txtErrorRealPrice.Text = "* Tannarxni kiriting! *";
                result = result && false;
            }
            else
            {
                tbRealPrice.txtInput.BorderThickness = thickness;
                tbRealPrice.txtInput.BorderBrush = Brushes.Green;
                txtErrorRealPrice.Text = string.Empty;
                result = result && true;
            }*/

            /*  if (string.IsNullOrWhiteSpace(tbPrice.txtInput.Text))
              {
                  tbPrice.txtInput.BorderThickness = thickness;
                  tbPrice.txtInput.BorderBrush = Brushes.Red;
                  txtErrorPrice.Text = "* Donabay narxni kiriting! *";
                  result = result && false;

              }
              else
              {
                  tbPrice.txtInput.BorderThickness = thickness;
                  tbPrice.txtInput.BorderBrush = Brushes.Green;
                  txtErrorPrice.Text = string.Empty;
                  result = result && true;

              }*/

            /*if (string.IsNullOrWhiteSpace(tbSalePrice.txtInput.Text))
            {
                tbSalePrice.txtInput.BorderThickness = thickness;
                tbSalePrice.txtInput.BorderBrush = Brushes.Red;
                txtErrorSalePrice.Text = "* Optom narxni kiriting! *";
                result = result && false;

            }
            else
            {
                tbSalePrice.txtInput.BorderThickness = thickness;
                tbSalePrice.txtInput.BorderBrush = Brushes.Green;
                txtErrorSalePrice.Text = string.Empty;
                result = result && true;

            }*/

            if (string.IsNullOrWhiteSpace(tbQuantity.txtInput.Text))
            {
                tbQuantity.txtInput.BorderThickness = thickness;
                tbQuantity.txtInput.BorderBrush = Brushes.Red;
                txtErrorQuantity.Text = "* Miqdorni  kiriting! *";
                result = result && false;

            }
            else
            {
                tbQuantity.txtInput.BorderThickness = thickness;
                tbQuantity.txtInput.BorderBrush = Brushes.Green;
                txtErrorQuantity.Text = string.Empty;
                result = result && true;
            }

            /*if (string.IsNullOrWhiteSpace(tbUsdPrice.txtInput.Text))
            {
                tbUsdPrice.txtInput.BorderThickness = thickness;
                tbUsdPrice.txtInput.BorderBrush = Brushes.Red;
                txtErrorUsdPrice.Text = "* $ tannarxni  kiriting! *";
                result = result && false;

            }
            else
            {
                tbUsdPrice.txtInput.BorderThickness = thickness;
                tbUsdPrice.txtInput.BorderBrush = Brushes.Green;
                txtErrorUsdPrice.Text = string.Empty;
                result = result && true;
            }*/
            // ComboBoxes  ********************************

            if (cbCountry.SelectedItem == null)
            {
                txtErrorCountry.Text = "* Davlatni tanlang!! *";
                result = result && false;
            }
            else
            {
                txtErrorCountry.Text = string.Empty;
                result = result && true;

            }

            if (cbCarType.SelectedItem == null)
            {
                txtErrorCarType.Text = "* Mashinani tanlang!! *";
                result = result && false;

            }
            else
            {
                txtErrorCarType.Text = string.Empty;
                result = result && true;

            }

            if (cbCompany.SelectedItem == null)
            {
                txtErrorCompany.Text = "* Zavodni tanlang!! *";
                result = result && false;

            }
            else
            {
                txtErrorCompany.Text = string.Empty;
                result = result && true;

            }

            if (cbSetType.SelectedItem == null)
            {
                txtErrorSetType.Text = "* O`ramni tanlang!! *";
                result = result && false;

            }
            else
            {
                txtErrorSetType.Text = string.Empty;
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

            if (!string.IsNullOrWhiteSpace(tbRealPrice.txtInput.Text) && !double.TryParse(tbRealPrice.txtInput.Text, out _))
            {
                tbRealPrice.txtInput.BorderThickness = new Thickness(2);
                tbRealPrice.txtInput.BorderBrush = Brushes.Red;
                txtErrorRealPrice.Text = "* Son kiriting! *";

                result = result && false;
            }
            if (!string.IsNullOrWhiteSpace(tbPrice.txtInput.Text) && !double.TryParse(tbPrice.txtInput.Text, out _))
            {
                tbPrice.txtInput.BorderThickness = new Thickness(2);
                tbPrice.txtInput.BorderBrush = Brushes.Red;
                txtErrorPrice.Text = "* Son kiriting! *";
                result = result && false;

            }
           
            if (!string.IsNullOrWhiteSpace(tbQuantity.txtInput.Text) && !int.TryParse(tbQuantity.txtInput.Text, out _))
            {
                tbQuantity.txtInput.BorderThickness = new Thickness(2);
                tbQuantity.txtInput.BorderBrush = Brushes.Red;
                txtErrorQuantity.Text = "* Son kiriting! *";
                result = result && false;

            }
            if (!string.IsNullOrWhiteSpace(tbUsdPrice.txtInput.Text) && !double.TryParse(tbUsdPrice.txtInput.Text, out _))
            {
                tbUsdPrice.txtInput.BorderThickness = new Thickness(2);
                tbUsdPrice.txtInput.BorderBrush = Brushes.Red;
                txtErrorUsdPrice.Text = "* Son kiriting! *";
                result = result && false;

            }
            if (!string.IsNullOrWhiteSpace(tbUsdPrice2.txtInput.Text) && !double.TryParse(tbUsdPrice2.txtInput.Text, out _))
            {
                tbUsdPrice2.txtInput.BorderThickness = new Thickness(2);
                tbUsdPrice2.txtInput.BorderBrush = Brushes.Red;
                txtErrorUsdPrice2.Text = "* Son kiriting! *";
                result = result && false;

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
    }
}


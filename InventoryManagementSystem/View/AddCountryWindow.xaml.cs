using InventoryManagementSystem.Model;
using Notification.Wpf;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace InventoryManagementSystem.View
{
    public partial class AddCountryWindow : Window
    {
        private NotificationManager notificationManager;
        public event EventHandler AddCountryButtonClicked;

        public AddCountryWindow()
        {
            notificationManager = new ();
            InitializeComponent();
            tbName.txtInput.Focus();
            KeyDown += btnAddCountry_KeyDown;
        }

        private void btnAddCountry_Click(object sender, RoutedEventArgs e)
        {
            
            if (string.IsNullOrWhiteSpace(tbName.txtInput.Text))
            {
                txtErrorName.Text = "* Davlat nomini kiriting ! *";
                tbName.txtInput.BorderThickness = new Thickness(2);
                tbName.txtInput.BorderBrush = Brushes.Red;
            }
            else
            {
                using (var dbContext = new AppDbContext())
                {
                    dbContext.Countries.Add(new Country { Name = tbName.txtInput.Text });
                    dbContext.SaveChanges();
             
                }

                notificationManager.Show("Success", "country added successfully", NotificationType.Success);
                tbName.txtInput.Clear();
                CountryAdded();
            }

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void tbName_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            tbName.txtInput.BorderThickness = new Thickness(2);
            tbName.txtInput.BorderBrush = Brushes.Green;
            txtErrorName.Text = string.Empty;
        }
        protected virtual void CountryAdded()
        {
            AddCountryButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnAddCountry_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (tbName.txtInput.IsFocused)
                {
                    btnAddCountry_Click((object)sender, e);
                }
            }
        }
    }
}

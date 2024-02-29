using InventoryManagementSystem.Model;
using InventoryManagementSystem.Services;
using Notification.Wpf;
using System.Windows;
using System.Windows.Input;

namespace InventoryManagementSystem.View
{
    public partial class AddCountryWindow : Window
    {
        private NotificationManager _notificationManager;
        public event EventHandler AddCountryButtonClicked;
        private readonly CountryService _countryService;
        public AddCountryWindow()
        {
            _notificationManager = new();
            _countryService = new(new AppDbContext());
            InitializeComponent();
            tbName.txtInput.Focus();
            KeyDown += btnAddCountry_KeyDown;
            KeyDown += btnCancel_KeyDown;
        }

        private void btnAddCountry_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(tbName.txtInput.Text))
            {
                DisplayError("* Davlat nomini kiriting ! *");
                return;
            }
            using (var dbContext = new AppDbContext())
            {
                if (_countryService.IsCountryExists(countryName: tbName.txtInput.Text))
                {
                    DisplayError("* Shu nomli davlat bazada mavjud ! *");
                    return;
                }

                _countryService.AddCountry(countryName: tbName.txtInput.Text);

            }

            _notificationManager.Show("Success", "country added successfully", NotificationType.Success);
            tbName.txtInput.Clear();
            CountryAdded();

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void tbName_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {

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
        private void DisplayError(string errorMessage)
        {
            txtErrorName.Text = errorMessage;

        }

        private void btnCancel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                btnCancel_Click((object)sender, e);
            }
        }
    }
}

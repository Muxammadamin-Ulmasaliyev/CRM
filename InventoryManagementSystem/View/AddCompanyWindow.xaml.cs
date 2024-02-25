using InventoryManagementSystem.Model;
using InventoryManagementSystem.Services;
using Notification.Wpf;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace InventoryManagementSystem.View
{
    public partial class AddCompanyWindow : Window
    {
        private NotificationManager notificationManager;
        public event EventHandler AddCompanyButtonClicked;
        private readonly CompanyService _companyService;

        public AddCompanyWindow()
        {
            notificationManager = new();
            _companyService = new(new AppDbContext());
            InitializeComponent();
            tbName.txtInput.Focus();
            KeyDown += btnAddCompany_KeyDown;
        }

        private void btnAddCompany_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbName.txtInput.Text))
            {
                DisplayError("* Zavod nomini kiriting ! *");
                return;
            }
            using (var dbContext = new AppDbContext())
            {
                if (_companyService.IsCompanyExists(companyName: tbName.txtInput.Text))
                {
                    DisplayError("* Shu nomli zavod bazada mavjud ! *");
                    return;
                }
                _companyService.AddCompany(companyName: tbName.txtInput.Text);
            }

            notificationManager.Show("Success", "company added successfully", NotificationType.Success);
            tbName.txtInput.Clear();
            CompanyAdded();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void tbName_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            
            txtErrorName.Text = string.Empty;
        }

        protected virtual void CompanyAdded()
        {
            AddCompanyButtonClicked?.Invoke(this, EventArgs.Empty);
        }



        private void btnAddCompany_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (tbName.txtInput.IsFocused)
                {
                    btnAddCompany_Click((object)sender, e);
                }
            }
        }

        private void DisplayError(string errorMessage)
        {
            txtErrorName.Text = errorMessage;
            
        }
    }
}

using InventoryManagementSystem.Model;
using InventoryManagementSystem.Services;
using Notification.Wpf;
using System.Windows;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

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
            WindowStylingHelper.SetDefaultFontFamily(this);

            tbName.txtInput.Focus();
            KeyDown += btnAddCompany_KeyDown;
            KeyDown += btnCancel_KeyDown;
        }

        private void btnAddCompany_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbName.txtInput.Text))
            {
                DisplayError("* Завод номини киритинг *");
                return;
            }
            if (_companyService.IsCompanyExists(companyName: tbName.txtInput.Text))
            {
                DisplayError("* Шу номли завод базада мавжуд ! *");
                return;
            }
            _companyService.AddCompany(companyName: tbName.txtInput.Text);

            notificationManager.Show("Муваффакият", "Завод кушилди", NotificationType.Success);
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

        private void btnCancel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                btnCancel_Click((object)sender, e);
            }
        }
    }
}

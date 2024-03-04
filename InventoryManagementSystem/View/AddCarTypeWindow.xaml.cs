using InventoryManagementSystem.Model;
using InventoryManagementSystem.Services;
using Notification.Wpf;
using System.Windows;
using System.Windows.Input;

namespace InventoryManagementSystem.View
{
    public partial class AddCarTypeWindow : Window
    {
        private NotificationManager notificationManager;
        public event EventHandler AddCarTypeButtonClicked;
        private readonly CarTypeService _carTypeService;


        public AddCarTypeWindow()
        {
            notificationManager = new();
            _carTypeService = new(new AppDbContext());
            InitializeComponent();
            WindowStylingHelper.SetDefaultFontFamily(this);
            tbName.txtInput.Focus();
            KeyDown += btnAddCarType_KeyDown;
            KeyDown += btnCancel_KeyDown;

        }

        private void btnAddCarType_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(tbName.txtInput.Text))
            {
                DisplayError("* Машина номини киритинг ! *");
                return;
            }
            if (_carTypeService.IsCarTypeExists(carTypeName: tbName.txtInput.Text))
            {
                DisplayError("* Шу номли машина базада мавжуд ! *");
                return;
            }
            _carTypeService.AddCarType(carTypeName: tbName.txtInput.Text);
            notificationManager.Show("Муваффакият !", "Машина кушилди", NotificationType.Success);
            tbName.txtInput.Clear();
            CarTypeAdded();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void tbName_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            txtErrorName.Text = string.Empty;
        }

        protected virtual void CarTypeAdded()
        {
            AddCarTypeButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnAddCarType_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (tbName.txtInput.IsFocused)
                {
                    btnAddCarType_Click((object)sender, e);
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

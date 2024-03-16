using InventoryManagementSystem.Model;
using InventoryManagementSystem.Services;
using Notification.Wpf;
using System.Windows;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace InventoryManagementSystem.View
{

    public partial class AddSetTypeWindow : Window
    {

        private NotificationManager notificationManager;
        public event EventHandler AddSetTypeButtonClicked;
        public AddSetTypeWindow()
        {
            notificationManager = new();
            InitializeComponent();
            WindowStylingHelper.SetDefaultFontFamily(this);

            tbName.txtInput.Focus();
            KeyDown += btnAddSetType_KeyDown;
            KeyDown += btnCancel_KeyDown;
        }

        private void tbName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            txtErrorName.Text = string.Empty;

        }

        private void btnAddSetType_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbName.txtInput.Text))
            {
                DisplayError("* Урам номини киритинг ! *");
                return;
            }
            using (var dbContext = new AppDbContext())
            {
                if (dbContext.SetTypes.Any(st => st.Name.ToLower() == tbName.txtInput.Text.ToLower()))
                {
                    DisplayError("* Шу номли Урам базада мавжуд ! *");
                    return;
                }
                dbContext.SetTypes.Add(new SetType() { Name = tbName.txtInput.Text });
                dbContext.SaveChanges();
            }
            notificationManager.Show("Муваффакият", "Урам кушилди", NotificationType.Success);
            tbName.txtInput.Clear();
            SetTypeAdded();
        }
        protected virtual void SetTypeAdded()
        {
            AddSetTypeButtonClicked?.Invoke(this, EventArgs.Empty);
        }
        private void btnAddSetType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnAddSetType_Click((object)sender, e);
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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

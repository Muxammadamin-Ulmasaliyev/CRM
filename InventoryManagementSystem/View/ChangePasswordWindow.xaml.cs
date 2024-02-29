using Notification.Wpf;
using System.Windows;
using System.Windows.Input;

namespace InventoryManagementSystem.View
{
    public partial class ChangePasswordWindow : Window
    {
        private NotificationManager _notificationManager;
        public ChangePasswordWindow()
        {
            KeyDown += btnConfirm_KeyDown;
            KeyDown += btnCancel_KeyDown;
            _notificationManager = new();
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (!IsPasswordsAreSame())
            {
                txtError.Text = "* Passwords must be same *";
                return;
            }
            if (!ValidatePasswordLengths())
            {
                txtError.Text = "* Passwords must be at least 4 char length *";
                return;
            }

            Properties.Settings.Default.AdminPassword = pbInput.Password;
            Properties.Settings.Default.Save();
            _notificationManager.Show("Success", "password changed successfully", NotificationType.Success);

            Close();

        }

        private bool IsPasswordsAreSame()
        {
            return pbInput.Password == pbInputConfirmation.Password;
        }

        private bool ValidatePasswordLengths()
        {
            return (pbInput.Password.Length >= 4 && pbInputConfirmation.Password.Length >= 4);
        }


        private void PasswordInput_PasswordChanged(object sender, RoutedEventArgs e)
        {
            txtError.Text = string.Empty;
        }

        private void btnConfirm_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnConfirm_Click(sender, e);
            }
        }

        private void btnCancel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                btnCancel_Click(sender, e);
            }
        }
    }
}

using InventoryManagementSystem.CustomControls;
using InventoryManagementSystem.Services;
using Notification.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InventoryManagementSystem.View
{
    public partial class LicenseCheckWindow : Window
    {
        private NotificationManager notificationManager;
        private bool _isPasswordCorrect = false;
        public LicenseCheckWindow()
        {
            
            notificationManager = new();
            InitializeComponent();
            WindowStylingHelper.SetDefaultFontFamily(this);

            passwordBox.Focus();

            KeyDown += btnSubmit_KeyDown;
        }

        public bool IsPasswordCorrect()
        {
            return _isPasswordCorrect;
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (passwordBox.Password == Properties.Settings.Default.LicensePassword)
            {
                this.Close();
                _isPasswordCorrect = true;
                Properties.Settings.Default.IsLicensed = true;
                Properties.Settings.Default.Save();
            }
            else
            {
                notificationManager.Show("Хатолик", "Пароль хато !", NotificationType.Error);
                passwordBox.Clear();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSubmit_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (passwordBox.IsFocused)
                {
                    btnSubmit_Click((object)sender, e);
                }
            }

        }
    }
}

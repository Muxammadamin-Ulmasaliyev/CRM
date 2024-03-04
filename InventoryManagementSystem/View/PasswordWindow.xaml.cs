using InventoryManagementSystem.CustomControls;
using InventoryManagementSystem.Services;
using Notification.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InventoryManagementSystem.View
{
    public partial class PasswordWindow : Window
    {
        private NotificationManager notificationManager;
        private Frame navframe;
        private NavButton navButton;
        private bool _isPasswordCorrect = false;

        public PasswordWindow(Frame navframe, NavButton navButton)
        {
            this.navframe = navframe;
            this.navButton = navButton;

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
            if (passwordBox.Password == Properties.Settings.Default.AdminPassword || passwordBox.Password == Properties.Settings.Default.AdminRecoveryPassword)
            {
                this.Close();
                notificationManager.Show("Муваффакият", "Хуш келибсиз", NotificationType.Success);
                _isPasswordCorrect = true;
                navframe.Navigate(navButton.NavLink);
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

using System.Windows;
using System.Windows.Controls;
using Notification.Wpf;
using InventoryManagementSystem.CustomControls;
using InventoryManagementSystem.View;
namespace InventoryManagementSystem
{
    public partial class MainWindow : Window
    {

        private NotificationManager notificationManager;
        private int sidebarPreviousSelectedIndex = 0;
        public MainWindow()
        {
            InitializeComponent();
            notificationManager = new();
            sidebar.SelectedIndex = 0;

        }


      
        private void sidebar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = sidebar.SelectedItem as NavButton;

            if (selected.Name == "navBtnAdmin")
            {
                CheckPassword(navframe, selected);
            }
            else
            {
                navframe.Navigate(selected.NavLink);
            }


        }

        private void CheckPassword(Frame navframe, NavButton navButton)
        {
            PasswordWindow passwordWindow = new PasswordWindow(navframe, navButton);
            this.Opacity = 0.4;
            passwordWindow.ShowDialog();
            this.Opacity = 1.0;

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var choice = MessageBox.Show("Dasturni yopmoqchimisz", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (choice == MessageBoxResult.No)
            {
                // Cancel the closing event to prevent the window from closing
                e.Cancel = true;
            }

        }

        
    }
}
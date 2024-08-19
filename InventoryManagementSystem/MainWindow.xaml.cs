using System.Windows;
using System.Windows.Controls;
using Notification.Wpf;
using InventoryManagementSystem.CustomControls;
using InventoryManagementSystem.View;
using InventoryManagementSystem.Shared;
namespace InventoryManagementSystem
{
    public partial class MainWindow : Window
    {

        private NotificationManager notificationManager;

        private List<int> sidebarSelectionsHistory;
        public MainWindow()
        {
            InitializeComponent();
            notificationManager = new();
            sidebarSelectionsHistory = new List<int>();
            sidebar.SelectedIndex = 0;
            sidebarSelectionsHistory.Add(sidebar.SelectedIndex);

           // Shared.Shared.SeedFakeProducts(5000);
		}


        /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private void sidebar_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sidebarSelectionsHistory.Count > 0 && sidebarSelectionsHistory[sidebarSelectionsHistory.Count - 1] == 0)
            {
                if (sidebar.SelectedIndex != 0)
                {
                    if(!Shared.Shared.IsCartEmpty)
                    {
                        var result = MessageBox.Show("Бошка менюга утсангиз, Корзинадаги товарлар корзинадан учиб кетади, давом этасизми?", "Огохлантириш !", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                        if (result == MessageBoxResult.Yes)
                        {
                            var selected = sidebar.SelectedItem as NavButton;

                            if (selected.Name == "navBtnAdmin" && Properties.Settings.Default.IsPasswordRequired)
                            {
                                CheckPassword(navframe, selected);
                            }
                            else
                            {
                                navframe.Navigate(selected.NavLink);
                            }
                            sidebarSelectionsHistory.Add(sidebar.SelectedIndex);
                        }
                        else
                        {
                            sidebar.SelectedIndex = sidebarSelectionsHistory[sidebarSelectionsHistory.Count - 1];

                        }
                    }
                    else
                    {
                        var selected = sidebar.SelectedItem as NavButton;

                        if (selected.Name == "navBtnAdmin" && Properties.Settings.Default.IsPasswordRequired)
                        {
                            CheckPassword(navframe, selected);
                        }
                        else
                        {
                            navframe.Navigate(selected.NavLink);
                        }
                        sidebarSelectionsHistory.Add(sidebar.SelectedIndex);
                    }
                    
                }

            }
            else
            {
                var selected = sidebar.SelectedItem as NavButton;

                if (selected.Name == "navBtnAdmin" && Properties.Settings.Default.IsPasswordRequired)
                {
                    CheckPassword(navframe, selected);
                }
                else
                {
                    navframe.Navigate(selected.NavLink);
                }
                sidebarSelectionsHistory.Add(sidebar.SelectedIndex);
            }
        }

        /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void CheckPassword(Frame navframe, NavButton navButton)
        {

            PasswordWindow passwordWindow = new PasswordWindow(navframe, navButton);
            this.Opacity = 0.4;
            passwordWindow.ShowDialog();
            this.Opacity = 1.0;

            if (!passwordWindow.IsPasswordCorrect())
            {
                sidebar.SelectedIndex = sidebarSelectionsHistory[sidebarSelectionsHistory.Count - 1];
            }

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var choice = MessageBox.Show("Дастурни ёпмокчимисиз ?", "Хабар", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (choice == MessageBoxResult.No)
            {
                // Cancel the closing event to prevent the window from closing
                e.Cancel = true;
            }

        }


    }
}
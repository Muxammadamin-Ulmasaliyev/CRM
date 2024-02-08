using InventoryManagementSystem.Model;
using Notification.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace InventoryManagementSystem.View
{
    public partial class AddCarTypeWindow : Window
    {
        private NotificationManager notificationManager;
        public event EventHandler AddCarTypeButtonClicked;



        public AddCarTypeWindow()
        {
            notificationManager = new();
            InitializeComponent();
            tbName.txtInput.Focus();
            KeyDown += btnAddCarType_KeyDown;

        }

        private void btnAddCarType_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(tbName.txtInput.Text))
            {
                txtErrorName.Text = "* Mashina nomini kiriting ! *";
                tbName.txtInput.BorderThickness = new Thickness(2);
                tbName.txtInput.BorderBrush = Brushes.Red;
            }
            else
            {
                using (var dbContext = new AppDbContext())
                {
                    dbContext.CarTypes.Add(new CarType { Name = tbName.txtInput.Text });
                    dbContext.SaveChanges();
                }
                notificationManager.Show("Success", "Car added successfully", NotificationType.Success);
                tbName.txtInput.Clear();
                CarTypeAdded();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void tbName_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            tbName.txtInput.BorderThickness = new Thickness(2);
            tbName.txtInput.BorderBrush = Brushes.Green;
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
    }
}

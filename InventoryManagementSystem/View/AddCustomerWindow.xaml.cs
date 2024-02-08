using InventoryManagementSystem.Model;
using InventoryManagementSystem.UserControls;
using Notification.Wpf;
using System.Windows;
using System.Windows.Media;

namespace InventoryManagementSystem.View
{
    public partial class AddCustomerWindow : Window
    {
        private NotificationManager notificationManager;
        public event EventHandler AddCustomerButtonClicked;

        public AddCustomerWindow()
        {
            notificationManager = new();
            InitializeComponent();
        }

        private void btnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            var customerToAdd = new Customer()
            {
                Name = tbName.txtInput.Text,
                Address = tbAddress.txtInput.Text,
                Phone = tbPhone.txtInput.Text,
            };
            using (var dbContext = new AppDbContext())
            {
                dbContext.Customers.Add(customerToAdd);
                dbContext.SaveChanges();
            }
            notificationManager.Show("Success", "Customer added successfully", NotificationType.Success);

            ClearTextBoxes();
            CustomerAdded();

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ClearableTextBox textBox = sender as ClearableTextBox;
            var textBoxName = textBox.Name;

            if (string.IsNullOrWhiteSpace(textBox.txtInput.Text))
            {
                textBox.txtInput.BorderThickness = new Thickness(2);
                textBox.txtInput.BorderBrush = Brushes.Red;
                switch (textBoxName)
                {
                    case "tbName": txtErrorName.Text = "* Klient FIO ni kiriting! *"; break;
                    case "tbPhone": txtErrorPhone.Text = "* Klient telefon raqamini  kiriting! *"; break;
                    default: break;
                }
            }
            else
            {
                textBox.txtInput.BorderThickness = new Thickness(2);
                textBox.txtInput.BorderBrush = Brushes.Green;
                switch (textBoxName)
                {
                    case "tbName": txtErrorName.Text = string.Empty; break;
                    case "tbPhone": txtErrorName.Text = string.Empty; break;
                    default: break;
                }
            }

            if (IsModelStateValid()) btnAddCustomer.IsEnabled = true;
            else btnAddCustomer.IsEnabled = false;
        }
        private bool IsModelStateValid()
        {
            return
                !string.IsNullOrWhiteSpace(tbName.txtInput.Text) &&
                !string.IsNullOrWhiteSpace(tbPhone.txtInput.Text);
        }

        private void tbAddress_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (IsModelStateValid()) { btnAddCustomer.IsEnabled = true; }
            else { btnAddCustomer.IsEnabled = false; }
        }


        private void ClearTextBoxes()
        {
            tbName.txtInput.Clear();
            tbPhone.txtInput.Clear();
            tbAddress.txtInput.Clear();
        }

        protected virtual void CustomerAdded()
        {
            AddCustomerButtonClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}

using InventoryManagementSystem.Model;
using InventoryManagementSystem.Services;
using InventoryManagementSystem.UserControls;
using Notification.Wpf;
using Notification.Wpf.Controls;
using System.Windows;
using System.Windows.Media;

namespace InventoryManagementSystem.View
{
    public partial class EditCustomerWindow : Window
    {

        private Customer customerToEdit;
        private NotificationManager notificationManager;
        private readonly CustomerService _customerService;
        public EditCustomerWindow(Customer customerToEdit)
        {
            notificationManager = new();
            _customerService = new(new AppDbContext());
            InitializeComponent();
            this.customerToEdit = customerToEdit;
            PopulateInfo(customerToEdit);
        }

        private void PopulateInfo(Customer customerToEdit)
        {
            tbName.txtInput.Text = customerToEdit.Name;
            tbPhone.txtInput.Text = customerToEdit.Phone;
            tbAddress.txtInput.Text = customerToEdit.Address;
        }
        private void btnEditCustomer_Click(object sender, RoutedEventArgs e)
        {
            var customerToUpdate = new Customer()
            {
                Id = customerToEdit.Id,
                Name = tbName.txtInput.Text,
                Address = tbAddress.txtInput.Text,
                Phone = tbPhone.txtInput.Text,
                TotalOrdersCount = customerToEdit.TotalOrdersCount
            };

            _customerService.Update(customerToUpdate);

            notificationManager.Show("Success", "Customer updated successfully", NotificationType.Success);

            Close();
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

            if (IsModelStateValid()) btnEditCustomer.IsEnabled = true;
            else btnEditCustomer.IsEnabled = false;
        }
        private bool IsModelStateValid()
        {
            return
                !string.IsNullOrWhiteSpace(tbName.txtInput.Text) &&
                !string.IsNullOrWhiteSpace(tbPhone.txtInput.Text);
        }

        private void tbAddress_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (IsModelStateValid()) { btnEditCustomer.IsEnabled = true; }
            else { btnEditCustomer.IsEnabled = false; }
        }


        private void ClearTextBoxes()
        {
            tbName.txtInput.Clear();
            tbPhone.txtInput.Clear();
            tbAddress.txtInput.Clear();
        }
    }
}


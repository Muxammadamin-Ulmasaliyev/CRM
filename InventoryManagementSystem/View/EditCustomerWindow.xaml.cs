using InventoryManagementSystem.Model;
using InventoryManagementSystem.Services;
using InventoryManagementSystem.UserControls;
using Notification.Wpf;
using Notification.Wpf.Controls;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace InventoryManagementSystem.View
{
    public partial class EditCustomerWindow : Window
    {

        public event EventHandler EditCustomerButtonClicked;


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

            KeyDown += btnEditCustomer_KeyDown;
            KeyDown += btnCancel_KeyDown;
        }

        private void PopulateInfo(Customer customerToEdit)
        {
            tbName.txtInput.Text = customerToEdit.Name;
            tbPhone.txtInput.Text = customerToEdit.Phone;
            tbAddress.txtInput.Text = customerToEdit.Address;
        }
        private void btnEditCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (!IsModelStateValid())
            {
                ShowErrorMessages();
                return;
            }
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

            CustomerEdited();
        }

        private void ShowErrorMessages()
        {
            if (string.IsNullOrWhiteSpace(tbName.txtInput.Text))
            {
                txtErrorName.Text = "* Klient FIO ni kiriting! *";
            }
            if (string.IsNullOrWhiteSpace(tbPhone.txtInput.Text))
            {
                txtErrorPhone.Text = "* Klient telefon raqamini  kiriting! *";
            }
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        
        private bool IsModelStateValid()
        {
            return
                !string.IsNullOrWhiteSpace(tbName.txtInput.Text) &&
                !string.IsNullOrWhiteSpace(tbPhone.txtInput.Text);
        }




        protected virtual void CustomerEdited()
        {
            EditCustomerButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnCancel_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                btnCancel_Click((object)sender, e);
            }
        }

        private void btnEditCustomer_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnEditCustomer_Click((object)sender, e);
            }
        }

        private void tbName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ClearableTextBox textBox = sender as ClearableTextBox;
            var textBoxName = textBox.Name;

            switch (textBoxName)
            {
                case "tbName": txtErrorName.Text = string.Empty; break;
                case "tbPhone": txtErrorPhone.Text = string.Empty; break;
                default: break;
            }
        }
    }
}


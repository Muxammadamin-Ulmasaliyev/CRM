﻿using InventoryManagementSystem.Model;
using InventoryManagementSystem.Services;
using InventoryManagementSystem.UserControls;
using Notification.Wpf;
using System.Windows;
using System.Windows.Input;

namespace InventoryManagementSystem.View
{
    public partial class AddCustomerWindow : Window
    {
        private NotificationManager notificationManager;
        public event EventHandler AddCustomerButtonClicked;
        private readonly CustomerService _customerService;

        public AddCustomerWindow()
        {
            _customerService = new(new AppDbContext());

            notificationManager = new();
            InitializeComponent();
            WindowStylingHelper.SetDefaultFontFamily(this);

            tbName.txtInput.Focus();
            KeyDown += btnAddCustomer_KeyDown;
            KeyDown += btnCancel_KeyDown;
        }

       

        private void btnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (!IsModelStateValid())
            {
                ShowErrorMessages();
                return;
            }
            var customerToAdd = new Customer()
            {
                Name = tbName.txtInput.Text,
                Address = tbAddress.txtInput.Text,
                Phone = tbPhone.txtInput.Text,
            };
           
            _customerService.Add(customerToAdd);
            notificationManager.Show("Муваффакият", "Мижоз кушилди", NotificationType.Success);

            ClearTextBoxes();
            CustomerAdded();

        }
        private void ShowErrorMessages()
        {
            if (string.IsNullOrWhiteSpace(tbName.txtInput.Text))
            {
                txtErrorName.Text = "* Клиент ФИО ни киритинг! *";
            }
           
        }
        private void tbName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ClearableTextBox textBox = sender as ClearableTextBox;
            var textBoxName = textBox.Name;

            switch (textBoxName)
            {
                case "tbName": txtErrorName.Text = string.Empty; break;
                default: break;
            }
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        
        private bool IsModelStateValid()
        {
            return
                !string.IsNullOrWhiteSpace(tbName.txtInput.Text);
        }

       


        private void ClearTextBoxes()
        {
            tbName.txtInput.Clear();
            tbPhone.txtInput.Clear();
            tbAddress.txtInput.Clear();
            txtErrorName.Text = string.Empty;
        }

        protected virtual void CustomerAdded()
        {
            AddCustomerButtonClicked?.Invoke(this, EventArgs.Empty);
        }

        private void btnAddCustomer_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnAddCustomer_Click((object)sender, e);
            }
        }

        private void btnCancel_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                btnCancel_Click((object)sender, e);
            }
        }

            
    }
}

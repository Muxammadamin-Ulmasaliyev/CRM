using InventoryManagementSystem.Services;
using InventoryManagementSystem.UserControls;
using Notification.Wpf;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace InventoryManagementSystem.View
{
    public partial class IncomeInputWindow : Window
    {
        NotificationManager _notificationManager;
        private double _paidAmount = 0;
        private bool isSuccess = false;
        public IncomeInputWindow()
        {
            _notificationManager = new();
            InitializeComponent();
            tbIncome.Focus();
            KeyDown += btnConfirmOrder_KeyDown;
            KeyDown += btnCancel_KeyDown;
        }


        public double GetTotalPaidAmount()
        {
            return _paidAmount;
        }
        public bool IsResultSuccessful()
        {
            return isSuccess;
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnConfirmOrder_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnConfirmOrder_Click((object)sender, e);
            }
        }

        private void btnCancel_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                btnCancel_Click((object)sender, e);
            }
        }

        private void btnConfirmOrder_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbIncome.Text))
            {
                txtErrorName.Text = "* Son kiriting *";
                isSuccess = false;
                return;
            }
            if (!double.TryParse(StringHelper.TrimAllWhiteSpaces(tbIncome.Text), out var income))
            {
                txtErrorName.Text = "* Son kiriting *";
                isSuccess = false;

                return;
            }

            _paidAmount = income;
            isSuccess = true;
            Close();

        }

       

        private void tbName_PreviewTextInput(object sender, TextChangedEventArgs e)
        {
            txtErrorName.Text = string.Empty;
            if (sender is TextBox textBox)
            {

                CultureInfo uzCulture = new CultureInfo("uz-UZ");
                uzCulture.NumberFormat.CurrencySymbol = "";
                // Remove non-numeric characters
                string input = new string(textBox.Text.Where(char.IsDigit).ToArray());

                if (double.TryParse(input, out var amount))
                {
                    // Format the amount as currency with uz-UZ culture
                    string formattedAmount = amount.ToString("C0", uzCulture);
                    textBox.Text = formattedAmount;
                    textBox.CaretIndex = formattedAmount.Length - 1;
                }


            }
        }
    }
}

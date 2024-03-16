using InventoryManagementSystem.Services;
using System.Windows;
using System.Windows.Input;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace InventoryManagementSystem.View
{
    public partial class SetQuantityWindow : Window
    {
        public int Quantity;
        public SetQuantityWindow()
        {
            InitializeComponent();
            WindowStylingHelper.SetDefaultFontFamily(this);

            tbQuantity.txtInput.Focus();
            KeyDown += btnSubmit_KeyDown;
            KeyDown += btnCancel_KeyDown;

        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(tbQuantity.txtInput.Text))
            {
                bool isNumber = int.TryParse(tbQuantity.txtInput.Text, out var quantity);
                if (isNumber)
                {
                    Quantity = quantity;
                    Close();
                }
                else
                {

                    txtError.Text = "* Сон киритинг * ";
                }

            }
            else
            {
                txtError.Text = "* Сон киритинг * ";
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public int GetQuantity()
        {
            return Quantity;
        }

        private void btnSubmit_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (tbQuantity.txtInput.IsFocused)
                {
                    btnSubmit_Click((object)sender, e);
                }
            }
        }

        private void btnCancel_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (tbQuantity.txtInput.IsFocused)
                {
                    btnCancel_Click((object)sender, e);
                }
            }
        }
    }
}

using InventoryManagementSystem.Model;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace InventoryManagementSystem.View
{
    public partial class PrintOverviewWindow : Window
    {
        private Order _order { get; }
      

        
        public PrintOverviewWindow(Order order)
        {
            InitializeComponent();
            _order = order;
            PopulateDataGrid(_order);
            PopulateTxts();


			KeyDown += PrintOverviewWindow_KeyDown;

        }

		private void PrintOverviewWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
            if(e.Key == System.Windows.Input.Key.Escape)
            {
                this.Close();
            }
		}

		private void PopulateDataGrid(Order order)
        {
            orderDetailsDataGrid.ItemsSource = _order.OrderDetails;
        }

        private void PopulateTxts()
        {

            var uzCulture = new CultureInfo("uz-UZ");
            uzCulture.NumberFormat.CurrencySymbol = "сум";
            txtCurrencyRate.Text = "Курс : " + Properties.Settings.Default.CurrencyRate.ToString("C0", uzCulture);
            txtCustomerName.Text = _order.Customer.Name.ToUpper();
            txtOrderDate.Text = _order.OrderDate.ToString("dd-MM-yyyy HH:mm:ss");
            txtOrderId.Text = _order.Id.ToString();
            txtDebtAmount.Text = _order.Customer.Debt.ToString("C0", uzCulture);
            txtTotalSum.Text = _order.TotalAmount.ToString("C0", uzCulture);
            txtTotalPaidSum.Text = _order.TotalPaidAmount.ToString("C0", uzCulture);
            txtNotPaidSum.Text = (_order.TotalAmount - _order.TotalPaidAmount).ToString("C0", uzCulture);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.IsEnabled = false;
                PrintDialog printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    printDialog.PrintVisual(print, "invoice");
                }
            }
            finally
            {
                this.IsEnabled = true;
            }
        }
    }
}

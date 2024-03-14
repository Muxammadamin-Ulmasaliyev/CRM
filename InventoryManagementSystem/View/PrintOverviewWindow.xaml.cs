﻿using InventoryManagementSystem.Model;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace InventoryManagementSystem.View
{
    public partial class PrintOverviewWindow : Window
    {
        private Order _order { get; }
        private bool _isUzsCurrency;



        private void ChangeCurrencyToUsd()
        {
            _order.OrderDetails.ForEach(od =>
            {
                od.Price = Math.Round((od.Price / Properties.Settings.Default.CurrencyRate), 3);

            });
        }

        
        public PrintOverviewWindow(Order order, bool isUzsCurrency)
        {
            _isUzsCurrency = isUzsCurrency;
            InitializeComponent();
            _order = order;
            PopulateDataGrid(_order);
            PopulateTxts();

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
            txtOrderDate.Text = _order.OrderDate.ToString("dd-MM-yyyy hh:mm:ss");
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
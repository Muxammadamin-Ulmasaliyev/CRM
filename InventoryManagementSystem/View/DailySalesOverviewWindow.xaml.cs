using InventoryManagementSystem.Model;
using InventoryManagementSystem.Services;
using System.Windows;

namespace InventoryManagementSystem.View
{
    public partial class DailySalesOverviewWindow : Window
    {

        private string _month;
        private int _year;
        private readonly OrderService _orderService;

        public DailySalesOverviewWindow(List<DailySale> dailySales, int monthIndex, int year)
        {
            _orderService = new(new AppDbContext());
            InitializeComponent();

            ChangeTitleOfWindow(monthIndex, year);
            PopulateDataGrid(dailySales);
            PopulateYearAndMonthTxt();
        }

        private void ChangeTitleOfWindow(int monthIndex, int year)
        {
            string monthName = string.Empty;
            switch (monthIndex)
            {
                case 1:
                    monthName = "Январь";
                    break;
                case 2:
                    monthName = "Февраль";
                    break;
                case 3:
                    monthName = "Март";
                    break;
                case 4:
                    monthName = "Апрель";
                    break;
                case 5:
                    monthName = "Май";
                    break;
                case 6:
                    monthName = "Июнь";
                    break;
                case 7:
                    monthName = "Июль";
                    break;
                case 8:
                    monthName = "Август";
                    break;
                case 9:
                    monthName = "Сентябрь";
                    break;
                case 10:
                    monthName = "Октябрь";
                    break;
                case 11:
                    monthName = "Ноябрь";
                    break;
                case 12:
                    monthName = "Декабрь";
                    break;
                default:
                    monthName = "Invalid month";
                    break;
            }

            this.Title = $"{year} - йил {monthName} ойи кунлик савдо хисоботи";


            _month = monthName;
            _year = year;

        }

        private void PopulateYearAndMonthTxt()
        {

            txtYearAndMonth.Text = $"{_month} / {_year}";
        }
        private void PopulateSelectedDateTxt(int day)
        {
            txtSelectedDate.Text = $"{day}/{_month}/{_year}";
        }
        private void PopulateDataGrid(List<DailySale> dailySales)
        {
            dailySalesDataGrid.ItemsSource = dailySales;
        }


        private void btnShowOrdersOfDate_Click(object sender, RoutedEventArgs e)
        {
            DailySale selectedDay = (DailySale)dailySalesDataGrid.SelectedItem;

            if (selectedDay != null)
            {
                var ordersOfDay = _orderService.GetOrdersOfDay(selectedDay.Year, selectedDay.Month, selectedDay.Day);
                ordersOfDayDataGrid.ItemsSource = ordersOfDay;
                PopulateSelectedDateTxt(selectedDay.Day);
            }
        }
    }
}

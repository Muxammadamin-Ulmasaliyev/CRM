﻿using InventoryManagementSystem.Model;
using InventoryManagementSystem.Services;
using Notification.Wpf;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Company = InventoryManagementSystem.Model.Company;

namespace InventoryManagementSystem.Pages
{
    public partial class CategoriesPage : Page
    {
        private ObservableCollection<Company> companies;
        private ObservableCollection<CarType> carTypes;
        private ObservableCollection<Country> countries;
        private ObservableCollection<SetType> setTypes;

        private readonly CompanyService _companyService;
        private readonly CarTypeService _carTypeService;
        private readonly CountryService _countryService;

        private NotificationManager notificationManager;


        public CategoriesPage()
        {
            InitializeComponent();
            notificationManager = new();
            _companyService = new(new AppDbContext());
            _carTypeService = new(new AppDbContext());
            _countryService = new(new AppDbContext());
            SetupUserCustomizationsSettings();
            PopulateDataGrids();
        }
        private void SetupUserCustomizationsSettings()
        {
            companyDataGrid.FontSize = Properties.Settings.Default.CategoriesDataGridFontSize;
            countryDataGrid.FontSize = Properties.Settings.Default.CategoriesDataGridFontSize;
            carTypeDataGrid.FontSize = Properties.Settings.Default.CategoriesDataGridFontSize;
            setTypeDataGrid.FontSize = Properties.Settings.Default.CategoriesDataGridFontSize;
            this.FontFamily = new FontFamily(Properties.Settings.Default.AppFontFamily);
        }
        private void PopulateDataGrids()
        {
            try
            {
                using (var dbContext = new AppDbContext())
                {
                    companies = new ObservableCollection<Company>(dbContext.Companies.ToList());
                    carTypes = new ObservableCollection<CarType>(dbContext.CarTypes.ToList());
                    countries = new ObservableCollection<Country>(dbContext.Countries.ToList());
                    setTypes = new ObservableCollection<SetType>(dbContext.SetTypes.ToList());
                }
                companyDataGrid.ItemsSource = companies;
                countryDataGrid.ItemsSource = countries;
                carTypeDataGrid.ItemsSource = carTypes;
                setTypeDataGrid.ItemsSource = setTypes;
            }
            catch (Exception)
            {

                throw;
            }
        }



        private void PopulateCompaniesDataGrid()
        {
            using (var dbContext = new AppDbContext())
            {
                companies = new ObservableCollection<Company>(dbContext.Companies.ToList());
                companyDataGrid.ItemsSource = companies;
            }
        }
        private void PopulateCountriesDataGrid()
        {
            using (var dbContext = new AppDbContext())
            {
                countries = new ObservableCollection<Country>(dbContext.Countries.ToList());
                countryDataGrid.ItemsSource = countries;
            }
        }
        private void PopulateCarTypesDataGrid()
        {
            using (var dbContext = new AppDbContext())
            {
                carTypes = new ObservableCollection<CarType>(dbContext.CarTypes.ToList());
                carTypeDataGrid.ItemsSource = carTypes;
            }
        }
        private void PopulateSetTypesDataGrid()
        {
            using (var dbContext = new AppDbContext())
            {
                setTypes = new ObservableCollection<SetType>(dbContext.SetTypes.ToList());
                setTypeDataGrid.ItemsSource = setTypes;
            }
        }
        private void scrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
                e.Handled = true;
            }
        }

        #region Companies
        private void btnDeleteCompany_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (button.DataContext is Company company)
                {
                    var choice = MessageBox.Show($"Бу заводга тегишли барча товарлар учиб кетади. ' {company.Name} ' : номли заводни учириб ташламокчимисиз ? ", "Огохлантириш !", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

                    if (choice == MessageBoxResult.Yes)
                    {
                        _companyService.Delete(company);
                        notificationManager.Show("Муваффакият !", "Завод учирилди !", NotificationType.Success);

                    }
                }
            }
            PopulateCompaniesDataGrid();
        }


        private void companyDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var editedCompany = e.Row.Item as Company;

            if (editedCompany != null)
            {
                var editedValue = (e.EditingElement as TextBox).Text;
                if (string.IsNullOrWhiteSpace(editedValue))
                {
                    notificationManager.Show("Хатолик !", "Завод номини киритинг !", NotificationType.Error);
                    (e.EditingElement as TextBox).Text = editedCompany.Name;
                    return;
                }

                editedCompany.Name = editedValue;
                _companyService.Update(editedCompany);
                notificationManager.Show("Муваффакият !", "Завод номи янгиланди !", NotificationType.Success);
            }
        }


        #endregion
        #region Countries
        private void btnDeleteCountry_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (button.DataContext is Country country)
                {
                    var choice = MessageBox.Show($"Бу давлатга тегишли барча товарлар учиб кетади. ' {country.Name} ' : номли давлатни учириб ташламокчимисиз ? ", "Огохлантириш !", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

                    if (choice == MessageBoxResult.Yes)
                    {
                        _countryService.Delete(country);

                        notificationManager.Show("Муваффакият !", "Давлат учирилди !", NotificationType.Success);

                    }
                }
            }
            PopulateCountriesDataGrid();
        }
        private void countryDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var editedCountry = e.Row.Item as Country;

            if (editedCountry != null)
            {
                var editedValue = (e.EditingElement as TextBox).Text;
                if (string.IsNullOrWhiteSpace(editedValue))
                {
                    notificationManager.Show("Хатолик !", "Давлат номини киритинг !", NotificationType.Error);
                    (e.EditingElement as TextBox).Text = editedCountry.Name;
                    return;
                }

                editedCountry.Name = editedValue;

                _countryService.Update(editedCountry);

                notificationManager.Show("Муваффакият !", "Давлат номи янгиланди !", NotificationType.Success);


            }
        }

        #endregion
        #region CarTypes
        private void btnDeleteCarType_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (button.DataContext is CarType carType)
                {
                    var choice = MessageBox.Show($"Бу машинага тегишли барча товарлар учиб кетади. ' {carType.Name} ' : номли машинани учириб ташламокчимисиз ? ", "Огохлантириш !", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

                    if (choice == MessageBoxResult.Yes)
                    {

                        _carTypeService.Delete(carType);

                        notificationManager.Show("Муваффакият !", "Машина учирилди !", NotificationType.Success);

                    }
                }
            }
            PopulateCarTypesDataGrid();
        }

        private void carTypeDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var editedCarType = e.Row.Item as CarType;

            if (editedCarType != null)
            {
                var editedValue = (e.EditingElement as TextBox).Text;
                if (string.IsNullOrWhiteSpace(editedValue))
                {
                    notificationManager.Show("Хатолик !", "Машина номини киритинг !", NotificationType.Error);
                    (e.EditingElement as TextBox).Text = editedCarType.Name;
                    return;
                }

                editedCarType.Name = editedValue;

                _carTypeService.Update(editedCarType);

                notificationManager.Show("Муваффакият !", "Машина номи янгиланди !", NotificationType.Success);


            }
        }
        #endregion
        #region SetTypes
        private void btnDeleteSetType_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                if (button.DataContext is SetType setType)
                {
                    var choice = MessageBox.Show($"Бу урамга тегишли барча товарлар учиб кетади. ' {setType.Name} ' : номли урамни учириб ташламокчимисиз ? ", "Огохлантириш !", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

                    if (choice == MessageBoxResult.Yes)
                    {
                        using (var dbContext = new AppDbContext())
                        {
                            dbContext.SetTypes.Remove(setType);
                            dbContext.SaveChanges();
                        }
                        notificationManager.Show("Муваффакият !", "Урам учирилди !", NotificationType.Success);

                    }
                }
            }
            PopulateSetTypesDataGrid();
        }

        private void setTypeDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            var editedSetType = e.Row.Item as SetType;

            if (editedSetType != null)
            {
                var editedValue = (e.EditingElement as TextBox).Text;
                if (string.IsNullOrWhiteSpace(editedValue))
                {
                    notificationManager.Show("Хатолик !", "Урам номини киритинг !", NotificationType.Error);

                    (e.EditingElement as TextBox).Text = editedSetType.Name;
                    return;

                }
                editedSetType.Name = editedValue;

                using (var dbContext = new AppDbContext())
                {
                    dbContext.SetTypes.Update(editedSetType);
                    dbContext.SaveChanges();

                }
                notificationManager.Show("Муваффакият !", "Урам номи янгиланди !", NotificationType.Success);
            }
        }
        #endregion
    }
}

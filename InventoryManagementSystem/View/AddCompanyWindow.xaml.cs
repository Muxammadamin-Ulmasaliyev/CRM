using InventoryManagementSystem.Model;
using Notification.Wpf;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace InventoryManagementSystem.View
{
    public partial class AddCompanyWindow : Window
    {
        private NotificationManager notificationManager;
        public event EventHandler AddCompanyButtonClicked;

        public AddCompanyWindow()
        {
            notificationManager = new();
            InitializeComponent();
            tbName.txtInput.Focus();
            KeyDown += btnAddCompany_KeyDown;
        }

        private void btnAddCompany_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbName.txtInput.Text))
            {
                txtErrorName.Text = "* Zavod nomini kiriting ! *";

                tbName.txtInput.BorderThickness = new Thickness(2);

                tbName.txtInput.BorderBrush = Brushes.Red;
            }
            else
            {
                using (var dbContext = new AppDbContext())
                {
                    dbContext.Companies.Add(new Company { Name = tbName.txtInput.Text });
                    dbContext.SaveChanges();
                }
               
                notificationManager.Show("Success", "company added successfully", NotificationType.Success);
                tbName.txtInput.Clear();
                CompanyAdded();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void tbName_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            tbName.txtInput.BorderThickness = new Thickness(2);
            tbName.txtInput.BorderBrush = Brushes.Green;
            txtErrorName.Text = string.Empty;
        }

        protected virtual void CompanyAdded()
        {
            AddCompanyButtonClicked?.Invoke(this, EventArgs.Empty);
        }

       

        private void btnAddCompany_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (tbName.txtInput.IsFocused)
                {
                    btnAddCompany_Click((object)sender, e);
                }
            }
        }
    }
}

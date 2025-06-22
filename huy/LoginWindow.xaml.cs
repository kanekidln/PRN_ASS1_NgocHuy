using System.Windows;

namespace huy
{
    public partial class LoginWindow : Window
    {
        private ViewModels.LoginViewModel viewModel;
        
        public LoginWindow()
        {
            InitializeComponent();
            viewModel = new ViewModels.LoginViewModel();
            DataContext = viewModel;
            
            // Handle successful login
            viewModel.LoginSuccessful += (sender, e) =>
            {
                if (e.IsAdmin)
                {
                    // Admin login
                    AdminDashboard adminDashboard = new AdminDashboard();
                    adminDashboard.Show();
                    this.Close();
                }
                else
                {
                    // Customer login
                    App.CurrentCustomerID = e.CustomerID;
                    CustomerProfile customerProfile = new CustomerProfile();
                    customerProfile.Show();
                    this.Close();
                }
            };
        }
    }
}
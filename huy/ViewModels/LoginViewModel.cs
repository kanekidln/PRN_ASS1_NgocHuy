using BusinessObjects;
using Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace huy.ViewModels
{
    public class LoginEventArgs : EventArgs
    {
        public bool IsAdmin { get; set; }
        public int CustomerID { get; set; }
    }
    
    public class LoginViewModel : ViewModelBase
    {
        private readonly CustomerService _customerService;
        private string _email = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;

        public event EventHandler<LoginEventArgs>? LoginSuccessful;

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand CancelCommand { get; }

        public LoginViewModel()
        {
            _customerService = new CustomerService();
            LoginCommand = new RelayCommand(parameter => ExecuteLogin(parameter));
            CancelCommand = new RelayCommand(_ => Application.Current.Shutdown());
        }

        private void ExecuteLogin(object? parameter)
        {
            try
            {
                ErrorMessage = string.Empty;
                
                // Get password from PasswordBox
                if (parameter is not PasswordBox passwordBox)
                {
                    ErrorMessage = "Password box not provided.";
                    return;
                }

                string password = passwordBox.Password;

                // Validate input
                if (string.IsNullOrWhiteSpace(Email))
                {
                    ErrorMessage = "Email is required.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    ErrorMessage = "Password is required.";
                    return;
                }

                // Check for admin login
                if (Email.Equals("admin@FUMiniHotelSystem.com", StringComparison.OrdinalIgnoreCase) && 
                    password.Equals("@@abc123@@"))
                {
                    // Admin login successful
                    OnLoginSuccessful(true, 0);
                    return;
                }

                // Check for customer login
                Customer? customer = _customerService.GetCustomerByEmail(Email);
                if (customer != null && customer.Password == password)
                {
                    // Check if customer is banned
                    if (customer.CustomerStatus == 2)
                    {
                        ErrorMessage = "This account has been banned. Please contact the administrator.";
                        return;
                    }
                    
                    // Customer login successful
                    OnLoginSuccessful(false, customer.CustomerID);
                    return;
                }

                // Login failed
                ErrorMessage = "Invalid email or password.";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
        }

        private void OnLoginSuccessful(bool isAdmin, int customerId)
        {
            LoginSuccessful?.Invoke(this, new LoginEventArgs 
            { 
                IsAdmin = isAdmin, 
                CustomerID = customerId 
            });
        }

        private void CloseCurrentWindow()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
} 
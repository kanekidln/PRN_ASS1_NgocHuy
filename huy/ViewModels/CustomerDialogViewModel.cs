using BusinessObjects;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace  huy.ViewModels
{
    public class CustomerDialogViewModel : ViewModelBase
    {
        private Customer _customer;
        private string _dialogTitle;
        private string _errorMessage = string.Empty;
        private bool _isEditing;

        public event EventHandler<bool> RequestClose;

        public Customer Customer
        {
            get => _customer;
            set => SetProperty(ref _customer, value);
        }

        public string DialogTitle
        {
            get => _dialogTitle;
            set => SetProperty(ref _dialogTitle, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public CustomerDialogViewModel(Customer customer, bool isEditing)
        {
            _isEditing = isEditing;
            _customer = customer ?? new Customer();

            DialogTitle = isEditing ? "Edit Customer" : "Add New Customer";

            SaveCommand = new RelayCommand(parameter => ExecuteSave(parameter));
            CancelCommand = new RelayCommand(_ => ExecuteCancel());
        }

        private void ExecuteSave(object parameter)
        {
            try
            {
                ErrorMessage = string.Empty;

                // Validate basic fields
                if (string.IsNullOrWhiteSpace(Customer.CustomerFullName))
                {
                    ErrorMessage = "Full name is required.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(Customer.EmailAddress))
                {
                    ErrorMessage = "Email is required.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(Customer.Telephone))
                {
                    ErrorMessage = "Phone number is required.";
                    return;
                }

                // Update password if a PasswordBox is provided
                if (parameter is PasswordBox passwordBox)
                {
                    string password = passwordBox.Password;
                    if (!string.IsNullOrWhiteSpace(password))
                    {
                        // Set the entered password
                        Customer.Password = password;
                    }
                    else if (!_isEditing)
                    {
                        // For new customers with no password input, set a default password
                        Customer.Password = "123456";
                    }
                    // For editing: if password is empty, keep the existing password
                }

                // Trigger the close request with a success result
                RequestClose?.Invoke(this, true);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
        }


        private void ExecuteCancel()
        {
            // Close dialog with cancel result
            RequestClose?.Invoke(this, false);
        }
    }
}
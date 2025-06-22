using BusinessObjects;
using Services;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace huy.ViewModels
{
    public class CustomerProfileViewModel : ViewModelBase
    {
        private readonly CustomerService _customerService;
        private readonly BookingReservationService _bookingService;
        private readonly RoomInformationService _roomService;
        private Customer _currentCustomer;
        private ObservableCollection<BookingReservation> _bookingHistory;
        private string _errorMessage = string.Empty;
        private string _selectedTab = "Profile";

        public Customer CurrentCustomer
        {
            get => _currentCustomer;
            set => SetProperty(ref _currentCustomer, value);
        }

        public ObservableCollection<BookingReservation> BookingHistory
        {
            get => _bookingHistory;
            set => SetProperty(ref _bookingHistory, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public string SelectedTab
        {
            get => _selectedTab;
            set => SetProperty(ref _selectedTab, value);
        }

        public ICommand UpdateProfileCommand { get; }
        public ICommand ChangePasswordCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand SelectTabCommand { get; }

        public CustomerProfileViewModel()
        {
            _customerService = new CustomerService();
            _bookingService = new BookingReservationService();
            _roomService = new RoomInformationService();
            _currentCustomer = new Customer();
            _bookingHistory = new ObservableCollection<BookingReservation>();

            UpdateProfileCommand = new RelayCommand(_ => UpdateProfile());
            ChangePasswordCommand = new RelayCommand(parameter => ChangePassword(parameter!));
            LogoutCommand = new RelayCommand(_ => Logout());
            SelectTabCommand = new RelayCommand(parameter => SelectedTab = parameter?.ToString() ?? "Profile");

            // Load customer data
            LoadCustomerData();
        }

        private void LoadCustomerData()
        {
            try
            {
                // Get current logged in customer
                int customerId = App.CurrentCustomerID;
                var customer = _customerService.GetCustomerByID(customerId);

                if (customer != null)
                {
                    CurrentCustomer = customer;
                    LoadBookingHistory(customerId);
                }
                else
                {
                    ErrorMessage = "Error: Unable to load customer data.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
        }

        private void LoadBookingHistory(int customerId)
        {
            try
            {
                var bookings = _bookingService.GetByCustomer(customerId);
                
                // Load room information for each booking
                foreach (var booking in bookings)
                {
                    booking.Room = _roomService.GetRoomByID(booking.RoomID);
                }
                
                BookingHistory = new ObservableCollection<BookingReservation>(bookings);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading booking history: {ex.Message}";
            }
        }

        private void UpdateProfile()
        {
            try
            {
                ErrorMessage = string.Empty;

                // Validate data
                if (string.IsNullOrWhiteSpace(CurrentCustomer.CustomerFullName))
                {
                    ErrorMessage = "Full name is required.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(CurrentCustomer.EmailAddress))
                {
                    ErrorMessage = "Email is required.";
                    return;
                }

                if (string.IsNullOrWhiteSpace(CurrentCustomer.Telephone))
                {
                    ErrorMessage = "Phone number is required.";
                    return;
                }

                // Update customer
                _customerService.UpdateCustomer(CurrentCustomer);
                MessageBox.Show("Profile updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
        }

        private void ChangePassword(object parameter)
        {
            try
            {
                ErrorMessage = string.Empty;

                if (parameter is object[] parameters && parameters.Length == 2)
                {
                    var newPasswordBox = parameters[0] as PasswordBox;
                    var confirmPasswordBox = parameters[1] as PasswordBox;

                    if (newPasswordBox == null || confirmPasswordBox == null)
                    {
                        ErrorMessage = "Invalid password input.";
                        return;
                    }

                    string newPassword = newPasswordBox.Password;
                    string confirmPassword = confirmPasswordBox.Password;

                    // Validate passwords
                    if (string.IsNullOrWhiteSpace(newPassword))
                    {
                        ErrorMessage = "New password is required.";
                        return;
                    }

                    if (newPassword != confirmPassword)
                    {
                        ErrorMessage = "Passwords do not match.";
                        return;
                    }

                    // Update password
                    CurrentCustomer.Password = newPassword;
                    _customerService.UpdateCustomer(CurrentCustomer);

                    // Clear password boxes
                    newPasswordBox.Password = string.Empty;
                    confirmPasswordBox.Password = string.Empty;

                    MessageBox.Show("Password changed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
        }

        private void Logout()
        {
            // Open login window
            var loginWindow = new LoginWindow();
            loginWindow.Show();

            // Close current window
            Application.Current.Windows[0].Close();
        }
    }
} 
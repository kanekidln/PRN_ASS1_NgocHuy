using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using huy;
using huy.ViewModels;

namespace huy.ViewModels
{
    public class AdminDashboardViewModel : ViewModelBase
    {
        private UserControl _currentViewContent;
        private string _currentView = "Customers";
        
        public UserControl CurrentViewContent
        {
            get => _currentViewContent;
            set => SetProperty(ref _currentViewContent, value);
        }
        
        public string CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }
        
        public ICommand NavigateCommand { get; }
        public ICommand LogoutCommand { get; }
        
        public AdminDashboardViewModel()
        {
            // Initialize commands
            NavigateCommand = new RelayCommand(NavigateTo);
            LogoutCommand = new RelayCommand(_ => Logout());
            
            // Set default view
            NavigateTo("Customers");
        }
        
        private void NavigateTo(object parameter)
        {
            if (parameter is string viewName)
            {
                CurrentView = viewName;
                
                switch (viewName)
                {
                    case "Customers":
                        CurrentViewContent = new Views.CustomerManagementView();
                        break;
                    case "Rooms":
                        CurrentViewContent = new Views.RoomManagementView();
                        break;
                    case "Bookings":
                        CurrentViewContent = new Views.BookingManagementView();
                        break;
                    case "Reports":
                        CurrentViewContent = new Views.ReportView();
                        break;
                }
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
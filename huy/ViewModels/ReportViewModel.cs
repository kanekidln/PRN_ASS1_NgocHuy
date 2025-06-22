using BusinessObjects;
using Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace huy.ViewModels
{
    public class ReportViewModel : ViewModelBase
    {
        private readonly BookingReservationService _bookingService;
        private readonly RoomInformationService _roomService;
        private readonly CustomerService _customerService;
        
        private DateTime _startDate = DateTime.Now.AddDays(-30);
        private DateTime _endDate = DateTime.Now;
        private ObservableCollection<BookingReservation> _bookings;
        private string _errorMessage = string.Empty;
        private decimal _totalRevenue;
        private int _totalBookings;
        private int _totalCustomers;
        private string _reportType = "Bookings";

        public DateTime StartDate
        {
            get => _startDate;
            set => SetProperty(ref _startDate, value);
        }

        public DateTime EndDate
        {
            get => _endDate;
            set => SetProperty(ref _endDate, value);
        }

        public ObservableCollection<BookingReservation> Bookings
        {
            get => _bookings;
            set => SetProperty(ref _bookings, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public decimal TotalRevenue
        {
            get => _totalRevenue;
            set => SetProperty(ref _totalRevenue, value);
        }

        public int TotalBookings
        {
            get => _totalBookings;
            set => SetProperty(ref _totalBookings, value);
        }

        public int TotalCustomers
        {
            get => _totalCustomers;
            set => SetProperty(ref _totalCustomers, value);
        }

        public string ReportType
        {
            get => _reportType;
            set => SetProperty(ref _reportType, value);
        }

        public ICommand GenerateReportCommand { get; }
        public ICommand BookingsReportCommand { get; }
        public ICommand RevenueReportCommand { get; }
        public ICommand CustomerReportCommand { get; }

        public ReportViewModel()
        {
            _bookingService = new BookingReservationService();
            _roomService = new RoomInformationService();
            _customerService = new CustomerService();
            _bookings = new ObservableCollection<BookingReservation>();

            GenerateReportCommand = new RelayCommand(_ => GenerateReport());
            BookingsReportCommand = new RelayCommand(_ => { ReportType = "Bookings"; GenerateReport(); });
            RevenueReportCommand = new RelayCommand(_ => { ReportType = "Revenue"; GenerateReport(); });
            CustomerReportCommand = new RelayCommand(_ => { ReportType = "Customers"; GenerateReport(); });

            // Generate initial report
            GenerateReport();
        }

        private void GenerateReport()
        {
            try
            {
                ErrorMessage = string.Empty;

                // Validate dates
                if (StartDate > EndDate)
                {
                    ErrorMessage = "Start date cannot be after end date.";
                    return;
                }

                // Get bookings for the selected period
                var bookings = _bookingService.GetByDateRange(StartDate, EndDate);
                
                // Sort by booking date in descending order
                bookings = bookings.OrderByDescending(b => b.BookingDate).ToList();
                
                // Load related data
                foreach (var booking in bookings)
                {
                    booking.Customer = _customerService.GetCustomerByID(booking.CustomerID);
                    booking.Room = _roomService.GetRoomByID(booking.RoomID);
                }
                
                Bookings = new ObservableCollection<BookingReservation>(bookings);
                
                // Calculate statistics
                TotalRevenue = bookings.Sum(b => b.TotalPrice);
                TotalBookings = bookings.Count;
                
                // Count unique customers
                var uniqueCustomers = bookings.Select(b => b.CustomerID).Distinct().Count();
                TotalCustomers = uniqueCustomers;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error generating report: {ex.Message}";
            }
        }
    }
} 
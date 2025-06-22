using BusinessObjects;
using Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace huy.ViewModels
{
    public class BookingViewModel : ViewModelBase
    {
        private readonly BookingReservationService _bookingService;
        private readonly RoomInformationService _roomService;
        private readonly CustomerService _customerService;
        private readonly RoomTypeService _roomTypeService;
        private ObservableCollection<BookingReservation> _bookings;
        private ObservableCollection<RoomInformation> _availableRooms;
        private ObservableCollection<Customer> _customers;
        private BookingReservation? _selectedBooking;
        private BookingReservation _newBooking = new();
        private string _searchText = string.Empty;
        private bool _isEditing;
        private string _errorMessage = string.Empty;
        private int _selectedBookingTypeFilter;
        private DateTime _startDate = DateTime.Now;
        private DateTime _endDate = DateTime.Now.AddDays(1);

        public ObservableCollection<BookingReservation> Bookings
        {
            get => _bookings;
            set => SetProperty(ref _bookings, value);
        }

        public ObservableCollection<RoomInformation> AvailableRooms
        {
            get => _availableRooms;
            set => SetProperty(ref _availableRooms, value);
        }

        public ObservableCollection<Customer> Customers
        {
            get => _customers;
            set => SetProperty(ref _customers, value);
        }

        public BookingReservation? SelectedBooking
        {
            get => _selectedBooking;
            set
            {
                if (SetProperty(ref _selectedBooking, value) && value != null)
                {
                    // Create a copy for editing
                    NewBooking = new BookingReservation
                    {
                        BookingReservationID = value.BookingReservationID,
                        CustomerID = value.CustomerID,
                        RoomID = value.RoomID,
                        BookingDate = value.BookingDate,
                        StartDate = value.StartDate,
                        EndDate = value.EndDate,
                        BookingDuration = value.BookingDuration,
                        TotalPrice = value.TotalPrice,
                        BookingStatus = value.BookingStatus,
                        BookingType = value.BookingType
                    };
                    IsEditing = true;
                }
            }
        }

        public BookingReservation NewBooking
        {
            get => _newBooking;
            set => SetProperty(ref _newBooking, value);
        }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public int SelectedBookingTypeFilter
        {
            get => _selectedBookingTypeFilter;
            set
            {
                if (SetProperty(ref _selectedBookingTypeFilter, value))
                {
                    FilterByBookingType();
                }
            }
        }

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                if (SetProperty(ref _startDate, value))
                {
                    // Update NewBooking as well
                    NewBooking.StartDate = value;
                    UpdateAvailableRooms();
                    CalculatePrice();
                }
            }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                if (SetProperty(ref _endDate, value))
                {
                    // Update NewBooking as well
                    NewBooking.EndDate = value;
                    UpdateAvailableRooms();
                    CalculatePrice();
                }
            }
        }

        public ICommand AddCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand FilterOnlineCommand { get; }
        public ICommand FilterOfflineCommand { get; }
        public ICommand FilterAllCommand { get; }

        public BookingViewModel()
        {
            _bookingService = new BookingReservationService();
            _roomService = new RoomInformationService();
            _customerService = new CustomerService();
            _roomTypeService = new RoomTypeService();
            _bookings = new ObservableCollection<BookingReservation>();
            _availableRooms = new ObservableCollection<RoomInformation>();
            _customers = new ObservableCollection<Customer>();

            AddCommand = new RelayCommand(_ => ExecuteAdd());
            SaveCommand = new RelayCommand(_ => ExecuteSave());
            DeleteCommand = new RelayCommand(_ => ExecuteDelete(), _ => SelectedBooking != null);
            CancelCommand = new RelayCommand(_ => ExecuteCancel());
            SearchCommand = new RelayCommand(_ => ExecuteSearch());
            RefreshCommand = new RelayCommand(_ => LoadBookings());
            FilterOnlineCommand = new RelayCommand(_ => { SelectedBookingTypeFilter = 1; });
            FilterOfflineCommand = new RelayCommand(_ => { SelectedBookingTypeFilter = 2; });
            FilterAllCommand = new RelayCommand(_ => { SelectedBookingTypeFilter = 0; });

            LoadBookings();
            LoadCustomers();
            UpdateAvailableRooms();
            ResetNewBooking();
        }

        private void LoadBookings()
        {
            var bookings = _bookingService.GetBookings();
            foreach (var booking in bookings)
            {
                booking.Customer = _customerService.GetCustomerByID(booking.CustomerID);
                booking.Room = _roomService.GetRoomByID(booking.RoomID);
            }
            Bookings = new ObservableCollection<BookingReservation>(bookings);
        }

        private void LoadCustomers()
        {
            Customers = new ObservableCollection<Customer>(_customerService.GetCustomers());
        }

        private void UpdateAvailableRooms()
        {
            // Get all active rooms
            var allRooms = _roomService.GetRooms().Where(r => r.RoomStatus == 1).ToList();
            
            // Get all bookings that overlap with the selected date range
            var overlappingBookings = _bookingService.GetBookings().Where(b => 
                b.BookingStatus == 1 && // Only consider active bookings
                ((b.StartDate <= StartDate && b.EndDate > StartDate) || // Booking starts before and ends during/after our period
                 (b.StartDate >= StartDate && b.StartDate < EndDate) || // Booking starts during our period
                 (b.StartDate <= StartDate && b.EndDate >= EndDate) || // Booking completely encompasses our period
                 (b.StartDate >= StartDate && b.EndDate <= EndDate))    // Booking is completely within our period
            ).ToList();
            
            // Get IDs of rooms that are booked during the selected period
            var bookedRoomIds = overlappingBookings.Select(b => b.RoomID).Distinct().ToList();
            
            // Filter out booked rooms
            var availableRooms = allRooms.Where(r => !bookedRoomIds.Contains(r.RoomID)).ToList();
            
            // Load room types for available rooms
            foreach (var room in availableRooms)
            {
                room.RoomType = _roomTypeService.GetRoomTypeByID(room.RoomTypeID);
            }
            
            AvailableRooms = new ObservableCollection<RoomInformation>(availableRooms);
        }

        private void ExecuteSearch()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadBookings();
            }
            else
            {
                // Simple search by customer name or room number
                var bookings = _bookingService.GetBookings();
                var filteredBookings = bookings.Where(b =>
                {
                    var customer = _customerService.GetCustomerByID(b.CustomerID);
                    var room = _roomService.GetRoomByID(b.RoomID);
                    return customer?.CustomerFullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true ||
                           room?.RoomNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true;
                }).ToList();

                foreach (var booking in filteredBookings)
                {
                    booking.Customer = _customerService.GetCustomerByID(booking.CustomerID);
                    booking.Room = _roomService.GetRoomByID(booking.RoomID);
                }

                Bookings = new ObservableCollection<BookingReservation>(filteredBookings);
            }
        }

        private void FilterByBookingType()
        {
            if (SelectedBookingTypeFilter == 0)
            {
                LoadBookings();
            }
            else
            {
                var bookings = _bookingService.GetByBookingType(SelectedBookingTypeFilter);
                foreach (var booking in bookings)
                {
                    booking.Customer = _customerService.GetCustomerByID(booking.CustomerID);
                    booking.Room = _roomService.GetRoomByID(booking.RoomID);
                }
                Bookings = new ObservableCollection<BookingReservation>(bookings);
            }
        }

        private void ExecuteAdd()
        {
            ResetNewBooking();
            IsEditing = false;
        }

        private void ExecuteSave()
        {
            try
            {
                ErrorMessage = string.Empty;

                // Validate data
                if (NewBooking.CustomerID <= 0)
                {
                    ErrorMessage = "Please select a customer.";
                    return;
                }

                if (NewBooking.RoomID <= 0)
                {
                    ErrorMessage = "Please select a room.";
                    return;
                }

                if (NewBooking.StartDate >= NewBooking.EndDate)
                {
                    ErrorMessage = "Check-out date must be after check-in date.";
                    return;
                }

                // Calculate duration and price
                CalculatePrice();

                // Set booking date to today
                NewBooking.BookingDate = DateTime.Now;

                // Save booking
                if (IsEditing)
                {
                    _bookingService.Update(NewBooking);
                }
                else
                {
                    _bookingService.Add(NewBooking);
                }

                // Show confirmation message with booking details
                var room = _roomService.GetRoomByID(NewBooking.RoomID);
                var customer = _customerService.GetCustomerByID(NewBooking.CustomerID);
                var roomType = room.RoomType?.RoomTypeName ?? "Standard";
                
                string confirmationMessage = $"Booking Confirmation\n\n" +
                    $"Booking ID: {(IsEditing ? NewBooking.BookingReservationID : "New Booking")}\n" +
                    $"Customer: {customer?.CustomerFullName}\n" +
                    $"Room: {room?.RoomNumber} ({roomType})\n" +
                    $"Check-in: {NewBooking.StartDate:dd/MM/yyyy}\n" +
                    $"Check-out: {NewBooking.EndDate:dd/MM/yyyy}\n" +
                    $"Duration: {NewBooking.BookingDuration} day(s)\n" +
                    $"Total Price: ${NewBooking.TotalPrice:N2}\n" +
                    $"Booking Type: {(NewBooking.BookingType == 1 ? "Online" : "Offline")}\n\n" +
                    $"Thank you for choosing FU Mini Hotel!";
                
                MessageBox.Show(confirmationMessage, "Booking Confirmed", MessageBoxButton.OK, MessageBoxImage.Information);

                // Refresh booking list
                LoadBookings();
                ResetNewBooking();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
        }

        private void ExecuteDelete()
        {
            if (SelectedBooking == null) return;

            var result = MessageBox.Show($"Are you sure you want to delete this booking?",
                "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _bookingService.Delete(SelectedBooking.BookingReservationID);
                    LoadBookings();
                    MessageBox.Show("Booking deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Error: {ex.Message}";
                }
            }
        }

        private void ExecuteCancel()
        {
            ResetNewBooking();
            IsEditing = false;
        }

        private void ResetNewBooking()
        {
            NewBooking = new BookingReservation
            {
                BookingDate = DateTime.Now,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                BookingStatus = 1,
                BookingType = 1, // Default to Online booking
                BookingDuration = 1,
                TotalPrice = 0
            };
            
            // Synchronize the StartDate and EndDate properties
            _startDate = NewBooking.StartDate;
            _endDate = NewBooking.EndDate;
            OnPropertyChanged(nameof(StartDate));
            OnPropertyChanged(nameof(EndDate));
            
            ErrorMessage = string.Empty;
            IsEditing = false;
            UpdateAvailableRooms();
        }

        private void CalculatePrice()
        {
            if (NewBooking == null) return;

            // Calculate booking duration
            TimeSpan duration = NewBooking.EndDate - NewBooking.StartDate;
            NewBooking.BookingDuration = Math.Max(1, (int)duration.TotalDays); // Ensure at least 1 day

            // Calculate price if a room is selected
            if (NewBooking.RoomID > 0)
            {
                var room = _roomService.GetRoomByID(NewBooking.RoomID);
                if (room != null)
                {
                    NewBooking.TotalPrice = room.RoomPricePerDate * NewBooking.BookingDuration;
                }
            }
            else
            {
                NewBooking.TotalPrice = 0;
            }
        }
    }
} 
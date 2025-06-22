using BusinessObjects;
using Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace huy.ViewModels
{
    public class CustomerBookingViewModel : ViewModelBase
    {
        private readonly BookingReservationService _bookingService;
        private readonly RoomInformationService _roomService;
        private readonly RoomTypeService _roomTypeService;
        private ObservableCollection<RoomInformation> _availableRooms;
        private ObservableCollection<BookingReservation> _customerBookings;
        private RoomInformation? _selectedRoom;
        private BookingReservation _newBooking = new();
        private string _errorMessage = string.Empty;
        private DateTime _startDate = DateTime.Now;
        private DateTime _endDate = DateTime.Now.AddDays(1);
        private bool _hasSelectedRoom;

        public ObservableCollection<RoomInformation> AvailableRooms
        {
            get => _availableRooms;
            set => SetProperty(ref _availableRooms, value);
        }

        public ObservableCollection<BookingReservation> CustomerBookings
        {
            get => _customerBookings;
            set => SetProperty(ref _customerBookings, value);
        }

        public RoomInformation? SelectedRoom
        {
            get => _selectedRoom;
            set
            {
                if (SetProperty(ref _selectedRoom, value) && value != null)
                {
                    NewBooking.RoomID = value.RoomID;
                    HasSelectedRoom = true;
                    CalculatePrice();
                }
            }
        }

        public BookingReservation NewBooking
        {
            get => _newBooking;
            set => SetProperty(ref _newBooking, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                if (SetProperty(ref _startDate, value))
                {
                    NewBooking.StartDate = value;
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
                    NewBooking.EndDate = value;
                    CalculatePrice();
                }
            }
        }

        public bool HasSelectedRoom
        {
            get => _hasSelectedRoom;
            set => SetProperty(ref _hasSelectedRoom, value);
        }

        public ICommand CheckAvailabilityCommand { get; }
        public ICommand SelectRoomCommand { get; }
        public ICommand BookNowCommand { get; }

        public CustomerBookingViewModel()
        {
            _bookingService = new BookingReservationService();
            _roomService = new RoomInformationService();
            _roomTypeService = new RoomTypeService();
            
            // Initialize collections
            _availableRooms = new ObservableCollection<RoomInformation>();
            _customerBookings = new ObservableCollection<BookingReservation>();

            // Set up commands
            CheckAvailabilityCommand = new RelayCommand(_ => UpdateAvailableRooms());
            SelectRoomCommand = new RelayCommand(room => SelectRoom((RoomInformation)room));
            BookNowCommand = new RelayCommand(_ => ExecuteBooking());

            // Initialize data
            ResetNewBooking();
            LoadCustomerBookings(); // Load bookings first
            UpdateAvailableRooms(); // Then update available rooms
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
            
            // Clear selected room if it's no longer available
            if (SelectedRoom != null && !availableRooms.Any(r => r.RoomID == SelectedRoom.RoomID))
            {
                SelectedRoom = null;
                HasSelectedRoom = false;
            }
        }

        private void LoadCustomerBookings()
        {
            // Get bookings for the current customer
            var customerID = App.CurrentCustomerID;
            if (customerID > 0)
            {
                // Get fresh data from the database
                var bookings = _bookingService.GetByCustomer(customerID);
                
                // Make sure we have all the related data loaded
                foreach (var booking in bookings)
                {
                    // Load room data
                    if (booking.Room == null)
                    {
                        booking.Room = _roomService.GetRoomByID(booking.RoomID);
                    }
                    
                    // Load room type data
                    if (booking.Room != null && booking.Room.RoomType == null)
                    {
                        booking.Room.RoomType = _roomTypeService.GetRoomTypeByID(booking.Room.RoomTypeID);
                    }
                }
                
                // Create a new observable collection with the fresh data
                CustomerBookings = new ObservableCollection<BookingReservation>(bookings);
                
                // Force UI update
                OnPropertyChanged(nameof(CustomerBookings));
            }
        }

        private void SelectRoom(RoomInformation room)
        {
            SelectedRoom = room;
            HasSelectedRoom = room != null;
        }

        private void CalculatePrice()
        {
            if (NewBooking == null) return;

            // Calculate booking duration
            TimeSpan duration = NewBooking.EndDate - NewBooking.StartDate;
            NewBooking.BookingDuration = Math.Max(1, (int)duration.TotalDays); // Ensure at least 1 day

            // Calculate price if a room is selected
            if (NewBooking.RoomID > 0 && SelectedRoom != null)
            {
                NewBooking.TotalPrice = SelectedRoom.RoomPricePerDate * NewBooking.BookingDuration;
            }
            else
            {
                NewBooking.TotalPrice = 0;
            }
        }

        private void ExecuteBooking()
        {
            try
            {
                ErrorMessage = string.Empty;

                // Validate data
                if (NewBooking.RoomID <= 0 || SelectedRoom == null)
                {
                    ErrorMessage = "Please select a room.";
                    return;
                }

                if (NewBooking.StartDate >= NewBooking.EndDate)
                {
                    ErrorMessage = "Check-out date must be after check-in date.";
                    return;
                }

                // Set customer ID from the app
                NewBooking.CustomerID = App.CurrentCustomerID;
                if (NewBooking.CustomerID <= 0)
                {
                    ErrorMessage = "You must be logged in to book a room.";
                    return;
                }

                // Calculate duration and price
                CalculatePrice();

                // Set booking date to today
                NewBooking.BookingDate = DateTime.Now;
                
                // Set booking type to Online
                NewBooking.BookingType = 1; // 1: Online

                // Save booking
                _bookingService.Add(NewBooking);

                // Create a copy of the booking for display in the UI
                var savedBooking = new BookingReservation
                {
                    BookingReservationID = NewBooking.BookingReservationID,
                    CustomerID = NewBooking.CustomerID,
                    RoomID = NewBooking.RoomID,
                    BookingDate = NewBooking.BookingDate,
                    StartDate = NewBooking.StartDate,
                    EndDate = NewBooking.EndDate,
                    BookingDuration = NewBooking.BookingDuration,
                    TotalPrice = NewBooking.TotalPrice,
                    BookingStatus = NewBooking.BookingStatus,
                    BookingType = NewBooking.BookingType,
                    Room = SelectedRoom
                };

                // Add the new booking to the CustomerBookings collection
                CustomerBookings.Add(savedBooking);
                OnPropertyChanged(nameof(CustomerBookings));

                // Show confirmation message with booking details
                var room = _roomService.GetRoomByID(NewBooking.RoomID);
                var roomType = room?.RoomType?.RoomTypeName ?? "Standard";
                
                string confirmationMessage = $"Booking Confirmation\n\n" +
                    $"Room: {room?.RoomNumber} ({roomType})\n" +
                    $"Check-in: {NewBooking.StartDate:dd/MM/yyyy}\n" +
                    $"Check-out: {NewBooking.EndDate:dd/MM/yyyy}\n" +
                    $"Duration: {NewBooking.BookingDuration} day(s)\n" +
                    $"Total Price: ${NewBooking.TotalPrice:N2}\n\n" +
                    $"Thank you for choosing FU Mini Hotel!";
                
                MessageBox.Show(confirmationMessage, "Booking Confirmed", MessageBoxButton.OK, MessageBoxImage.Information);

                // Refresh data
                ResetNewBooking();
                UpdateAvailableRooms();
                
                // Force UI to refresh the bookings list
                var temp = CustomerBookings;
                CustomerBookings = null;
                CustomerBookings = temp;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
        }

        private void ResetNewBooking()
        {
            NewBooking = new BookingReservation
            {
                BookingDate = DateTime.Now,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(1),
                BookingStatus = 1,
                BookingType = 1, // Online booking
                BookingDuration = 1,
                TotalPrice = 0
            };
            
            // Synchronize the StartDate and EndDate properties
            _startDate = NewBooking.StartDate;
            _endDate = NewBooking.EndDate;
            OnPropertyChanged(nameof(StartDate));
            OnPropertyChanged(nameof(EndDate));
            
            ErrorMessage = string.Empty;
            HasSelectedRoom = false;
        }
    }
} 
using BusinessObjects;
using Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace huy.ViewModels
{
    public class RoomViewModel : ViewModelBase
    {
        private readonly RoomInformationService _roomService;
        private readonly RoomTypeService _roomTypeService;
        private ObservableCollection<RoomInformation> _rooms;
        private ObservableCollection<RoomType> _roomTypes;
        private RoomInformation? _selectedRoom;
        private string _searchText = string.Empty;
        private string _errorMessage = string.Empty;
        private int? _selectedRoomTypeFilter;

        public ObservableCollection<RoomInformation> Rooms
        {
            get => _rooms;
            set => SetProperty(ref _rooms, value);
        }

        public ObservableCollection<RoomType> RoomTypes
        {
            get => _roomTypes;
            set => SetProperty(ref _roomTypes, value);
        }

        public RoomInformation? SelectedRoom
        {
            get => _selectedRoom;
            set => SetProperty(ref _selectedRoom, value);
        }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public int? SelectedRoomTypeFilter
        {
            get => _selectedRoomTypeFilter;
            set 
            {
                if (SetProperty(ref _selectedRoomTypeFilter, value))
                {
                    FilterRooms();
                }
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand RefreshCommand { get; }

        public RoomViewModel()
        {
            _roomService = new RoomInformationService();
            _roomTypeService = new RoomTypeService();
            _rooms = new ObservableCollection<RoomInformation>();
            _roomTypes = new ObservableCollection<RoomType>();

            AddCommand = new RelayCommand(_ => ExecuteAdd());
            EditCommand = new RelayCommand(_ => ExecuteEdit(), _ => SelectedRoom != null);
            DeleteCommand = new RelayCommand(_ => ExecuteDelete(), _ => SelectedRoom != null);
            SearchCommand = new RelayCommand(_ => ExecuteSearch());
            RefreshCommand = new RelayCommand(_ => LoadRooms());

            LoadRoomTypes();
            LoadRooms();
            
            if (_selectedRoomTypeFilter.HasValue && _selectedRoomTypeFilter.Value > 0)
            {
                FilterRooms();
            }
        }

        private void LoadRooms()
        {
            var rooms = _roomService.GetRooms();
            foreach (var room in rooms)
            {
                room.RoomType = _roomTypeService.GetRoomTypeByID(room.RoomTypeID);
            }
            Rooms = new ObservableCollection<RoomInformation>(rooms);
        }

        private void FilterRooms()
        {
            if (_selectedRoomTypeFilter.HasValue && _selectedRoomTypeFilter.Value > 0)
            {
                var filteredRooms = _roomService.GetRooms().Where(r => r.RoomTypeID == _selectedRoomTypeFilter.Value);
                foreach (var room in filteredRooms)
                {
                    room.RoomType = _roomTypeService.GetRoomTypeByID(room.RoomTypeID);
                }
                Rooms = new ObservableCollection<RoomInformation>(filteredRooms);
            }
            else if (Rooms.Count == 0) // Chỉ load lại khi chưa có dữ liệu
            {
                var rooms = _roomService.GetRooms();
                foreach (var room in rooms)
                {
                    room.RoomType = _roomTypeService.GetRoomTypeByID(room.RoomTypeID);
                }
                Rooms = new ObservableCollection<RoomInformation>(rooms);
            }
        }

        private void LoadRoomTypes()
        {
            // Add "All" option
            var allTypes = new List<RoomType>
            {
                new RoomType { RoomTypeID = 0, RoomTypeName = "All Types" }
            };
            
            // Add actual room types
            allTypes.AddRange(_roomTypeService.GetRoomTypes());
            
            RoomTypes = new ObservableCollection<RoomType>(allTypes);
        }

        private void ExecuteSearch()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                LoadRooms();
            }
            else
            {
                var rooms = _roomService.SearchRooms(SearchText);
                foreach (var room in rooms)
                {
                    room.RoomType = _roomTypeService.GetRoomTypeByID(room.RoomTypeID);
                }
                Rooms = new ObservableCollection<RoomInformation>(rooms);
            }
        }

        private void ExecuteAdd()
        {
            try
            {
                // Create a new room object
                var newRoom = new RoomInformation
                {
                    RoomStatus = 1,
                    RoomMaxCapacity = 2,
                    RoomPricePerDate = 100
                };

                // Show dialog
                var dialog = new RoomDialog(newRoom, RoomTypes, false);
                
                if (dialog.ShowDialog() == true)
                {
                    // Get the room from dialog
                    var room = dialog.ViewModel.Room;
                    
                    // Add to database
                    _roomService.AddRoom(room);
                    
                    // Refresh list
                    LoadRooms();
                    
                    MessageBox.Show("Room added successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
        }

        private void ExecuteEdit()
        {
            if (SelectedRoom == null) return;

            try
            {
                // Create a copy for editing
                var roomToEdit = new RoomInformation
                {
                    RoomID = SelectedRoom.RoomID,
                    RoomNumber = SelectedRoom.RoomNumber,
                    RoomDescription = SelectedRoom.RoomDescription,
                    RoomMaxCapacity = SelectedRoom.RoomMaxCapacity,
                    RoomStatus = SelectedRoom.RoomStatus,
                    RoomPricePerDate = SelectedRoom.RoomPricePerDate,
                    RoomTypeID = SelectedRoom.RoomTypeID
                };

                // Show dialog
                var dialog = new RoomDialog(roomToEdit, RoomTypes, true);
                
                if (dialog.ShowDialog() == true)
                {
                    // Get the room from dialog
                    var room = dialog.ViewModel.Room;
                    
                    // Update in database
                    _roomService.UpdateRoom(room);
                    
                    // Refresh list
                    LoadRooms();
                    
                    MessageBox.Show("Room updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error: {ex.Message}";
            }
        }

        private void ExecuteDelete()
        {
            if (SelectedRoom == null) return;

            var result = MessageBox.Show($"Are you sure you want to delete room {SelectedRoom.RoomNumber}?",
                "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    _roomService.DeleteRoom(SelectedRoom.RoomID);
                    LoadRooms();
                    MessageBox.Show("Room deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Error: {ex.Message}";
                }
            }
        }
    }
} 
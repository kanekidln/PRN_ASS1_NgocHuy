using BusinessObjects;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace huy.ViewModels
{
    public class RoomDialogViewModel : ViewModelBase
    {
        private RoomInformation _room;
        private ObservableCollection<RoomType> _roomTypes;
        private string _dialogTitle;
        private string _errorMessage = string.Empty;
        private bool _isEditing;

        public event EventHandler<bool> RequestClose;

        public RoomInformation Room
        {
            get => _room;
            set => SetProperty(ref _room, value);
        }

        public ObservableCollection<RoomType> RoomTypes
        {
            get => _roomTypes;
            set => SetProperty(ref _roomTypes, value);
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

        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public RoomDialogViewModel(RoomInformation room, ObservableCollection<RoomType> roomTypes, bool isEditing)
        {
            _isEditing = isEditing;
            _room = room ?? new RoomInformation();
            _roomTypes = roomTypes;
            
            DialogTitle = isEditing ? "Edit Room" : "Add New Room";
            
            SaveCommand = new RelayCommand(_ => ExecuteSave());
            CancelCommand = new RelayCommand(_ => ExecuteCancel());
        }

        private void ExecuteSave()
        {
            try
            {
                ErrorMessage = string.Empty;

                // Validate data
                if (string.IsNullOrWhiteSpace(Room.RoomNumber))
                {
                    ErrorMessage = "Room number is required.";
                    return;
                }

                if (Room.RoomMaxCapacity <= 0)
                {
                    ErrorMessage = "Max capacity must be greater than 0.";
                    return;
                }

                if (Room.RoomPricePerDate <= 0)
                {
                    ErrorMessage = "Price per date must be greater than 0.";
                    return;
                }

                if (Room.RoomTypeID <= 0)
                {
                    ErrorMessage = "Room type is required.";
                    return;
                }

                // If this is a new room, set status to active
                if (!IsEditing)
                {
                    Room.RoomStatus = 1;
                }

                // Close dialog with success result
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
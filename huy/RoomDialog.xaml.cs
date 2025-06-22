using BusinessObjects;
using System.Collections.ObjectModel;
using System.Windows;

namespace huy
{
    public partial class RoomDialog : Window
    {
        public ViewModels.RoomDialogViewModel ViewModel { get; private set; }

        public RoomDialog(RoomInformation room, ObservableCollection<RoomType> roomTypes, bool isEditing)
        {
            InitializeComponent();
            ViewModel = new ViewModels.RoomDialogViewModel(room, roomTypes, isEditing);
            DataContext = ViewModel;
            
            // Handle dialog result
            ViewModel.RequestClose += (sender, result) =>
            {
                DialogResult = result;
                Close();
            };
        }
    }
} 
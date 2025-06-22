using BusinessObjects;
using System.Windows;

namespace huy
{
    public partial class CustomerDialog : Window
    {
        public ViewModels.CustomerDialogViewModel ViewModel { get; private set; }

        public CustomerDialog(Customer customer, bool isEditing)
        {
            InitializeComponent();
            ViewModel = new ViewModels.CustomerDialogViewModel(customer, isEditing);
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
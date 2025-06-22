using System.Windows;

namespace huy
{
    public partial class CustomerProfile : Window
    {
        public CustomerProfile()
        {
            InitializeComponent();
            DataContext = new ViewModels.CustomerProfileViewModel();
        }
    }
}
using MartineobotIOTMvvm.ViewModels;

namespace MartineobotIOTMvvm.Views
{
    public sealed partial class MenuPage
    {
       
        public MenuPage()
        {
            InitializeComponent();
            Loaded += async delegate { await ViewModel.CallOnLoaded(); };
            Unloaded += async delegate { await ViewModel.CallOnUnloaded(); };
        }
        private MenuViewModel ViewModel => (MenuViewModel)Resources["ViewModel"];
    }
}

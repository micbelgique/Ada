using AdaW10.ViewModels;

namespace AdaW10.Views
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

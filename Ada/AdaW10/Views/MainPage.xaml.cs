using AdaW10.ViewModels;

namespace AdaW10.Views
{
    
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();

            Loaded += async delegate { await ViewModel.CallOnLoaded(); };
            Unloaded += async delegate { await ViewModel.CallOnUnloaded(); };
        }

        private MainViewModel ViewModel => (MainViewModel) Resources["ViewModel"];
    }
}

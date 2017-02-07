using MartineobotIOTMvvm.ViewModels;

namespace MartineobotIOTMvvm.Views
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

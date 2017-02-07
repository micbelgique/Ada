using MartineobotIOTMvvm.ViewModels;

namespace MartineobotIOTMvvm.Views
{
    public sealed partial class SwitchPage
    {
        public SwitchPage()
        {
            InitializeComponent();
            Loaded += async delegate { await ViewModel.CallOnLoaded(); };
            Unloaded += async delegate { await ViewModel.CallOnUnloaded(); };

        }

        private SwitchViewModel ViewModel => (SwitchViewModel) Resources["ViewModel"];
    }
}

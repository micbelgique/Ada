using AdaW10.ViewModels;

namespace AdaW10.Views
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

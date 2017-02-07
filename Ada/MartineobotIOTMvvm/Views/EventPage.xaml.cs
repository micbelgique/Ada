using MartineobotIOTMvvm.ViewModels;

namespace MartineobotIOTMvvm.Views
{
    public sealed partial class EventPage
    {
        public EventPage()
        {
            InitializeComponent();
            Loaded += async delegate { await ViewModel.CallOnLoaded(); };
            Unloaded += async delegate { await ViewModel.CallOnUnloaded(); };
        }
        private EventViewModel ViewModel => (EventViewModel)Resources["ViewModel"];
    }
}

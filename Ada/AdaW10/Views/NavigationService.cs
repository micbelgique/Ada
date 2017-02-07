using GalaSoft.MvvmLight.Messaging;

namespace AdaW10.Views
{
    public class NavigationService : GalaSoft.MvvmLight.Views.NavigationService, Models.Interfaces.INavigationService
    {
        public void NavigateTo<T>(string pageKey, T parameter)
        {
            base.NavigateTo(pageKey, parameter);
            Messenger.Default.Send(parameter);
        }

        public new void NavigateTo(string pageKey, object parameter)
        {
            NavigateTo<object>(pageKey, parameter);
        }
    }
}

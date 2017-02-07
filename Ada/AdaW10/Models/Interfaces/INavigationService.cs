namespace AdaW10.Models.Interfaces
{
    /// <summary>
    /// An interface defining how navigation between pages 
    /// </summary>
    public interface INavigationService : GalaSoft.MvvmLight.Views.INavigationService
    {
        //
        // Summary:
        //     Instructs the navigation service to display a new page corresponding to the given
        //     key, and passes a parameter to the new page. Depending on the platforms, the
        //     navigation service might have to be Configure with a key/page list.
        //
        // Parameters:
        //   pageKey:
        //     The key corresponding to the page that should be displayed.
        //
        //   parameter:
        //     The parameter that should be passed to the new page.
        void NavigateTo<T>(string pageKey, T parameter);
    }
}

using GalaSoft.MvvmLight.Ioc;
using MartineobotIOTMvvm.Models;
using MartineobotIOTMvvm.Models.EventsLoaderServices;
using MartineobotIOTMvvm.Models.Interfaces;
using MartineobotIOTMvvm.Models.VoiceInterface;
using Microsoft.Practices.ServiceLocation;

namespace MartineobotIOTMvvm.ViewModels
{
    public class ViewModelLocator
    {
        public const string MainPage = "Main";
        public const string SwitchPage = "Switch";
        public const string MenuPage = "Menu";
        public const string EventPage = "Event";

        public ViewModelLocator()
        {
            // SimpleIoC
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            
            // Models
            if (GalaSoft.MvvmLight.ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IDataService, Design.DesignDataService>();
                SimpleIoc.Default.Register<IDialogService, Design.DesignDialogService>();
                SimpleIoc.Default.Register<INavigationService, Design.DesignNavigationService>();
            }
            else
            {
                SimpleIoc.Default.Register<IDataService, DataService>();
                SimpleIoc.Default.Register<IDialogService, Views.DialogService>();
                SimpleIoc.Default.Register(CreateNavigationService);
            }

            SimpleIoc.Default.Register<WebcamService>();
            SimpleIoc.Default.Register<VoiceInterface>();
            SimpleIoc.Default.Register<EventsLoaderService>();

            //View Models
            SimpleIoc.Default.Register<SwitchViewModel>();
            SimpleIoc.Default.Register<EventViewModel>();
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<MenuViewModel>();
        }

        private INavigationService CreateNavigationService()
        {
            var navigationService = new Views.NavigationService();
            navigationService.Configure(SwitchPage, typeof(Views.SwitchPage));
            navigationService.Configure(MainPage, typeof(Views.MainPage));
            navigationService.Configure(MenuPage, typeof (Views.MenuPage));
            navigationService.Configure(EventPage, typeof(Views.EventPage));
            return navigationService;
        }
    }
}

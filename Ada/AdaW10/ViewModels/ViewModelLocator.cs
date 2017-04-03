using GalaSoft.MvvmLight.Ioc;
using AdaW10.Models;
using AdaW10.Models.EventsLoaderServices;
using AdaW10.Models.Interfaces;
using AdaW10.Models.VoiceInterface;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Bot.Connector.DirectLine;
using AdaW10.Views;

namespace AdaW10.ViewModels
{
    public class ViewModelLocator
    {
        public const string MainPage = "Main";
        public const string MenuPage = "Menu";
        public const string EventPage = "Event";
        public const string CarouselPage = "Carousel";

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
                SimpleIoc.Default.Register<IDialogService, DialogService>();
                SimpleIoc.Default.Register(CreateNavigationService);
            }

            SimpleIoc.Default.Register<WebcamService>();
            SimpleIoc.Default.Register<VoiceInterface>();
            SimpleIoc.Default.Register<EventsLoaderService>();

            //View Models
            SimpleIoc.Default.Register<EventViewModel>();
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<MenuViewModel>();
            SimpleIoc.Default.Register<CarouselViewModel>();
        }

        private INavigationService CreateNavigationService()
        {
            var navigationService = new Views.NavigationService();
            navigationService.Configure(MainPage, typeof(Views.MainPage));
            navigationService.Configure(MenuPage, typeof (Views.MenuPage));
            navigationService.Configure(EventPage, typeof(Views.EventPage));
            navigationService.Configure(CarouselPage, typeof(Views.CarouselPage));
            return navigationService;
        }

        public CarouselViewModel Carousel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CarouselViewModel>();
            }
        }
    }
}

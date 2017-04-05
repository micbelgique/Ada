using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using AdaW10.Messages;
using AdaW10.Models.VoiceInterface;
using Microsoft.Practices.ServiceLocation;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using AdaSDK;
using AdaW10.Models.EventsLoaderServices;
using System.Collections.Generic;
using Microsoft.Bot.Connector.DirectLine;
using System;
using AdaW10.Models.Carousel;
using Newtonsoft.Json;
using System.Diagnostics;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Ioc;
using Windows.UI.Core;
using System.Xml;
using System.Collections.ObjectModel;

namespace AdaW10.ViewModels
{
    public class CarouselViewModel : ViewModelBase
    {
        public CarouselViewModel()
        {
            NavigationRegistering<PersonDto>();
            DispatcherHelper.Initialize();

            VoiceInterface = ServiceLocator.Current.GetInstance<VoiceInterface>();
            EventsLoaderService = ServiceLocator.Current.GetInstance<EventsLoaderService>();

            GoBackToMainPageCommand = new RelayCommand(async () => await GoBackToMainExecute());
        }

        public RelayCommand GoBackToMainPageCommand { get; set; }

        public IEventsLoaderService EventsLoaderService { get; set; }

        public RelayCommand GoToCarouselPageCommand { get; set; }
        public VoiceInterface VoiceInterface { get; set; }

        private List<Attachment> _attachments;

        public List<Attachment> Attachments
        {
            get { return _attachments; }
            set { Set(() => Attachments, ref _attachments, value); }
        }

        public async void OnNavigatedTo(NavigationEventArgs e)
        {
            Attachments = (List<Attachment>)e.Parameter;
            await OnLoadedAsync();
        }

        private List<Carousel> _carouselList;
        public List<Carousel> CarouselList
        {
            get { return _carouselList; }
            set { Set(() => CarouselList, ref _carouselList, value); }
        }

        protected override async Task OnLoadedAsync()
        {
            List<Carousel> ListCarousel = new List<Carousel>();
            await RunTaskAsync(async () =>
            {
                string _carousel;
                foreach (Attachment attachment in Attachments)
                {
                    _carousel = JsonConvert.SerializeObject(attachment.Content);
                    ListCarousel.Add(JsonConvert.DeserializeObject<Carousel>(_carousel));
                }
                await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                         () =>
                         {
                             CarouselList = ListCarousel;
                         }
                         );
            })
            .ContinueWith(async task =>
            {
                await VoiceInterface.ListeningCancellation();

                Messenger.Default.Register<SpeechResultGeneratedMessage>(this, async e =>
                {
                    if (e.Result.Constraint.Tag == "constraint_abord_words")
                    {
                        await DispatcherHelper.RunAsync(async () => await GoBackToMainExecute());
                    }
                });
            });
        }


        private async Task GoBackToMainExecute()
        {
            // Cleans up services and messenger
            await VoiceInterface.StopListening();
            Messenger.Default.Unregister(this);

            NavigationService.NavigateTo(ViewModelLocator.MainPage);

        }
    }
}

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
        }

        public IEventsLoaderService EventsLoaderService { get; set; }

        public RelayCommand GoToCarouselPageCommand { get; set; }
        public VoiceInterface VoiceInterface { get; set; }

        public IList<string> Attachments { get; set; }

        public async Task OnNavigatedTo(NavigationEventArgs e)
        {
            await Task.Run(() => Attachments = (IList<string>)e.Parameter);
        }

        private List<Carousel> _carouselList;
        public List<Carousel> CarouselList
        {
            get { return _carouselList; }
            set { Set(() => CarouselList, ref _carouselList, value); }
        }

        public Carousel Carouseltest { get; set; }

        protected override async Task OnLoadedAsync()
        {
            await RunTaskAsync(() =>
            {
                Carouseltest = JsonConvert.DeserializeObject<Carousel>(Convert.ToString(Attachments[0]));
            })
            .ContinueWith(async task =>
            {
                if (!task.IsFaulted)
                {
                    await VoiceInterface.SayEventsAvailable();
                }

                await VoiceInterface.ListeningCancellation();

                Messenger.Default.Register<SpeechResultGeneratedMessage>(this, async e =>
                {
                    if (e.Result.Constraint.Tag == "constraint_abord_words")
                    {
                        if (e.Result.Text.Contains("au-revoir") || e.Result.Text.Contains("bonne journée"))
                            await DispatcherHelper.RunAsync(async () => await GoBackToMainExecute());
                    }
                });
            });
        }

        private async Task GoBackToMainExecute()
        {
            // Cleans up services and messenger
            await VoiceInterface.StopListening();
            await VoiceInterface.SayGoodBye();
            Messenger.Default.Unregister(this);

            NavigationService.NavigateTo(ViewModelLocator.MainPage);
        }
    }
}

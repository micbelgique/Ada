using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using AdaSDK;
using AdaW10.Messages;
using AdaW10.Models.EventsLoaderServices;
using AdaW10.Models.VoiceInterface;
using Microsoft.Practices.ServiceLocation;

namespace AdaW10.ViewModels
{
    public class EventViewModel : ViewModelBase
    {
        public EventViewModel()
        {
            NavigationRegistering<PersonDto>();
            DispatcherHelper.Initialize();

            VoiceInterface = ServiceLocator.Current.GetInstance<VoiceInterface>();
            EventsLoaderService = ServiceLocator.Current.GetInstance<EventsLoaderService>();

            GoBackToMenuPageCommand = new RelayCommand(async () => await GoBackToMenuExecute());
        }

        #region Services, Commands and Properties

        // Services
        public VoiceInterface VoiceInterface { get; set; }
        public IEventsLoaderService EventsLoaderService { get; set; }

        // Commands
        public RelayCommand GoBackToMenuPageCommand { get; set; }

        // Properties
        private List<MeetupEvent> _eventList;
        public List<MeetupEvent> EventList
        {
            get { return _eventList; }
            set { Set(() => EventList, ref _eventList, value); }
        }
        // Properties
        public PersonDto CurrentPerson { get; set; }
        #endregion

        #region Events
        protected override async Task OnNavigationFrom(object parameter)
        {
            await Task.Run(() => CurrentPerson = (PersonDto)parameter);
        }
        protected override async Task OnLoadedAsync()
        {
            await RunTaskAsync(async () =>
            {
                EventList = await EventsLoaderService.GetEventsJsonAsync(10);
            })
            .ContinueWith(async task =>
            {
                if (!task.IsFaulted){
                    await VoiceInterface.SayEventsAvailable();
                }
            
                await VoiceInterface.ListeningCancellation();

                Messenger.Default.Register<SpeechResultGeneratedMessage>(this, async e =>
                {
                    if (e.Result.Constraint.Tag == "constraint_abord_words")
                    {
                        if(e.Result.Text.Contains("au-revoir") || e.Result.Text.Contains("bonne journée"))
                            await DispatcherHelper.RunAsync(async () => await GoBackToMainExecute());
                        else
                            await DispatcherHelper.RunAsync(async () => await GoBackToMenuExecute());
                    }
                });
            });
        }


        #endregion

        #region Actions

        // Navigation actions
        private async Task GoBackToMainExecute()
        {
            // Cleans up services and messenger
            await VoiceInterface.StopLinstening();
            await VoiceInterface.SayGoodBye();
            Messenger.Default.Unregister(this);

            NavigationService.NavigateTo(ViewModelLocator.MainPage);
        }
        private async Task GoBackToMenuExecute()
        {
            // Cleans up services and messenger
            await VoiceInterface.StopLinstening();
            Messenger.Default.Unregister(this);
            NavigationService.NavigateTo(ViewModelLocator.MenuPage, CurrentPerson);
        }
        #endregion
    }
}

using System;
using System.Diagnostics;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using AdaW10.Messages;
using AdaW10.Models.VoiceInterface;
using Microsoft.Practices.ServiceLocation;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using AdaSDK;
using AdaW10.Models;

namespace AdaW10.ViewModels
{
    public class MenuViewModel : ViewModelBase
    {
        public MenuViewModel()
        {
            DispatcherHelper.Initialize();
            NavigationRegistering<PersonDto>();

            VoiceInterface = ServiceLocator.Current.GetInstance<VoiceInterface>();
            WebcamService = ServiceLocator.Current.GetInstance<WebcamService>();

            GoToEventPageCommand = new RelayCommand(async () => await GoToEventPageExecute());
            GoToSandwichPageCommand = new RelayCommand(async () => await GoToSandwichPageExecute());
            GoToReservationPageCommand = new RelayCommand(async () => await GoToReservationPageExecute());
            DescribeCommand = new RelayCommand(async () => await DescribeExecute());
            CallSomeoneCommand = new RelayCommand(async () => await CallSomeoneExecute());
            GoBackToMainPageCommand = new RelayCommand(async () => await GoBackToMainPageExecute());
        }

        #region Services, Commands and Properties

        // Services
        public VoiceInterface VoiceInterface { get; }
        public WebcamService WebcamService { get; }

        // Commands
        public RelayCommand GoToEventPageCommand { get; set; }
        public RelayCommand GoToSandwichPageCommand { get; set; }
        public RelayCommand GoToReservationPageCommand { get; set; }
        public RelayCommand DescribeCommand { get; set; }
        public RelayCommand CallSomeoneCommand { get; set; }
        public RelayCommand GoBackToMainPageCommand { get; set; }
        
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
            await VoiceInterface.ListeningWhatToDo();

            Messenger.Default.Register<SpeechResultGeneratedMessage>(this, async e =>
            {
                await DispatcherHelper.RunAsync(async () =>
                {
                    switch (e.Result.Constraint.Tag)
                    {
                        case "constraint_events": await GoToEventPageExecute();
                        break;

                        case "constraint_description":
                            await DescribeExecute();
                            break;

                        case "constraint_abord_words":
                            await GoBackToMainPageExecute();
                            break;

                        case "constraint_sandwich":
                            await GoToSandwichPageExecute();
                            break;

                        case "constraint_calling":
                            await CallSomeoneExecute();
                            break;

                        case "constraint_reservation":
                            await GoToReservationPageExecute();
                            break;
                    }
                });
            });
        }

        #endregion

        #region Actions

        // Navigations
        public async Task GoToEventPageExecute()
        {
            await RunTaskAsync(async () =>
            {
                await VoiceInterface.StopListening();
                Messenger.Default.Unregister(this);
                NavigationService.NavigateTo(ViewModelLocator.EventPage, CurrentPerson);
            });
        }

        public async Task GoToSandwichPageExecute()
        { 
            await RunTaskAsync(async () =>
            {
                await VoiceInterface.StopListening();
                await VoiceInterface.SayNotAvailableService();
                await VoiceInterface.ListeningWhatToDo();
            });
        }

        public async Task GoToReservationPageExecute()
        {
            await RunTaskAsync(async () => 
            {
                await VoiceInterface.StopListening();
                await VoiceInterface.SayNotAvailableService();
                await VoiceInterface.ListeningWhatToDo();
            });
        }

        public async Task GoBackToMainPageExecute()
        {
            await RunTaskAsync(async () =>
            {
                await VoiceInterface.StopListening();
                await VoiceInterface.SayGoodBye();
                Messenger.Default.Unregister(this);

                NavigationService.NavigateTo(ViewModelLocator.MainPage);
            });
        }

        // General actions
        public async Task DescribeExecute()
        {
            await RunTaskAsync(async () =>
            {
                await VoiceInterface.StopListening();
                await VoiceInterface.SayDescriptionOfSomeone(CurrentPerson);
                await VoiceInterface.ListeningWhatToDo();
            });
        }

        public async Task CallSomeoneExecute()
        {
            await RunTaskAsync(async () =>
            {
                await VoiceInterface.StopListening();
                await VoiceInterface.SayNotAvailableService();
                await VoiceInterface.ListeningWhatToDo();
            });
        }

        #endregion
    }
}

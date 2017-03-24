using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using AdaSDK;
using AdaW10.Helper;
using AdaW10.Messages;
using AdaW10.Models;
using AdaW10.Models.VoiceInterface;
using AdaW10.Models.VoiceInterface.TextToSpeech;
using Microsoft.Practices.ServiceLocation;

namespace AdaW10.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            DispatcherHelper.Initialize();

            WebcamService = ServiceLocator.Current.GetInstance<WebcamService>(); 
            VoiceInterface = ServiceLocator.Current.GetInstance<VoiceInterface>();            
            //    SelectModeCommand = new RelayCommand<ModeValues>(OnSelectMode);
        }

        #region Services, Commands and Properties

        // Services
        public WebcamService WebcamService { get; }
        public VoiceInterface VoiceInterface { get; }
        
        // Properties
        private string _logMessage;
        public string LogMessage
        {
            get { return _logMessage; }
            set { Set(() => LogMessage, ref _logMessage, value); }
        }

        private CaptureElement _captureElement;

        public CaptureElement CaptureElement
        {
            get { return _captureElement; }
            set { Set(() => CaptureElement, ref _captureElement, value);  }
        }
        
        #endregion

        #region Events
        
        protected override async Task OnLoadedAsync()
        {
            // Registers to messenger for on screen log messages
            Messenger.Default.Register<LogMessage>(this, async e => await DispatcherHelper.RunAsync(() => LogMessage += e.Message));
            
            // Begins to listening "hello ada"
            await VoiceInterface.ListeningHelloAda();

            // Registers to messenger to catch messages when a speech recognition result
            // was generated
            Messenger.Default.Register<SpeechResultGeneratedMessage>(this, async e =>
            {
                if (e.Result.Constraint.Tag == "constraint_hello_ada")
                {
                    await DispatcherHelper.RunAsync(async () => { await SolicitExecute(); });
                }
            });

            // Prepares capture element to camera feed and load camera
            CaptureElement = new CaptureElement();
            await CameraLoadExecute();
        }

        private async void OnFaceDetected(FaceDetectionEffect sender, FaceDetectedEventArgs args)
        {
            if (!IsLoading && args.ResultFrame.DetectedFaces.Any())
            {
                await DispatcherHelper.RunAsync(async () =>
                {
                    await RunTaskAsync(async () =>
                    {
                        var persons = await MakeRecognition();
                        // LogHelper.Log("Recognition ended");
                        await VoiceInterface.SayHelloAsync(persons);
                        // LogHelper.Log("Seaking ended");
                    });
                });
            }
        }

        #endregion

        #region Actions

        private async Task CameraLoadExecute()
        {
            LogHelper.Log<WebcamService>("Je me mets au travail !");

            await WebcamService.InitializeCameraAsync();
            WebcamService.CaptureElement = CaptureElement;
            await WebcamService.StartCameraPreviewAsync();
            
            if (WebcamService.IsInitialized && await WebcamService.StartFaceDetectionAsync(300))
            {
               WebcamService.FaceDetectionEffect.FaceDetected += OnFaceDetected;        
            }
        }

        private async Task SolicitExecute()
        {
            LogHelper.Log("Je suis à toi dans un instant...");

            await RunTaskAsync(async () =>
            {
                // Arrêt de l'écoute continue, sinon le programme plante lors de l'écoute du nom ou de la raison
                await WebcamService.StopFaceDetectionAsync();
                await VoiceInterface.StopListening();

                var person = (await MakeRecognition())?.FirstOrDefault();

                if (person != null)
                {
                    bool update = false;
                    PersonUpdateDto updateDto = new PersonUpdateDto
                    {
                        PersonId = person.PersonId,
                        RecognitionId = person.RecognitionId
                    };

                    await VoiceInterface.SayHelloAsync(person);

                    // Update person's name
                    if (person.FirstName == null)
                    {
                        string name = await VoiceInterface.AskNameAsync();

                        if (name == null)
                        {
                            return;
                        }

                        updateDto.FirstName = name;
                        person.FirstName = name;
                        update = true;
                    }

                    // Update person's visit
                    if (person.ReasonOfVisit == null)
                    {
                        string reason = await VoiceInterface.AskReason();
                        updateDto.ReasonOfVisit = reason;
                        person.ReasonOfVisit = reason;
                        update = true;
                    }

                    if (update)
                    {
                        await DataService.UpdatePersonInformation(updateDto);
                    }

                    if (person.FirstName != null && person.ReasonOfVisit != null)
                    {
                        await GoToMenuPage(person);
                    }
                    else
                    {
                        await VoiceInterface.ListeningHelloAda();
                    }
                }
                else
                {
                    await VoiceInterface.ListeningHelloAda();
                }
            });


        }

        public async Task<PersonDto[]> MakeRecognition()
        {
            using (var stream = new InMemoryRandomAccessStream())
            {
                await WebcamService.MediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), stream);
                await stream.FlushAsync();
                stream.Seek(0);

                try
                {
                //    LogHelper.Log("Je vois quelqu'un :-) Qui est-ce ?");
                    PersonDto[] persons = await DataService.RecognizePersonsAsync(stream.AsStreamForRead());
                    // Logs results on screen
                    if (persons != null) LogHelper.LogPersons(persons);
                    if (persons == null) LogHelper.Log("Ho, j'ai cru voir quelqu'un :'(");

                    return persons;
                }
                catch (HttpRequestException)
                {
                    await TtsService.SayAsync("Veuillez m'excuser je ne suis pas disponible pour le moment. Veuillez ré-éssayer dans quelque secondes");
                    await Task.Delay(5000);
                    return null;
                }
            }
        }

        // Navigations

        public async Task GoToMenuPage(PersonDto person)
        {
            // Clean up services and messenger
            await VoiceInterface.StopListening();
            await WebcamService.CleanUpAsync();
            Messenger.Default.Unregister(this);

            NavigationService.NavigateTo(ViewModelLocator.MenuPage, person);
        }
        
        #endregion       
    }
}

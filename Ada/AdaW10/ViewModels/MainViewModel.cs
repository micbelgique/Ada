using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using AdaSDK;
using AdaW10.Helper;
using AdaW10.Messages;
using AdaW10.Models;
using AdaW10.Models.VoiceInterface;
using AdaW10.Models.VoiceInterface.TextToSpeech;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Bot.Connector.DirectLine;
using System.Collections.Generic;
using System.Net;
using GalaSoft.MvvmLight.Views;
using AdaW10.Views;
using GalaSoft.MvvmLight.Command;
using Windows.UI.Core;
using System.Threading;
using AdaSDK.Models;

namespace AdaW10.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private DirectLineClient _client;
        private Conversation _conversation;

        public MainViewModel()
        {
            DispatcherHelper.Initialize();

            WebcamService = ServiceLocator.Current.GetInstance<WebcamService>();
            VoiceInterface = ServiceLocator.Current.GetInstance<VoiceInterface>();
        }

        public RelayCommand GoToCarouselPageCommand { get; set; }

        public async Task GoToCarouselPageExecute(IList<Attachment> attachments)
        {
            await RunTaskAsync(async () =>
            {
                await VoiceInterface.StopListening();
                Messenger.Default.Unregister(this);
                NavigationService.NavigateTo(ViewModelLocator.CarouselPage, attachments);
            });
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
            set { Set(() => CaptureElement, ref _captureElement, value); }
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
                    LogHelper.Log("Message reçu ;)");
                    LogHelper.Log("Je suis à toi dans un instant");

                    PersonDto person = null;

                    var test = WebcamService.FaceDetectionEffect;

                    if(test != null)
                    {
                        await WebcamService.StopFaceDetectionAsync();
                        person = (await MakeRecognition())?.FirstOrDefault();
                    }
                    await VoiceInterface.StopListening();

                    if (person != null)
                    {                      
                        PersonUpdateDto updateDto = new PersonUpdateDto
                        {
                            PersonId = person.PersonId,
                            RecognitionId = person.RecognitionId
                        };

                        await VoiceInterface.SayHelloAsync(person);

                        // Update person's name
                        if (person.FirstName == null)
                        {
                            string answer = await VoiceInterface.AskIdentified();

                            if (answer != "non")
                            {
                                string name = await VoiceInterface.AskNameAsync();

                                if (name == null)
                                {
                                    return;
                                }

                                updateDto.FirstName = name;
                                person.FirstName = name;
                            }
                        }
                    }

                    await TtsService.SayAsync("En quoi puis je t'aider ?");
                    await DispatcherHelper.RunAsync(async () => { await SolicitExecute(); });
                }
            });

            _client = new DirectLineClient(AppConfig.DirectLine);
            _conversation = (await _client.Conversations.StartConversationWithHttpMessagesAsync()).Body;

            int timeout = 10;
            var task = ReadBotMessagesAsync(_client, _conversation.ConversationId);
            await Task.WhenAny(task, Task.Delay(timeout));
            

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
                        await VoiceInterface.SayHelloAsync(persons);
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
            LogHelper.Log("Que puis-je faire pour toi ?");

            var str = await VoiceInterface.Listen();
            LogHelper.Log(str);

            var activity = new Activity
            {
                From = new ChannelAccount("Jean"),
                Text = str,
                Type = ActivityTypes.Message
            };

            if (activity.Text == "")
            {
                await TtsService.SayAsync("au revoir");

                await VoiceInterface.ListeningHelloAda();
            }
            else
            {
                activity.Text = (activity.Text).Replace('.', ' ');
                activity.Text = (activity.Text).ToLower();

                if(activity.Text == "oui ")
                {
                    activity.Text = "yes";
                }
                if (activity.Text == "non ")
                {
                    activity.Text = "no";
                }

                await _client.Conversations.PostActivityAsync(_conversation.ConversationId, activity);
            }
        }

        private async Task ReadBotMessagesAsync(DirectLineClient client, string conversationId)
        {
            string watermark = null;

            while (true)
            {
                ActivitySet activitySet = await client.Conversations.GetActivitiesAsync(conversationId, watermark);

                watermark = activitySet?.Watermark;

                var activities = from x in activitySet.Activities
                                 where x.From.Id != "Jean"
                                 select x;
                var enumerable = activities as IList<Activity> ?? activities.ToList();
                foreach (Activity activity in enumerable)
                {
                    var text = WebUtility.HtmlDecode(activity.Text);
                    var attachments = activity.Attachments;

                    if (attachments.Count > 0)
                    {
                        var token = new CancellationTokenSource();

                        await VoiceInterface.StopListening();
                        await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                         async () =>
                       {
                           await WebcamService.CleanUpAsync();
                           await GoToCarouselPageExecute(attachments);
                       }
                       );
                    } 
                    LogHelper.Log(text);
                    await TtsService.SayAsync(text);
                }

                if (enumerable.Count > 0)
                {
                    await SolicitExecute();
                }
            }
        }

        public async Task<PersonDto[]> MakeRecognition()
        {
            AdaClient client = new AdaClient() { WebAppUrl = AppConfig.WebUri };
            using (var stream = new InMemoryRandomAccessStream())
            {
                await WebcamService.MediaCapture.CapturePhotoToStreamAsync(ImageEncodingProperties.CreateJpeg(), stream);
                await stream.FlushAsync();
                stream.Seek(0);

                try
                {
                    PersonDto[] persons = await DataService.RecognizePersonsAsync(stream.AsStreamForRead());
                    // Logs results on screen
                    if (persons != null)
                    {
                        LogHelper.LogPersons(persons);

                        foreach (PersonDto person in persons)
                        {
                            Guid faceApi = person.PersonId;
                            var personMessage = await client.GetPersonByFaceId(faceApi);
                            List<MessageDto> messages = await client.GetMessageByReceiver(personMessage.PersonId);
                            foreach (MessageDto message in messages)
                            {
                                await TtsService.SayAsync("Bonjour" + person.FirstName + "Tu as un nouveau message de la part de " + message.From);
                                await TtsService.SayAsync(message.Contenu);
                                message.IsRead = true;
                                message.Read = DateTime.Now;

                                await client.PutMessage(message);
                            }
                        }
                    }
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

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
using GalaSoft.MvvmLight.Command;
using Windows.UI.Core;
using System.Threading;
using AdaSDK.Models;
using Websockets;
using System.Diagnostics;
using Newtonsoft.Json;
using Websockets.Universal;
using Windows.Storage;
using AdaSDK.Services;

namespace AdaW10.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private static MainViewModel _instance;
        static readonly object instanceLock = new object();

        private DirectLineClient _client;
        private Conversation _conversation;
        private CaptureElement _captureElement;
        private IWebSocketConnection connection;
        private bool _isDirectLineInitialized;
        private string _logMessage;

        public MainViewModel()
        {
            DispatcherHelper.Initialize();

            WebcamService = ServiceLocator.Current.GetInstance<WebcamService>();
            VoiceInterface = ServiceLocator.Current.GetInstance<VoiceInterface>();

            InitializeDirectLine();
        }

        private async void InitializeDirectLine()
        {
            _client = new DirectLineClient(AppConfig.DirectLine);
            _conversation = (await _client.Conversations.StartConversationWithHttpMessagesAsync()).Body;

            //Register the UWP as a Client UWP in database
            var response = _client.Conversations.PostActivity(_conversation.ConversationId, new Activity("message")
            {
                From = new ChannelAccount("Jean"),
                Text = "RegisterApp",
            });

            //WEBSOCKET HERE
            WebsocketConnection.Link();
            connection = WebSocketFactory.Create();
            connection.OnLog += Connection_OnLog;
            connection.OnError += Connection_OnError;
            connection.OnOpened += Connection_OnOpened;

            connection.Open(_conversation.StreamUrl);

            _isDirectLineInitialized = true;
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
        public string LogMessage
        {
            get { return _logMessage; }
            set { Set(() => LogMessage, ref _logMessage, value); }
        }

        public CaptureElement CaptureElement
        {
            get { return _captureElement; }
            set { Set(() => CaptureElement, ref _captureElement, value); }
        }

        #endregion

        #region Events

        protected override async Task OnLoadedAsync()
        {
            await Task.Run(() => { while (!_isDirectLineInitialized) { } });

            connection.OnMessage += Connection_OnMessage;

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
                    if (VoiceInterface != null)
                    {
                        await VoiceInterface.StopListening();
                    }

                    LogHelper.Log("Message reçu ;)");
                    LogHelper.Log("Je suis à toi dans un instant");
                    await TtsService.SayAsync("Message reçu, je suis à toi dans un instant");

                    PersonDto person = null;

                    if (WebcamService.FaceDetectionEffect != null)
                    {
                        await WebcamService.StopFaceDetectionAsync();
                        person = (await MakeRecognition())?.FirstOrDefault();
                    }

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
                    else
                    {
                        await TtsService.SayAsync("Bonjour");
                    }
                    await DispatcherHelper.RunAsync(async () => { await SolicitExecute(); });
                }
            });

            //// Prepares capture element to camera feed and load camera
            CaptureElement = new CaptureElement();
            await CameraLoadExecute();
        }

        protected override async Task OnUnloadedAsync()
        {
            connection.OnMessage -= Connection_OnMessage;
            Messenger.Default.Unregister(this);
        }

        private void Connection_OnOpened()
        {
            Debug.WriteLine("Opened !");
        }

        private async void Connection_OnMessage(string obj)
        {
            if (string.IsNullOrWhiteSpace(obj))
                return;

            ActivitySet activitySet = JsonConvert.DeserializeObject<ActivitySet>(obj);

            foreach (var activity in activitySet.Activities)
            {
                if (activity.From.Id == "Jean")
                    continue;

                switch (activity.Text)
                {
                    case "take picture":
                        await TakePicture(activity.Conversation.Id, activity.ServiceUrl);
                        return;
                    case "registered":
                        return;
                    default:
                        HandleActivity(activity);
                        return;
                }
            }
        }

        private async void HandleActivity(Activity activity)
        {
            var text = WebUtility.HtmlDecode(activity.Text);
            var attachments = activity.Attachments;

            if (attachments?.Count > 0)
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

            if (activity.Name == "End")
            {
                connection.OnMessage -= Connection_OnMessage;

                if (WebcamService.FaceDetectionEffect != null)
                {
                    await WebcamService.StopFaceDetectionAsync();
                }

                if (WebcamService.IsInitialized && await WebcamService.StartFaceDetectionAsync(300))
                {
                    WebcamService.FaceDetectionEffect.FaceDetected += OnFaceDetected;
                }

                await VoiceInterface.ListeningHelloAda();
            }
            else if (activity.Name != "NotFinish")
            {
                await DispatcherHelper.RunAsync(async () => { await SolicitExecute(); });
            }
        }

        private void Connection_OnError(string obj)
        {
            Debug.Write("ERROR " + obj);
        }

        private void Connection_OnLog(string obj)
        {
            Debug.Write(obj);
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
            LogHelper.Log<WebcamService>("Je me mets au travail!");

            await WebcamService.InitializeCameraAsync();
            WebcamService.CaptureElement = CaptureElement;

            await WebcamService.StartCameraPreviewAsync();

            if (WebcamService.IsInitialized && await WebcamService.StartFaceDetectionAsync(300))
            {
                WebcamService.FaceDetectionEffect.FaceDetected += OnFaceDetected;
            }
        }

        private async Task TakePicture(string conversID, string serviceUrl)
        {
            if (!WebcamService.IsInitialized)
            {
                await WebcamService.InitializeCameraAsync();
                await WebcamService.StartCameraPreviewAsync();
            }

            using (var stream = new InMemoryRandomAccessStream())
            {
                ImageEncodingProperties imgFormat = ImageEncodingProperties.CreatePng();

                // create storage file in local app storage
                StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                    "AdaPhotoTMP.png",
                    CreationCollisionOption.ReplaceExisting);

                // take photo
                await WebcamService.MediaCapture.CapturePhotoToStorageFileAsync(imgFormat, file);

                FileStream fileStream = new FileStream(file.Path, FileMode.Open);
                Stream streamFinal = fileStream.AsRandomAccessStream().AsStream();

                StringConstructorSDK client = new StringConstructorSDK() { WebAppUrl = $"{ AppConfig.WebUri}" };

                try
                {
                    var activity = new Activity
                    {
                        From = new ChannelAccount("Jean"),
                        Text = "Picture from UWP",
                        Type = ActivityTypes.Message,
                        //Envoyer le stream
                        ChannelData = await client.PictureAnalyseAsync(AppConfig.Vision, streamFinal),
                        Name = conversID,
                        //Summary = serviceUrl
                    };
                    await _client.Conversations.PostActivityAsync(_conversation.ConversationId, activity);
                }
                catch (HttpRequestException)
                {
                    //Impossible to take picture
                }
            }
        }

        private async Task SolicitExecute()
        {
            if (WebcamService.FaceDetectionEffect != null)
            {
                await WebcamService.StopFaceDetectionAsync();
            }

            LogHelper.Log("Que puis-je faire pour toi?");
            await TtsService.SayAsync("Que puis-je faire pour toi?");

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

                connection.OnMessage -= Connection_OnMessage;

                if (WebcamService.FaceDetectionEffect != null)
                {
                    await WebcamService.StopFaceDetectionAsync();
                }

                if (WebcamService.IsInitialized && await WebcamService.StartFaceDetectionAsync(300))
                {
                    WebcamService.FaceDetectionEffect.FaceDetected += OnFaceDetected;
                }

                await VoiceInterface.ListeningHelloAda();
            }
            else
            {
                activity.Text = (activity.Text).Replace('.', ' ');
                activity.Text = (activity.Text).ToLower();

                await _client.Conversations.PostActivityAsync(_conversation.ConversationId, activity);
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
                            if (personMessage != null)
                            {
                                List<MessageDto> messages = await client.GetMessageByReceiver(personMessage.PersonId);

                                foreach (MessageDto message in messages)
                                {
                                    if (message.From != null)
                                    {
                                        await TtsService.SayAsync("Bonjour" + person.FirstName + "Tu as un nouveau message de la part de " + message.From);
                                    }
                                    else
                                    {
                                        await TtsService.SayAsync("Bonjour" + person.FirstName + "Tu as un nouveau message ");
                                    }
                                    await TtsService.SayAsync(message.Contenu);
                                    message.IsRead = true;
                                    message.Read = DateTime.Now;

                                    await client.PutMessage(message);
                                }

                                List<IndicatePassageDto> indicatePassages = await client.GetIndicatePassageByPerson(personMessage.PersonId);

                                foreach (IndicatePassageDto indicatePassage in indicatePassages)
                                {
                                    // need to send message to the person on facebook
                                    try
                                    {
                                        var activity = new Activity
                                        {
                                            From = new ChannelAccount("Jean"),
                                            Type = ActivityTypes.Message,
                                            Text = "Passage person from UWP",
                                            ChannelData = indicatePassage.Firtsname,
                                            Name = "Passage person from UWP"
                                        };

                                        await _client.Conversations.PostActivityAsync(indicatePassage.IdFacebookConversation, activity);

                                    }
                                    catch (HttpRequestException)
                                    {
                                        //Impossible to take picture
                                    }

                                    indicatePassage.IsSend = true;
                                    await client.PutIndicatePassage(indicatePassage);
                                }
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

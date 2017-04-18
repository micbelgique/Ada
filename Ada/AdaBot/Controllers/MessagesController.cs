using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using AdaBot.Dialogs;
using Microsoft.Bot.Builder.Luis;
using System.Configuration;
using AdaSDK;
using AdaSDK.Models;
using Microsoft.ProjectOxford.Vision;
using System.Diagnostics;
using System.IO;
using AdaBot.Services;
using Microsoft.ProjectOxford.Vision.Contract;
using Newtonsoft.Json;
using System.Web.Http;
using Microsoft.ProjectOxford.Face;
using System.Collections.Generic;
using AdaBot.Models;
using System.Text;

using Yam.Microsoft.Translator.TranslatorService;
namespace AdaBot
{
    [BotAuthentication]
    public class MessagesController : ApiController 
    {
        string visionApiKey;

        VisionServiceClient visionClient;
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary> 
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            bool answer = true;

            AdaClient client = new AdaClient() { WebAppUrl = $"{ ConfigurationManager.AppSettings["WebAppUrl"] }" };

            visionApiKey = ConfigurationManager.AppSettings["VisionApiKey"];

            //Vision SDK classes
            visionClient = new VisionServiceClient(visionApiKey);

            string accessAllow;
            string idUser;

            if (activity.ServiceUrl == "https://facebook.botframework.com")
            {
                idUser = activity.From.Id;
                accessAllow = await client.CheckIdFacebook(idUser);

                if (accessAllow == "false")
                {
                    UserIndentifiedDto userIndentified = new UserIndentifiedDto();
                    string nameUser = activity.From.Name + " ";
                    string[] nameUserSplit;
                    nameUserSplit = nameUser.Split(' ');

                    userIndentified.IdFacebook = idUser;
                    userIndentified.Firtsname = nameUserSplit[0];
                    var nbNameSplit = nameUserSplit.Count();
                    string lastName = "";
                    for (int i = 1; i < nbNameSplit; i++)
                    {
                        lastName += nameUserSplit[i] + " ";
                    }
                    userIndentified.LastName = lastName;
                    userIndentified.authorization = false;

                    var respond = await client.AddNewUserIndentified(userIndentified);
                }
            }

            if(activity.ServiceUrl == "https://slack.botframework.com" && !activity.Text.Contains("ada"))
            {
                answer = false;
            }

            if (activity.Type == ActivityTypes.Message)
            {
                DataService dataService = new DataService();

                if (activity.Attachments?.Count() >= 1)
                {
                    if (activity.Attachments[0].ContentType == "image/png" || activity.Attachments[0].ContentType == "image/jpeg" || activity.Attachments[0].ContentType == "image/jpg")
                    {
                        StringBuilder reply = new StringBuilder();

                        try
                        {
                            VisionService visionService = new VisionService(activity);
                            VisualFeature[] visualFeatures = new VisualFeature[] {
                                        VisualFeature.Adult, //recognize adult content
                                        VisualFeature.Categories, //recognize image features
                                        VisualFeature.Description //generate image caption
                                        };
                            AnalysisResult analysisResult = null;
                            string description = "";
                            GoogleTranslatorService translator = new GoogleTranslatorService();
                            //If the user uploaded an image, read it, and send it to the Vision API
                            if (activity.Attachments.Any() && activity.Attachments.First().ContentType.Contains("image"))
                            {
                                //stores image url (parsed from attachment or message)
                                string uploadedImageUrl = activity.Attachments.First().ContentUrl;

                                List<PersonVisitDto> person = new List<PersonVisitDto>();

                                using (Stream imageFileStream = GetStreamFromUrl(uploadedImageUrl))
                                {
                                    try
                                    {
                                        analysisResult = await visionClient.AnalyzeImageAsync(imageFileStream, visualFeatures);
                                        reply.Append("Je vois: " + analysisResult.Description.Captions.First().Text + ". ");

                                        if (analysisResult.Description.Tags.Contains("person"))                                          
                                        {
                                            imageFileStream.Seek(0, SeekOrigin.Begin);

                                            PersonDto[] persons = await dataService.recognizepersonsPictureAsync(imageFileStream);

                                            reply.Append("Il y a " + persons.Count() + " personne(s) sur la photo. ");

                                            foreach (PersonDto result in persons)
                                            {
                                                if(result.FirstName != null)
                                                {
                                                    reply.Append("Je connais " + result.FirstName + ", cette personne a " + result.Age + "ans.");
                                                }
                                                else
                                                {
                                                    reply.Append("Je ne connais malheureusement pas cette personne mais elle a " + result.Age + "ans.");
                                                }

                                                person.Add(await client.GetPersonByFaceId(result.PersonId));
                                            }
                                        }
                                        description = translator.TranslateText(analysisResult.Description.Captions[0].Text.ToString(), "en|fr");
                                    }
                                    catch (Exception e)
                                    {
                                        analysisResult = null; //on error, reset analysis result to null
                                    }
                                }
                            }
                            var reply = activity.CreateReply(description);

                            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                            await connector.Conversations.ReplyToActivityAsync(activity.CreateReply(reply.ToString()));
                            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
                        }
                        catch(ClientException e)
                        {
                            Debug.WriteLine(e.Error.Message);
                        }
                        catch(Exception e)
                        {
                            Debug.WriteLine(e.Message);
                        }
                    }
                }

                if (answer)
                {
                    idUser = activity.From.Id;
                    accessAllow = await client.GetAuthorizationFacebook(idUser);
                    if (accessAllow == "false")
                    {
                        await Conversation.SendAsync(activity, () => new NotAllowedAdaDialog(
                        new LuisService(new LuisModelAttribute(
                        ConfigurationManager.AppSettings["ModelId"],
                        ConfigurationManager.AppSettings["SubscriptionKey"]))));
                    }
                    else
                    {
                        await Conversation.SendAsync(activity, () => new AdaDialog(
                        new LuisService(new LuisModelAttribute(
                        ConfigurationManager.AppSettings["ModelId"],
                        ConfigurationManager.AppSettings["SubscriptionKey"]))));
                    }
                }
            }
            else
            {
                //add code to handle errors, or non-messaging activities
            }

            return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }
            return null;
        }
        private static Stream GetStreamFromUrl(string url)
        {
            byte[] imageData = null;

            using (var wc = new System.Net.WebClient())
                imageData = wc.DownloadData(url);

            return new MemoryStream(imageData);
        }
    }
}
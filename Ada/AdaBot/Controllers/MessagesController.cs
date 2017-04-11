using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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

namespace AdaBot
{
    [BotAuthentication]
    public class MessagesController : ApiController 
    {
        string visionApiKey;
        string FaceApiKey;

        VisionServiceClient visionClient;
        FaceServiceClient faceClient;
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary> 
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            bool answer = true;

            AdaClient client = new AdaClient() { WebAppUrl = $"{ ConfigurationManager.AppSettings["WebAppUrl"] }" };

            visionApiKey = ConfigurationManager.AppSettings["VisionApiKey"];
            visionApiKey = ConfigurationManager.AppSettings["FaceApiKey"];

            //Vision SDK classes
            visionClient = new VisionServiceClient(visionApiKey);
            faceClient = new FaceServiceClient(visionApiKey);
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
                if (activity.Attachments?.Count() >= 1)
                {
                    if (activity.Attachments[0].ContentType == "image/png" || activity.Attachments[0].ContentType == "image/jpeg" || activity.Attachments[0].ContentType == "image/jpg")
                    {
                        try
                        {
                            VisionService visionService = new VisionService(activity);
                            VisualFeature[] visualFeatures = new VisualFeature[] {
                                        VisualFeature.Adult, //recognize adult content
                                        VisualFeature.Categories, //recognize image features
                                        VisualFeature.Description //generate image caption
                                        };
                            AnalysisResult analysisResult = null;

                            //If the user uploaded an image, read it, and send it to the Vision API
                            if (activity.Attachments.Any() && activity.Attachments.First().ContentType.Contains("image"))
                            {
                                Guid faceId;

                                //stores image url (parsed from attachment or message)
                                string uploadedImageUrl = activity.Attachments.First().ContentUrl; 

                                using (Stream imageFileStream = GetStreamFromUrl(uploadedImageUrl))
                                {
                                    try
                                    {
                                        var resultFace = await faceClient.DetectAsync(imageFileStream);

                                        foreach(Microsoft.ProjectOxford.Face.Contract.Face result in resultFace)
                                        {
                                            faceId = result.FaceId;

                                            var personMessage = await client.GetPersonByFaceId(faceId);
                                        }

                                        analysisResult = await visionClient.AnalyzeImageAsync(imageFileStream, visualFeatures);
                                    }
                                    catch (Exception e)
                                    {
                                        analysisResult = null; //on error, reset analysis result to null
                                    }
                                }
                            }
                            var reply = activity.CreateReply("Je vois: " + analysisResult.Description.Captions.First().Text);

                            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                            await connector.Conversations.ReplyToActivityAsync(reply);
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
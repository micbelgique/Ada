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
using AdaBot.Services;


namespace AdaBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        public static string serviceUrl;
        public static ChannelAccount from;
        public static ChannelAccount botAccount;
        public static ConversationAccount conversation;

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary> 
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            bool answer = true;

            AdaClient client = new AdaClient() { WebAppUrl = $"{ ConfigurationManager.AppSettings["WebAppUrl"] }" };

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

            if (activity.ServiceUrl == "https://slack.botframework.com" && !activity.Text.Contains("ada"))
            {
                answer = false;
            }

            if (activity.Type == ActivityTypes.Message)
            {
                if (activity.Text == "RegisterApp")
                {
                    // persist this information
                    serviceUrl = activity.ServiceUrl;
                    from = activity.From;
                    botAccount = activity.Recipient;
                    conversation = activity.Conversation;

                    ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                    await connector.Conversations.ReplyToActivityAsync(activity.CreateReply("registered"));
                    return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
                }
                if (activity.Text == "Picture from UWP")
                {
                    CommunicationService communicationLine = new CommunicationService();
                    string[] logs = activity.Name.ToString().Split('|');
                    await communicationLine.SendProactiveMessageFacebook(logs[1], logs[2], logs[0], activity.ChannelData.ToString());
                }

                if (activity.Text == "Passage person from UWP")
                {
                    answer = false;
                    activity.Conversation.Id = Convert.ToString(activity.ChannelData);
                    ConnectorClient connector = new ConnectorClient(new Uri("https://facebook.botframework.com"));
                    await connector.Conversations.SendToConversationAsync((Activity)activity.ChannelData);
                }

                if (activity.Attachments?.Count() >= 1)
                {
                    if (activity.Attachments[0].ContentType == "image/png" || activity.Attachments[0].ContentType == "image/jpeg" || activity.Attachments[0].ContentType == "image/jpg")
                    {
                        StringConstructor stringConstructor = new StringConstructor();
                        try
                        {
                            await stringConstructor.PictureAnalyseAsync(activity);
                            answer = false;
                        }
                        catch (ClientException e)
                        {
                            Debug.WriteLine(e.Error.Message);
                        }
                        catch (Exception e)
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
    }
}
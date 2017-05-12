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
using System.Collections.Generic;
using AdaBot.Answers;
using AdaBot.Bot.Utils;
using System.Web;

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
            ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
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

                    await connector.Conversations.ReplyToActivityAsync(activity.CreateReply("registered"));
                    return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
                }
                if (activity.Text == "Picture from UWP")
                {
                    answer = false;
                    CommunicationService communicationLine = new CommunicationService();
                    string[] logs = activity.Name.ToString().Split('|');
                    if (logs[3] == "Facebook")
                    {
                        await communicationLine.SendProactiveMessageFacebook(logs[1], logs[2], logs[0], activity.ChannelData.ToString());
                    }
                    else
                    {
                        await communicationLine.SendProactiveMessageSlack(logs[1], logs[2], logs[0], activity.ChannelData.ToString());
                    }
                }

                if (activity.Text == "Passage person from UWP")
                {
                    answer = false;
                    CommunicationService communicationLine = new CommunicationService();
                    string[] logs = activity.Name.ToString().Split('|');
                    if (logs[3] == "facebook" || logs[3] == "Facebook")
                    {
                        await communicationLine.SendProactiveMessageFacebook(logs[1], logs[2], logs[0], activity.ChannelData.ToString());
                    }
                    else
                    {
                        await communicationLine.SendProactiveMessageSlack(logs[1], logs[2], logs[0], activity.ChannelData.ToString());
                    }
                }

                if (activity.Text.Contains("ConfirmationIdentityFace "))
                {
                    answer = false;
                    var splitResult = activity.Text.Split(':');

                    VisitDto lastVisit = new VisitDto();
                    Activity replyToConversation;
                    TreatmentDialog treatment = new TreatmentDialog();

                    List<VisitDto> visits = await client.GetVisitPersonById(Convert.ToInt32(splitResult[1]), 1);
                    lastVisit = visits.Last();
                    ProfilePictureDto picture = lastVisit.ProfilePicture.Last();
                    string response = treatment.describe(lastVisit, picture);
                    replyToConversation = activity.CreateReply(response);
                    await connector.Conversations.ReplyToActivityAsync(replyToConversation);
                }
                else if (activity.Text.Contains("ChoosePersonId"))
                {
                    answer = false;
                    var splitResult = activity.Text.Split(':');

                    int nbVisit = 10;
                    Activity replyToConversation;
                    replyToConversation = activity.CreateReply($"{Dialog.Waiting.Spintax()}");
                    replyToConversation.Recipient = activity.From;
                    replyToConversation.Name = "NotFinish";
                    replyToConversation.Type = "message";
                    await Post(replyToConversation);

                    int idPerson = Convert.ToInt32(splitResult[1]);

                    nbVisit = Convert.ToInt32(splitResult[3]);

                    List<VisitDto> visitsById = await client.GetVisitPersonById(idPerson, nbVisit);

                    replyToConversation = activity.CreateReply($"{Dialog.VisitsPerson.Spintax()}");
                    replyToConversation.Recipient = activity.From;
                    replyToConversation.Type = "message";
                    replyToConversation.AttachmentLayout = "carousel";
                    replyToConversation.Attachments = new List<Attachment>();

                    foreach (var visit in visitsById)
                    {
                        List<CardImage> cardImages = new List<CardImage>();
                        cardImages.Add(new CardImage(url: $"{ ConfigurationManager.AppSettings["WebAppUrl"] }{VirtualPathUtility.ToAbsolute(visit.ProfilePicture.Last().Uri)}"));

                        var customDialog = new CreateDialog();
                        var messageDate = customDialog.GetVisitsMessage(visit.PersonVisit.FirstName, visit.Date.AddHours(2));

                        HeroCard plCard = new HeroCard()
                        {
                            Title = visit.PersonVisit.FirstName,
                            Text = messageDate,
                            Images = cardImages
                        };

                        Attachment plAttachment = plCard.ToAttachment();
                        replyToConversation.Attachments.Add(plAttachment);
                    }
                    await connector.Conversations.ReplyToActivityAsync(replyToConversation);
                }
                else if (activity.Text.Contains("IndicatePassage"))
                {
                    answer = false;
                    var splitResult = activity.Text.Split('|');

                    Activity replyToConversation;
                    IndicatePassageDto indicatePassage = new IndicatePassageDto();

                    int personId = Convert.ToInt32(splitResult[1]);

                    indicatePassage.IdFacebookConversation = activity.Conversation.Id;
                    indicatePassage.To = personId;
                    indicatePassage.Firtsname = splitResult[2];
                    indicatePassage.FromId = activity.From.Id;
                    indicatePassage.RecipientID = activity.Recipient.Id;
                    indicatePassage.Channel = activity.ChannelId;
                    await client.AddIndicatePassage(indicatePassage);

                    replyToConversation = activity.CreateReply("Bien je le ferai.");
                    replyToConversation.Recipient = activity.From;
                    replyToConversation.Type = "message";

                    await connector.Conversations.ReplyToActivityAsync(replyToConversation);
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
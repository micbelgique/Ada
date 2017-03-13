using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using Microsoft.Bot.Builder.Dialogs;
using AdaBot.Dialogs;
using Microsoft.Bot.Builder.Luis;
using System.Configuration;
using AdaSDK;
using AdaSDK.Models;
using Newtonsoft.Json.Linq;

namespace AdaBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary> 
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            AdaClient client = new AdaClient();
            var idUser = activity.From.Id;
            var accessAllow = await client.CheckIdFacebook(idUser);

            if(Convert.ToString(accessAllow) == "false")
            {
                UserIndentifiedDto userIndentified = new UserIndentifiedDto();
                string nameUser = activity.From.Name + " ";
                string[] nameUserSplit;
                nameUserSplit  = nameUser.Split(' ');

                userIndentified.IdFacebook = idUser;
                userIndentified.Firtsname = nameUserSplit[0];
                var nbNameSplit = nameUserSplit.Count();
                string lastName ="";
                for(int i = 1; i < nbNameSplit ; i++)
                {
                    lastName +=nameUserSplit[i]+ " ";
                }
                userIndentified.LastName = lastName;
                userIndentified.authorization = false;

                var respond = await client.AddNewUserIndentified(userIndentified);
            }

            if (activity.Type == ActivityTypes.Message)
            {
                activity.Text = activity.Text.Replace("?",""); 

                await Conversation.SendAsync(activity, () => new AdaDialog(
                    new LuisService(new LuisModelAttribute(
                        ConfigurationManager.AppSettings["ModelId"],
                        ConfigurationManager.AppSettings["SubscriptionKey"]))));
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
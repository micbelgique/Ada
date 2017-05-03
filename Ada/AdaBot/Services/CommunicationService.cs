using Microsoft.Bot.Connector;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AdaBot.Services
{
    public class CommunicationService
    {
        public async Task SendProactiveMessageSlack(string userId, string botId, string conversationId, string text)
        {
            const string slackConnector = "https://slack.botframework.com";
            MicrosoftAppCredentials.TrustServiceUrl(slackConnector);
            var recipient = new ChannelAccount(userId.ToString());
            var account = new ChannelAccount(botId); //ID BOT

            var connector = new ConnectorClient(new Uri(slackConnector), ConfigurationManager.AppSettings["MicrosoftAppId"],
                                                ConfigurationManager.AppSettings["MicrosoftAppPassword"]);

            var msg = Activity.CreateMessageActivity();
            msg.Type = ActivityTypes.Message;
            msg.From = account;
            msg.Recipient = recipient;
            msg.ChannelId = "slack";
            var conversation = conversationId; //ID de la conversation
            msg.Conversation = new ConversationAccount(id: conversation);
            msg.Text = text;
            await connector.Conversations.SendToConversationAsync((Activity)msg);
        }

        public async Task SendProactiveMessageFacebook(string userId, string botId, string conversationId, string text)
        {
            const string facebookConnector = "https://facebook.botframework.com";
            MicrosoftAppCredentials.TrustServiceUrl(facebookConnector);
            var recipient = new ChannelAccount(userId.ToString());
            var account = new ChannelAccount(botId); //ID BOT

            var connector = new ConnectorClient(new Uri(facebookConnector), ConfigurationManager.AppSettings["MicrosoftAppId"],
                                                ConfigurationManager.AppSettings["MicrosoftAppPassword"]);

            var msg = Activity.CreateMessageActivity();
            msg.Type = ActivityTypes.Message;
            msg.From = account;
            msg.Recipient = recipient;
            msg.ChannelId = "facebook";
            var conversation = conversationId; //ID de la conversation
            msg.Conversation = new ConversationAccount(id: conversation);
            msg.Text = text;
            await connector.Conversations.SendToConversationAsync((Activity)msg);
        }
    }
}
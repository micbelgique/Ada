using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AdaSDK;
using System.Net.Http;
using System.Configuration;
using AdaSDK.Models;
using AdaBot.BotFrameworkHelpers;
using System.Threading;
using System.Drawing;

namespace AdaBot.Dialogs
{
    [Serializable]
    public class AdaDialogNotAllowed : LuisDialog<object>
    {
        public AdaDialogNotAllowed(params ILuisService[] services) : base(services)
        {

        }

        protected override async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            var message = (Activity)await item;
            await base.MessageReceived(context, item);
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.Forward(new TrivialLuisDialog(new LuisService(new LuisModelAttribute(
                           ConfigurationManager.AppSettings["ModelIdTrivial"],
                           ConfigurationManager.AppSettings["SubscriptionKeyTrivial"]))),
                   BasicCallback, context.Activity as Activity, CancellationToken.None);
        }

        [LuisIntent("GetHelp")]
        public async Task GetHelp(IDialogContext context, LuisResult result)
        {
            Activity replyToConversation;
            replyToConversation = ((Activity)context.Activity).CreateReply("En quoi puis-je t'aider? :)");
            replyToConversation.Recipient = context.Activity.From;
            replyToConversation.Type = "message";
            replyToConversation.AttachmentLayout = "carousel";
            replyToConversation.Attachments = new List<Attachment>();

            List<string> pictures = new List<string>();
            pictures.Add("http://i.imgur.com/c8iWoTH.jpg?1");
            pictures.Add("http://i.imgur.com/2TsS2aq.png");
            pictures.Add("http://i.imgur.com/gjQkeXr.png");

            List<string> btnAction = new List<string>();
            btnAction.Add(ConfigurationManager.AppSettings["FaceBookMIC"]);
            btnAction.Add(ConfigurationManager.AppSettings["YoutubeMIC"]);
            btnAction.Add(ConfigurationManager.AppSettings["MeetupMIC"]);

            List<string> btnString = new List<string>();
            btnString.Add("Notre Facebook");
            btnString.Add("Notre chaîne Youtube");
            btnString.Add("Notre Meetup");

            for (int i = 0; i < btnAction.Count(); i++)
            {
                List<CardAction> cardsAction = new List<CardAction>();
                CardAction action = new CardAction()
                {
                    Value = btnAction[i].ToString(),
                    Type = "openUrl",
                    Title = btnString[i]
                };
                cardsAction.Add(action);
                List<CardImage> cardsImage = new List<CardImage>();
                CardImage img = new CardImage(url: pictures[i]);
                cardsImage.Add(img);

                HeroCard tmp = new HeroCard()
                {
                    Images = cardsImage,
                    Buttons = cardsAction
                };
                Attachment plAttachment = tmp.ToAttachment();
                replyToConversation.Attachments.Add(plAttachment);
            }
            await context.PostAsync(replyToConversation);
            context.Wait(MessageReceived);
        }

        private async Task BasicCallback(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("GetVisitsToday")]
        public async Task GetVisitsToday(IDialogContext context, LuisResult result)
        {
            
        }

        [LuisIntent("GetLastVisitPerson")]
        public async Task GetLastVisitPerson(IDialogContext context, LuisResult result)
        {
           

        }

        [LuisIntent("GetStatsVisits")]
        public async Task GetLastVisGetStatsVisitsitPerson(IDialogContext context, LuisResult result)
        {
         
        }

        [LuisIntent("GetAverageVisits")]
        public async Task GetAverageVisits(IDialogContext context, LuisResult result)
        {
           
        }
    }
}
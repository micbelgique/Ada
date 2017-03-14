using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdaSDK;
using System.Net.Http;
using System.Configuration;
using System.Threading;
using AdaBot.Answers;
using AdaBot.Bot.Utils;
using AdaBot.Models.EventsLoaderServices;

namespace AdaBot.Dialogs
{
    [Serializable]
    public class NotAllowedAdaDialog : LuisDialog<object>
    {
        public NotAllowedAdaDialog(params ILuisService[] services) : base(services)
        {

        }

        protected override async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            AdaClient client = new AdaClient();
            var idUser = context.Activity.From.Id;

            var accessAllow = await client.GetAuthorizationFacebook(idUser);
            if (accessAllow == "true")
            {
                await context.Forward(new AdaDialog(
                new LuisService(new LuisModelAttribute(
                ConfigurationManager.AppSettings["ModelId"],
                ConfigurationManager.AppSettings["SubscriptionKey"]))),
                BasicCallback, context.Activity as Activity, CancellationToken.None);
            }
            else
            {
                var message = (Activity)await item;
                await base.MessageReceived(context, item);
            }       
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.Forward(new TrivialLuisDialog(new LuisService(new LuisModelAttribute(
                           ConfigurationManager.AppSettings["ModelIdTrivial"],
                           ConfigurationManager.AppSettings["SubscriptionKeyTrivial"]))),
                   BasicCallback, context.Activity as Activity, CancellationToken.None);
        }

        [LuisIntent("Possibilities")]
        public async Task Possibilities(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Je suis capable de te renseigner sur pas mal de chose! :D Tu peux me demander:");
            await context.PostAsync("- la liste des visites du jour");
            await context.PostAsync("- des informations concernant les visiteurs du MIC (âge, sexe, ...)");
            await context.PostAsync("- le nombre de visiteurs moyen du MIC");
            await context.PostAsync("- la liste des évènements du MIC");
            await context.PostAsync("- des informations concernant ma maison, le MIC");
        }

        [LuisIntent("What'sUp")]
        public async Task Event(IDialogContext context, LuisResult result)
        {
            List<MeetupEvent> _eventList = new List<MeetupEvent>();
            TreatmentDialog treatment = new TreatmentDialog();
            _eventList = await treatment.getEvents();

            Activity replyToConversation;
            replyToConversation = ((Activity)context.Activity).CreateReply("Voici la liste des évènements à venir au MIC: ");
            replyToConversation.Recipient = context.Activity.From;
            replyToConversation.Type = "message";
            replyToConversation.AttachmentLayout = "carousel";
            replyToConversation.Attachments = new List<Attachment>();

            foreach (var meetup in _eventList)
            {
                List<CardImage> cardImages = new List<CardImage>();
                cardImages.Add(new CardImage(url: $"{ConfigurationManager.AppSettings["IMGMIC"]}"));

                List<CardAction> cardsAction = new List<CardAction>();
                CardAction action = new CardAction()
                {
                    Value = meetup.Link,
                    Type = "openUrl",
                    Title = "Consulter"
                };
                cardsAction.Add(action);

                DateTime date = new DateTime(1970, 1, 1).Add(TimeSpan.FromMilliseconds((meetup.Time))).AddHours(2);

                HeroCard plCard = new HeroCard()
                {
                    Title = meetup.Name + " (" + date + ")",
                    Text = "Lieux: " + meetup.Venue.Name + " " + meetup.Venue.City,
                    //Subtitle = Regex.Replace(meetup.Description, @"<(.|\n)*?>", string.Empty),
                    Subtitle = meetup.HowToFind,
                    Images = cardImages,
                    Buttons = cardsAction
                };

                Attachment plAttachment = plCard.ToAttachment();
                replyToConversation.Attachments.Add(plAttachment);
            }

            await context.PostAsync(replyToConversation);
            context.Wait(MessageReceived);
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
            pictures.Add(ConfigurationManager.AppSettings["IMGFacebook"]);
            pictures.Add(ConfigurationManager.AppSettings["IMGYoutube"]);
            pictures.Add(ConfigurationManager.AppSettings["IMGMeetup"]);
            pictures.Add(ConfigurationManager.AppSettings["IMGMIC"]);

            List<string> btnAction = new List<string>();
            btnAction.Add(ConfigurationManager.AppSettings["FaceBookMIC"]);
            btnAction.Add(ConfigurationManager.AppSettings["YoutubeMIC"]);
            btnAction.Add(ConfigurationManager.AppSettings["MeetupMIC"]);
            btnAction.Add(ConfigurationManager.AppSettings["SiteMIC"]);

            List<string> btnString = new List<string>();
            btnString.Add("Notre Facebook");
            btnString.Add("Notre chaîne Youtube");
            btnString.Add("Notre Meetup");
            btnString.Add("Notre Site");

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
            string message = $"{Dialog.NotAllowed.Spintax()}";

            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("GetLastVisitPerson")]
        public async Task GetLastVisitPerson(IDialogContext context, LuisResult result)
        {
            string message = $"{Dialog.NotAllowed.Spintax()}";

            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("GetStatsVisits")]
        public async Task GetLastVisGetStatsVisitsitPerson(IDialogContext context, LuisResult result)
        {
            string message = $"{Dialog.NotAllowed.Spintax()}";

            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("GetVisitsPersonByFirstname")]
        public async Task GetVisitsPersonByFirstname(IDialogContext context, LuisResult result)
        {
            string message = $"{Dialog.NotAllowed.Spintax()}";

            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("GetAverageVisits")]
        public async Task GetAverageVisits(IDialogContext context, LuisResult result)
        {
            string message = $"{Dialog.NotAllowed.Spintax()}";

            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }
    }
}
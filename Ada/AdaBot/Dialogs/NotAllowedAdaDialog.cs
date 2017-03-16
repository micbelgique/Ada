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
using System.Text.RegularExpressions;
using HtmlAgilityPack;

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
            CreateDialog createCarousel = new CreateDialog();

            Activity replyToConversation = createCarousel.CarouselPossibilitiesNotAllowed(context);


            await context.PostAsync(replyToConversation);
            context.Wait(MessageReceived);
        }

        [LuisIntent("What'sUp")]
        public async Task Event(IDialogContext context, LuisResult result)
        {
            CreateDialog createCarousel = new CreateDialog();

            Activity replyToConversation = await createCarousel.GetEvent(context);


            await context.PostAsync(replyToConversation);
            context.Wait(MessageReceived);
        }

        [LuisIntent("GetHelp")]
        public async Task GetHelp(IDialogContext context, LuisResult result)
        {
            CreateDialog createCarousel = new CreateDialog();

            Activity replyToConversation = createCarousel.GetHelp(context);


            await context.PostAsync(replyToConversation);
            context.Wait(MessageReceived);
        }

        private async Task BasicCallback(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("BestFriend")]
        public async Task BestFriend(IDialogContext context, LuisResult result)
        {
            string message = $"{Dialog.NotAllowed.Spintax()}";

            await context.PostAsync(message);
            context.Wait(MessageReceived);
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
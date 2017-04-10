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
using Microsoft.Bot.Builder.FormFlow;
using System.Diagnostics;
using AdaBot.Models.FormFlows;

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
            AdaClient client = new AdaClient() { WebAppUrl = $"{ ConfigurationManager.AppSettings["WebAppUrl"] }" };
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

            replyToConversation.Name = "Finish";
            await context.PostAsync(replyToConversation);
            context.Wait(MessageReceived);
        }

        [LuisIntent("What'sUp")]
        public async Task Event(IDialogContext context, LuisResult result)
        {
            CreateDialog createCarousel = new CreateDialog();

            Activity replyToConversation = await createCarousel.GetEvent(context);

            replyToConversation.Name = "Finish";
            await context.PostAsync(replyToConversation);
            context.Wait(MessageReceived);
        }

        [LuisIntent("GetHelp")]
        public async Task GetHelp(IDialogContext context, LuisResult result)
        {
            if (result.TopScoringIntent.Score > 0.70)
            {
                var form = MakeInfo();
                context.Call(form, ResumeAfterInfo);
            }
            else
            {
                Activity replyToConversation;
                replyToConversation = ((Activity)context.Activity).CreateReply($"{Dialog.None.Spintax()}");
                replyToConversation.Recipient = context.Activity.From;
                replyToConversation.Name = "Finish";
                replyToConversation.Type = "message";
                await context.PostAsync(replyToConversation);
            }
        }

        private async Task ResumeAfterInfo(IDialogContext context, IAwaitable<FormInfo> result)
        {
            try
            {
                var ourResult = await result;
                TreatmentDialog treatment = new TreatmentDialog();
                string question = "";
                if (ourResult.QuestionDev.ToString() != "test")
                {
                    question = ourResult.QuestionDev.ToString();
                }
                else if (ourResult.QuestionProject.ToString() != "test")
                {
                    question = ourResult.QuestionProject.ToString();
                }
                else if (ourResult.QuestionStudent.ToString() != "test")
                {
                    question = ourResult.QuestionStudent.ToString();
                }
                else if (ourResult.QuestionVisit.ToString() != "test")
                {
                    question = ourResult.QuestionVisit.ToString();
                }
                string response = treatment.getResponseToInfo(question);
                await context.PostAsync(response);
            }
            catch (FormCanceledException<FormInfo> e)
            {
                Debug.WriteLine(e.Message);
                context.Done(true);
            }
        }

        internal static IDialog<FormInfo> MakeInfo()
        {
            return Chain.From(() => FormDialog.FromForm(FormInfo.BuildForm, options: FormOptions.PromptInStart));
        }

        private async Task BasicCallback(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("SeeNow")]
        public async Task SeeNow(IDialogContext context, LuisResult result)
        {
            Activity replyToConversation;
            replyToConversation = ((Activity)context.Activity).CreateReply($"{Dialog.NotAllowed.Spintax()}");
            replyToConversation.Recipient = context.Activity.From;
            replyToConversation.Name = "Finish";
            replyToConversation.Type = "message";
            await context.PostAsync(replyToConversation);
            context.Wait(MessageReceived);
        }

        [LuisIntent("SendMessage")]
        public async Task SendMessage(IDialogContext context, LuisResult result)
        {
            Activity replyToConversation;
            replyToConversation = ((Activity)context.Activity).CreateReply($"{Dialog.NotAllowed.Spintax()}");
            replyToConversation.Recipient = context.Activity.From;
            replyToConversation.Name = "Finish";
            replyToConversation.Type = "message";
            await context.PostAsync(replyToConversation);
            context.Wait(MessageReceived);
        }

        [LuisIntent("BestFriend")]
        public async Task BestFriend(IDialogContext context, LuisResult result)
        {
            Activity replyToConversation;
            replyToConversation = ((Activity)context.Activity).CreateReply($"{Dialog.NotAllowed.Spintax()}");
            replyToConversation.Recipient = context.Activity.From;
            replyToConversation.Name = "Finish";
            replyToConversation.Type = "message";
            await context.PostAsync(replyToConversation);
            context.Wait(MessageReceived);
        }

        [LuisIntent("GetVisitsToday")]
        public async Task GetVisitsToday(IDialogContext context, LuisResult result)
        {
            Activity replyToConversation;
            replyToConversation = ((Activity)context.Activity).CreateReply($"{Dialog.NotAllowed.Spintax()}");
            replyToConversation.Recipient = context.Activity.From;
            replyToConversation.Name = "Finish";
            replyToConversation.Type = "message";
            await context.PostAsync(replyToConversation);
            context.Wait(MessageReceived);
        }

        [LuisIntent("GetLastVisitPerson")]
        public async Task GetLastVisitPerson(IDialogContext context, LuisResult result)
        {
            Activity replyToConversation;
            replyToConversation = ((Activity)context.Activity).CreateReply($"{Dialog.NotAllowed.Spintax()}");
            replyToConversation.Recipient = context.Activity.From;
            replyToConversation.Name = "Finish";
            replyToConversation.Type = "message";
            await context.PostAsync(replyToConversation);
            context.Wait(MessageReceived);
        }

        [LuisIntent("GetStatsVisits")]
        public async Task GetLastVisGetStatsVisitsitPerson(IDialogContext context, LuisResult result)
        {
            Activity replyToConversation;
            replyToConversation = ((Activity)context.Activity).CreateReply($"{Dialog.NotAllowed.Spintax()}");
            replyToConversation.Recipient = context.Activity.From;
            replyToConversation.Name = "Finish";
            replyToConversation.Type = "message";
            await context.PostAsync(replyToConversation);
            context.Wait(MessageReceived);
        }

        [LuisIntent("GetVisitsPersonByFirstname")]
        public async Task GetVisitsPersonByFirstname(IDialogContext context, LuisResult result)
        {
            Activity replyToConversation;
            replyToConversation = ((Activity)context.Activity).CreateReply($"{Dialog.NotAllowed.Spintax()}");
            replyToConversation.Recipient = context.Activity.From;
            replyToConversation.Name = "Finish";
            replyToConversation.Type = "message";
            await context.PostAsync(replyToConversation);
            context.Wait(MessageReceived);
        }

        [LuisIntent("GetAverageVisits")]
        public async Task GetAverageVisits(IDialogContext context, LuisResult result)
        {
            Activity replyToConversation;
            replyToConversation = ((Activity)context.Activity).CreateReply($"{Dialog.NotAllowed.Spintax()}");
            replyToConversation.Recipient = context.Activity.From;
            replyToConversation.Name = "Finish";
            replyToConversation.Type = "message";
            await context.PostAsync(replyToConversation);
            context.Wait(MessageReceived);
        }
    }
}
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Bot.Connector;
using System;
using AdaBot.Answers;
using AdaBot.Bot.Utils;
using AdaSDK;
using System.Configuration;

namespace AdaBot.Dialogs
{
    [Serializable]
    public class TrivialLuisDialog : LuisDialog<object>
    {
        public TrivialLuisDialog(params ILuisService[] services) : base(services)
        {

        }

        protected override async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {

            var message = (Activity)await item;
            await base.MessageReceived(context, item);
        }

        private async Task TrivialCallback(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var activity = await result;
        }

        [LuisIntent("Insult")]
        public async Task Insult(IDialogContext context, LuisResult result)
        {
            double? score = result.TopScoringIntent.Score;
            string message;
            if (score >= 0.85)
            {
                message = $"{Dialog.Insult.Spintax()}";
            }
            else
            {
                message = $"{Dialog.NotSureInsult.Spintax()}";
            }

            Activity replyToConversation;
            replyToConversation = ((Activity)context.Activity).CreateReply(message);
            replyToConversation.Recipient = context.Activity.From;
            replyToConversation.Type = "message";
            await context.PostAsync(replyToConversation);
            context.Done<object>(null);
        }

        [LuisIntent("Age")]
        public async Task Age(IDialogContext context, LuisResult result)
        {
            string message = $"{Dialog.Age.Spintax()}";
            Activity replyToConversation;
            replyToConversation = ((Activity)context.Activity).CreateReply(message);
            replyToConversation.Recipient = context.Activity.From;
            replyToConversation.Type = "message";
            await context.PostAsync(replyToConversation);
            context.Done<object>(null);
        }

        [LuisIntent("Compliment")]
        public async Task Compliment(IDialogContext context, LuisResult result)
        {
            string message = $"{Dialog.Compliment.Spintax()}";
            Activity replyToConversation;
            replyToConversation = ((Activity)context.Activity).CreateReply(message);
            replyToConversation.Recipient = context.Activity.From;
            replyToConversation.Type = "message";
            await context.PostAsync(replyToConversation);
            context.Done<object>(null);
        }

        [LuisIntent("Greetings")]
        public async Task Greetings(IDialogContext context, LuisResult result)
        {
            string nameUser = context.Activity.From.Name;
            AdaClient client = new AdaClient() { WebAppUrl = $"{ ConfigurationManager.AppSettings["WebAppUrl"] }" };
            Activity replyToConversation;
            string message;

            if (nameUser != null)
            {
                string[] firstNameUser = nameUser.Split(' ');
                message = $"{Dialog.Greeting.Spintax()} {firstNameUser[0]}";
            }
            else
            {
                message = $"{Dialog.Greeting.Spintax()}";
            }
            await context.PostAsync(message);

            CreateDialog createCarousel = new CreateDialog();
            var idUser = context.Activity.From.Id;
            var accessAllow = await client.GetAuthorizationFacebook(idUser);

            if (accessAllow == "true")
            {
                replyToConversation = createCarousel.CarouselPossibilities(context);
            }
            else
            {
                replyToConversation = createCarousel.CarouselPossibilitiesNotAllowed(context);
            }
            await context.PostAsync(replyToConversation);

            context.Done<object>(null);
        }

        [LuisIntent("Farewell")]
        public async Task Farewell(IDialogContext context, LuisResult result)
        {
            string message = $"{Dialog.Bye.Spintax()}";
            Activity replyToConversation;
            replyToConversation = ((Activity)context.Activity).CreateReply(message);
            replyToConversation.Recipient = context.Activity.From;
            replyToConversation.Type = "message";
            replyToConversation.Name = "End";
            await context.PostAsync(replyToConversation);
            context.Done<object>(null);
        }

        [LuisIntent("LifeSignification")]
        public async Task LifeSignification(IDialogContext context, LuisResult result)
        {
            string message = $"{Dialog.Sens.Spintax()}";
            Activity replyToConversation;
            replyToConversation = ((Activity)context.Activity).CreateReply(message);
            replyToConversation.Recipient = context.Activity.From;
            replyToConversation.Type = "message";
            await context.PostAsync(replyToConversation);
            context.Done<object>(null);
        }

        [LuisIntent("Feelings")]
        public async Task Feelings(IDialogContext context, LuisResult result)
        {
            string message = $"{Dialog.Feelings.Spintax()}";
            Activity replyToConversation;
            replyToConversation = ((Activity)context.Activity).CreateReply(message);
            replyToConversation.Recipient = context.Activity.From;
            replyToConversation.Type = "message";
            await context.PostAsync(replyToConversation);
            context.Done<object>(null);
        }

        [LuisIntent("Home")]
        public async Task Home(IDialogContext context, LuisResult result)
        {
            string message = $"{Dialog.Home.Spintax()}";
            Activity replyToConversation;
            replyToConversation = ((Activity)context.Activity).CreateReply(message);
            replyToConversation.Recipient = context.Activity.From;
            replyToConversation.Type = "message";
            await context.PostAsync(replyToConversation);
            context.Done<object>(null);
        }

        [LuisIntent("InfoRequest")]
        public async Task InfoRequest(IDialogContext context, LuisResult result)
        {
            string message = $"{Dialog.InfoRequest.Spintax()}";
            Activity replyToConversation;
            replyToConversation = ((Activity)context.Activity).CreateReply(message);
            replyToConversation.Recipient = context.Activity.From;
            replyToConversation.Type = "message";
            await context.PostAsync(replyToConversation);
            context.Done<object>(null);
        }

        [LuisIntent("JokeRequest")]
        public async Task JokeRequest(IDialogContext context, LuisResult result)
        {
            string message = $"{Dialog.JokeRequest.Spintax()}";
            Activity replyToConversation;
            replyToConversation = ((Activity)context.Activity).CreateReply(message);
            replyToConversation.Recipient = context.Activity.From;
            replyToConversation.Type = "message";
            await context.PostAsync(replyToConversation);
            context.Done<object>(null);
        }

        [LuisIntent("Name")]
        public async Task Name(IDialogContext context, LuisResult result)
        {
            string message = $"{Dialog.Name.Spintax()}";
            Activity replyToConversation;
            replyToConversation = ((Activity)context.Activity).CreateReply(message);
            replyToConversation.Recipient = context.Activity.From;
            replyToConversation.Type = "message";
            await context.PostAsync(replyToConversation);
            context.Done<object>(null);
        }

        [LuisIntent("Phone")]
        public async Task Phone(IDialogContext context, LuisResult result)
        {
            string message = $"{Dialog.Phone.Spintax()}";
            Activity replyToConversation;
            replyToConversation = ((Activity)context.Activity).CreateReply(message);
            replyToConversation.Recipient = context.Activity.From;
            replyToConversation.Type = "message";
            await context.PostAsync(replyToConversation);
            context.Done<object>(null);
        }

        [LuisIntent("Reality")]
        public async Task Reality(IDialogContext context, LuisResult result)
        {
            string message = $"{Dialog.Reality.Spintax()}";
            Activity replyToConversation;
            replyToConversation = ((Activity)context.Activity).CreateReply(message);
            replyToConversation.Recipient = context.Activity.From;
            replyToConversation.Type = "message";
            await context.PostAsync(replyToConversation);
            context.Done<object>(null);
        }

        [LuisIntent("Sex")]
        public async Task Sex(IDialogContext context, LuisResult result)
        {
            string message = $"{Dialog.Sex.Spintax()}";
            Activity replyToConversation;
            replyToConversation = ((Activity)context.Activity).CreateReply(message);
            replyToConversation.Recipient = context.Activity.From;
            replyToConversation.Type = "message";
            await context.PostAsync(replyToConversation);
            context.Done<object>(null);
        }

        [LuisIntent("Thanks")]
        public async Task Thanks(IDialogContext context, LuisResult result)
        {
            string message = $"{Dialog.Thanks.Spintax()}";
            Activity replyToConversation;
            replyToConversation = ((Activity)context.Activity).CreateReply(message);
            replyToConversation.Recipient = context.Activity.From;
            replyToConversation.Type = "message";
            await context.PostAsync(replyToConversation);
            context.Done<object>(null);
        }

        [LuisIntent("Time")]
        public async Task Time(IDialogContext context, LuisResult result)
        {
            DateTime date = DateTime.Now;
            date = date.AddHours(2);
            string message = $"Nous sommes le : " + date.ToString("dd/MM/yyyy") + " et il est : " + date.Hour + "h" + date.Minute;
            Activity replyToConversation;
            replyToConversation = ((Activity)context.Activity).CreateReply(message);
            replyToConversation.Recipient = context.Activity.From;
            replyToConversation.Type = "message";
            await context.PostAsync(replyToConversation);
            context.Done<object>(null);
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            FAQDialog faqDialog = new FAQDialog();
            string response = faqDialog.AskSomething(result.Query.ToString());
            Activity replyToConversation;
            replyToConversation = ((Activity)context.Activity).CreateReply(response);
            replyToConversation.Recipient = context.Activity.From;
            replyToConversation.Type = "message";
            await context.PostAsync(replyToConversation);
            context.Done<object>(null);
        }
    }
}
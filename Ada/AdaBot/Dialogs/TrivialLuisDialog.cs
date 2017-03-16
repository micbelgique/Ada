using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Bot.Connector;
using System;
using AdaBot.Answers;
using AdaBot.Bot.Utils;

namespace AdaBot.Dialogs
{
    [Serializable]
    public class TrivialLuisDialog : LuisDialog<object>
    {
        public TrivialLuisDialog(params ILuisService[] services) : base(services)
        {

        }

        private async Task TrivialCallback(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var activity = await result;
        }

        [LuisIntent("Insult")]
        public async Task Insult(IDialogContext context, LuisResult result)
        {
            string message = $"{Dialog.Insult.Spintax()}";
            await context.PostAsync(message);
            context.Done<object>(null);
        } 

        [LuisIntent("Age")]
        public async Task Age(IDialogContext context, LuisResult result)
        {
            string message = $"{Dialog.Age.Spintax()}";
            await context.PostAsync(message);
            context.Done<object>(null);
        }

        [LuisIntent("Compliment")]
        public async Task Compliment(IDialogContext context, LuisResult result)
        {
            string message = $"{Dialog.Compliment.Spintax()}";
            await context.PostAsync(message);
            context.Done<object>(null);
        }

        [LuisIntent("Greetings")]
        public async Task Greetings(IDialogContext context, LuisResult result)
        {
            string nameUser = context.Activity.From.Name;

            string[] firstNameUser = nameUser.Split(' ');
            string message = $"{Dialog.Greeting.Spintax()} {firstNameUser[0]}";
            await context.PostAsync(message);
            context.Done<object>(null);
        }

        [LuisIntent("Farewell")]
        public async Task Farewell(IDialogContext context, LuisResult result)
        {
            string nameUser = context.Activity.From.Name;

            string[] firstNameUser = nameUser.Split(' ');
            string message = $"{Dialog.Bye.Spintax()}{firstNameUser[0]}";
            await context.PostAsync(message);
            context.Done<object>(null);
        }

        [LuisIntent("LifeSignification")] 
        public async Task LifeSignification(IDialogContext context, LuisResult result)
        {
            string message = $"{Dialog.Sens.Spintax()}";
            await context.PostAsync(message);
            context.Done<object>(null);
        }

        [LuisIntent("Feelings")]
        public async Task Feelings(IDialogContext context, LuisResult result)
        {
            string message = $"{Dialog.Feelings.Spintax()}";
            await context.PostAsync(message);
            context.Done<object>(null);
        }

        [LuisIntent("Home")]
        public async Task Home(IDialogContext context, LuisResult result)
        {
            string message = $"{Dialog.Home.Spintax()}";
            await context.PostAsync(message);
            context.Done<object>(null);
        }

        [LuisIntent("InfoRequest")]
        public async Task InfoRequest(IDialogContext context, LuisResult result)
        {
            string message = $"{Dialog.InfoRequest.Spintax()}";
            await context.PostAsync(message);
            context.Done<object>(null);
        }

        [LuisIntent("JokeRequest")]
        public async Task JokeRequest(IDialogContext context, LuisResult result)
        {
            string message = $"{Dialog.JokeRequest.Spintax()}";
            await context.PostAsync(message);
            context.Done<object>(null);
        }

        [LuisIntent("Name")]
        public async Task Name(IDialogContext context, LuisResult result)
        {
            string message = $"{Dialog.Name.Spintax()}";
            await context.PostAsync(message);
            context.Done<object>(null);
        }

        [LuisIntent("Phone")]
        public async Task Phone(IDialogContext context, LuisResult result)
        {
            string message = $"{Dialog.Phone.Spintax()}";
            await context.PostAsync(message);
            context.Done<object>(null);
        }

        [LuisIntent("Reality")]
        public async Task Reality(IDialogContext context, LuisResult result)
        {
            string message = $"{Dialog.Reality.Spintax()}";
            await context.PostAsync(message);
            context.Done<object>(null);
        }

        [LuisIntent("Sex")]
        public async Task Sex(IDialogContext context, LuisResult result)
        {
            string message = $"{Dialog.Sex.Spintax()}";
            await context.PostAsync(message);
            context.Done<object>(null);
        }

        [LuisIntent("Thanks")]
        public async Task Thanks(IDialogContext context, LuisResult result)
        {
            string message = $"{Dialog.Thanks.Spintax()}";
            await context.PostAsync(message);
            context.Done<object>(null);
        }

        [LuisIntent("Time")]
        public async Task Time(IDialogContext context, LuisResult result)
        {
            DateTime date = DateTime.Now;
            string message = $"Nous sommes le : " + date.ToString("dd/MM/yyyy") + " et il est : " + date.Hour +"h" + date.Minute;
            await context.PostAsync(message);
            context.Done<object>(null);
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"{Dialog.None.Spintax()}";
            await context.PostAsync(message);
            context.Done<object>(null);
        }
    }
}
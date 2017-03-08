using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AdaBot.Dialogs
{
    public class TrivialCommunication : LuisDialog<object>
    {
        public TrivialCommunication(params ILuisService[] services) : base(services)
        {

        }

        protected override async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            var message = (Activity)await item;
            await base.MessageReceived(context, item);
        }

        [LuisIntent("Insult")]
        public async Task SayHello(IDialogContext context, LuisResult result)
        {
            string message = $"Ce n'est pas gentil";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }
    }
}
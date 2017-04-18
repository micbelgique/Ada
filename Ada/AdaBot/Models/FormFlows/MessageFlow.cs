using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdaBot.Models.FormFlows
{
    [Serializable]
    public class MessageFlow
    {
        [Prompt("Quel message dois-je faire passer?")]
        public string Message { get; set; }

        public static IForm<MessageFlow> BuildForm()
        {
            var builder = new FormBuilder<MessageFlow>()
                .Field(nameof(Message))
                .Confirm(async (state) =>
               {
                   return new PromptAttribute($"Êtes-vous sûr de vouloir envoyer ce message: {state.Message.ToString()}");
               });
            builder.Configuration.Yes = new string[] { "Yes", "yes", "Oui", "oui" };
            builder.Configuration.No = new string[] { "No", "no", "Non", "non" };

            return builder.Build();
        }
    }
}
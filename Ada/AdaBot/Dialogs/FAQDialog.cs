using AdaBot.Answers;
using AdaBot.Bot.Utils;
using AdaBot.Models;
using Microsoft.Bot.Builder.CognitiveServices.QnAMaker;
using Microsoft.IdentityModel.Protocols;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;

namespace AdaBot.Dialogs
{
    [Serializable]
    public class FAQDialog : QnAMakerDialog
    {
        public FAQDialog() : base(new QnAMakerService
        (new QnAMakerAttribute(ConfigurationManager.AppSettings["QnASubscriptionKey"],
        ConfigurationManager.AppSettings["QnAKnowledgebaseId"],
        "Je ne comprends pas ta question, peux-tu la reformuler?",
        0.5)))
        { }

        public string AskSomething(string question)
        {
            string response = $"{Dialog.None.Spintax()}";
            
            //Build the URI
            Uri qnamakerUriBase = new Uri("https://westus.api.cognitive.microsoft.com/qnamaker/v1.0");
            var builder = new UriBuilder($"{qnamakerUriBase}/knowledgebases/{ConfigurationManager.AppSettings["QnAKnowledgebaseId"]}/generateAnswer");

            //Add the question as part of the body
            var postBody = $"{{\"question\": \"{question}\"}}";

            //Send the POST request
            using (WebClient client = new WebClient())
            {
                //Set the encoding to UTF8
                client.Encoding = System.Text.Encoding.UTF8;

                //Add the subscription key header
                client.Headers.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["QnASubscriptionKey"]);
                client.Headers.Add("Content-Type", "application/json");
                string responseString = client.UploadString(builder.Uri, postBody);

                var answer = JsonConvert.DeserializeObject<AnswersFAQ>(responseString);
                response = answer.Answer.ToString();
            }
            return response;
        }
    }
}
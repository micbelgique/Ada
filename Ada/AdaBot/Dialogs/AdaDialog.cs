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
using Microsoft.IdentityModel.Protocols;
using System.Configuration;
using System.Net;
using Newtonsoft.Json;
using AdaSDK.Models;

namespace AdaBot.Dialogs
{
    [Serializable]
    public class AdaDialog : LuisDialog<object>
    {
        private static Activity _message;

        public AdaDialog(params ILuisService[] services): base(services)
        {

        }

        protected override async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            _message = (Activity)await item;
            await base.MessageReceived(context, item);
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Je n'ai pas compris :/";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
            message = $"Je suis constamment en apprentissage, je vais demander à mes administrateurs de me l'apprendre.";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("SayHello")]
        public async Task SayHello(IDialogContext context, LuisResult result)
        {
            string message = $"Bonjour";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("GetVisitsToday")]
        public async Task GetVisitsToday(IDialogContext context, LuisResult result)
        {
            List<VisitDto> visits;

            using (var client = new HttpClient())
            {
                //ToDo Addapter URL
                var httpResponse = await client.GetAsync(ConfigurationManager.AppSettings["ApiGetVisitsToday"]);

                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    var x = await httpResponse.Content.ReadAsStringAsync();
                    visits = JsonConvert.DeserializeObject<List<VisitDto>>(x);
                    Activity replyToConversation;

                    if (visits.Count == 0)
                    {
                        replyToConversation = _message.CreateReply("Je n'ai encore vu personne aujourd'hui... :'(");
                        replyToConversation.Recipient = _message.From;
                        replyToConversation.Type = "message";
                    }
                    else
                    {
                        replyToConversation = _message.CreateReply("Voici les visites du jour:");
                        replyToConversation.Recipient = _message.From;
                        replyToConversation.Type = "message";
                        replyToConversation.AttachmentLayout = "carousel";
                        replyToConversation.Attachments = new List<Attachment>();

                        foreach (var visit in visits)
                        {
                            List<CardImage> cardImages = new List<CardImage>();
                            cardImages.Add(new CardImage(url: $"{ ConfigurationManager.AppSettings["WebAppUrl"] }{VirtualPathUtility.ToAbsolute(visit.ProfilePicture.Uri)}"));

                            HeroCard plCard = new HeroCard()
                            {
                                Title = visit.PersonVisit.FirstName,
                                Text = Convert.ToString(visit.PersonVisit.DateVisit),
                                //Subtitle = 
                                Images = cardImages
                                //Buttons = cardButtons
                            };

                            Attachment plAttachment = plCard.ToAttachment();
                            replyToConversation.Attachments.Add(plAttachment);
                        }
                    }

                    await context.PostAsync(replyToConversation);
                    context.Wait(MessageReceived);
                }
            }
        }

        [LuisIntent("GetLastVisitPerson")]
        public async Task GetLastVisitPersonHello(IDialogContext context, LuisResult result)
        {
            string firstname = result.Entities[0].Entity;
            List<VisitDto> visits;

            using (var client = new HttpClient())
            {
                //ToDo Addapter URL
                var httpResponse = await client.GetAsync(ConfigurationManager.AppSettings["ApiGetVisitsFirstname"] + "/" + firstname);

                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    var x = await httpResponse.Content.ReadAsStringAsync();
                    visits = JsonConvert.DeserializeObject<List<VisitDto>>(x);
                    Activity replyToConversation;

                    if (visits.Count == 0)
                    {
                        replyToConversation = _message.CreateReply("Je n'ai pas encore rencontré " + firstname + " :/");
                        replyToConversation.Recipient = _message.From;
                        replyToConversation.Type = "message";
                    }
                    else
                    {
                        replyToConversation = _message.CreateReply("J'ai vu " + firstname + " à cette date:");
                        replyToConversation.Recipient = _message.From;
                        replyToConversation.Type = "message";
                        replyToConversation.AttachmentLayout = "carousel";
                        replyToConversation.Attachments = new List<Attachment>();

                        foreach (var visit in visits)
                        {
                            List<CardImage> cardImages = new List<CardImage>();
                            cardImages.Add(new CardImage(url: $"{ ConfigurationManager.AppSettings["WebAppUrl"] }{VirtualPathUtility.ToAbsolute(visit.ProfilePicture.Uri)}"));

                            HeroCard plCard = new HeroCard()
                            {
                                Title = visit.PersonVisit.FirstName,
                                Text = Convert.ToString(visit.PersonVisit.DateVisit),
                                //Subtitle = recipe
                                Images = cardImages
                                //Buttons = cardButtons
                            };

                            Attachment plAttachment = plCard.ToAttachment();
                            replyToConversation.Attachments.Add(plAttachment);
                        }
                    }
                    
                    await context.PostAsync(replyToConversation);
                    context.Wait(MessageReceived);
                }
            }
        }


    }
}
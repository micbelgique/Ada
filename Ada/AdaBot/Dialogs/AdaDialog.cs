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
            message = $"Je suis constamment en apprentissage, je vais demander à mes créateurs de m'apprendre ta phrase ;)";
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
                        replyToConversation = _message.CreateReply("J'ai vu du monde aujourd'hui! :D");
                        replyToConversation.Recipient = _message.From;
                        replyToConversation.Type = "message";
                        replyToConversation.AttachmentLayout = "carousel";
                        replyToConversation.Attachments = new List<Attachment>();

                        foreach (var visit in visits)
                        {
                            List<CardImage> cardImages = new List<CardImage>();
                            cardImages.Add(new CardImage(url: $"{ ConfigurationManager.AppSettings["WebAppUrl"] }{VirtualPathUtility.ToAbsolute(visit.ProfilePicture.Uri)}"));

                            //Calcul la bonne année et la bonne heure.
                            DateTime today = DateTime.Today;
                            int wrongDate = visit.PersonVisit.DateVisit.Year;
                            int goodDate = DateTime.Today.Year - wrongDate;
                            string messageDate = "";
                            string firstname = visit.PersonVisit.FirstName;

                            //Préparation du message du HeroCard en fonction de la date de la visite
                            if (visit.PersonVisit.DateVisit.Day == today.Day)
                            {
                                if (visit.PersonVisit.DateVisit.Hour <= 12)
                                {
                                    messageDate = "J'ai croisé " + firstname + " ce matin.";
                                }
                                else if (visit.PersonVisit.DateVisit.Hour >= 12 && visit.PersonVisit.DateVisit.Hour <= 17)
                                {
                                    messageDate = "J'ai croisé " + firstname + " cet après-midi.";
                                }
                                else
                                {
                                    messageDate = "J'ai croisé " + firstname + " cette nuit... Il doit sûrement faire des heures sup'!";
                                }
                            }
                            else if (visit.PersonVisit.DateVisit.Day == today.Day - 1)
                            {
                                if (visit.PersonVisit.DateVisit.Hour <= 12)
                                {
                                    messageDate = "J'ai croisé " + firstname + " hier matin.";
                                }
                                else if (visit.PersonVisit.DateVisit.Hour >= 12 && visit.PersonVisit.DateVisit.Hour <= 17)
                                {
                                    messageDate = "J'ai croisé " + firstname + " hier après-midi.";
                                }
                                else
                                {
                                    messageDate = "J'ai croisé " + firstname + " la nuit dernière... Il doit sûrement faire des heures sup'!";
                                }
                            }
                            else
                            {
                                var dayDiff = visit.PersonVisit.DateVisit.Day - today.Day;
                                messageDate = "J'ai croisé " + firstname + " il y a " + dayDiff + " jours.";
                            }

                            HeroCard plCard = new HeroCard()
                            {
                                Title = visit.PersonVisit.FirstName,
                                Text = messageDate + "(" + Convert.ToString(visit.PersonVisit.DateVisit.AddHours(1).AddYears(goodDate)) + ")",
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
        public async Task GetLastVisitPerson(IDialogContext context, LuisResult result)
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
                        replyToConversation = _message.CreateReply("Je n'ai pas encore rencontré " + firstname + " :/ Il faudrait nous présenter! ^^");
                        replyToConversation.Recipient = _message.From;
                        replyToConversation.Type = "message";
                    }
                    else
                    {
                        replyToConversation = _message.CreateReply("Voyons voir...");
                        replyToConversation.Recipient = _message.From;
                        replyToConversation.Type = "message";
                        replyToConversation.AttachmentLayout = "carousel";
                        replyToConversation.Attachments = new List<Attachment>();

                        foreach (var visit in visits)
                        {
                            List<CardImage> cardImages = new List<CardImage>();
                            cardImages.Add(new CardImage(url: $"{ ConfigurationManager.AppSettings["WebAppUrl"] }{VirtualPathUtility.ToAbsolute(visit.ProfilePicture.Uri)}"));

                            //Calcul la bonne année et la bonne heure.
                            DateTime today = DateTime.Today;
                            int wrongDate = visit.PersonVisit.DateVisit.Year;
                            int goodDate = DateTime.Today.Year - wrongDate;
                            string messageDate = "";

                            //Préparation du message du HeroCard en fonction de la date de la visite
                            if (visit.PersonVisit.DateVisit.Day == today.Day)
                            {
                                if (visit.PersonVisit.DateVisit.Hour <= 12)
                                {
                                    messageDate = "J'ai croisé " + firstname + " ce matin.";
                                }
                                else if (visit.PersonVisit.DateVisit.Hour >= 12 && visit.PersonVisit.DateVisit.Hour <= 17)
                                {
                                    messageDate = "J'ai croisé " + firstname + " cet après-midi.";
                                }
                                else
                                {
                                    messageDate = "J'ai croisé " + firstname + " cette nuit... Il doit sûrement faire des heures sup'!";
                                }
                            }
                            else if (visit.PersonVisit.DateVisit.Day == today.Day - 1)
                            {
                                if (visit.PersonVisit.DateVisit.Hour <= 12)
                                {
                                    messageDate = "J'ai croisé " + firstname + " hier matin.";
                                }
                                else if (visit.PersonVisit.DateVisit.Hour >= 12 && visit.PersonVisit.DateVisit.Hour <= 17)
                                {
                                    messageDate = "J'ai croisé " + firstname + " hier après-midi.";
                                }
                                else
                                {
                                    messageDate = "J'ai croisé " + firstname + " la nuit dernière... Il doit sûrement faire des heures sup'!";
                                }
                            }
                            else
                            {
                                var dayDiff = visit.PersonVisit.DateVisit.Day - today.Day;
                                messageDate = "J'ai croisé " + firstname + " il y a " + dayDiff + " jours.";
                            }

                            HeroCard plCard = new HeroCard()
                            {
                                Title = visit.PersonVisit.FirstName,
                                Text = messageDate + "(" + Convert.ToString(visit.PersonVisit.DateVisit.AddHours(1).AddYears(goodDate)) + ")",
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


    }
}
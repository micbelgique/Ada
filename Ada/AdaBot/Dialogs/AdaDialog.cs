﻿using Microsoft.Bot.Builder.Dialogs;
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
        private Activity _message;

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
            string nameUser = _message.From.Name;
            string[] firstNameUser = nameUser.Split(' ');
            string message = $"Bonjour {firstNameUser[0]}";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("GetVisitsToday")]
        public async Task GetVisitsToday(IDialogContext context, LuisResult result)
        {
            AdaClient client = new AdaClient();
            List<VisitDto> visits = await client.GetVisitsToday();

            Activity replyToConversation;

            if (visits.Count == 0)
            {
                replyToConversation = _message.CreateReply("Je n'ai encore vu personne aujourd'hui... :'(");
                replyToConversation.Recipient = _message.From;
                replyToConversation.Type = "message";
            }
            else
            {
                replyToConversation = _message.CreateReply("J'ai vu " + visits.Count + " personnes aujourd'hui! :D");
                replyToConversation.Recipient = _message.From;
                replyToConversation.Type = "message";
                replyToConversation.AttachmentLayout = "carousel";
                replyToConversation.Attachments = new List<Attachment>();

                foreach (var visit in visits)
                {
                    List<CardImage> cardImages = new List<CardImage>();
                    cardImages.Add(new CardImage(url: $"{ ConfigurationManager.AppSettings["WebAppUrl"] }{VirtualPathUtility.ToAbsolute(visit.ProfilePicture.Uri)}")); //A mettre dans le SDK

                    //Calcul la bonne année et la bonne heure.
                    DateTime today = DateTime.Today;
                    int wrongDate = visit.PersonVisit.DateVisit.Year;
                    int goodDate = DateTime.Today.Year - wrongDate;
                    string messageDate = "";
                    string firstname;

                    //Recherche du prénom de la personne
                    if (visit.PersonVisit.FirstName == null)
                    {
                        firstname = "une personne inconnue";
                    }
                    else
                    {
                        firstname = visit.PersonVisit.FirstName;
                    }

                    //Préparation du message du HeroCard en fonction de la date de la visite
                    if (visit.PersonVisit.DateVisit.Day == today.Day)
                    {
                        if (visit.PersonVisit.DateVisit.Hour < 12)
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
                        if (visit.PersonVisit.DateVisit.Hour < 12)
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

        //[LuisIntent("")]
        //public async Task GetLastVisitPerson(IDialogContext context, LuisResult result)
        //{
        //    string firstname = result.Entities[0].Entity;
        //    AdaClient client = new AdaClient();
        //    List<VisitDto> visits = await client.GetLastVisitPerson(firstname);

        //    Activity replyToConversation;

        //    if (visits.Count == 0)
        //    {
        //        replyToConversation = _message.CreateReply("Je n'ai pas encore rencontré " + firstname + " :/ Il faudrait nous présenter! ^^");
        //        replyToConversation.Recipient = _message.From;
        //        replyToConversation.Type = "message";
        //    }
        //    else
        //    {
        //        replyToConversation = _message.CreateReply("Voyons voir...");
        //        replyToConversation.Recipient = _message.From;
        //        replyToConversation.Type = "message";
        //        replyToConversation.AttachmentLayout = "carousel";
        //        replyToConversation.Attachments = new List<Attachment>();

        //        foreach (var visit in visits)
        //        {
        //            List<CardImage> cardImages = new List<CardImage>();
        //            cardImages.Add(new CardImage(url: $"{ ConfigurationManager.AppSettings["WebAppUrl"] }{VirtualPathUtility.ToAbsolute(visit.ProfilePicture.Uri)}")); // a mettre dans le SDK

        //            //Calcul la bonne année et la bonne heure.
        //            DateTime today = DateTime.Today;
        //            int wrongDate = visit.PersonVisit.DateVisit.Year;
        //            int goodDate = DateTime.Today.Year - wrongDate;
        //            string messageDate = "";

        //            //Préparation du message du HeroCard en fonction de la date de la visite
        //            if (visit.PersonVisit.DateVisit.Day == today.Day)
        //            {
        //                if (visit.PersonVisit.DateVisit.Hour < 12)
        //                {
        //                    messageDate = "J'ai croisé " + firstname + " ce matin.";
        //                }
        //                else if (visit.PersonVisit.DateVisit.Hour >= 12 && visit.PersonVisit.DateVisit.Hour <= 17)
        //                {
        //                    messageDate = "J'ai croisé " + firstname + " cet après-midi.";
        //                }
        //                else
        //                {
        //                    messageDate = "J'ai croisé " + firstname + " cette nuit... Il doit sûrement faire des heures sup'!";
        //                }
        //            }
        //            else if (visit.PersonVisit.DateVisit.Day == today.Day - 1)
        //            {
        //                if (visit.PersonVisit.DateVisit.Hour < 12)
        //                {
        //                    messageDate = "J'ai croisé " + firstname + " hier matin.";
        //                }
        //                else if (visit.PersonVisit.DateVisit.Hour >= 12 && visit.PersonVisit.DateVisit.Hour <= 17)
        //                {
        //                    messageDate = "J'ai croisé " + firstname + " hier après-midi.";
        //                }
        //                else
        //                {
        //                    messageDate = "J'ai croisé " + firstname + " la nuit dernière... Il doit sûrement faire des heures sup'!";
        //                }
        //            }
        //            else
        //            {
        //                var dayDiff = today.Day - visit.PersonVisit.DateVisit.Day;
        //                messageDate = "J'ai croisé " + firstname + " il y a " + dayDiff + " jours.";
        //            }

        //            HeroCard plCard = new HeroCard()
        //            {
        //                Title = visit.PersonVisit.FirstName,
        //                Text = messageDate + "(" + Convert.ToString(visit.PersonVisit.DateVisit.AddHours(1).AddYears(goodDate)) + ")",
        //                //Subtitle = 
        //                Images = cardImages
        //                //Buttons = cardButtons
        //            };

        //            Attachment plAttachment = plCard.ToAttachment();
        //            replyToConversation.Attachments.Add(plAttachment);
        //        }
        //    }

        //    await context.PostAsync(replyToConversation);
        //    context.Wait(MessageReceived);
        //}

        [LuisIntent("GetLastVisitPerson")]
        public async Task GetVisitsPersonByFirstname(IDialogContext context, LuisResult result)
        {
            string firstname = result.Entities[0].Entity;
            AdaClient client = new AdaClient();
            List<VisitDto> visits = await client.GetLastVisitPerson(firstname);

            Activity replyToConversation = null;

            if (visits.Count == 0)
            {
                replyToConversation = _message.CreateReply("Je n'ai pas encore rencontré " + firstname + " :/ Il faudrait nous présenter! ^^");
                replyToConversation.Recipient = _message.From;
                replyToConversation.Type = "message";
            }
            else if (visits.Count == 1)
            {
                int id = visits[0].PersonVisit.PersonId;
                List<VisitDto> visitsById = await client.GetVisitPersonById(id);

                string reply = "J'ai vu " + firstname + " à ces dates : <br>";
                reply += Environment.NewLine;

                foreach (var visit in visitsById)
                {
                    int wrongDate = visit.PersonVisit.DateVisit.Year;
                    int goodDate = DateTime.Today.Year - wrongDate;
                    reply += "     -"+Convert.ToString(visit.Date.AddHours(1)) + "<br>";
                }

                replyToConversation = _message.CreateReply(reply);
            }
            else
            {
                replyToConversation = _message.CreateReply("Je connais " + visits.Count + " " + firstname + ". Les voici :)");
                replyToConversation.Recipient = _message.From;
                replyToConversation.Type = "message";
                replyToConversation.AttachmentLayout = "carousel";
                replyToConversation.Attachments = new List<Attachment>();

                foreach (var visit in visits)
                {
                    List<CardImage> cardImages = new List<CardImage>();
                    cardImages.Add(new CardImage(url: $"{ ConfigurationManager.AppSettings["WebAppUrl"] }{VirtualPathUtility.ToAbsolute(visit.ProfilePicture.Uri)}")); // a mettre dans le SDK

                    List<CardAction> cardButtons = new List<CardAction>();

                    CardAction plButtonChoice = new CardAction()
                    { 

                        Value = $"http://adawebapp.azurewebsites.net/Api/Visits/VisitPersonById/"+visit.PersonVisit.PersonId,//GetVisitePersonByIdButton(visit.PersonVisit, context),
                        Type = "imBack",
                        Title = "Le voilà !"
                    };
                    cardButtons.Add(plButtonChoice);

                    HeroCard plCard = new HeroCard()
                    {
                        Title = visit.PersonVisit.FirstName,
                        Images = cardImages,
                        Subtitle = Convert.ToString( await GetVisitePersonByIdButton(visit.PersonVisit, context)),
                        Buttons = cardButtons
                    };

                    Attachment plAttachment = plCard.ToAttachment();
                    replyToConversation.Attachments.Add(plAttachment);
                }
            }

            await context.PostAsync(replyToConversation);
            context.Wait(MessageReceived);
        }

        private async Task<string> GetVisitePersonByIdButton(PersonVisitDto person, IDialogContext context)
        {
            int id = person.PersonId;

            AdaClient client = new AdaClient();
            List<VisitDto> visitsById = await client.GetVisitPersonById(id);

            string reply = "J'ai vu " + person.FirstName + " à ces dates : ";

            foreach (var visit in visitsById)
            {
                int wrongDate = visit.PersonVisit.DateVisit.Year;
                int goodDate = DateTime.Today.Year - wrongDate;
                reply += "     -" + Convert.ToString(visit.Date.AddHours(1));
            }

            return reply;
        }
    }
}
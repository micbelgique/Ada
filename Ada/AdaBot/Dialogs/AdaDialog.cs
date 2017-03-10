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
using System.Configuration;
using AdaSDK.Models;
using AdaBot.BotFrameworkHelpers;
using System.Threading;
using System.Drawing;

namespace AdaBot.Dialogs
{
    [Serializable]
    public class AdaDialog : LuisDialog<object>
    {
        public AdaDialog(params ILuisService[] services) : base(services)
        {

        }

        protected override async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            var message = (Activity)await item;
            await base.MessageReceived(context, item); 
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.Forward(new TrivialLuisDialog(new LuisService(new LuisModelAttribute(
                           ConfigurationManager.AppSettings["ModelIdTrivial"],
                           ConfigurationManager.AppSettings["SubscriptionKeyTrivial"]))),
                   BasicCallback, context.Activity as Activity, CancellationToken.None); 
        }

        [LuisIntent("GetHelp")]
        public async Task GetHelp(IDialogContext context, LuisResult result)
        {
            Activity replyToConversation;
            replyToConversation = ((Activity)context.Activity).CreateReply("En quoi puis-je t'aider? :)");
            replyToConversation.Recipient = context.Activity.From;
            replyToConversation.Type = "message";
            replyToConversation.AttachmentLayout = "carousel";
            replyToConversation.Attachments = new List<Attachment>();

            List<string> pictures = new List<string>(); 
            pictures.Add("http://i.imgur.com/c8iWoTH.jpg?1"); 
            pictures.Add("http://i.imgur.com/2TsS2aq.png");
            pictures.Add("http://i.imgur.com/gjQkeXr.png");

            List<string> btnAction = new List<string>();
            btnAction.Add(ConfigurationManager.AppSettings["FaceBookMIC"]);
            btnAction.Add(ConfigurationManager.AppSettings["YoutubeMIC"]);
            btnAction.Add(ConfigurationManager.AppSettings["MeetupMIC"]);

            List<string> btnString = new List<string>();
            btnString.Add("Notre Facebook");
            btnString.Add("Notre chaîne Youtube");
            btnString.Add("Notre Meetup");
                        
            for (int i=0; i<btnAction.Count(); i++)
            {
                List<CardAction> cardsAction = new List<CardAction>();
                CardAction action = new CardAction()
                {
                    Value = btnAction[i].ToString(),
                    Type = "openUrl",
                    Title = btnString[i]
                };
                cardsAction.Add(action);
                List<CardImage> cardsImage = new List<CardImage>();
                CardImage img = new CardImage(url: pictures[i]);
                cardsImage.Add(img);

                HeroCard tmp = new HeroCard()
                {
                    Images = cardsImage,
                    Buttons = cardsAction
                };
                Attachment plAttachment = tmp.ToAttachment();
                replyToConversation.Attachments.Add(plAttachment);
            }
            await context.PostAsync(replyToConversation);
            context.Wait(MessageReceived);
        }

        private async Task BasicCallback(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(this.MessageReceived);
        }

        [LuisIntent("GetVisitsToday")]
        public async Task GetVisitsToday(IDialogContext context, LuisResult result)
        {
            //Message d'attente
            await context.PostAsync("Un petit instant, je vais te chercher ça! ;)");

            AdaClient client = new AdaClient();
            List<VisitDto> visits = await client.GetVisitsToday();

            Activity replyToConversation;

            if (visits.Count == 0)
            {
                replyToConversation = ((Activity)context.Activity).CreateReply("Je n'ai encore vu personne aujourd'hui... :'(");
                replyToConversation.Recipient = context.Activity.From;
                replyToConversation.Type = "message";
            }
            else
            {
                replyToConversation = ((Activity)context.Activity).CreateReply("J'ai vu " + visits.Count + " personnes aujourd'hui! :D");
                replyToConversation.Recipient = context.Activity.From;
                replyToConversation.Type = "message";
                replyToConversation.AttachmentLayout = "carousel";
                replyToConversation.Attachments = new List<Attachment>();

                int compteurCarrousel = 1;
                foreach (var visit in visits)
                {
                    if (compteurCarrousel <= 9)
                    {
                        List<CardImage> cardImages = new List<CardImage>();
                        cardImages.Add(new CardImage(url: $"{ ConfigurationManager.AppSettings["WebAppUrl"] }{VirtualPathUtility.ToAbsolute(visit.ProfilePicture.Last().Uri)}"));

                        //Calcul la bonne année et la bonne heure.
                        DateTime today = DateTime.Today;
                        int yearVisit = visit.Date.Year;
                        int wrongDate = visit.PersonVisit.DateVisit.Year;
                        int goodDate = DateTime.Today.Year - wrongDate;
                        string messageDate = "";
                        string firstname;
                        DateTime visitDate = visit.PersonVisit.DateVisit;

                        //Recherche du prénom de la personne
                        if (visit.PersonVisit.FirstName == null)
                        {
                            firstname = "une personne inconnue";
                        }
                        else
                        {
                            firstname = visit.PersonVisit.FirstName;
                        }

                        var customDialog = new CreateDialog();
                        messageDate = customDialog.GetVisitsMessage(firstname, visitDate.AddYears(goodDate));

                        HeroCard plCard = new HeroCard()
                        {
                            Title = firstname,
                            Text = messageDate + " (" + Convert.ToString(visit.PersonVisit.DateVisit.AddHours(1).AddYears(goodDate)) + ")",
                            //Subtitle = 
                            Images = cardImages
                            //Buttons = cardButtons
                        };

                        Attachment plAttachment = plCard.ToAttachment();
                        replyToConversation.Attachments.Add(plAttachment);
                        compteurCarrousel += 1;
                    }
                    else if (compteurCarrousel == 10)
                    {
                        List<CardImage> cardImages = new List<CardImage>();
                        cardImages.Add(new CardImage(""));

                        HeroCard plCard = new HeroCard()
                        {
                            Title = "Afficher plus (A venir)",
                            Text = "",
                            //Subtitle = 
                            Images = cardImages
                            //Buttons = cardButtons
                        };

                        Attachment plAttachment = plCard.ToAttachment();
                        replyToConversation.Attachments.Add(plAttachment);
                        compteurCarrousel += 1;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            await context.PostAsync(replyToConversation);
            context.Wait(MessageReceived);
        }

        [LuisIntent("GetLastVisitPerson")]
        public async Task GetLastVisitPerson(IDialogContext context, LuisResult result)
        {
            string firstname = result.Entities[0].Entity;
            AdaClient client = new AdaClient();
            List<VisitDto> visits = await client.GetLastVisitPerson(firstname);

            Activity replyToConversation;

            if (visits.Count == 0)
            {
                replyToConversation = ((Activity)context.Activity).CreateReply("Je n'ai pas encore rencontré " + firstname + " :/ Il faudrait nous présenter! ^^");
                replyToConversation.Recipient = context.Activity.From;
                replyToConversation.Type = "message";
            }
            else
            {
                replyToConversation = ((Activity)context.Activity).CreateReply("Voyons voir...");
                replyToConversation.Recipient = context.Activity.From;
                replyToConversation.Type = "message";
                replyToConversation.AttachmentLayout = "carousel";
                replyToConversation.Attachments = new List<Attachment>();

                int compteurCarrousel = 1;
                foreach (var visit in visits)
                {
                    if (compteurCarrousel <= 9)
                    {
                        List<CardImage> cardImages = new List<CardImage>();
                        cardImages.Add(new CardImage(url: $"{ ConfigurationManager.AppSettings["WebAppUrl"] }{VirtualPathUtility.ToAbsolute(visit.ProfilePicture.Last().Uri)}")); // a mettre dans le SDK

                        //Calcul la bonne année et la bonne heure.
                        DateTime today = DateTime.Today;
                        int yearVisit = visit.Date.Year;
                        int wrongDate = visit.PersonVisit.DateVisit.Year;
                        int goodDate = DateTime.Today.Year - wrongDate;
                        string messageDate = "";
                        DateTime visitDate = visit.PersonVisit.DateVisit;

                        var customDialog = new CreateDialog();
                        messageDate = customDialog.GetVisitsMessage(firstname, visitDate.AddYears(goodDate));

                        HeroCard plCard = new HeroCard()
                        {
                            Title = visit.PersonVisit.FirstName,
                            Text = messageDate + " (" + Convert.ToString(visit.PersonVisit.DateVisit.AddHours(1).AddYears(goodDate)) + ")",
                            //Subtitle = 
                            Images = cardImages
                            //Buttons = cardButtons
                        };

                        Attachment plAttachment = plCard.ToAttachment();
                        replyToConversation.Attachments.Add(plAttachment);
                        compteurCarrousel++;
                    }
                    else if (compteurCarrousel == 10)
                    {
                        List<CardImage> cardImages = new List<CardImage>();
                        cardImages.Add(new CardImage(""));

                        HeroCard plCard = new HeroCard()
                        {
                            Title = "Afficher plus (A venir)",
                            Text = "",
                            //Subtitle = 
                            Images = cardImages
                            //Buttons = cardButtons
                        };

                        Attachment plAttachment = plCard.ToAttachment();
                        replyToConversation.Attachments.Add(plAttachment);
                        compteurCarrousel += 1;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            await context.PostAsync(replyToConversation);
            context.Wait(MessageReceived);
        }

        [LuisIntent("GetStatsVisits")]
        public async Task GetLastVisGetStatsVisitsitPerson(IDialogContext context, LuisResult result)
        {
            //Message d'attente
            await context.PostAsync("Un petit instant, je vais te chercher ça! ;)");

            AdaClient client = new AdaClient();
            Activity replyToConversation;
            CreateDialog customDialog = new CreateDialog();
            TreatmentDialog treatment = new TreatmentDialog();
            EntityRecognizer recog = new EntityRecognizer();

            List<VisitDto> visitsReturn = new List<VisitDto>();
            List<VisitDto> tmp = new List<VisitDto>();
            List<ProfilePictureDto> EmotionPicture = new List<ProfilePictureDto>();
            bool askingEmotion = false;

            int nbEntities;
            int nbVisits = 0;
            string genderReturn = "personne(s)";
            string ageReturn = "";
            string emotionReturn = "";
            string emotion = "";
            string dateReturn = "aujourd'hui";

            DateTime? date1 = null;
            DateTime? date2 = null;
            int? age1 = null;
            int? age2 = null;
            GenderValues? gender = null;

            //Composite Entities
            if (result.CompositeEntities != null)
            {
                nbEntities = result.CompositeEntities.Count();
                for (int i = 0; i < nbEntities; i++)
                {
                    if (result.CompositeEntities[i].ParentType == "IntervalDate")
                    {
                        foreach (var entity in result.Entities)
                        {
                            if (entity.Type == "builtin.datetime.date")
                            {
                                DateTime? date;
                                List<EntityRecommendation> dates = new List<EntityRecommendation>();
                                dates.Add(entity);
                                recog.ParseDateTime(dates, out date);

                                if (date1 == null)
                                {
                                    date1 = date;
                                    dateReturn = "le " + Convert.ToDateTime(date1).ToString("yyyy-MM-dd");
                                }
                                else if (date1 != null)
                                {
                                    date2 = date;
                                    if (date2 < date1 && date2 != null)
                                    {
                                        DateTime? tmpDate = date1;
                                        date1 = date2;
                                        date2 = tmpDate;
                                    }
                                    dateReturn = "entre le " + Convert.ToDateTime(date1).ToString("yyyy-MM-dd") + " et le " + Convert.ToDateTime(date2).ToString("yyyy-MM-dd");
                                }
                            }
                        }
                    }
                    //Process of ages
                    if (result.CompositeEntities[i].ParentType == "SingleAge")
                    {
                        age1 = Convert.ToInt32(result.CompositeEntities[i].Children[0].Value);
                        ageReturn = "de " + age1;
                    }
                    else if (result.CompositeEntities[i].ParentType == "IntervalAge")
                    {
                        age1 = Convert.ToInt32(result.CompositeEntities[i].Children[0].Value);
                        age2 = Convert.ToInt32(result.CompositeEntities[i].Children[1].Value);
                        if (age2 < age1 && age2 != -1)
                        {
                            int? ageTmp = age1;
                            age1 = age2;
                            age2 = ageTmp;
                        }
                        ageReturn = "entre " + age1 + " et " + age2 + " ans";
                    }
                }
            }

            //Single entities
            nbEntities = result.Entities.Count();
            for (int i = 0; i < nbEntities; i++)
            {
                if (result.Entities[i].Type == "Gender")
                {
                    string value = result.Entities[i].Entity;
                    gender = treatment.getVisitsByGender(value);

                    if (gender == GenderValues.Male)
                    {
                        genderReturn = "homme(s)";
                    }
                    else
                    {
                        genderReturn = "femme(s)";
                    }
                }
            }

            visitsReturn = await client.GetVisitsForStats(date1, date2, gender, age1, age2);
            nbVisits = visitsReturn.Count();

            nbEntities = result.Entities.Count();
            for (int i = 0; i < nbEntities; i++)
            {
                if (result.Entities[i].Type == "Emotion")
                {
                    tmp = visitsReturn.ToList();
                    visitsReturn.Clear();
                    emotion = result.Entities[i].Entity;
                    //Pour le moment, on gère avec code (à modifier une fois Dico OK)
                    if (emotion == "heureux" || emotion == "heureuse" || emotion == "heureuses" || emotion == "souriant" || emotion == "souriants" || emotion == "souriante" || emotion == "souriantes")
                    {
                        emotion = "Happiness";
                        emotionReturn = "heureux(ses)";
                    }
                    if (emotion == "neutre" || emotion == "neutres")
                    {
                        emotion = "Neutral";
                        emotionReturn = "neutre(s)";
                    }
                    if (emotion == "triste" || emotion == "tristes")
                    {
                        emotion = "Sadness";
                        emotionReturn = "triste(s)";
                    }
                    if (emotion == "faché" || emotion == "fachés" || emotion == "fachée" || emotion == "fachées")
                    {
                        emotion = "Anger";
                        emotionReturn = "faché(es)";
                    }
                    if (emotion == "surpris" || emotion == "surprise" || emotion == "surprises")
                    {
                        emotion = "Surprise";
                        emotionReturn = "surpris(es)";
                    }

                    for (int y = 0; y < nbVisits; y++)
                    {
                        int nbEmotion = tmp[y].ProfilePicture.Count();
                        for (int z = 0; z < nbEmotion; z++)
                        {
                            if (customDialog.getEmotion(tmp[y].ProfilePicture[z].EmotionScore) == emotion &&
                            customDialog.getEmotion(tmp[y].ProfilePicture[z].EmotionScore) != null)
                            {
                                visitsReturn.Add(tmp[y]);
                                EmotionPicture.Add(tmp[y].ProfilePicture[z]);
                                askingEmotion = true;
                            }
                        }
                    }
                }
            }

            //NbPersonForReal
            nbVisits = visitsReturn.Count();
            int nbPerson = 0;
            nbPerson = treatment.getNbPerson(visitsReturn, nbPerson);

            //Return results
            if (nbPerson != 0)
            {
                replyToConversation = ((Activity)context.Activity).CreateReply("J'ai vu " + nbPerson + " " + genderReturn + " " + emotionReturn + " " + ageReturn + " " + dateReturn + ".");
                replyToConversation.Recipient = context.Activity.From;
                replyToConversation.Type = "message";
            }
            else
            {
                replyToConversation = ((Activity)context.Activity).CreateReply("Je n'ai croisé personne correspondant à ta description " + dateReturn + "... :/");
            }

            if (visitsReturn.Count() != 0)
            {
                replyToConversation.AttachmentLayout = "carousel";
                replyToConversation.Attachments = new List<Attachment>();
                int compteur = 0;

                int compteurCarrousel = 1;
                foreach (var visit in visitsReturn)
                {
                    if (compteurCarrousel <= 9)
                    {
                        List<CardImage> cardImages = new List<CardImage>();
                        if (!askingEmotion)
                        {
                            cardImages.Add(new CardImage(url: $"{ ConfigurationManager.AppSettings["WebAppUrl"] }{VirtualPathUtility.ToAbsolute(visit.ProfilePicture.Last().Uri)}")); // a mettre dans le SDK
                        }
                        else
                        {
                            cardImages.Add(new CardImage(url: $"{ ConfigurationManager.AppSettings["WebAppUrl"] }{VirtualPathUtility.ToAbsolute(EmotionPicture[compteur].Uri)}")); // a mettre dans le SDK
                        }
                        string messageDate = "";
                        string firstname = "";

                        if (visit.PersonVisit.FirstName == null)
                        {
                            firstname = "une personne inconnue";
                        }
                        else
                        {
                            firstname = visit.PersonVisit.FirstName;
                        }
                        messageDate = customDialog.GetVisitsMessage(firstname, visit.Date);

                        HeroCard plCard = new HeroCard()
                        {
                            Title = firstname,
                            Text = messageDate + " (" + Convert.ToString(visit.Date.ToString("dd/MM/yyyy")) + ")",
                            Images = cardImages
                        };

                        Attachment plAttachment = plCard.ToAttachment();
                        replyToConversation.Attachments.Add(plAttachment);
                        compteur++;
                        compteurCarrousel++;
                    }
                    else if (compteurCarrousel == 10)
                    {
                        List<CardImage> cardImages = new List<CardImage>();
                        cardImages.Add(new CardImage(""));

                        HeroCard plCard = new HeroCard()
                        {
                            Title = "Afficher plus (A venir)",
                            Text = "",
                            //Subtitle = 
                            Images = cardImages
                            //Buttons = cardButtons
                        };

                        Attachment plAttachment = plCard.ToAttachment();
                        replyToConversation.Attachments.Add(plAttachment);
                        compteurCarrousel += 1;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            await context.PostAsync(replyToConversation);
            context.Wait(MessageReceived);
        }

        [LuisIntent("GetVisitsPersonByFirstname")]
        public async Task GetVisitsPersonByFirstname(IDialogContext context, LuisResult result)
        {
            Activity replyToConversation = null;
            AdaClient client = new AdaClient();

            int nbVisit = 10;

            var splitResult = result.Query.Split(':');

            if (splitResult[0] == "ChoosePersonId ")
            {

                int idPerson = Convert.ToInt32(splitResult[1]);

                nbVisit = Convert.ToInt32(splitResult[3]);

                List<VisitDto> visitsById = await client.GetVisitPersonById(idPerson, nbVisit);

                replyToConversation = ((Activity)context.Activity).CreateReply("Je l'ai vu à ces dates:");
                replyToConversation.Recipient = context.Activity.From;
                replyToConversation.Type = "message";
                replyToConversation.AttachmentLayout = "carousel";
                replyToConversation.Attachments = new List<Attachment>();

                //Affichage
                int compteurCarrousel = 1;
                foreach (var visit in visitsById)
                {
                    if (compteurCarrousel <= 9)
                    {
                        List<CardImage> cardImages = new List<CardImage>();
                        cardImages.Add(new CardImage(url: $"{ ConfigurationManager.AppSettings["WebAppUrl"] }{VirtualPathUtility.ToAbsolute(visit.ProfilePicture.Last().Uri)}"));
                        
                        var customDialog = new CreateDialog();
                        var messageDate = customDialog.GetVisitsMessage(visit.PersonVisit.FirstName, visit.Date.AddHours(1));

                        HeroCard plCard = new HeroCard()
                        {
                            Title = visit.PersonVisit.FirstName,
                            Text = messageDate,
                            //Subtitle = 
                            Images = cardImages
                            //Buttons = cardButtons
                        };

                        Attachment plAttachment = plCard.ToAttachment();
                        replyToConversation.Attachments.Add(plAttachment);
                        compteurCarrousel += 1;
                    }
                    else if (compteurCarrousel == 10)
                    {
                        List<CardImage> cardImages = new List<CardImage>();
                        cardImages.Add(new CardImage(""));

                        HeroCard plCard = new HeroCard()
                        {
                            Title = "Afficher plus (A venir)",
                            Text = "",
                            //Subtitle = 
                            Images = cardImages
                            //Buttons = cardButtons
                        };

                        Attachment plAttachment = plCard.ToAttachment();
                        replyToConversation.Attachments.Add(plAttachment);
                        compteurCarrousel += 1;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                string firstname = null;

                int nbEntities = result.Entities.Count();
                for (int i = 0; i < nbEntities; i++)
                {
                    if (result.Entities[i].Type == "Firstname")
                    {
                        firstname = result.Entities[i].Entity;
                    }
                    if (result.Entities[i].Type == "Number")
                    {
                        nbVisit = Convert.ToInt32(result.Entities[i].Entity);
                    }
                }

                List<VisitDto> visits = await client.GetLastVisitPerson(firstname);

                if (visits.Count == 0)
                {
                    replyToConversation = ((Activity)context.Activity).CreateReply("Je n'ai pas encore rencontré " + firstname + " :/ Il faudrait nous présenter! ^^");
                    replyToConversation.Recipient = context.Activity.From;
                    replyToConversation.Type = "message";
                }
                else if (visits.Count == 1)
                {
                    int id = visits[0].PersonVisit.PersonId;

                    List<VisitDto> visitsById = await client.GetVisitPersonById(id, nbVisit);

                    replyToConversation = ((Activity)context.Activity).CreateReply("Je l'ai vu à ces dates:");
                    replyToConversation.Recipient = context.Activity.From;
                    replyToConversation.Type = "message";
                    replyToConversation.AttachmentLayout = "carousel";
                    replyToConversation.Attachments = new List<Attachment>();

                    //Affichage 
                    int compteurCarrousel = 1;
                    foreach (var visit in visitsById)
                    {
                        if (compteurCarrousel <= 9)
                        {
                            List<CardImage> cardImages = new List<CardImage>();
                            cardImages.Add(new CardImage(url: $"{ ConfigurationManager.AppSettings["WebAppUrl"] }{VirtualPathUtility.ToAbsolute(visit.ProfilePicture.Last().Uri)}"));

                            var customDialog = new CreateDialog();
                            var messageDate = customDialog.GetVisitsMessage(visit.PersonVisit.FirstName, visit.Date.AddHours(1));

                            HeroCard plCard = new HeroCard()
                            {
                                Title = visit.PersonVisit.FirstName,
                                Text = messageDate,
                                //Subtitle = 
                                Images = cardImages
                                //Buttons = cardButtons
                            };

                            Attachment plAttachment = plCard.ToAttachment();
                            replyToConversation.Attachments.Add(plAttachment);
                            compteurCarrousel += 1;
                        }
                        else if (compteurCarrousel == 10)
                        {
                            List<CardImage> cardImages = new List<CardImage>();
                            cardImages.Add(new CardImage(""));

                            HeroCard plCard = new HeroCard()
                            {
                                Title = "Afficher plus (A venir)",
                                Text = "",
                                //Subtitle = 
                                Images = cardImages
                                //Buttons = cardButtons
                            };

                            Attachment plAttachment = plCard.ToAttachment();
                            replyToConversation.Attachments.Add(plAttachment);
                            compteurCarrousel += 1;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    replyToConversation = ((Activity)context.Activity).CreateReply("Je connais " + visits.Count + " " + firstname + ". Les voici :)");
                    replyToConversation.Recipient = context.Activity.From;
                    replyToConversation.Type = "message";
                    replyToConversation.AttachmentLayout = "carousel";
                    replyToConversation.Attachments = new List<Attachment>();

                    foreach (var visit in visits)
                    {
                        List<CardImage> cardImages = new List<CardImage>();
                        cardImages.Add(new CardImage(url: $"{ ConfigurationManager.AppSettings["WebAppUrl"] }{VirtualPathUtility.ToAbsolute(visit.ProfilePicture.Last().Uri)}")); // a mettre dans le SDK

                        List<CardAction> cardButtons = new List<CardAction>();

                        CardAction plButtonChoice = new CardAction()
                        {

                            Value = "ChoosePersonId :" + visit.PersonVisit.PersonId + " : nbVisit :" + nbVisit,
                            Type = "postBack",
                            Title = "Le voilà !"
                        };
                        cardButtons.Add(plButtonChoice);

                        HeroCard plCard = new HeroCard()
                        {
                            Title = visit.PersonVisit.FirstName,
                            Images = cardImages,
                            Buttons = cardButtons
                        };

                        Attachment plAttachment = plCard.ToAttachment();
                        replyToConversation.Attachments.Add(plAttachment);
                    }
                }
            }

            await context.PostAsync(replyToConversation);
            context.Wait(MessageReceived);
        }

        [LuisIntent("GetAverageVisits")]
        public async Task GetAverageVisits(IDialogContext context, LuisResult result)
        {
            AdaClient client = new AdaClient();

            GenderValues? gender;
            int? age1 = null;
            string ageReturn1 = "null";
            int? age2 = null;
            string ageReturn2 = "null";
            string ageReturn = "";
            string genderReturn = "null";

            int nbEntities = result.Entities.Count();
            for (int i = 0; i < nbEntities; i++)
            {
                if (result.Entities[i].Type == "Gender")
                {
                    string value = result.Entities[i].Entity;

                    gender = GenderValues.Male;
                    if (value == "femme" || value == "femmes" || value == "fille" || value == "filles")
                    {
                        gender = GenderValues.Female;
                    }
                    genderReturn = Convert.ToString(gender.Value);

                }
            }

            if (result.CompositeEntities != null)
            {
                nbEntities = result.CompositeEntities.Count();
                for (int i = 0; i < nbEntities; i++)
                {
                    //Process of ages
                    if (result.CompositeEntities[i].ParentType == "SingleAge")
                    {
                        age1 = Convert.ToInt32(result.CompositeEntities[i].Children[0].Value);
                        ageReturn1 = Convert.ToString(age1);
                        ageReturn = " de " + age1 + " ans";
                    }
                    else if (result.CompositeEntities[i].ParentType == "IntervalAge")
                    {
                        age1 = Convert.ToInt32(result.CompositeEntities[i].Children[0].Value);
                        age2 = Convert.ToInt32(result.CompositeEntities[i].Children[1].Value);
                        if (age2 < age1 && age2 != -1)
                        {
                            int? ageTmp = age1;
                            age1 = age2;
                            age2 = ageTmp;
                        }
                        ageReturn1 = Convert.ToString(age1);
                        ageReturn2 = Convert.ToString(age2);

                        ageReturn = " entre " + age1 + " et " + age2 + " ans";
                    }
                }

            }

            int nbVisits = await client.GetNbVisits(genderReturn, ageReturn1, ageReturn2);

            if (genderReturn == "null")
            {
                genderReturn = "personne(s)";
            }
            else if (genderReturn == Convert.ToString(GenderValues.Female))
            {
                genderReturn = "femme(s)";
            }
            else
            {
                genderReturn = "homme(s)";
            }

            string message = "Je vois en moyenne : " + nbVisits + " " + genderReturn + " " + ageReturn + " par jour.";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }
    }
}
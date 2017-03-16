using AdaBot.Models.EventsLoaderServices;
using AdaSDK.Models;
using HtmlAgilityPack;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace AdaBot.Dialogs
{
    public class CreateDialog
    {
        public CreateDialog()
        {

        }

        public Activity CarouselPossibilities(IDialogContext context)
        {
            List<string> OurPossibilities = new List<string>();

            Activity replyToConversation;

            replyToConversation = ((Activity)context.Activity).CreateReply("Je suis capable de te renseigner sur pas mal de chose! :D Tu peux me demander:");
            replyToConversation.Recipient = context.Activity.From;
            replyToConversation.Type = "message";
            replyToConversation.AttachmentLayout = "carousel";
            replyToConversation.Attachments = new List<Attachment>();

            List<string> pictures = new List<string>();
            pictures.Add(ConfigurationManager.AppSettings["IMGMore"]);
            pictures.Add(ConfigurationManager.AppSettings["IMGMore"]);
            pictures.Add(ConfigurationManager.AppSettings["IMGMeetup"]);
            pictures.Add(ConfigurationManager.AppSettings["IMGMIC"]);
            pictures.Add(ConfigurationManager.AppSettings["IMGMore"]);

            List<string> btnAction = new List<string>();
            btnAction.Add("Liste visite jour");
            btnAction.Add("Combien de personne viennent en moyenne au MIC?");
            btnAction.Add("On fait un truc?");
            btnAction.Add("tu as des infos?");
            btnAction.Add("Qui est ton meilleur ami?");

            List<string> btnString = new List<string>();
            btnString.Add("La liste des visites du jour");
            btnString.Add("Le nombre de visiteurs moyen du MIC");
            btnString.Add("La liste des évènements du MIC");
            btnString.Add("Des informations concernant ma maison, le MIC");
            btnString.Add("Mon meilleur ami");

            for (int i = 0; i < btnAction.Count(); i++)
            {
                List<CardAction> cardsAction = new List<CardAction>();
                CardAction action = new CardAction()
                {
                    Value = btnAction[i].ToString(),
                    Type = "postBack",
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

            return replyToConversation;
        }

        public Activity CarouselPossibilitiesNotAllowed(IDialogContext context)
        {
            List<string> OurPossibilities = new List<string>();

            Activity replyToConversation;

            replyToConversation = ((Activity)context.Activity).CreateReply("Je suis capable de te renseigner sur pas mal de chose! :D Tu peux me demander:");
            replyToConversation.Recipient = context.Activity.From;
            replyToConversation.Type = "message";
            replyToConversation.AttachmentLayout = "carousel";
            replyToConversation.Attachments = new List<Attachment>();

            List<string> pictures = new List<string>();
            pictures.Add(ConfigurationManager.AppSettings["IMGMeetup"]);
            pictures.Add(ConfigurationManager.AppSettings["IMGMIC"]);

            List<string> btnAction = new List<string>();
            btnAction.Add("On fait un truc?");
            btnAction.Add("tu as des infos?");

            List<string> btnString = new List<string>();
            btnString.Add("La liste des évènements du MIC");
            btnString.Add("Des informations concernant ma maison, le MIC");

            for (int i = 0; i < btnAction.Count(); i++)
            {
                List<CardAction> cardsAction = new List<CardAction>();
                CardAction action = new CardAction()
                {
                    Value = btnAction[i].ToString(),
                    Type = "postBack",
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

            return replyToConversation;
        }

        public Activity GetHelp(IDialogContext context)
        {
            Activity replyToConversation;
            replyToConversation = ((Activity)context.Activity).CreateReply("En quoi puis-je t'aider? :)");
            replyToConversation.Recipient = context.Activity.From;
            replyToConversation.Type = "message";
            replyToConversation.AttachmentLayout = "carousel";
            replyToConversation.Attachments = new List<Attachment>();

            List<string> pictures = new List<string>();
            pictures.Add(ConfigurationManager.AppSettings["IMGFacebook"]);
            pictures.Add(ConfigurationManager.AppSettings["IMGYoutube"]);
            pictures.Add(ConfigurationManager.AppSettings["IMGMeetup"]);
            pictures.Add(ConfigurationManager.AppSettings["IMGMIC"]);

            List<string> btnAction = new List<string>();
            btnAction.Add(ConfigurationManager.AppSettings["FaceBookMIC"]);
            btnAction.Add(ConfigurationManager.AppSettings["YoutubeMIC"]);
            btnAction.Add(ConfigurationManager.AppSettings["MeetupMIC"]);
            btnAction.Add(ConfigurationManager.AppSettings["SiteMIC"]);

            List<string> btnString = new List<string>();
            btnString.Add("Notre Facebook");
            btnString.Add("Notre chaîne Youtube");
            btnString.Add("Notre Meetup");
            btnString.Add("Notre Site");

            for (int i = 0; i < btnAction.Count(); i++)
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

            return replyToConversation;
        }

        public async Task<Activity> GetEvent(IDialogContext context)
        {
            List<MeetupEvent> _eventList = new List<MeetupEvent>();
            TreatmentDialog treatment = new TreatmentDialog();
            _eventList = await treatment.getEvents();

            Activity replyToConversation;
            replyToConversation = ((Activity)context.Activity).CreateReply("Voici la liste des évènements à venir au MIC: ");
            replyToConversation.Recipient = context.Activity.From;
            replyToConversation.Type = "message";
            replyToConversation.AttachmentLayout = "carousel";
            replyToConversation.Attachments = new List<Attachment>();

            foreach (var meetup in _eventList)
            {
                //Récupération du lien image
                List<string> possiblePictures = new List<string>();

                foreach (Match m in Regex.Matches(meetup.Description, "<a.+?href=[\"'](.+?)[\"'].+?>", RegexOptions.IgnoreCase | RegexOptions.Multiline))
                {
                    string src = m.Groups[1].Value;
                    //string tmp = treatment.getHtmlSourceCode(src);
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(src);
                    string tmp = "";
                    try
                    {
                        tmp = treatment.getHtmlSourceCode(src);
                    }
                    catch (Exception e)
                    {
                        tmp = "";
                    }

                    foreach (Match m2 in Regex.Matches(tmp, "<img.+?src=[\"'](.+?)[\"'].+?>", RegexOptions.IgnoreCase | RegexOptions.Multiline))
                    {
                        string src2 = m2.Groups[1].Value;
                        possiblePictures.Add(src2);
                    }
                }

                List<CardImage> cardImages = new List<CardImage>();
                if (possiblePictures.Count == 0)
                {
                    cardImages.Add(new CardImage(url: $"{ConfigurationManager.AppSettings["IMGMIC"]}"));
                }
                else
                {
                    cardImages.Add(new CardImage(url: Convert.ToString(possiblePictures[0])));
                }

                List<CardAction> cardsAction = new List<CardAction>();
                CardAction action = new CardAction()
                {
                    Value = meetup.Link,
                    Type = "openUrl",
                    Title = "Consulter"
                };
                cardsAction.Add(action);

                DateTime date = new DateTime(1970, 1, 1).Add(TimeSpan.FromMilliseconds((meetup.Time))).AddHours(2);

                HeroCard plCard = new HeroCard()
                {
                    Title = meetup.Name + " (" + date + ")",
                    Text = "Lieux: " + meetup.Venue.Name + " " + meetup.Venue.City,
                    //Subtitle = Regex.Replace(meetup.Description, @"<(.|\n)*?>", string.Empty),
                    Subtitle = meetup.HowToFind,
                    Images = cardImages,
                    Buttons = cardsAction
                };

                Attachment plAttachment = plCard.ToAttachment();
                replyToConversation.Attachments.Add(plAttachment);
            }

            return replyToConversation;
        }

        public string GetVisitsMessage(string firstname, DateTime dateVisit)
        {
            string message;

            var diffDate = DateTime.Now - dateVisit;
            if(diffDate.TotalMinutes < 1)
            {
                message = "Je vois " + firstname + " en ce moment. :)";
            }
            else if(diffDate.TotalMinutes < 60)
            {
                message = "J'ai croisé " + firstname + " il y a " + Convert.ToInt32(diffDate.Minutes) + " minute(s)";
            }
            else if(diffDate.TotalHours < 24)
            { 
                message = "J'ai croisé " + firstname + " il y a " + Convert.ToInt32(diffDate.Hours) + " heure(s)";
            }
            else if (diffDate.TotalDays < 30)
            {
                message = "J'ai croisé " + firstname + " il y a " + Convert.ToInt32(diffDate.Days) + " jour(s)";
            }
            else if(diffDate.TotalDays < 366) 
            {
                int nbMonth = Convert.ToInt32((diffDate.TotalDays) / 30);

                message = "J'ai croisé " + firstname + " il y a " + nbMonth + " mois";
            }
            else
            {
                int nbYears = Convert.ToInt32((diffDate.TotalDays) / 365);

                message = "J'ai croisé " + firstname + " il y a " + nbYears + " année(s)";
            }


            return message;
        }
        public string getEmotion(EmotionDto emotion)
        { 
            if (emotion != null)
            {
                string result = "Neutral";
                double value = 0.75 ;

                if (emotion.Sadness > value)
                {
                    result = "Sadness";
                }
                if (emotion.Happiness > value)
                {
                    result = "Happiness";
                }
                if (emotion.Surprise > value)
                {
                    result = "Surprise";
                }
                if (emotion.Anger > value)
                {
                    result = "Anger";
                }
                return result;
            }
            return null;
        }
    }
}
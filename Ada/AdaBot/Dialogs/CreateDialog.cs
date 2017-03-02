using AdaSDK.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdaBot.Dialogs
{
    public class CreateDialog
    {
        public CreateDialog()
        {

        }

        public string GetVisitsMessage(string firstname, DateTime dateVisit)
        {
            string message;
            //dateVisit = dateVisit.AddHours(1);

            var diffDate = DateTime.Now - dateVisit;

            if(diffDate.TotalMinutes < 60)
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
                float test = emotion.Happiness;
                string result = "Happiness";
                if (test < emotion.Sadness)
                {
                    test = emotion.Sadness;
                    result = "Sadness";
                }
                if (test < emotion.Neutral)
                {
                    test = emotion.Neutral;
                    result = "Neutral";
                }
                if (test < emotion.Surprise)
                {
                    test = emotion.Surprise;
                    result = "Surprise";
                }
                if (test < emotion.Anger)
                {
                    test = emotion.Anger;
                    result = "Anger";
                }
                return result;
            }
            return null;
        }
    }
}
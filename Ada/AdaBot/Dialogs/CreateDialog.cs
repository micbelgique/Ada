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
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

            if (dateVisit.Day == DateTime.Today.Day)
            {
                if (dateVisit.Hour < 12)
                {
                    message = "J'ai croisé " + firstname + " ce matin.";
                }
                else if (dateVisit.Hour >= 12 && dateVisit.Hour <= 17)
                {
                    message = "J'ai croisé " + firstname + " cet après-midi.";
                }
                else
                {
                    message = "J'ai croisé " + firstname + " cette nuit... Il doit sûrement faire des heures sup'!";
                }
            }
            else if (dateVisit.Day == DateTime.Today.Day - 1)
            {
                if (dateVisit.Hour < 12)
                {
                    message = "J'ai croisé " + firstname + " hier matin.";
                }
                else if (dateVisit.Hour >= 12 && dateVisit.Hour <= 17)
                {
                    message = "J'ai croisé " + firstname + " hier après-midi.";
                }
                else
                {
                    message = "J'ai croisé " + firstname + " la nuit dernière... Il doit sûrement faire des heures sup'!";
                }
            }
            else
            {
                int monthDiff;
                int dayDiff;
                if (DateTime.Today.Year == dateVisit.Year)
                {
                    if(DateTime.Today.Month == dateVisit.Month)
                    {
                        dayDiff = DateTime.Today.Day - dateVisit.Day;

                        message = "J'ai croisé " + firstname + " il y a " + dayDiff + " jours.";
                    }
                    else
                    {
                        monthDiff = DateTime.Today.Month - dateVisit.Month;

                        if (monthDiff == 1)
                        {
                            message = "J'ai croisé " + firstname + " le mois passé.";
                        }
                        else
                        {
                            message = "J'ai croisé " + firstname + " il y a " + monthDiff + " mois.";
                        }
                    }
                }
                else
                {
                    int yearDiff;
                    yearDiff = DateTime.Today.Year - dateVisit.Year; 

                    if (yearDiff == 1)
                    {

                    }

                }

                dayDiff = DateTime.Today.Day - dateVisit.Day;

                message = "J'ai croisé " + firstname + " il y a " + dayDiff + " jours.";
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
                return result;
            }
            return null;
        }
    }
}
using AdaBot.Models.EventsLoaderServices;
using AdaSDK;
using AdaSDK.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AdaBot.Dialogs
{
    public class TreatmentDialog
    {
        // Services
        public IEventsLoaderService EventsMeetupLoaderService { get; set; }

        // Properties
        private List<MeetupEvent> _eventList = new List<MeetupEvent>();

        public TreatmentDialog()
        {

        }

        public GenderValues getVisitsByGender(string valueGender)
        {
            if (valueGender == "femme" || valueGender == "femmes" || valueGender == "fille" || valueGender == "filles")
            {
                return GenderValues.Female;
            }

            else return GenderValues.Male;
        }

        public int getNbPerson(List<VisitDto> visitsReturn, int nbPerson)
        {
            List<int> listID = new List<int>();
            List<int> listIDVisit = new List<int>();
            foreach (var visit in visitsReturn)
            {
                if (!listID.Contains(visit.PersonVisit.PersonId) && !listIDVisit.Contains(visit.ID))
                {
                    listID.Add(visit.PersonVisit.PersonId);
                    listIDVisit.Add(visit.ID);
                    nbPerson += 1;
                }
            }

            return nbPerson;
        }

        public async Task<List<MeetupEvent>> getEvents() 
        {
            EventsMeetupLoaderService = new EventsLoaderService();
            _eventList = await EventsMeetupLoaderService.GetEventsJsonAsync(20);
            return _eventList;
        }

        public string GetValueButton(DateTime? date1, DateTime? date2, GenderValues? gender, int? age1, int? age2)
        {
            string returnDate1 = "null";
            string returnDate2 = "null";
            string returnGender = "null";
            string returnAge1 = "null";
            string returnAge2 = "null";

            if(date1 != null)
            {
                DateTime date1bis = Convert.ToDateTime(date1);
                returnDate1 = Convert.ToString(date1bis.ToString("yyyy/MM/dd"));

                if(date2 != null)
                {
                    DateTime date2bis = Convert.ToDateTime(date2);
                    returnDate2 = Convert.ToString(date2bis.ToString("yyyy/MM/dd"));
                }
            }
            if(gender != null)
            {
                returnGender = Convert.ToString(gender.Value);
            }
            if(age1 != null)
            {
                returnAge1 = Convert.ToString(age1);

                if(age2 != null)
                {
                    returnAge2 = Convert.ToString(age2);
                }
            }


            return returnDate1 + ":" + returnDate2 + ":" + returnGender + ":" + returnAge1 + ":" + returnAge2;
        }

        public string getHtmlSourceCode(string url)
        {
            using (WebClient client = new WebClient())
            {
                return client.DownloadString(url);
            }
        }
    }
}
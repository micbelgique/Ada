using AdaBot.Models.EventsLoaderServices;
using AdaSDK;
using AdaSDK.Models;
using System;
using System.Collections.Generic;
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
            _eventList = await EventsMeetupLoaderService.GetEventsJsonAsync(10);
            return _eventList;
        }
    }
}
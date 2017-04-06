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

        public string GetValueButton(DateTime? date1, DateTime? date2, GenderValues? gender, int? age1, int? age2, bool glasse, bool beard, bool moustache)
        {
            string returnDate1 = "null";
            string returnDate2 = "null";
            string returnGender = "null";
            string returnAge1 = "null";
            string returnAge2 = "null";
            string returnGlasses = "null";
            string returnBeard = "null";
            string returnMoustache = "null";

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
            if (glasse)
            {
                returnGlasses = "true";
            }
            if (beard)
            {
                returnBeard = "true";
            }
            if (moustache)
            {
                returnMoustache = "true";
            }


            return returnDate1 + ":" + returnDate2 + ":" + returnGender + ":" + returnAge1 + ":" + returnAge2 + ":" + returnGlasses + ":" + returnBeard + ":" + returnMoustache;
        }

        public string getHtmlSourceCode(string url)
        {
            using (WebClient client = new WebClient())
            {
                return client.DownloadString(url);
            }
        }

        public string getResponseToInfo(string question)
        {
            if (question == "Que_fait_le_MIC")
            {
                return "Le Mic accompagne les PME qui le désire dans le développement de leurs projets";
            }
            else if (question == "Que_faire_au_MIC")
            {
                return "Il est possible de demander un accompagnement dans le cadre d'un projet personnel ou de participer à un des nombreux événements organisés par le Mic";
            }
            else if (question == "Quels_sont_les_horaires_du_MIC")
            {
                return "Le MIC est ouvert du lundi au vendredi de 9h à 17h :)";
            }
            else if (question == "Comment_postuler_pour_un_stage")
            {
                return "Ton stage doit durer 4 mois et pour postuler, il te suffit de prendre contact avec nous. Nous te proposerons alors de venir passer un petit test :)";
            }
            else if (question == "Comment_se_déroule_un_stage")
            {
                return "Chaque stagiaire se voit attribuer un projet et est intégré au sein d'une petite équipe. Le projet s'inscrit dans un cadre authentique puisqu'il s'agit bien de réaliser un produit pour un client réel";
            }
            else if (question == "Du_travail_pour_moi")
            {
                return "Je n'ai pas encore de réponse à cette question :/";
            }
            else if (question == "Comment_travailler_ici")
            {
                return "Pour travailler au Mic il te suffit de louer une de nos salle de réunion ou de passer nous voir le vendredi quand tout nos bureaux sont en mode 'Open Space'! :D";
            }
            else if (question == "Comment_louer_une_salle")
            {
                return "Si tu désires louer une salle au MIC, il te suffit de contacter Martine, notre Office Manager. Elle sera ravie de t'aider et en plus, c'est gratuit! :D";
            }
            else if (question == "A_qui_parler_de_mon_projet")
            {
                return "Tu peux en parler à Frédéric Carbonelle. Il saura quoi faire! ;)";
            }
            else if (question == "Comment_se_passe_mon_accompagnement_dans_mon_projet")
            {
                return "Le Mic te propose un programme d'accompagnement complet d'une durée de 7 mois! Pour plus d'informations, je t'invite à consulter notre site à ce propos: http://www.digitalboostcamp.be/";
            }
            else
            {
                return "Je n'ai pas encore de réponse à ta question :/";
            }
        }
    }
}
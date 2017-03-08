using AdaSDK;
using AdaSDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdaBot.Dialogs
{
    public class TreatmentDialog
    {
        public TreatmentDialog()
        {

        }

        public List<VisitDto> getVisitsByGender(string valueGender, List<VisitDto> tmp, List<VisitDto> visitsReturn, int nbVisits)
        {
            GenderValues gender = GenderValues.Male;
            if (valueGender == "femme" || valueGender == "femmes" || valueGender == "fille" || valueGender == "filles")
            {
                gender = GenderValues.Female;
            }

            for (int y = 0; y < nbVisits; y++)
            {
                if (tmp[y].PersonVisit.Gender == gender)
                {
                    visitsReturn.Add(tmp[y]);
                }
            }

            return visitsReturn;
        }

        public int getNbPerson(List<VisitDto> visitsReturn, int nbPerson)
        {
            List<int> listID = new List<int>();
            foreach (var visit in visitsReturn)
            {
                if (!listID.Contains(visit.ID))
                {
                    listID.Add(visit.ID);
                    nbPerson += 1;
                }
            }

            return nbPerson;
        }
    }
}
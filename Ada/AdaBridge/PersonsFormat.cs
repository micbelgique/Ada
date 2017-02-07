using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdaBridge
{
    class PersonsFormat
    {
        public PersonDto[] persons { get; set; }
        public PersonDto[] IdentifiedPersons { get; set; }
        public PersonDto[] UnknownPersons { get; set; }

        public int NbrOfUnknownMale { get; set; }
        public int NbrOfUnknownFemale { get; set; }

        private bool isEmpty;
        private bool hasOnlyOneIdentifiedPerson;
        private bool hasOnlyOneUnknownPerson;
        private bool hasUnknownMaleAndNoUnknownFemale;
        private bool hasUnknownFemaleAndNoUnknownMale;
        private bool hasUnknownFemaleAndUnknownMale;

        #region Custom Getters

        public bool IsEmpty
        {
            get
            {
                return persons.Length == 0;
            }
        }

        public bool HasOnlyOneIdentifiedPerson {
            get
            {
                return IdentifiedPersons.Length == 1 && UnknownPersons.Length == 0;
            }
        }

        public bool HasOnlyOneUnknownPerson
        {
            get
            {
                return IdentifiedPersons.Length == 0 && UnknownPersons.Length == 1;
            }
        }

        public bool HasUnknownMaleAndNoUnknownFemale
        {
            get
            {
                return NbrOfUnknownMale > 0 && NbrOfUnknownFemale == 0;
            }
        }

        public bool HasUnknownFemaleAndNoUnknownMale
        {
            get
            {
                return NbrOfUnknownMale == 0 && NbrOfUnknownFemale > 0;
            }
        }

        public bool HasUnknownFemaleAndUnknownMale
        {
            get
            {
                return NbrOfUnknownMale > 0 && NbrOfUnknownFemale > 0;
            }
        }

        #endregion

        public PersonsFormat(PersonDto[] persons)
        {
            this.persons = persons;
            Initialize();
        }

        #region Initializations

        private void Initialize()
        {
            SplitPersons(persons);
            calculateGenders();
        }

        private void SplitPersons(PersonDto[] persons)
        {
            IdentifiedPersons = (from p in persons
                                 where p.FirstName != null
                                 select p).ToArray();
            UnknownPersons = (from p in persons
                              where p.FirstName == null
                              select p).ToArray();
        }

        private void calculateGenders()
        {
            // Count number of unknown person
            NbrOfUnknownMale = (from p in UnknownPersons
                                  where p.Gender == GenderValues.Male
                                  select p).Count();

            NbrOfUnknownFemale = (from p in UnknownPersons
                                    where p.Gender == GenderValues.Female
                                    select p).Count();
        }

        #endregion

        public override string ToString()
        {
            string toString                 = "";
            string identifiedPersonsInfo    = "";
            string unknownPersonsInfos      = "";

            // 1. Checking simple toString like empty array or with only one person
            if(IsEmpty)
            {
                toString = "Il n'y a personne :'(";
            }
            else if (HasOnlyOneIdentifiedPerson)
            {
                toString = $"Voici {IdentifiedPersons[0].FirstName} pour la {IdentifiedPersons[0].NbPasses} fois aujourd'hui.";
            }
            else if (HasOnlyOneUnknownPerson)
            {
                var sexe = UnknownPersons[0].Gender == GenderValues.Male ? "un homme" : "une femme";
                toString = $"Voilà {sexe} de {UnknownPersons[0].Age} ans que je ne connais pas.";
            }
            else
            {
                // 2. Getting identified persons info (list of names)
                identifiedPersonsInfo = ConcatNames(IdentifiedPersons);
                
                // 3. Getting unknown persons info (number of each gender)
                if (HasUnknownMaleAndNoUnknownFemale) // plusieurs hommes, 0 femmes
                {
                    unknownPersonsInfos = $"{NbrOfUnknownMale} {(NbrOfUnknownMale > 1 ? "hommes" : "homme")} que je ne connais pas.";
                }
                else if (HasUnknownFemaleAndNoUnknownMale) // 0 hommes, plusieurs femmes
                {
                    unknownPersonsInfos = $"{NbrOfUnknownFemale} {(NbrOfUnknownFemale > 1 ? "femmes" : "femme")}  que je ne connais pas.";
                }
                else if (HasUnknownFemaleAndUnknownMale) // plusieurs hommes, plusieurs femmes
                {
                    unknownPersonsInfos = $"{NbrOfUnknownFemale} {(NbrOfUnknownFemale > 1 ? "femmes" : "femme")}  et {NbrOfUnknownMale} {(NbrOfUnknownMale > 1 ? "hommes" : "homme")}  inconnus";
                }

                // 4. Concat identified and unknown if necessary
                if (identifiedPersonsInfo != "" && unknownPersonsInfos == "")
                {
                    toString = "Voici " + identifiedPersonsInfo;
                }
                else if (identifiedPersonsInfo == "" && unknownPersonsInfos != "")
                {
                    toString = "Voilà " + unknownPersonsInfos;
                }
                else
                {
                    toString = "Voilà " + identifiedPersonsInfo + " ainsi que " + unknownPersonsInfos;
                }
            }

            return toString;
        }

        private string ConcatNames(PersonDto[] persons)
        {
            string IdentitfiedPersonsNames = "";
            var cpter = 1;
            foreach (var person in IdentifiedPersons)
            {
                IdentitfiedPersonsNames += person.FirstName;
                if (IdentifiedPersons.Length - cpter == 1)
                {
                    IdentitfiedPersonsNames += " et ";
                }
                else if (IdentifiedPersons.Length - cpter > 1)
                {
                    IdentitfiedPersonsNames += ", ";
                }
                cpter++;
            }
            return IdentitfiedPersonsNames;
        }
    }
}

using GalaSoft.MvvmLight.Messaging;
using MartineobotBridge;
using MartineobotIOTMvvm.Messages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MartineobotIOTMvvm.Helper
{
    public static class LogHelper
    {
        public static void Log(object message)
        {
            Messenger.Default.Send(new LogMessage {Message = $"\n{message}"});
        }

        public static void LogPerson(PersonDto person)
        {
            // Mettre un random sur la structure de la phrase + récupérer un tableau pour les personnes multiples
            if(person.FirstName != null)
            {
                Messenger.Default.Send(new LogMessage { Message = $"\nVoici {person.FirstName} pour la {person.NbPasses} fois aujourd'hui." });
            }
            else
            {
                var sexe = person.Gender == GenderValues.Male ? "un homme" : "une femme";
                Messenger.Default.Send(new LogMessage { Message = $"\nVoilà {sexe} de {person.Age} ans que je ne connais pas" });
            }
            
        }

        public static void LogPersons(PersonDto[] persons)
        {
            PersonsFormat personsFormat = new PersonsFormat(persons);
            Messenger.Default.Send(new LogMessage { Message = $"\n{personsFormat}" });
        }
/*
        public static void LogPersons(PersonDto[] persons)
        {
            string LogMessageContent = "";

            PersonDto[] IdentifiedPersons = (from p in persons
                                             where p.FirstName != null
                                             select p).ToArray();
            PersonDto[] UnknownPersons = (from p in persons
                                          where p.FirstName == null
                                          select p).ToArray();

            if (IdentifiedPersons.Length == 1 && UnknownPersons.Length == 0)
            {
                LogMessageContent = $"Voici {IdentifiedPersons[0].FirstName} pour la {IdentifiedPersons[0].NbPasses} fois aujourd'hui.";
            }
            else if (IdentifiedPersons.Length == 0 && UnknownPersons.Length == 1)
            {
                var sexe = UnknownPersons[0].Gender == GenderValues.Male ? "un homme" : "une femme";
                LogMessageContent = $"Voilà {sexe} de {UnknownPersons[0].Age} ans que je ne connais pas.";
            }
            else
            {
                // Concat identified persons names
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

                // Count number of unknown person
                string UnknownPersonsInfos = "";
                var UnknownMaleNbr = (from p in UnknownPersons
                                      where p.Gender == GenderValues.Male
                                      select p).Count();

                var UnknownFemaleNbr = (from p in UnknownPersons
                                        where p.Gender == GenderValues.Female
                                        select p).Count();



                if (UnknownMaleNbr > 0 && UnknownFemaleNbr == 0) // plusieurs hommes, 0 femmes
                {
                    UnknownPersonsInfos = $"{UnknownMaleNbr} {(UnknownMaleNbr>1? "hommes" : "homme")} que je ne connais pas.";
                }
                else if (UnknownMaleNbr == 0 && UnknownFemaleNbr > 0) // 0 hommes, plusieurs femmes
                {
                    UnknownPersonsInfos = $"{UnknownFemaleNbr} {(UnknownMaleNbr > 1 ? "femmes" : "femme")}  que je ne connais pas.";
                }
                else // plusieurs hommes, plusieurs femmes
                {
                    UnknownPersonsInfos = $"{UnknownFemaleNbr} {(UnknownMaleNbr > 1 ? "femmes" : "femme")}  et {UnknownMaleNbr} {(UnknownMaleNbr > 1 ? "hommes" : "homme")}  inconnus";
                }

                if(IdentitfiedPersonsNames != "" && UnknownPersonsInfos == "")
                {
                    LogMessageContent = "Voici " + IdentitfiedPersonsNames;
                }
                else if (IdentitfiedPersonsNames == "" && UnknownPersonsInfos != "")
                {
                    LogMessageContent = "Voilà " + UnknownPersonsInfos;
                }
                else
                {
                    LogMessageContent = "Voilà " + IdentitfiedPersonsNames + " ainsi que " + UnknownPersonsInfos;
                }                
            }

            Messenger.Default.Send(new LogMessage { Message = $"\n{LogMessageContent}" });
        }
*/

        public static void Log<T>(object message)
        {
        //    Log($"{typeof (T).Name} : {message}");
            Log($"{message}");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdaBridge;

namespace AdaW10.Models.VoiceInterface.TextToSpeech
{
    public static class SpeechDictionnary
    {
        private const int LimitPersonsToWelcom = 3;

        #region Random sentences
        private static readonly List<string> RandomMaleSentences = new List<string>
        {
            "Bienvenue au D T A !",
            "Bienvenue au digital boostcampe !",
            "Passez une bonne journée !",
            "Bienvenue au MIC.",
            "N'hésitez pas à vous prendre un petit café.",
            "Je suis ADA, votre assistante numérique içi au MIC.",
            "De l'eau est à votre disposition derrière le mur."
            /*
            "Que la force soit avec toi !",
            "Tu es rayonnant aujourd'hui.",
            "J'espère que tout se passe bien pour toi",
            "Et la famille ?",
            "Que la journée soit bonne.",
            "Tranquille mon pote ?",
            "Alors, ça farte ?",
            "On se fait un Douroum bientôt ?",
            "Winteur is cauming...",
            "Paix et félicité.",
            "Force et robustesse.",
            "Avé toi.",
            "Longue vie et prospérité.",
            "Je suis ADA, l'assistante de Martine.",
            "Je suis ADA, ravie de faire ta connaissance.",
            "On ne se serait pas déjà rencontré quelque part ?",
            "çava bébé ?",
            "Auto destruction dans. 5. 4. 3. 2. 1. Je rigole. Ha. ha. ha. ha.",
            "On va au Brasse-temps ?",
            "Attention derrière toi !",
            "Bonjour, je suis ADA, je suis parfaite !"
            */
        };

        private static readonly List<string> RandomFemaleSentences = new List<string>
        {
            "Bienvenue au D T A !",
            "Bienvenue au digital boostcampe !",
            "Passez une bonne journée !",
            "Bienvenue au MIC.",
            "N'hésitez pas à vous prendre un petit café.",
            "Je suis ADA, votre assistante numérique içi au MIC.",
            "De l'eau est à votre disposition derrière le mur."

            /*"Passe une bonne journée !",
            "Que la force soit avec toi !",
            "Tu es rayonnante aujourd'hui.", 
            "J'espère que tout se passe bien pour toi.",
            "Et la famille ?",
            "Que la journée soit bonne.",
            "Tranquille cousine ?",
            "Alors, ça farte ?",
            "On se fait un Douroum bientôt ?",
            "Winteur is cauming...",
            "Bienvenue au MIC.",
            "Paix et félicité.",
            "Force et robustesse.",
            "Avé toi.",
            "Longue vie et prospérité.",
            "N'hésite pas à te prendre un petit café.",
            "De l'eau est à ta disposition derrière le mur.",
            "Je suis ADA, l'assistante de Martine",
            "Je suis ADA, ravie de faire ta connaissance.",
            "Je suis ADA, ton assistante numérique içi au MIC.",
            "On ne se serait pas déjà rencontré quelque part ?",
            "çava bébé ?",
            "Auto destruction dans 5. 4. 3. 2. 1. Je rigole. Ha. ha. ha. ha.",
            "On va au Brasse-temps ?",
            "Attention derrière toi !",
            "Bonjour, je suis ADA, je suis parfaite !" */
        };

        private static readonly List<string> RandomSentences = new List<string>
        {
            "Bienvenue au D T A !",
            "Bienvenue au digital boostcampe !",
            "Passez une bonne journée !",
            "Bienvenue au MIC.",
            "N'hésitez pas à vous prendre un petit café.",
            "Je suis ADA, votre assistante numérique içi au MIC.",
            "De l'eau est à votre disposition derrière le mur."

          /*  "Passez une bonne journée !",
            "Que la force soit avec vous !",
            "Vous êtes rayonnants aujourd'hui.",
            "J'espère que tout se passe bien pour vous.",
            "Et la famille ?",
            "Que votre journée soit bonne.",
            "Tranquille la famille ?",
            "Alors, ça farte les gars ?",
            "On se fait un Dürüm ensemble un de ces quattres?",
            "Winter is coming...",
            "Bienvenue au MIC.",
            "Paix et félicité.",
            "Force et robustesse.",
            "Avé à vous.",
            "Longue vie et prospérité.",
            "N'hésitez pas à vous prendre un petit café.",
            "De l'eau est à votre disposition derrière le mur.",
            "Je suis ADA, l'assistante de Martine.",
            "Je suis ADA, ravie de faire votre connaissance.",
            "Je suis ADA, ton assistante numérique içi au MIC.",
            "ça va mes amours ?",
            "Auto destruction dans 5. 4. 3. 2. 1. Je rigole. Ha. ha. ha. ha.",
            "On va au Brasse-temps ?",
            "Attention derrière vous !",
            "Bonjour, je suis ADA, je suis parafaite !",
            "Bonjour tout le monde ! Alors, toujours à traîner ensemble ?"
            */
        };
        #endregion

        public static string GetHelloSentence(PersonDto[] persons)
        {
            var builder = new StringBuilder();
            var randomizer = new Random(); 
            
            // Generates welcom sentence            
            builder.Append(persons.Length >= LimitPersonsToWelcom ? 
                // If there are more persons than limit of persons
                "Bonjour tout le monde !" : 
                // If there are one or two persons
                string.Join(". ", persons.Select(p => $"Bonjour {p.FirstName}")));

            // Generates random sentence
            builder.Append(persons.Length == 1 ? 
                // If there are one person
                persons[0].Gender == GenderValues.Male ? 
                    // If it's a male
                    $". {RandomMaleSentences[randomizer.Next(0, RandomMaleSentences.Count)]}" : 
                    // If it's a female 
                    $". {RandomFemaleSentences[randomizer.Next(0, RandomFemaleSentences.Count)]}" : 
                // If there are more than one 
                $". {RandomSentences[randomizer.Next(0, RandomFemaleSentences.Count)]}");

            return builder.ToString(); 
        }

        public static string GetAskNameSentence()
        {
            return "Quel est votre prénom ?";
        }

        public static string GetNotUnderstoodSentence()
        {
            return "Je n'ai pas très bien compris ce que vous avez dit ! Pouvez-vous répéter ?";
        }

        public static string GetYesOrNoSentence(string name)
        {
            return $"Votre nom est-il bien {name}?";
        }
        public static string GetReasonSentence()
        {
            return "Quelle est la raison de votre visite ?";
        }

        public static string GetAskWhatToDoSentence()
        {
            return "Que puis-je faire pour vous ?";
        }

        public static string GetEventsAvailableSentence()
        {
            return "Très bien ! Voici la liste des événements !";
        }
    }
}

using AdaSDK;
using AdaSDK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace AdaBot.Services
{
    public class StringConstructor
    {
        public StringBuilder DescriptionPersonImage(FullPersonDto person)
        {
            StringBuilder reply = new StringBuilder();

            if (person.FirstName != null)
            {
                reply.Append(" Je connais cette personne c'est " + person.FirstName + ". ");
            }
            else
            {
                reply.Append(" Je ne connais malheureusement pas cette personne. ");
            }

            reply.Append(DescriptionGender(person));

            if (Convert.ToInt32(person.Glasses) != 0)
            {
                reply.Append(DescriptionGlasses(person));
            }

            if (Convert.ToString(person.Gender) == "male")
            {
                reply.Append(DescriptionEmotion(person));
            }
            else if(Convert.ToString(person.Gender) == "female")
            {
                reply.Append(DescriptionEmotionFemale(person));
            }


            reply.Append(".");
            return reply;
        }

        public StringBuilder DescriptionGender(FullPersonDto person)
        {
            StringBuilder reply = new StringBuilder();

            if (Convert.ToString(person.Gender) == "male")
            {
                reply.Append("C'est un homme");

                if(person.Beard >= 0.5 && person.Mustache >= 0.5)
                {
                    reply.Append(" barbu et moustachu");
                }
                else if(person.Beard >= 0.5)
                {
                    reply.Append(" barbu");
                }
                else if(person.Mustache >= 0.5)
                {
                    reply.Append(" moustachu");
                }

                reply.Append(" d'environ " + (int)person.Age + " ans ");
            }
            else
            {
                reply.Append("C'est une femme d'environ " + (int)person.Age + " ans ");
            }

            return reply;
        }

        public StringBuilder DescriptionGlasses(FullPersonDto person)
        {
            StringBuilder reply = new StringBuilder();

            if (Convert.ToInt32(person.Glasses) == 1)
            {
                reply.Append("qui porte des lunettes de soleil ");
            }
            else if (Convert.ToInt32(person.Glasses) == 2)
            {
                reply.Append("qui porte des lunettes ");
            }
            else
            {
                reply.Append("qui porte des lunettes de piscine ");
            }

            return reply;
        }
        public StringBuilder DescriptionEmotion(FullPersonDto person)
        {
            StringBuilder reply = new StringBuilder();

            if(person.Happiness >= 0.75)
            {
                reply.Append("et qui est heureux");
            }
            else if (person.Neutral >= 0.75)
            {
                reply.Append("et qui est neutre");
            }
            else if(person.Sadness >= 0.75)
            {
                reply.Append("et qui est triste");
            }
            else if(person.Surprise >= 0.75)
            {
                reply.Append("et qui est surpris");
            }
            else if(person.Anger >= 0.75)
            {
                reply.Append("et qui est en colère");
            }
            else if(person.Contempt >= 0.75)
            {
                reply.Append("et qui est méprisant");
            }
            else if(person.Disgust >= 0.75)
            {
                reply.Append("et qui est dégouté");
            }
            else if(person.Fear >=0.75)
            {
                reply.Append("et qui a peur");
            }

            return reply;
        }

        public StringBuilder DescriptionEmotionFemale(FullPersonDto person)
        {
            StringBuilder reply = new StringBuilder();

            if (person.Happiness >= 0.75)
            {
                reply.Append("et qui est heureuse");
            }
            else if (person.Neutral >= 0.75)
            {
                reply.Append("et qui est neutre");
            }
            else if (person.Sadness >= 0.75)
            {
                reply.Append("et qui est triste");
            }
            else if (person.Surprise >= 0.75)
            {
                reply.Append("et qui est surprise");
            }
            else if (person.Anger >= 0.75)
            {
                reply.Append("et qui est en colère");
            }
            else if (person.Contempt >= 0.75)
            {
                reply.Append("et qui est méprisante");
            }
            else if (person.Disgust >= 0.75)
            {
                reply.Append("et qui est dégoutée");
            }
            else if (person.Fear >= 0.75)
            {
                reply.Append("et qui a peur");
            }

            return reply;
        }
    }
}
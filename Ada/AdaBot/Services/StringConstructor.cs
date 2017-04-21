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
                reply.Append("Je connais " + person.FirstName +". ");
                reply.Append(DescriptionGender(person));
            }
            else
            {
                reply.Append("Je ne connais malheureusement pas cette personne. ");
                reply.Append(DescriptionGender(person));
            }

            return reply;
        }

        public StringBuilder DescriptionGender(FullPersonDto person)
        {
            StringBuilder reply = new StringBuilder();

            if(Convert.ToString(person.Gender) == "Male")
            {
                reply.Append("C'est un homme d'environ " + person.Age + " ans. ");
            }
            else
            {
                reply.Append("C'est une femme d'environ " + person.Age + " ans. ");
            }


            return reply;
        }
    }
}
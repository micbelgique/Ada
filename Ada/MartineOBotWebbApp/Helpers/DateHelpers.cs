using System;

namespace MartineOBotWebApp.Helpers
{
    public class DateHelpers
    {
        public static int ConvertDateOfBirthToAge(DateTime dob)
        {
            return DateTime.Now.Year - dob.Year; 
        }
    }
}
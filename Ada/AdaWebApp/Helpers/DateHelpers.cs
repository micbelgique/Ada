using System;

namespace AdaWebApp.Helpers
{
    public class DateHelpers
    {
        public static int ConvertDateOfBirthToAge(DateTime dob)
        {
            return DateTime.Now.Year - dob.Year; 
        }
    }
}
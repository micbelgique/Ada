using System;
using System.Linq;
using MartineobotBridge;
using MartineOBotWebApp.Helpers;

// ReSharper disable once CheckNamespace
namespace MartineOBotWebApp.Models.Entities
{
    public static class PersonExtensions
    {
        public static bool HasReachedMaxLimitOfFaces(this Person person) 
            => person.Visits.Sum(v => v.ProfilePictures.Count(p => p.FaceApiId != default(Guid))) >= 64;

        public static void UpdateAge(this Person person, double age)
        {
            int count = person.Visits.Sum(v => v.ProfilePictures.Count);

            if (count == 0)
                person.DateOfBirth = DateTime.UtcNow.AddYears(-(int)age);
            else
            {
                int oldAge = DateHelpers.ConvertDateOfBirthToAge(person.DateOfBirth);

                var result = ((oldAge * count) + age) / (count + 1);

                person.DateOfBirth = DateTime.UtcNow.AddYears(-(int)result);
            }
        }

        public static void UpdateGender(this Person person, GenderValues gender)
        {
            //First check whether the gender value is male
            if (gender == GenderValues.Male)
                person.MaleCounter++;
            else
                person.FemaleCounter++;

            person.Gender = person.MaleCounter >= person.FemaleCounter ? GenderValues.Male : GenderValues.Female;
        }
    }
}

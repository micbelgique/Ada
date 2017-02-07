using System;
using System.Linq;
using AdaBridge;
using AdaWebApp.Helpers;

// ReSharper disable once CheckNamespace
namespace AdaWebApp.Models.Entities
{
    public static class RecognitionItemExtension
    {
        // Converts a recognition item entity to person data transfert object
        public static PersonDto ToPersonDto(this RecognitionItem recognitionItem)
        {
            var lastVisit = recognitionItem.Person?.Visits.LastOrDefault();

            var age = recognitionItem.Person != null 
                ? DateHelpers.ConvertDateOfBirthToAge(recognitionItem.Person.DateOfBirth)
                : recognitionItem.Face.FaceAttributes.Age;

            var gender = recognitionItem.Person?.Gender ?? 
                GenderValuesHelper.Parse(recognitionItem.Face.FaceAttributes.Gender);

            return new PersonDto
            {
                RecognitionId = recognitionItem.Id,
                FirstName = recognitionItem.Person?.FirstName,
                IsRecognized = recognitionItem.Person != null,
                PersonId = recognitionItem.Person?.PersonApiId ?? default(Guid),
                NbPasses = lastVisit?.NbPasses ?? 0,
                ReasonOfVisit = lastVisit?.Reason, 
                Age = (int)age, 
                Gender = gender
            };
        }
    }
}
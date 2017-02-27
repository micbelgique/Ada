using System;
using System.Linq;
using AdaSDK;
using AdaWebApp.Helpers;
using AdaSDK.Models;
using AdaWebApp.Models.Entities;

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

        public static VisitDto ToDto(this Visit visit)
        {
            return new VisitDto()
            {
                ID = visit.Id,
                Date = visit.Date,
                NbPasses = visit.NbPasses,
                ProfilePicture = visit.ProfilePictures.Last().ToDto(),
                PersonVisit = visit.Person.ToDto()
            };
        }

        public static PersonVisitDto ToDto(this Person person)
        {
            return new PersonVisitDto()
            {
                PersonId = person.Id,
                FirstName = person.FirstName,
                DateVisit = person.DateOfBirth,
                Gender = person.Gender
            };
        }

        public static ProfilePictureDto ToDto(this ProfilePicture picture)
        {
            return new ProfilePictureDto()
            {
                Uri = picture.Uri
            };
        }

    }
}
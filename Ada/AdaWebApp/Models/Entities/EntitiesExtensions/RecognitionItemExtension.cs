using System;
using System.Linq;
using AdaSDK;
using AdaWebApp.Helpers;
using AdaSDK.Models;
using System.Collections.Generic;

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

        public static MessageDto ToDto(this Message message)
        {
            if (message == null)
            {
                return null;
            }
            else
            {
                return new MessageDto()
                {
                    ID = message.Id,
                    From = message.From,
                    Contenu = message.Contenu,
                    IsRead = message.IsRead,
                    Send = message.Send,
                    Read = message.Read,
                    To = message.ToId
                };
            }
        }

        public static VisitDto ToDto(this Visit visit)
        {
            if (visit == null)
            {
                return null;
            }
            else
            {
                List<ProfilePicture> tmp = visit.ProfilePictures.ToList();
                return new VisitDto()
                {
                    ID = visit.Id,
                    Date = visit.Date,
                    NbPasses = visit.NbPasses,
                    ProfilePicture = tmp.Last().ToDto(),
                    PersonVisit = visit.Person.ToDto()
                };
            }
        }
        public static VisitDto ToDtoListPicture(this Visit visit)
        {
            List<ProfilePicture> tmp = visit.ProfilePictures.ToList();
            return new VisitDto()
            {
                ID = visit.Id,
                Date = visit.Date,
                NbPasses = visit.NbPasses,
                ProfilePicture = tmp.ToDto(),
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
                Gender = person.Gender,
                Age = person.DateOfBirth.Year
            };
        }

        public static List<ProfilePictureDto> ToDto(this ProfilePicture picture)
        {
            List<ProfilePictureDto> listReturn = new List<ProfilePictureDto>();

            listReturn.Add(new ProfilePictureDto()
            {
                Uri = picture.Uri,
                Glasses = picture.Glasses,
                Beard = picture.Beard,
                Mustache = picture.Moustache,
                EmotionScore = null
            });
            return listReturn;
        }

        public static List<ProfilePictureDto> ToDto(this List<ProfilePicture> picture)
        {
            List<ProfilePictureDto> listReturn = new List<ProfilePictureDto>();
            for (int i = 0 ; i<picture.Count() ; i++)
            {
                if (picture[i].EmotionScores == null)
                {
                    listReturn.Add(new ProfilePictureDto()
                    {
                        Uri = picture[i].Uri,
                        Glasses = picture[i].Glasses,
                        Beard = picture[i].Beard,
                        Mustache = picture[i].Moustache,
                        EmotionScore = null
                    });
                }
                else
                {
                    listReturn.Add(new ProfilePictureDto()
                    {
                        Uri = picture[i].Uri,
                        Glasses = picture[i].Glasses,
                        Beard = picture[i].Beard,
                        Mustache = picture[i].Moustache,
                        EmotionScore = picture[i].EmotionScores.ToDto()
                    });
                }
            }
            return listReturn;
        }

        public static EmotionDto ToDto(this EmotionScores emotion)
        {
            return new EmotionDto()
            {
                Anger = emotion.Anger,
                Contempt = emotion.Contempt,
                Disgust = emotion.Disgust,
                Fear = emotion.Fear,
                Happiness = emotion.Happiness,
                Neutral = emotion.Neutral,
                Sadness = emotion.Sadness,
                Surprise = emotion.Surprise
            };
        }

    }
}
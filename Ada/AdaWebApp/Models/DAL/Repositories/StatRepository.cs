using AdaBridge;
using AdaWebApp.Helpers;
using AdaWebApp.Models.Entities;
using System;
using System.Linq;

namespace AdaWebApp.Models.DAL.Repositories
{
    public class StatRepository
    {
        public ApplicationDbContext Context { get; }
        public StatRepository(ApplicationDbContext context)
        {
            Context = context;
        }

        public int GetNumberOfPersons()
        {
            return Context.Visitors.Count();
        }

        public double GetAverageAge()
        {
            return Math.Round(Context.Visitors.Select(p => p.DateOfBirth)
                .ToList().Average(d => DateHelpers.ConvertDateOfBirthToAge(d)), 2);
        }

        public EmotionScores GetAverageEmotions()
        {
            return new EmotionScores
            {
                Anger = (float)Math.Round(Context.EmotionScores.Average(p => p.Anger) * 100, 2),
                Contempt = (float)Math.Round(Context.EmotionScores.Average(p => p.Contempt) * 100, 2),
                Disgust = (float)Math.Round(Context.EmotionScores.Average(p => p.Disgust) * 100, 2),
                Fear = (float)Math.Round(Context.EmotionScores.Average(p => p.Fear) * 100, 2),
                Happiness = (float)Math.Round(Context.EmotionScores.Average(p => p.Happiness) * 100, 2),
                Neutral = (float)Math.Round(Context.EmotionScores.Average(p => p.Neutral) * 100, 2),
                Sadness = (float)Math.Round(Context.EmotionScores.Average(p => p.Sadness) * 100, 2),
                Surprise = (float)Math.Round(Context.EmotionScores.Average(p => p.Surprise) * 100, 2),
            };
        }

        public double GetPercentOfMale()
        {
            return Math.Round(Context.Visitors.Count(p => p.Gender == GenderValues.Male) / (double)GetNumberOfPersons() * 100, 2);
        }

        public double GetPercentOfFemale()
        {
            return Math.Round(Context.Visitors.Count(p => p.Gender == GenderValues.Female) / (double)GetNumberOfPersons() * 100, 2);
        }
    }
}
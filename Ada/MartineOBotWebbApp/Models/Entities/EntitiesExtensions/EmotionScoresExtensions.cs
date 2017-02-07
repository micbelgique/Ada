using Microsoft.ProjectOxford.Emotion.Contract;

namespace MartineOBotWebApp.Models.Entities.EntitiesExtensions
{
    public static class EmotionScoresExtensions
    {
        public static EmotionScores ToEmotionScores(this Emotion oxfordEmotion)
        {
            return new EmotionScores
            {
                Anger = oxfordEmotion?.Scores.Anger ?? 0,
                Contempt = oxfordEmotion?.Scores.Contempt ?? 0,
                Disgust = oxfordEmotion?.Scores.Disgust ?? 0,
                Fear = oxfordEmotion?.Scores.Fear ?? 0,
                Happiness = oxfordEmotion?.Scores.Happiness ?? 0,
                Neutral = oxfordEmotion?.Scores.Neutral ?? 0,
                Sadness = oxfordEmotion?.Scores.Sadness ?? 0,
                Surprise = oxfordEmotion?.Scores.Surprise ?? 0
            };
        }
    }
}

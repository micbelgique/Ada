using Windows.Media.SpeechRecognition;

namespace MartineobotIOTMvvm.Messages
{
    public class SpeechStateChangedMessage
    {
        public SpeechRecognizerState SpeechRecognizerState { get; set; }

        public SpeechStateChangedMessage(SpeechRecognizerState state)
        {
            SpeechRecognizerState = state;
        }
    }
}

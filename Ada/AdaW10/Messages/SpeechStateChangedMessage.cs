using Windows.Media.SpeechRecognition;

namespace AdaW10.Messages
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

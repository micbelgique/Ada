using Windows.Media.SpeechRecognition;

namespace MartineobotIOTMvvm.Messages
{
    internal class SpeechResultGeneratedMessage
    {
        public SpeechRecognitionResult Result {get; set;}

        public SpeechResultGeneratedMessage(SpeechRecognitionResult result)
        {
            this.Result = result;
        }
    }
}
using System;
using Windows.Media.SpeechRecognition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace AdaW10.Converters
{
    public class SpeechStateToVisibilityConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            SpeechRecognizerState state = (SpeechRecognizerState)value;
            switch (state)
            {
                case SpeechRecognizerState.Capturing:
                case SpeechRecognizerState.SpeechDetected:
                case SpeechRecognizerState.SoundStarted:
                case SpeechRecognizerState.SoundEnded:
                    return Visibility.Visible;
                default:
                    return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Media.SpeechRecognition;
using GalaSoft.MvvmLight.Messaging;
using MartineobotIOTMvvm.Messages;

namespace MartineobotIOTMvvm.Controls
{
    public sealed partial class MicrophoneState : INotifyPropertyChanged
    {
        private SpeechRecognizerState _state;

        public SpeechRecognizerState State
        {
            get { return _state; }
            set
            {
                _state = value;
                OnPropertyChanged();
            }
        }

        public MicrophoneState()
        {
            InitializeComponent();
            DataContext = this;

            Messenger.Default.Register<SpeechStateChangedMessage>(this, OnStateChanged);
        }

        private async void OnStateChanged(SpeechStateChangedMessage message)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
            {
                State = message.SpeechRecognizerState;
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

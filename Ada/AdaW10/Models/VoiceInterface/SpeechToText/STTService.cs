using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Media.SpeechRecognition;
using GalaSoft.MvvmLight.Messaging;
using AdaW10.Messages;
using AdaW10.Helper;
using System.Threading;

namespace AdaW10.Models.VoiceInterface.SpeechToText
{
    public class SttService : IDisposable
    {
        public SpeechRecognizer SpeechRecognizer { get; set; }
        private bool _disposed;

        public SttService()
        {
            SpeechRecognizer = new SpeechRecognizer();
        }
        
        /// <summary>
        /// Asynchronously add constraint in speech recognizer
        /// </summary>
        public async Task<SpeechRecognitionCompilationResult> AddConstraintAsync(ISpeechRecognitionConstraint constraint, bool compile = true)
        {
            if (SpeechRecognizer.State != SpeechRecognizerState.Idle)
            {
                Debug.WriteLine($"STTService Failed to add constraint because of speech recognizer state : {SpeechRecognizer.State}");
                return null;
            }

            SpeechRecognizer.Constraints.Add(constraint);

            if (!compile) return null; 

            return await SpeechRecognizer.CompileConstraintsAsync();
        }

        /// <summary>
        /// Asynchronously clean constraint in speech recognizer
        /// </summary>
        public async Task CleanConstraintsAsync()
        {
            SpeechRecognizer.Constraints.Clear();

            if (SpeechRecognizer.State != SpeechRecognizerState.Idle){
                await SpeechRecognizer.CompileConstraintsAsync();
            }
        }

        /// <summary>
        /// Event fired when the state of speechrecognize object change for a simple speech recognition
        /// </summary>
        private void InstantRecognition_StateChanged(SpeechRecognizer sender, SpeechRecognizerStateChangedEventArgs args)
        {
            Messenger.Default.Send(new SpeechStateChangedMessage(args.State));   
        }

        /// <summary>
        /// Asynchronously start a simple speech recognition
        /// </summary>
        public async Task<SpeechRecognitionResult> RecognizeAsync()
        {
            if (SpeechRecognizer.State != SpeechRecognizerState.Idle) return null;

            SpeechRecognizer.StateChanged += InstantRecognition_StateChanged;

            var result = await SpeechRecognizer.RecognizeAsync();

            SpeechRecognizer.StateChanged -= InstantRecognition_StateChanged;

            return result;
        }

        /// <summary>
        /// Asynchronously start continuous speech recognition 
        /// </summary>
        public async Task StartContinuousRecognitionAsync()
        {
            if (SpeechRecognizer.State == SpeechRecognizerState.Idle)
            {
                await SpeechRecognizer.ContinuousRecognitionSession.StartAsync();
                SpeechRecognizer.ContinuousRecognitionSession.ResultGenerated +=
                    ContinuousRecognitionSession_ResultGenerated;
            }
        }

        /// <summary>
        /// Asynchronously cancal continuous speech recognition
        /// </summary>
        public async Task<bool> CancelContinuousRecognitionAsync()
        {
            try{
                if (SpeechRecognizer != null)
                {
                    await SpeechRecognizer.ContinuousRecognitionSession.CancelAsync();
                }
                return true; 
            }
            catch (Exception){
                Debug.WriteLine("STTService failed to cancel continuous recognition");
                return false; 
            }
        }

        /// <summary>
        /// Event for continuous speech recognition which is fired when a result is generated
        /// </summary>
        private void ContinuousRecognitionSession_ResultGenerated(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {
            Debug.WriteLine($"STTService speech detected: {args.Result.Text}");

            if (args.Result.Confidence != SpeechRecognitionConfidence.Rejected)
            {
                Messenger.Default.Send(new SpeechResultGeneratedMessage(args.Result));
            }    
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (_disposed) return; 

            if (disposing)
            {
                SpeechRecognizer?.Dispose();
                SpeechRecognizer = null; 
            }

            _disposed = true; 
        }
    }
}

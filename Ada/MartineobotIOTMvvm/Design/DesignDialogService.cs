using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MartineobotIOTMvvm.Design
{
    public class DesignDialogService : Models.Interfaces.IDialogService
    {
        public Task ShowError(Exception error, string title, string buttonText, Action afterHideCallback)
        {
            return Task.Run(() => { 
                Debug.WriteLine($"ERROR: {error.Message}");
            });
        }

        public Task ShowError(string message, string title, string buttonText, Action afterHideCallback)
        {
            return Task.Run(() => {
                Debug.WriteLine($"ERROR: {message}");
            });
        }

        public Task ShowMessage(string message, string title)
        {
            return Task.Run(() => {
                Debug.WriteLine($"MESSAGE: {message}");
            });
        }

        public Task ShowMessage(string message, string title, string buttonText, Action afterHideCallback)
        {
            return Task.Run(() => {
                Debug.WriteLine($"MESSAGE: {message}");
            });
        }

        public Task<bool> ShowMessage(string message, string title, string buttonConfirmText, string buttonCancelText, Action<bool> afterHideCallback)
        {
            return Task.Run(() => {
                Debug.WriteLine($"MESSAGE: {message}");
                return false;
            });
        }

        public Task ShowMessageBox(string message, string title)
        {
            return Task.Run(() => {
                Debug.WriteLine($"MESSAGE: {message}");
            });
        }
    }
}

using System.Diagnostics;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;

namespace AdaW10.ViewModels
{
    public enum ModeValues
    {
        Passive,
        Actif
    }

    public class SwitchViewModel : ViewModelBase
    {
        public SwitchViewModel()
        {
            Debug.WriteLine("Constructeur Switch");
            SelectModeCommand = new RelayCommand<ModeValues>(OnSelectMode);
        }

        public RelayCommand<ModeValues> SelectModeCommand { get; set; }

        private void OnSelectMode(ModeValues mode)
        {
            NavigationService.NavigateTo(ViewModelLocator.MainPage, mode);
        }

    }
}

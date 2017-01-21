using System.Linq;
using System.Windows;
using Converter.Helpers;

namespace Converter.Mvvm.ViewModel
{
    internal interface IViewModelBase
    {
        string WindowTitle { get; }
        RelayCommand CloseWindowCommand { get; }
    }
    internal class ViewModelBase : Notifier, IViewModelBase
    {
        public string WindowTitle { get; protected set; }
        public RelayCommand CloseWindowCommand { get; protected set; }

        public Window MainWindowOfApplication
        {
            get { return Application.Current.MainWindow; }
        }

        public ViewModelBase()
        {
            CloseWindowCommand = new RelayCommand(CloseWindow);
        }

        public void CloseWindow(object parametr)
        {
            var currentActiveWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            if (currentActiveWindow != null) currentActiveWindow.Close();
        }
    }
}

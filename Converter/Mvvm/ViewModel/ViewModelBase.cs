using System;
using System.Linq;
using System.Windows;
using Converter.Helpers;

namespace Converter.Mvvm.ViewModel
{
    class ViewModelBase : Notifier
    {
        public string WindowTitle { get; set; }
        public RelayCommand CloseWindowCommand { get; set; }

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

        public void ExceptionHandler(Exception exception)
        {
            var myException = new ExceptionWindow(exception);
            myException.ShowExceptionWindow();
        }
    }
}

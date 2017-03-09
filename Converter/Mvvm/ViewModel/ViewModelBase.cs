using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using Converter.Helpers;

namespace Converter.Mvvm.ViewModel
{
    internal interface IViewModelBase
    {
        string WindowTitle { get; }
        RelayCommand CloseWindowCommand { get; }
        RelayCommand ViewLogCommand { get; }
    }
    internal class ViewModelBase : Notifier, IViewModelBase
    {
        private readonly string _dataDirectory =
            (string)AppDomain.CurrentDomain.GetData("DataDirectory");

        public string WindowTitle { get; protected set; }
        public RelayCommand ViewLogCommand { get; private set; }
        public RelayCommand CloseWindowCommand { get; protected set; }

        public Window MainWindowOfApplication
        {
            get { return Application.Current.MainWindow; }
        }

        public ViewModelBase()
        {
            ViewLogCommand = new RelayCommand(ViewLog, CanViewLog);
            CloseWindowCommand = new RelayCommand(CloseWindow);
        }

        private void ViewLog(object obj)
        {
            Process.Start(_dataDirectory + @"\log.txt");
        }

        private bool CanViewLog(object obj)
        {
            return File.Exists(_dataDirectory + @"\log.txt");
        }

        public void CloseWindow(object parametr)
        {
            var currentActiveWindow = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            if (currentActiveWindow != null) currentActiveWindow.Close();
        }
    }
}

using System;
using System.IO;
using Converter.Mvvm.View;
using Converter.Mvvm.ViewModel;

namespace Converter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public App()
        {
            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
        }
        
        private static void Current_DispatcherUnhandledException(
            object sender, 
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            ExceptionWindowShow(e.Exception.Message);
            Logger(e.Exception);
            e.Handled = true;
        }

        private static void ExceptionWindowShow(string message)
        {
            var exceptionViewModel = new ExceptionViewModel(message, "Exception window");
            var mainWindowOfApplication = exceptionViewModel.MainWindowOfApplication;
            var exceptionView = new ExceptionView
            {
                Owner = mainWindowOfApplication,
                ShowInTaskbar = false,
                DataContext = exceptionViewModel,
                Icon = mainWindowOfApplication.Icon
            };
            exceptionView.ShowDialog();
        }

        private static void Logger(Exception exception)
        {
            using (var file = new StreamWriter(Environment.CurrentDirectory + "\\log.txt", true))
            {
                if (exception.InnerException == null) return;
                var message = string.Format("{0}\t{1}", DateTime.Now, exception.Message);
                file.WriteLine(message);
                file.WriteLine("Exception StackTrace:");
                file.WriteLine(exception.InnerException.StackTrace);
            }
        }
    }
}

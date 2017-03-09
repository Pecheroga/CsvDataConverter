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
            var myCommonAppFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            AppDomain.CurrentDomain.SetData("DataDirectory", myCommonAppFolder + @"\CSV Data Converter");
        }
        
        private static void Current_DispatcherUnhandledException(
            object sender, 
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Logger(e.Exception);
            ExceptionWindowShow(e.Exception.Message);
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
            WriteLogToFile(exception.InnerException ?? exception);
        }

        private static void WriteLogToFile(Exception exception)
        {
            var dataDirectory = AppDomain.CurrentDomain.GetData("DataDirectory");
            using (var file = new StreamWriter(dataDirectory + @"\log.txt", true))
            {
                var message = string.Format("{0}\t{1}", DateTime.Now, exception.Message);
                file.WriteLine(message);
                file.WriteLine(exception.Message);
                file.WriteLine("Exception StackTrace:");
                file.WriteLine(exception.StackTrace);
            }
        }
    }
}

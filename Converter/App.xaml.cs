using System.Windows;
using Converter.Mvvm.View;
using Converter.Mvvm.ViewModel;

namespace Converter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
        }
        
        private static void Current_DispatcherUnhandledException(
            object sender, 
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var exceptionViewModel = new ExceptionViewModel(e.Exception.Message, "Exception window");
            var mainWindowOfApplication = exceptionViewModel.MainWindowOfApplication;
            var exceptionView = new ExceptionView
            {
                Owner = mainWindowOfApplication,
                ShowInTaskbar = false,
                DataContext = exceptionViewModel,
                Icon = mainWindowOfApplication.Icon
            };
            exceptionView.ShowDialog();
            e.Handled = true;
        }
    }
}

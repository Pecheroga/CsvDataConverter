using System;
using System.Windows;
using Converter.Mvvm.Model;
using Converter.Mvvm.View;
using Converter.Mvvm.ViewModel;

namespace Converter.Helpers
{
    internal sealed class ExceptionWindow : Exception
    {
        public ExceptionWindow(Exception exception) : base(exception.Message, exception) { }

        public void ShowExceptionWindow()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (InnerException == null) return;
                var title = string.Format("Method: \"{0}\"", InnerException.TargetSite.Name);
                var exceptionViewModel = new ExceptionViewModel(Message, title);
                var mainWindowOfApplication = exceptionViewModel.MainWindowOfApplication;
                var exceptionView = new ExceptionView
                {
                    Owner = mainWindowOfApplication,
                    ShowInTaskbar = false,
                    DataContext = exceptionViewModel,
                    Icon = mainWindowOfApplication.Icon
                };
                exceptionView.ShowDialog();
            });
        }
    }

    internal sealed class ExcelException : Exception
    {
        public ExcelException(ExcelAppBase excelAppBase, string exceptionMessage)
            : base(exceptionMessage)
        {
            excelAppBase.QuitExcelApp();
        }
    }
}

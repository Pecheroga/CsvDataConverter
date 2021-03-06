﻿namespace Converter.Mvvm.ViewModel
{
    internal sealed class ExceptionViewModel : ViewModelBase
    {
        public string ExceptionText { get; private set; }

        public ExceptionViewModel(string message, string title)
        {
            WindowTitle = title;
            ExceptionText = message;
        }
    }
}

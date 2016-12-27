namespace Converter.Mvvm.ViewModel
{
    class ExceptionViewModel : ViewModelBase
    {
        public string ExceptionText { get; set; }

        public ExceptionViewModel() { }

        public ExceptionViewModel(string message, string title)
        {
            WindowTitle = title;
            ExceptionText = message;
        }
    }
}

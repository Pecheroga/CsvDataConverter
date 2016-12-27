using System;
using System.Windows.Input;

namespace Converter.Helpers
{
    class RelayCommand : ICommand
    {
        private readonly Action<object> _executeAction;
        private readonly Predicate<object> _canExecutePredicate;

        public RelayCommand(Action<object> executeAction) : this(executeAction, null) { }

        public RelayCommand(Action<object> executeAction, Predicate<object> canExecutePredicate)
        {
            if (executeAction == null) return;
            _executeAction = executeAction;
            _canExecutePredicate = canExecutePredicate;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecutePredicate == null || _canExecutePredicate(parameter);
        }

        public void Execute(object parameter)
        {
            if (_executeAction != null)
            {
                _executeAction.Invoke(parameter);
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}

using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Converter.Helpers;
using DataSource.Base;
using DataSource.Db;

namespace Converter.Mvvm.ViewModel.Settings
{
    internal interface IDialogViewModelBase: IViewModelBase
    {
        Program SelectedProgram { get; }
        RelayCommand OkCommand { get; }
        BindingGroup EditBindingGroup { get; }
    }

    internal class DialogViewModelBase : ViewModelBase, IDialogViewModelBase
    {
        protected IToDb ToDb;
        protected readonly ISettingsViewModel SettingsViewModel;

        public virtual Program SelectedProgram { get { return null; } }

        private BindingGroup _editBindingGroup;
        public BindingGroup EditBindingGroup
        {
            get { return _editBindingGroup; }
            private set
            {
                _editBindingGroup = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand OkCommand { get; private set; }

        public DialogViewModelBase(ISettingsViewModel settingsViewModel)
        {
            ToDb = new ToDb();
            SettingsViewModel = settingsViewModel;
            EditBindingGroup = new BindingGroup
            {
                Name = "EditBindingGroup_1"
            };

            OkCommand = new RelayCommand(Ok, CanOk);
        }

        protected virtual void Ok(object parameter)
        {
            CloseWindow(null);
        }

        private bool CanOk(object obj)
        {
            return !EditBindingGroup.
                BindingExpressions.
                Select(bindingExpression => bindingExpression.Target as TextBox).
                Any(textBox => textBox != null && string.IsNullOrEmpty(textBox.Text));
        }
    }
}

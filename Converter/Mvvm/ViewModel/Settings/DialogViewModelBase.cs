using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Converter.Helpers;
using DataSource.Db;

namespace Converter.Mvvm.ViewModel.Settings
{
    public interface IDialogViewModelBase { }

    class DialogViewModelBase : ViewModelBase, IDialogViewModelBase
    {
        protected readonly IToDb ToDb;
        protected readonly ISettingsViewModel SettingsViewModel;

        private BindingGroup _editBindingGroup;
        public BindingGroup EditBindingGroup
        {
            get { return _editBindingGroup; }
            set
            {
                _editBindingGroup = value;
                OnPropertyChanged();
            }
        }

        private bool EditStarted { get; set; }
        public Visibility AddBtmVisibility { get; set; }
        public Visibility EditBtmVisibility { get; set; }

        public RelayCommand OkCommand { get; set; }
        public RelayCommand ApplyCommand { get; set; }
        public RelayCommand UndoCommand { get; set; }

        public DialogViewModelBase(ISettingsViewModel settingsViewModel)
        {
            ToDb = new ToDb();
            SettingsViewModel = settingsViewModel;
            EditBindingGroup = new BindingGroup
            {
                Name = "EditBindingGroup_1"
            };

            OkCommand = new RelayCommand(Ok, CanOk);
            ApplyCommand = new RelayCommand(Apply, CanApply);
            UndoCommand = new RelayCommand(Undo, CanUndo);

            SelectAllBehavior.TxtbEditStarted += _selectAllBehavior_TxtbEditStarted;
            EditStarted = false;
        }

        protected virtual void Ok(object parameter)
        {
            CloseWindow(null);
        }

        private bool CanOk(object obj)
        {
            return !EditBindingGroup.BindingExpressions.Select(bindingExpression =>
            bindingExpression.Target as TextBox).Any(textBox =>
                textBox != null && string.IsNullOrEmpty(textBox.Text));
        }

        protected virtual void Apply(object obj)
        {
            EditStarted = false;
        }

        private bool CanApply(object obj)
        {
            return !EditBindingGroup.BindingExpressions.Select(bindingExpression =>
                bindingExpression.Target as TextBox).Any(textBox =>
                    textBox != null && string.IsNullOrEmpty(textBox.Text))
                    && EditStarted;
        }

        private void Undo(object obj)
        {
            EditBindingGroup.CancelEdit();
            EditStarted = false;
        }

        private bool CanUndo(object obj)
        {
            return EditStarted;
        }

        private void _selectAllBehavior_TxtbEditStarted(object sender, TextBox e)
        {
            EditStarted = true;
        }
    }
}

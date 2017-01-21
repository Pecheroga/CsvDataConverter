using System.Linq;
using System.Windows.Controls;
using Converter.Helpers;
using DataSource.Base;

namespace Converter.Mvvm.ViewModel.Settings
{
    internal interface IEditDialogViewModel : IDialogViewModelBase
    {
        RelayCommand ApplyCommand { get; }
        RelayCommand UndoCommand { get; }
    }

    internal sealed class EditDialogViewModel : DialogViewModelBase, IEditDialogViewModel
    {
        private Program _selectedProgram;
        public override Program SelectedProgram
        {
            get { return _selectedProgram = SettingsViewModel.SelectedProgram; }
        }

        private bool EditStarted { get; set; }
        public RelayCommand ApplyCommand { get; private set; }
        public RelayCommand UndoCommand { get; private set; }

        public EditDialogViewModel(ISettingsViewModel settingsViewModel)
            : base(settingsViewModel)
        {
            WindowTitle = "Edit Program";
            ApplyCommand = new RelayCommand(Apply, CanApply);
            UndoCommand = new RelayCommand(Undo, CanUndo);

            SelectAllBehavior.TxtbEditStarted += _selectAllBehavior_TxtbEditStarted;
            EditStarted = false;
        }

        protected override void Ok(object obj)
        {
            Update();
            base.Ok(null);
        }

        private void Apply(object obj)
        {
            Update();
            EditStarted = false;
        }

        private void Update()
        {
            EditBindingGroup.CommitEdit();
            ToDb.Update(_selectedProgram);
        }

        private bool CanApply(object obj)
        {
            return !EditBindingGroup.
                BindingExpressions.
                Select(bindingExpression => bindingExpression.Target as TextBox).
                Any(textBox => textBox != null && string.IsNullOrEmpty(textBox.Text))
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

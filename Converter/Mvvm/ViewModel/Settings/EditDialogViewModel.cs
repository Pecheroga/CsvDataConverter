using System;
using System.Windows;
using DataSource.Base;

namespace Converter.Mvvm.ViewModel.Settings
{
    internal sealed class EditDialogViewModel : DialogViewModelBase
    {
        private Program _selectedProgram;
        public Program SelectedProgram
        {
            get
            {
                return _selectedProgram = SettingsViewModel.SelectedProgram;
            }
            set
            {
                _selectedProgram = value;
                OnPropertyChanged();
            }
        }

        public EditDialogViewModel(ISettingsViewModel settingsViewModel)
            : base(settingsViewModel)
        {
            WindowTitle = "Edit Program";
            AddBtmVisibility = Visibility.Collapsed;
            EditBtmVisibility = Visibility.Visible;
        }

        protected override void Ok(object obj)
        {
            Update();
            base.Ok(null);
        }

        protected override void Apply(object obj)
        {
            Update();
            base.Apply(null);
        }

        private void Update()
        {
            EditBindingGroup.CommitEdit();
            ToDb.Update(_selectedProgram);
        }
    }
}

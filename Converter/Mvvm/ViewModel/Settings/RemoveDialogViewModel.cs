using System;

namespace Converter.Mvvm.ViewModel.Settings
{
    internal sealed class RemoveDialogViewModel : DialogViewModelBase
    {
        public string ConfirmText { get; private set; }
        private readonly int _selectedIndex;
        private readonly int _removeIndex;

        public RemoveDialogViewModel(ISettingsViewModel settingsViewModel)
            : base(settingsViewModel)
        {
            WindowTitle = "Confirm";
            ConfirmText = settingsViewModel.SelectedProgram.Title;
            _selectedIndex = SettingsViewModel.SelectedIndex;
            _removeIndex = SettingsViewModel.Programs.IndexOf(SettingsViewModel.SelectedProgram);
        }

        protected override void Ok(object obj)
        {
            ToDb.Remove(SettingsViewModel.SelectedProgram);
            SettingsViewModel.Programs.RemoveAt(_removeIndex);
            SetNewSelectedIndex();
            base.Ok(obj);
        }
        
        private void SetNewSelectedIndex()
        {
            if (_selectedIndex != 0)
            {
                SettingsViewModel.SelectedIndex = _selectedIndex - 1;
            }
        }
    }
}

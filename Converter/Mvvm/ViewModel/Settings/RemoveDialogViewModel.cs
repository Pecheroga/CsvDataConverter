namespace Converter.Mvvm.ViewModel.Settings
{
    internal interface IRemoveDialogViewModel : IDialogViewModelBase
    {
        string ConfirmText { get; }
    }

    internal sealed class RemoveDialogViewModel : DialogViewModelBase, IRemoveDialogViewModel
    {
        public string ConfirmText { get { return SettingsViewModel.SelectedProgram.Title; } }
        private readonly int _selectedIndex;
        private readonly int _removeIndex;

        public RemoveDialogViewModel(ISettingsViewModel settingsViewModel)
            : base(settingsViewModel)
        {
            WindowTitle = "Confirm";
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

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DataSource.Base;

namespace Converter.Mvvm.ViewModel.Settings
{
    internal sealed class AddDialogViewModel : DialogViewModelBase
    {
        public Program SelectedProgram { get; set; }

        public AddDialogViewModel(ISettingsViewModel settingsViewModel)
            : base(settingsViewModel)
        {
            WindowTitle = "Add Program";
            AddBtmVisibility = Visibility.Visible;
            EditBtmVisibility = Visibility.Collapsed;
        }

        protected override void Ok(object parameter)
        {
            AddProgram();
            SettingsViewModel.SelectedIndex = 0;
            base.Ok(null);
        }

        private void AddProgram()
        {
            var maxIdInPrograms = 
                SettingsViewModel.Programs.Count == 0 
                ? 0 
                : SettingsViewModel.Programs.Max(p => p.Id);
            var newProgramId = maxIdInPrograms + 1;
            var program = new Program
            {
                Id = newProgramId,
                Title = ((TextBox)EditBindingGroup.BindingExpressions[0].Target).Text,
                StartLabel = ((TextBox)EditBindingGroup.BindingExpressions[1].Target).Text,
                EndLabel = ((TextBox)EditBindingGroup.BindingExpressions[2].Target).Text,
                Lang = ((TextBox)EditBindingGroup.BindingExpressions[3].Target).Text,
                Author = ((TextBox)EditBindingGroup.BindingExpressions[4].Target).Text,
                Presenter = ((TextBox)EditBindingGroup.BindingExpressions[5].Target).Text
            };
            ToDb.Add(program);
            SettingsViewModel.Programs.Add(program);
        }
    }
}

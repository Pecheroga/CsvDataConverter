using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DataSource.Base;

namespace Converter.Mvvm.ViewModel.Settings
{
    class AddDialogViewModel : DialogViewModelBase
    {
        public Program SelectedProgram { get; set; }
        private Program _program;

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
            var maxIdInPrograms = SettingsViewModel.Programs.Max(p => p.Id);
            var newProgramId = maxIdInPrograms + 1;
            _program = new Program
            {
                Id = newProgramId,
                Title = ((TextBox)EditBindingGroup.BindingExpressions[0].Target).Text,
                StartLabel = ((TextBox)EditBindingGroup.BindingExpressions[1].Target).Text,
                EndLabel = ((TextBox)EditBindingGroup.BindingExpressions[2].Target).Text,
                Lang = ((TextBox)EditBindingGroup.BindingExpressions[3].Target).Text,
                Author = ((TextBox)EditBindingGroup.BindingExpressions[4].Target).Text,
                Presenter = ((TextBox)EditBindingGroup.BindingExpressions[5].Target).Text
            };
            TryAddProgram();
        }

        private void TryAddProgram()
        {
            try
            {
                ToDb.Add(_program);
                SettingsViewModel.Programs.Add(_program);
            }
            catch (Exception exception)
            {
                ExceptionHandler(exception);
            }
        }
    }
}

using System.ComponentModel;
using System.Windows;
using Converter.Mvvm.ViewModel.Settings;

namespace Converter.Mvvm.ViewModel
{
    internal class ViewModelLocator
    {
        private static readonly DependencyObject Dummy = new DependencyObject();

        public static IMainViewModel MainViewModel
        {
            get
            {
                if (IsInDesignMode())
                {
                    return new MockMainViewModel();
                }
                return new MainViewModel();
            }
        }

        public static ISettingsViewModel SettingsViewModel
        {
            get
            {
                if (IsInDesignMode())
                {
                    return new MockSettingsViewModel();
                }
                return new SettingsViewModel();
            }
        }

        private static IDialogViewModelBase _addDialogViewModel;
        public static IDialogViewModelBase AddDialogViewModel
        {
            get { return IsInDesignMode() ? new MockDialogViewModelBase() : _addDialogViewModel; }
            set { _addDialogViewModel = value; }
        }

        private static IEditDialogViewModel _editDialogViewModel;
        public static IEditDialogViewModel EditDialogViewModel
        {
            get { return IsInDesignMode() ? new MockEditDialogViewModelBase() : _editDialogViewModel; }
            set { _editDialogViewModel = value; }
        }

        private static IRemoveDialogViewModel _removeDialogViewModel;
        public static IRemoveDialogViewModel RemoveDialogViewModel
        {
            get { return IsInDesignMode() ? new MockRemoveDialogViewModel() : _removeDialogViewModel; }
            set { _removeDialogViewModel = value; }
        }

        public ExceptionViewModel ExceptionViewModel
        {
            get { return new ExceptionViewModel("Dummy exception", "Exception"); }
        }

        private static bool IsInDesignMode()
        {
            return DesignerProperties.GetIsInDesignMode(Dummy);
        } 
    }
}

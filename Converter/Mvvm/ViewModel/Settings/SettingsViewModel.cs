using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using Converter.Helpers;
using Converter.Mvvm.View;
using DataSource.Db;
using DataSource.Base;

namespace Converter.Mvvm.ViewModel.Settings
{
    internal interface ISettingsViewModel : IViewModelBase
    {
        ObservableCollection<Program> Programs { get; }
        Program SelectedProgram { get; }
        int SelectedIndex { get; set; }
        bool IsAddBtnFocus { get; set; }
        Visibility LoadingUserControlVisibility { get; }
        Visibility ProgramsVisibility { get; }
        RelayCommand AddProgramCommand { get; }
        RelayCommand EditProgramCommand { get;  }
        RelayCommand RemoveProgramCommand { get; }
    }

    internal sealed class SettingsViewModel : ViewModelBase, ISettingsViewModel
    {
        private readonly IFromDb _fromDb;

        private ObservableCollection<Program> _programs;
        public ObservableCollection<Program> Programs
        {
            get { return _programs; }
            set
            {
                _programs = value;
                OnPropertyChanged();
            }
        }

        public Program SelectedProgram { get; set; }

        private int _selectedIndex;
        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                OnPropertyChanged();
            }
        }

        private Visibility _loadingUserControlVisibility;
        public Visibility LoadingUserControlVisibility
        {
            get { return _loadingUserControlVisibility; }
            set
            {
                _loadingUserControlVisibility = value;
                OnPropertyChanged();
            }
        }

        private Visibility _programsVisibility;
        public Visibility ProgramsVisibility
        {
            get { return _programsVisibility; }
            set
            {
                _programsVisibility = value;
                OnPropertyChanged();
            }
        }

        private bool _isAddBtnFocus;
        public bool IsAddBtnFocus
        {
            get { return _isAddBtnFocus; }
            set
            {
                _isAddBtnFocus = value; 
                OnPropertyChanged();
            }
        }

        public RelayCommand AddProgramCommand { get; private set; }
        public RelayCommand EditProgramCommand { get; private set; }
        public RelayCommand RemoveProgramCommand { get; private set; }

        public SettingsViewModel()
        {
            _fromDb = new FromDb();
            AsyncGetProgramsFromDb();
            
            AddProgramCommand = new RelayCommand(AddWindowShow, CanAddProgram);
            EditProgramCommand = new RelayCommand(EditWindowShow, CanEditProgram);
            RemoveProgramCommand = new RelayCommand(RemoveWindowShow, CanRemoveProgram);

            _programsVisibility = Visibility.Collapsed;
        }

        public async void AsyncGetProgramsFromDb()
        {
            await GetProgramsFromDb();
            SetControlsLoadedState();
        }

        private Task GetProgramsFromDb()
        {
            return Task.Run(() =>
            {
                _fromDb.FillPrograms();
                Programs = _fromDb.GetPrograms();
            });
        }

        private void SetControlsLoadedState()
        {
            LoadingUserControlVisibility = Visibility.Collapsed;
            ProgramsVisibility = Visibility.Visible;
            SelectedIndex = -1;
            IsAddBtnFocus = true;
        }

        private void AddWindowShow(object parameter)
        {
            ViewModelLocator.AddDialogViewModel = new AddDialogViewModel(this);
            var dialogView = new AddDialogView
            {
                Owner = MainWindowOfApplication,
                ShowInTaskbar = false,
                Icon = MainWindowOfApplication.Icon
            };
            dialogView.ShowDialog();
        }

        private bool CanAddProgram(object obj)
        {
            return _programs != null;
        }

        private void EditWindowShow(object parameter)
        {
            ViewModelLocator.EditDialogViewModel = new EditDialogViewModel(this);
            var dialogView = new EditDialogView
            {
                Owner = MainWindowOfApplication,
                ShowInTaskbar = false,
                Icon = MainWindowOfApplication.Icon
            };
            dialogView.ShowDialog();
        }

        private bool CanEditProgram(object parameter)
        {
            return SelectedProgram != null;
        }

        private void RemoveWindowShow(object parameter)
        {
            ViewModelLocator.RemoveDialogViewModel = new RemoveDialogViewModel(this);
            var removeView = new RemoveView
            {
                Owner = MainWindowOfApplication,
                ShowInTaskbar = false,
                Icon = MainWindowOfApplication.Icon
            };
            removeView.ShowDialog();
        }

        private bool CanRemoveProgram(object parameter)
        {
            return SelectedProgram != null;
        }
    }
}

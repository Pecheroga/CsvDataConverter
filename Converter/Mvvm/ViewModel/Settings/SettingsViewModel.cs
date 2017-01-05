using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using Converter.Helpers;
using Converter.Mvvm.View;
using DataSource.Db;
using DataSource.Base;

namespace Converter.Mvvm.ViewModel.Settings
{
    public interface ISettingsViewModel
    {
        ObservableCollection<Program> Programs { get; set; }
        Program SelectedProgram { get; set; }
        int SelectedIndex { get; set; }
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

        public RelayCommand FillProgramsWithDbDataCommand { get; set; }
        public RelayCommand AddProgramCommand { get; set; }
        public RelayCommand EditProgramCommand { get; set; }
        public RelayCommand RemoveProgramCommand { get; set; }

        public SettingsViewModel()
        {
            _fromDb = new FromDb();
            AsyncGetProgramsFromDb();

            AddProgramCommand = new RelayCommand(AddWindowShow);
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
                _programs = _fromDb.GetPrograms();
            });
        }

        private void SetControlsLoadedState()
        {
            LoadingUserControlVisibility = Visibility.Collapsed;
            ProgramsVisibility = Visibility.Visible;
            OnPropertyChanged("Programs");
            SelectedIndex = -1;
        }

        private void AddWindowShow(object parameter)
        {
            var addDialogViewModel = new AddDialogViewModel(this);
            var dialogView = new AddEditDialogView
            {
                Owner = MainWindowOfApplication,
                ShowInTaskbar = false,
                DataContext = addDialogViewModel,
                Icon = MainWindowOfApplication.Icon
            };
            dialogView.ShowDialog();
        }

        private void EditWindowShow(object parameter)
        {
            var editDialogViewModel = new EditDialogViewModel(this);
            var dialogView = new AddEditDialogView
            {
                Owner = MainWindowOfApplication,
                ShowInTaskbar = false,
                DataContext = editDialogViewModel,
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
            var removeDialogViewModel = new RemoveDialogViewModel(this);
            var removeView = new RemoveView
            {
                Owner = MainWindowOfApplication,
                ShowInTaskbar = false,
                DataContext = removeDialogViewModel,
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

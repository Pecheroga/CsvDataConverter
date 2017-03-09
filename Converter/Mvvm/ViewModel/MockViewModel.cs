using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Converter.Helpers;
using Converter.Mvvm.ViewModel.Settings;
using DataSource.Base;

namespace Converter.Mvvm.ViewModel
{
    internal sealed class MockMainViewModel : IMainViewModel
    {
        public ObservableCollection<OutputProgram> OutputPrograms { get; private set;}
        public string NameOfChosenFile { get { return "Dummy"; }}
        public bool IsBrowseButtonFocused { get { return true; } }
        public Visibility ExcelDataContainerVisability { get { return Visibility.Visible; } }
        public Visibility SucessfulEndImgVisability { get { return Visibility.Visible; } }
        public int PercentValueProgressBar { get { return 50; } }
        public string NameOfOutputFile { get { return "Dummy"; } }
        public string NameOfCurrentWorkPhase { get { return "Dummy..."; } }
        public SolidColorBrush ColorOfProgressText { get { return Brushes.LightGreen; } }
        public RelayCommand BrowseCommand { get { return null; } }
        public RelayCommand StartCommand { get { return null; } }
        public RelayCommand CancelCommand { get { return null; } }
        public RelayCommand OpenOutputFileCommand { get { return null; } }
        public RelayCommand SaveAsCommand { get { return null; } }
        public RelayCommand SettingsWindowShowCommand { get { return null; } }
        public RelayCommand AboutWindowShowCommand { get { return null; } }
        public RelayCommand ViewLogCommand { get { return null; } }
        public string WindowTitle { get { return "Dummy"; } }
        public RelayCommand CloseWindowCommand { get { return null; } }

        public MockMainViewModel()
        {
            OutputPrograms = new ObservableCollection<OutputProgram>();
            for (var i = 0; i < 50; i++)
            {
                var outputProgram = new OutputProgram
                {
                    StartTime = "00:00:00",
                    EndTime = "00:00:00",
                    Title = "Dummy Dummy Dummy Dummy Dummy",
                    Author = "Dummy Dummy",
                    Lang = "Dummy Dummy",
                    Presenter = "Dummy Dummy",
                };
                OutputPrograms.Add(outputProgram);
            }
        }
    }

    internal sealed class MockSettingsViewModel: ISettingsViewModel
    {
        public ObservableCollection<Program> Programs { get; private set; }
        public Program SelectedProgram { get { return null; } }
        public int SelectedIndex { get; set; }
        public bool IsAddBtnFocus { get; set; }
        public bool IsAddBtnEnabled { get; set; }
        public Visibility LoadingUserControlVisibility { get { return Visibility.Collapsed; } }
        public Visibility ProgramsVisibility { get { return Visibility.Visible; } }
        public RelayCommand AddProgramCommand { get { return null; } }
        public RelayCommand EditProgramCommand { get { return null; } }
        public RelayCommand RemoveProgramCommand { get { return null; } }
        public string WindowTitle { get { return "Dummy"; } }
        public RelayCommand ViewLogCommand { get { return null; } }
        public RelayCommand CloseWindowCommand { get { return null; } }

        public MockSettingsViewModel()
        {
            Programs = new ObservableCollection<Program>();
            for (var i = 0; i < 50; i++)
            {
                var program = new Program
                {
                    Id = 0,
                    Title = "Dummy Dummy Dummy Dummy Dummy",
                    Author = "Dummy Dummy",
                    Lang = "Dummy Dummy",
                    Presenter = "Dummy Dummy",
                    EndLabel = "Dummy Dummy",
                    StartLabel = "Dummy Dummy"
                };
                Programs.Add(program);
            }
        }
    }

    internal class MockDialogViewModelBase : IDialogViewModelBase
    {
        public Program SelectedProgram
        {
            get
            {
                return new Program
                {
                    Id = 0,
                    Title = "Dummy",
                    Author = "Dummy",
                    StartLabel = "Dummy",
                    EndLabel = "Dummy",
                    Lang = "Dummy",
                    Presenter = "Dummy",
                };
            }
        }

        public string WindowTitle { get { return "Dummy"; } }
        public RelayCommand ViewLogCommand { get { return null; } }
        public RelayCommand CloseWindowCommand { get { return null; } }
        public RelayCommand OkCommand { get { return null; } }
        public BindingGroup EditBindingGroup { get { return null; } }
    }

    internal sealed class MockEditDialogViewModelBase : MockDialogViewModelBase, IEditDialogViewModel
    {
        public RelayCommand ApplyCommand { get { return null; } }
        public RelayCommand UndoCommand { get { return null; } }
    }

    internal sealed class MockRemoveDialogViewModel:IRemoveDialogViewModel
    {
        public string ConfirmText { get { return "Dummy"; } }
        public string WindowTitle { get { return "Dummy"; } }
        public RelayCommand ViewLogCommand { get { return null; } }
        public RelayCommand CloseWindowCommand { get { return null; } }
        public Program SelectedProgram { get { return null; } }
        public RelayCommand OkCommand { get { return null; } }
        public BindingGroup EditBindingGroup { get { return null; } }
    }
}

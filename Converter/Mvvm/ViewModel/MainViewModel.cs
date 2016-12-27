using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using Converter.Helpers;
using Converter.Mvvm.Model;
using Converter.Mvvm.View;
using DataSource.Base;
using Microsoft.Win32;

namespace Converter.Mvvm.ViewModel
{
    public interface IMainViewModel { }

    class MainViewModel : ViewModelBase, IMainViewModel
    {
        private bool _canSaveAs;
        private bool _canOpenOutputFile;

        private readonly MainModel _mainModel;
        public ObservableCollection<OutputProgram> OutputPrograms
        {
            get { return _mainModel.OutputPrograms; }
        }

        private string _nameOfChosenFile;
        public string NameOfChosenFile
        {
            get { return _nameOfChosenFile; }
            set
            {
                _nameOfChosenFile = value;
                OnPropertyChanged();
            }
        }

        private bool _isBrowseButtonFocused;
        public bool IsBrowseButtonFocused
        {
            get { return _isBrowseButtonFocused; }
            set
            {
                _isBrowseButtonFocused = value;
                OnPropertyChanged();
            }
        }

        private Visibility _excelDataContainerVisability;
        public Visibility ExcelDataContainerVisability
        {
            get { return _excelDataContainerVisability; }
            set
            {
                _excelDataContainerVisability = value;
                OnPropertyChanged();
            }
        }

        private Visibility _sucessfulEndImgVisability;
        public Visibility SucessfulEndImgVisability
        {
            get { return _sucessfulEndImgVisability; }
            set
            {
                _sucessfulEndImgVisability = value;
                OnPropertyChanged();
            }
        }

        public int ValueProgressBar
        {
            get { return _mainModel.ValueProgressBar; }
            set { _mainModel.ValueProgressBar = value; }
        }

        public int PercentValueProgressBar
        {
            get { return _mainModel.PercentValueProgressBar; }
            set { _mainModel.PercentValueProgressBar = value; }
        }

        public double MaximumProgressBar
        {
            get { return _mainModel.MaximumProgressBar; }
            set { _mainModel.MaximumProgressBar = value; }
        }

        public string NameOfOutputFile
        {
            get { return _mainModel.NameOfOutputFile; }
            set { _mainModel.NameOfOutputFile = value; }
        }

        private SolidColorBrush _colorOfProgressText;
        public SolidColorBrush ColorOfProgressText
        {
            get { return _colorOfProgressText; }
            set
            {
                _colorOfProgressText = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand BrowseCommand { get; set; }
        public RelayCommand StartCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }
        public RelayCommand OpenOutputFileCommand { get; set; }
        public RelayCommand SaveAsCommand { get; set; }
        public RelayCommand SettingsWindowShowCommand { get; set; }
        public RelayCommand AboutWindowShowCommand { get; set; }

        public MainViewModel()
        {
            BrowseCommand = new RelayCommand(BrowseSourceFile, CanBrowseSourceFile);
            StartCommand = new RelayCommand(StartAsyncParsing, CanStartAsyncParsing);
            CancelCommand = new RelayCommand(CancelParsing, CanCancelParsing);
            OpenOutputFileCommand = new RelayCommand(OpenOutputFile, CanOpenOutputExcel);
            SaveAsCommand = new RelayCommand(SaveAs, CanSaveAs);
            SettingsWindowShowCommand = new RelayCommand(SettingsWindowShow);
            AboutWindowShowCommand = new RelayCommand(AboutWindowShow);

            _mainModel = new MainModel();
            _mainModel.PropertyChanged += _mainModel_PropertyChanged;
            _mainModel.ParsingInSeparateThread.RunWorkerCompleted += ParsingInSeparateThread_RunWorkerCompleted;

            SetControlsDefaultState();
        }

        private void _mainModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        private void ParsingInSeparateThread_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            var isParsingCanceled = e.Cancelled;
            if (isParsingCanceled)
            {
                SetControlsDefaultState();
            }
            else
            {
                SetControlsStateThenParsingComleted();
            }
        }

        private void SetControlsDefaultState()
        {
            ColorOfProgressText = Brushes.Azure;
            ValueProgressBar = 0;
            PercentValueProgressBar = 0;
            MaximumProgressBar = 100;
            IsBrowseButtonFocused = true;
            ExcelDataContainerVisability = Visibility.Hidden;
            SucessfulEndImgVisability = Visibility.Hidden;
        }

        private void SetControlsStateThenParsingComleted()
        {
            ColorOfProgressText = Brushes.LightGreen;
            IsBrowseButtonFocused = true;
            ExcelDataContainerVisability = Visibility.Visible;
            SucessfulEndImgVisability = Visibility.Visible;
            _canSaveAs = true;
            _canOpenOutputFile = true;
        }

        private void BrowseSourceFile(object parameter)
        {
            var chooseFileDialog = new OpenFileDialog
            {
                DefaultExt = ".csv",
                Filter = "*.csv|*.csv"
            };
            var isFileChosen = chooseFileDialog.ShowDialog().GetValueOrDefault();
            if (!isFileChosen) return;

            NameOfChosenFile = chooseFileDialog.FileName;
        }

        private bool CanBrowseSourceFile(object parameter)
        {
            return true;
        }

        private void StartAsyncParsing(object parameter)
        {
            if (_mainModel.ParsingInSeparateThread.IsBusy == false)
            {
                _mainModel.ParsingInSeparateThread.RunWorkerAsync(NameOfChosenFile);
            }
            SetControlsStateWhileParsingIsOccurs();
        }

        private void SetControlsStateWhileParsingIsOccurs()
        {
            IsBrowseButtonFocused = false;
            ExcelDataContainerVisability = Visibility.Hidden;
            SucessfulEndImgVisability = Visibility.Hidden;
            ColorOfProgressText = Brushes.Azure;
            _canSaveAs = false;
            _canOpenOutputFile = false;
        }

        private bool CanStartAsyncParsing(object parameter)
        {
            return !_mainModel.ParsingInSeparateThread.IsBusy && NameOfChosenFile != null;
        }

        private void CancelParsing(object parameter)
        {
            _mainModel.ParsingInSeparateThread.CancelAsync();
        }

        private bool CanCancelParsing(object parameter)
        {
            return _mainModel.ParsingInSeparateThread.IsBusy;
        }

        private void SaveAs(object parameter)
        {
            _mainModel.TrySaveAsCopyOfOutputFile();
        }

        private bool CanSaveAs(object parameter)
        {
            return _canSaveAs;
        }

        private void OpenOutputFile(object parameter)
        {
            _mainModel.TryOpenOutputFile();
        }

        private bool CanOpenOutputExcel(object parameter)
        {
            return _canOpenOutputFile;
        }

        private void SettingsWindowShow(object parameter)
        {
            var settingsView = new SettingsView
            {
                Owner = MainWindowOfApplication,
                ShowInTaskbar = false,
                Icon = MainWindowOfApplication.Icon
            };
            settingsView.ShowDialog();
        }

        private void AboutWindowShow(object parameter)
        {
            var aboutView = new AboutView
            {
                Owner = MainWindowOfApplication,
                ShowInTaskbar = false,
                Icon = MainWindowOfApplication.Icon
            };
            aboutView.ShowDialog();
        }
    }
}

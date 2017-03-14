using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Converter.Helpers;
using Converter.Mvvm.Model;
using Converter.Mvvm.View;
using DataSource.Structure;
using Microsoft.Win32;

namespace Converter.Mvvm.ViewModel
{
    internal interface IMainViewModel: IViewModelBase
    {
        ObservableCollection<OutputProgram> OutputPrograms { get; }
        string NameOfChosenFile { get; }
        bool IsBrowseButtonFocused { get; }
        Visibility ExcelDataContainerVisability { get; }
        Visibility SucessfulEndImgVisability { get; }
        int PercentValueProgressBar { get; }
        string NameOfCurrentWorkPhase { get; }
        string NameOfOutputFile { get; }
        SolidColorBrush ColorOfProgressText { get; }
        RelayCommand BrowseCommand { get; }
        RelayCommand StartCommand { get; }
        RelayCommand CancelCommand { get; }
        RelayCommand OpenOutputFileCommand { get; }
        RelayCommand SaveAsCommand { get; }
        RelayCommand SettingsWindowShowCommand { get; }
        RelayCommand AboutWindowShowCommand { get; }
    }

    internal sealed class MainViewModel : ViewModelBase, IMainViewModel
    {
        private bool _canSaveAs;
        private bool _canOpenOutputFile;
        
        private readonly Worker _worker;
        public ObservableCollection<OutputProgram> OutputPrograms
        {
            get { return _worker.OutputPrograms; }
        }

        private string _nameOfChosenFile;
        public string NameOfChosenFile
        {
            get { return _nameOfChosenFile; }
            private set
            {
                _nameOfChosenFile = value;
                OnPropertyChanged();
            }
        }

        private bool _isBrowseButtonFocused;
        public bool IsBrowseButtonFocused
        {
            get { return _isBrowseButtonFocused; }
            private set
            {
                _isBrowseButtonFocused = value;
                OnPropertyChanged();
            }
        }

        private Visibility _excelDataContainerVisability;
        public Visibility ExcelDataContainerVisability
        {
            get { return _excelDataContainerVisability; }
            private set
            {
                _excelDataContainerVisability = value;
                OnPropertyChanged();
            }
        }

        private Visibility _sucessfulEndImgVisability;
        public Visibility SucessfulEndImgVisability
        {
            get { return _sucessfulEndImgVisability; }
            private set
            {
                _sucessfulEndImgVisability = value;
                OnPropertyChanged();
            }
        }

        public int PercentValueProgressBar
        {
            get { return _worker.PercentValueProgressBar; }
        }

        public string NameOfOutputFile
        {
            get { return _worker.NameOfOutputFile; }
        }

        public string NameOfCurrentWorkPhase
        {
            get { return _worker.NameOfCurrentWorkPhase; }
        }

        private SolidColorBrush _colorOfProgressText;
        public SolidColorBrush ColorOfProgressText
        {
            get { return _colorOfProgressText; }
            private set
            {
                _colorOfProgressText = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand BrowseCommand { get; private set; }
        public RelayCommand StartCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }
        public RelayCommand OpenOutputFileCommand { get; private set; }
        public RelayCommand SaveAsCommand { get; private set; }
        public RelayCommand SettingsWindowShowCommand { get; private set; }
        public RelayCommand AboutWindowShowCommand { get; private set; }
        
        public MainViewModel()
        {
            _worker = new Worker();

            InitializeCommands();
            WireUpEventHandlers();
            SetDefaultControlsState();
        }

        private void InitializeCommands()
        {
            BrowseCommand = new RelayCommand(BrowseSourceFile, CanBrowseSourceFile);
            StartCommand = new RelayCommand(StartAsyncParsing, CanStartAsyncParsing);
            CancelCommand = new RelayCommand(CancelParsing, CanCancelParsing);
            OpenOutputFileCommand = new RelayCommand(OpenOutputFile, CanOpenOutputFile);
            SaveAsCommand = new RelayCommand(SaveAs, CanSaveAs);
            SettingsWindowShowCommand = new RelayCommand(SettingsWindowShow);
            AboutWindowShowCommand = new RelayCommand(AboutWindowShow);
        }

        private void BrowseSourceFile(object parameter)
        {
            var chooseFileDialog = new OpenFileDialog
            {
                DefaultExt = ".csv",
                Filter = "*.csv|*.csv|All files (*.*)|*.*"
            };
            var isFileChosen = chooseFileDialog.ShowDialog().GetValueOrDefault();
            if (!isFileChosen) return;

            NameOfChosenFile = chooseFileDialog.FileName;
        }

        private bool CanBrowseSourceFile(object parameter)
        {
            return !_worker.MainWorker.IsBusy;
        }

        private void StartAsyncParsing(object parameter)
        {
            SetParsingIsOccursControlsState();
            if (_worker.MainWorker.IsBusy == false)
            {
                _worker.MainWorker.RunWorkerAsync(NameOfChosenFile);
            }
        }

        private void SetParsingIsOccursControlsState()
        {
            _worker.PercentValueProgressBar = 0;
            ColorOfProgressText = Brushes.Azure;
            IsBrowseButtonFocused = false;
            ExcelDataContainerVisability = Visibility.Hidden;
            SucessfulEndImgVisability = Visibility.Hidden;
            _canSaveAs = false;
            _canOpenOutputFile = false;
        }

        private bool CanStartAsyncParsing(object parameter)
        {
            return !_worker.MainWorker.IsBusy && NameOfChosenFile != null;
        }

        private void CancelParsing(object parameter)
        {
            if (_worker.MainWorker.IsBusy)
            {
                _worker.MainWorker.CancelAsync();
            }
        }

        private bool CanCancelParsing(object parameter)
        {
            return _worker.MainWorker.IsBusy;
        }

        private void SaveAs(object parameter)
        {
            var outputFileNameWithoutExtension = Path.GetFileNameWithoutExtension(NameOfOutputFile);
            var saveDialog = new SaveFileDialog
            {
                FileName = outputFileNameWithoutExtension,
                DefaultExt = ".xlsx",
                Filter = "*.xlsx|*.xlsx"
            };
            var saveDialogResult = saveDialog.ShowDialog().GetValueOrDefault();
            if (!saveDialogResult) return;
            File.Copy(NameOfOutputFile, saveDialog.FileName);
        }

        private bool CanSaveAs(object parameter)
        {
            return _canSaveAs;
        }

        private void OpenOutputFile(object parameter)
        {
            Process.Start(NameOfOutputFile);
        }

        private bool CanOpenOutputFile(object parameter)
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

        private void WireUpEventHandlers()
        {
            _worker.PropertyChanged += _worker_PropertyChanged;
            _worker.MainWorker.RunWorkerCompleted += MainWorker_RunWorkerCompleted;
        }

        private void _worker_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged(e.PropertyName);
        }

        private void MainWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                SetDefaultControlsState();
                if (e.Error.InnerException != null) throw new Exception(e.Error.Message, e.Error);
                const string defaultMessage = 
                    "Error has occurred while process of parsing or converting.\nCheck log file for more information.";
                throw new Exception(defaultMessage, e.Error);
            }
            SetParsingComletedControlsState();
        }

        private void SetDefaultControlsState()
        {
            _worker.PercentValueProgressBar = 0;
            ColorOfProgressText = Brushes.Azure;
            IsBrowseButtonFocused = true;
            ExcelDataContainerVisability = Visibility.Hidden;
            SucessfulEndImgVisability = Visibility.Hidden;
        }

        private void SetParsingComletedControlsState()
        {
            ColorOfProgressText = Brushes.LightGreen;
            IsBrowseButtonFocused = true;
            ExcelDataContainerVisability = Visibility.Visible;
            SucessfulEndImgVisability = Visibility.Visible;
            _canSaveAs = true;
            _canOpenOutputFile = true;
        }
    }
}

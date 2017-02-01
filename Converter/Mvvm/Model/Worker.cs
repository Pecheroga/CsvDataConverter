using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Converter.Helpers;
using Converter.Mvvm.View;
using DataSource.Base;

namespace Converter.Mvvm.Model
{
    internal delegate void LoadNumberDelegate(int number);

    internal sealed class Worker : Notifier
    {
        private string _nameOfChosenFile;
        private int _prevProgressValue;
        private int _savedValueOfComplitedSteps;
        private const int WorkStepsCount = 4;
        private const int WorkStepValue = 100 / WorkStepsCount;
        private List<string[]> _parsedData;
        private LoadNumberDelegate _tempToOutputProgramsRef;
        private DispatcherOperation _programsLoadingProcess;
        private readonly MainView _mainView 
            = Application.Current.Windows.OfType<MainView>().Single();

        public BackgroundWorker MainWorker { get; private set; }

        private ObservableCollection<OutputProgram> _tempPrograms 
            = new ObservableCollection<OutputProgram>();

        private ObservableCollection<OutputProgram> _outputPrograms 
            = new ObservableCollection<OutputProgram>();
        public ObservableCollection<OutputProgram> OutputPrograms
        {
            get { return _outputPrograms; }
            set
            {
                _outputPrograms = value;
                OnPropertyChanged();
            }
        }

        private int _percentValueProgressBar;
        public int PercentValueProgressBar
        {
            get { return _percentValueProgressBar; }
            set
            {
                _percentValueProgressBar = value;
                OnPropertyChanged();
            }
        }

        private string _nameOfCurrentWorkPhase;
        public string NameOfCurrentWorkPhase
        {
            get { return _nameOfCurrentWorkPhase; }
            set
            {
                _nameOfCurrentWorkPhase = value; 
                OnPropertyChanged();
            }
        }
        

        private string _nameOfOutputFile;
        public string NameOfOutputFile
        {
            get { return _nameOfOutputFile; }
            set
            {
                _nameOfOutputFile = value;
                OnPropertyChanged();
            }
        }

        public Worker()
        {
            MainWorker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            MainWorker.DoWork += MainWorker_DoWork;
            MainWorker.ProgressChanged += MainWorker_ProgressChanged;
            MainWorker.RunWorkerCompleted += MainWorker_RunWorkerCompleted;
        }

        private void MainWorker_DoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            _nameOfChosenFile = (string)doWorkEventArgs.Argument;
            if (_programsLoadingProcess != null) _programsLoadingProcess.Abort();

            ParseSourceFileToArray();
            ConvertArrayToOutputPrograms();
            SaveOutputProgramsToFile();
        }

        private void ParseSourceFileToArray()
        {
            NameOfCurrentWorkPhase = "Parsing...";
            var parser = new Parser(_nameOfChosenFile, worker:this);
            parser.Start();
            _parsedData = parser.GetOutputArray();
        }

        private void ConvertArrayToOutputPrograms()
        {
            var converter = new Converter(_parsedData, this);
            NameOfCurrentWorkPhase = "Connecting to database...";
            converter.GetProgramsFromDb();
            NameOfCurrentWorkPhase = "Converting...";
            converter.ConvertSourceToOutputProgram();
            _tempPrograms = converter.GetOutputPrograms();
        }

        private void SaveOutputProgramsToFile()
        {
            NameOfCurrentWorkPhase = "Saving...";
            var save = new Save(_nameOfChosenFile, this);
            save.SaveOutputFile(_tempPrograms);
            NameOfOutputFile = save.NameOfOutputFile;
        }

        public void OnProgressChanged(int interation, int maximum)
        {
            var percentProgress = (int)((double)interation / maximum * 100);
            if (percentProgress != _prevProgressValue)
            {
                MainWorker.ReportProgress(percentProgress);
                _prevProgressValue = percentProgress;
            }
            if (MainWorker.CancellationPending) 
                throw new Exception("Parsing process has been canceled.", new Exception());
        }

        private void MainWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (PercentValueProgressBar == WorkStepValue + _savedValueOfComplitedSteps)
            {
                _savedValueOfComplitedSteps += WorkStepValue;
            }
            var percentValueOfActivePhase = e.ProgressPercentage/WorkStepsCount;
            PercentValueProgressBar = percentValueOfActivePhase + _savedValueOfComplitedSteps;    
        }

        private void MainWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Reset();
            if (e.Error != null) return;
            RunOutputProgramsLoading();
        }

        private void Reset()
        {
            _savedValueOfComplitedSteps = 0;
            NameOfCurrentWorkPhase = string.Empty;
            OutputPrograms.Clear();
        }

        private void RunOutputProgramsLoading()
        {
            _tempToOutputProgramsRef = TempToOutputPrograms;
            
            _programsLoadingProcess = 
                _mainView.Dispatcher.BeginInvoke(DispatcherPriority.Background, _tempToOutputProgramsRef, 0);
        }

        private void TempToOutputPrograms(int number)
        {
            OutputPrograms.Add(_tempPrograms[number]);

            if (number >= _tempPrograms.Count - 1) return;
            _programsLoadingProcess =
                _mainView.Dispatcher.BeginInvoke(DispatcherPriority.Background, _tempToOutputProgramsRef, ++number);
        }
    }
}

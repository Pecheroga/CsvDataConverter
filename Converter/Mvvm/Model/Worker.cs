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
    internal sealed class Worker : Notifier
    {
        private string _nameOfChosenFile;
        private int _totalCallsProgressChanged;
        private int _prevProgressValue;
        private List<string[]> _parsedData;
        private delegate void LoadNumberDelegate(int number);
        private LoadNumberDelegate _tempToOutputProgramsRef;
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
            _tempToOutputProgramsRef = null;
            ParseSourceFileToArray();
            ConvertArrayToOutputPrograms();
            SaveOutputProgramsToFile();
        }

        private void ParseSourceFileToArray()
        {
            var parser = new Parser(_nameOfChosenFile);
            parser.Start(this);
            _parsedData = parser.GetOutputArray();
        }

        private void ConvertArrayToOutputPrograms()
        {
            var converter = new Converter(_parsedData);
            converter.ConvertSourceToOutputProgram(this);
            _tempPrograms = converter.GetOutputPrograms();
        }

        private void SaveOutputProgramsToFile()
        {
            var save = new Save(_nameOfChosenFile);
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
            _totalCallsProgressChanged++;
            if (_totalCallsProgressChanged % 2 != 0) return;
            PercentValueProgressBar += 1;    
        }

        private void MainWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _totalCallsProgressChanged = 0;
            OutputPrograms.Clear();

            if (e.Error != null) return;
            RunOutputProgramsLoading();
        }

        private void RunOutputProgramsLoading()
        {
            _tempToOutputProgramsRef = TempToOutputPrograms;
            _mainView.Dispatcher.BeginInvoke(DispatcherPriority.Background, _tempToOutputProgramsRef, 0);
        }

        private void TempToOutputPrograms(int number)
        {
            OutputPrograms.Add(_tempPrograms[number]);

            if (number >= _tempPrograms.Count - 1) return;
            _mainView.Dispatcher.BeginInvoke(DispatcherPriority.Background, _tempToOutputProgramsRef, ++number);
        }
    }
}

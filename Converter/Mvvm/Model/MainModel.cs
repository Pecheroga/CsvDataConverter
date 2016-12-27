using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Converter.Helpers;
using DataSource.Base;

namespace Converter.Mvvm.Model
{
    internal class MainModel : Notifier
    {
        private string _nameOfChosenFile;
        private SourceFile _sourceFile;
        private OutputFile _outputFile;
        private Exception _exception;
        private CancelEventArgs _parsingState;

        public BackgroundWorker ParsingInSeparateThread { get; set; }

        private ObservableCollection<OutputProgram> _outputPrograms;
        public ObservableCollection<OutputProgram> OutputPrograms
        {
            get { return _outputPrograms; }
            set
            {
                _outputPrograms = value;
                OnPropertyChanged();
            }
        }

        private int _valueProgressBar;
        public int ValueProgressBar
        {
            get { return _valueProgressBar; }
            set
            {
                _valueProgressBar = value;
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

        private double _maximumProgressBar;
        public double MaximumProgressBar
        {
            get { return _maximumProgressBar; }
            set
            {
                _maximumProgressBar = value;
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

        public MainModel()
        {
            OutputPrograms = new ObservableCollection<OutputProgram>();
            ParsingInSeparateThread = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };
            ParsingInSeparateThread.DoWork += ParsingInSeparateThread_DoWork;
            ParsingInSeparateThread.ProgressChanged += ParsingInSeparateThread_ProgressChanged;
            ParsingInSeparateThread.RunWorkerCompleted += ParsingInSeparateThread_RunWorkerCompleted;
        }

        private void ParsingInSeparateThread_DoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            ResetDataOfPreviousParsing();
            _nameOfChosenFile = (string)doWorkEventArgs.Argument;
            _parsingState = doWorkEventArgs;
            TryParseSourceFile();
        }

        private void ResetDataOfPreviousParsing()
        {
            OutputPrograms = null;
            _sourceFile = null;
            _outputFile = null;
        }

        private void TryParseSourceFile()
        {
            try
            {
                _sourceFile = new SourceFile(_nameOfChosenFile);
                MaximumProgressBar = _sourceFile.CountRowsOfFirstWorksheet;
                _sourceFile.Start(ParsingInSeparateThread);
                OutputPrograms = _sourceFile.GetOutputPrograms();
            }
            catch (Exception exception)
            {
                _exception = exception;
                ExceptionHandler();
            }
        }

        private void ParsingInSeparateThread_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ValueProgressBar = e.ProgressPercentage;
            PercentValueProgressBar = (int)(ValueProgressBar / MaximumProgressBar * 100);
        }

        private void ParsingInSeparateThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var isParsingCanceled = e.Cancelled;
            if (isParsingCanceled) return;

            TrySaveOutputFile();
            NameOfOutputFile = _outputFile.NameOfOutputFile;
        }

        private void TrySaveOutputFile()
        {
            try
            {
                _outputFile = new OutputFile(_nameOfChosenFile);
                _outputFile.SaveOutputFile(OutputPrograms);
            }
            catch (Exception exception)
            {
                _exception = exception;
                ExceptionHandler();
            }
        }

        public void TrySaveAsCopyOfOutputFile()
        {
            try
            {
                _outputFile.SaveAsCopyOfOutputFile();
            }
            catch (Exception exception)
            {
                _exception = exception;
                ExceptionHandler();
            }
        }

        public void TryOpenOutputFile()
        {
            try
            {
                _outputFile.Open();
            }
            catch (Exception exception)
            {
                _exception = exception;
                ExceptionHandler();
            }
        }

        private void ExceptionHandler()
        {
            if (_sourceFile != null)
            {
                _sourceFile.SourceExcelApp.QuitExcelApp();
            }
            if (_outputFile != null)
            {
                _outputFile.OutputExcelApp.QuitExcelApp();
            }

            _parsingState.Cancel = true;
            var myException = new ExceptionWindow(_exception);
            myException.ShowExceptionWindow();
        }
    }
}

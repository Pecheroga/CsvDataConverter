using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Converter.Helpers;
using DataSource.Base;

namespace Converter.Mvvm.Model
{
    internal sealed class MainModel : Notifier
    {
        private string _nameOfChosenFile;

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
        }

        private void ParsingInSeparateThread_DoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            ParsingInSeparateThread.RunWorkerCompleted += ParsingInSeparateThread_RunWorkerCompleted;
            ResetDataOfPreviousParsing();
            _nameOfChosenFile = (string)doWorkEventArgs.Argument;
            ParseSourceFile();
        }

        private void ResetDataOfPreviousParsing()
        {
            OutputPrograms = null;
        }

        private void ParseSourceFile()
        {
             var sourceFile = new SourceFile(_nameOfChosenFile);
            MaximumProgressBar = sourceFile.RowsCountOfFirstWorksheet;
            sourceFile.Start(ParsingInSeparateThread);
            OutputPrograms = sourceFile.GetOutputPrograms();
        }

        private void ParsingInSeparateThread_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ValueProgressBar = e.ProgressPercentage;
            PercentValueProgressBar = (int)(ValueProgressBar / MaximumProgressBar * 100);
        }

        private void ParsingInSeparateThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                throw new Exception(e.Error.Message);
            }
            SaveOutputFile();
        }

        private void SaveOutputFile()
        {
            var outputFile = new OutputFile(_nameOfChosenFile);
            outputFile.SaveOutputFile(OutputPrograms);
            NameOfOutputFile = outputFile.NameOfOutputFile;
        }
    }
}

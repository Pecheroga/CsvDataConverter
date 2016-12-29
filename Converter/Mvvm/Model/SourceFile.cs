using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using DataSource.Db;
using DataSource.Base;
using Microsoft.Office.Interop.Excel;

namespace Converter.Mvvm.Model
{
    internal sealed class SourceFile
    {
        private SourceProgram _newSourceProgram;
        private OutputProgram _outputProgram;
        private readonly ObservableCollection<Program> _programs;
        private readonly ObservableCollection<OutputProgram> _outputPrograms;
        private readonly ExcelAppFromFile _sourceExcelApp;
        private readonly Worksheet _sourceFirstWorksheet;
        private bool _toNextRowWithoutSave;
        private readonly int _countColumnsOfFirstWorksheet;
        private readonly ArrayList _sourceFileColumnNames;

        public ExcelAppFromFile SourceExcelApp
        {
            get { return _sourceExcelApp; }
        }

        public int CountRowsOfFirstWorksheet { get; set; }

        public SourceFile(string nameOfChosenFile)
        {
            _outputPrograms = new ObservableCollection<OutputProgram>();
            IFromDb fromDb = new FromDb();
            fromDb.FillPrograms();
            _programs = fromDb.GetPrograms();
            _sourceExcelApp = new ExcelAppFromFile(nameOfChosenFile);
            _sourceFirstWorksheet = _sourceExcelApp.GetFirstWorksheet();
            CountRowsOfFirstWorksheet = _sourceFirstWorksheet.UsedRange.Rows.Count;
            _sourceFileColumnNames = new ArrayList();
            _countColumnsOfFirstWorksheet = _sourceFirstWorksheet.UsedRange.Columns.Count;
            TryParseColumnNamesFromSourceFile();
        }

        private void TryParseColumnNamesFromSourceFile()
        {
            try
            {
                ParseColumnNamesFromSourceFile();
            }
            catch (Exception)
            {
                _sourceExcelApp.QuitExcelApp();
                throw new Exception("Fail to parsing column names in the source file.");
            }
        }

        private void ParseColumnNamesFromSourceFile()
        {
            for (var parsingColumn = 1; parsingColumn <= _countColumnsOfFirstWorksheet; parsingColumn++)
            {
                var columnNameCell = _sourceFirstWorksheet.UsedRange.Cells[1, parsingColumn] as Range;
                if (columnNameCell == null) continue;
                var columnName = columnNameCell.Value2.ToString();
                _sourceFileColumnNames.Add(columnName);
            }
        }

        public void Start(BackgroundWorker parsingInSeparateThread)
        {
            for (var parsingRow = 2; parsingRow <= CountRowsOfFirstWorksheet; parsingRow++)
            {
                if (parsingInSeparateThread.CancellationPending)
                {
                    throw new Exception("Parsing in canceled by User");
                }
                parsingInSeparateThread.ReportProgress(parsingRow);
                CheckFormatStartTimeIn(parsingRow);
                _toNextRowWithoutSave = false;
                FillNewSourceProgramFrom(parsingRow);
                if (_toNextRowWithoutSave) continue;
                AddToOutputPrograms(_newSourceProgram);
            }
            _sourceExcelApp.QuitExcelApp();
        }

        private void CheckFormatStartTimeIn(int parsingRow)
        {
            var startTimeColumnIndex = _sourceFileColumnNames.IndexOf("Start Time");
            if (startTimeColumnIndex == -1) return;
            var startTimeCellColumnIndex = startTimeColumnIndex + 1;
            var startTimeCell = _sourceFirstWorksheet.UsedRange.Cells[parsingRow, startTimeCellColumnIndex] as Range;
            if (startTimeCell == null) return;
            var startTime = startTimeCell.Value2.ToString();
            if (startTime.Contains("(1)"))
            {
                throw new Exception("The Start Time column of source file " +
                                    "can't contains next day symblol \"(1)\"");
            }
        }

        private void FillNewSourceProgramFrom(int parsingRow)
        {
            _newSourceProgram = new SourceProgram();
            for (var parsingColumn = 1; parsingColumn <= _countColumnsOfFirstWorksheet; parsingColumn++)
            {
                var parsingCell = _sourceFirstWorksheet.UsedRange.Cells[parsingRow, parsingColumn] as Range;
                if (parsingCell == null) continue;

                var columnName = (string)_sourceFileColumnNames[parsingColumn - 1];

                switch (columnName)
                {
                    case "Start Time":
                        _newSourceProgram.SourceStartTime = parsingCell.Value2;
                        break;
                    case "Title":
                        _newSourceProgram.SourceTitle = parsingCell.Value2.ToString();
                        break;
                    case "Duration":
                        var sourceDuration = parsingCell.Value2;
                        if (sourceDuration == 0) _toNextRowWithoutSave = true;
                        _newSourceProgram.SourceDuration = sourceDuration;
                        break;
                    default:
                        continue;
                }
            }
        }

        public void AddToOutputPrograms(SourceProgram sourceProgram)
        {
            _outputProgram = new OutputProgram
            {
                StartTime = TimeSpan.FromDays(sourceProgram.SourceStartTime).ToString(),
                EndTime = TimeSpan.FromDays(sourceProgram.SourceStartTime + sourceProgram.SourceDuration).ToString(),
                Title = sourceProgram.SourceTitle,
                Lang = "-------------",
                Presenter = "-------------",
                Author = "-------------"
            };

            foreach (var program in _programs)
            {
                if (!sourceProgram.SourceTitle.Contains(program.EndLabel)) continue;

                if (program.StartLabel != program.EndLabel)
                {
                    TryToReverseMerge(program);
                }

                _outputProgram.Title = program.Title;
                _outputProgram.Lang = program.Lang;
                _outputProgram.Presenter = program.Presenter;
                _outputProgram.Author = program.Author;

                break;
            }

            _outputPrograms.Add(_outputProgram);
        }

        private void TryToReverseMerge(IProgram program)
        {
            try
            {
                string newStartTime = ReverseMergeFromEndLabelTo(program.StartLabel);
                if (!string.IsNullOrEmpty(newStartTime))
                {
                    _outputProgram.StartTime = newStartTime;
                }
            }
            catch (Exception)
            {
                throw new Exception("Error while merging the \"" + program.Title + "\".\n " +
                                        "Check the Start and End Labels in the Settings.");
            }
        }

        private string ReverseMergeFromEndLabelTo(string startLable)
        {
            var countRows = _outputPrograms.Count;
            string currentTitle;
            string startTime;
            do
            {
                currentTitle = _outputPrograms[countRows - 1].Title;
                startTime = _outputPrograms[countRows - 1].StartTime;
                _outputPrograms.RemoveAt(countRows - 1);
                countRows--;
            } while (!currentTitle.Contains(startLable));

            return startTime;
        }

        public ObservableCollection<OutputProgram> GetOutputPrograms()
        {
            return _outputPrograms;
        }
    }
}

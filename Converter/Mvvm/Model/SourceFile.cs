using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using DataSource.Db;
using DataSource.Base;
using Microsoft.VisualBasic.FileIO;

namespace Converter.Mvvm.Model
{
    internal sealed class SourceFile
    {
        private readonly string _nameOfChosenFile;
        private readonly List<string[]> _playlist = new List<string[]>();
        private string[] _columnNames;
        private int _columnsCount;
        private bool _toNextRowWithoutSave;
        private ObservableCollection<Program> _programs;
        private SourceProgram _newSourceProgram;
        private OutputProgram _outputProgram;
        private readonly ObservableCollection<OutputProgram> _outputPrograms = new ObservableCollection<OutputProgram>();

        public int RowsCountOfSourceFile { get; set; }

        public SourceFile(string nameOfChosenFile)
        {
            _nameOfChosenFile = nameOfChosenFile;
        }

        public void Parse()
        {
            using (var csv = new TextFieldParser(_nameOfChosenFile, Encoding.GetEncoding(1251)))
            {
                csv.Delimiters = new[] {";"};
                csv.TextFieldType = FieldType.Delimited;
                csv.HasFieldsEnclosedInQuotes = false;

                var line = new List<string>();
                while (!csv.EndOfData)
                {
                    var fields = csv.ReadFields();
                    if (fields == null) continue;
                    line.AddRange(
                        from field in fields
                        where field != null
                        select field.Trim('/', '"'));
                    _playlist.Add(line.ToArray());
                    line.Clear();
                }
            }

            RowsCountOfSourceFile = _playlist.Count;
            _columnsCount = _playlist[0].Length;
            _columnNames = _playlist[0];
        }

        public void ConvertSourceToOutput(BackgroundWorker parsingInSeparateThread)
        {
            GetProgramsFromDb();
            for (var parsingRow = 1; parsingRow < RowsCountOfSourceFile; parsingRow++)
            {
                if (parsingInSeparateThread.CancellationPending)
                {
                    throw new Exception("Parsing in canceled by User");
                }
                parsingInSeparateThread.ReportProgress(parsingRow);

                _toNextRowWithoutSave = false;
                FillNewSourceProgramFrom(parsingRow);
                if (_toNextRowWithoutSave) continue;

                AddToOutputPrograms(_newSourceProgram);
            }
        }

        private void GetProgramsFromDb()
        {
            IFromDb fromDb = new FromDb();
            fromDb.FillPrograms();
            _programs = fromDb.GetPrograms();
        }

        private void FillNewSourceProgramFrom(int parsingRow)
        {
            _newSourceProgram = new SourceProgram();
            for (var parsingColumn = 0; parsingColumn < _columnsCount; parsingColumn++)
            {
                switch (_columnNames[parsingColumn])
                {
                    case "Start Time":
                        var sourceStartTime = _playlist[parsingRow][parsingColumn];
                        if (sourceStartTime.Contains("(1)"))
                        {
                            throw new Exception("The Start Time column of source file can't contains next day symblol \"(1)\"");
                        }
                        _newSourceProgram.SourceStartTime = TimeSpanParse(sourceStartTime);
                        break;
                    case "Title":
                        _newSourceProgram.SourceTitle = _playlist[parsingRow][parsingColumn];
                        break;
                    case "Duration":
                        var sourceDuration = _playlist[parsingRow][parsingColumn];
                        var duration = TimeSpanParse(sourceDuration);
                        if (duration.Equals(0)) _toNextRowWithoutSave = true;
                        _newSourceProgram.SourceDuration = duration;
                        break;
                    default:
                        continue;
                }
            }
        }

        private static double TimeSpanParse(string field)
        {
            TimeSpan t;
            const int lengthWithFrames = 11;
            if (field.Length == lengthWithFrames)
            {
                var trimIndex = field.LastIndexOf(":", StringComparison.Ordinal);
                var startTimeTrimed = field.Remove(trimIndex);
                t = TimeSpan.Parse(startTimeTrimed);
            }
            else
            {
                t = TimeSpan.Parse(field);
            }
            return t.TotalDays;
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
                throw new Exception(
                    "Error while merging the \"" + program.Title + "\".\n " +
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

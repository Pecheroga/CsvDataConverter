using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DataSource.Db;
using DataSource.Base;

namespace Converter.Mvvm.Model
{
    internal sealed class Converter
    {
        private readonly List<string[]> _convertibleArray;
        private bool _toNextRowWithoutSave;
        private ObservableCollection<Program> _programs;
        private SourceProgram _newSourceProgram;
        private OutputProgram _outputProgram;
        private readonly ObservableCollection<OutputProgram> _outputPrograms = new ObservableCollection<OutputProgram>();

        public Converter(List<string[]> convertibleArray)
        {
            _convertibleArray = convertibleArray;
        }

        public void ConvertSourceToOutputProgram(Worker worker = null)
        {
            GetProgramsFromDb();
            for (var parsingRow = 1; parsingRow < _convertibleArray.Count; parsingRow++)
            {
                if (worker != null) worker.OnProgressChanged(parsingRow, _convertibleArray.Count);

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
            for (var parsingColumn = 0; parsingColumn < _convertibleArray[0].Length; parsingColumn++)
            {
                switch (_convertibleArray[0][parsingColumn])
                {
                    case "Start Time":
                        var sourceStartTime = _convertibleArray[parsingRow][parsingColumn];
                        if (sourceStartTime.Contains("(1)"))
                        {
                            throw new Exception("The Start Time column of source file can't contains next day symblol \"(1)\"");
                        }
                        _newSourceProgram.SourceStartTime = TryParseToDoubleTotalDays(sourceStartTime);
                        break;
                    case "Title":
                        _newSourceProgram.SourceTitle = _convertibleArray[parsingRow][parsingColumn];
                        break;
                    case "Duration":
                        var sourceDuration = _convertibleArray[parsingRow][parsingColumn];
                        var duration = TryParseToDoubleTotalDays(sourceDuration);
                        if (duration.Equals(0)) _toNextRowWithoutSave = true;
                        _newSourceProgram.SourceDuration = duration;
                        break;
                    default:
                        continue;
                }
            }
        }

        private static double TryParseToDoubleTotalDays(string field)
        {
            try
            {
                return ParseToDoubleTotalDays(field);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new Exception("Can't trim frames in StartTime column");
            }
            catch (FormatException)
            {
                throw new Exception("Wrong data format in StartTime column");
            }
            catch (Exception)
            {
                throw new Exception("Can't parse StarTime column");
            }
        }

        private static double ParseToDoubleTotalDays(string field)
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

﻿using System.Collections.ObjectModel;
using System.IO;
using DataSource.Structure;
using Microsoft.Office.Interop.Excel;

namespace Converter.Mvvm.Model
{
    internal sealed class Save
    {
        private readonly string _nameOfChosenFileWithoutExtension;
        private readonly string _nameOfOutputFile;
        private readonly Worker _worker;
        private ObservableCollection<OutputProgram> _outputPrograms;
        private readonly ExcelAppBlank _outputExcelApp = new ExcelAppBlank();
        private readonly Worksheet _outputFirstWorksheet;
        
        public string NameOfOutputFile
        {
            get { return _nameOfOutputFile; }
        }

        public Save(string nameOfChosenFile, Worker worker = null)
        {
            _nameOfChosenFileWithoutExtension = Path.GetFileNameWithoutExtension(nameOfChosenFile);
            _nameOfOutputFile = Path.GetDirectoryName(nameOfChosenFile) + "\\Витяг_" + _nameOfChosenFileWithoutExtension + ".xlsx";
            _worker = worker;

            _outputFirstWorksheet = _outputExcelApp.GetFirstWorksheet();
            _outputFirstWorksheet.Name = "Витяг_" + _nameOfChosenFileWithoutExtension;
        }

        public void SaveOutputFile(ObservableCollection<OutputProgram> outputPrograms)
        {
            _outputPrograms = outputPrograms;
            FillCellsWithData();
            SetColorOfColumnNamesCells();
            SetFontSize();
            AutoFitCells();
            _outputFirstWorksheet.SaveAs(_nameOfOutputFile);
            _outputExcelApp.QuitExcelApp();
        }

        private void FillCellsWithData()
        {
            const int columnCount = 7;
            var headerToCells = new string[1, columnCount];
            var columnNamesToCells = new string[1, columnCount];
            var outputProgramsToCells = new string[_outputPrograms.Count, columnCount];

            headerToCells[0, 2] = "Витяг з журналу обліку передач \n " +
                                  "за " + _nameOfChosenFileWithoutExtension + " ТОВ \"Магнолія - ТВ\"";
            columnNamesToCells[0, 0] = "Початок";
            columnNamesToCells[0, 1] = "Кінець";
            columnNamesToCells[0, 2] = "Назва";
            columnNamesToCells[0, 3] = "Мова";
            columnNamesToCells[0, 4] = "Автор";
            columnNamesToCells[0, 5] = "Ведучий";
            columnNamesToCells[0, 6] = "Тема";

            for (var row = 0; row < _outputPrograms.Count; row++)
            {
                if (_worker != null) _worker.OnProgressChanged(row, _outputPrograms.Count - 1);
                for (var column = 0; column < columnCount; column++)
                {
                    switch (column)
                    {
                        case 0:
                            outputProgramsToCells[row, column] = _outputPrograms[row].StartTime;
                            break;
                        case 1:
                            outputProgramsToCells[row, column] = _outputPrograms[row].EndTime;
                            break;
                        case 2:
                            outputProgramsToCells[row, column] = _outputPrograms[row].Title;
                            break;
                        case 3:
                            outputProgramsToCells[row, column] = _outputPrograms[row].Lang;
                            break;
                        case 4:
                            outputProgramsToCells[row, column] = _outputPrograms[row].Author;
                            break;
                        case 5:
                            outputProgramsToCells[row, column] = _outputPrograms[row].Presenter;
                            break;
                        case 6:
                            outputProgramsToCells[row, column] = _outputPrograms[row].Subject;
                            break;
                        default:
                            continue;
                    }
                }
            }

            var range = _outputExcelApp.GetFirstWorksheetRange("A1", "G1");
            range.Value2 = headerToCells;
            range = _outputExcelApp.GetFirstWorksheetRange("A2", "G2");
            range.Value2 = columnNamesToCells;
            var lastRowOfData = (_outputPrograms.Count + 2).ToString();
            range = _outputExcelApp.GetFirstWorksheetRange("A3", "G" + lastRowOfData);
            range.Value2 = outputProgramsToCells;
        }

        private void SetColorOfColumnNamesCells()
        {
            var range = _outputExcelApp.GetFirstWorksheetRange("A2", "G2");
            var interior = range.Interior;
            interior.Color = XlRgbColor.rgbLightGray;
        }

        private void SetFontSize()
        {
            var range = _outputFirstWorksheet.UsedRange;
            var font = range.Font;
            font.Size = 8;
        }

        private void AutoFitCells()
        {
            var range = _outputFirstWorksheet.UsedRange;
            var columns = range.Columns;
            columns.AutoFit();
        }
    }
}

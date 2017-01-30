﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic.FileIO;

namespace Converter.Mvvm.Model
{
    internal sealed class Parser
    {
        private readonly string _file;
        private readonly string _delimiter;
        private readonly int _encoding;
        private readonly bool _hasQuotes;
        private readonly List<string[]> _outputArray = new List<string[]>();

        public string[] ColumnNames
        {
            get { return _outputArray[0]; }
        }

        public int RowsCount { get; private set; }
        public int ColumnsCount { get; private set; }

        public Parser(
            string file, 
            string delimiter = ";", 
            int encoding = 1251, 
            bool hasQuotes = false)
        {
            _file = file;
            _delimiter = delimiter;
            _encoding = encoding;
            _hasQuotes = hasQuotes;
        }

        public void Start(Worker worker = null)
        {
            SetOutputArrayRowsAndColumns();
            using (var csv = new TextFieldParser(_file, Encoding.GetEncoding(_encoding)))
            {
                csv.Delimiters = new[] { _delimiter };
                csv.TextFieldType = FieldType.Delimited;
                csv.HasFieldsEnclosedInQuotes = _hasQuotes;
                
                var line = new List<string>();
                var parsingRow = 1;
                while (!csv.EndOfData)
                {
                    var fields = csv.ReadFields();
                    if (fields == null) continue;
                    line.AddRange(
                        from field in fields
                        where field != null
                        select field.Trim('/', '"'));
                    _outputArray.Add(line.ToArray());
                    line.Clear();
                    if (worker != null) worker.OnProgressChanged(parsingRow++, RowsCount);
                }
            }
        }

        private void SetOutputArrayRowsAndColumns()
        {
            var notParsedArray = File.ReadAllLines(_file);
            RowsCount = notParsedArray.Length;
            ColumnsCount = notParsedArray[0].Length;
        }
        
        public List<string[]> GetOutputArray()
        {
            return _outputArray;
        }
    }
}

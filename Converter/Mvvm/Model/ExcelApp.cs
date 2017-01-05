using Converter.Helpers;
using Microsoft.Office.Interop.Excel;
using Application = Microsoft.Office.Interop.Excel.Application;

namespace Converter.Mvvm.Model
{
    internal class ExcelAppBase
    {
        protected Application ExcelApp;
        protected Workbooks Workbooks;
        protected Workbook Workbook;
        private Worksheet _firstWorksheet;

        public ExcelAppBase()
        {
            CreateNewExcelApp();
            Workbooks = ExcelApp.Workbooks;
        }

        private void CreateNewExcelApp()
        {
            ExcelApp = new Application
            {
                DisplayAlerts = false
            };
            if (ExcelApp != null) return;
            throw new ExcelException(this, "Could not create Excel Application");
        }

        protected virtual void AddWorkbook()
        {
            if (Workbook != null) return;
            throw new ExcelException(this, "Could not create Workbook");
        }

        protected void SetFirstWorksheet()
        {
            var worksheets = Workbook.Worksheets;
            _firstWorksheet = (Worksheet)worksheets.Item[1];
            if (_firstWorksheet != null) return;
            throw new ExcelException(this, "Could not create Worksheet");
        }

        public Worksheet GetFirstWorksheet()
        {
            return _firstWorksheet;
        }

        public Range GetFirstWorksheetRange(string startCell, string endCell)
        {
            var selectedRange = _firstWorksheet.Range[startCell, endCell];
            if (selectedRange != null) return selectedRange;
            throw new ExcelException(this, "Could not create Range");
        }

        public void QuitExcelApp()
        {
            Workbook.Close();
            ExcelApp.Quit();
        }
    }

    internal sealed class ExcelAppBlank : ExcelAppBase
    {
        public ExcelAppBlank()
        {
            AddWorkbook();
            SetFirstWorksheet();
        }

        protected override void AddWorkbook()
        {
            Workbook = Workbooks.Add();
            base.AddWorkbook();
        }
    }

    internal sealed class ExcelAppFromFile : ExcelAppBase
    {
        private readonly string _nameOfChosenFile;

        public ExcelAppFromFile(string nameOfChosenFile)
        {
            _nameOfChosenFile = nameOfChosenFile;
            AddWorkbook();
            SetFirstWorksheet();
        }

        protected override void AddWorkbook()
        {
            Workbook = Workbooks.Open(
                _nameOfChosenFile, // File name
                0, // UpdateLink
                true, // ReadOnly
                2, // Format
                "", // Password
                "", // WriteResPassword
                true, // IgnorReadOnlyRecommended
                XlPlatform.xlWindows, // Origin
                false, // Delimiter    
                false, // Editable
                false, // Notify
                0, // Converter
                true, // AddToMru
                1, // Local
                0 // CoruptLoad
                );
            base.AddWorkbook();
        }
    }
}

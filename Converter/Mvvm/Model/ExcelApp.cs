using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Converter.Helpers;
using Microsoft.Office.Interop.Excel;
using Application = Microsoft.Office.Interop.Excel.Application;

namespace Converter.Mvvm.Model
{
    internal class ExcelAppBase
    {
        protected Application ExcelApp;
        protected Workbook Workbook;
        private Worksheet _firstWorksheet;
        private int _instanceOfExcelApp;

        public ExcelAppBase()
        {
            CreateNewExcelApp();
            _instanceOfExcelApp = 1;
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
            _firstWorksheet = (Worksheet)Workbook.Worksheets.Item[1];
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
            if (_instanceOfExcelApp == 0) return;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Marshal.FinalReleaseComObject(_firstWorksheet);
            Workbook.Close(Type.Missing, Type.Missing, Type.Missing);
            Marshal.FinalReleaseComObject(Workbook);
            QuitExcelAppAsync();
        }

        private async void QuitExcelAppAsync()
        {
            await QuitExcelAppTask();
            _instanceOfExcelApp = Marshal.FinalReleaseComObject(ExcelApp);
        }

        private Task QuitExcelAppTask()
        {
            return Task.Run(() =>
            {
                ExcelApp.Quit();
            });
        }
    }

    internal class ExcelAppBlank : ExcelAppBase
    {
        public ExcelAppBlank()
        {
            AddWorkbook();
            SetFirstWorksheet();
        }

        protected sealed override void AddWorkbook()
        {
            Workbook = ExcelApp.Workbooks.Add();
            base.AddWorkbook();
        }
    }

    internal class ExcelAppFromFile : ExcelAppBase
    {
        private readonly string _nameOfChosenFile;

        public ExcelAppFromFile(string nameOfChosenFile)
        {
            _nameOfChosenFile = nameOfChosenFile;
            AddWorkbook();
            SetFirstWorksheet();
        }

        protected sealed override void AddWorkbook()
        {
            Workbook = ExcelApp.Workbooks.Open(
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

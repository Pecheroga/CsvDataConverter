using System;
using Converter.Mvvm.Model;

namespace Converter.Helpers
{
    internal sealed class ExcelException : Exception
    {
        public ExcelException(ExcelAppBase excelAppBase, string exceptionMessage)
            : base(exceptionMessage)
        {
            excelAppBase.QuitExcelApp();
        }
    }
}

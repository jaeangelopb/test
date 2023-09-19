using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITPFunctions
{
    public class TACUploadFile
    {

        public string Type { get; set; }

        public string Adjustments { get; set; }

        public string? TaxNoteCode { get; set; }

        public string? TaxNoteReportingCategories { get; set; }

        public string? FormCSTICategory { get; set; }

    }
}

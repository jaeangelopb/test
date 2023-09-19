using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITPFunctions
{
    public class ETRAdjustments
    {
        public int? ETRId { get; set; }
        public int EntityId { get; set; }

        public string Period { get; set; }

        public int? TNCId { get; set; }

        public string Description { get; set; }

        public string? Category { get; set; }

        public double? Amount { get; set; }

        public string? Comments { get; set; }
        public string? Process { get; set; }
    }
}

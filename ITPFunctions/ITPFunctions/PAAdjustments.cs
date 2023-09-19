using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITPFunctions
{
    public class PAAdjustments
    {
        public int? PAId { get; set; }
        public int EntityId { get; set; }

        public string Period { get; set; }

        public string YearIncurred { get; set; }

        public double? OpeningBalanceAdjustment { get; set; }

        public double? Additions { get; set; }

        public double? Disposals { get; set; }

        public double? Other { get; set; }

        public string? Comments { get; set; }
        public string? Process { get; set; }
    }
}

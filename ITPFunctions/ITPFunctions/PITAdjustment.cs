using System;
using System.Collections.Generic;
using System.Text;

namespace ITPFunctions
{
    public class PITAdjustment
    {
        public int? PITId { get; set; }
        public int EntityId { get; set; }

        public string Period { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        public double? Amount { get; set; }

        public string? Comments { get; set; }
        public string? Process { get; set; }

    }
}

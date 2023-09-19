using System;
using System.Collections.Generic;
using System.Text;

namespace ITPFunctions
{
    public class TEASummary
    {

        public int? TEAId { get; set; }

        public int EntityId { get; set; }

        public string Period { get; set; }

        public string Description { get; set; }


        public double? CurrentTaxExpense { get; set; }

        public double? DeferredTaxExpense { get; set; }

        public double? DeferredTaxAsset { get; set; }

        public double? DeferredTaxLiability { get; set; }

        public double? TaxPayable { get; set; }

        public double? Equity { get; set; }
        public double? Intercompany { get; set; }

        public double? Other { get; set; }

        public string? Comments { get; set; }
        public string? Process { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public string ModifiedOn { get; set; }
    }
}

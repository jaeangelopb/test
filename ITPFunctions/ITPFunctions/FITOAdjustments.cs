using System;
using System.Collections.Generic;
using System.Text;

namespace ITPFunctions
{
    public class FITOAdjustments
    {

        public int? FITOId { get; set; }

        public int EntityId { get; set; }

        public string Period { get; set; }

        public string YearIncurred { get; set; }

        public string YearOfExpiry { get; set; }

        public string Description { get; set; }

        public double? OpeningBalanceRollForward { get; set; }

        public double? OpeningBalanceAdjustment { get; set; }

        public double? CurrentYearTaxCredits { get; set; }

        public double? TaxCreditsUtilised { get; set; }

        public double? TaxCreditsExpired { get; set; }

        public double? OtherAdjustment { get; set; }

        public string? Comments { get; set; }
        public string? Process { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public string ModifiedOn { get; set; }
    }
}

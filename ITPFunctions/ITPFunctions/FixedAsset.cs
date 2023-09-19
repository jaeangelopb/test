using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITPFunctions
{
    public class FixedAsset
    {
        public int? FARId_Accounting { get; set; }
        public int? FARId_Tax { get; set; }
        public int EntityId { get; set; }

        public int? TACId { get; set; }

        public int? TaxSensitivityId { get; set; }

        public string Period { get; set; }

        public string AssetClass { get; set; }

        public string AssetDescription { get; set; }
   
        [JsonProperty(PropertyName = "Accounting_OpeningBalanceAdjustment")]
        public double? OpeningBalanceAdjustment { get; set; }

        [JsonProperty(PropertyName = "Accounting_PriorYearAdjustments")]
        public double? PriorYearAdjustments { get; set; }

        [JsonProperty(PropertyName = "Accounting_Additions")]
        public double? Additions { get; set; }

        [JsonProperty(PropertyName = "Accounting_Depreciation")]
        public double? Depreciation { get; set; }

        [JsonProperty(PropertyName = "Accounting_Disposals")]
        public double? Disposals { get; set; }

        [JsonProperty(PropertyName = "Accounting_IntercompanyTransfers")]
        public double? IntercompanyTransfers { get; set; }

        [JsonProperty(PropertyName = "Accounting_Other")]
        public double? Other { get; set; }

        [JsonProperty(PropertyName = "Accounting_ClosingBalance")]
        public double? ClosingBalance { get; set; }

        [JsonProperty(PropertyName = "Accounting_Proceeds")]
        public double? Proceeds { get; set; }

        [JsonProperty(PropertyName = "Tax_OpeningBalanceAdjustment")]
        public double? TaxOpeningBalanceAdjustment { get; set; }

        [JsonProperty(PropertyName = "Tax_PriorYearAdjustments")]
        public double? TaxPriorYearAdjustments { get; set; }

        [JsonProperty(PropertyName = "Tax_Additions")]
        public double? TaxAdditions { get; set; }

        [JsonProperty(PropertyName = "Tax_Depreciation")]
        public double? TaxDepreciation { get; set; }

        [JsonProperty(PropertyName = "Tax_Disposals")]
        public double? TaxDisposals { get; set; }

        [JsonProperty(PropertyName = "Tax_IntercompanyTransfers")]
        public double? TaxIntercompanyTransfers { get; set; }

        [JsonProperty(PropertyName = "Tax_Other")]
        public double? TaxOther { get; set; }

        [JsonProperty(PropertyName = "Tax_ClosingBalance")]
        public double? TaxClosingBalance { get; set; }

        [JsonProperty(PropertyName = "Tax_Proceeds")]
        public double? TaxProceeds { get; set; }

        public bool IsAccounting { get; set; }

        public bool? IsTangible { get; set; }
        public string Comments { get; set; }
        public string? Process { get; set; }


        public string CreatedBy { get; set; }

        public string CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public string ModifiedOn { get; set; }

    }
}

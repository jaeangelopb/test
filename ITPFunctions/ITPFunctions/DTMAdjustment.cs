using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ITPFunctions
{

    public class DTMAdjustment
    {
        public int? DTMAdjustmentId_Acct { get; set; }

        public int? DTMAdjustmentId_Tax { get; set; }

        public int? TACId { get; set; }

        public int? TNCId { get; set; }
        public int? AccountId { get; set; }
        public int EntityId { get; set; }
        public string Period { get; set; }

        [JsonProperty(PropertyName = "Accounting_Opening Balance Adjustment")]        
        public double? OpeningBalanceAdjustment { get; set; }

        [JsonProperty(PropertyName = "Accounting_Under/(Over) Provision Prior Year")]        
        public double? UnderProvisionPriorYear { get; set; }

        [JsonProperty(PropertyName = "Accounting_Transfers")]        
        public double? Transfers { get; set; }

        [JsonProperty(PropertyName = "Accounting_Other Movements")]        
        public double? OtherMovements { get; set; }

        [JsonProperty(PropertyName = "Accounting_Business Acquisition")]        
        public double? BusinessAcquistion { get; set; }

        [JsonProperty(PropertyName = "Accounting_Business Disposal")]        
        public double? BusinessDisposal { get; set; }

        [JsonProperty(PropertyName = "Accounting_Equity")]        
        public double? Equity { get; set; }

        [JsonProperty(PropertyName = "Tax_Opening Balance Adjustment")]        
        public double? TaxOpeningBalanceAdjustment { get; set; }

        [JsonProperty(PropertyName = "Tax_Under/(Over) Provision Prior Year")]        
        public double? TaxUnderProvisionPriorYear { get; set; }

        [JsonProperty(PropertyName = "Tax_Transfers")]        
        public double? TaxTransfers { get; set; }

        [JsonProperty(PropertyName = "Tax_Other Movements")]        
        public double? TaxOtherMovements { get; set; }

        [JsonProperty(PropertyName = "Tax_Business Acquisition")]        
        public double? TaxBusinessAcquistion { get; set; }

        [JsonProperty(PropertyName = "Tax_Business Disposal")]        
        public double? TaxBusinessDisposal { get; set; }

        [JsonProperty(PropertyName = "Tax_Equity")]        
        public double? TaxEquity { get; set; }

        [JsonProperty(PropertyName = "Tax_Closing Balance")]
        public double? TaxClosingBalance { get; set; }

        public bool IsAccounting { get; set; }

        public string Comments { get; set; }
        public string? Process { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public string ModifiedOn { get; set; }


    }
}

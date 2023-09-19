using Microsoft.WindowsAzure.Storage.Blob.Protocol;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITPFunctions
{
    public class TrialBalanceAdjustment
    {
        public int TrialBalanceId { get; set; }
        public int EntityId { get; set; }
        public string EntityName{ get; set; }
        public string Period { get; set; }
        public string Account { get; set; }
        public string Jurisdiction { get; set; }
        public string Year { get; set; }
        public string Currency { get; set; }
        public double? Amount { get; set; }
        public double? Adjustment { get; set; }
        [JsonProperty(PropertyName = "Adjusted Amount")]
        public double? AdjustedAmount { get; set; }
        [JsonProperty(PropertyName = "Data Source")]
        public string DataSource { get; set; }
        public string Schedule { get; set; }
        [JsonProperty(PropertyName = "Tax Sensitivity")]
        public string TaxSensitivity { get; set; }

        [JsonProperty(PropertyName = "Tax Adjustment")]
        public string TaxAdjustment { get; set; }

        [JsonProperty(PropertyName = "Tax Return Disclosure")]
        public string FormCDisclosure { get; set; }
        public string Comments { get; set; }
    }
}

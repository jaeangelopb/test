using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITPFunctions
{
   public class Adjustment
   {
        public int RowId { get; set; }

        [JsonProperty(PropertyName = "Account Number")]
        public string AccountNumber { get; set; }

        [JsonProperty(PropertyName = "Account Description")]
        public string AccountDescription { get; set; }

        
        public string Jurisdiction { get; set; }

        
        public string Entity { get; set; }

        
        public string Group { get; set; }

        [JsonProperty(PropertyName = "Reporting Year")]
        public string ReportingYear { get; set; }

        [JsonProperty(PropertyName = "Reporting Month")]
        public string ReportingMonth { get; set; }

        [JsonProperty(PropertyName = "Reporting Quarter")]
        public string ReportingQuarter { get; set; }

        public string Interval { get; set; }


        [JsonProperty(PropertyName = "Base Value")]
        public string BaseValue { get; set; }

        [JsonProperty(PropertyName = "Adjustment Value")]
        public string AdjustmentValue { get; set; }

        [JsonProperty(PropertyName = "Total Value")]
        public string TotalValue { get; set; }

        public string Comments { get; set; }

        
        public string AdjustmentId { get; set; }

        




    }
}

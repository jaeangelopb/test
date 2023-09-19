using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITPFunctions
{
    public class STIAdjustment
    {

        public int? STIAdjustmentId { get; set; }

        public int? OPAId { get; set; }
        public int? AccountId { get; set; }
        public int? TACId { get; set; }

        public int? TNCId { get; set; }

        public int? DisclosuresId { get; set; }

        public int? EntityId { get; set; }

        public string Period { get; set; }

        [JsonProperty(PropertyName = "Tax Adjustment Description")]
        public string Description { get; set; }

        [JsonProperty(PropertyName = "Adjustment Amount")]
        public double? Amount { get; set; }

        public string? Comments { get; set; }
        public string? Process { get; set; }

        public string? CreatedBy { get; set; }

        public string? CreatedOn { get; set; }

        public string? ModifiedBy { get; set; }

        public string? ModifiedOn { get; set; }
    }
}

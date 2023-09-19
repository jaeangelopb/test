using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITPFunctions
{
    public class TaxNoteAdjustmentsPerm
    {
        public int? TNAdjId_Gross { get; set; }

        public int? TNAdjId_Net { get; set; }

        public string GroupingName { get; set; }

        public int ParentEntityId { get; set; }

        public string Period { get; set; }

        public int? TNCId { get; set; }

        public string Description { get; set; }

        [JsonProperty(PropertyName = "gross_Adjustment")]
        public double? GrossAdjustment { get; set; }

        [JsonProperty(PropertyName = "net_Adjustment")]
        public double? NetAdjustment { get; set; }

        public string? Comments { get; set; }
        public string? Process { get; set; }

        public string? CreatedBy { get; set; }

        public string? CreatedOn { get; set; }

        public string? ModifiedBy { get; set; }

        public string? ModifiedOn { get; set; }
    }
}
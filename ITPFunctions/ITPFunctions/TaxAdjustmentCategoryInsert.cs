using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITPFunctions
{
   public class TaxAdjustmentCategoryInsert
    {
        [JsonProperty(PropertyName = "MappingId")]
        public int? TACId { get; set; }

        [JsonProperty(PropertyName = "TNCID")]
        public int? TNCId { get; set; }
        public int? DisclosuresId { get; set; }


        [JsonProperty(PropertyName = "Adjustments")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "Type")]
        public string AdjustmentCategory { get; set; }

        [JsonProperty(PropertyName = "MappingName")]
        public string Source { get; set; }
        public bool Default { get; set; }
        public string Sorting { get; set; }


    }
}

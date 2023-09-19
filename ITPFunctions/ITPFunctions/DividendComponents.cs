using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITPFunctions
{
   public class DividendComponents
    {
        public int? DCId { get; set; }
        public int EntityId { get; set; }

        public string Period { get; set; }

        public string? Date { get; set; }

        [JsonProperty(PropertyName = "Total dividend to be paid")]
        public double? TotalDividendToBePaid { get; set; }
        public string? Process { get; set; }
    }
}

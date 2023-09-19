using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITPFunctions.Models
{
    public class Jurisdiction
    {
        public int? JurisdictionId { get; set; }

        [JsonProperty("Jurisdiction")]
        public string JurisdictionName { get; set; }
        public double TaxRate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITPFunctions.Models
{
    public class Entity
    {

        public int? EntityId { get; set; }

        public int JurisdictionId { get; set; }
        public int ReportingCurrencyId { get; set; }
        public string EntityCode { get; set; }
        public string EntityName { get; set; }
        [JsonProperty("LocalEntityType")]
        public string EntityType { get; set; }
        public string TFN { get; set; }
        public string ABN { get; set; }
        public bool Active { get; set; }
        public DateTime? SetUpDate { get; set; }
        public DateTime? WoundUpDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITPFunctions.Models
{
    public class Calculation
    {
        public int? CalculationId { get; set; }
        public int EntityId { get; set; }
        public string Period { get; set; }
        [JsonProperty("Entity Volume")]
        public string EntityVolume { get; set; }
        public string Status { get; set; }
        [JsonProperty("Ingestion Type")]
        public string IngestionType { get; set; }
        public string Process { get; set; }
        [JsonProperty("Trial Balance Filename")]
        public string TrialBalanceFileName { get; set; }

        public string? TrialBalanceHeaderId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}

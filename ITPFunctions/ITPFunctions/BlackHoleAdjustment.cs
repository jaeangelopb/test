using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ITPFunctions
{
    public class BlackHoleAdjustment
    {
        public int? BHAdjustmentId { get; set; }

        public int? TACId { get; set; }

        public int? TNCId { get; set; }

        public int EntityId { get; set; }

        public string Project { get; set; }

        [JsonProperty(PropertyName = "Description of Costs")]
        public string Description { get; set; }

        public string Period { get; set; }

        [JsonProperty(PropertyName = "Year Incurred")]
        public string YearIncurred { get; set; }

        [JsonProperty(PropertyName = "Initial Cost")]
        public double? InitialCost { get; set; }

        [JsonProperty(PropertyName = "s40-880")]
        [Column("s40-880")]
        public double? S40880 { get; set; }

        public double? Deductible { get; set; }

        [JsonProperty(PropertyName = "Non Deductible")]
        public double? NonDeductible { get; set; }

        [JsonProperty(PropertyName = "40-880 Initial Costs")]
        [Column("40-880_InitialCosts")]
        public double? InitialCosts_40880 { get; set; }

        [JsonProperty(PropertyName = "Opening Balance Adjustment")]
        public double? OpeningBalanceAdjustment { get; set; }

        public string Comments { get; set; }
        public string? Process { get; set; }
        public string CreatedBy { get; set; }

        public string CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public string ModifiedOn { get; set; }
    }
}

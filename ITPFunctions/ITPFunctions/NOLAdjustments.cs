using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ITPFunctions
{
    public class NOLAdjustments
    {
        public int? TACId { get; set; }
        public int? TNCId { get; set; }
        public int EntityId { get; set; }
        public string Period { get; set; }
        public double? TaxRate { get; set; }
        public string YearIncurred { get; set; }
        public string YearOfExpiry { get; set; }

        [JsonProperty("Tax Loss Amount")]
        public double? TaxLossAmount { get; set; }
        public int? Recognised_NOLId { get; set; }

        [JsonProperty("Recognised_Opening Balance Adjustment")]
        public double? Recognised_OpeningBalanceAdjustment { get; set; }

        [JsonProperty("Recognised_Losses Previously Unrecognised Now Recognised")]
        public double? Recognised_LossesPreviouslyUnrecognisedNowRecognised { get; set; }

        [JsonProperty("Recognised_Current Year Losses")]
        public double? Recognised_CurrentYearLosses { get; set; }

        [JsonProperty("Recognised_Losses Utilised In / (Out)")]
        public double? Recognised_LossesUtilisedInOut { get; set; }

        [JsonProperty("Recognised_Losses Expired In / (Out)")]
        public double? Recognised_LossesExpiredInOut { get; set; }

        [JsonProperty("Recognised_Unrecognised Losses / Other Adjustment")]
        public double? Recognised_UnrecognisedLossesOtherAdjustment { get; set; }

        [JsonProperty("Recognised_Loss Transfers In / (Out)")]
        public double? Recognised_LossTransfersInOut { get; set; }

        public string? Entity { get; set; }

        public string? Comments { get; set; }
        public int? Unrecognised_NOLId { get; set; }

        [JsonProperty("Unrecognised_Opening Balance Adjustment")]
        public double? Unrecognised_OpeningBalanceAdjustment { get; set; }

        [JsonProperty("Unrecognised_Losses Previously Unrecognised Now Recognised")]
        public double? Unrecognised_LossesPreviouslyUnrecognisedNowRecognised { get; set; }

        [JsonProperty("Unrecognised_Current Year Losses")]
        public double? Unrecognised_CurrentYearLosses { get; set; }

        [JsonProperty("Unrecognised_Losses Utilised In / (Out)")]
        public double? Unrecognised_LossesUtilisedInOut { get; set; }

        [JsonProperty("Unrecognised_Losses Expired In / (Out)")]
        public double? Unrecognised_LossesExpiredInOut { get; set; }

        [JsonProperty("Unrecognised_Loss Transfers In / (Out)")]
        public double? Unrecognised_LossTransfersInOut { get; set; }
        public string? Process { get; set; }

    }

}

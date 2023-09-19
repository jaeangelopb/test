using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITPFunctions
{
    public class ChartOfAccount
    {

        public int? AccountId { get; set; }

        public int? AccountTypeId { get; set; }

        public int? TaxSensitivityId { get; set; }

        [JsonProperty(PropertyName = "FormCDisclosureId")]
        public int? DisclosuresId { get; set; }

        [JsonProperty(PropertyName = "AdjustmentCategoryId")]
        public int? TACId { get; set; }
        public int? TNCId { get; set; }
        
        [JsonProperty(PropertyName = "Account")]
        public string AccountCode { get; set; }

        public string AccountName { get; set; }

        public string? Risk { get; set; }

        public string? AccountingTreatment { get; set; }

        public string? TaxTreatment { get; set; }

        public string? Comments { get; set; }


        [JsonProperty(PropertyName = "TBMappingName")]
        public string? VersionName { get; set; }
        public string? SourceSystem { get; set; }

        public string CreatedBy { get; set; }

        public string CreatedOn { get; set; }

        public string ModifiedBy { get; set; }

        public string ModifiedOn { get; set; }
    }
}

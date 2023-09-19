using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITPFunctions
{
   public class TaxAdjustmentCategory
    {
        public int? MappingId { get; set; }
        public string MappingName { get; set; }
        public bool Default { get; set; }
        public string ModifiedBy { get; set; }

        public string ModifiedOn { get; set; }

        public string OldMappingName { get; set; }


    }
}

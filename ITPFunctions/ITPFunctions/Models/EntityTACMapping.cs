using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITPFunctions.Models
{
    public class EntityTACMapping
    {

        public int? EntityTACMappingId { get; set; }
        public int EntityId { get; set; }

        [JsonProperty(PropertyName = "MappingName")]
        public string TACName { get; set; }
        public string Period { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}

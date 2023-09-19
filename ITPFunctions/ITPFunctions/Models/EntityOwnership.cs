using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITPFunctions.Models
{
    public class EntityOwnership
    {

        public int? EntityOwnershipId { get; set; }
        [JsonProperty("EntityGroup")]
        public string GroupingName { get; set; }
        public int ParentEntityId { get; set; }
        public int SubEntityId { get; set; }
        public double? OwnershipPercentage { get; set; }
        public DateTime? EntryDate { get; set; }
        public DateTime? ExitDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}

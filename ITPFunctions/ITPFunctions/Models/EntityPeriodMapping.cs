using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITPFunctions.Models
{
    public class EntityPeriodMapping
    {

        public int? EntityPeriodId { get; set; }
        public int EntityId { get; set; }
        public int YearId { get; set; }
        public int MonthStartId { get; set; }
        public int MonthEndId { get; set; }
        public string Frequency { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}

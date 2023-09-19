using Newtonsoft.Json;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITPFunctions
{
    public class COAEntityMapping
    {
        public int? MappingId { get; set; }
        public int EntityId { get; set; }
        public string EntityName { get; set; }


        [JsonProperty(PropertyName = "MappingName")]
        public string COAName { get; set; }

        public string Period { get; set; }

        public string OldMappingName { get; set; }



    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITPFunctions.Models
{
    public class SaveCopyCalculation
    {

        public int EntityId { get; set; }
        public string OldPeriod { get; set; }
        public string Period { get; set; }
        public string? FileName { get; set; }
        public string Process { get; set; }
    }
}

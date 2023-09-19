using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITPFunctions.Models
{
    internal class SaveTaxReturn
    {
        public int EntityID { get; set; }
        public string Period { get; set; }
        public string? FileName { get; set; }
        public string Process { get; set; }
    }
}

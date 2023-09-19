using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ITPFunctions
{
   public class ErrorMessage
    {
        public string ValidationType { get; set; }
        public string[] Validation { get; set; }

    }
}

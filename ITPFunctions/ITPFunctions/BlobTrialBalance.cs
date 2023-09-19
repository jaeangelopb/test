using Microsoft.WindowsAzure.Storage.Blob.Protocol;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ITPFunctions
{
    public class BlobTrialBalance
    {
        [MaxLength(50)]
        public string EntityCode { get; set; }
        [MaxLength(100)]
        public string EntityName { get; set; }
        [MaxLength(50)]
        public string AccountCode { get; set; }
        [MaxLength(100)]
        public string AccountDescription { get; set; }
        public string Amount { get; set; }
    }
}

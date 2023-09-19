using System.ComponentModel.DataAnnotations;

namespace EY.ITP.API.Models.Entities
{
    public class StoredProcParameter
    {
        [Key]
        public string Parameter_Name { get; set; }
        public string Parameter_Mode { get; set; }
        public string Data_Type { get; set; }
        public string Schema { get; set; }
        public string Type_Name { get; set; }
    }
}

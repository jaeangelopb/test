using System.ComponentModel.DataAnnotations;

namespace EY.ITP.API.Models.Entities
{
    public class TableTypeColumn
    {
        [Key]
        public string Column_Name { get; set; }
        public string Column_Type { get; set; }
        public bool Nullable { get; set; }
    }
}

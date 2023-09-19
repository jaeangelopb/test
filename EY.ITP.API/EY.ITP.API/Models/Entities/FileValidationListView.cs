using System.ComponentModel.DataAnnotations.Schema;

namespace EY.ITP.API.Models.Entities
{
    [Table("vw_FileValidation_List")]
    public class FileValidationListView
    {
        public int FileValidationId { get; set; }
        public string Filename { get; set; }
        public string Uploadedby { get; set; }
        public DateTime Uploadeddate { get; set; }
        public string Status { get; set; }
        public string FileType { get; set; }
        public string Modules { get; set; }
        public string EntityVolume { get; set; }
    }
}

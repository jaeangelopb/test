namespace EY.ITP.API.Models.Requests
{
    public class FileValidationSaveRequest
    {
        public int? GAAPId { get; set; }
        public string TaxYear { get; set; }
        public string Period { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
        public string TemplateName { get; set; }
        public string SourceSystem { get; set; }
        public string Modules { get; set; }
        public string EntityVolume { get; set; }
        public string Status { get; set; }
        public string Errors { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
    }
}

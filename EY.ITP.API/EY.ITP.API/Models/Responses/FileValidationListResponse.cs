namespace EY.ITP.API.Models.Responses
{
    public class FileValidationListResponse
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

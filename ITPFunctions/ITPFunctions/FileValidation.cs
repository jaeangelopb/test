using System;
using System.Collections.Generic;
using System.Text;

namespace ITPFunctions
{
    internal class FileValidation
    {
        public int? GAAPId { get; set; }

        public string Period { get; set; }

        public string FileType { get; set; }

        public string FileName { get; set; }

        public string TemplateName { get; set; }

        public string SourceSystem { get; set; }

        public string Modules { get; set; }

        public string EntityVolume { get; set; }

        public string Status { get; set; }

        public string Errors { get; set; }

    }
}

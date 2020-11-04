using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace T3.Areas.Templates.Data
{
    public class Template
    {
        public int Id { get; set; }

        public byte[] Content { get; set; }

        [Display(Name = "文件名")]
        public string UntrustedName { get; set; }

        [Display(Name = "备注")]
        public string Note { get; set; }

        [Display(Name = "大小 (bytes)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public long Size { get; set; }

        [Display(Name = "Uploaded (UTC)")]
        [DisplayFormat(DataFormatString = "{0:G}")]
        public DateTime UploadDT { get; set; }
    }

}



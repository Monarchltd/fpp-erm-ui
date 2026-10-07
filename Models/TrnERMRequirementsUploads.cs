

using System;

namespace fppErm.Models
{
    public class TrnERMRequirementsUploads
    {
        public double id { get; set; }
        public double? MasErmTender { get; set; }
        public double? MasErmRequirements { get; set; }
        public string tenderref { get; set; }
        public string heading { get; set; }
        public string upload_file { get; set; }
        public string cusr { get; set; }
        public DateTime cdte { get; set; }
        public string uusr { get; set; }
        public DateTime udte { get; set; }
        public int isActive { get; set; }
    }
}
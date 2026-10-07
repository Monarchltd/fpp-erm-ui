using System;

namespace DotNet8.WebApi.Factory.Model
{
    public class TrnERMTenderNotes_1
    {
        public double id { get; set; }
        public double? MasErmTender_id { get; set; }
        public string tenderref { get; set; }
        public string notes { get; set; }
        public string cusr { get; set; }
        public DateTime cdte { get; set; }
        public string uusr { get; set; }
        public DateTime udte { get; set; }
        public int isActive { get; set; }
    }
}


using System;

namespace fppErm.Models
{
    public class TrnERMRequirementsContractDuration
    {
        public double id { get; set; }
        public double? MasErmTender { get; set; }
        public double MasErmRequirements { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Duration { get; set; }
        public DateTime cdte { get; set; }
        public string cusr { get; set; }

        public string uusr { get; set; }
        public DateTime udte { get; set; }
        public int isActive { get; set; }
    }
}
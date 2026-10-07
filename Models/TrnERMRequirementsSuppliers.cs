
using System;

namespace fppErm.Models
{
    public class TrnERMRequirementsSuppliers
    {
        public double id { get; set; }
        public double? MasErmTender { get; set; }
        public double MasErmRequirements { get; set; }
        public string tenderref { get; set; }
        public double? SupplierId { get; set; }
        public string InvitationToTender { get; set; }
        public string EmailIds { get; set; }
        public string cusr { get; set; }
        public DateTime cdte { get; set; }
        public string uusr { get; set; }
        public DateTime udte { get; set; }
        public int isActive { get; set; }
    }
}
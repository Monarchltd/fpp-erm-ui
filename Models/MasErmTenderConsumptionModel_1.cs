using System;

namespace DotNet8.WebApi.Factory.Model
{
    public class MasErmTenderConsumptionModel_1
    {
        public double? id { get; set; }
        public string MasErmTender { get; set; }
        public string TrnERMTenderMsn { get; set; }
        public string TrnERMTenderMpan { get; set; }
        public decimal day { get; set; }
        public decimal night { get; set; }
        public decimal evening { get; set; }
        public decimal total { get; set; }
        public string cusr { get; set; }
        public DateTime cdte { get; set; }
        public string uusr { get; set; }
        public DateTime udte { get; set; }

        public int isActive { get; set; }

    }
}

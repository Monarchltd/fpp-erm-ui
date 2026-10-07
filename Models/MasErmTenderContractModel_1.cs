using System;

namespace DotNet8.WebApi.Factory.Model
{
    public class MasErmTenderContractModel_1
    {
        public double? id { get; set; }
        public string MasErmTender { get; set; }
        public string TrnERMTenderMsn { get; set; }
        public string TrnERMTenderMpan { get; set; }
        public string TrnERMTenderConsumption { get; set; }
        public DateTime startdate { get; set; }
        public string payment_method { get; set; }
        public string payment_duration { get; set; }
        public string cusr { get; set; }
        public DateTime cdte { get; set; }
        public string uusr { get; set; }
        public DateTime udte { get; set; }

        public int isActive { get; set; }

    }
}

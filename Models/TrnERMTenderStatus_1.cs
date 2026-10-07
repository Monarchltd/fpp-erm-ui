using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fppErm.Models
{
    public class TrnERMTenderStatus_1
    {
        public double id { get; set; }
        public double? MasErmTender { get; set; }
        public string tenderref { get; set; }
        public int StatusId { get; set; }
        public string StatusDesc { get; set; }
        public int SubStatusId { get; set; }
        public string SubStatusDesc { get; set; }
        public int CurrentState { get; set; }
        public string cusr { get; set; }
        public DateTime cdte { get; set; }
        public string uusr { get; set; }
        public DateTime udte { get; set; }
        public int isActive { get; set; }

    }
}
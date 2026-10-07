using fppErm.Models;
using System;

namespace DotNet8.WebApi.Factory.Model
{
    public class MasErmTenderHeaderModel
    {
        public double? id { get; set; }
        public string CnbNo { get; set; }
        public string AccountId { get; set; }
        public string TenderId { get; set; }
        public string TenderRef { get; set; }
        public string StatusId { get; set; }
        public string VersionId { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string uusr { get; set; }
        public DateTime udte { get; set; }
        public int isActive { get; set; }
        public MasErmClientModel MasErmClientModel_s { get; set; }
        public MasErmNetworkModel MasErmNetworkModel_s { get; set; }
        public MasErmSupplierModel MasErmSupplierModel_s { get; set; }
        public MasErmTenderModel_1 MasErmTenderModel_s { get; set; }
        public TrnERMTenderStatus_1 TrnERMTenderStatus_s { get; set; }
    }
}

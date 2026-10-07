using fppErm.Models;
using System.Collections.Generic;

namespace DotNet8.WebApi.Factory.Model
{
    public class viewERMTenderModel
    {
        public MasErmTenderModel_1                      ermTender { get; set; }
        public MasErmNetworkModelNHH                    ermNetwork { get; set; }
        public MasErmTenderHeaderModel_1                ermHeader { get; set; }
        public List<TrnERMTenderDetailsNHH>             ermDetailNHH { get; set; }
        public TrnERMRequirements                       ermRequirements { get; set; }
        public List<TrnERMRequirementsSuppliers>         ermSuppliers { get; set; }
        public List<TrnERMRequirementsUploads>           ermUploads { get; set; }
        public List<TrnERMRequirementsContractDuration>  ermContractDuration { get; set; }
        public List<TrnERMTenderNotes_1>                 ermNotes { get; set; }
        public List<TrnERMTenderStatus_1>                ermStatus { get; set; }

    }
}

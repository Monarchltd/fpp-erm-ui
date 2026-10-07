using System;
using System.Collections.Generic;

namespace DotNet8.WebApi.Factory.Model
{
    public class MasErmTenderAnalysis
    {
        public double? MasErmTender { get; set; }
        public List<MasErmTenderMsnModel_1> MasErmTenderMsnModel_s { get; set; }
        public List<MasErmTenderMpanModel_1> MasErmTenderMpanModel_s { get; set; }
        public List<MasErmTenderConsumptionModel_1> MasErmTenderConsumptionModel_s { get; set; }
        public List<MasErmTenderContractModel_1> MasErmTenderContractModel_s { get; set; }
        public List<MasErmTenderCurrentRatesModel_1> MasErmTenderCurrentRatesModel_s { get; set; }

    }
}

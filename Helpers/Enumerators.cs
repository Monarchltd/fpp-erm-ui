using fppErm.Models;
using Microsoft.Graph.Models.TermStore;
using Microsoft.Graph.Models;
using Microsoft.Kiota.Abstractions;
using Net.Pkcs11Interop.Common;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using System.Web.Razor.Parser.SyntaxTree;

namespace fppErm.Helpers
{
    public enum StatusMessages
    {
        //Tender_Created = 0,
        //Tender_InProgress,
        //Tender_Validation,
        //Tender_Sent,
        //Supplier_Query,
        //Supplier_Rejected,
        //Supplier_Acknowledge,
        //No_Supplier_Respose,
        //Pending_with_Supplier,
        //ERM_Validation_InProgress,
        //ERM_Validation_Failed,
        //Price_Accepted,
        //Tender_Repricing_Sent,
        //Tender_Signed
        Tender_Created = 0,
        Tender_Sent,
        Tender_Recalled,
        Tender_Repricing_Sent,
        Tender_Cancelled,
        Tender_Signed,
        Tender_Repriced
    }

    public class nhh
    {
        public List<string> nhhHeaders = new List<string>()
        {
            "Location Ref",
            "Client Ref",
            "Address1",
            "Address2",
            "Address3",
            "Address4",
            "Address5",
            "Post Code",
            "MSN",
            "MPAN Full",
            "Profile",
            "MTC",
            "LLF",
            "Distributor ID",
            "Mpan Core",
            "Mpan Status",
            "Day (kWh)",
            "Night (kWh)",
            "Evening (kWh)",
            "Peak (kWh)",
            "Off Peak (kWh)",
            "Total (kWh)",
            "kVa",
            "Start Date",
            "Payment Method",
            "Payment Duration",
            "Supplier",
            "Last Bill Date",
            "Last Bill Date Supplier",
            "Standing Charge(pence/day)",
            "Day Rate",
            "Night Rate",
            "Evening Rate",
            "Avail Rate",
            "Peak Decjan",
            "Peak Novfeb",
            "Peak Other",
            "Off Peak",
            "Other",
            "MeteringCharge(pence/day)",
            "COMMSCharge(pence/day)",
            "SettlementAgencyCharge(pence/day)",
            "MeterAdministratorAgentCharge(pence/day)",
            "NonEnergy(pence/day)",
            "UMSCharge(pence/day)",
            "TNUosFixedDailyCharge (pence/day)",
            "IntelligentAnalytics (pence/day)",
            "RenewableObligation (pence/day)",
            "NuclearRABLevy(pence/day)",
            "AMRCharge(pence/day)",
            "MeterAssetProvider(MAP)Charge (pence/day)",
            "DataCollectorDataAggregator (pence/day)",
            "MSPUnitCharge(pence/day)",
            "MeterServiceAdvancedCharge (pence/day)",
            "SiteFee(pence/day)",
            "DayAheadPremium(pence/day)",
            "REGO Charge(pence/day)",
            "Cfd Charge(pence/day)",
            "GreengasLevyCharge(pence/day)",
            "DataServicesAdvanced(pence/day)",
            "Remarks",
            "ERM Remarks",
        };
    }
    
}
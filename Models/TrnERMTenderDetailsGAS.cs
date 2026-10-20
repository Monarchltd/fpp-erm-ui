using System;

namespace DotNet8.WebApi.Factory.Model
{
    public class TrnERMTenderDetailsGAS
    {
        public double id { get; set; }
        public double? MasErmTender { get; set; }
        public string Location_Ref { get; set; }
        public string Client_Ref { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }
        public string Address5 { get; set; }
        public string Postcode { get; set; }
        public string Msn { get; set; }
        public string Mprn { get; set; }
        public string kWh { get; set; }
        public string Startdate { get; set; }
        public string Payment_method { get; set; }
        public string Payment_duration { get; set; }
        public string SuppId { get; set; }
        public string Last_bill_date { get; set; }
        public string Last_bill_date_supp { get; set; }
        public string Standing_charge_penceperday { get; set; }
        public string Day_Rate { get; set; }
        public string Peak_Decjan { get; set; }
        public string Peak_Novfeb { get; set; }
        public string Peak_Other { get; set; }
        public string Offpeak { get; set; }
        public string Other { get; set; }
        public string DistCommodity_penceperday { get; set; }
        public string MeteringCharge_penceperday { get; set; }
        public string DistributionSystemCapacity_penceperday { get; set; }
        public string ManagementFee_penceprday { get; set; }
        public string DistFixed_penceperday { get; set; }
        public string Unidentifiedgas_penceperday { get; set; }
        public string TransportFee_penceperday { get; set; }
        public string AMRCharge_penceperday { get; set; }
        public string DistCustomerCapacity_penceprday { get; set; }
        public string SiteFee_penceprday { get; set; }
        public string Remarks { get; set; }
        public string ERMRemarks { get; set; }
        public string cusr { get; set; }
        public DateTime cdte { get; set; }
        public string uusr { get; set; }
        public DateTime udte { get; set; }
        public int isActive { get; set; }
    }
}

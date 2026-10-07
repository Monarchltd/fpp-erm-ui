using System;

namespace DotNet8.WebApi.Factory.Model
{
    public class TrnERMRequirements
    {
        public double id { get; set; }
        public double? MasErmTender { get; set; }
        public string EnergyType { get; set; }
        public string ContractDuration { get; set; }
        public string PaymentTerms { get; set; }
        public string ContractType { get; set; }
        public string IncludedCommission { get; set; }
        public string PaymentDuration { get; set; }
        public DateTime Contract_startdate { get; set; }
        public DateTime Contract_enddate { get; set; }
        public string Dcda_agent { get; set; }
        public DateTime Backbydate { get; set; }
        public string Billing { get; set; }
        public DateTime Submission_deadline { get; set; }
        public string cusr { get; set; }
        public DateTime cdte { get; set; }
        public string uusr { get; set; }
        public DateTime udte { get; set; }
        public int isActive { get; set; }
    }
}

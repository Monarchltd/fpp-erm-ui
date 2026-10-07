using System;

namespace DotNet8.WebApi.Factory.Model
{
    public class MasErmNetworkModel
    {
        public double? id { get; set; }
        public double? clientid { get; set; }
        public string clientref { get; set; }
        public string name { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string postcode { get; set; }
        public string email { get; set; }
        public string headaddress1 { get; set; }
        public string headaddress2 { get; set; }
        public string headpostcode { get; set; }
        public string heademail { get; set; }
        public string cusr { get; set; }
        public DateTime cdte { get; set; }
        public string uusr { get; set; }
        public DateTime udte { get; set; }
        public int isActive { get; set; }
    }
}

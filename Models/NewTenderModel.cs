using System;
using System.Web;

namespace DotNet8.WebApi.Factory.Model
{
    public class NewTenderModel
    {
        public string id { get; set; }
        public uploadPage uploadPage { get; set; }
        public string isSelected { get; set; }
        public string isChecked { get; set; }
    }

    public class uploadPage
    {
        public string supplytype { get; set; }
        public string commissions { get; set; }
        public string commissions_type { get; set; }
        public string currentsupplier { get; set; }
        public string companyreg { get; set; }
        public string newlondps { get; set; }
        public string fueltype { get; set; }
        public string enddatelength { get; set; }
        public string analysisaq { get; set; }
        public string fillers { get; set; }
        public string mopagreement { get; set; }
        public string tenderid { get; set; }

        public string cnbno { get; set; }
        public HttpPostedFileBase uploadedFile { get; set; }
        public string fueltype_brown { get; set; }
        public string fueltype_green { get; set; }
        public string fueltype_carbon { get; set; }
        public string fueltype_selectgreen { get; set; }

    }
}

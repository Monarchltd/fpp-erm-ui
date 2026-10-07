using DotNet8.WebApi.Factory.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fppErm.Helpers
{
    public class nhhValidator
    {
        public List<string> nhhValidation(MasErmTenderModel_3 model)
        {
            List<string> result = new List<string>();
            try
            {
                //count of records.
                int count_of_records = model.TrnERMTenderDetails.Count;

                //check supplier blank data
                int locationref =              model.TrnERMTenderDetails.Where(x => x.Location_Ref.Trim().Length > 0).Count();
                int supplier =                 model.TrnERMTenderDetails.Where(x => x.Supplier.Trim().Length > 0).Count();
                int last_bill_date =           model.TrnERMTenderDetails.Where(x => x.LastBill_Date.Trim().Length > 0).Count();
                int last_bill_date_supplier =  model.TrnERMTenderDetails.Where(x => x.LastBill_Date_Supplier.Trim().Length > 0).Count();
                int duplicateLocref =          model.TrnERMTenderDetails.GroupBy(x => x.Location_Ref).ToList().Count();
                int duplicatemsn =             model.TrnERMTenderDetails.GroupBy(x => x.Msn).ToList().Count();

                if (locationref != count_of_records)
                {
                    result.Add("Location reference column has blank values.");
                }

                if (supplier != count_of_records )
                {
                    result.Add("Supplier column has blank values.");
                }

                if (last_bill_date != count_of_records )
                {
                    result.Add("Last bill date column has blank values.");
                }

                if (last_bill_date_supplier != count_of_records )
                {
                    result.Add("last bill date supplier column has blank values.");
                }

                if (result.Count > 0)
                {
                    result.Add("Upload file has errors. Click Previous button, correct the file and re-upload.");
                }

                if (duplicateLocref < count_of_records)
                {
                    result.Add("Location Ref - duplicates identified");
                }

                if (duplicatemsn < count_of_records)
                {
                    result.Add("Meter Serial No - duplicates identified");
                }

            }
            catch (Exception ex)
            {

            }
            return result;
        }
    }
}
using DotNet8.WebApi.Factory.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace fppErm.Models
{
    public class supplierListModel
    {
        public List<MasErmSupplierModel> StoreList { get; set; } = new List<MasErmSupplierModel>();

    }
}
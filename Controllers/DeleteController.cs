using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fppErm.Controllers
{
    public class DeleteController : Controller
    {
        // GET: Delete
        public ActionResult DeletePage()
        {
            string tid = Request.QueryString["tender_id"] != null ? Request.QueryString["tender_id"].ToString() : "";
            string delflag = Request.QueryString["deleteflag"] != null ? Request.QueryString["deleteflag"].ToString() : "";
            TempData["iconClicked"] = "delete";

            return RedirectToAction("Index", "Home", new { deleteflag = delflag, tender_id = tid });
        }
    }
}
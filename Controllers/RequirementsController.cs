using DotNet8.WebApi.Factory.Model;
using fppErm.Models;
using fppErm.Sessions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Net.Http.Json;

namespace fppErm.Controllers
{
    public class RequirementsController : Controller
    {
        private string _baseurl = null;
        public RequirementsController()
        {
            _baseurl = ConfigurationManager.AppSettings["DEVorPROD"].Equals("dev") ? ConfigurationManager.AppSettings["DEV-API"] : ConfigurationManager.AppSettings["PROD-API"];
        }

        // GET: Requirements
        public ActionResult NewRequirements(HttpPostedFileBase req_uploadFile_1, HttpPostedFileBase req_uploadFile_2, HttpPostedFileBase req_uploadFile_3, HttpPostedFileBase req_uploadFile_4)
        {
            NewTenderModel newtender = (NewTenderModel)TempData["NEWTENDER"];

            HttpClient client =     new HttpClient();
            client.BaseAddress =    new Uri(_baseurl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response1 =          client.GetAsync("diMasERMSupplier/getSupplier?id=" + 0).Result;
            List<MasErmSupplierModel> suppList1 =    new List<MasErmSupplierModel>();
            var logPath =                            Path.Combine(Server.MapPath("~/Uploads"), "logs.txt");
//            StreamWriter writer =                    new StreamWriter(logPath);

            try
            {

                if (response1.IsSuccessStatusCode)
                {
                    var res = response1.Content.ReadFromJsonAsync<IEnumerable<MasErmSupplierModel>>().Result;

                    foreach (var item in res)
                    {
                        string svalue = "";

                        //ELECTRICITY
                        if (newtender.uploadPage.supplytype == "1" || 
                            newtender.uploadPage.supplytype == "2")
                        {
                            if (item.type == 1 ||
                                item.type == 4 ||
                                item.type == 5 ||
                                item.type == 6)
                            {
                                svalue = item.type == 1 ? "E" : item.type == 4 ? "E/G" : item.type == 5 ? "E/G/W" : item.type == 6 ? "E/W" : "";
                                suppList1.Add(new MasErmSupplierModel()
                                {
                                    id = item.id,
                                    name = item.name + " (" + svalue + ")"
                                });
                            }
                        }

                        //GAS
                        if (newtender.uploadPage.supplytype == "3")
                        {
                            if (item.type == 2 ||
                                item.type == 4 ||
                                item.type == 5 ||
                                item.type == 7)
                            {
                                suppList1.Add(new MasErmSupplierModel()
                                {
                                    id = item.id,
                                    name = item.name
                                });
                            }

                        }
                    }

                }

                //Request parameters.
                string conDura =            Request.Params["conDura"] != null ? Request.Params["conDura"] : "0";
                string suppId =             Request.Params["suppId"] != null ? Request.Params["suppId"] : "0";
                var bill_comment =          Request.Form["req_billingtxt"];
                var req_contractenddate =   Request.Form["req_contractenddate"];
                var req_contractstartdate = Request.Form["req_contractstartdate"];
                var req_dcda =              Request.Form["req_dcda"];
                var req_subdeadline =       Request.Form["req_subdeadline"];
                var req_paymentduration =   Request.Form["req_paymentduration"];
                var req_energytype =        Request.Form["req_energytype"];
                var req_paymentterms =      Request.Form["req_paymentterms"];

                string req_contractduration = "";

                if (req_contractstartdate != null && req_contractenddate != null)
                {
                    if (req_contractstartdate.Length > 0 && req_contractenddate.Length > 0)
                    {
                        List<tmpStartEndDateModel> tmpStartEndDateModel = new List<tmpStartEndDateModel>();

                        var tmpstartend = (List<tmpStartEndDateModel>)TempData["tmpStartEndDateModel"];

                        if (tmpstartend != null)
                        {
                            foreach (var itm in tmpstartend)
                            {
                                if (itm.id != Convert.ToInt32(conDura))
                                {
                                    tmpStartEndDateModel.Add(itm);
                                    req_contractduration += itm.duration.ToString() + ",";
                                }
                            }
                        }

                        int iDays = (Convert.ToDateTime(req_contractenddate) - Convert.ToDateTime(req_contractstartdate)).Days;

                        if (Convert.ToInt32(conDura) == 0 &&
                            iDays > 0)
                        {

                            int daysExist = tmpStartEndDateModel.Where(i => i.duration == iDays).ToList().Count();

                            int cid = (tmpStartEndDateModel != null ? (tmpStartEndDateModel.Count > 0 ? tmpStartEndDateModel.Max(i => i.id) + 1 : 1) : 1);

                            if (daysExist == 0)
                            {
                                tmpStartEndDateModel.Add(new tmpStartEndDateModel()
                                {
                                    id = cid,
                                    startDate = Convert.ToDateTime(req_contractstartdate).ToString("dd/MM/yyyy"),
                                    endDate = Convert.ToDateTime(req_contractenddate).ToString("dd/MM/yyyy"),
                                    duration = iDays
                                });

                                req_contractduration += iDays.ToString();
                            }
                        }

                        TempData["tmpStartEndDateModel"] = tmpStartEndDateModel;
                        ViewBag.tmpStartEndDateModel = tmpStartEndDateModel;
                    }
                }

                ViewBag.req_contractstartdate = req_contractstartdate;
                ViewBag.req_contractenddate =   req_contractenddate;
                ViewBag.req_contractduration =  req_contractduration;
                ViewBag.Username =              TempData["username"];
                ViewBag.supplierList =          suppList1.Select(x => new SelectListItem { Value = x.id.ToString(), Text = x.name }).ToList(); // TempData["supplierList"];
                TempData["supplierList"] =      ViewBag.supplierList;
                ViewBag.req_dcda =              req_dcda;
                ViewBag.req_subdeadline =       req_subdeadline;
                ViewBag.req_paymentduration =   req_paymentduration;
                ViewBag.req_energytype =        req_energytype;
                ViewBag.req_paymentterms =      req_paymentterms;
                ViewBag.req_energytypeSel =     "selected";
                ViewBag.req_paymenttermsSel =   "selected";
                ViewBag.req_uploadFile_1 =      req_uploadFile_1 != null ? req_uploadFile_1 : null;
                ViewBag.req_uploadFile_2 =      req_uploadFile_2;
                ViewBag.req_uploadFile_3 =      req_uploadFile_3;
                ViewBag.req_uploadFile_4 =      req_uploadFile_4;

                var master_supplierlist = (IEnumerable<MasErmSupplierModel>)TempData["masterSupplierList"];

                List<SuppSubmissionModel> lst_ssModel = new List<SuppSubmissionModel>();
                int lst_ssModel_index = 1;

                if (TempData["SuppSubmissionModel"] != null)
                {
                    var items = (List<SuppSubmissionModel>)TempData["SuppSubmissionModel"];

                    foreach (var item in items)
                    {
                        if (item.id != Convert.ToInt32(suppId))
                        {
                            lst_ssModel.Add(item);
                            lst_ssModel_index++;
                        }
                    }
                }

                SuppSubmissionModel ssModel = null;

                //if (Request.Form["invitationtotender"]  != null &&
                //    Request.Form["emailids"] != null)
                {

                    ssModel = new SuppSubmissionModel();

                    foreach (var itm in ViewBag.supplierList)
                    {
                        if (Request.Form["reqSupplier"] != null)
                        {
                            if (itm.Value == Request.Form["reqSupplier"])
                            {
                                ssModel.id = Convert.ToInt32(itm.Value);
                                ssModel.supplier_name = itm.Text;
                            }
                        }
                    }
                    //    if (ssModel.id > 0)
                    //    {
                    //        ssModel.invitation_subject =    Request.Form["invitationtotender"];
                    //        ssModel.emailids =              Request.Form["emailids"];

                    //        int exist = lst_ssModel.Where(i => i.invitation_subject == ssModel.invitation_subject && i.emailids == ssModel.emailids).Count();

                    //        if (exist <=0)
                    //            lst_ssModel.Add(ssModel);
                    //    }
                }

                //get the selected supplier email ids.
                if (ssModel != null)
                {
                    HttpResponseMessage response = client.GetAsync("diMasERMSupplier/getSupplier?id=" + ssModel.id).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var res = response.Content.ReadFromJsonAsync<IEnumerable<MasErmSupplierModel>>().Result;

                        foreach (var item in res)
                        {
                            ViewBag.SupplierEmail = item.email;
                        }
                    }


                    if (ssModel.id > 0)
                    {
                        ssModel.invitation_subject = Request.Form["invitationtotender"];
                        ssModel.emailids = ViewBag.SupplierEmail;

                        int exist = lst_ssModel.Where(i => i.invitation_subject == ssModel.invitation_subject && i.emailids == ssModel.emailids).Count();

                        if (exist <= 0)
                            lst_ssModel.Add(ssModel);
                    }

                }

                if (lst_ssModel != null)
                {
                    if (lst_ssModel.Count > 0)
                    {
                        ViewBag.SubmissionSuppliers = lst_ssModel;
                        ViewBag.Disabled = "disabled";
                    }
                }

                if (TempData["NEWTENDER"] != null)
                {
                    //NewTenderModel newtender =      (NewTenderModel)TempData["NEWTENDER"];
                    MasErmTenderModel_3 masTender = (MasErmTenderModel_3)TempData["ANALYSIS"];

                    newtender.uploadPage.companyreg = masTender.MasErmNetworkModel.registration_no;
                    TempData["NEWTENDER"] = newtender;

                    ViewBag.NewTender = newtender;
                    ViewBag.companyreg = newtender.uploadPage.companyreg;

                    //Elec-1, Gas-2, Water-3
                    ViewBag.req_energytype = newtender.uploadPage.commissions_type.Equals("1") ? "1" :
                                                    newtender.uploadPage.commissions_type.Equals("2") ? "1" :
                                                        newtender.uploadPage.commissions_type.Equals("3") ? "2" : "3";

                    if (newtender.uploadPage.commissions_type.Equals("1"))
                        ViewBag.IncludeCommission = "£ " + newtender.uploadPage.commissions + "( £/pa )";
                    else
                        ViewBag.IncludeCommission = newtender.uploadPage.commissions + "( p/kwh )";

                    if (ssModel.id == 0)
                    {
                        if (lst_ssModel != null)
                        {
                            if (lst_ssModel.Count > 0)
                            {
                                foreach (var ss in lst_ssModel)
                                {
                                    ssModel = ss;
                                }
                            }
                        }
                    }
                    //var supplierList = suppList.Select(x => new SelectListItem { Value = x.id.ToString(), Text = x.name }).ToList();

                    //Get suppler list.
                    //var currsupplier =      ((List<SelectListItem>)TempData["supplierList"]).Where(i => i.Value == newtender.uploadPage.currentsupplier).FirstOrDefault();
                    var currsupplier = ((List<SelectListItem>)ViewBag.supplierList).Where(i => i.Value == ssModel.id.ToString()).FirstOrDefault();

                    //int total_noofsites =   masTender.MasErmTenderMsnModel_s.Count();
                    int total_noofsites = masTender.TrnERMTenderDetails.Count();

                    if (currsupplier != null && ssModel != null)
                    {
                        //Invitation to Tender.
                        ViewBag.Invitationtotender = "Invitation to Tender - " +
                                                        (newtender.uploadPage.supplytype == "1" ? "Electric" : newtender.uploadPage.supplytype == "2" ? "Electric" : "Gas") + " ITT" + " - " +
                                                        ssModel.supplier_name + " " + DateTime.Now.DayOfWeek.ToString().Substring(0, 3) + "-" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second + " (" +
                                                        "Current Supplier - " + currsupplier.Text + ") " +
                                                        "(" + total_noofsites + " " + (total_noofsites > 1 ? "Sites" : "Site") + ") " + ConfigurationManager.AppSettings["ERM_DPSREF"];
                    }

                }

                TempData["SuppSubmissionModel"] = lst_ssModel;

                ViewBag.req_billingtxt = ConfigurationManager.AppSettings["ERM_BILLING"].ToString();
                TempData["username"] = ViewBag.Username;
                TempData.Keep();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                //writer.Close();
            }

            return View();
        }
    }
}
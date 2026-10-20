using Azure;
using DotNet8.WebApi.Factory.Model;
using DotNet8.WebApi.Factory.Model.Edf;
using DotNet8.WebApi.Factory.Model.sse;
using fppErm.Helpers;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Web;
using System.Web.Mvc;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;
using static System.Net.WebRequestMethods;



namespace fppErm.Controllers
{

    public class AnalysisController : Controller
    {
        private string _baseurl = null;

        public AnalysisController()
        {
            _baseurl = ConfigurationManager.AppSettings["DEVorPROD"].Equals("dev") ? ConfigurationManager.AppSettings["DEV-API"] : ConfigurationManager.AppSettings["PROD-API"];
        }

        [HttpPost]
        public ActionResult AnalysisPage(HttpPostedFileBase monthfile12, HttpPostedFileBase monthfile24, HttpPostedFileBase monthfile36)
        {
            ViewBag.analyProc = false;
            var dwnFlag =       Request.QueryString["download"] != null ? Request.QueryString["download"] : "0";
            var tender_id =     Request.Form["tenderId"] != null ? Request.Form["tenderId"] : TempData["TenderId"];
            var cnbno =         Request.Form["cnbNo"] != null ? Request.Form["cnbNo"] : TempData["CnbNo"];
            var supplier =      Request.Form["supplier"] != null ? Request.Form["supplier"] : TempData["supplier"];
            var type =          Request.Form["type"] != null ? Request.Form["type"] : TempData["type"];
            var status_update = Request.Form["statusUpdate"];

            ViewBag.Username =  TempData["username"];
            TempData["username"] = ViewBag.Username;
            TempData["iconClicked"] =   "analysis";

            TempData["TenderId"] =      tender_id;
            TempData["CnbNo"] =         cnbno;
            TempData["supplier"] =      supplier;
            TempData["type"] =          type;
            ViewBag.Supplier =          supplier;

            HttpClient client =     new HttpClient();
            client.BaseAddress =    new Uri(_baseurl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (monthfile12 != null)
            {
                var uploadPath = Server.MapPath("~/Analysis");

                var fileName =  Path.GetFileName(monthfile12.FileName);
                var path =      Path.Combine(uploadPath, fileName);

                monthfile12.SaveAs(path);

                if (supplier.Equals("EDF Energy"))
                {
                    HttpResponseMessage response = client.GetAsync("analysis\\readData?filepath=" + path + "&tenderid=" + tender_id + "&type=" + type + "&supplier=" + supplier).Result;

                    var res = response.Content.ReadFromJsonAsync<IEnumerable<sseSCModel>>().Result;

                    if (res != null)
                    {
                        List<sseSCModel> res1 = new List<sseSCModel>();
                        List<sseSCModel> res2 = new List<sseSCModel>();
                        List<sseSCModel> res3 = new List<sseSCModel>();

                        foreach (var results in res)
                        {
                            if (Convert.ToInt32(results.noof_months) >= 0 && Convert.ToInt32(results.noof_months) <= 12)
                            {
                                res1.Add(results);
                            }
                            else if (Convert.ToInt32(results.noof_months) >= 13 && Convert.ToInt32(results.noof_months) <= 24)
                            {
                                res2.Add(results);
                            }
                            else if (Convert.ToInt32(results.noof_months) >= 25 && Convert.ToInt32(results.noof_months) <= 36)
                            {
                                res3.Add(results);
                            }
                        }

                        ViewBag.Result1 = res1;
                        ViewBag.Result2 = res2;
                        ViewBag.Result3 = res3;

                        if (res1.Count > 0 || res2.Count > 0 || res3.Count > 0)
                            ViewBag.analyProc = true;
                    }

                }
                else if(supplier.Equals("SSE"))
                {
                    HttpResponseMessage response = client.GetAsync("analysis\\readData?filepath=" + path + "&tenderid=" + tender_id + "&type=" + type + "&supplier=" + supplier).Result;

                    var res = response.Content.ReadFromJsonAsync<IEnumerable<sseSCModel>>().Result;

                    if (res != null)
                    {
                        List<sseSCModel> res1 = new List<sseSCModel>();
                        List<sseSCModel> res2 = new List<sseSCModel>();
                        List<sseSCModel> res3 = new List<sseSCModel>();

                        foreach (var results in res)
                        {
                            if (Convert.ToInt32(results.noof_months) >= 0 && Convert.ToInt32(results.noof_months) <= 12)
                            {
                                res1.Add(results);
                            }
                            else if (Convert.ToInt32(results.noof_months) >= 13 && Convert.ToInt32(results.noof_months) <= 24)
                            {
                                res2.Add(results);
                            }
                            else if (Convert.ToInt32(results.noof_months) >= 25 && Convert.ToInt32(results.noof_months) <= 36)
                            {
                                res3.Add(results);
                            }

                        }

                        ViewBag.Result1 = res1;
                        ViewBag.Result2 = res2;
                        ViewBag.Result3 = res3;

                        if (res1.Count > 0 || res2.Count > 0 || res3.Count > 0)
                            ViewBag.analyProc = true;
                    }

                }
            }

            if (dwnFlag=="1")
            {
                try
                {
                    return Download();
                }
                catch(Exception ex)
                {
                }
            }

            return View();
        }

        [HttpPost]
        public ActionResult readXLSX(HttpPostedFileBase uploadedFile)
        {
            TempData.Keep();
            nhh n =                             new nhh();
            gas g =                             new gas();
            ViewBag.nhhHeader =                 n.nhhHeaders;
            ViewBag.gasHeader =                 g.gasHeaders;

            bool nextPage =                     false;
            ViewBag.Username =                  TempData["username"];
            ViewBag.supplierList =              TempData["supplierList"];
            var master_supplierlist =           TempData["masterSupplierList"];

            string guid =                       Guid.NewGuid().ToString();

            StreamWriter writer =               new StreamWriter(AppDomain.CurrentDomain.BaseDirectory + "\\Uploads\\logs.txt");

            try
            {

                NewTenderModel model =          new NewTenderModel();
                uploadPage uploadPage =         new uploadPage();
                uploadPage.supplytype =         Request.Form["supplyType"];
                uploadPage.commissions =        Request.Form["commissions"];
                uploadPage.commissions_type =   Request.Form["commissionstype"];
                uploadPage.currentsupplier =    Request.Form["currentSupplier"];
                uploadPage.companyreg =         ""; // Request.Form["companyreg"];
                uploadPage.newlondps =          Request.Form["newlondps"];
                uploadPage.fueltype_brown =     Request.Form["fueltype_brown"];
                uploadPage.fueltype_green =     Request.Form["fueltype_green"];
                uploadPage.fueltype_carbon =    Request.Form["fueltype_carbon"];
                uploadPage.fueltype_selectgreen = Request.Form["fueltype_selectgreen"];

                string fueltype =               uploadPage.fueltype_brown != null ? uploadPage.fueltype_brown : "00";
                fueltype +=                     uploadPage.fueltype_green != null ? uploadPage.fueltype_green : "00";
                fueltype +=                     uploadPage.fueltype_carbon != null ? uploadPage.fueltype_carbon : "00";
                fueltype +=                     uploadPage.fueltype_selectgreen != null ? uploadPage.fueltype_selectgreen : "00";

                //uploadPage.fueltype =         Request.Form["fueltype"];
                uploadPage.fueltype =           fueltype;
                uploadPage.enddatelength =      ""; // Request.Form["enddatelength"];
                uploadPage.analysisaq =         Request.Form["analysisaq"];
                uploadPage.fillers =            Request.Form["fillers"];
                uploadPage.mopagreement =       Request.Form["mopagreement"];
                uploadPage.cnbno =              Request.Params["cnbno"];
                uploadPage.uploadedFile =       uploadedFile;
                model.uploadPage =              uploadPage;
                model.isSelected =              "selected";
                model.isChecked =               "checked";

                string tenderid =               " ... > ";

                model.id =                      tenderid;

                Session["tenderid"] =           tenderid;

                string tabname =                uploadPage.supplytype != null ?
                                                    (uploadPage.supplytype == "1") ? "Ele-HH" :
                                                        (uploadPage.supplytype == "2") ? "Ele-NHH" : "Gas" : String.Empty; ;

                if (model.uploadPage.uploadedFile != null)
                {
                    if (model.uploadPage.supplytype == "1" || model.uploadPage.supplytype == "2")
                    {
                        if (model.uploadPage.uploadedFile.ContentLength > 0 && model.uploadPage.supplytype.Length > 0)
                        {
                            //create guid folder.
                            var uploadPath = Path.Combine(Server.MapPath("~/Uploads"), guid);

                            if (!Directory.Exists(uploadPath))
                            {
                                DirectoryInfo di = Directory.CreateDirectory(uploadPath);
                            }

                            var fileName = Path.GetFileName(model.uploadPage.uploadedFile.FileName);
                            var path = Path.Combine(uploadPath, fileName);

                            writer.WriteLine(AppDomain.CurrentDomain.BaseDirectory.ToString());
                            writer.WriteLine(uploadedFile.ToString());
                            writer.WriteLine(path);

                            var destPath = path;

                            uploadedFile.SaveAs(destPath);

                            ViewBag.Analysis = null;
                            ViewBag.Supplytype = model.uploadPage.supplytype;
                            ViewBag.TenderId = model.id;

                            HttpClient client = new HttpClient();
                            client.BaseAddress = new Uri(_baseurl);
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                            HttpResponseMessage cnbResponse = client.GetAsync("NHHValidation\\chekforcnb?cnb=" + uploadPage.cnbno).Result;

                            var cnbRes = cnbResponse.Content.ReadFromJsonAsync<bool>().Result;

                            if (cnbResponse.IsSuccessStatusCode)
                            {
                                if (cnbRes == true)
                                {
                                    TempData["ERROR"] = "CNB No already exist.";
                                    nextPage = false;
                                }
                            }

                            HttpResponseMessage response = client.GetAsync("ReadXL?filepath=" + destPath + "&tabname=" + tabname).Result;

                            if (response.IsSuccessStatusCode && cnbRes == false)
                            {
                                var res = response.Content.ReadFromJsonAsync<MasErmTenderModel_3>().Result;

                                if (res.MasErmTenderHeaderModel != null)
                                {

                                    res.supplytype = uploadPage.supplytype;
                                    res.commission = Convert.ToDecimal(uploadPage.commissions);
                                    res.commission_type = uploadPage.commissions_type;
                                    res.supplier = uploadPage.currentsupplier;
                                    res.comp_reg = uploadPage.companyreg;
                                    res.newlondps = uploadPage.newlondps;
                                    res.fueltype = uploadPage.fueltype;
                                    res.enddatelength = uploadPage.enddatelength;
                                    res.analysisaq = uploadPage.analysisaq;
                                    res.fillers = uploadPage.fillers;
                                    res.mopagreement = uploadPage.mopagreement;
                                    //res.cnbno =             uploadPage.cnbno;
                                    res.MasErmTenderHeaderModel.TenderRef = guid;

                                    model.uploadPage.companyreg = res.comp_reg;

                                    TempData["INFO"] = null;
                                    TempData["ERROR"] = null;

                                    if (res.successMessage != null)
                                    {
                                        ViewBag.Analysis = res;
                                        TempData["INFO"] = res.successMessage;
                                        TempData["ANALYSIS"] = res;
                                        TempData["masterSupplierList"] = master_supplierlist;

                                        if (res.successMessage == "Error")
                                        {
                                            List<string> errors = new List<string>();
                                            TempData["ERROR"] = res.errMessage;
                                            string[] errMessage = res.errMessage.Split('|');
                                            foreach (string s in errMessage)
                                            {
                                                errors.Add(s);
                                            }
                                            ViewBag.Errors = errors;
                                            nextPage = false;
                                            ViewBag.Nextpage = "false";
                                        }

                                        nhhValidator nhhValidator = new nhhValidator();
                                        var validation_result = nhhValidator.nhhValidation(res);
                                        if (validation_result.Count > 0)
                                        {
                                            TempData["ERROR"] = "Errors";
                                            ViewBag.Errors = validation_result;
                                            nextPage = false;
                                            ViewBag.Nextpage = "false";
                                        }
                                        else
                                        {
                                            nextPage = true;
                                            ViewBag.Nextpage = "true";
                                        }
                                    }
                                    else
                                    {
                                        TempData["ERROR"] = res.errMessage;
                                        nextPage = false;
                                        ViewBag.Nextpage = "false";
                                    }
                                }
                                else
                                {
                                    nextPage = false; ViewBag.Nextpage = "false";
                                    TempData["ERROR"] = "Upload not parsed. Please check the Energy Type and Uploaded file";
                                }
                            }
                            else
                            {
                            }
                        }
                        else
                        {

                        }
                    }
                    else if (model.uploadPage.supplytype == "3")
                    {
                        if (model.uploadPage.uploadedFile.ContentLength > 0 && model.uploadPage.supplytype.Length > 0)
                        {
                            //create guid folder.
                            var uploadPath = Path.Combine(Server.MapPath("~/Uploads"), guid);

                            if (!Directory.Exists(uploadPath))
                            {
                                DirectoryInfo di = Directory.CreateDirectory(uploadPath);
                            }

                            var fileName = Path.GetFileName(model.uploadPage.uploadedFile.FileName);
                            var path = Path.Combine(uploadPath, fileName);

                            writer.WriteLine(AppDomain.CurrentDomain.BaseDirectory.ToString());
                            writer.WriteLine(uploadedFile.ToString());
                            writer.WriteLine(path);

                            var destPath = path;

                            uploadedFile.SaveAs(destPath);

                            ViewBag.Analysis = null;
                            ViewBag.Supplytype = model.uploadPage.supplytype;
                            ViewBag.TenderId = model.id;

                            HttpClient client = new HttpClient();
                            client.BaseAddress = new Uri(_baseurl);
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                            HttpResponseMessage cnbResponse = client.GetAsync("NHHValidation\\chekforcnb?cnb=" + uploadPage.cnbno).Result;

                            var cnbRes = cnbResponse.Content.ReadFromJsonAsync<bool>().Result;

                            if (cnbResponse.IsSuccessStatusCode)
                            {
                                if (cnbRes == true)
                                {
                                    TempData["ERROR"] = "CNB No already exist.";
                                    nextPage = false;
                                }
                            }

                            HttpResponseMessage response = client.GetAsync("ReadXL?filepath=" + destPath + "&tabname=" + tabname).Result;

                            if (response.IsSuccessStatusCode && cnbRes == false)
                            {
                                var res = response.Content.ReadFromJsonAsync<MasErmTenderModel_3>().Result;

                                if (res.MasErmTenderHeaderModel != null)
                                {

                                    res.supplytype =        uploadPage.supplytype;
                                    res.commission =        Convert.ToDecimal(uploadPage.commissions);
                                    res.commission_type =   uploadPage.commissions_type;
                                    res.supplier =          uploadPage.currentsupplier;
                                    res.comp_reg =          uploadPage.companyreg;
                                    res.newlondps =         uploadPage.newlondps;
                                    res.fueltype =          uploadPage.fueltype;
                                    res.enddatelength =     uploadPage.enddatelength;
                                    res.analysisaq =        uploadPage.analysisaq;
                                    res.fillers =           uploadPage.fillers;
                                    res.mopagreement =      uploadPage.mopagreement;

                                    res.MasErmTenderHeaderModel.TenderRef = guid;
                                    model.uploadPage.companyreg =           res.comp_reg;
                                    TempData["INFO"] =                      null;
                                    TempData["ERROR"] =                     null;

                                    if (res.successMessage != null)
                                    {
                                        ViewBag.Analysis =                  res;
                                        TempData["INFO"] =                  res.successMessage;
                                        TempData["ANALYSIS"] =              res;
                                        TempData["masterSupplierList"] =    master_supplierlist;

                                        if (res.successMessage == "Error")
                                        {
                                            List<string> errors =           new List<string>();
                                            TempData["ERROR"] =             res.errMessage;
                                            string[] errMessage =           res.errMessage.Split('|');
                                            foreach (string s in errMessage)
                                            {
                                                errors.Add(s);
                                            }
                                            ViewBag.Errors =    errors;
                                            nextPage =          false;
                                            ViewBag.Nextpage =  "false";
                                        }

                                        gasValidator gasValidator = new gasValidator();
                                        var validation_result = gasValidator.gasValidation(res);
                                        if (validation_result.Count > 0)
                                        {
                                            TempData["ERROR"] = "Errors";
                                            ViewBag.Errors =    validation_result;
                                            nextPage =          false;
                                            ViewBag.Nextpage =  "false";
                                        }
                                        else
                                        {
                                            nextPage =          true;
                                            ViewBag.Nextpage =  "true";
                                        }
                                    }
                                    else
                                    {
                                        TempData["ERROR"] = res.errMessage;
                                        nextPage =          false;
                                        ViewBag.Nextpage =  "false";
                                    }
                                }
                                else
                                {
                                    nextPage =          false;
                                    ViewBag.Nextpage =  "false";
                                    TempData["ERROR"] = "Upload not parsed. Please check the Energy Type and Uploaded file";
                                }
                            }
                            else
                            {
                            }
                        }
                        else
                        {

                        }

                    }
                }

                if (model.uploadPage.supplytype != null)
                    TempData["NEWTENDER"] = model;

            }
            catch (Exception ex)
            {
                writer.WriteLine(ex.ToString());
            }
            finally
            {
                writer.Close();
            }

            TempData.Keep();

            if (uploadedFile != null && nextPage)
            {
                ViewBag.NewTender = TempData["NEWTENDER"];
                return View();
            }
            else if (TempData["ANALYSIS"] != null)
            {
                ViewBag.Analysis =              TempData["ANALYSIS"];
                ViewBag.NewTender =             TempData["NEWTENDER"];
                TempData["INFO"] =              ViewBag.Analysis.successMessage;
                ViewBag.TenderId =              ViewBag.NewTender.id;
                TempData["masterSupplierList"]= master_supplierlist;

                MasErmTenderModel_3 tmpAnalysis = (MasErmTenderModel_3)TempData["ANALYSIS"];

                if (tmpAnalysis.TrnERMTenderDetails.Count>0 ||
                    tmpAnalysis.TrnERMTenderDetailsGAS.Count > 0)
                {
                    ViewBag.Nextpage = "true";
                }

                return View();
            }
            else
            {
                return RedirectToAction("NewTender", "Tenders");
            }
        }

        [HttpPost]
        public ActionResult Download()
        {
            //Build the File Path.
            string path = Path.Combine(Server.MapPath("~/Analysis"), "Temp", "Analysis Template 2026.xlsx");

            //Read the File data into Byte Array.
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            //Send the File to Download.
            return File(bytes, "application/octet-stream", "Analysis_Template_2026.xlsx");
        }
    }

}

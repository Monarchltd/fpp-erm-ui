using Azure;
using DotNet8.WebApi.Factory.Model;
using DotNet8.WebApi.Factory.Model.Edf;
using DotNet8.WebApi.Factory.Model.sse;
using fppErm.Helpers;
using fppErm.Models;
using Microsoft.Graph.Models;
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Windows.Shapes;
using Path = System.IO.Path;



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
        public async Task<ActionResult> AnalysisPage(HttpPostedFileBase monthfile12,
            HttpPostedFileBase monthfile13,
            HttpPostedFileBase monthfile14,
            HttpPostedFileBase monthfile15,
            HttpPostedFileBase monthfile16,
            HttpPostedFileBase monthfile17)
        {
            ViewBag.analyProc = false;
            ViewBag.Run =       "Run";
            var dwnFlag =       Request.QueryString["download"] != null ? Request.QueryString["download"] : "0";
            var tender_id =     Request.Form["tenderId"] != null ? Request.Form["tenderId"] : TempData["TenderId"];
            var cnbno =         Request.Form["cnbNo"] != null ? Request.Form["cnbNo"] : TempData["CnbNo"];
            var supplier =      Request.Form["supplier"] != null ? Request.Form["supplier"] : TempData["supplier"];
            var type =          Request.Form["type"] != null ? Request.Form["type"] : TempData["type"];
            var status_update = Request.Form["statusUpdate"];
            var clientname =    Request.Form["clientName"] != null ? Request.Form["clientName"] : TempData["clientName"];

            ViewBag.Username =          TempData["username"];
            TempData["username"] =      ViewBag.Username;
            TempData["iconClicked"] =   "analysis";
            TempData["TenderId"] =      tender_id;
            TempData["CnbNo"] =         cnbno;
            TempData["supplier"] =      supplier;
            TempData["type"] =          type;
            TempData["clientName"] =    clientname;
            ViewBag.Supplier =          supplier;

            if (supplier.Equals("EDF Energy"))
            {
                if (type.Equals("HH") || 
                    type.Equals("NHH"))
                {
                    ViewBag.Run = "Price Loader";
                }
            }

            //Check for duplicates.
            List<string> monFiles = new List<string>();
            monFiles.Add(monthfile12 != null ? monthfile12.FileName : "monthfile12");
            monFiles.Add(monthfile13 != null ? monthfile13.FileName : "monthfile13");
            monFiles.Add(monthfile14 != null ? monthfile14.FileName : "monthfile14");
            monFiles.Add(monthfile15 != null ? monthfile15.FileName : "monthfile15");
            monFiles.Add(monthfile16 != null ? monthfile16.FileName : "monthfile16");
            monFiles.Add(monthfile17 != null ? monthfile17.FileName : "monthfile17");

            List<string> unique_monFiles = monFiles.Distinct().ToList();

            if (monFiles.Count == unique_monFiles.Count)
            {
                //No Duplicates.

                var uploadPath =    Server.MapPath("~/Analysis");
                string filepath12 = "0";
                string filepath13 = "0";
                string filepath14 = "0";
                string filepath15 = "0";
                string filepath16 = "0";
                string filepath17 = "0";

                if (monthfile12 != null)
                {
                    filepath12 = Path.Combine(uploadPath, Path.GetFileName(monthfile12.FileName));
                    monthfile12.SaveAs(filepath12);
                }
                if (monthfile13 != null)
                {
                    filepath13 = Path.Combine(uploadPath, Path.GetFileName(monthfile13.FileName));
                    monthfile13.SaveAs(filepath13);
                }
                if (monthfile14 != null)
                {
                    filepath14 = Path.Combine(uploadPath, Path.GetFileName(monthfile14.FileName));
                    monthfile14.SaveAs(filepath14);
                }
                if (monthfile15 != null)
                {
                    filepath15 = Path.Combine(uploadPath, Path.GetFileName(monthfile15.FileName));
                    monthfile15.SaveAs(filepath15);
                }
                if (monthfile16 != null)
                {
                    filepath16 = Path.Combine(uploadPath, Path.GetFileName(monthfile16.FileName));
                    monthfile16.SaveAs(filepath16);
                }
                if (monthfile17 != null)
                {
                    filepath17 = Path.Combine(uploadPath, Path.GetFileName(monthfile17.FileName));
                    monthfile17.SaveAs(filepath17);
                }

                if (monthfile12 != null || monthfile13 != null || monthfile14 != null ||
                    monthfile15 != null || monthfile16 != null || monthfile17 != null)
                {
                    if (supplier.Equals("EDF Energy"))
                    {
                        try
                        {
                            HttpResponseMessage response = await performProcess(filepath12, filepath13, filepath14, filepath15, filepath16, filepath17, tender_id.ToString(), type.ToString(), supplier.ToString());
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
                        catch(TaskCanceledException ex)
                        {

                        }
                    }

                    if (supplier.Equals("SSE"))
                    {
                        try
                        {
                            HttpResponseMessage response = await performProcessSSE(filepath12, filepath13, filepath14, filepath15, filepath16, filepath17, tender_id.ToString(), type.ToString(), supplier.ToString());

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
                        catch(TaskCanceledException ex)
                        {

                        }

                    }
                }
            }
            else
            {
                //Duplicate files found in upload.
                ViewBag.Message = " ** Trying to upload, dupilcates files.";
            }

            /*
            if (monthfile12 != null)
            {
                var uploadPath = Server.MapPath("~/Analysis");

                var fileName = Path.GetFileName(monthfile12.FileName);
                var path = Path.Combine(uploadPath, fileName);

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
                else if (supplier.Equals("SSE"))
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
            
            */
            
            if (dwnFlag=="1")
            {
                try
                {
                    return Download(tender_id.ToString(), cnbno.ToString(), supplier.ToString(), type.ToString(), clientname.ToString());
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
        public ActionResult Download(string tender_id, string cnbno, string supplier, string type, string clientname)
        {
            string sourcefilename = String.Empty;
            string destfilename =   clientname + "-MON-" + cnbno.ToLower().Replace("mon-", "") + ".xlsx";

            if (type=="HH" || type=="NHH")
            {
                sourcefilename = "Ele_Analysis Template 2026.xlsx";
            }
            else
            {
                sourcefilename = "Gas_Analysis Template 2026.xlsx";
            }

            //Build the File Path.
            string path = Path.Combine(Server.MapPath("~/Analysis"), "Temp", sourcefilename);

            //Read the File data into Byte Array.
            byte[] bytes = System.IO.File.ReadAllBytes(path);

            //Send the File to Download.
            return File(bytes, "application/octet-stream", destfilename);
        }

        public ActionResult MultiAnalysisPage()
        {
            string suppid =         Request.QueryString["suppId"];
            string suppIdremove =   Request.QueryString["suppIdremove"];
            string searchclick =    Request.QueryString["searchclick"];
            string runreport =      Request.QueryString["runreport"];
            string supplytype =     Request.Form["supplytype"];
            var tmpSid =            (List<string>)TempData["suppid"];

            HttpClient client =  new HttpClient();
            client.BaseAddress = new Uri(_baseurl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            int stype = supplytype != null ? supplytype == "HH" ? 1 : supplytype == "NHH" ? 2 : 3 : 1;

            if (searchclick != null)
            {
                if (searchclick == "1")
                {
                    if (tmpSid != null)
                        tmpSid.Clear();
                }
            }

            List<string> sid = new List<string>();

            if (tmpSid != null && suppid != null)
            {
                foreach (string s in tmpSid)
                {
                    sid.Add(s);
                }
                sid.Add(suppid);
            }
            if (tmpSid != null && suppIdremove != null)
            {
                foreach (string s in tmpSid)
                {
                    sid.Add(s);
                }

                sid.Remove(suppIdremove);
            }

            if (runreport == "1")
            {
                List<MultiAnalysisModel> multiAnalyses = new List<MultiAnalysisModel>();

                foreach (string s in tmpSid)
                {
                    multiAnalyses.Add(new MultiAnalysisModel { tenderid = s });
                }

                JsonContent content =           JsonContent.Create(multiAnalyses);
                HttpResponseMessage response1 = client.PostAsync("analysis/multiAnalysis", content).Result;
                //HttpResponseMessage response1 = await performMultiAnalysis(content);
                ViewBag.analyProc = true;
            }

            TempData["suppid"] =            sid;
            ViewBag.Username =              TempData["username"];
            HttpResponseMessage response =  client.GetAsync("Tender/SearchTenderwithEnergytype?type=" + stype).Result;
            ViewBag.Tenders =               null;

            if (response.IsSuccessStatusCode)
            {
                var tenders =       response.Content.ReadFromJsonAsync<IEnumerable<MasErmTenderHeaderModel>>().Result;
                ViewBag.Tenders =   tenders;

                List<MultiSuppAnaModel> msa = new List<MultiSuppAnaModel>();

                foreach (var tender in tenders)
                {
                    msa.Add(new MultiSuppAnaModel
                    {
                        id =            tender.TenderId.ToString(),
                        cnbno=          tender.CnbNo.ToString(),
                        networkname =   tender.MasErmNetworkModel_s.name,
                        suppliername =  tender.MasErmSupplierModel_s.name,
                        suppliertype =  (tender.MasErmTenderModel_s.supplytype != null ? tender.MasErmTenderModel_s.supplytype == "1" ? "HH" : tender.MasErmTenderModel_s.supplytype == "2" ? "NHH" : "Gas" : ""),
                        statusdesc =    tender.TrnERMTenderStatus_s.StatusDesc,
                        versionid =     "0",
                        createdby =     tender.CreatedBy,
                        createddate =   tender.CreatedDate.ToString(),
                        analysisAvail = tender.VersionId.Length > 0 ? Convert.ToInt32(tender.VersionId.Trim()) > 0 ? "Yes" : "No" : "No",
                        button_desc =   (sid.Where(x => x == tender.TenderId.ToString()).Count() > 0) ? "Remove" : "Add"
                    });
                }
                ViewBag.Tenders = msa;

            }
            else
            {
            }

            TempData["username"] =      ViewBag.Username;
            TempData["supplyType"] =    supplytype !=null ? supplytype : "HH";
            ViewBag.SupplyType =        TempData["supplyType"];
            ViewBag.SuppTypeSelected =  "selected";
            ViewBag.ActionDisabled =    "disabled";
            return View();
        }

        private async Task<HttpResponseMessage> performProcess(string filepath12,
            string filepath13,
            string filepath14,
            string filepath15,
            string filepath16,
            string filepath17,
            string tender_id,
            string type,
            string supplier)
        {
            return await Task.Run(() =>
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(_baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Simulating an operation that requires intensive CPU processing
                int result = 0;
                HttpResponseMessage response = client.GetAsync("analysis\\readDatav1?filepath1=" + filepath12 +
                    "&filepath2=" + filepath13 +
                    "&filepath3=" + filepath14 +
                    "&filepath4=" + filepath15 +
                    "&filepath5=" + filepath16 +
                    "&filepath6=" + filepath17 +
                    "&tenderid=" + tender_id + "&type=" + type + "&supplier=" + supplier).Result;

                //var res = response.Content.ReadFromJsonAsync<IEnumerable<sseSCModel>>().Result;

                return response;
            });
        }


        private async Task<HttpResponseMessage> performProcessSSE(string filepath12,
                                                                  string filepath13,
                                                                  string filepath14,
                                                                  string filepath15,
                                                                  string filepath16,
                                                                  string filepath17,
                                                                  string tender_id,
                                                                  string type,
                                                                  string supplier)
        {
            return await Task.Run(() =>
            {
                HttpClient client =     new HttpClient();
                client.BaseAddress =    new Uri(_baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Simulating an operation that requires intensive CPU processing
                int result = 0;
                HttpResponseMessage response = client.GetAsync("analysis\\readDatav1?filepath1=" + filepath12 +
                    "&filepath2=" + filepath13 +
                    "&filepath3=" + filepath14 +
                    "&filepath4=" + filepath15 +
                    "&filepath5=" + filepath16 +
                    "&filepath6=" + filepath17 +
                    "&tenderid=" + tender_id + "&type=" + type + "&supplier=" + supplier).Result;

                return response;
            });
        }


        private async Task<HttpResponseMessage> performMultiAnalysis(JsonContent content)
        {
            return await Task.Run(() =>
            {
                HttpClient client =     new HttpClient();
                client.BaseAddress =    new Uri(_baseurl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Simulating an operation that requires intensive CPU processing
                int result = 0;
                var response = client.PostAsync("analysis/multiAnalysis", content).Result;

                return response;
            });
        }

    }

}

using DotNet8.WebApi.Factory.Model;
using fppErm.Helpers;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Web;
using System.Web.Mvc;
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
        public ActionResult readXLSX(HttpPostedFileBase uploadedFile)
        {
            TempData.Keep();
            nhh n =                             new nhh();
            ViewBag.nhhHeader =                 n.nhhHeaders;

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

                string fueltype =               uploadPage.fueltype_brown != null ? uploadPage.fueltype_brown : "00";
                fueltype +=                     uploadPage.fueltype_green != null ? uploadPage.fueltype_green : "00";
                fueltype +=                     uploadPage.fueltype_carbon != null ? uploadPage.fueltype_carbon : "00";

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
                    if (model.uploadPage.uploadedFile.ContentLength > 0 && model.uploadPage.supplytype.Length > 0)
                    {
                        //create guid folder.
                        var uploadPath =   Path.Combine(Server.MapPath("~/Uploads"), guid);

                        if (!Directory.Exists(uploadPath))
                        {
                            DirectoryInfo di = Directory.CreateDirectory(uploadPath);
                        }

                        var fileName =  Path.GetFileName(model.uploadPage.uploadedFile.FileName);
                        var path =      Path.Combine(uploadPath, fileName);

                        writer.WriteLine(AppDomain.CurrentDomain.BaseDirectory.ToString());
                        writer.WriteLine(uploadedFile.ToString());
                        writer.WriteLine(path);

                        var destPath = path;

                        uploadedFile.SaveAs(destPath);

                        ViewBag.Analysis =      null;
                        ViewBag.Supplytype =    model.uploadPage.supplytype;
                        ViewBag.TenderId =      model.id;

                        HttpClient client =     new HttpClient();
                        client.BaseAddress =    new Uri(_baseurl);
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
                            var res =               response.Content.ReadFromJsonAsync<MasErmTenderModel_3>().Result;

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
                                //res.cnbno =             uploadPage.cnbno;
                                res.MasErmTenderHeaderModel.TenderRef = guid;

                                model.uploadPage.companyreg = res.comp_reg;

                                TempData["INFO"] = null;
                                TempData["ERROR"] = null;

                                if (res.successMessage != null)
                                {
                                    ViewBag.Analysis =               res;
                                    TempData["INFO"] =               res.successMessage;
                                    TempData["ANALYSIS"] =           res;
                                    TempData["masterSupplierList"] = master_supplierlist;

                                    if (res.successMessage == "Error")
                                    {
                                        List<string> errors =       new List<string>();
                                        TempData["ERROR"] =         res.errMessage;
                                        string[] errMessage =       res.errMessage.Split('|');
                                        foreach (string s in errMessage)
                                        {
                                            errors.Add(s);
                                        }
                                        ViewBag.Errors = errors;
                                        nextPage = false;
                                        ViewBag.Nextpage = "false";
                                    }

                                    nhhValidator nhhValidator = new nhhValidator();
                                    var validation_result =     nhhValidator.nhhValidation(res);
                                    if (validation_result.Count > 0)
                                    {
                                        TempData["ERROR"] = "Errors";
                                        ViewBag.Errors =    validation_result;
                                        nextPage =          false;
                                        ViewBag.Nextpage =  "false";
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
                                    nextPage =          false;
                                    ViewBag.Nextpage = "false";
                                }
                            }
                            else
                            {
                                nextPage = false; ViewBag.Nextpage = "false";
                                TempData["ERROR"] =                  "Upload not parsed. Please check the Energy Type and Uploaded file";
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

                if (tmpAnalysis.TrnERMTenderDetails.Count>0)
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
    }

}
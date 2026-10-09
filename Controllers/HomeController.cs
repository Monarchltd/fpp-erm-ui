using DotNet8.WebApi.Factory.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using System.Net.Http.Json;
using static System.Net.Mime.MediaTypeNames;
using System.Text.Json;
using fppErm.Sessions;
using System.Configuration;
using System.IO;
using DotNet8.WebApi.Factory.Helpers;
using fppErm.Models;

namespace fppErm.Controllers
{
    public class HomeController : Controller
    {
        private string _baseurl = null;

        public HomeController()
        {
            _baseurl = ConfigurationManager.AppSettings["DEVorPROD"].Equals("dev") ? ConfigurationManager.AppSettings["DEV-API"] : ConfigurationManager.AppSettings["PROD-API"];
        }

        public ActionResult Index()
        {
            TempData["actClicked"] = "false";
            string pagevalue =    Request.Form["routeVals"];

            ViewBag.Username =    TempData["username"];

            HttpClient client   = new HttpClient();
            client.BaseAddress  = new Uri(_baseurl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync("Tender").Result;

            ViewBag.Tenders = null;

            if (response.IsSuccessStatusCode)
            {
                var tenders     = response.Content.ReadFromJsonAsync<IEnumerable<MasErmTenderHeaderModel>>().Result;

                
                foreach(var tndr in tenders)
                {
                    switch (tndr.MasErmTenderModel_s.supplytype)
                    {
                        case "1": tndr.MasErmTenderModel_s.supplytype = "Ele-HH"; break;
                        case "2": tndr.MasErmTenderModel_s.supplytype = "Ele-NHH";  break;
                        case "3": tndr.MasErmTenderModel_s.supplytype = "Gas";      break;
                        default:  tndr.MasErmTenderModel_s.supplytype = "Error";    break;
                    }
                }
                
                ViewBag.Tenders = tenders;
            }
            else
            {
                //MessageBox.Show("Error Code" + response.StatusCode + " : Message - " + response.ReasonPhrase);
            }

            //Status.
            List<ERMStatuses> status = new List<ERMStatuses>();

            status.Add(new ERMStatuses { id = 0, status = "Tender_Created" });
            //status.Add(new ERMStatuses { id = 1, status = "Tender_Sent" });
            status.Add(new ERMStatuses { id = 2, status = "Tender_Recalled" });
            //status.Add(new ERMStatuses { id = 3, status = "Tender_Repricing_Sent" });
            status.Add(new ERMStatuses { id = 4, status = "Tender_Cancelled" });
            status.Add(new ERMStatuses { id = 5, status = "Tender_Signed" });
            status.Add(new ERMStatuses { id = 6, status = "Tender_Repriced" });

            var statusList = status.Select(x => new SelectListItem { Value = x.id.ToString(), Text = x.status }).ToList();
            ViewBag.statusList = statusList;

            TempData.Keep();

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Login()
        {
            string username = Request["username"]; // "Senthil.Dhesign";
            string password = Request["password"]; //"GardenTree88";
            string domain =   Request["domain"];

            bool res = false;

            try
            {
                if (username != null && password != null)
                {
                    LoginValidation login = new LoginValidation();
                    res = login.getDetails(username, "", domain, password);
                    if (res)
                    {
                        TempData["username"] = username;
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            catch (Exception e)
            {

            }
            finally
            {

            }

            return View();
        }

    }
}
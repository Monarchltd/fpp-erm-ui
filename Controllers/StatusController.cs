using DotNet8.WebApi.Factory.Model;
using fppErm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using fppErm.Helpers;
using System.Reflection;
using System.Text;

namespace fppErm.Controllers
{
    public class StatusController : Controller
    {
        private HttpClient _client = null;
        private string _baseurl = null;

        public StatusController()
        {
            _baseurl = ConfigurationManager.AppSettings["DEVorPROD"].Equals("dev") ? ConfigurationManager.AppSettings["DEV-API"] : ConfigurationManager.AppSettings["PROD-API"];
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_baseurl);

        }

        [HttpPost]
        // GET: Status
        public ActionResult StatusPage()
        {
            var tender_id =             Request.Form["tenderId"];
            var status_update =         Request.Form["statusUpdate"];
            TempData["iconClicked"] =   "status";
            TempData["TenderId"] =      tender_id;

            var statusId =              Request.Form["statusId"];

            if (status_update.Equals("true"))
            {
                TempData["iconClicked"] = "none";
                TrnERMTenderStatus_1 model = new TrnERMTenderStatus_1();

                model.id =              0;
                model.MasErmTender =    tender_id != null ? Convert.ToDouble(tender_id.ToString()) : 0;
                model.tenderref =       "";
                model.StatusId =        Convert.ToInt32(statusId);
                model.StatusDesc =      Enum.GetName(typeof(StatusMessages), Convert.ToInt32(statusId));
                model.SubStatusId =     0;
                model.SubStatusDesc =   "";
                model.CurrentState =    0;
                model.cusr =            TempData["username"] != null ? TempData["username"].ToString() : "";
                model.cdte =            DateTime.Now;
                model.uusr =            "";
                model.udte =            DateTime.MinValue;
                model.isActive =        1;


                JsonContent content = JsonContent.Create(model);

                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                try
                {
                    if (model.MasErmTender != 0)
                    {
                        TempData["TenderId"] = 0;
                        HttpResponseMessage response = _client.PostAsync("diTrnERMTenderStatus/InsertStatus", content).Result;
                        var res = response.Content.ReadFromJsonAsync<TrnERMTenderStatus_1>().Result;
                    }
                }
                catch (Exception ex)
                {
                }
            }
            else
            {
                HttpResponseMessage response = _client.GetAsync("diTrnERMTenderStatus/GetStatus?id=" + tender_id).Result;
                var res = response.Content.ReadFromJsonAsync<IEnumerable<TrnERMTenderStatus_1>>().Result;

                StringBuilder sb = new StringBuilder();

                foreach (TrnERMTenderStatus_1 s in res)
                {
                    sb.AppendLine("Created by " + s.cusr.ToString() + " @ " + s.cdte.ToString());
                    sb.AppendLine(s.StatusDesc.ToString().Trim());
                    sb.AppendLine(" ");
                }

                TempData["statusHistory"] = sb.ToString();

            }



            return RedirectToAction("Index", "Home");
        }
    }
}
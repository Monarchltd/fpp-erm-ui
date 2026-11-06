
using System.Net.Http.Headers;
using System.Net.Http;
using System;
using System.Web.Mvc;
using System.Configuration;
using System.Net.Http.Json;
using System.Reflection;
using DotNet8.WebApi.Factory.Model;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using Microsoft.Ajax.Utilities;

namespace fppErm.Controllers
{
    public class NotesController : Controller
    {

        private HttpClient _client = null;
        private string _baseurl = null;

        public NotesController()
        {
            _baseurl = ConfigurationManager.AppSettings["DEVorPROD"].Equals("dev") ? ConfigurationManager.AppSettings["DEV-API"] : ConfigurationManager.AppSettings["PROD-API"];
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_baseurl);

        }

        [HttpPost]
        public ActionResult NotesPage()
        {
            var tender_id =             Request.Form["tenderId"] != null ? Request.Form["tenderId"].ToString().Trim().Length > 0 ? Request.Form["tenderId"] : Request.QueryString["tender_id"] : "0";
            var notes_new =             Request.Form["notesNew"];
            var notes_update =          Request.Form["notesUpdate"];
            var notes_his =             Request.Form["notesHistory"];
            var cnbno =                 Request.Form["cnbNo"] != null ? Request.Form["cnbNo"].ToString().Trim().Length > 0 ? Request.Form["cnbNo"] : Request.QueryString["cnbNo"] : "0";
            
            //TempData["notes"] =       "False";
            TempData["tid"] =           tender_id != null ? tender_id : "";
            TempData["TenderId"] =      tender_id != null ? tender_id : "";
            TempData["iconClicked"] =   "notes";
            TempData["CnbNo"] =         cnbno;


            if (notes_update.Equals("true"))
            {
                //TempData["notes"] = "False";
                TempData["iconClicked"] =   "none";
                TrnERMTenderNotes_1 model = new TrnERMTenderNotes_1();
                model.id =                  0;
                model.MasErmTender_id =     TempData["TenderId"] != null ? TempData["TenderId"].ToString().Trim().Length > 0 ? Convert.ToDouble(TempData["TenderId"].ToString()) : 0 : 0;
                model.tenderref =           "";
                model.notes =               notes_new.ToString();
                model.cusr =                TempData["username"] != null ? TempData["username"].ToString() : "";
                model.cdte =                DateTime.Now;
                model.uusr =                "";
                model.udte =                DateTime.MinValue;
                model.isActive =            1;


                JsonContent content =   JsonContent.Create(model);

                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                try
                {
                    if (model.MasErmTender_id != 0)
                    {
                        TempData["TenderId"] =          0;
                        HttpResponseMessage response =  _client.PostAsync("Notes/InsertNotes", content).Result;
                        var res =                       response.Content.ReadFromJsonAsync<TrnERMTenderNotes_1>().Result;
                    }
                }
                catch (Exception ex)
                {
                }
            }
            else
            {
                //TempData["notes"] =             "True";
                HttpResponseMessage response =  _client.GetAsync("Notes/GetNotes?id=" + tender_id).Result;
                var res =                       response.Content.ReadFromJsonAsync<IEnumerable<TrnERMTenderNotes_1>>().Result;
                StringBuilder sb =              new StringBuilder();

                foreach (TrnERMTenderNotes_1 s in res)
                {
                    sb.AppendLine("-- " + s.cusr.ToString() + " @ " + s.cdte.ToString().Substring(0, 16));
                    sb.AppendLine(s.notes.ToString().Trim());
                    sb.AppendLine(" ");
                }

                TempData["History"] = sb.ToString();
            }

            return RedirectToAction("Index", "Home", new { tender_id = tender_id});
        }

        [HttpGet]
        public ActionResult NotesDetails(string id)
        {
            return RedirectToAction("Index", "Home", new { tenderid = id});
        }
    }
}


using System;
using System.Net.Http;
using System.Web.Mvc;
using System.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using DotNet8.WebApi.Factory.Model;
using fppErm.Helpers;

using System.IO;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace fppErm.Controllers
{
    public class SendController : Controller
    {
        private HttpClient _client = null;
        private string _baseurl = null;

        public SendController()
        {
            _baseurl = ConfigurationManager.AppSettings["DEVorPROD"].Equals("dev") ? ConfigurationManager.AppSettings["DEV-API"] : ConfigurationManager.AppSettings["PROD-API"];
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_baseurl);

        }
        public ActionResult ViewSend()
        {
            string tender_id = Request.Form["tenderId"].ToString();
            ViewBag.Username = TempData["username"];

            ViewBag.id = tender_id;

            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                HttpResponseMessage response =  _client.GetAsync("getTenderDetails/getTender?id=" + Convert.ToDouble(tender_id)).Result;
                var res =                       response.Content.ReadFromJsonAsync<viewERMTenderModel>().Result;
                nhh nhh = new nhh();
                ViewBag.nhhHeader =             nhh.nhhHeaders;
                ViewBag.Result =                res;
            }
            catch (Exception ex)
            {
            }

            TempData["username"] =      ViewBag.Username;
            TempData["iconClicked"] =   "";

            return View();

        }

        public ActionResult SendMail()
        {
            ViewBag.MailSent = false;
            string tender_id = Request.Form["tenderId"] != null ? Request.Form["tenderId"].ToString() : "0";
            ViewBag.Username = TempData["username"];

            ViewBag.id = tender_id;

            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                TempData["username"] =      ViewBag.Username;
                TempData["iconClicked"] =   "";

                if (Convert.ToInt32(tender_id) > 0)
                {
                    HttpResponseMessage response = _client.GetAsync("getTenderDetails/getTender?id=" + Convert.ToDouble(tender_id)).Result;
                    var res = response.Content.ReadFromJsonAsync<viewERMTenderModel>().Result;
                    TempData["sendEmail"] = res;
                    nhh nhh = new nhh();
                    ViewBag.nhhHeader = nhh.nhhHeaders;
                    ViewBag.Result = res;
                    ViewBag.Sitecount = res.ermDetailNHH.Count;

                    string fueltype = res.ermTender.fueltype.ToString();

                    if (fueltype.Substring(0, 2) == "BR")
                        fueltype = "Brown";
                    if (fueltype.Substring(1, 2) == "GR")
                        fueltype = "Green";
                    if (fueltype.Substring(3, 2) == "BR")
                        fueltype = "Brown";

                    ViewBag.fueltype = fueltype;

                }
                else
                {
                    string[] emails =   { "SenthilRaja.Dhesign@welcomeenergy.co.uk", "Tamzyn.Elliott-Pullen@eic.co.uk", "Guy.Macdougall-Mitchell@eic.co.uk" };

                    string[] splittext = ViewBag.Username.Split('.');

                    string searchname = ViewBag.Username;

                    if (splittext.Length > 0)
                    {
                        searchname = ViewBag.Username.Split('.')[0];
                    }

                    string mailto =     emails.Where(i => i.ToLower().Contains(searchname.ToLower())).FirstOrDefault();

                    var res =           (viewERMTenderModel)TempData["sendEmail"];
                    ViewBag.Result =    res;
                    string guid =       res.ermHeader.TenderRef.ToString();
                    var uploadPath =    Path.Combine(AppDomain.CurrentDomain.BaseDirectory + @"\Uploads", guid);
                    string subject =    CreateItemFromTemplate(res, uploadPath);
                    string spath =      "sendEmail/sndEmail?foldername=" + Uri.EscapeDataString(uploadPath) + 
                                        "&subjectline=" + subject + 
                                        "&mailto=" + mailto +
                                        "&tenderid=" + res.ermTender.id +
                                        "&username=" + ViewBag.Username;
                    HttpResponseMessage sendEmailresponse = _client.GetAsync(spath).Result;

                    ViewBag.MailSent = true;
                }

            }
            catch (System.Exception ex)
            {
            }

            return View();

        }

        
        public string CreateItemFromTemplate(viewERMTenderModel emailData, string folderpath)
        {
            string retval = "";
            try
            {
                StringBuilder emailTemplate = new StringBuilder();
                emailTemplate.AppendLine("<html><body><p><b>[TEXT-1]</b></p>");
                emailTemplate.AppendLine("<p><b>[TEXT-2]</b></p>");
                emailTemplate.AppendLine("<p>Company Number: [TEXT-3]</p>");
                emailTemplate.AppendLine("<p>Price Under [TEXT-4]</p>");
                emailTemplate.AppendLine("<p>[TEXT-5]</p>");
                emailTemplate.AppendLine("<p><b>[TEXT-6]</b></p>");
                emailTemplate.AppendLine("<p>[TEXT-7]</p>");
                emailTemplate.AppendLine("<table>");
                emailTemplate.AppendLine("<tr><td>Payment term(s):</td><td>[TEXT-8]</td></tr>");
                emailTemplate.AppendLine("<tr><td>Payment duration(s):</td><td>[TEXT-9]</td></tr>");
                emailTemplate.AppendLine("<tr><td>Energy type(s):</td><td>[TEXT-10]</td></tr>");
                emailTemplate.AppendLine("<tr><td>Contract type(s):</td><td>Fully Fixed including all pass through charges</td></tr>");
                emailTemplate.AppendLine("<tr><td>Contract Start Date:</td><td>[TEXT-11]</td></tr>");
                emailTemplate.AppendLine("<tr><td>Contract Duration(s):</td><td>End Date [TEXT-12]</td></tr>");
                emailTemplate.AppendLine("<tr><td>Included commission:</td><td>[TEXT-13]</td></tr>");
                emailTemplate.AppendLine("<tr><td>Micro Business</td><td>[TEXT-14]</td></tr>");
                emailTemplate.AppendLine("<tr><td>DC/DA Agent (HH, P272 and AMR):</td><td>[TEXT-15]</td></tr>");
                emailTemplate.AppendLine("<tr><td>Billing</td><td>Original paper invoices to be sent to EIC partnership   Head Office.</td></tr>");
                emailTemplate.AppendLine("<tr><td></td><td>E-bills to be emailed to e.billing@eic.co.uk<br/>Online access to copy invoices to be provided to EIC partnership</td></tr>");
                emailTemplate.AppendLine("<table>");
                emailTemplate.AppendLine("<p><b>Can you please supply EAC and Take or Pay conditions, when supplying your prices.</b></p>");
                emailTemplate.AppendLine("<p><b>Please confirm that you can register and support smets2 meters also Targeted Charging Review (TCR) is included in this quote.</b></p>");
                emailTemplate.AppendLine("<p><b>Submission</b></p>");
                emailTemplate.AppendLine("<table>");
                emailTemplate.AppendLine("<tr><td>Tenders should be emailed to:</td><td>EnergyTendersTeam@eic.co.uk</td></tr>");
                emailTemplate.AppendLine("<tr><td>Submission deadline:</td><td>[TEXT-16]</td></tr>");
                emailTemplate.AppendLine("<table><br/>");
                emailTemplate.AppendLine("<p>Please include your latest terms and conditions and ensure that you provide your most competitive price as you may not get another opportunity to re-tender. All submissions including re-quotes must comply with the requirements in this ITT, any variations made by EIC partnership   will be confirmed in writing only. Any late, incomplete, incorrect or non-confirming tenders may be disqualified by EIC partnership   without further notice.</p>");
                emailTemplate.AppendLine("<p>By submitting an offer for this ITT you agree that your submission is bona-fide, correct and complies with the requirement. The requirement within this ITT will prevail over any supply terms and conditions provided unless non-conformances are clearly highlighted and accepted in writing by EIC partnership</p>");
                emailTemplate.AppendLine("<p>Kind regards,</p></body></html>");

                emailTemplate.Replace("[TEXT-1]", "Dear Sir/Madam,");

                foreach (var itm in emailData.ermSuppliers)
                {
                    retval = itm.InvitationToTender;
                }

                string analysisaq =     emailData.ermTender.analysisaq.Equals("MONAR") ? "Monarch" : emailData.ermTender.analysisaq.Equals("SUPPL") ? "Supplier" : "Half-Hourly Data";
                string paymentterms =   emailData.ermRequirements.PaymentTerms == "1" ? "BACS" : "DD";
                string fueltype =       emailData.ermTender.fueltype.Substring(0, 2) == "BR" ? "Brown" : emailData.ermTender.fueltype.Substring(2, 2) == "GR" ? "Green" : "Carbon Zero Gas - Ecotricity";

                emailTemplate.Replace("[TEXT-2]",   retval);
                emailTemplate.Replace("[TEXT-3]",   emailData.ermTender.comp_reg);
                emailTemplate.Replace("[TEXT-4]",   analysisaq);
                emailTemplate.Replace("[TEXT-5]",   "EIC partnership has been appointed by Futures to assist with its energy procurement requirements. You have been invited to participate in this mini-competition for the provision of electricity supplies under Lot 1 within the DPS agreement referenced above.");
                emailTemplate.Replace("[TEXT-6]",   "Requirements");
                emailTemplate.Replace("[TEXT-7]",   "For the sites listed in the attached schedule please provide quotations for supply including the following options:");
                emailTemplate.Replace("[TEXT-8]",   paymentterms);
                emailTemplate.Replace("[TEXT-9]",   emailData.ermRequirements.ContractDuration + " days");
                emailTemplate.Replace("[TEXT-10]",  fueltype);
                emailTemplate.Replace("[TEXT-11]",  emailData.ermRequirements.Contract_startdate.ToString("dd/MM/yyyy"));
                emailTemplate.Replace("[TEXT-12]",  emailData.ermRequirements.Contract_enddate.ToString("dd/MM/yyyy"));
                emailTemplate.Replace("[TEXT-13]",  emailData.ermRequirements.IncludedCommission);
                emailTemplate.Replace("[TEXT-14]",  "No");
                emailTemplate.Replace("[TEXT-15]",  "Stark (supplier nomination)");
                emailTemplate.Replace("[TEXT-16]",  emailData.ermRequirements.Submission_deadline.ToString("dd/MM/yyyy"));


                using (StreamWriter sw = new StreamWriter(folderpath + "\\email.txt"))
                {
                    sw.WriteLine(emailTemplate.ToString());
                }

            }
            catch (Exception ex)
            {

            }
            return retval;
        }

    }
}
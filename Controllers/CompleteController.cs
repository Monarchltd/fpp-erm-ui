using DotNet8.WebApi.Factory.Model;
using System;

using System.IO;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using fppErm.Models;
using System.Collections.Generic;
using System.Linq;
using fppErm.Helpers;
using static Azure.Core.HttpHeader;


namespace fppErm.Controllers
{
    public class CompleteController : Controller
    {

        private string _baseurl = null;

        public CompleteController()
        {
            _baseurl = ConfigurationManager.AppSettings["DEVorPROD"].Equals("dev") ? ConfigurationManager.AppSettings["DEV-API"] : ConfigurationManager.AppSettings["PROD-API"];
        }


        // GET: Complete
        [HttpPost]
        public ActionResult CompletePage(HttpPostedFileBase req_uploadFile_1, HttpPostedFileBase req_uploadFile_2, HttpPostedFileBase req_uploadFile_3, HttpPostedFileBase req_uploadFile_4)
        {
            string req_comment =            Request.Form["req_comment"];
            string req_companyreg =         Request.Form["req_companyreg"];
            string req_energytype =         Request.Form["req_energytype"];
            string req_contractduration =   Request.Form["req_contractduration"];
            string req_paymentterms =       Request.Form["req_paymentterms"];
            string req_contracttype =       Request.Form["req_contracttype"];
            string req_includedcommission = Request.Form["req_includedcommission"];
            string req_paymentduration =    Request.Form["req_paymentduration"];
            string req_contractstartdate =  Request.Form["req_contractstartdate"];
            string req_contractenddate =    Request.Form["req_contractenddate"];
            string req_dcda =               Request.Form["req_dcda"];
            string req_backbydate =         Request.Form["req_backbydate"];
            string billing =                Request.Form["req_billingtxt"];
            string req_subdeadline =        Request.Form["req_subdeadline"];
            string req_invitationtotender = Request.Form["req_invitationtotender"];
            string req_supplieremail =      Request.Form["req_supplieremail"];

            nhh n =                         new nhh();
            ViewBag.nhhHeader =             n.nhhHeaders;
            var suppliers =                 (List<SuppSubmissionModel>)TempData["SuppSubmissionModel"];

            //ViewBag.Analysis =              (MasErmTenderModel_1)TempData["ANALYSIS"];
            ViewBag.Analysis =              (MasErmTenderModel_3)TempData["ANALYSIS"];

            string username =               TempData["username"] != null ? TempData["username"].ToString() : "System";

            var newTender =                 (NewTenderModel)TempData["NEWTENDER"];

            var model =                     (MasErmTenderModel_3)TempData["ANALYSIS"];

            //Upload file.
            string guid =                   model.MasErmTenderHeaderModel.TenderRef; //Guid.NewGuid().ToString(); 
            var uploadPath =                Path.Combine(AppDomain.CurrentDomain.BaseDirectory + @"\Uploads", guid);

            string upload_file1 =           req_uploadFile_1 != null ? req_uploadFile_1.FileName : "";
            string upload_file2 =           req_uploadFile_2 != null ? req_uploadFile_2.FileName : "";
            string upload_file3 =           req_uploadFile_3 != null ? req_uploadFile_3.FileName : "";
            string upload_file4 =           req_uploadFile_4 != null ? req_uploadFile_4.FileName : "";

            TrnERMRequirements trnERMRequirements = new TrnERMRequirements();
            trnERMRequirements.EnergyType =         req_energytype;
            trnERMRequirements.ContractDuration =   req_contractduration;
            trnERMRequirements.PaymentTerms =       req_paymentterms;
            trnERMRequirements.ContractType =       req_contracttype;
            trnERMRequirements.IncludedCommission = req_includedcommission;
            trnERMRequirements.PaymentDuration =    req_paymentduration;
            trnERMRequirements.Contract_startdate = Convert.ToDateTime(req_contractstartdate);
            trnERMRequirements.Contract_enddate =   Convert.ToDateTime(req_contractenddate);
            trnERMRequirements.Dcda_agent =         req_dcda;
            trnERMRequirements.Backbydate =         Convert.ToDateTime(req_backbydate);
            trnERMRequirements.Billing =            String.Empty; // billing.Trim();
            trnERMRequirements.Submission_deadline =Convert.ToDateTime(req_subdeadline);
            ViewBag.Requirements =                  trnERMRequirements;
            ViewBag.ReqEnergyTyp =                  ViewBag.Requirements.EnergyType == "1" ? "Electric" : ViewBag.Requirements.EnergyType == "2" ? "Gas" : "Water";

            //Check for the folder with tender_ref
            if (Directory.Exists(uploadPath) && 
                (upload_file1 != null || 
                 upload_file2 != null || 
                 upload_file3 != null || 
                 upload_file4 != null))
            {
                try
                {
                    var upfile = Path.GetFileName(upload_file1);
                    var file_to_upload = Path.Combine(uploadPath, upfile);
                    if (upload_file1.Length > 0)
                        req_uploadFile_1.SaveAs(file_to_upload);

                    upfile = Path.GetFileName(upload_file2);
                    file_to_upload = Path.Combine(uploadPath, upfile);
                    if (upload_file2.Length > 0)
                        req_uploadFile_2.SaveAs(file_to_upload);

                    upfile = Path.GetFileName(upload_file3);
                    file_to_upload = Path.Combine(uploadPath, upfile);
                    if (upload_file3.Length > 0)
                        req_uploadFile_3.SaveAs(file_to_upload);

                    upfile = Path.GetFileName(upload_file4);
                    file_to_upload = Path.Combine(uploadPath, upfile);
                    if (upload_file4.Length > 0)
                        req_uploadFile_4.SaveAs(file_to_upload);
                }
                catch (UnauthorizedAccessException ex)
                {
                }
                finally
                {
                }
            }

            HttpClient client =                 new HttpClient();
            client.BaseAddress =                new Uri(_baseurl);

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            string clientname =                     model.MasErmClientModel.name.ToLower();
            model.MasErmClientModel.id =            clientname.Equals("the eic partnership") ? 2 : clientname.Equals("the monarch partnership") ? 1 : 0;
            //model.MasErmClientModel.address1 =      "Monarch House, 7-9 Stafford Rd";
            //model.MasErmClientModel.address2 =      "Wallington ";
            //model.MasErmClientModel.postcode =      "SM6 9AN";
            //model.MasErmClientModel.email =         "help@monarchpartnership.co.uk";

            //STEP 1: Insert the data to MasERMNetwork DB.
            model.MasErmNetworkModel.id =             0;
            model.MasErmNetworkModel.clientid =       model.MasErmClientModel.id;
            model.MasErmNetworkModel.clientref =      "";
            string[] address =                        model.MasErmNetworkModel.headaddress1.Split(',');
            model.MasErmNetworkModel.headaddress1 =   address.Length > 1 ? address[0] + address[1] + address[2] : "";
            model.MasErmNetworkModel.headaddress2 =   address.Length > 3 ? address[3] + address[4] : "";
            model.MasErmNetworkModel.headpostcode =   address.Length > 4 ? address[4] : "";
            model.MasErmNetworkModel.heademail =      "";
            address =                                 model.MasErmNetworkModel.corraddress1.Split(',');
            model.MasErmNetworkModel.corraddress1 =   address.Length > 1 ? address[0] + address[1] + address[2] : "";
            model.MasErmNetworkModel.corraddress2 =   address.Length > 3 ? address[3] + address[4] : "";
            model.MasErmNetworkModel.corrpostcode =   address.Length > 4 ? address[4] : "";
            model.MasErmNetworkModel.corremail =      "";
            model.MasErmNetworkModel.isActive =       1;

            JsonContent content =                     JsonContent.Create(model.MasErmNetworkModel);
            HttpResponseMessage response =            client.PostAsync("diMasERMNetwork", content).Result;

            var res_masermnetwork =                   response.Content.ReadFromJsonAsync<MasErmNetworkModel>().Result;

            foreach(var s in suppliers)
            {
                model.supplier = s.id.ToString();
            }

            MasErmTenderModel_2 masErmTenderModel_2 = new MasErmTenderModel_2();
            masErmTenderModel_2.id =                  model.id;
            masErmTenderModel_2.network_id =          res_masermnetwork.id;
            masErmTenderModel_2.consultantname =      model.consultantname;
            masErmTenderModel_2.consultantemail =     model.consultantemail;
            masErmTenderModel_2.supplytype =          model.supplytype;
            masErmTenderModel_2.notes =               model.notes;
            masErmTenderModel_2.commission =          model.commission;
            masErmTenderModel_2.commission_type =     model.commission_type;
            masErmTenderModel_2.supplier =            model.supplier;
            masErmTenderModel_2.comp_reg =            req_companyreg; // model.comp_reg;
            masErmTenderModel_2.newlondps =           model.newlondps;
            masErmTenderModel_2.fueltype =            model.fueltype;
            masErmTenderModel_2.enddatelength =       req_contractduration; //model.enddatelength;
            masErmTenderModel_2.analysisaq =          model.analysisaq;
            masErmTenderModel_2.fillers =             model.fillers;
            masErmTenderModel_2.mopagreement =        model.mopagreement;
            masErmTenderModel_2.status =              model.status;
            masErmTenderModel_2.version =             model.version;
            masErmTenderModel_2.cusr =                username;
            masErmTenderModel_2.cdte =                DateTime.Now;
            masErmTenderModel_2.uusr =                model.uusr;
            masErmTenderModel_2.udte =                model.udte;
            masErmTenderModel_2.isActive =            1;

            masErmTenderModel_2.id =                  1;
            masErmTenderModel_2.uusr =                "";

            content =                                 JsonContent.Create(masErmTenderModel_2);
            response =                                client.PostAsync("DataInsert", content).Result;
            var res =                                 response.Content.ReadFromJsonAsync<MasErmTenderModel_2>().Result;

            ViewBag.NewTender =                       res;
            ViewBag.Cnbno =                           newTender.uploadPage.cnbno;


            //Update MasERNTenderHeader after getting the Tender Id
            MasErmTenderHeaderModel_1 masErmTenderHeaderModel_1 =   new MasErmTenderHeaderModel_1();
            masErmTenderHeaderModel_1.CnbNo =                       newTender.uploadPage.cnbno; // model.cnbno;
            masErmTenderHeaderModel_1.AccountId =                   model.MasErmClientModel.id.ToString(); // This should be client id
            masErmTenderHeaderModel_1.TenderId =                    res.id.ToString();
            masErmTenderHeaderModel_1.TenderRef =                   guid;
            masErmTenderHeaderModel_1.StatusId =                    res.status.ToString();
            masErmTenderHeaderModel_1.VersionId =                   res.version.ToString();
            masErmTenderHeaderModel_1.CreatedBy =                   res.cusr;
            masErmTenderHeaderModel_1.CreatedDate =                 DateTime.Now;
            masErmTenderHeaderModel_1.uusr =                        "";
            masErmTenderHeaderModel_1.udte =                        model.udte;
            masErmTenderHeaderModel_1.isActive =                    1;

            content =                                               JsonContent.Create(masErmTenderHeaderModel_1);
            response =                                              client.PostAsync("diMasERMTenderHeader", content).Result;
            var res2 =                                              response.Content.ReadFromJsonAsync<MasErmTenderHeaderModel_1>().Result;

            //Update Notes.
            TrnERMTenderNotes_1 TenderNotes =                       new TrnERMTenderNotes_1();
            TenderNotes.id =                                        0;
            TenderNotes.MasErmTender_id =                           res.id;
            TenderNotes.tenderref =                                 "";
            TenderNotes.notes =                                     req_comment.ToString();
            TenderNotes.cusr =                                      username != null ? username : "System";
            TenderNotes.cdte =                                      DateTime.Now;
            TenderNotes.uusr =                                      "";
            TenderNotes.udte =                                      DateTime.MinValue;
            TenderNotes.isActive =                                  1;
            content =                                               JsonContent.Create(TenderNotes);
            response =                                              client.PostAsync("Notes/InsertNotes", content).Result;
            var res3 =                                              response.Content.ReadFromJsonAsync<TrnERMTenderNotes_1>().Result;
            ViewBag.Notes =                                         res3;

            //Requirements.
            trnERMRequirements.MasErmTender =                       res.id;
            trnERMRequirements.cusr =                               username;
            trnERMRequirements.cdte =                               DateTime.Now;
            trnERMRequirements.uusr =                               "";
            trnERMRequirements.isActive =                           1;
            content =                                               JsonContent.Create(trnERMRequirements);
            response =                                              client.PostAsync("diTrnERMRequirements/InsertRequirements", content).Result;
            var res4 =                                              response.Content.ReadFromJsonAsync<TrnERMRequirements>().Result;

            //Uploads.
            List<TrnERMRequirementsUploads> req_upload = new List<TrnERMRequirementsUploads>();
            req_upload.Add(new TrnERMRequirementsUploads { MasErmTender = res.id, MasErmRequirements = res4.id, tenderref = guid, heading = "HH Data", upload_file = upload_file1, cusr = username, cdte = DateTime.Now, uusr="", udte= DateTime.MinValue, isActive=1 });
            req_upload.Add(new TrnERMRequirementsUploads { MasErmTender = res.id, MasErmRequirements = res4.id, tenderref = guid, heading = "Monarch Terms and Condition of Tender", upload_file = upload_file2, cusr = username, cdte = DateTime.Now, uusr = "", udte = DateTime.MinValue, isActive = 1 });
            req_upload.Add(new TrnERMRequirementsUploads { MasErmTender = res.id, MasErmRequirements = res4.id, tenderref = guid, heading = "MOP Agreement", upload_file = upload_file3, cusr = username, cdte = DateTime.Now, uusr = "", udte = DateTime.MinValue, isActive = 1 });
            req_upload.Add(new TrnERMRequirementsUploads { MasErmTender = res.id, MasErmRequirements = res4.id, tenderref = guid, heading = "LOA", upload_file = upload_file4, cusr = username, cdte = DateTime.Now, uusr = "", udte = DateTime.MinValue, isActive = 1 });
            req_upload.Add(new TrnERMRequirementsUploads { MasErmTender = res.id, MasErmRequirements = res4.id, tenderref = guid, heading = "Meter File Upload", upload_file = newTender.uploadPage.uploadedFile.FileName, cusr = username, cdte = DateTime.Now, uusr = "", udte = DateTime.MinValue, isActive = 1 });
            ViewBag.Uploads = req_upload;
            content = JsonContent.Create(req_upload);
            response = client.PostAsync("diTrnERMRequirementsUpload/InsertUploads", content).Result;

            //Supplier.
            var supplierList =                                        (List<SelectListItem>)TempData["supplierList"];
            TrnERMRequirementsSuppliers trnERMRequirementsSuppliers = new TrnERMRequirementsSuppliers();
            foreach (var item in suppliers)
            {
                trnERMRequirementsSuppliers.MasErmTender =          res.id;
                trnERMRequirementsSuppliers.MasErmRequirements =    res4.id;
                trnERMRequirementsSuppliers.tenderref =             guid;
                trnERMRequirementsSuppliers.SupplierId =            Convert.ToDouble(item.id);
                trnERMRequirementsSuppliers.InvitationToTender =    req_invitationtotender; // item.invitation_subject;
                trnERMRequirementsSuppliers.EmailIds =              item.emailids;
                ViewBag.Supplier =                                  supplierList.Where(x => x.Value == item.id.ToString().Trim()).Select(x => x.Text).ToList()[0];
                trnERMRequirementsSuppliers.cusr =                  username;
                trnERMRequirementsSuppliers.cdte =                  DateTime.Now;
                trnERMRequirementsSuppliers.uusr =                  "";
                trnERMRequirementsSuppliers.udte =                  DateTime.MinValue;
                trnERMRequirementsSuppliers.isActive =              1;
                break;
            }
            ViewBag.SupplierList =                                  trnERMRequirementsSuppliers;
            content =                                               JsonContent.Create(trnERMRequirementsSuppliers);
            response =                                              client.PostAsync("diTrnERMRequirementsSuppliers/InsertSuppliers", content).Result;

            //Contract Duration.
            try
            {
                var contractDuration =                                                  (List<tmpStartEndDateModel>)TempData["tmpStartEndDateModel"];
                List<TrnERMRequirementsContractDuration> trnERMRequirementsContractDuration = new List<TrnERMRequirementsContractDuration>();

                foreach (var item in contractDuration)
                {
                    trnERMRequirementsContractDuration.Add(new TrnERMRequirementsContractDuration()
                    {
                        id =                    0,
                        MasErmTender =          res.id,
                        MasErmRequirements =    res4.id,
                        StartDate =             Convert.ToDateTime(item.startDate),
                        EndDate =               Convert.ToDateTime(item.endDate),
                        Duration =              item.duration,
                        cdte =                  DateTime.Now,
                        cusr =                  username,
                        udte =                  DateTime.MinValue,
                        uusr =                  "",
                        isActive =              1
                        
                    });
                }

                ViewBag.ContractDuration = trnERMRequirementsContractDuration;
                content = JsonContent.Create(trnERMRequirementsContractDuration);
                response = client.PostAsync("diTrnERMRequirementsContractduration/InsertDuration", content).Result;

            }
            catch (Exception ex)
            {

            }

            //Status.
            TrnERMTenderStatus_1 TrnERMTenderStatus =               new TrnERMTenderStatus_1();

            TrnERMTenderStatus.id =                                 0;
            TrnERMTenderStatus.MasErmTender =                       res.id;
            TrnERMTenderStatus.tenderref =                          "";
            TrnERMTenderStatus.StatusId =                           Convert.ToInt32(0);
            TrnERMTenderStatus.StatusDesc =                         Enum.GetName(typeof(StatusMessages), Convert.ToInt32(0));
            TrnERMTenderStatus.SubStatusId =                        0;
            TrnERMTenderStatus.SubStatusDesc =                      "";
            TrnERMTenderStatus.CurrentState =                       0;
            TrnERMTenderStatus.cusr =                               TempData["username"] != null ? TempData["username"].ToString() : "";
            TrnERMTenderStatus.cdte =                               DateTime.Now;
            TrnERMTenderStatus.uusr =                               "";
            TrnERMTenderStatus.udte =                               DateTime.MinValue;
            TrnERMTenderStatus.isActive =                           1;
            content =                                               JsonContent.Create(TrnERMTenderStatus);
            response =                                              client.PostAsync("diTrnERMTenderStatus/InsertStatus", content).Result;

            //MSN.
            //update the null fields to call the api.
            List<TrnERMTenderDetailsNHH> tenderAnalysis = new List<TrnERMTenderDetailsNHH>();

            try
            {
                tenderAnalysis =    model.TrnERMTenderDetails;

                //get the current supplier.
                var cSupplst  =     tenderAnalysis.AsEnumerable().Select(x => x.Supplier.Trim()).Distinct().ToList();
                var cSuppname =     String.Join(", ", cSupplst.ToArray());
                string cSuppid =    String.Empty;

                foreach (string s in cSupplst)
                {
                    cSuppid += supplierList.Where(x => x.Text.Contains(s)).Select(x => x.Value).FirstOrDefault().ToString() + ",";
                }

                ViewBag.CurrentSupplier = cSuppname;

                int idx = 0;
                while(idx < tenderAnalysis.Count)
                {
                    string suupliername =               tenderAnalysis[idx].Supplier.Trim();
                    tenderAnalysis[idx].Supplier =      supplierList.Where(x => x.Text.Contains(suupliername)).Select(x => x.Value).FirstOrDefault().ToString();
                    tenderAnalysis[idx].MasErmTender =  res.id;
                    tenderAnalysis[idx].cusr =          username;
                    tenderAnalysis[idx].uusr =          "";
                    tenderAnalysis[idx].isActive =      1;
                    idx++;
                }

                content =                                       JsonContent.Create(tenderAnalysis);
                response =                                      client.PostAsync("diTrnERMTenderDetails/InsertAnalysis", content).Result;
            }
            catch (Exception ex)
            {

            }

            TempData["username"] =  username;
            ViewBag.Username =      username;

            List<string> fueltype = new List<string>();
            if (newTender.uploadPage.fueltype != null)
            {
                if(newTender.uploadPage.fueltype.Substring(0, 2) == "BR")
                {
                    ViewBag.FuelType = "Brown";
                }
                
                if (newTender.uploadPage.fueltype.Substring(2, 2) == "GR")
                {
                    ViewBag.FuelType += ", Green";
                }

                if (newTender.uploadPage.fueltype.Substring(4, 2) == "CA")
                {
                    ViewBag.FuelType += ", Carbon Zero Gas - Ecotricity";
                }

                if (newTender.uploadPage.fueltype.Substring(6, 2) == "SG")
                {
                    ViewBag.FuelType += ", Select Green";
                }

            }

            switch (newTender.uploadPage.supplytype)
            {
                case "1": ViewBag.HhnhhGas = "Electric - Half Haourly"; break;
                case "2": ViewBag.HhnhhGas = "Electric - Non Half Haourly"; break;
                case "3": ViewBag.HhnhhGas = "Gas"; break;
                default: break;
            }

            return View();
        }
    }
}
using System.Configuration;
using System.Web.Mvc;

namespace fppErm.Controllers
{
    public class TendersController : Controller
    {

        // GET: Tenders
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult NewTender()
        {
            ViewBag.Username =  TempData["username"];
            ViewBag.Error =     TempData["ERROr"];
            ViewBag.Info =      TempData["INFO"];


            if (TempData["NewTender"] != null)
                ViewBag.NewTender = TempData["NewTender"];

            TempData.Clear();

            TempData["username"] =  ViewBag.Username;
            TempData["ERROr"] =     ViewBag.Error;
            TempData["INFO"] =      ViewBag.Info;

            return View();
        }
    }
}
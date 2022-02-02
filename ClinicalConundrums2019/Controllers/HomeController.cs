
using System;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using ClinicalConundrums2019.Models;
using ClinicalConundrums2019.Util;


namespace ClinicalConundrums2019.Controllers
{
    public class HomeController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            return Redirect("/Program/Index");
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

        public ActionResult ProgramMaterials()
        {

            return View();
        }


      

        public ActionResult ImplementationDocument()
        {
            var CurrentUser = UserHelper.GetLoggedInUser();
            if (Session["ProgramID"] != null)
            {

                if (CurrentUser != null)
                {
                    ViewBag.UserID = UserHelper.GetLoggedInUser().UserID;

                    ViewBag.ProgramRequestStatusCounts = UserHelper.GetProgramRequestStatusCounts(Convert.ToInt32(Session["ProgramID"]));

                    return View();
                }
                else
                {

                    return RedirectToAction("Login", "Account");
                }

            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
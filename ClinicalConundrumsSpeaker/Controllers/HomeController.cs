using ClinicalConundrumsSpeaker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClinicalConundrumsSpeaker.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            if (UserHelper.GetLoggedInUser() != null)

            {
                // UserHelper.ReloadUser();
                int UserID = UserHelper.GetLoggedInUser().UserID;
                ViewBag.UserID = UserID;
                ViewBag.RegistrationStatus = UserHelper.GetRegistrationStatus(UserID);
                ViewBag.FirstName = UserHelper.GetLoggedInUser().FirstName;
                ViewBag.LastName = UserHelper.GetLoggedInUser().LastName;


                return View();

            }
            else
                return RedirectToAction("Login", "Account");




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

        public ActionResult ScientificPlanningCommittee(int ProgramID)
        {
            if (UserHelper.GetLoggedInUser() != null)

            {
                // UserHelper.ReloadUser();
                int UserID = UserHelper.GetLoggedInUser().UserID;
                ViewBag.UserID = UserID;
                ViewBag.RegistrationStatus = UserHelper.GetRegistrationStatus(UserID);
                ViewBag.FirstName = UserHelper.GetLoggedInUser().FirstName;
                ViewBag.LastName = UserHelper.GetLoggedInUser().LastName;


                return View("ScientificCommittee_" + ProgramID);

            }
            else
                return RedirectToAction("Login", "Account");
        }
    }
}
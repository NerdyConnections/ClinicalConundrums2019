using ClinicalConundrumsSpeaker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClinicalConundrumsSpeaker.Controllers
{
    public class ProgramMaterialsController : Controller
    {
        public ActionResult Index(int ProgramID)
        {
            var CurrentUser = UserHelper.GetLoggedInUser();


            if (CurrentUser != null)
            {


                int UserID = UserHelper.GetLoggedInUser().UserID;
                ViewBag.UserID = UserID;
                ViewBag.RegistrationStatus = UserHelper.GetRegistrationStatus(UserID);
                ViewBag.FirstName = UserHelper.GetLoggedInUser().FirstName;
                ViewBag.LastName = UserHelper.GetLoggedInUser().LastName;

                return View("ProgramMaterials_" + ProgramID);
            }


            return RedirectToAction("Login", "Account");


        }
    }
}
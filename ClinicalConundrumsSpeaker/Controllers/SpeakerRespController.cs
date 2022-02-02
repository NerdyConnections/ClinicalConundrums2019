using ClinicalConundrumsSpeaker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClinicalConundrumsSpeaker.Controllers
{
    public class SpeakerRespController : Controller
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
                //they are the same page regardless of program SpeakerResp_1 fixed the bottom 3 links
                return View("SpeakerResp_" + ProgramID);
            }


            return RedirectToAction("Login", "Account");


        }
    }
}
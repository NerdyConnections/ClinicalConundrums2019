using ClinicalConundrums2019.DAL;
using ClinicalConundrums2019.Models;
using ClinicalConundrums2019.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClinicalConundrums2019.Controllers
{
    public class SpeakerController : Controller
    {
       
        public ActionResult Index()
        {
            if (Session["ProgramID"] != null)
            {

                ViewBag.ProgramRequestStatusCounts = UserHelper.GetProgramRequestStatusCounts(Convert.ToInt32(Session["ProgramID"]));
                ViewBag.ProgramID = Session["ProgramID"];
            }
            else
                return RedirectToAction("Index", "Home");
            return View();
        }

        public ActionResult NewSpeaker()
        {
            return View();
        }


        [HttpPost]
        public JsonResult NewSpeaker(SpeakerModel model)
        {
            SpeakerModel sm = model;
            SpeakerRepository repo = new SpeakerRepository();

            try
            {

                if (repo.SaveNewSpeaker(model))
                {
                    var success = new { msg = "New Speaker saved successfully" };
                    return Json(new { success = success });
                }
                else
                {
                    var error = "Unable to save speaker. Duplicate speaker is found";
                    return Json(new { error = error });
                }
            }

            catch (Exception e)
            {
                var error = "Unable to save speaker. Please try again later";
                return Json(new { error = error });

            }
        }


        public ActionResult SpeakerDetails(int user_id)
        {
            SpeakerModel sm;
            SpeakerRepository repo = new SpeakerRepository();

            sm = repo.GetSpeakerByuserid(user_id);

            return View(sm);
        }


        public ActionResult GetAllApprovedSpeakers(int ProgramID)
        {

            List<SpeakerModel> liSpeakerModel;

            SpeakerRepository speakerrepo = new SpeakerRepository();
            liSpeakerModel = speakerrepo.GetAllApprovedSpeakers(ProgramID);

            return Json(new { data = liSpeakerModel }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetCOISlides(int UserID)
        {
            ViewBag.SpeakerCOIURL = ConfigurationManager.AppSettings["SpeakerCOIURL"];
            ViewBag.SpeakerBaseURL = ConfigurationManager.AppSettings["SpeakerBaseURL"];
            List<COISlide> liCOISlides;
            SpeakerRepository repo = new SpeakerRepository();
            ProgramRepository programrepo = new ProgramRepository();


            liCOISlides = repo.GetCOISlides(UserID);

            foreach (COISlide slide in liCOISlides)
            {
                slide.ProgramName = programrepo.GetProgramName(slide.ProgramID);
            }

            return View(liCOISlides);
        }


        public ActionResult GetPresenterPayments(int userid)
        {
            List<PresenterPayment> liPresenterPayment;
            SpeakerModel sm;
            SpeakerRepository repo = new SpeakerRepository();


            sm = repo.GetSpeakerByuserid(userid);
            if (sm.SpeakerHonoraria != null)
                ViewBag.SpeakerHonoraria = "$ " + sm.SpeakerHonoraria;
            if (sm.ModeratorHonoraria != null)
                ViewBag.ModeratorHonoraria = "$ " + sm.ModeratorHonoraria;

            liPresenterPayment = repo.GetPresenterPayments(userid);

            return View(liPresenterPayment);
        }

    }
}
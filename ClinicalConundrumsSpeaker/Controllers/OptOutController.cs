using ClinicalConundrumsSpeaker.DAL;
using ClinicalConundrumsSpeaker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClinicalConundrumsSpeaker.Controllers
{
    public class OptOutController : Controller
    {
        public ActionResult Index(string userid)
        {
            if (!String.IsNullOrEmpty(userid))
            {
                var UserRepo = new UserRepository();
                UserModel um = new UserModel();


                int id = Convert.ToInt32(userid);
                um = UserRepo.GetUserForConfirmEmail(id);

                ViewBag.Email = um.EmailAddress;
                ViewBag.id = userid;


            }



         


            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public JsonResult Index(string userid, string Email, string FullName)
        {
           


            var userRepo = new UserRepository();
            int SpeakerOrModeratorId;

            string error = string.Empty;

            if (String.IsNullOrEmpty(Email))
            {
                error = "Please Enter Email";
                return Json(new { Error = error });
            }

            if (userRepo.CheckEmailInUserinfo(Email))
            {
                if (string.IsNullOrEmpty(userid))
                {
                    SpeakerOrModeratorId = userRepo.GetUserIdFromEmail(Email);

                }
                else
                {
                    SpeakerOrModeratorId = Convert.ToInt32(userid);

                }


                userRepo.UpdateOptOutStatusAndProgramRequest(SpeakerOrModeratorId);
                return Json(new { redirectTo = Url.Action("OptOutConfirmation") });

            }
            else
            {
                error = "*Please Contact Admin - Email not valid";
                return Json(new { Error = error });

            }

        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult OptOutConfirmation()
        {
            
            return View();
        }
    }
}
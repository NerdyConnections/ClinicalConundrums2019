using ClinicalConundrum2019.Data;
using ClinicalConundrumsSpeaker.DAL;
using ClinicalConundrumsSpeaker.Models;
using ClinicalConundrumsSpeaker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ClinicalConundrumsSpeaker.Controllers
{
    public class ActivationController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Index(SpeakerActivationEmailModel emailModel)
        {
            if (!ModelState.IsValid)
            {
                return View(emailModel);
            }else
            {
                ViewBag.IsHttpPost = true;

                ActivateRepository repo = new ActivateRepository();
                SpeakerActivationModel model = repo.GetActivationSpeakerbyEmail(emailModel.EmailAddress);
                if(model == null)
                {
                    ViewBag.Msg = "This email does not exist";
                    return View(emailModel);
                }
                else 
                {
                    if(model.UserId != null || repo.GetUserByEmail(emailModel.EmailAddress) != null)
                    {
                        ViewBag.Msg = "This email has been activated";
                        return View(emailModel);
                    }else
                    {
                        return RedirectToAction("account", model);
                    }           
                }
            }
        }


        [HttpGet]
        public ActionResult Account(SpeakerActivationModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ActionName("Account")]
        public ActionResult AccountPost(SpeakerActivationModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }else
            {
                ViewBag.IsHttpPost = true;

                ActivateRepository repo = new ActivateRepository();
                UserInfo userInfo = repo.GetUserInfoByEmail(model.Username);
                if(userInfo == null)
                {
                    ViewBag.Msg = "This email does not exist";
                    ViewBag.IsSuccessful = false;
                }
                else if(userInfo.UserID != null || repo.GetUserByEmail(model.Username) != null)
                {
                    ViewBag.Msg = "This email has been activated";
                    ViewBag.IsSuccessful = false;
                }
                else
                {
                    repo.ActivateSpeaker(model, userInfo);
                    ViewBag.IsSuccessful = true;
                    
                    Task.Factory.StartNew(() => {
                        UserHelper.SpeakerActivationEmail(model.LastName, model.Username, model.Password, this);
                    });
                }
                return View(model);
            }  
        }
    }
}
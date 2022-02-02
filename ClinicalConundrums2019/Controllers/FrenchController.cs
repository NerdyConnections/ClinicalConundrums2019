using ClinicalConundrums2019.DAL;
using ClinicalConundrums2019.Models;
using ClinicalConundrums2019.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ClinicalConundrums2019.Controllers
{
    public class FrenchController : Controller
    {
        // GET: French
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Disclosure()
        {
            return View();
        }
        public ActionResult ScientificPlanningCommittee()
        {
            return View();
        }
        public ActionResult ProgramMaterials()
        {
            return View();
        }
        public ActionResult HandoutRequest()
        {
            UserModel user = UserHelper.GetLoggedInUser();
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                HandoutRequestRepository repo = new HandoutRequestRepository();
                HandoutRequestModel model = repo.GetHandoutRequest(user.UserID);
                if (model == null)
                {
                    model = new HandoutRequestModel();
                }/*else
                {
                    ViewBag.HideSubmit = true;
                }*/
                return View(model);
            }
        }


        [HttpPost]
        public ActionResult HandoutRequest(HandoutRequestModel model)
        {
            UserModel currentUser = UserHelper.GetLoggedInUser();
            if (currentUser != null)
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                else
                {
                    model.UserID = currentUser.UserID;
                    HandoutRequestRepository repo = new HandoutRequestRepository();
                    repo.SaveHandoutRequest(model);
                }
            }
            else
            {
                RedirectToAction("Login", "Account");
            }

            //return RedirectToAction("Index", "Program");
            ViewBag.IsHttpPost = true;
            ViewBag.IsSuccessful = true;

            Task.Factory.StartNew(() =>
            {
                UserHelper.SendEmailAfterHandoutRequest(model, false);
                UserHelper.SendEmailAfterHandoutRequest(model, true);
            });

            return View(model);
        }
    }
}

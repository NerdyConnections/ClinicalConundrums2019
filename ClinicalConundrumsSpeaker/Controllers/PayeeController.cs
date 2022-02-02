using ClinicalConundrumsSpeaker.DAL;
using ClinicalConundrumsSpeaker.Models;
using ClinicalConundrumsSpeaker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClinicalConundrumsSpeaker.Controllers
{
    public class PayeeController : Controller
    {
        public ActionResult Index()
        {
            var currentUser = UserHelper.GetLoggedInUser();

            if (currentUser != null)
            {
                int userId = currentUser.UserID;
                ViewBag.RegistrationStatus = UserHelper.GetRegistrationStatus(userId);

                PayeeRepository repo = new PayeeRepository();
                PayeeModel model = repo.GetPayeeModelByUserId(userId);
                return View("Index", model);
            }else
            {
                return RedirectToAction("Login", "Account");
            }    
        }

        [HttpPost]
        public ActionResult Index(PayeeModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                PayeeRepository repo = new PayeeRepository();
                repo.AddOrUpdatePayeeInfo(model);

                ViewBag.IsHttpPost = true;
                ViewBag.RegistrationStatus = UserHelper.GetRegistrationStatus(model.UserId);
                return View(model);
            }
        }
    }
}
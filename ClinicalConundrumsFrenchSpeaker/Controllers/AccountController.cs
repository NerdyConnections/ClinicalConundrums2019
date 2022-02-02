using ClinicalConundrumsFrenchSpeaker.Models;
using ClinicalConundrumsFrenchSpeaker.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ClinicalConundrumsFrenchSpeaker.Controllers
{
    public class AccountController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {

            // TempData["ImageUrl"] = UserHelper.GetCPDLogo(Request);
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }
        //
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }

         //   var userRepo = new UserRepository();
            bool IsAuthenticated;
            string CorrectUsername = ConfigurationManager.AppSettings["CorrectUsername"].ToString();
            string CorrectPassword = ConfigurationManager.AppSettings["CorrectPassword"].ToString();
            if (model.Email== ConfigurationManager.AppSettings["CorrectUsername"].ToString() && model.Password== ConfigurationManager.AppSettings["CorrectPassword"].ToString())
            {
                IsAuthenticated = true;
            }
            else
            {
                IsAuthenticated = false;
            }
          
            if (IsAuthenticated)
            {
                //the database has the correct credentials but is the account activated yet?
                //IsActivated = userRepo.IsActivated(model.Email, Encryptor.Encrypt(model.Password));
                //if (IsActivated)
               // {
                    HttpCookie AuthorizationCookie = UserHelper.GetAuthorizationCookie(model.Email, ""); //roles are pipe delimited
                    Response.Cookies.Add(AuthorizationCookie);
                    string[] userRoles = {""};
                    System.Web.HttpContext.Current.User = new GenericPrincipal(System.Web.HttpContext.Current.User.Identity, userRoles);  //set the roles of Current.User.Identity

                // FormsAuthentication.SetAuthCookie(model.Email, false);


                //bool result1 = User.IsInRole("SPECIALIST");
                //bool result2 = User.IsInRole("PCP");

                UserModel CurrentUser = new UserModel();//cannot user identity.name because it will set only when the auth cookie is passed in in the next request.
                CurrentUser.FirstName = "Speaker";
                    //making sure nothing in the temppaf table from previous session
                    // Session["LogoUrl"]=UserHelper.GetCPDLogo(Request);

                    UserHelper.SetLoggedInUser(CurrentUser, System.Web.HttpContext.Current.Session);

                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/") & !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        if (User.IsInRole("Admin"))
                            return RedirectToAction("Index", "Admin");
                        else
                            return RedirectToAction("Index", "Home");
                        //return RedirectToAction("Index", "Home");
                    }
                }//not yet activate, redirect to activate account screen
                else
                {

                    return RedirectToAction("Login", "Account");
                }
            
           

 
        }

        [HttpGet]
        public ActionResult LogOff()
        {
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();


            return RedirectToAction("Index", "Home");
        }

      

       
    }
}
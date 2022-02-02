using ClinicalConundrum2019.Data;
using ClinicalConundrumsSpeaker.DAL;
using ClinicalConundrumsSpeaker.Models;
using ClinicalConundrumsSpeaker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ClinicalConundrumsSpeaker.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
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

            var userRepo = new UserRepository();
            bool IsAuthenticated, IsActivated;
            IsAuthenticated = userRepo.AuthenticateSpeaker(model.Email, Encryptor.Encrypt(model.Password));
            if (IsAuthenticated)
            {
                //the database has the correct credentials but is the account activated yet?
                IsActivated = userRepo.IsActivated(model.Email, Encryptor.Encrypt(model.Password));
                if (IsActivated)
                {
                    HttpCookie AuthorizationCookie = UserHelper.GetAuthorizationCookie(model.Email, userRepo.GetRoles(model.Email)); //roles are pipe delimited
                    Response.Cookies.Add(AuthorizationCookie);
                    string[] userRoles = userRepo.GetRolesAsArray((model.Email));
                    System.Web.HttpContext.Current.User = new GenericPrincipal(System.Web.HttpContext.Current.User.Identity, userRoles);  //set the roles of Current.User.Identity

                    // FormsAuthentication.SetAuthCookie(model.Email, false);


                    //bool result1 = User.IsInRole("SPECIALIST");
                    //bool result2 = User.IsInRole("PCP");

                    UserModel CurrentUser = userRepo.GetUserDetails(model.Email);//cannot user identity.name because it will set only when the auth cookie is passed in in the next request.

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

                    return RedirectToAction("Activate", "Account");
                }
            }
            else
            {
                ModelState.AddModelError("Email", "Invalid Username or Password");

                return View();
            }

 ;
        }

        [HttpGet]
        public ActionResult LogOff()
        {
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();


            return RedirectToAction("Index", "Home");
        }

        public ActionResult ForgotPassword()
        {
            ViewBag.Msg = "";
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string Email)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            UserRepository repo = new UserRepository();
            User user = repo.GetUserByEmail(Email);
            if (user == null)
            {
                UserInfo userInfo = repo.GetUserInfoByEmail(Email);
                if (userInfo == null)
                {
                    ViewBag.Msg = "This account doesn't existed";
                }
                else
                {
                    ViewBag.Msg = "This account has not been activated";
                }
                return View();

            }
            else
            {
                Task.Factory.StartNew(() => {
                    UserHelper.SendEmailForgotPassword(Email, Encryptor.Decrypt(user.Password));
                });
                return View("PasswordConfirmation");
            }
        }

        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
    }
}
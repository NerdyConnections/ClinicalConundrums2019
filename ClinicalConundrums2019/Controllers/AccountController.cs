using ClinicalConundrum2019.Data;
using ClinicalConundrums2019.DAL;
using ClinicalConundrums2019.Models;
using ClinicalConundrums2019.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ClinicalConundrums2019.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {

           
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [HttpGet]
        public ActionResult LogOff()
        {
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut();


            return RedirectToAction("Index", "Program");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {

            if (!ModelState.IsValid)
            {
               
                return View();
            }

            var userRepo = new UserRepository();
            bool IsAuthenticated;
            IsAuthenticated = userRepo.Authenticate(model.Email, Encryptor.Encrypt(model.Password));
            if (IsAuthenticated)
            {
                //the database has the correct credentials but is the account activated yet?
             
                    HttpCookie AuthorizationCookie = UserHelper.GetAuthorizationCookie(model.Email, userRepo.GetRoles(model.Email)); //roles are pipe delimited
                    Response.Cookies.Add(AuthorizationCookie);
                    string[] userRoles = userRepo.GetRolesAsArray((model.Email));
                    System.Web.HttpContext.Current.User = new GenericPrincipal(System.Web.HttpContext.Current.User.Identity, userRoles);  //set the roles of Current.User.Identity

                    // FormsAuthentication.SetAuthCookie(model.Email, false);


                    //bool result1 = User.IsInRole("SPECIALIST");
                    //bool result2 = User.IsInRole("PCP");

                    UserModel CurrentUser = userRepo.GetUserDetails(model.Email);//cannot user identity.name because it will set only when the auth cookie is passed in in the next request.

                  
                    UserHelper.SetLoggedInUser(CurrentUser, System.Web.HttpContext.Current.Session);

                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/") & !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        if (User.IsInRole(Util.Constants.Admin))
                            return RedirectToAction("Index", "Admin");
                        else
                            return RedirectToAction("Index", "Program");
                    }
              
            }
            else
            {
                ModelState.AddModelError("", "Invalid Username/Password");
                return View();
            }

 
        }

        [HttpGet]
        public ActionResult Activate()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Activate(ActivationModel model)
        {
            if (!ModelState.IsValid)
            {               
                return View();
            }

            UserRepository repo = new UserRepository();
            UserInfo userInfo = repo.GetUserInfoByEmail(model.Email);

            ActivationSubmitModel submitModel = new ActivationSubmitModel();
            submitModel.Email = model.Email;
            submitModel.FirstName = userInfo.FirstName;
            submitModel.LastName = userInfo.LastName;
            submitModel.PhoneNumber = userInfo.Phone;

            return View("ActivationSubmit", submitModel);
        }

      

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActivationSubmit(ActivationSubmitModel model)
        {

            if (!ModelState.IsValid)
            {
                return View();
            }

            UserRepository repo = new UserRepository();
            if (repo.ActivateUser(model))
            {
                UserRepository userRepo = new UserRepository();
                UserModel CurrentUser = userRepo.GetUserDetails(model.Email);
                UserHelper.SendActivationEmail(model.FirstName, model.Username, model.Password);
                //add email code here.
                UserHelper.SetLoggedInUser(CurrentUser, System.Web.HttpContext.Current.Session);
                return Json(new { success = "true" });
            }else
            {
                return Json(new { success = "false" });
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult SpeakerProgramDateUpdate(string ProgramDate, int ProgramRequestID)
        {
            

            ViewBag.ProgramDate = ProgramDate;
            ViewBag.ProgramRequestID = ProgramRequestID;


            return View("SpeakerProgramDateUpdate");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult ConfirmSpeakerEmail(string ProgramRequestID, string ProgramDate)
        {
            ProgramRepository repo = new ProgramRepository();
            UserRepository userRepo = new UserRepository();
            EventRequestModel pr = new EventRequestModel();
            UserModel um = new UserModel();


            if (string.IsNullOrEmpty(ProgramRequestID) || string.IsNullOrEmpty(ProgramRequestID))
            {
                return RedirectToAction("Login");

            }
            //check to see if user already confirmed the date
            int programRequestID = int.Parse(ProgramRequestID);
            string ChoosenDate = ProgramDate;

            if (repo.CheckIfSpeakerConfirmedEmail(programRequestID))
            {
                return RedirectToAction("SpeakerProgramDateUpdate", "Account", new { ProgramDate = "AlreadyExists", ProgramRequestID = programRequestID });
            }

            pr = repo.GetProgramRequestForSpeaker(programRequestID);
            string ProgramName = repo.GetProgramRequestName(pr.ProgramID);
            //get the user(ie. the speaker) object from userinfo table


            um = userRepo.GetUserForConfirmEmail(pr.ProgramSpeakerID);
            if (ProgramDate.Equals("NotAvailable"))
            {
                //change Speakerstatus to Not Not Available
                repo.UpdateSpeakerToNotAvailable(programRequestID);
                
                UserHelper.FromSpeakerToAdmin(pr, um, ProgramName, ProgramDate);


                //send email to salerep when speaker is not available 
                //Production Import Dija
                UserHelper.FromSpeakerToSalesRep(pr, um, ProgramName, ProgramDate);


                return RedirectToAction("SpeakerProgramDateUpdate", "Account", new { ProgramDate = ProgramDate, ProgramRequestID = programRequestID });
            }
            //send email to admin about Speaker's selection
            UserHelper.FromSpeakerToAdmin(pr, um, ProgramName, ProgramDate);
            // repo.UpdateSpeakerConfirmDate(programRequestID, ProgramDate);
            //check to see if speaker2 exists and if exists fire email to speaker 2 about the date confirmed by speaker.
            if (repo.checkifSpeaker2Exist(programRequestID))
            {
                string SessionCredit = repo.SessionCredit(pr);
                
                //get info about speaker 2
                um = userRepo.GetUserForConfirmEmail(pr.ProgramSpeaker2ID ?? 0);
                //  UserHelper.FromSpeakerToModerator(pr, um, ProgramName, ChoosenDate, SessionCredit);
                UserHelper.FromSpeakerToSpeaker2(pr, um, ProgramName, ChoosenDate, SessionCredit);

            }
            //    else
            //  {
            //     repo.UpdateSpeakerConfirmDateWhenNoModerator(programRequestID, ProgramDate);
            // UserHelper.FromSpeakerToAdmin(pr, um, ProgramName, ProgramDate);
            // }
            //send email to moderator to confirm program date if moderator exists
           if (repo.checkifModeratorExist(programRequestID))
            {
                string SessionCredit = repo.SessionCredit(pr);
                //not yet do it when moderator confirms
                //repo.UpdateModeratorConfirmDate(programRequestID, ProgramDate);
                um = userRepo.GetUserForConfirmEmail(pr.ProgramModeratorID ?? 0);
                UserHelper.FromSpeakerToModerator(pr, um, ProgramName, ChoosenDate, SessionCredit);

            }
            //if both moderator and speaker2 not exist set ConfirmedSessionDate
            if (!repo.checkifModeratorExist(programRequestID) && !repo.checkifSpeaker2Exist(programRequestID))//when both speaker2 and moderator do not exist
            {
                repo.UpdateSpeakerConfirmDateWhenNoModerator(programRequestID, ProgramDate);
            }
            else  //if either moderator or speaker 2 is present set SpeakerConfirmedProgramDate only
            {
                repo.UpdateSpeakerConfirmDate(programRequestID, ProgramDate);
            }
            //else
            //{
            //   repo.UpdateSpeakerConfirmDateWhenNoModerator(programRequestID, ProgramDate);
              //UserHelper.FromSpeakerToAdmin(pr, um, ProgramName, ProgramDate);
            //}

            return RedirectToAction("SpeakerProgramDateUpdate", "Account", new { ProgramDate = ProgramDate, ProgramRequestID = programRequestID });
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ConfirmSpeaker2Email(string ProgramRequestID, string ProgramDate)
        {
            ProgramRepository repo = new ProgramRepository();
            UserRepository userRepo = new UserRepository();
            
            UserModel um = new UserModel();


            if (string.IsNullOrEmpty(ProgramDate) || string.IsNullOrEmpty(ProgramRequestID))
            {
                return RedirectToAction("Login");

            }
            //check to see if user already confirmed the date
            int programRequestID = int.Parse(ProgramRequestID);


            if (repo.CheckIfSpeaker2ConfirmedEmail(programRequestID))
            {
                return RedirectToAction("SpeakerProgramDateUpdate", "Account", new { ProgramDate = "AlreadyExists", ProgramRequestID = programRequestID });

            }

          //  ProgramRequest pr = repo.GetProgramRequest(programRequestID);


                 EventRequestModel   pr = repo.GetProgramRequestForSpeaker(programRequestID);

            string ProgramName = repo.GetProgramRequestName(pr.ProgramID);
            um = userRepo.GetUserForConfirmEmail(pr.ProgramSpeaker2ID ?? 0);

            if (ProgramDate.Equals("NotAvailable"))
            {

             
                repo.UpdateSpeaker2ToNotAvailable(programRequestID);
                //send email to admin
                //UserHelper.FromModeratorToAdmin(pr, um, ProgramName, ProgramDate);
                //send email to SalesRep
                //Production IMport for Dija
                UserHelper.FromSpeakerToSalesRep(pr, um, ProgramName, ProgramDate);

                return RedirectToAction("SpeakerProgramDateUpdate", "Account", new { ProgramDate = ProgramDate, ProgramRequestID = programRequestID });
            }
            else
            {
              

                repo.UpdateSpeaker2ConfirmDate(programRequestID, ProgramDate);
                UserHelper.FromSpeaker2ToAdmin(pr, um, ProgramName, ProgramDate);

            }

            return RedirectToAction("SpeakerProgramDateUpdate", "Account", new { ProgramDate = ProgramDate, ProgramRequestID = programRequestID });
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ConfirmModeratorEmail(string ProgramRequestID, string ProgramDate)
        {
            ProgramRepository repo = new ProgramRepository();
            UserRepository userRepo = new UserRepository();
            UserModel um = new UserModel();


            if (string.IsNullOrEmpty(ProgramDate) || string.IsNullOrEmpty(ProgramRequestID))
            {
                return RedirectToAction("Login");

            }
            //check to see if user already confirmed the date
            int programRequestID = int.Parse(ProgramRequestID);


            if (repo.CheckIfModeratorConfirmedEmail(programRequestID))
            {
                return RedirectToAction("SpeakerProgramDateUpdate", "Account", new { ProgramDate = "AlreadyExists", ProgramRequestID = programRequestID });

            }
            EventRequestModel pr = repo.GetProgramRequestForSpeaker(programRequestID);
           // ProgramRequest pr = repo.GetProgramRequest(programRequestID);
            string ProgramName = repo.GetProgramRequestName(pr.ProgramID);
            //get the user object from userinfo table
            um = userRepo.GetUserForConfirmEmail(pr.ProgramModeratorID ?? 0);

            if (ProgramDate.Equals("NotAvailable"))
            {

                repo.UpdateModeratorToNotAvailable(programRequestID);
                //send email to admin
                UserHelper.FromModeratorToAdmin(pr, um, ProgramName, ProgramDate);
                //send email to SalesRep
                //UserHelper.FromModeratorToSalesRep(pr, um, ProgramName, ProgramDate);

                return RedirectToAction("SpeakerProgramDateUpdate", "Account", new { ProgramDate = ProgramDate, ProgramRequestID = programRequestID });
            }
            else
            {
                repo.UpdateModeratorConfirmDate(programRequestID, ProgramDate);
                UserHelper.FromModeratorToAdmin(pr, um, ProgramName, ProgramDate);

            }

            return RedirectToAction("SpeakerProgramDateUpdate", "Account", new { ProgramDate = ProgramDate, ProgramRequestID = programRequestID });
        }
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            ViewBag.Msg = "";
            return View();
        }
       
      /*  [AllowAnonymous]
        public JsonResult ForgotPassword(string Email)
        {
            var userRepo = new UserRepository();

            string error = string.Empty;

            if (String.IsNullOrEmpty(Email))
            {
                error = "Please Enter Email and click Submit";
                return Json(new { Error = error });
            }

            if (userRepo.CheckIfActivated(Email))
            {
                var model = userRepo.GetUserCredentials(Email);
                UserHelper.SendEmailForgotPassword(model.Email, model.CurrentPassword);
                return Json(new { redirectTo = Url.Action("ForgotPasswordConfirmation", new { email = model.Email }) });
            }

            else
            {
                error = "Please activate your account.No User found";
                return Json(new { Error = error });
            }
        }
        */
        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            UserRepository repo = new UserRepository();
            User user = repo.GetUserByEmail(model.Email);
            if (user == null)
            {
                UserInfo userInfo = repo.GetUserInfoByEmail(model.Email);
                if (userInfo == null)
                {
                    ViewBag.Msg = "This account doesn't existed";
                }
                else
                {
                    ViewBag.Msg = "This account has not been activated";
                }
                return View();

            }else
            {
                Task.Factory.StartNew(() => {
                    UserHelper.SendEmailForgotPassword(model.Email, Encryptor.Decrypt(user.Password));
                });
                return View("ForgotPassword");
            }
        }

        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

    }
}
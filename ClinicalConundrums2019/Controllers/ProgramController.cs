using ClinicalConundrum2019.Data;
using ClinicalConundrums2019.DAL;
using ClinicalConundrums2019.Models;
using ClinicalConundrums2019.Util;
using ClinicalConundrums2019.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ClinicalConundrums2019.Controllers
{
    [Authorize]
    public class ProgramController : Controller
    {
        // GET: Program
        public ActionResult Index(bool showPopup = false)
        {
            List<Models.Program> liProgram;
            ViewBag.ShowPopup = showPopup;
            ProgramRepository tr = new ProgramRepository();

            liProgram = tr.GetPrograms();
            return View(liProgram);
        }
        public ActionResult Description()
        {
            if (Session["ProgramID"] != null)
            {
                UserModel userModel = UserHelper.GetLoggedInUser();
                if (userModel == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                ViewBag.UserID = userModel.UserID;
                ViewBag.ProgramID = Session["ProgramID"].ToString();


                return View("Description_" + Session["ProgramID"]);

            }
            else
            {
                return RedirectToAction("Index", "Program");

            }


        }
        public ActionResult Committee()
        {
            if (Session["ProgramID"] != null)
            {
                ViewBag.UserID = UserHelper.GetLoggedInUser().UserID;
                ViewBag.ProgramID = Session["ProgramID"].ToString();


                return View("Committee_" + Session["ProgramID"]);

            }
            else
            {
                return RedirectToAction("Index", "Program");

            }






        }

        public ActionResult NewEventRequest()
        {
            if (Session["ProgramID"] != null)
            {
                UserModel um = UserHelper.GetLoggedInUser();
                if (um != null)
                {
                    int ProgramID = Convert.ToInt32(Session["ProgramID"]);
                    ProgramRepository ProgramRepo = new ProgramRepository();
                    ViewBag.ProgramName = ProgramRepo.GetProgramName(ProgramID);

                    EventRequestModel erm = ProgramRepo.InitialEventRequestForm(ProgramID);
                    erm.UserID = um.UserID;
                    return View(erm);
                }
                else
                {
                    /*Session.Clear();
                    Session.Abandon();
                    FormsAuthentication.SignOut();
                    return RedirectToAction("Index", "Program");*/
                    return RedirectToAction("Login", "Account");

                }

            }
            else
            {
                return RedirectToAction("Index", "Program");

            }


        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="erm"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveEventRequest(EventRequestModel erm)
        {
            ProgramRepository PRepo = new ProgramRepository();

            //string destinationPath = Server.MapPath("~/App_Data/MultSessionAgenda.pdf");

            var CurrentUser = UserHelper.GetLoggedInUser();
            if (CurrentUser == null)
            {
                Session.Clear();
                Session.Abandon();
                FormsAuthentication.SignOut();
                return RedirectToAction("Index", "Program");
            }

            EventRequestModel val = PRepo.PopulateSpeakerModeratorDropdowns();
            string ProgramName = PRepo.GetProgramRequestName(erm.ProgramID);
            UserRepository userRepo = new UserRepository();

            //can only validate program when the program request is first save, cannot validate date when a program request is updated because by then the program date will not be 4 weeks from today's date for sure
            ////Commented out for Dija Production IMport
            if (!IsValidProgramDate(erm.ProgramDate1))
            {

                ModelState.AddModelError("ProgramDate1", "* Must be 4 weeks from today's date");
            }
            if (!IsValidProgramDate(erm.ProgramDate2))
            {

                ModelState.AddModelError("ProgramDate2", "* Must be 4 weeks from today's date");
            }
            if (!IsValidProgramDate(erm.ProgramDate3))
            {

                ModelState.AddModelError("ProgramDate3", "* Must be 4 weeks from today's date");
            }

            if (!ModelState.IsValid)
            {
                erm.Speakers = val.Speakers;
                erm.Moderators = val.Moderators;
                //ViewBag.id = CurrentUser.UserID;
                // ViewBag.IsAdmin = pr.IsAdmin;


                return View("NewEventRequest", erm);
            }

            if (!erm.SessionCredit1 && !erm.SessionCredit2 && !erm.SessionCredit3 && !erm.SessionCredit4 && !erm.SessionCredit5 && !erm.SessionCredit6
            && !erm.SessionCredit7 && !erm.SessionCredit8 && !erm.SessionCredit9 && !erm.SessionCredit10 && !erm.SessionCredit11 && !erm.SessionCredit12)
            {
                erm.Speakers = val.Speakers;
                erm.Moderators = val.Moderators;
                ViewBag.ShowSelectCreditMsg = true;
                return View("NewEventRequest", erm);
            }

            if (erm.IsAdmin != 1)//this is an end  user
            {
                bool TimechangesBySalesRep = PRepo.IsMealTimesChangesBySalesRep(erm);
                bool VenueChangeBySalesRep = PRepo.IsVenueChangesBySalesRep(erm);
                //if either speaker changed or program date/time/venue changed
                if (PRepo.CheckIfSpeakerChanges(erm) || PRepo.CompareProgramDates(erm) || TimechangesBySalesRep || VenueChangeBySalesRep)
                {
                    //save event request
                    ViewBag.IsSuccessful = PRepo.SaveNewSession(erm);

                    //since something important has changed, hence need to resend invitation emails to speaker/moderator
                    //PRepo.ResetSpeakerModeratorConfirmDates(erm.ProgramRequestID);


                    var SpeakerList = val.Speakers.ToList();
                    var speakername = SpeakerList.Find(x => x.Value == erm.ProgramSpeakerID.ToString()).Text;
                    var speaker2name = "";
                    if (!string.IsNullOrEmpty(erm.ProgramSpeaker2ID.ToString()))
                    {
                        speaker2name = SpeakerList.Find(x => x.Value == erm.ProgramSpeaker2ID.ToString()).Text;
                    }
                    var ModeratorList = val.Moderators.ToList();
                    var ModeratorName = "";
                    if (!string.IsNullOrEmpty(erm.ProgramModeratorID.ToString()))
                    {
                        ModeratorName = ModeratorList.Find(x => x.Value == erm.ProgramModeratorID.ToString()).Text;
                    }
                    //a string containing all session credits
                    string SessionCredit = PRepo.SessionCredit(erm);

                    UserModel um = new UserModel();
                    um = userRepo.GetUserForConfirmEmail(erm.ProgramSpeakerID);

                  // Task.Factory.StartNew(() =>
                   // {
                        UserHelper.SendAdminProgramRequest(erm, speakername, speaker2name, ModeratorName, SessionCredit, ProgramName, this);
                        //Commented out for Dija Production IMport 1/3
                        UserHelper.SelectInvitationOption(erm, speaker2name, ModeratorName, SessionCredit, this);
                  // });


                    ViewBag.IsHttpPost = true;

                    erm.Speakers = val.Speakers;
                    erm.Moderators = val.Moderators;
                    return View("NewEventRequest", erm);
                }

                else  //no  changes in time/venue speakers
                {
                    //if no changes in time/venue/speaker but moderator changed
                    if (PRepo.CheckIfModeratorChanges(erm))
                    {
                        //if there is a moderator
                        if (erm.ProgramModeratorID != null)
                        {
                            var SpeakerList = val.Speakers.ToList();
                            var speakername = SpeakerList.Find(x => x.Value == erm.ProgramSpeakerID.ToString()).Text;
                            var ModeratorList = val.Moderators.ToList();
                            var ModeratorName = "";
                            if (!string.IsNullOrEmpty(erm.ProgramModeratorID.ToString()))
                            {
                                ModeratorName = ModeratorList.Find(x => x.Value == erm.ProgramModeratorID.ToString()).Text;
                            }

                            string SessionCredit = PRepo.SessionCredit(erm);
                            //get date from speaker confirmation date
                            var chosendate = PRepo.GetSpeakerConfirmationDate(erm.ProgramRequestID);

                            PRepo.UpdateSession(erm);
                            PRepo.ResetModeratorstatusAndConfirmSessionDate(erm.ProgramRequestID);


                            //send email to moderator only this time  because moderator has been updated

                            UserModel um = new UserModel();
                            um = userRepo.GetUserForConfirmEmail(erm.ProgramModeratorID ?? 0);
                            //Commented out for Dija Production IMport 2/3
                            UserHelper.FromSpeakerToModerator(erm, um, ProgramName, chosendate, SessionCredit);
                        }
                    }
                    //if no changes in time/venue/speaker but speaker2 changed
                    if (PRepo.CheckIfSpeaker2Changes(erm))
                    {
                        //if there is a speaker2
                        if (erm.ProgramSpeaker2ID != null)
                        {
                            var SpeakerList = val.Speakers.ToList();
                            var speakername = SpeakerList.Find(x => x.Value == erm.ProgramSpeakerID.ToString()).Text;
                            var ModeratorList = val.Moderators.ToList();
                            var ModeratorName = "";
                            var Speaker2Name = "";
                            if (!string.IsNullOrEmpty(erm.ProgramSpeaker2ID.ToString()))
                            {
                                Speaker2Name = SpeakerList.Find(x => x.Value == erm.ProgramSpeaker2ID.ToString()).Text;
                            }

                            string SessionCredit = PRepo.SessionCredit(erm);
                            //get date from speaker confirmation date
                            var chosendate = PRepo.GetSpeakerConfirmationDate(erm.ProgramRequestID);

                            PRepo.UpdateSession(erm);
                            PRepo.ResetSpeaker2statusAndConfirmSessionDate(erm.ProgramRequestID);


                            //send email to speaker2 only this time  because speaker2 has been updated

                            UserModel um = new UserModel();
                            um = userRepo.GetUserForConfirmEmail(erm.ProgramSpeaker2ID ?? 0);
                            //Commented out for Dija Production IMport 3/3
                            UserHelper.FromSpeakerToSpeaker2(erm, um, ProgramName, chosendate, SessionCredit);
                        }
                    }


                }

                PRepo.UpdateSession(erm);
                return RedirectToAction("Index", "Program");
            }


            else //this is an admin updating event request.
            {
                //  bool TimechangesBySalesRep = PRepo.IsMealTimesChangesBySalesRep(erm);
                // bool VenueChangeBySalesRep = PRepo.IsVenueChangesBySalesRep(erm);

                // if (PRepo.CheckIfSpeakerChanges(erm) || PRepo.CompareProgramDates(erm) || TimechangesBySalesRep || VenueChangeBySalesRep)
                // {

                // PRepo.SaveNewSession(erm);
                //  PRepo.ResetSpeakerModeratorConfirmDates(erm.ProgramRequestID);


                //  var SpeakerList = val.Speakers.ToList();
                //   var speakername = SpeakerList.Find(x => x.Value == erm.ProgramSpeakerID.ToString()).Text;
                //  var ModeratorList = val.Moderators.ToList();
                // var ModeratorName = "";
                //   if (!string.IsNullOrEmpty(erm.ProgramModeratorID.ToString()))
                // {
                //     ModeratorName = ModeratorList.Find(x => x.Value == erm.ProgramModeratorID.ToString()).Text;
                //  }

                //    string SessionCredit = PRepo.SessionCredit(erm);

                //  UserModel um = new UserModel();
                //  UserModel SalesRep = new UserModel();

                // um = userRepo.GetUserForConfirmEmail(erm.ProgramSpeakerID);
                //Get the Sales Rep from Userinfo table 
                // SalesRep = userRepo.GetUserByUserID(erm.UserID);

                //   UserHelper.SendAdminProgramRequest(erm, speakername, ModeratorName, SessionCredit, ProgramName);

                //ToDo:  send email to salesrep

                //     UserHelper.EmailSalesRepWhenAdminMakeChanges(erm, SalesRep, ModeratorName, SessionCredit, ProgramName);
                //   UserHelper.SelectInvitationOption(erm, ModeratorName, SessionCredit);
                //    return RedirectToAction("Index", "Admin");
                //   }
                //admin update moderator
                //if (PRepo.CheckIfSpeaker2Changes(erm))
                //{
                //    if (erm.ProgramSpeaker2ID != null)
                //    {
                //        var SpeakerList = val.Speakers.ToList();
                //        var speakername = SpeakerList.Find(x => x.Value == erm.ProgramSpeaker2ID.ToString()).Text;
                //        var ModeratorList = val.Moderators.ToList();
                //        var ModeratorName = "";
                //        if (!string.IsNullOrEmpty(erm.ProgramModeratorID.ToString()))
                //        {
                //            ModeratorName = ModeratorList.Find(x => x.Value == erm.ProgramModeratorID.ToString()).Text;
                //        }

                //        string SessionCredit = PRepo.SessionCredit(erm);
                //        //get date from speaker confirmation date
                //        var chosendate = PRepo.GetSpeakerConfirmationDate(erm.ProgramRequestID);

                //        PRepo.UpdateSession(erm);
                //        PRepo.ResetModeratorstatusAndConfirmSessionDate(erm.ProgramRequestID);
                //        //send email to moderator only this time

                //        UserModel um = new UserModel();
                //        um = userRepo.GetUserForConfirmEmail(erm.ProgramModeratorID ?? 0);
                //        UserHelper.FromSpeakerToModerator(erm, um, ProgramName, chosendate, SessionCredit);
                //        return RedirectToAction("Index", "Admin");
                //    }

                //}

                //admin update moderator
                //if (PRepo.CheckIfModeratorChanges(erm))
                //{
                //    if (erm.ProgramModeratorID != null)
                //    {
                //        var SpeakerList = val.Speakers.ToList();
                //        var speakername = SpeakerList.Find(x => x.Value == erm.ProgramSpeakerID.ToString()).Text;
                //        var ModeratorList = val.Moderators.ToList();
                //        var ModeratorName = "";
                //        if (!string.IsNullOrEmpty(erm.ProgramModeratorID.ToString()))
                //        {
                //            ModeratorName = ModeratorList.Find(x => x.Value == erm.ProgramModeratorID.ToString()).Text;
                //        }

                //        string SessionCredit = PRepo.SessionCredit(erm);
                //        //get date from speaker confirmation date
                //        var chosendate = PRepo.GetSpeakerConfirmationDate(erm.ProgramRequestID);

                //        PRepo.UpdateSession(erm);
                //        PRepo.ResetModeratorstatusAndConfirmSessionDate(erm.ProgramRequestID);
                //        //send email to moderator only this time

                //        UserModel um = new UserModel();
                //        um = userRepo.GetUserForConfirmEmail(erm.ProgramModeratorID ?? 0);
                //        UserHelper.FromSpeakerToModerator(erm, um, ProgramName, chosendate, SessionCredit);
                //        return RedirectToAction("Index", "Admin");
                //    }

                //}

                if (erm.AdminVenueConfirmed != null && (erm.AdminVenueConfirmed).Equals("N"))
                {
                    //if admin cannot reserve the venue need to inform sale rep

                    //speaker has alrady chosen a program date by click in email 
                    var chosendate = PRepo.GetSpeakerConfirmationDate(erm.ProgramRequestID);
                    //send email to SalesRep
                    //get the sales rep data from userinfo data. 
                    UserModel um = new UserModel();
                    um = userRepo.GetSalesRepForConfirmEmail(erm.UserID);
                    //email to sales rep venue is not available
                    UserHelper.AdminToSalesRep(erm.LocationName, um, ProgramName, chosendate);
                    PRepo.UpdateSession(erm);
                    //make the program request "change required" so that sales rep can pick another venue
                    PRepo.UpdateProgramRequestWhenVenueChangedByAdmin(erm.ProgramRequestID);
                    return RedirectToAction("Index", "Admin");

                }

                else
                {
                    PRepo.UpdateSession(erm);
                    return RedirectToAction("Index", "Admin");


                }

            }

        }







        private bool IsValidProgramDate(string strProgramDate)
        {
            DateTime today = DateTime.Now;

            if (!String.IsNullOrEmpty(strProgramDate))
            {
                DateTime ProgramDate = DateTime.ParseExact(strProgramDate, "yyyy/MM/dd", null);
                var daycount = (ProgramDate - today).TotalDays;

                if (daycount <= 28)//if the requested program date is less than 4 week it is not valid
                    return false;
                else
                    return true;



            }
            else
                return true;  //

        }
        public JsonResult UploadFile(string ProgramRequestID)
        {
            string returnPath = "";
            string UploadedFileName = "";
            try
            {
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {

                        //check if file is supported
                        var supportedTypes = new[] { "doc", "docx", "pdf" };
                        var extension = Path.GetExtension(file).Substring(1);
                        if (!supportedTypes.Contains(extension))
                        {
                            var error = "File Extension Is InValid";
                            return Json(new { error = error });
                        }



                        //check if the folder exists, if not then make one. 
                        string FolderPath = Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["UserFileUploadPath"] + ProgramRequestID + "/Agenda/");


                        if (!Directory.Exists(FolderPath))
                        {
                            Directory.CreateDirectory(FolderPath);
                        }
                        // get a stream
                        var stream = fileContent.InputStream;
                        // and optionally write the file to disk
                        //var fileName = Path.GetFileName(file);
                        string fileName = "Agenda." + extension;

                        var path = Path.Combine(Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["UserFileUploadPath"] + ProgramRequestID + "/Agenda/"), fileName);


                        using (var fileStream = System.IO.File.Create(path))
                        {
                            stream.CopyTo(fileStream);
                        }
                        returnPath = FolderPath + fileName;
                        UploadedFileName = fileName;
                    }
                }
            }
            catch (Exception e)
            {
                //Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed");
            }

            var success = new { msg = "File uploaded successfully", returnFileName = returnPath, uploadedFilename = UploadedFileName };
            return Json(new { success = success });
        }
        public ActionResult GetAllProgramRequests(int programID)
        {

            List<EventRequestModel> liProgramRequest;

            ProgramRepository ur = new ProgramRepository();

            liProgramRequest = ur.GetAllProgramRequests(programID);

            return Json(new { data = liProgramRequest }, JsonRequestBehavior.AllowGet);

        }
        public FileResult OpenFile(string fileName)

        {
            try

            {
                return File(new FileStream(Server.MapPath("~/App_Data/" + fileName), FileMode.Open), "application/octetstream", fileName);

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]

        public ActionResult RemoveUploadFile(string fileName, int ProgramRequestID)

        {

            int retProgramRequestID = 0;

            // ((List<FileUploadModel>)Session["fileUploader"]).RemoveAll(x => x.FileName == fileName);

            //  sessionFileCount = ((List<FileUploadModel>)Session["fileUploader"]).Count;

            if (fileName != null || fileName != string.Empty)

            {

                FileInfo file = new FileInfo(Server.MapPath("~/App_Data/" + fileName));

                if (file.Exists)

                {

                    ProgramRepository PRepo = new ProgramRepository();
                    PRepo.UpdateUploadFileStatus(ProgramRequestID, false);
                    retProgramRequestID = ProgramRequestID;
                    file.Delete();

                }

            }
            return Json(retProgramRequestID, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Cancel(int ProgramRequestID)
        {

            if (ProgramRequestID != 0)
            {
                ProgramRequestCancellationVM PReqCancel;
                ProgramRepository PRepo = new ProgramRepository();
                PReqCancel = PRepo.GetProgramRequestCancellationbyID(ProgramRequestID);
                if (PReqCancel != null)
                {

                    return View(PReqCancel);
                }
                else
                    return View();
            }

            return View();
        }

        [HttpPost]
        public ActionResult Cancel(ProgramRequestCancellationVM prcvm)
        {
            ProgramRepository repo = new ProgramRepository();

            if (UserHelper.GetLoggedInUser() != null)
            {

                UserHelper.EmailFromSaleRepToAdmin_Cancellation(UserHelper.GetLoggedInUser().EmailAddress, prcvm);

                repo.CancelProgramRequest(prcvm);
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult Modify(int ProgramRequestID)
        {

            if (ProgramRequestID != 0)
            {
                ProgramRequestModifyVM PReqModify;
                ProgramRepository PRepo = new ProgramRepository();
                PReqModify = PRepo.GetProgramRequestModifybyID(ProgramRequestID);
                if (PReqModify != null)
                {

                    return View(PReqModify);
                }
                else
                    return View();
            }

            return View();
        }

        [HttpPost]
        public ActionResult Modify(ProgramRequestModifyVM prmvm)
        {
            ProgramRepository repo = new ProgramRepository();

            if (UserHelper.GetLoggedInUser() != null)
            {

                UserHelper.EmailFromSaleRepToAdmin_Modify(UserHelper.GetLoggedInUser().EmailAddress, prmvm);

                repo.ModifyProgramRequest(prmvm);
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult GetProgramRequest(int ProgramRequestID, int? IsAdmin)
        {
            var user = UserHelper.GetLoggedInUser();

            if (user != null)
            {
                ViewBag.id = user.UserID;

                if (ProgramRequestID != 0)
                {
                    EventRequestModel PReq;
                    ProgramRepository PRepo = new ProgramRepository();


                    PReq = PRepo.GetProgramRequestbyQueryString(ProgramRequestID);
                    ViewBag.ProgramName = PRepo.GetProgramName(PReq.ProgramID);

                    if (PReq != null)
                    {
                        if (IsAdmin == 1)
                        {
                            ViewBag.IsAdmin = IsAdmin;
                            int AdminID = user.UserID;

                            PReq.AdminUserID = AdminID;
                            PReq.IsAdmin = 1;

                        }
                        else
                        {

                            PReq.FromQueryStringBySalesRep = true;

                        }
                        return View("NewEventRequest", PReq);
                    }
                    else
                        return View("NewEventRequest");
                }
                else
                    return View("NewEventRequest");
            }
            return RedirectToAction("Login", "Account");
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


        public ActionResult Finance()
        {
            if (Session["ProgramID"] != null)
            {
                UserModel userModel = UserHelper.GetLoggedInUser();
                if (userModel == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                ViewBag.UserID = userModel.UserID;
                ViewBag.ProgramID = Session["ProgramID"].ToString();

                int UserId = Convert.ToInt32(UserHelper.GetLoggedInUser().UserID);
                int ProgramId = Convert.ToInt32(Session["ProgramID"]);

                ViewBag.ProgramRequestStatusCounts = UserHelper.GetProgramRequestStatusCounts(Convert.ToInt32(Session["ProgramID"]));

                var lst = (new FinanceRepository()).GetAllFinances(UserId, ProgramId);

                ViewBag.SumSubtotal = lst.Sum(item => item.SubTotal).ToString();
                ViewBag.SumTaxes = lst.Sum(item => item.TaxesCombined).ToString();
                ViewBag.SumEventTotal = lst.Sum(item => item.EventTotal).ToString();
            }

            return View();
        }


        public ActionResult GetFinanceInfo(string UserID, string ProgramID)
        {
            if (!string.IsNullOrEmpty(UserID) && !string.IsNullOrEmpty(ProgramID))
            {
                int UserId = Convert.ToInt32(UserID);
                int ProgramId = Convert.ToInt32(ProgramID);

                ViewBag.ProgramRequestStatusCounts = UserHelper.GetProgramRequestStatusCounts(Convert.ToInt32(Session["ProgramID"]));
                var lst = (new FinanceRepository()).GetAllFinances(UserId, ProgramId);
                return Json(new { data = lst }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { data = new Object() }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult GetProgramSessionPayment(int ProgramRequestID)
        {
            ProgramSessionPayment psu = new ProgramSessionPayment();
            ProgramRepository ur = new ProgramRepository();
            psu = ur.GetProgramSessionPayment(ProgramRequestID);
            return View(psu);
        }

    }
}
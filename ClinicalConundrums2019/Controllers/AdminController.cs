using ClinicalConundrums2019.DAL;
using ClinicalConundrums2019.Models;
using ClinicalConundrums2019.Util;
using ClinicalConundrums2019.ViewModels;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClinicalConundrums2019.Controllers
{

    public class AdminController : Controller
    {

        public FileResult OpenFile(string ProgramRequestID, string FileType, string FileExt)

        {
            string FileName = String.Empty;
            try

            {
                if (FileType == "Evaluation")
                    FileName = "Evaluation/EvaluationForm." + FileExt;
                else if (FileType == "SignIn")
                    FileName = "SignIn/SignIn." + FileExt;
                else if (FileType == "UserOther")
                    FileName = "UserOther/UserOther." + FileExt;
                else if ((FileType == "SpeakerAgreement"))
                    FileName = "SpeakerAgreement/SpeakerAgreement." + FileExt;
                else if ((FileType == "FinalAttendanceList"))
                    FileName = "FinalAttendanceList/FinalAttendanceList." + FileExt;

                //File Type: Evaluation,Other,SignIn
                string FileToOpen = Server.MapPath(ConfigurationManager.AppSettings["UserFileUploadPath"] + ProgramRequestID + "/" + FileName);

                if (System.IO.File.Exists(FileToOpen))
                {
                    return File(new FileStream(FileToOpen, FileMode.Open), "application/octetstream", FileToOpen);
                }
                return null;

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpGet]
        public ActionResult Index()
        {

            ProgramRepository repo = new ProgramRepository();
            ViewBag.ProgramList = repo.GetAllProgram();//populate bootstrap modal with all available programs
            return View();
        }


        public ActionResult Edit(int id)
        {
            ProgramRepository repo = new ProgramRepository();
            var model = repo.GetEditProgramRequestByID(id);
            return View("Edit", model);
        }
        //admin to make changes to post session values
        public ActionResult PostSession(int ProgramRequestID)
        {


            PostSessionRepository psr = new PostSessionRepository();
            PostSessionViewModel psvm = new PostSessionViewModel();
            if (ProgramRequestID != 0)
            {
                ProgramRepository pr = new ProgramRepository();
                SessionUpload su = new SessionUpload();
                ViewBag.ProgramRequestID = ProgramRequestID;
                ViewBag.UserID = UserHelper.GetLoggedInUser().UserID;

                su = pr.GetSessionUpload(ProgramRequestID);
                if (su != null)
                    ViewBag.SessionUpload = su;
                else
                {

                    ViewBag.SessionUpload = new SessionUpload()
                    {
                        EvaluationUploaded = false,
                        SignInUploaded = false,
                        UserOtherUploaded = false,
                        SpeakerAgreementUploaded = false,
                        FinalAttendanceListUploaded =false
                    };
                }
                //get fileupload information

                //ProgramRequestCancellationVM PReqCancel;
                //ProgramRepository PRepo = new ProgramRepository();
                //PReqCancel = PRepo.GetProgramRequestCancellationbyID(ProgramRequestID);
                //if (PReqCancel != null)
                //{

                //    return View(PReqCancel);
                //}
                //else

            }


            psvm = psr.GetPostSessionByProgramRequestID(ProgramRequestID);
            return View(psvm);
        }
        [HttpPost]
        public JsonResult PostSession(PostSessionViewModel psvm)
        {
            AdminRepository repo = new AdminRepository();
            repo.SavePostSession(psvm);
            var success = new { msg = "PostSession Data saved successfully" };
            return Json(new { success = success });


        }


        //handle the program request status update from the Admin Tool
        [HttpPost]
        public ActionResult UpdateProgramRequestStatus(ProgramRequestViewModel pr)
        {
            ProgramRepository repo = new ProgramRepository();
            AdminRepository Adminrepo = new AdminRepository();
            StatusChangeEmailViewModel sc = new StatusChangeEmailViewModel();

            int ProgramRequestID = pr.ProgramRequestID;
            int StatusId = pr.RequestStatus;

            bool ConfirmSessionDate = Adminrepo.CheckConfirmedSessionDate(ProgramRequestID);
            bool AdminSessionID = Adminrepo.CheckAdminSessionID(ProgramRequestID);
            //AdminSessionID = true;
            //cancelled or under review
            if (StatusId == 6 || StatusId == 1)
            {


                repo.UpdateRequestStatusByAdmin(ProgramRequestID, StatusId);

                return Json(new { result = "success" });

            }


            if (ConfirmSessionDate)
            {
                //Active – Regional Ethics Review Pending
                if (StatusId == 2)
                {
                    //Your Event Information has been submitted for Regional Ethics Review
                    repo.UpdateRequestStatusByAdmin(ProgramRequestID, StatusId);
                    sc = Adminrepo.GetStatusChangeByAdminEmail(ProgramRequestID);
                    //production import
                    //Dija
                   UserHelper.AdminChangeRequestStatusID2(sc);
                    return Json(new { result = "success" });

                }

                if (StatusId == 5)
                {

                    //Please upload the post-program materials 
                    repo.UpdateRequestStatusByAdmin(ProgramRequestID, StatusId);
                    sc = Adminrepo.GetStatusChangeByAdminEmail(ProgramRequestID);
                    //production import
                    //Dija
                   UserHelper.AdminChangeRequestStatusID5(sc);
                    return Json(new { result = "success" });

                }

                if (StatusId == 4)
                {
                    //Completed – Session Closed

                    repo.UpdateRequestStatusByAdmin(ProgramRequestID, StatusId);

                    return Json(new { result = "success" });

                }



                if ((StatusId == 3) && (AdminSessionID == true))
                {

                    //Your event has received regional ethics approval  
                    repo.UpdateRequestStatusByAdmin(ProgramRequestID, StatusId);
                    sc = Adminrepo.GetStatusChangeByAdminEmail(ProgramRequestID);
                    //production import
                    //Dija
                   UserHelper.AdminChangeRequestStatusID3(sc);
                    return Json(new { result = "success" });
                }
                else
                {

                    return Json(new { error = "AdminSessionID" });

                }



            }
            else
            {
                return Json(new { error = "error" });


            }



        }





        [HttpPost]
        public ActionResult Edit(ProgramRequestViewModel pr)
        {
            ProgramRepository repo = new ProgramRepository();
            if (ModelState.IsValid)
            {
                repo.EditProgramRequest(pr);
                return Json(new { result = "Success" });

            }

            return Json(new { error = "error" });

        }

        [HttpPost]
        public ActionResult Approved(ProgramRequestViewModel pr)
        {
            ProgramRepository repo = new ProgramRepository();

            repo.ApproveProgramRequest(pr);
            return Json(new { result = "Success" });

        }


        #region Users

        public ActionResult GetAllUsersExceptSpeakers()
        {

            List<UserModel> liUserModel;

            UserRepository ur = new UserRepository();
            liUserModel = ur.GetAllUsersExceptSpeakers();

            return Json(new { data = liUserModel }, JsonRequestBehavior.AllowGet);


        }

        public ActionResult EditUser(int ID)
        {
            UserModel um;
            UserRepository repo = new UserRepository();

            um = repo.GetUserByID(ID);

            return View(um);
        }

        [HttpPost]
        public ActionResult EditUser(UserViewModel um)
        {
            try
            {
                UserRepository repo = new UserRepository();
                if (ModelState.IsValid)
                {
                    repo.EditUser(um);
                    return Json(new { result = true }, JsonRequestBehavior.AllowGet);

                }
                else
                {

                    return Json(new { error = "error" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e)
            {

                Console.WriteLine("Error:" + e.Message);
                return Json(new { error = "error" }, JsonRequestBehavior.AllowGet);
            }
            // return Json(new { result = true }, JsonRequestBehavior.AllowGet);


        }


        public ActionResult AddUser()
        {
            return View();
        }
        public ActionResult Reports()
        {
            return View();
        }
        public ActionResult ExportToExcel()
        {


            try
            {


                AdminRepository ar = new AdminRepository();
                //need to use nuget manager to install EPPlus
                ExcelPackage pck = new ExcelPackage();
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("ProgramRequestReport");
                ws.Cells["A1"].Value = "Record ID";
                ws.Cells["B1"].Value = "Date Submitted";
                ws.Cells["C1"].Value = "Contact First Name";
                ws.Cells["D1"].Value = "Contact Last Name";
                ws.Cells["E1"].Value = "Program Status";
                ws.Cells["F1"].Value = "Program Date";
                ws.Cells["G1"].Value = "Program Location";
                ws.Cells["H1"].Value = "Speaker";
                ws.Cells["I1"].Value = "Moderator";
                ws.Cells["J1"].Value = "Location";
                ws.Cells["K1"].Value = "City";
                ws.Cells["L1"].Value = "Province";
                ws.Cells["M1"].Value = "Final Attendance";
                ws.Cells["N1"].Value = "Company Name";
                int rowStart = 2;
                foreach (var item in ar.GetProgramRequestReport())
                {
                    ws.Row(rowStart).Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Row(rowStart).Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("white")));
                    ws.Cells[string.Format("A{0}", rowStart)].Value = item.ProgramRequestID;
                    ws.Cells[string.Format("B{0}", rowStart)].Value = item.SubmittedDate;
                    ws.Cells[string.Format("C{0}", rowStart)].Value = item.ContactFirstName;
                    ws.Cells[string.Format("D{0}", rowStart)].Value = item.ContactLastName;
                    ws.Cells[string.Format("E{0}", rowStart)].Value = item.RequestStatus;
                    ws.Cells[string.Format("F{0}", rowStart)].Value = item.ConfirmedProgramDate;
                    ws.Cells[string.Format("G{0}", rowStart)].Value = item.LocationName;
                    ws.Cells[string.Format("H{0}", rowStart)].Value = item.Speaker;
                    ws.Cells[string.Format("I{0}", rowStart)].Value = item.Moderator;
                    ws.Cells[string.Format("J{0}", rowStart)].Value = item.LocationAddress;
                    ws.Cells[string.Format("K{0}", rowStart)].Value = item.LocationCity;
                    ws.Cells[string.Format("L{0}", rowStart)].Value = item.LocationProvince;
                    ws.Cells[string.Format("M{0}", rowStart)].Value = item.FinalAttendance;
                    ws.Cells[string.Format("N{0}", rowStart)].Value = item.CompanyName;
                    rowStart = rowStart + 1;
                }
                //  ws.Cells["F1"].Value = string.Format("{0:dd MMMM yyyy} at ")

                ws.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();

                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=ProgramRequestReport.xlsx");
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.BinaryWrite(pck.GetAsByteArray());
                Response.Flush();
                Response.End();
                return View("Reports");
            }
            catch (Exception e)
            {
                UserHelper.WriteToLog("Error Message:" + e.Message);
                return View("Reports");
            }

        }
        public ActionResult ExportUsersToExcel()
        {


            try
            {


                AdminRepository ar = new AdminRepository();
                //need to use nuget manager to install EPPlus
                ExcelPackage pck = new ExcelPackage();
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("UserReport");
                ws.Cells["A1"].Value = "ID";
                ws.Cells["B1"].Value = "UserID";
                ws.Cells["C1"].Value = "UserType";
                ws.Cells["D1"].Value = "TerritoryID";
                ws.Cells["E1"].Value = "RepID";
                ws.Cells["F1"].Value = "First Name";
                ws.Cells["G1"].Value = "Last Name";
                ws.Cells["H1"].Value = "Email Address";
                ws.Cells["I1"].Value = "Company";
                ws.Cells["J1"].Value = "Address";
                ws.Cells["K1"].Value = "City";
                ws.Cells["L1"].Value = "Province";
                ws.Cells["M1"].Value = "Phone";
                ws.Cells["N1"].Value = "Activated";
                int rowStart = 2;
                foreach (var item in ar.GetUserReport())
                {
                    ws.Row(rowStart).Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Row(rowStart).Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml(string.Format("white")));
                    ws.Cells[string.Format("A{0}", rowStart)].Value = item.ID;
                    ws.Cells[string.Format("B{0}", rowStart)].Value = item.UserID;
                    ws.Cells[string.Format("C{0}", rowStart)].Value = item.UserType;
                    ws.Cells[string.Format("D{0}", rowStart)].Value = item.TerritoryID;
                    ws.Cells[string.Format("E{0}", rowStart)].Value = item.RepID;
                    ws.Cells[string.Format("F{0}", rowStart)].Value = item.FirstName;
                    ws.Cells[string.Format("G{0}", rowStart)].Value = item.LastName;
                    ws.Cells[string.Format("H{0}", rowStart)].Value = item.EmailAddress;
                    ws.Cells[string.Format("I{0}", rowStart)].Value = item.ClinicName;
                    ws.Cells[string.Format("J{0}", rowStart)].Value = item.Address;
                    ws.Cells[string.Format("K{0}", rowStart)].Value = item.City;
                    ws.Cells[string.Format("L{0}", rowStart)].Value = item.Province;
                    ws.Cells[string.Format("M{0}", rowStart)].Value = item.Phone;
                    ws.Cells[string.Format("N{0}", rowStart)].Value = item.Activated;
                    rowStart = rowStart + 1;
                }
                //  ws.Cells["F1"].Value = string.Format("{0:dd MMMM yyyy} at ")

                ws.Cells["A:AZ"].AutoFitColumns();
                Response.Clear();

                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=CCUserReport.xlsx");
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.BinaryWrite(pck.GetAsByteArray());
                Response.Flush();
                Response.End();
                return View("Reports");
            }
            catch (Exception e)
            {
                UserHelper.WriteToLog("Error Message:" + e.Message);
                return View("Reports");
            }

        }
        [HttpPost]
        public ActionResult AddUser(UserViewModel um)
        {

            try
            {
                UserRepository repo = new UserRepository();
                if (!ModelState.IsValid)
                {
                    return Json(new { error = "error" }, JsonRequestBehavior.AllowGet);
                    // return View("AddUser",um);

                }
                else
                {
                    ViewBag.SystemMsg = "";
                    repo.AddUser(um);
                    return Json(new { result = true }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e)
            {
                return Json(new { error = "error" }, JsonRequestBehavior.AllowGet);

                //Console.WriteLine("Error:" + e.Message);
                //ViewBag.ShowSystemMsg = "true";
                //Console.WriteLine("ViewBag.SystemMsg:" + ViewBag.SystemMsg);
                ////  ModelState.AddModelError("", e.Message);
                //// ModelState.AddModelError("", "Duplicate UserName");
                //return View("AddUser", um);
            }
            // return Json(new { result = true }, JsonRequestBehavior.AllowGet);


        }
        public ActionResult Users()
        {


            return View();
        }
        [HttpPost]
        public ActionResult DeleteUser(UserModel um)
        {
            ProgramRepository repo = new ProgramRepository();

            repo.DeleteUser(um);
            return Json(new { result = "Success" });

        }

        [HttpPost]
        public ActionResult UpdateUserActivation(ActivationModel model)
        {
            UserRepository repo = new UserRepository();

            repo.UpdateUserActivation(model);
            return Json(new { result = "Success" });
        }

        #endregion
        #region Speakers
        [HttpPost]
        public ActionResult ApproveSpeaker(SpeakerViewModel spVM)
        {
            UserRepository repo = new UserRepository();
            UserModel UMSpeaker = new UserModel();
            UserModel UMSalesRep = new UserModel();

            repo.ApproveSpeaker(spVM);
            UMSpeaker = repo.GetUserByuserid(spVM.ID);

            //send email to EmailSalesRep_SpeakerApproved

            UMSalesRep = repo.GetUserByUserID(UMSpeaker.UserIDRequestedBy);

            UserHelper.EmailSaleRep_SpeakerApproved(UMSalesRep, UMSpeaker);

            //send email to Speaker. this include activation or optout.

            UserHelper.EmailSpeaker_SpeakerApproved(UMSpeaker);

            return Json(new { result = "Success" });

        }

        [HttpPost]
        public ActionResult EditSpeaker(SpeakerViewModel spVM)
        {

            try
            {
                UserRepository repo = new UserRepository();
                if (ModelState.IsValid)
                {
                    repo.EditSpeaker(spVM);
                    return Json(new { result = true }, JsonRequestBehavior.AllowGet);

                }
                else
                {

                    return Json(new { error = "error" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e)
            {

                Console.WriteLine("Error:" + e.Message);
                return Json(new { error = "error" }, JsonRequestBehavior.AllowGet);
            }
            // return Json(new { result = true }, JsonRequestBehavior.AllowGet);


        }
        [HttpPost]
        public ActionResult AddSpeaker(SpeakerViewModel spVM)
        {

            try
            {
                UserRepository repo = new UserRepository();
                UserModel um = new UserModel();
                if (ModelState.IsValid)
                {
                    int id = repo.AddSpeaker(spVM);

                    um = repo.GetUserByuserid(id);
                    UserHelper.EmailSpeaker_SpeakerApproved(um);

                    return Json(new { result = true }, JsonRequestBehavior.AllowGet);

                }
                else
                {

                    return Json(new { error = "error" }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e)
            {

                Console.WriteLine("Error:" + e.Message);
                return Json(new { error = "error" }, JsonRequestBehavior.AllowGet);
            }
            // return Json(new { result = true }, JsonRequestBehavior.AllowGet);


        }

        public ActionResult AddSpeaker()
        {

            SpeakerViewModel spVM = new SpeakerViewModel();
            spVM.UserType = 2;//default to speaker

            return View(spVM);

        }
        [HttpGet]
        public ActionResult EditSpeaker(int userid)
        {
            UserModel um;
            UserRepository repo = new UserRepository();

            um = repo.GetUserByuserid(userid);

            return View("EditSpeaker", um);
        }


        [HttpGet]
        public ActionResult EditPayee(int UserID)
        {
            PayeeModel pm;
            AdminRepository repo = new AdminRepository();

            pm = repo.GetPayeeByUserID(UserID);

            return View("EditPayee", pm);
        }



        [HttpPost]
        public ActionResult EditPayee(PayeeModel pm)
        {

            try
            {
                AdminRepository repo = new AdminRepository();

                if (repo.UpdatePayee(pm))

                    return Json(new { result = true }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { error = "error" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                return Json(new { error = "error" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetAllSpeakers()
        {

            List<UserModel> liUserModel;

            UserRepository ur = new UserRepository();
            liUserModel = ur.GetAllSpeakers();

            return Json(new { data = liUserModel }, JsonRequestBehavior.AllowGet);


        }



        public ActionResult Speakers()
        {

            ViewBag.SpeakerCOIURL = ConfigurationManager.AppSettings["SpeakerCOIURL"];

            return View();
        }



        // GET: COIUpload
        [HttpGet]
        public ActionResult COIForm(int UserID)
        {
            ViewBag.UserID = UserID;
            ViewBag.SpeakerCOIURL = ConfigurationManager.AppSettings["SpeakerCOIURL"];

            AdminRepository ar = new AdminRepository();
            UserRegistration ur = ar.GetUserRegistration(UserID);

            var list = ar.GetProgramList(UserID);
            ViewBag.ProgramList = list;

            if (ur != null)

                return View(ur);
            else
                return View(new UserRegistration());
            //um = repo.GetUserByUserID(UserID);

            //ProgramRepository pr = new ProgramRepository();
            //SessionUpload su = new SessionUpload();

            //su = pr.GetSessionUpload(ProgramRequestID);
            //if (su != null)
            //    return View(su);
            //else
            //{

            //    return View(new SessionUpload());
            //}
            //get fileupload information

            //ProgramRequestCancellationVM PReqCancel;
            //ProgramRepository PRepo = new ProgramRepository();
            //PReqCancel = PRepo.GetProgramRequestCancellationbyID(ProgramRequestID);
            //if (PReqCancel != null)
            //{

            //    return View(PReqCancel);
            //}
            //else

        }



        //handle COI form upload
        public JsonResult COIUpload(int UserID)
        {

            string solutiondir = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            string SpeakerSite = "CPDPortalSpeaker";
            string COIFormPath = Server.MapPath(solutiondir + "/" + SpeakerSite + ConfigurationManager.AppSettings["UserFileUploadPath"] + UserID + "/COIForm");

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
                        var supportedTypes = new[] { "doc", "docx", "pdf", "DOC", "DOCX", "PDF" };
                        var extension = Path.GetExtension(file).Substring(1);
                        if (!supportedTypes.Contains(extension))
                        {
                            var error = "File Extension Is InValid";
                            return Json(new { error = error });
                        }



                        //check if the folder exists, if not then make one. 
                        //  string FolderPath = Server.MapPath(ConfigurationManager.AppSettings["UserFileUploadPath"] + id + "/ProgramID/" + programId + "/");


                        if (!Directory.Exists(COIFormPath))
                        {
                            Directory.CreateDirectory(COIFormPath);
                        }
                        else
                        {

                            //let's clean up the directory before accept the new file
                            System.IO.DirectoryInfo di = new DirectoryInfo(COIFormPath);
                            foreach (FileInfo ff in di.EnumerateFiles())
                            {
                                ff.Delete();
                            }

                        }
                        // get a stream
                        var stream = fileContent.InputStream;
                        // and optionally write the file to disk
                        var fileName = Path.GetFileName(file);
                        //rename files to prevent user upload unlimited number of different files into the system.
                        string path = COIFormPath + "/" + "COIForm." + extension;
                        //var path = Path.Combine(Server.MapPath(EvaluationPath + "/"), fileName);
                        using (var fileStream = System.IO.File.Create(path))
                        {
                            stream.CopyTo(fileStream);
                        }
                        //save file full path and uploaded flag to db
                        //SessionUpload objSessionUpload = new SessionUpload();
                        //objSessionUpload.ProgramRequestID = Convert.ToInt32(ProgramRequestID);
                        //objSessionUpload.EvaluationFullPath = path;//full path
                        //objSessionUpload.EvaluationUploaded = true;
                        //objSessionUpload.EvaluationFileName = fileName;
                        //objSessionUpload.EvaluationFileExt = extension;
                        AdminRepository pr = new AdminRepository();

                        pr.UpdateCOIForm(UserID, extension);

                        returnPath = COIFormPath + "\\" + "COIForm." + extension;
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
        #endregion Speakers

    }
}

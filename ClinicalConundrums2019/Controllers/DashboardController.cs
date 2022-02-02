using ClinicalConundrums2019.DAL;
using ClinicalConundrums2019.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClinicalConundrums2019.Util;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace ClinicalConundrums2019.Controllers
{
    public class DashboardController : Controller
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
        //handle Final Attendance List
        public JsonResult FinalAttendanceListUpload(string ProgramRequestID)
        {
            string FinalAttendanceListPath = Server.MapPath(ConfigurationManager.AppSettings["UserFileUploadPath"] + ProgramRequestID + "/FinalAttendanceList");

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
                        var supportedTypes = new[] { "doc", "docx", "pdf", "DOC", "DOCX", "PDF","xls","xlsx" };
                        var extension = Path.GetExtension(file).Substring(1);
                        if (!supportedTypes.Contains(extension))
                        {
                            var error = "File Extension Is InValid";
                            return Json(new { error = error });
                        }



                        //check if the folder exists, if not then make one. 
                        //  string FolderPath = Server.MapPath(ConfigurationManager.AppSettings["UserFileUploadPath"] + id + "/ProgramID/" + programId + "/");


                        if (!Directory.Exists(FinalAttendanceListPath))
                        {
                            Directory.CreateDirectory(FinalAttendanceListPath);
                        }
                        else
                        {

                            //let's clean up the directory before accept the new file
                            System.IO.DirectoryInfo di = new DirectoryInfo(FinalAttendanceListPath);
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
                        string path = FinalAttendanceListPath + "/" + "FinalAttendanceList." + extension;
                        //var path = Path.Combine(Server.MapPath(EvaluationPath + "/"), fileName);
                        using (var fileStream = System.IO.File.Create(path))
                        {
                            stream.CopyTo(fileStream);
                        }
                        //save file full path and uploaded flag to db
                        SessionUpload objSessionUpload = new SessionUpload();
                        objSessionUpload.ProgramRequestID = Convert.ToInt32(ProgramRequestID);
                        objSessionUpload.FinalAttendanceListFullPath = path;//full path
                        objSessionUpload.FinalAttendanceListUploaded = true;
                        objSessionUpload.FinalAttendanceListFileName = fileName;
                        objSessionUpload.FinalAttendanceListFileExt = extension;
                        ProgramRepository pr = new ProgramRepository();

                        pr.UpdateProgramRequestFileUpload(objSessionUpload);

                        returnPath = FinalAttendanceListPath + "\\" + "FinalAttendanceList." + extension;
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
        //handle evaluation form upload
        public JsonResult EvaluationFormUpload(string ProgramRequestID)
        {
            string EvaluationPath = Server.MapPath(ConfigurationManager.AppSettings["UserFileUploadPath"] + ProgramRequestID + "/Evaluation");

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
                        var supportedTypes = new[] { "doc", "docx", "pdf", "DOC", "DOCX", "PDF" ,"xls","xlsx"};
                        var extension = Path.GetExtension(file).Substring(1);
                        if (!supportedTypes.Contains(extension))
                        {
                            var error = "File Extension Is InValid";
                            return Json(new { error = error });
                        }



                        //check if the folder exists, if not then make one. 
                        //  string FolderPath = Server.MapPath(ConfigurationManager.AppSettings["UserFileUploadPath"] + id + "/ProgramID/" + programId + "/");


                        if (!Directory.Exists(EvaluationPath))
                        {
                            Directory.CreateDirectory(EvaluationPath);
                        }
                        else
                        {

                            //let's clean up the directory before accept the new file
                            System.IO.DirectoryInfo di = new DirectoryInfo(EvaluationPath);
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
                        string path = EvaluationPath + "/" + "EvaluationForm." + extension;
                        //var path = Path.Combine(Server.MapPath(EvaluationPath + "/"), fileName);
                        using (var fileStream = System.IO.File.Create(path))
                        {
                            stream.CopyTo(fileStream);
                        }
                        //save file full path and uploaded flag to db
                        SessionUpload objSessionUpload = new SessionUpload();
                        objSessionUpload.ProgramRequestID = Convert.ToInt32(ProgramRequestID);
                        objSessionUpload.EvaluationFullPath = path;//full path
                        objSessionUpload.EvaluationUploaded = true;
                        objSessionUpload.EvaluationFileName = fileName;
                        objSessionUpload.EvaluationFileExt = extension;
                        ProgramRepository pr = new ProgramRepository();

                        pr.UpdateProgramRequestFileUpload(objSessionUpload);

                        returnPath = EvaluationPath + "\\" + "EvaluationForm." + extension;
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

        //handle signIn form upload
        public JsonResult SignInUpload(string ProgramRequestID)
        {
            string SignInPath = Server.MapPath(ConfigurationManager.AppSettings["UserFileUploadPath"] + ProgramRequestID + "/SignIn");

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
                        var supportedTypes = new[] { "doc", "docx", "pdf", "xls", "xlsx" };
                        var extension = Path.GetExtension(file).Substring(1);
                        if (!supportedTypes.Contains(extension))
                        {
                            var error = "File Extension Is InValid";
                            return Json(new { error = error });
                        }



                        //check if the folder exists, if not then make one. 
                        //  string FolderPath = Server.MapPath(ConfigurationManager.AppSettings["UserFileUploadPath"] + id + "/ProgramID/" + programId + "/");


                        if (!Directory.Exists(SignInPath))
                        {
                            Directory.CreateDirectory(SignInPath);
                        }
                        else
                        {

                            //let's clean up the directory before accept the new file
                            System.IO.DirectoryInfo di = new DirectoryInfo(SignInPath);
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
                        string path = SignInPath + "/" + "SignIn." + extension;
                        //var path = Path.Combine(Server.MapPath(EvaluationPath + "/"), fileName);
                        using (var fileStream = System.IO.File.Create(path))
                        {
                            stream.CopyTo(fileStream);
                        }
                        //save file full path and uploaded flag to db
                        SessionUpload objSessionUpload = new SessionUpload();
                        objSessionUpload.ProgramRequestID = Convert.ToInt32(ProgramRequestID);
                        objSessionUpload.SignInFullPath = path;//full path
                        objSessionUpload.SignInUploaded = true;
                        objSessionUpload.SignInFileName = fileName;
                        objSessionUpload.SignInFileExt = extension;
                        ProgramRepository pr = new ProgramRepository();

                        pr.UpdateProgramRequestFileUpload(objSessionUpload);

                        returnPath = SignInPath + "\\" + "SignIn." + extension;
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
        public JsonResult SpeakerAgreementUpload(string ProgramRequestID)
        {
            string SpeakerAgreementPath = Server.MapPath(ConfigurationManager.AppSettings["UserFileUploadPath"] + ProgramRequestID + "/SpeakerAgreement");

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
                        //  string FolderPath = Server.MapPath(ConfigurationManager.AppSettings["UserFileUploadPath"] + id + "/ProgramID/" + programId + "/");


                        if (!Directory.Exists(SpeakerAgreementPath))
                        {
                            Directory.CreateDirectory(SpeakerAgreementPath);
                        }
                        else
                        {

                            //let's clean up the directory before accept the new file
                            System.IO.DirectoryInfo di = new DirectoryInfo(SpeakerAgreementPath);
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
                        string path = SpeakerAgreementPath + "/" + "SpeakerAgreement." + extension;
                        //var path = Path.Combine(Server.MapPath(EvaluationPath + "/"), fileName);
                        using (var fileStream = System.IO.File.Create(path))
                        {
                            stream.CopyTo(fileStream);
                        }
                        //save file full path and uploaded flag to db
                        SessionUpload objSessionUpload = new SessionUpload();
                        objSessionUpload.ProgramRequestID = Convert.ToInt32(ProgramRequestID);
                        objSessionUpload.SpeakerAgreementFullPath = path;//full path
                        objSessionUpload.SpeakerAgreementUploaded = true;
                        objSessionUpload.SpeakerAgreementFileName = fileName;
                        objSessionUpload.SpeakerAgreementFileExt = extension;
                        ProgramRepository pr = new ProgramRepository();

                        pr.UpdateProgramRequestFileUpload(objSessionUpload);

                        returnPath = SpeakerAgreementPath + "\\" + "SpeakerAgreement." + extension;
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
        //handle other upload
        public JsonResult UserOtherUpload(string ProgramRequestID)
        {
            string UserOtherPath = Server.MapPath(ConfigurationManager.AppSettings["UserFileUploadPath"] + ProgramRequestID + "/UserOther");

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
                        //  string FolderPath = Server.MapPath(ConfigurationManager.AppSettings["UserFileUploadPath"] + id + "/ProgramID/" + programId + "/");


                        if (!Directory.Exists(UserOtherPath))
                        {
                            Directory.CreateDirectory(UserOtherPath);
                        }
                        else
                        {

                            //let's clean up the directory before accept the new file
                            System.IO.DirectoryInfo di = new DirectoryInfo(UserOtherPath);
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
                        string path = UserOtherPath + "/" + "UserOther." + extension;
                        //var path = Path.Combine(Server.MapPath(EvaluationPath + "/"), fileName);
                        using (var fileStream = System.IO.File.Create(path))
                        {
                            stream.CopyTo(fileStream);
                        }
                        //save file full path and uploaded flag to db
                        SessionUpload objSessionUpload = new SessionUpload();
                        objSessionUpload.ProgramRequestID = Convert.ToInt32(ProgramRequestID);
                        objSessionUpload.UserOtherFullPath = path;//full path
                        objSessionUpload.UserOtherUploaded = true;
                        objSessionUpload.UserOtherFileName = fileName;
                        objSessionUpload.UserOtherFileExt = extension;
                        ProgramRepository pr = new ProgramRepository();

                        pr.UpdateProgramRequestFileUpload(objSessionUpload);

                        returnPath = UserOtherPath + "\\" + "UserOther." + extension;
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

        // GET: Dashboard
        public ActionResult Index()
        {

            try
            {
                if (Session["ProgramID"] != null)
                {
                    ViewBag.UserID = UserHelper.GetLoggedInUser().UserID;
                    ViewBag.ProgramID = Session["ProgramID"].ToString();

                    ViewBag.ProgramRequestStatusCounts = UserHelper.GetProgramRequestStatusCounts(Convert.ToInt32(Session["ProgramID"]));

                }
                else
                {
                    return RedirectToAction("Index", "Home");

                }
                // ViewBag.ProgramList = repo.GetAllDashboardItems();//populate bootstrap modal with all available programs

            }
            catch (Exception exc)
            {
                ViewBag.Error = "Error:" + exc.Message;
            }
            return View();
        }
        public ActionResult FileUpload(int UserID, int ProgramRequestID)
        {
            //UserModel um;
            //UserRepository repo = new UserRepository();
            ViewBag.UserID = UserID;
            ViewBag.ProgramRequestID = ProgramRequestID;
            //um = repo.GetUserByUserID(UserID);
            if (ProgramRequestID != 0)
            {
                ProgramRepository pr = new ProgramRepository();
                SessionUpload su = new SessionUpload();

                su = pr.GetSessionUpload(ProgramRequestID);
                if (su != null)
                    return View(su);
                else
                {

                    return View(new SessionUpload());
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

            return View();
        }
        public ActionResult GetAllDashboardItems(int UserID, int ProgramID)
        {

            List<DashboardItem> liDashboardItems;
            if (Session["ProgramID"] != null)
                ViewBag.ProgramRequestStatusCounts = UserHelper.GetProgramRequestStatusCounts(Convert.ToInt32(Session["ProgramID"]));
            else
                return RedirectToAction("Index", "Home");


            // int UserID = UserHelper.GetLoggedInUser().UserID;

            DashboardRepository repo = new DashboardRepository();
            liDashboardItems = repo.GetAllDashboardItems(UserID, ProgramID);

            return Json(new { data = liDashboardItems }, JsonRequestBehavior.AllowGet);


        }

      
    }
}
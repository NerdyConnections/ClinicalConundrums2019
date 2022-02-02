
using ClinicalConundrumsSpeaker.CustomAttribute;
using ClinicalConundrumsSpeaker.DAL;
using ClinicalConundrumsSpeaker.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http.Cors;
using System.Web.Mvc;

namespace ClinicalConundrumSpeaker.Controllers
{
    //[AllowAnonymous]
    //[AllowCrossSiteJson]

    public class FileUploadController : Controller
    {
        // GET: FileUpload
        public ActionResult Index()
        {
           

            return View();
        }
        [AllowCrossSiteJson]
        public FileResult OpenFile(string UserID, string FileType, string FileExt)

        {
            string FileName = String.Empty;
            string FileToOpen = string.Empty;
            try

            {
                if (FileType == "COIForm")
                {
                    FileName = "COIForm/COIForm." + FileExt;
                    FileToOpen = Server.MapPath(ConfigurationManager.AppSettings["UserFileUploadPath"] + UserID + "/" + FileName);
                }
                else if (FileType == "COISlides")
                {
                    FileName = "COISlides." + FileExt;
                    FileToOpen = Server.MapPath("/COISlides/" + UserID + "/" + FileName);


                }
                Console.WriteLine("file to open:" + FileName);
                Console.WriteLine("file path to open:" + FileToOpen);
                //File Type: Evaluation,Other,SignIn


                if (System.IO.File.Exists(FileToOpen))
                {
                    Console.WriteLine("File Exist");
                    return File(new FileStream(FileToOpen, FileMode.Open), "application/octetstream", FileToOpen);
                }
                else
                {
                    Response.Write("file not found:" + FileToOpen);

                }
                return null;

            }

            catch (Exception ex)
            {
                Console.WriteLine("exception:" + ex.Message);
                throw ex;
            }
        }

        //[AllowCrossSiteJson]
        //public string[] GetCOIFileList(int UserID)
        //{
        //    List<string> RetList = new List<string>();

        //    String Path = Server.MapPath("/COISlides/" + "117");

        //    string[] FileNames = Directory.GetFiles(Path);




        //    return FileNames;

        //}

        [AllowCrossSiteJson]
        public JsonResult COIForm(int UserID)
        {
            //handle COI form uploadC:\Users\laia\Documents\Visual Studio 2015\Projects\ClinicalConundrums2019\ClinicalConundrumsSpeaker\Controllers\AccountController.cs

            
            string COIFormPath = Server.MapPath(ConfigurationManager.AppSettings["UserFileUploadPath"] + UserID + "/COIForm");

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
                        var supportedTypes = new[] { "doc", "docx", "pdf","DOC","DOCX","PDF" };
                        var extension = Path.GetExtension(file).Substring(1);
                        if (!supportedTypes.Contains(extension))
                        {
                            var error = "File Extension Is InValid";
                            return Json(new { error = error }, JsonRequestBehavior.AllowGet);
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
                        FileUploadRepository fur = new FileUploadRepository();

                        fur.UpdateCOIForm(UserID, extension);

                        returnPath = COIFormPath + "\\" + "COIForm." + extension;
                        UploadedFileName = fileName;
                    }
                }
            }
            catch (Exception e)
            {
                //Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return Json("Upload failed", JsonRequestBehavior.AllowGet);
            }

            var success = new { msg = "File uploaded successfully", returnFileName = returnPath, uploadedFilename = UploadedFileName };
            return Json(new { success = success }, JsonRequestBehavior.AllowGet);
        }
       // [AllowCrossSiteJson]
      // [EnableCors(origins: "*", headers: "*", methods: "*")]
        public JsonResult COISlides2(int UserID, int ProgramID)
        {
            //handle COI form upload
            //  HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");

            UserHelper.WriteToLog("Uploading COISlide...");

            string COISlidesPath = Server.MapPath("/COISlides/" + UserID + "/" + ProgramID);


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
                        var supportedTypes = new[] { "pptx", "ppt" };
                        var extension = Path.GetExtension(file).Substring(1);
                        if (!supportedTypes.Contains(extension))
                        {
                            var error = "File Extension Is InValid";
                            return Json(new { error = error }, JsonRequestBehavior.AllowGet);
                        }



                        //check if the folder exists, if not then make one. 
                        //  string FolderPath = Server.MapPath(ConfigurationManager.AppSettings["UserFileUploadPath"] + id + "/ProgramID/" + programId + "/");


                        if (!Directory.Exists(COISlidesPath))
                        {
                            Directory.CreateDirectory(COISlidesPath);
                        }
                        else
                        {

                            //let's clean up the directory before accept the new file
                            System.IO.DirectoryInfo di = new DirectoryInfo(COISlidesPath);
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
                        string path = COISlidesPath + "/" + "COISlides." + extension;
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
                        FileUploadRepository fur = new FileUploadRepository();
                        extension = "pptx";
                        fur.UpdateCOISlides(UserID, ProgramID, extension);

                        returnPath = COISlidesPath + "\\" + " COISlides." + extension;
                        UploadedFileName = fileName;
                    }
                }
            }
            catch (Exception e)
            {
                //Response.StatusCode = (int)HttpStatusCode.BadRequest;
                UserHelper.WriteToLog("Error in Uploading COISlide..." + e.Message);
                return Json("Upload failed", JsonRequestBehavior.AllowGet);
            }
            //HttpContext.Response.AppendHeader("Access - Control - Allow - Origin","*");
            var success = new { msg = "File uploaded successfully", returnFileName = returnPath, uploadedFilename = UploadedFileName };
            return Json(new { success = success }, JsonRequestBehavior.AllowGet);
        }
       // [AllowCrossSiteJson]
        //[EnableCors(origins: "*", headers: "*", methods: "*")]
        public JsonResult GetCOIFileNames(int UserID)
        {

            List<string> RetList = new List<string>();
            List<Tuple<string, string >> Programlist = new List<Tuple<string, string>>();
            FileUploadRepository repo = new FileUploadRepository();
            try
            {
                //String Path =  Server.MapPath(ConfigurationManager.AppSettings["UserFileUploadPath"] + "COISlides/" + UserID);

                String Path = Server.MapPath("/COISlides/" + UserID);

                string[] FileNames = Directory.GetFiles(Path);

                foreach (string s in Directory.GetDirectories(Path))
                {

                    RetList.Add((s.Remove(0, s.LastIndexOf('\\') + 1)));

                }


                for (var i = 0; i < RetList.Count; i++)
                {
                    string filename = repo.GetProgramNames(RetList[i]);

                    Programlist.Add(new Tuple<string, string>(RetList[i], filename));

                }
            }catch (Exception e)
            {

                UserHelper.WriteToLog("GetCOIFileNames..." + e.Message);
            }
           
            
            return Json(new { list = Programlist }, JsonRequestBehavior.AllowGet);
        }

        [AllowCrossSiteJson]
        public JsonResult COISlides(int UserID, int ProgramID)
        {
            //handle COI form upload
          //  HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");

            UserHelper.WriteToLog("Uploading COISlide...");

            string COISlidesPath = Server.MapPath("/COISlides/" + UserID + "/" + ProgramID);


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
                        var supportedTypes = new[] { "pptx", "ppt" };
                        var extension = Path.GetExtension(file).Substring(1);
                        if (!supportedTypes.Contains(extension))
                        {
                            var error = "File Extension Is InValid";
                            return Json(new { error = error }, JsonRequestBehavior.AllowGet);
                        }



                        //check if the folder exists, if not then make one. 
                        //  string FolderPath = Server.MapPath(ConfigurationManager.AppSettings["UserFileUploadPath"] + id + "/ProgramID/" + programId + "/");


                        if (!Directory.Exists(COISlidesPath))
                        {
                            Directory.CreateDirectory(COISlidesPath);
                        }
                        else
                        {

                            //let's clean up the directory before accept the new file
                            System.IO.DirectoryInfo di = new DirectoryInfo(COISlidesPath);
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
                        string path = COISlidesPath + "/" + "COISlides." + extension;
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
                        FileUploadRepository fur = new FileUploadRepository();
                      
                        fur.UpdateCOISlides(UserID, ProgramID, extension);

                        returnPath = COISlidesPath + "\\" + " COISlides." + extension;
                        UploadedFileName = fileName;
                    }
                }
            }
            catch (Exception e)
            {
                //Response.StatusCode = (int)HttpStatusCode.BadRequest;
                UserHelper.WriteToLog("Error in Uploading COISlide..." + e.Message);
                return Json("Upload failed", JsonRequestBehavior.AllowGet);
            }
          //HttpContext.Response.AppendHeader("Access - Control - Allow - Origin","*");
            var success = new { msg = "File uploaded successfully", returnFileName = returnPath, uploadedFilename = UploadedFileName };
            return Json(new { success = success }, JsonRequestBehavior.AllowGet);
        }

        [AllowCrossSiteJson]
        public FileResult OpenCOIFile(string UserID, string FileType, string FileExt, string ProgramID)

        {
            string FileName = String.Empty;
            string FileToOpen = string.Empty;
            try

            {
                if (FileType == "COIForm")
                {
                    FileName = "COIForm/COIForm." + FileExt;
                    FileToOpen = Server.MapPath(ConfigurationManager.AppSettings["UserFileUploadPath"] + UserID + "/" + ProgramID +"/" + FileName);
                }
                else if (FileType == "COISlides")
                {
                    FileName = "COISlides." + FileExt;
                    FileToOpen = Server.MapPath("/COISlides/" + UserID + "/" + ProgramID + "/" + FileName);
                    //FileToOpen = Server.MapPath("/COISlides/" + UserID + "/" + ProgramID + "/" + FileName);


                }
                Console.WriteLine("file to open:" + FileName);
                Console.WriteLine("file path to open:" + FileToOpen);
                //File Type: Evaluation,Other,SignIn


                if (System.IO.File.Exists(FileToOpen))
                {
                    Console.WriteLine("File Exist");
                    return File(new FileStream(FileToOpen, FileMode.Open), "application/octetstream", FileToOpen);
                }
                else
                {
                    Response.Write("file not found:" + FileToOpen);

                }
                return null;

            }

            catch (Exception ex)
            {
                Console.WriteLine("exception:" + ex.Message);
                throw ex;
            }
        }
    }
}
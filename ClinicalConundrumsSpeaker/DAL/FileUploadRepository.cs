using ClinicalConundrumsSpeaker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClinicalConundrumsSpeaker.DAL
{
    public class FileUploadRepository:BaseRepository
    {
        public bool UpdateCOISlides(int UserID, int ProgramID, string COISlidesExt)
        {
            try
            {
                

                var val = Entities.COISlidesUploads.Where(x => x.UserID == UserID && x.ProgramID==ProgramID).SingleOrDefault();
                if (val != null)
                {

                    Entities.COISlidesUploads.Remove(val);
                    Entities.SaveChanges();
                    
                }

                ClinicalConundrum2019.Data.COISlidesUpload objcoislidesupload = new ClinicalConundrum2019.Data.COISlidesUpload();

                objcoislidesupload.UserID = UserID;
                objcoislidesupload.ProgramID = ProgramID;
                objcoislidesupload.COISlides = true;
                objcoislidesupload.COISlidesExt = COISlidesExt;



                objcoislidesupload.LastUpdated = DateTime.Now;
                Entities.COISlidesUploads.Add(objcoislidesupload);
                Entities.SaveChanges();
               
                  
                    return true;



                
            }
            catch (Exception e)
            {
                UserHelper.WriteToLog("Error UpdateCOISlides..." + e.Message);
                String innerMessage = (e.InnerException != null)? e.InnerException.Message: "";
                UserHelper.WriteToLog("Error UpdateCOISlides... Inner Exception" + e.InnerException);
                return false;
            }

        }
        public bool UpdateCOIForm(int UserID, string COIFormExt)
        {
            try
            {
                var val = Entities.UserRegistrations.Where(x => x.UserID == UserID).SingleOrDefault();
                if (val != null)
                {

                    val.COIForm = true;
                    val.COIFormExt = COIFormExt;
                    Entities.SaveChanges();
                    return true;
                }
                else
                {
                    ClinicalConundrum2019.Data.UserRegistration objUserRegistration = new ClinicalConundrum2019.Data.UserRegistration();
                    objUserRegistration.UserID = UserID;
                    objUserRegistration.COIForm = true;
                    objUserRegistration.COIFormExt = COIFormExt;
                    Entities.UserRegistrations.Add(objUserRegistration);
                    Entities.SaveChanges();
                    return true;



                }
            }
            catch (Exception e)
            {

                return false;
            }

        }

        public string GetProgramNames(string programId)
        {
            int ProgramID = Convert.ToInt32(programId);

            var ProgramName = Entities.Programs.Where(x => x.ProgramID == ProgramID).Select(x => x.ProgramName).SingleOrDefault();

            return ProgramName;

        }
    }
}
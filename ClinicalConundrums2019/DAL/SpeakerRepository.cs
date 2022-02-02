using ClinicalConundrums2019.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using ClinicalConundrum2019.Data;
using ClinicalConundrums2019.Util;
using System.Data.Entity.SqlServer;

namespace ClinicalConundrums2019.DAL
{
    public class SpeakerRepository : BaseRepository
    {
        public List<SpeakerModel> GetAllApprovedSpeakers(int ProgramID)//it actually retrieve both speakers and moderators
        {
            //get all speakers who are not optout
            //programID is only used in the COISlide, so we know when copy of COISlide to display
            List<SpeakerModel> speakerList = null;
            speakerList = (from ui in Entities.UserInfoes
                           where (ui.UserType == 2 || ui.UserType == 3) && ui.Status != 5 //speakers: usertype=2  moderator: usertype=3 and not opt-out

                           orderby ui.Status  //the un approved should be top of the grid
                           select new SpeakerModel()
                           {
                               user_id = ui.id,
                               UserID = ui.UserID ?? 0,
                               //UserType = ui.UserTypeLookUp.Description,
                               FirstName = ui.FirstName,
                               LastName = ui.LastName,
                               EmailAddress = ui.EmailAddress,
                               ClinicName = ui.ClinicName,
                               Address = ui.Address,
                               //Address2 = ui.Address2,
                               City = ui.City,
                               Province = ui.Province,
                               PostalCode = ui.PostalCode,
                               Phone = ui.Phone,
                               Fax = ui.Fax,
                               Specialty = ui.Specialty,
                               //HonariumRange = ui.SpeakerHonariumRange,
                               CompletedPrograms = "",
                               UpcomingPrograms = "",
                               SubmittedDate = (ui.SubmittedDate == null) ? null : SqlFunctions.DateName("year", ui.SubmittedDate) + "/" + SqlFunctions.DatePart("m", ui.SubmittedDate) + "/" + SqlFunctions.DateName("day", ui.SubmittedDate),
                               Comment = ui.Comment,
                               //StatusString = ui.UserStatus.UserStatusDescription,
                               //Status = ui.Status ?? 0


                           }).ToList();
            string strCompletedProgram = "";
            string strUpcomingProgram = "";
            string COISlidesExt = "";
            //bool HasCOISlides;
            foreach (var speakerModel in speakerList)
            {
                ProgramRepository pr = new ProgramRepository();
                SpeakerRepository sr = new SpeakerRepository();
                List<CompletedProgram> liCompletedProgram;
                List<CompletedProgram> liUpcomingProgram;
                //get the first 3 completed program
                liUpcomingProgram = pr.GetUpcomingProgramsByuserid(3, speakerModel.user_id);
                liCompletedProgram = pr.GetCompletedProgramsByuserid(3, speakerModel.user_id);
                if (speakerModel.UserID != 0)
                {
                    speakerModel.COISlides = sr.GetCOISlidesByUserID(speakerModel.UserID, ProgramID);
                    speakerModel.COISlidesExt = COISlidesExt = sr.GetCOISlidesExtByUserID(speakerModel.UserID, ProgramID);
                    /*if (speakerModel.COISlides)
                        speakerModel.ProgramID = ProgramID;
                    else
                        speakerModel.ProgramID = 0;*/
                }

                if (liCompletedProgram != null && liCompletedProgram.Count > 0)
                {
                    foreach (var completedProgram in liCompletedProgram)
                    {
                        strCompletedProgram = String.Empty;
                        strCompletedProgram = completedProgram.ProgramName + ", " + completedProgram.LocationName + ", " + completedProgram.ConfirmedSessionDate;
                        strCompletedProgram = strCompletedProgram + "<br/>";

                    }
                    string CompletedProgramCount = (liCompletedProgram.Count > 0) ? liCompletedProgram.Count.ToString() : "";
                    string tooltipCompletedProgram = @"<button type='button' class=""btn btn-xs"" data-toggle=""tooltip"" data-placement='left' data-html='true' title='<p><strong>" + strCompletedProgram + "</strong><p>' id='TTCompletedProgram' style=\"background:none; padding:0px\">" + CompletedProgramCount + "</button>";

                    speakerModel.CompletedPrograms = tooltipCompletedProgram;
                }
                else
                    speakerModel.CompletedPrograms = String.Empty;

                //set upcoming program
                if (liUpcomingProgram != null && liUpcomingProgram.Count > 0)
                {
                    foreach (var completedProgram in liUpcomingProgram)
                    {
                        strUpcomingProgram = String.Empty;
                        strUpcomingProgram = completedProgram.ProgramName + ", " + completedProgram.LocationName + ", " + completedProgram.ConfirmedSessionDate;
                        strUpcomingProgram = strUpcomingProgram + "<br/>";

                    }
                    string UpcomingProgramCount = (liUpcomingProgram.Count > 0) ? liUpcomingProgram.Count.ToString() : "";
                    string tooltipUpcomingProgram = @"<button type='button' class=""btn btn-xs"" data-toggle=""tooltip"" data-placement='left' data-html='true' title='<p><strong>" + strUpcomingProgram + "</strong><p>' id='TTUpcomingProgram' style=\"background:none; padding:0px\">" + UpcomingProgramCount + "</button>";

                    speakerModel.UpcomingPrograms = tooltipUpcomingProgram;
                }
                else
                    speakerModel.UpcomingPrograms = String.Empty;

            }

            return speakerList;


        }

        public List<PresenterPayment> GetPresenterPayments(int userid)
        {
            List<PresenterPayment> PresenterPaymentList = null;
            //union by userid using programrequest table the presenter could be a speaker or a moderator
            var SpeakerPayment = from pr in Entities.ProgramRequests

                                 where pr.ProgramSpeakerID == userid
                                 select new
                                 {
                                     ProgramDate = pr.ConfirmedSessionDate,
                                     PresenterPaymentAmount = pr.AdminSpeakerHonorium,
                                     PaymentSentDate = pr.AdminSpeakerPaymentSentDate
                                 };

            var Speaker2Payment = from pr in Entities.ProgramRequests

                                 where pr.ProgramSpeaker2ID == userid
                                 select new
                                 {
                                     ProgramDate = pr.ConfirmedSessionDate,
                                     PresenterPaymentAmount = pr.AdminSpeaker2Honorium,
                                     PaymentSentDate = pr.AdminSpeaker2PaymentSentDate
                                 };
            var ModeratorPayment = from pr in Entities.ProgramRequests

                                   where pr.ProgramModeratorID == userid
                                   select new
                                   {
                                       ProgramDate = pr.ConfirmedSessionDate,
                                       PresenterPaymentAmount = pr.AdminModeratorHonorium,
                                       PaymentSentDate = pr.AdminModeratorPaymentSentDate
                                   };


            var CombinedPayments = SpeakerPayment.Union(ModeratorPayment).Union(Speaker2Payment).Select(
                x => new PresenterPayment()
                {

                    ProgramDate = (x.ProgramDate == null) ? null : SqlFunctions.DateName("year", x.ProgramDate) + "/" + SqlFunctions.DatePart("m", x.ProgramDate) + "/" + SqlFunctions.DateName("day", x.ProgramDate),
                    PaymentAmount = x.PresenterPaymentAmount.ToString(),
                    PaymentSentDate = (x.PaymentSentDate == null) ? null : SqlFunctions.DateName("year", x.PaymentSentDate) + "/" + SqlFunctions.DatePart("m", x.PaymentSentDate) + "/" + SqlFunctions.DateName("day", x.PaymentSentDate),
                }).Where(x => x.PaymentSentDate != null).OrderByDescending(x => x.PaymentSentDate);

            PresenterPaymentList = CombinedPayments.ToList();





            return PresenterPaymentList;


        }
        public List<COISlide> GetCOISlides(int UserID)
        {
            List<COISlide> liCOISlides = null;
            //union by userid using programrequest table the presenter could be a speaker or a moderator
            var COISlides = (from cu in Entities.COISlidesUploads

                             where cu.UserID == UserID
                             select new COISlide()
                             {
                                 UserID = cu.UserID,
                                 ProgramID = cu.ProgramID,
                                 FileExtension = cu.COISlidesExt
                                 //   LastUpdated = cu.LastUpdated.HasValue ? cu.LastUpdated.Value.ToString("MMMM dd, yyyy") : ""
                             }).ToList();







            return COISlides;


        }
        public SpeakerModel GetSpeakerByuserid(int user_id)
        {
            SpeakerModel sm = null;

            sm = Entities.UserInfoes.Where(x => x.id == user_id).Select(ui =>
                     new SpeakerModel
                     {
                         user_id = user_id,
                         FirstName = ui.FirstName,
                         LastName = ui.LastName,
                         EmailAddress = ui.EmailAddress,
                         ClinicName = ui.ClinicName,
                         Address = ui.Address,
                         //Address2 = ui.Address2,
                         City = ui.City,
                         Province = ui.Province,
                         PostalCode = ui.PostalCode,
                         Phone = ui.Phone,
                         Fax = ui.Fax,
                         Specialty = ui.Specialty,
                         AssignedRole = ui.AssignedRole ?? 0,
                         //HonariumRange = ui.SpeakerHonariumRange,
                         SpeakerHonoraria = ui.SpeakerHonariumRange,
                         ModeratorHonoraria = ui.ModeratorHonariumRange,
                         //TherapeuticArea = ui.TherapeuticArea.TherapeuticName,
                         Comment = ui.Comment,

                     }).SingleOrDefault();

            return sm;

        }
        public bool SaveNewSpeaker(SpeakerModel sm)
        {
            try
            {
                bool IsAnyFound = false;
                int UserID = UserHelper.GetLoggedInUser().UserID;
                //int SponsorID = UserHelper.GetLoggedInUser().SponsorID;
                IsAnyFound = Entities.UserInfoes.Any(x => (x.FirstName == sm.FirstName && x.LastName == sm.LastName && x.ClinicName.ToUpper() == sm.ClinicName.ToUpper()) || x.EmailAddress.ToUpper() == sm.EmailAddress.ToUpper());


                if (!IsAnyFound)
                {



                    UserInfo val = new UserInfo();
                    val.AssignedRole = sm.AssignedRole;
                    val.FirstName = sm.FirstName;
                    val.LastName = sm.LastName;
                    val.Specialty = sm.Specialty;
                    val.UserIDRequestedBy = UserID;  //need to who made the request so when chrc admin approve the speaker addition request the sales rep is notified
                    val.UserType = 2;  //default to speaker, the dropdown in the program request form decide who is speaker vs moderator, everyone is available in both dropdowns
                    val.ClinicName = sm.ClinicName;
                    val.Address = sm.Address;
                    val.City = sm.City;
                    val.Province = sm.Province;
                    val.PostalCode = sm.PostalCode;

                    //val.TherapeuticID = Convert.ToInt32(sm.TherapeuticID);

                    val.Phone = sm.Phone;
                    val.Fax = sm.Fax;
                    val.SponsorID = 0;//zero for speakers/moderators
                    val.EmailAddress = sm.EmailAddress;

                    val.Comment = sm.Comment;

                    val.Status = 1; //pending approval
                    val.SubmittedDate = DateTime.Now;
                    val.LastUpdated = DateTime.Now;
                    Entities.UserInfoes.Add(val);
                    Entities.SaveChanges();
                    return true;

                }
                else
                    return false;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool GetCOISlidesByUserID(int UserID, int ProgramID)
        {

            bool COISlides = false;
            try
            {


                var val = Entities.COISlidesUploads.Where(u => u.UserID == UserID && u.ProgramID == ProgramID).FirstOrDefault();
                if (val != null)
                {
                    COISlides = val.COISlides ?? false;

                }


                return COISlides;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public string GetCOISlidesExtByUserID(int UserID, int ProgramID)
        {

            string COISlidesExt = String.Empty;
            try
            {


                var val = Entities.COISlidesUploads.Where(u => u.UserID == UserID && u.ProgramID == ProgramID).FirstOrDefault();
                if (val != null)
                {
                    COISlidesExt = val.COISlidesExt;

                }


                return COISlidesExt;
            }
            catch (Exception e)
            {
                return String.Empty;
            }
        }
    }
}
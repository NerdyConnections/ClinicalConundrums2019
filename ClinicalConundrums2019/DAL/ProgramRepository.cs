using ClinicalConundrum2019.Data;
using ClinicalConundrums2019.Models;
using ClinicalConundrums2019.Util;
using ClinicalConundrums2019.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClinicalConundrums2019.DAL
{

    public class ProgramRepository : BaseRepository
    {
        public string GetTotalSessionCredits(int ProgramRequestID)
        {

            List<ProgramRequestSessionCredits> prsc;
            decimal TotalSessionCredits = (decimal)0.0;
            prsc = (from pr in Entities.ProgramRequestSessionCredits
                    where pr.ProgramRequestID == ProgramRequestID
                    // display all requests for now where pr.RequestStatus == 2 || pr.RequestStatus == 3  //show only submitted requests (either approved 3 or waiting to be approved  2 in the admin tool)
                    //orderby pr.Approved, pr.ContactLastName
                    select new ProgramRequestSessionCredits()
                    {
                        id = pr.SessionCreditID,
                        ProgramRequestID = pr.ProgramRequestID,
                        SessionCreditValue = ((decimal)pr.SessionCreditLookUp.Value)



                    }).ToList();
            foreach (ProgramRequestSessionCredits item in prsc)
            {
                TotalSessionCredits = TotalSessionCredits + item.SessionCreditValue;


            }
            return string.Format("{0:0.0}", TotalSessionCredits);

        }
        public ProgramRequestInfoModel GetProgramRequestInfo(int ProgramRequestID)
        {
            ProgramRequestInfoModel prim = new ProgramRequestInfoModel();

            var val = Entities.ProgramRequests.Where(x => x.ProgramRequestID == ProgramRequestID).SingleOrDefault();

            if (val != null)
            {
                prim.ConfirmedSessionDate = val.ConfirmedSessionDate.HasValue ? val.ConfirmedSessionDate.Value.ToString("MMMM-dd-yyyy") : "";
                prim.Speaker1FirstName = val.SpeakerInfo.FirstName;
                prim.Speaker1LastName = val.SpeakerInfo.LastName;
                if (val.Speaker2Info != null)
                {
                    prim.Speaker2FirstName = val.Speaker2Info.FirstName;
                    prim.Speaker2LastName = val.Speaker2Info.LastName;
                }
                if (val.ModeratorInfo != null)
                {
                    prim.ModeratorFirstName = val.ModeratorInfo.FirstName;
                    prim.ModeratorLastName = val.ModeratorInfo.LastName;
                }
                prim.location = val.LocationName + " " + val.LocationAddress + " " + val.LocationCity + " " + val.LocationProvince;
                prim.programstarttime = val.ProgramStartTime.Value.ToString("HH:mm");
                prim.programendtime = val.ProgramEndTime.Value.ToString("HH:mm");
                prim.VenueName = val.LocationName;
                prim.VenueCity = val.LocationCity;
                prim.VenueProvince = UserHelper.GetProvinceFullName(val.LocationProvince);
                prim.VenueAddress = val.LocationAddress;
                prim.Province = getProvinceFullName(val.LocationProvince) + " Chapter"; //always add the word Chapter at the end
                string MealStartTime = val.MealStartTime.HasValue ? val.MealStartTime.Value.ToString("HH:mm") : val.MealOption;
                prim.RegistrationComments1 = val.RegistrationArrivalTime.HasValue ? val.RegistrationArrivalTime.Value.ToString("HH:mm") : " ";
                prim.RegistrationComments2 = " Meal Start Time:" + MealStartTime;
                prim.SessionID = val.AdminSessionID;
                prim.TotalSessionCredits = GetTotalSessionCredits(ProgramRequestID);
                prim.RSVP = val.ContactFirstName + " " + val.ContactLastName + " " + val.ContactEmail + " " + val.ContactPhone + " " + val.AdditionalContactName + " " + val.AdditionalContactEmail + " " + val.AdditionalContactPhone;
                //get session credits
                ProgramRepository progRepo = new ProgramRepository();
                List<ProgramRequestSessionCredits> SessionList = new List<ProgramRequestSessionCredits>();
                SessionList = progRepo.GetSessionCredits(ProgramRequestID);
                foreach (var item in SessionList)
                {
                    if (item.id == 1)
                    {

                        prim.SessionCredit1 = true;
                    }

                    if (item.id == 2)
                    {

                        prim.SessionCredit2 = true;
                    }

                    if (item.id == 3)
                    {

                        prim.SessionCredit3 = true;
                    }
                    if (item.id == 4)
                    {

                        prim.SessionCredit4 = true;
                    }
                    if (item.id == 5)
                    {

                        prim.SessionCredit5 = true;
                    }
                    if (item.id == 6)
                    {

                        prim.SessionCredit6 = true;
                    }
                    if (item.id == 7)
                    {

                        prim.SessionCredit7 = true;
                    }
                    if (item.id == 8)
                    {

                        prim.SessionCredit8 = true;
                    }
                    if (item.id == 9)
                    {

                        prim.SessionCredit9 = true;
                    }
                    if (item.id == 10)
                    {

                        prim.SessionCredit10 = true;
                    }
                    if (item.id == 11)
                    {

                        prim.SessionCredit11 = true;
                    }
                    if (item.id == 12)
                    {

                        prim.SessionCredit12 = true;
                    }
                }

            }
            else
            {

                return null;
            }
            return prim;

        }
        private string getProvinceFullName(string provinceCode)
        {
            if (provinceCode == "AB")
                return "Alberta";
            else if (provinceCode == "BC")
                return "British Columbia";
            else if (provinceCode == "MB")
                return "Manitoba";
            else if (provinceCode == "NB")
                return "New Brunswick";
            else if (provinceCode == "NL")
                return "Newfoundland";
            else if (provinceCode == "NS")
                return "Nova Scotia";
            else if (provinceCode == "ON")
                return "Ontario";
            else if (provinceCode == "PEI")
                return "Prince Edward Island";
            else if (provinceCode == "QC")
                return "Quebec";
            else if (provinceCode == "SK")
                return "Saskatchewan";
            else
                return "Ontario";

        }
        public List<Models.Program> GetPrograms()
        {
            Models.Program objProgram = new Models.Program();
            ProgramRepository pr = new ProgramRepository();

            List<Models.Program> liProgram = null;
            liProgram = Entities.Programs.
            Select(u => new Models.Program
            {
                ProgramID = u.ProgramID,
                ProgramID_CHRC = u.ProgramID_CHRC,
                ProgramName = u.ProgramName,
                DevelopedBy = u.DevelopedBy,
                CertifiedBy = u.CertifiedBy,
                ExpirationDate = u.ExpirationDate,
                TargetAudience = u.TargetAudience,
                CreditHours = u.CreditHours,
                CustomProgram=u.CustomProgram ?? false,
                ProgramCompleted = u.ProgramCompleted

            }).ToList();

            foreach (Models.Program pg in liProgram)
            {

                pg.EventsCompleted = pr.GetEventsCompleted(pg.ProgramID);
            }

            return liProgram;



        }
        public int GetEventsCompleted(int programID)
        {
            int val = 0;
            //int UserID = UserHelper.GetLoggedInUser().UserID;


            //head office and sale director see every sessions for a program for a sponsor no need to query for territoryID
            // if (UserRole == Util.Constants.HeadOffice || UserRole == Util.Constants.SalesDirector)
            //{

            val = Entities.ProgramRequests.Where(x => x.ProgramID == programID && x.RequestStatus == 4).Count();

            //}//see all users under the manager's territoryID
            //else 
            // {  //need to retrieve all users with the same territoryID and get all dashboard items with the same territoryID

            //the manager's territoryID ie. 41
            //var TerritoryID = (from ut in Entities.UserInfoes where ut.UserID == UserID select ut.TerritoryID).FirstOrDefault();
            //all userids under territoryID 41
            // var TerritorialUserIDs = (from ut in Entities.UserInfoes where ut.TerritoryID == TerritoryID select ut.UserID).ToList();
            //dashboarditems of all userids belonging to territory 41

            //val = Entities.ProgramRequests.Where(x => x.ProgramID == programID && x.RequestStatus == 4 &&  TerritorialUserIDs.Contains(x.UserID)).Count();


            //}



            return val;

        }

        public EventRequestModel InitialEventRequestForm(int ProgramID)
        {
            var CurrentUser = UserHelper.GetLoggedInUser();

            EventRequestModel pr = new EventRequestModel();
            //they are all in the hidden field, for saving later.
            pr.ContactName = CurrentUser.FirstName + " " + CurrentUser.LastName;
            pr.ContactFirstName = CurrentUser.FirstName;
            pr.ContactLastName = CurrentUser.LastName;
            pr.ContactEmail = CurrentUser.Username;
            pr.ContactPhone = CurrentUser.Phone;
            pr.SpeakerChosenProgramDate = false;
            pr.ModeratorChosenProgramDate = false;
            pr.ContactPhone = CurrentUser.Phone;
            pr.ProgramRequestID = GetProgramRequestID();
            pr.ProgramID = ProgramID;


            pr.Speakers = Entities.UserInfoes.Where(x => (((x.UserType == 2) || (x.UserType == 3)) && ((x.Status == 2) || (x.Status == 3) || (x.Status == 4)))).OrderBy(x=>x.FirstName).Select(c => new SelectListItem
            {
                Value = c.id.ToString(),
                Text = c.FirstName + " " + c.LastName

            });
            //get approved moderators only
            pr.Moderators = Entities.UserInfoes.Where(x => (((x.UserType == 2) || (x.UserType == 3)) && ((x.Status == 2) || (x.Status == 3) || (x.Status == 4)))).OrderBy(x => x.FirstName).Select(c => new SelectListItem
            {
                Value = c.id.ToString(),
                Text = c.FirstName + " " + c.LastName

            });







            //get approved speakers

            return pr;
        }

        public int GetProgramRequestID()
        {
            int ProgramRequestID;
            try
            {


                ProgramRequest pr = new ProgramRequest();

                Entities.ProgramRequests.Add(pr);
                Entities.SaveChanges();

                ProgramRequestID = pr.ProgramRequestID;
                return ProgramRequestID;
            }
            catch (Exception e)
            {
                ProgramRequestID = 0;
                return ProgramRequestID;
            }


        }
        public string GetProgramName(int programID)
        {
            string retVal = "";
            var val = Entities.Programs.Where(x => x.ProgramID == programID).SingleOrDefault();
            if (val != null)
            {
                retVal = val.ProgramName;

            }
            return retVal;

        }
        public EventRequestModel PopulateSpeakerModeratorDropdowns()
        {

            EventRequestModel erm = new EventRequestModel();

            //speaker who have approved, registered not complete (Speaker chose email/password but not yet upload coi or payeeform) or registercomplete (User chose email/password and upload coi and payeeform)

            erm.Speakers = Entities.UserInfoes.Where(x => (((x.UserType == 2) || (x.UserType == 3)) && ((x.Status == 2) || (x.Status == 3) || (x.Status == 4)))).Select(c => new SelectListItem
            {
                Value = c.id.ToString(),
                Text = c.FirstName + " " + c.LastName

            });
            //get approved moderators only
            erm.Moderators = Entities.UserInfoes.Where(x => (((x.UserType == 2) || (x.UserType == 3)) && ((x.Status == 2) || (x.Status == 3) || (x.Status == 4)))).Select(c => new SelectListItem
            {
                Value = c.id.ToString(),
                Text = c.FirstName + " " + c.LastName

            });

            return erm;

        }

        public string GetProgramRequestName(int id)
        {
            string retVal = "";
            var val = Entities.Programs.Where(x => x.ProgramID == id).SingleOrDefault();
            if (val != null)
            {
                retVal = val.ProgramName;

            }
            return retVal;

        }

        public EventRequestModel GetProgramRequestForEmail(int ProgramRequestID)
        {

            EventRequestModel pr;
            pr = Entities.ProgramRequests.Where(x => x.ProgramRequestID == ProgramRequestID).Select(ui =>
                     new EventRequestModel
                     {
                         ProgramRequestID = ProgramRequestID,
                         ContactFirstName = ui.ContactFirstName,
                         ContactLastName = ui.ContactLastName,
                         ContactEmail = ui.ContactEmail,
                         ContactPhone = ui.ContactPhone,
                         LocationName = ui.LocationName,
                         ProgramSpeakerID = ui.ProgramSpeakerID ?? 0,
                         ProgramModeratorID = ui.ProgramModeratorID ?? 0


                     }).SingleOrDefault();

            return pr;
        }

        //return a session credits in a string
        public string SessionCredit(EventRequestModel erm)
        {
            string val = "";
            int count = 0;
            if (erm.ProgramID == 4)
            {
                if (erm.SessionCredit1)
                {
                    val += "Clinical Conundrums V: 2020 Update - 0.5 Mainpro+ Credits (30 minutes) ";
                    count++;
                }

                if (erm.SessionCredit2)
                {
                    if (count > 0)
                    {
                        val += ", " + "Clinical Conundrums V: 2020 Update & Interactive FAQ - 1.0 Mainpro + Credits(1 hour) ";

                    }
                    else
                    {
                        val += "Clinical Conundrums V: 2020 Update & Interactive FAQ - 1.0 Mainpro + Credits(1 hour)";


                    }

                    count++;

                }

                if (erm.SessionCredit3)
                {
                    if (count > 0)
                    {
                        val += ", " + "Case 1: 0.5 Mainpro + Credits(30 minutes)";

                    }
                    else
                    {
                        val += " Case 1: 0.5 Mainpro + Credits(30 minutes) ";

                    }

                    count++;
                }

                if (erm.SessionCredit4)
                {
                    if (count > 0)
                    {
                        val += ", " + "Case 2: 0.5 Mainpro + Credits(45 minutes) ";

                    }
                    else
                    {
                        val += " Case 2: 0.5 Mainpro + Credits(45 minutes) ";

                    }
                    count++;
                }

                if (erm.SessionCredit5)
                {
                    if (count > 0)
                    {
                        val += ", " + " Case 3: 0.5 Mainpro + Credits(30 minutes) ";

                    }
                    else
                    {
                        val += " Case 3: 0.5 Mainpro + Credits(30 minutes) ";

                    }
                    count++;
                }
                if (erm.SessionCredit6)
                {
                    if (count > 0)
                    {
                        val += ", " + " Case 4: 0.5 Mainpro + Credits(30 minutes) ";

                    }
                    else
                    {
                        val += " Case 4: 0.5 Mainpro + Credits(30 minutes) ";

                    }
                    count++;
                }
                if (erm.SessionCredit7)
                {
                    if (count > 0)
                    {
                        val += ", " + " Case 5: 0.5 Mainpro+ Credits (30 minutes)  ";

                    }
                    else
                    {
                        val += " Case 5: 0.5 Mainpro+ Credits (30 minutes)  ";

                    }
                    count++;
                }
                if (erm.SessionCredit8)
                {
                    if (count > 0)
                    {
                        val += ", " + " Case 6: 1.0 Mainpro+ Credits (1 hour) ";

                    }
                    else
                    {
                        val += " Case 6: 1 Mainpro+ Credits (1 hour) ";

                    }
                    count++;
                }
                if (erm.SessionCredit9)
                {
                    if (count > 0)
                    {
                        val += ", " + " Case 7: Clinical Encounters 0.5 Mainpro+ Credits (30 minutes) ";

                    }
                    else
                    {
                        val += " Case 7: Clinical Encounters 0.5 Mainpro+ Credits (30 minutes) ";

                    }
                    count++;
                }
                if (erm.SessionCredit10)
                {
                    if (count > 0)
                    {
                        val += ", " + " Case 8: Clinical Vignettes 0.5 Mainpro + Credits(30 minutes) ";

                    }
                    else
                    {
                        val += " Case 8: Clinical Vignettes 0.5 Mainpro + Credits(30 minutes) ";

                    }
                    count++;
                }
                if (erm.SessionCredit11)
                {
                    if (count > 0)
                    {
                        val += ", " + " Case 9: 0.5 Mainpro + Credits(45 minutes) ";

                    }
                    else
                    {
                        val += " Case 9: 0.5 Mainpro + Credits(45 minutes) ";

                    }
                    count++;
                }
                if (erm.SessionCredit12)
                {
                    if (count > 0)
                    {
                        val += ", " + " Case 10: 1.0 Mainpro + Credits(1 hour)  ";

                    }
                    else
                    {
                        val += " Case 10: 1.0 Mainpro + Credits(1 hour) ";

                    }
                    count++;
                }
            }//end of program 1



            return val;
        }

        public void UpdateSpeakerConfirmDate(int ProgramRequestId, string date)
        {
            var val = Entities.ProgramRequests.Where(x => x.ProgramRequestID == ProgramRequestId).SingleOrDefault();
            if (val != null)
            {

                val.SpeakerConfirmedProgramDate = DateTime.ParseExact(date, "yyyy/MM/dd", null);
                val.SpeakerConfirmationDate = DateTime.Now;
                val.SpeakerChosenProgramDate = true;
                val.SpeakerStatus = "Accepted";
                val.SpeakerProgramDateNA = false;
                Entities.SaveChanges();

            }

        }

        public void UpdateModeratorConfirmDate(int ProgramRequestId, string date)
        {
            var val = Entities.ProgramRequests.Where(x => x.ProgramRequestID == ProgramRequestId).SingleOrDefault();
            if (val != null)
            {
                val.ModeratorConfirmedProgramDate = DateTime.ParseExact(date, "yyyy/MM/dd", null);
                val.ModeratorConfirmationDate = DateTime.Now;
                val.ModeratorChosenProgramDate = true;
                val.ModeratorProgramDateNA = false;
                val.ModeratorStatus = "Accepted";
                val.SpeakerStatus = "Accepted";
                //If speaker2 accepted the invite or if speaker2 doesn't exists set the confirmedSessionDate
                if (val.Speaker2Status == "Accepted" || val.ProgramSpeaker2ID == null)
                    val.ConfirmedSessionDate = DateTime.ParseExact(date, "yyyy/MM/dd", null);
              //  val.ConfirmedSessionDate = DateTime.ParseExact(date, "yyyy/MM/dd", null);
                Entities.SaveChanges();

            }

        }

        public void UpdateSpeaker2ConfirmDate(int ProgramRequestId, string date)
        {
            var val = Entities.ProgramRequests.Where(x => x.ProgramRequestID == ProgramRequestId).SingleOrDefault();
            if (val != null)
            {
                val.Speaker2ConfirmedProgramDate = DateTime.ParseExact(date, "yyyy/MM/dd", null);
                val.Speaker2ConfirmationDate = DateTime.Now;
                val.Speaker2ChosenProgramDate = true;
                val.Speaker2ProgramDateNA = false;
                val.Speaker2Status = "Accepted";
                val.SpeakerStatus = "Accepted";
                //set confirmed session date if moderator accepted the program date or moderator is not present in this program event
                if (val.ModeratorStatus=="Accepted" || val.ProgramModeratorID == null)
                    val.ConfirmedSessionDate = DateTime.ParseExact(date, "yyyy/MM/dd", null);
                Entities.SaveChanges();

            }

        }

        public void UpdateSpeakerToNotAvailable(int ProgramRequestId)
        {
            var val = Entities.ProgramRequests.Where(x => x.ProgramRequestID == ProgramRequestId).SingleOrDefault();
            if (val != null)
            {

                val.SpeakerProgramDateNA = true;
                val.SpeakerChosenProgramDate = true;
                val.SpeakerStatus = "Not Available";
                val.ReadOnly = false;
                val.RequestStatus = 7;
                Entities.SaveChanges();

            }

        }
        
        public void UpdateModeratorToNotAvailable(int ProgramRequestId)
        {
            var val = Entities.ProgramRequests.Where(x => x.ProgramRequestID == ProgramRequestId).SingleOrDefault();
            if (val != null)
            {

                val.ModeratorProgramDateNA = true;
                val.ModeratorChosenProgramDate = true;
                val.ModeratorStatus = "Not Available";
                val.ReadOnly = false;
                val.RequestStatus = 7;
                Entities.SaveChanges();

            }

        }
        public void UpdateSpeaker2ToNotAvailable(int ProgramRequestId)
        {
            var val = Entities.ProgramRequests.Where(x => x.ProgramRequestID == ProgramRequestId).SingleOrDefault();
            if (val != null)
            {

                val.Speaker2ProgramDateNA = true;
                val.Speaker2ChosenProgramDate = true;
                val.Speaker2Status = "Not Available";
                val.ReadOnly = false;
                val.RequestStatus = 7;
                Entities.SaveChanges();

            }

        }

        public bool CheckIfSpeakerConfirmedEmail(int ProgramRequestId)
        {
            bool retVal = false;

            var val = Entities.ProgramRequests.Where(x => x.ProgramRequestID == ProgramRequestId).SingleOrDefault();
            if (val != null)
            {

                if ((val.SpeakerChosenProgramDate == true))
                {
                    retVal = true;
                }

            }
            return retVal;

        }

        public bool CheckIfModeratorConfirmedEmail(int ProgramRequestId)
        {
            bool retVal = false;

            var val = Entities.ProgramRequests.Where(x => x.ProgramRequestID == ProgramRequestId).SingleOrDefault();
            if (val != null)
            {

                if ((val.ModeratorChosenProgramDate == true))
                {
                    retVal = true;
                }

            }
            return retVal;

        }
        public bool CheckIfSpeaker2ConfirmedEmail(int ProgramRequestId)
        {
            bool retVal = false;

            var val = Entities.ProgramRequests.Where(x => x.ProgramRequestID == ProgramRequestId).SingleOrDefault();
            if (val != null)
            {

                if ((val.Speaker2ChosenProgramDate == true))
                {
                    retVal = true;
                }

            }
            return retVal;

        }
        public List<EventRequestModel> GetAllProgramRequests(int? programID)
        {
            List<EventRequestModel> programRequestList = null;

            if (programID == null || programID == 0)
            {
                programRequestList = (from pr in Entities.ProgramRequests

                                          // display all requests for now where pr.RequestStatus == 2 || pr.RequestStatus == 3  //show only submitted requests (either approved 3 or waiting to be approved  2 in the admin tool)
                                      orderby pr.Approved, pr.ContactLastName
                                      select new EventRequestModel()
                                      {
                                          ProgramRequestID = pr.ProgramRequestID,
                                          ProgramID = pr.ProgramID ?? 0,
                                          ContactInformation = pr.ContactFirstName + ", " + pr.ContactLastName + "<br/>" + pr.ContactPhone + "," + pr.ContactEmail + "<br/>======================<br/>" + pr.AdditionalContactName + "<br/>" + pr.AdditionalContactPhone + "," + pr.AdditionalContactEmail,
                                          ContactFirstName = pr.ContactFirstName,
                                          ContactLastName = pr.ContactLastName,
                                          ContactPhone = pr.ContactPhone,
                                          ContactEmail = pr.ContactEmail,
                                          SpeakerStatus = Entities.UserInfoes.Where(x => x.id == pr.ProgramSpeakerID).Select(x => x.FirstName + ", " + x.LastName + ", " + x.Phone + ", " + x.EmailAddress + "<br />" +
                                          (x.UserStatus.UserStatusID.ToString() == "0" ? "Not a speaker or moderator" : "") + (x.UserStatus.UserStatusID.ToString() == "1" ? "Pending Approval" : "") + (x.UserStatus.UserStatusID.ToString() == "2" ? "Approved" : "") + (x.UserStatus.UserStatusID.ToString() == "3" ? "RegisteredNotComplete" : "") + (x.UserStatus.UserStatusID.ToString() == "4" ? "RegisteredCompleted" : "") + (x.UserStatus.UserStatusID.ToString() == "5" ? "Opt-Out" : "") +
                                            "<br />" + pr.SpeakerStatus).FirstOrDefault(),
                                          Speaker2Status = Entities.UserInfoes.Where(x => x.id == pr.ProgramSpeaker2ID).Select(x => x.FirstName + ", " + x.LastName + ", " + x.Phone + ", " + x.EmailAddress + "<br />" +
                                        (x.UserStatus.UserStatusID.ToString() == "0" ? "Not a speaker or moderator" : "") + (x.UserStatus.UserStatusID.ToString() == "1" ? "Pending Approval" : "") + (x.UserStatus.UserStatusID.ToString() == "2" ? "Approved" : "") + (x.UserStatus.UserStatusID.ToString() == "3" ? "RegisteredNotComplete" : "") + (x.UserStatus.UserStatusID.ToString() == "4" ? "RegisteredCompleted" : "") + (x.UserStatus.UserStatusID.ToString() == "5" ? "Opt-Out" : "") +
                                          "<br />" + pr.Speaker2Status).FirstOrDefault(),
                                          ModeratorStatus = pr.ProgramModeratorID == null ? "Not Applicable" : Entities.UserInfoes.Where(x => x.id == pr.ProgramModeratorID).Select(x => x.FirstName + "," + x.LastName + "<br />" + x.Phone + "," + x.EmailAddress + "<br />" +
                                          (x.UserStatus.UserStatusID.ToString() == "0" ? "Not a speaker or moderator" : "") + (x.UserStatus.UserStatusID.ToString() == "1" ? "Pending Approval" : "") + (x.UserStatus.UserStatusID.ToString() == "2" ? "Approved" : "") + (x.UserStatus.UserStatusID.ToString() == "3" ? "RegisteredNotComplete" : "") + (x.UserStatus.UserStatusID.ToString() == "4" ? "RegisteredCompleted" : "") + (x.UserStatus.UserStatusID.ToString() == "5" ? "Opt-Out" : "") + "<br />" + pr.ModeratorStatus).FirstOrDefault(),
                                          ConfirmedSessionDate = (pr.ConfirmedSessionDate == null) ? null : SqlFunctions.DateName("year", pr.ConfirmedSessionDate) + "/" + SqlFunctions.DatePart("m", pr.ConfirmedSessionDate) + "/" + SqlFunctions.DateName("day", pr.ConfirmedSessionDate),

                                          ProgramDate1 = (pr.ProgramDate1 == null) ? null : SqlFunctions.DateName("year", pr.ProgramDate1) + "/" + SqlFunctions.DatePart("m", pr.ProgramDate1) + "/" + SqlFunctions.DateName("day", pr.ProgramDate1),
                                          ProgramDate2 = (pr.ProgramDate2 == null) ? null : SqlFunctions.DateName("year", pr.ProgramDate2) + "/" + SqlFunctions.DatePart("m", pr.ProgramDate2) + "/" + SqlFunctions.DateName("day", pr.ProgramDate2),
                                          ProgramDate3 = (pr.ProgramDate3 == null) ? null : SqlFunctions.DateName("year", pr.ProgramDate3) + "/" + SqlFunctions.DatePart("m", pr.ProgramDate3) + "/" + SqlFunctions.DateName("day", pr.ProgramDate3),




                                          MealStartTime = (pr.MealStartTime == null) ? String.Empty : pr.MealStartTime.ToString(),
                                          ProgramStartTime = (pr.ProgramStartTime == null) ? String.Empty : SqlFunctions.DatePart("Hour", pr.ProgramStartTime) + ":" + SqlFunctions.DatePart("Minute", pr.ProgramStartTime),
                                          ProgramEndTime = (pr.ProgramEndTime == null) ? String.Empty : SqlFunctions.DatePart("Hour", pr.ProgramEndTime) + ":" + SqlFunctions.DatePart("Minute", pr.ProgramEndTime),
                                          //   ProgramCredits = pr.ProgramCredits??0,
                                          SubmittedDate = (pr.SubmittedDate == null) ? null : SqlFunctions.DateName("year", pr.SubmittedDate) + "/" + SqlFunctions.DatePart("m", pr.SubmittedDate) + "/" + SqlFunctions.DateName("day", pr.SubmittedDate),

                                          RequestStatusID = pr.RequestStatus ?? 0,
                                          RequestStatus = pr.RequestStatusLookup.RequestStatusDescription,
                                          Approved = pr.Approved ?? false


                                      }).ToList();
            }
            else
            {
                programRequestList = (from pr in Entities.ProgramRequests
                                      where pr.ProgramID == programID
                                      // display all requests for now where pr.RequestStatus == 2 || pr.RequestStatus == 3  //show only submitted requests (either approved 3 or waiting to be approved  2 in the admin tool)
                                      orderby pr.Approved, pr.ContactLastName
                                      select new EventRequestModel()
                                      {
                                          ProgramRequestID = pr.ProgramRequestID,
                                          ProgramID = pr.ProgramID ?? 0,
                                          ContactInformation = pr.ContactFirstName + ", " + pr.ContactLastName + "<br/>" + pr.ContactPhone + "," + pr.ContactEmail + "<br/>=======================<br/>" + pr.AdditionalContactName + "<br/>" + pr.AdditionalContactPhone + "," + pr.AdditionalContactEmail,
                                          ContactFirstName = pr.ContactFirstName,
                                          ContactLastName = pr.ContactLastName,
                                          ContactPhone = pr.ContactPhone,
                                          ContactEmail = pr.ContactEmail,

                                          SpeakerStatus = Entities.UserInfoes.Where(x => x.id == pr.ProgramSpeakerID).Select(x => x.FirstName + ", " + x.LastName + ", " + x.Phone + ", " + x.EmailAddress + "<br />" +
                                          (x.UserStatus.UserStatusID.ToString() == "0" ? "Not a speaker or moderator" : "") + (x.UserStatus.UserStatusID.ToString() == "1" ? "Pending Approval" : "") + (x.UserStatus.UserStatusID.ToString() == "2" ? "Approved" : "") + (x.UserStatus.UserStatusID.ToString() == "3" ? "RegisteredNotComplete" : "") + (x.UserStatus.UserStatusID.ToString() == "4" ? "RegisteredCompleted" : "") + (x.UserStatus.UserStatusID.ToString() == "5" ? "Opt-Out" : "") +
                                          "<br />" + pr.SpeakerStatus).FirstOrDefault(),

                                          Speaker2Status = Entities.UserInfoes.Where(x => x.id == pr.ProgramSpeaker2ID).Select(x => x.FirstName + ", " + x.LastName + ", " + x.Phone + ", " + x.EmailAddress + "<br />" +
                                       (x.UserStatus.UserStatusID.ToString() == "0" ? "Not a speaker or moderator" : "") + (x.UserStatus.UserStatusID.ToString() == "1" ? "Pending Approval" : "") + (x.UserStatus.UserStatusID.ToString() == "2" ? "Approved" : "") + (x.UserStatus.UserStatusID.ToString() == "3" ? "RegisteredNotComplete" : "") + (x.UserStatus.UserStatusID.ToString() == "4" ? "RegisteredCompleted" : "") + (x.UserStatus.UserStatusID.ToString() == "5" ? "Opt-Out" : "") +
                                         "<br />" + pr.Speaker2Status).FirstOrDefault(),

                                          ModeratorStatus = pr.ProgramModeratorID == null ? "Not Applicable" : Entities.UserInfoes.Where(x => x.id == pr.ProgramModeratorID).Select(x => x.FirstName + "," + x.LastName + "<br />" + x.Phone + "," + x.EmailAddress + "<br />" +
                                          (x.UserStatus.UserStatusID.ToString() == "0" ? "Not a speaker or moderator" : "") + (x.UserStatus.UserStatusID.ToString() == "1" ? "Pending Approval" : "") + (x.UserStatus.UserStatusID.ToString() == "2" ? "Approved" : "") + (x.UserStatus.UserStatusID.ToString() == "3" ? "RegisteredNotComplete" : "") + (x.UserStatus.UserStatusID.ToString() == "4" ? "RegisteredCompleted" : "") + (x.UserStatus.UserStatusID.ToString() == "5" ? "Opt-Out" : "") + "<br />" + pr.ModeratorStatus).FirstOrDefault(),

                                          ConfirmedSessionDate = (pr.ConfirmedSessionDate == null) ? null : SqlFunctions.DateName("year", pr.ConfirmedSessionDate) + "/" + SqlFunctions.DatePart("m", pr.ConfirmedSessionDate) + "/" + SqlFunctions.DateName("day", pr.ConfirmedSessionDate),
                                          ProgramDate1 = (pr.ProgramDate1 == null) ? null : SqlFunctions.DateName("year", pr.ProgramDate1) + "/" + SqlFunctions.DatePart("m", pr.ProgramDate1) + "/" + SqlFunctions.DateName("day", pr.ProgramDate1),
                                          ProgramDate2 = (pr.ProgramDate2 == null) ? null : SqlFunctions.DateName("year", pr.ProgramDate2) + "/" + SqlFunctions.DatePart("m", pr.ProgramDate2) + "/" + SqlFunctions.DateName("day", pr.ProgramDate2),
                                          ProgramDate3 = (pr.ProgramDate3 == null) ? null : SqlFunctions.DateName("year", pr.ProgramDate3) + "/" + SqlFunctions.DatePart("m", pr.ProgramDate3) + "/" + SqlFunctions.DateName("day", pr.ProgramDate3),




                                          MealStartTime = (pr.MealStartTime == null) ? String.Empty : pr.MealStartTime.ToString(),
                                          ProgramStartTime = (pr.ProgramStartTime == null) ? String.Empty : SqlFunctions.DatePart("Hour", pr.ProgramStartTime) + ":" + SqlFunctions.DatePart("Minute", pr.ProgramStartTime),
                                          ProgramEndTime = (pr.ProgramEndTime == null) ? String.Empty : SqlFunctions.DatePart("Hour", pr.ProgramEndTime) + ":" + SqlFunctions.DatePart("Minute", pr.ProgramEndTime),
                                          //   ProgramCredits = pr.ProgramCredits??0,
                                          SubmittedDate = (pr.SubmittedDate == null) ? null : SqlFunctions.DateName("year", pr.SubmittedDate) + "/" + SqlFunctions.DatePart("m", pr.SubmittedDate) + "/" + SqlFunctions.DateName("day", pr.SubmittedDate),

                                          RequestStatusID = pr.RequestStatus ?? 0,
                                          RequestStatus = pr.RequestStatusLookup.RequestStatusDescription,
                                          Approved = pr.Approved ?? false


                                      }).ToList();


            }


            return programRequestList;


        }
        public List<ProgramRequestSessionCredits> GetSessionCredits(int ProgramRequestId)
        {

            //retreieve the SessionCreditID from the ProgramRequestSessionCredits table
            List<ProgramRequestSessionCredits> SessionsList = new List<ProgramRequestSessionCredits>();


            var list = Entities.ProgramRequestSessionCredits.Where(x => x.ProgramRequestID == ProgramRequestId).Select(ui =>

                         new ProgramRequestSessionCredits
                         {
                             id = ui.SessionCreditID,
                             ProgramRequestID = ui.ProgramRequestID




                         }
            ).ToList();



            return list;

        }
        public ProgramRequestModifyVM GetProgramRequestModifybyID(int ProgramRequestId)
        {
            var CurrentUser = UserHelper.GetLoggedInUser();
            ProgramRequestModifyVM pr = new ProgramRequestModifyVM();
            if (CurrentUser != null)
            {



                var objPr = Entities.ProgramRequests.Where(x => x.ProgramRequestID == ProgramRequestId).SingleOrDefault();

                if (objPr != null)
                {
                    pr.ContactName = CurrentUser.FirstName + " " + CurrentUser.LastName;
                    pr.ContactFirstName = CurrentUser.FirstName;
                    pr.ContactLastName = CurrentUser.LastName;
                    pr.ContactEmail = CurrentUser.Username;
                    pr.ContactPhone = CurrentUser.Phone;
                    pr.ProgramRequestID = objPr.ProgramRequestID;
                    pr.ProgramID = objPr.ProgramID ?? 0;


                    pr.ProgramSpeakerID = objPr.ProgramSpeakerID ?? 0;
                    pr.ProgramSpeaker2ID = objPr.ProgramSpeaker2ID ?? 0;
                    pr.ProgramModeratorID = objPr.ProgramModeratorID ?? 0;


                    pr.Comments = objPr.Comments;

                    pr.LocationAddress = objPr.LocationAddress;
                    pr.LocationCity = objPr.LocationCity;
                    pr.LocationName = objPr.LocationName;
                    pr.LocationPhoneNumber = objPr.LocationPhoneNumber;
                    pr.LocationProvince = objPr.LocationProvince;
                    pr.LocationType = objPr.LocationType;
                    pr.LocationWebsite = objPr.LocationWebsite;



                    /* added to support sessioncancellation */
                    pr.ModifyReason = objPr.ModifyReason;
                    pr.ModifyRequested = objPr.ModifyRequested;
                    pr.ConfirmedSessionDate = objPr.ConfirmedSessionDate.HasValue ? objPr.ConfirmedSessionDate.Value.ToString("yyyy-MM-dd") : "";
                    pr.SpeakerName = objPr.ProgramSpeakerID.HasValue ? objPr.SpeakerInfo.FirstName + " " + objPr.SpeakerInfo.LastName : "";
                    pr.Speaker2Name = objPr.ProgramSpeaker2ID.HasValue ? objPr.Speaker2Info.FirstName + " " + objPr.Speaker2Info.LastName : "";
                    pr.ModeratorName = objPr.ProgramModeratorID.HasValue ? objPr.ModeratorInfo.FirstName + " " + objPr.ModeratorInfo.LastName : "";
                    /*end of session cancellation*/



                    pr.RequestStatus = (objPr.RequestStatus.HasValue ? objPr.RequestStatus.Value : 0).ToString();



                }



            }

            return pr;


        }
        public int UpdateUploadFileStatus(int ProgramRequestID, bool FileUploadStatus)
        {
            try
            {
                int FileCount = 0;
                var objProgramRequest = (from v in Entities.ProgramRequests

                                         where v.ProgramRequestID == ProgramRequestID
                                         select v).FirstOrDefault();

                if (objProgramRequest != null)
                {
                    objProgramRequest.SessionAgendaUploaded = FileUploadStatus;
                    Entities.SaveChanges();
                    FileCount = 1;

                }
                return FileCount;
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
                return 0;
            }

        }
        public EventRequestModel GetProgramRequestbyQueryString(int ProgramRequestId)
        {
            var CurrentUser = UserHelper.GetLoggedInUser();
            EventRequestModel pr = new EventRequestModel();
            if (CurrentUser != null)
            {

                List<ProgramRequestSessionCredits> SessionList = new List<ProgramRequestSessionCredits>();

                var objPr = Entities.ProgramRequests.Where(x => x.ProgramRequestID == ProgramRequestId).SingleOrDefault();

                if (objPr != null)
                {
                    pr.ProgramRequestID = objPr.ProgramRequestID;
                    pr.UserID = objPr.UserID ?? 0;
                    pr.ProgramID = objPr.ProgramID ?? 0;
                    pr.ContactName = objPr.ContactFirstName + " " + objPr.ContactLastName;
                    pr.ContactFirstName = objPr.ContactFirstName;
                    pr.ContactLastName = objPr.ContactLastName;
                    pr.ContactPhone = objPr.ContactPhone;
                    pr.ContactEmail = objPr.ContactEmail;


                    pr.SpeakerStatus = objPr.SpeakerStatus;
                    pr.Speaker2Status = objPr.Speaker2Status;
                    pr.ModeratorStatus = objPr.ModeratorStatus;
                    pr.SessionAgendaFileName = objPr.SessionAgendaFileName;
                    pr.SessionAgendaUploaded = objPr.SessionAgendaUploaded;

                    pr.ProgramDate1 = objPr.ProgramDate1.HasValue ? objPr.ProgramDate1.Value.ToString("yyyy-MM-dd") : "";
                    pr.ProgramDate2 = objPr.ProgramDate2.HasValue ? objPr.ProgramDate2.Value.ToString("yyyy-MM-dd") : "";
                    pr.ProgramDate3 = objPr.ProgramDate3.HasValue ? objPr.ProgramDate3.Value.ToString("yyyy-MM-dd") : "";
                    pr.MealStartTime = objPr.MealStartTime.HasValue ? objPr.MealStartTime.Value.ToString("HH:mm") : "";
                    pr.ProgramStartTime = objPr.ProgramStartTime.HasValue ? objPr.ProgramStartTime.Value.ToString("HH:mm") : "";
                    pr.ProgramEndTime = objPr.ProgramEndTime.HasValue ? objPr.ProgramEndTime.Value.ToString("HH:mm") : "";
                    

                    SessionList = GetSessionCredits(objPr.ProgramRequestID);

                    foreach (var item in SessionList)
                    {
                        switch (item.id)
                        {
                            case 1:
                                pr.SessionCredit1 = true;
                                break;
                            case 2:
                                pr.SessionCredit2 = true;
                                break;
                            case 3:
                                pr.SessionCredit3 = true;
                                break;
                            case 4:
                                pr.SessionCredit4 = true;
                                break;
                            case 5:
                                pr.SessionCredit5 = true;
                                break;
                            case 6:
                                pr.SessionCredit6 = true;
                                break;
                            case 7:
                                pr.SessionCredit7 = true;
                                break;
                            case 8:
                                pr.SessionCredit8 = true;
                                break;
                            case 9:
                                pr.SessionCredit9 = true;
                                break;
                            case 10:
                                pr.SessionCredit10 = true;
                                break;
                            case 11:
                                pr.SessionCredit11 = true;
                                break;
                            case 12:
                                pr.SessionCredit12 = true;
                                break;
                        }                       

                    }


                    pr.MultiSession = objPr.MultiSession;
                    pr.ProgramSpeakerID = objPr.ProgramSpeakerID ?? 0;
                    pr.ProgramSpeaker2ID = objPr.ProgramSpeaker2ID ?? 0;
                    pr.ProgramModeratorID = objPr.ProgramModeratorID ?? 0;

                    pr.Speakers = Entities.UserInfoes.Where(x => (((x.UserType == 2) || (x.UserType == 3)) && ((x.Status == 2) || (x.Status == 3) || (x.Status == 4)))).Select(c => new SelectListItem
                    {
                        Value = c.id.ToString(),
                        Text = c.FirstName + " " + c.LastName

                    });


                    pr.Moderators = Entities.UserInfoes.Where(x => (((x.UserType == 2) || (x.UserType == 3)) && ((x.Status == 2) || (x.Status == 3) || (x.Status == 4)))).Select(c => new SelectListItem
                    {
                        Value = c.id.ToString(),
                        Text = c.FirstName + " " + c.LastName

                    });

                    pr.VenueContacted = objPr.VenueContacted;
                    pr.LocationType = objPr.LocationType;
                    pr.LocationTypeOther = objPr.LocationTypeOther;
                    pr.LocationName = objPr.LocationName;
                    pr.LocationAddress = objPr.LocationAddress;
                    pr.LocationCity = objPr.LocationCity;
                    pr.LocationProvince = objPr.LocationProvince;
                    pr.LocationPhoneNumber = objPr.LocationPhoneNumber;
                    pr.LocationWebsite = objPr.LocationWebsite;

                   


                    pr.MealType = objPr.MealType;
                    pr.CostPerPerson = objPr.CostPerPerson ?? 0;
                    pr.CostPerparticipants = objPr.CostByParticipant ?? 0;
                    pr.AVEquipment = objPr.AVEquipment;
                    pr.Comments = objPr.Comments;
                    pr.RequestStatus = (objPr.RequestStatus.HasValue ? objPr.RequestStatus.Value : 0).ToString();
                    pr.ReadOnly = objPr.ReadOnly.HasValue ? objPr.ReadOnly.Value : false;
                    pr.AdminVenueConfirmed = objPr.AdminVenueAvailable;
                    pr.AdminUserID = objPr.AdminUserID ?? 0;
                    pr.AdditionalSessionContact = objPr.AdditionalSessionContact;
                    pr.AdditionalContactName = objPr.AdditionalContactName;
                    pr.AdditionalContactPhone = objPr.AdditionalContactPhone;
                    pr.AdditionalContactEmail = objPr.AdditionalContactEmail;
                    pr.EventType = objPr.EventType;
                    pr.EventTypeQuestion1 = objPr.EventTypeQuestion1;
                    pr.EventTypeQuestion2 = objPr.EventTypeQuestion2;
                    pr.EventTypeQuestion3 = objPr.EventTypeQuestion3;
                    pr.EventTypeQuestion4 = objPr.EventTypeQuestion4;
                    pr.EventTypeQuestion5 = objPr.EventTypeQuestion5;
                    pr.RegistrationArrivalTime = objPr.RegistrationArrivalTime.HasValue ? objPr.ProgramStartTime.Value.ToString("HH:mm") : "";
                    
                    pr.MealOption = objPr.MealOption;
                }
            }

            return pr;
        }


        public void CancelProgramRequest(ProgramRequestCancellationVM pr)
        {

            var val = Entities.ProgramRequests.Where(x => x.ProgramRequestID == pr.ProgramRequestID).SingleOrDefault();
            if (val != null)
            {

                val.CancellationRequested = true;
                val.CancellationReason = pr.CancellationReason;
                Entities.SaveChanges();

            }
        }
        public ProgramRequestCancellationVM GetProgramRequestCancellationbyID(int ProgramRequestId)
        {
            var CurrentUser = UserHelper.GetLoggedInUser();
            ProgramRequestCancellationVM pr = new ProgramRequestCancellationVM();
            if (CurrentUser != null)
            {



                var objPr = Entities.ProgramRequests.Where(x => x.ProgramRequestID == ProgramRequestId).SingleOrDefault();

                if (objPr != null)
                {
                    pr.ContactName = CurrentUser.FirstName + " " + CurrentUser.LastName;
                    pr.ContactFirstName = CurrentUser.FirstName;
                    pr.ContactLastName = CurrentUser.LastName;
                    pr.ContactEmail = CurrentUser.Username;
                    pr.ContactPhone = CurrentUser.Phone;
                    pr.ProgramRequestID = objPr.ProgramRequestID;
                    pr.ProgramID = objPr.ProgramID ?? 0;


                    pr.ProgramSpeakerID = objPr.ProgramSpeakerID ?? 0;
                    pr.ProgramModeratorID = objPr.ProgramModeratorID ?? 0;


                    pr.Comments = objPr.Comments;

                    pr.LocationAddress = objPr.LocationAddress;
                    pr.LocationCity = objPr.LocationCity;
                    pr.LocationName = objPr.LocationName;
                    pr.LocationPhoneNumber = objPr.LocationPhoneNumber;
                    pr.LocationProvince = objPr.LocationProvince;
                    pr.LocationType = objPr.LocationType;
                    pr.LocationWebsite = objPr.LocationWebsite;



                    /* added to support sessioncancellation */
                    pr.CancellationReason = objPr.CancellationReason;
                    pr.CancellationRequested = objPr.CancellationRequested;
                    pr.ConfirmedSessionDate = objPr.ConfirmedSessionDate.HasValue ? objPr.ConfirmedSessionDate.Value.ToString("yyyy-MM-dd") : "";
                    pr.SpeakerName = objPr.ProgramSpeakerID.HasValue ? objPr.SpeakerInfo.FirstName + " " + objPr.SpeakerInfo.LastName : "";
                    pr.Speaker2Name = objPr.ProgramSpeaker2ID.HasValue ? objPr.Speaker2Info.FirstName + " " + objPr.Speaker2Info.LastName : "";
                    pr.ModeratorName = objPr.ProgramModeratorID.HasValue ? objPr.ModeratorInfo.FirstName + " " + objPr.ModeratorInfo.LastName : "";
                    /*end of session cancellation*/



                    pr.RequestStatus = (objPr.RequestStatus.HasValue ? objPr.RequestStatus.Value : 0).ToString();



                }



            }

            return pr;


        }
        public void ModifyProgramRequest(ProgramRequestModifyVM pr)
        {

            var val = Entities.ProgramRequests.Where(x => x.ProgramRequestID == pr.ProgramRequestID).SingleOrDefault();
            if (val != null)
            {

                val.ModifyRequested = true;
                val.ModifyReason = pr.ModifyReason;
                Entities.SaveChanges();

            }
        }

        /*
                public EventRequestModel GetProgramRequestbyQueryString(int ProgramRequestId)
                {
                    var CurrentUser = UserHelper.GetLoggedInUser();
                    EventRequestModel pr = new EventRequestModel();
                    if (CurrentUser != null)
                    {

                        List<EventRequestSessionCredits> SessionList = new List<EventRequestSessionCredits>();

                        var objPr = Entities.ProgramRequests.Where(x => x.ProgramRequestID == ProgramRequestId).SingleOrDefault();

                        if (objPr != null)
                        {
                            pr.ContactName = objPr.ContactFirstName + ", " + objPr.ContactLastName;
                            pr.ContactFirstName = objPr.ContactFirstName;
                            pr.ContactLastName = objPr.ContactLastName;
                            pr.ContactEmail = objPr.ContactEmail;
                            pr.ContactPhone = objPr.ContactPhone;
                            pr.ProgramRequestID = objPr.ProgramRequestID;
                            pr.ProgramID = objPr.ProgramID ?? 0;
                            pr.UserID = objPr.UserID ?? 0;


                            int? TherapeuticID = 0;







                            pr.Speakers = Entities.UserInfoes.Where(x => (((x.UserType == 2) || (x.UserType == 3)) && ((x.Status == 2) || (x.Status == 3) || (x.Status == 4)))).Select(c => new SelectListItem
                            {
                                Value = c.id.ToString(),
                                Text = c.FirstName + " " + c.LastName

                            });
                            //get approved moderators only
                            pr.Moderators = Entities.UserInfoes.Where(x => (((x.UserType == 2) || (x.UserType == 3)) && ((x.Status == 2) || (x.Status == 3) || (x.Status == 4)))).Select(c => new SelectListItem
                            {
                                Value = c.id.ToString(),
                                Text = c.FirstName + " " + c.LastName

                            });


                            pr.ProgramSpeakerID = objPr.ProgramSpeakerID ?? 0;
                            pr.ProgramModeratorID = objPr.ProgramModeratorID ?? 0;

                            pr.AVEquipment = objPr.AVEquipment;
                            pr.Comments = objPr.Comments;
                            pr.CostPerparticipants = objPr.CostByParticipant ?? 0;
                            pr.CostPerPerson = objPr.CostPerPerson ?? 0;
                            pr.LocationAddress = objPr.LocationAddress;
                            pr.LocationCity = objPr.LocationCity;
                            pr.LocationName = objPr.LocationName;
                            pr.LocationPhoneNumber = objPr.LocationPhoneNumber;
                            pr.LocationProvince = objPr.LocationProvince;
                            pr.LocationType = objPr.LocationType;
                            pr.LocationWebsite = objPr.LocationWebsite;
                            pr.MealStartTime = objPr.MealStartTime.HasValue ? objPr.MealStartTime.Value.ToString("HH:mm") : "";
                            pr.MealType = objPr.MealType;
                            pr.MultiSession = objPr.MultiSession;

                            pr.ProgramDate1 = objPr.ProgramDate1.HasValue ? objPr.ProgramDate1.Value.ToString("yyyy-MM-dd") : "";
                            pr.ProgramDate2 = objPr.ProgramDate2.HasValue ? objPr.ProgramDate2.Value.ToString("yyyy-MM-dd") : "";
                            pr.ProgramDate3 = objPr.ProgramDate3.HasValue ? objPr.ProgramDate3.Value.ToString("yyyy-MM-dd") : "";
                            pr.ProgramEndTime = objPr.ProgramEndTime.HasValue ? objPr.ProgramEndTime.Value.ToString("HH:mm") : "";
                            pr.ProgramStartTime = objPr.ProgramStartTime.HasValue ? objPr.ProgramStartTime.Value.ToString("HH:mm") : "";


                            ////update session credits here if new program is added here
                            SessionList = GetSessionCredits(objPr.ProgramRequestID);

                            foreach (var item in SessionList)
                            {
                                if (item.id == 1)
                                {

                                    pr.SessionCredit1 = true;
                                }

                                if (item.id == 2)
                                {

                                    pr.SessionCredit2 = true;
                                }

                                if (item.id == 3)
                                {

                                    pr.SessionCredit3 = true;
                                }

                                if (item.id == 4)
                                {

                                    pr.SessionCredit4 = true;
                                }

                                if (item.id == 5)
                                {

                                    pr.SessionCredit5 = true;
                                }
                                if (item.id == 6)
                                {
                                    pr.SessionCredit6 = true;
                                }
                                if (item.id == 7)
                                {
                                    pr.SessionCredit7 = true;
                                }
                                if (item.id == 8)
                                {
                                    pr.SessionCredit8 = true;
                                }
                                if (item.id == 9)
                                {
                                    pr.SessionCredit9 = true;
                                }
                                if (item.id == 10)
                                {
                                    pr.SessionCredit10 = true;
                                }
                                if (item.id == 11)
                                {
                                    pr.SessionCredit11 = true;
                                }
                                if (item.id == 12)
                                {
                                    pr.SessionCredit12 = true;
                                }

                            }


                            pr.RequestStatus = (objPr.RequestStatus.HasValue ? objPr.RequestStatus.Value : 0).ToString();
                            pr.ReadOnly = objPr.ReadOnly.HasValue ? objPr.ReadOnly.Value : false;
                            pr.VenueContacted = objPr.VenueContacted;
                            pr.AdminVenueConfirmed = objPr.AdminVenueAvailable;


                        }



                    }

                    return pr;


                }*/

        /*
                public List<EventRequestSessionCredits> GetSessionCredits(int ProgramRequestId)
                {

                    //retreieve the SessionCreditID from the ProgramRequestSessionCredits table
                    List<EventRequestSessionCredits> SessionsList = new List<EventRequestSessionCredits>();


                    var list = Entities.ProgramRequestSessionCredits.Where(x => x.ProgramRequestID == ProgramRequestId).ToList();

                    foreach (var item in list)
                    {


                        SessionsList.Add(new EventRequestSessionCredits
                        {
                            id = item.SessionCreditID ?? 0,
                            ProgramRequestID = item.ProgramRequestID ?? 0
                        });

                    }

                    return SessionsList;

                }*/
        /*
                public void SaveNewSession(EventRequestModel pr)
                {

                    var val = Entities.ProgramRequests.Where(x => x.ProgramRequestID == pr.ProgramRequestID).SingleOrDefault();
                    EventRequestSessionCredits credits = new EventRequestSessionCredits();

                    if (val != null)
                    {
                        DateTime? dt = null;
                        val.ProgramID = pr.ProgramID;
                        val.UserID = pr.UserID;


                        val.ContactFirstName = pr.ContactFirstName;
                        val.ContactLastName = pr.ContactLastName;
                        val.ContactEmail = pr.ContactEmail;
                        val.ContactPhone = pr.ContactPhone;
                        val.ProgramDate1 = DateTime.ParseExact(pr.ProgramDate1, "yyyy/MM/dd", null);

                        val.ProgramDate2 = !(string.IsNullOrEmpty(pr.ProgramDate2)) ? (DateTime.ParseExact(pr.ProgramDate2, "yyyy/MM/dd", null)) : dt;
                        val.ProgramDate3 = !(string.IsNullOrEmpty(pr.ProgramDate3)) ? (DateTime.ParseExact(pr.ProgramDate3, "yyyy/MM/dd", null)) : dt;

                        DateTime MealStart = DateTime.ParseExact(pr.ProgramDate1, "yyyy/MM/dd", null);
                        MealStart = MealStart.Add(TimeSpan.Parse(pr.MealStartTime));
                        val.MealStartTime = MealStart;

                        DateTime ProgramStart = DateTime.ParseExact(pr.ProgramDate1, "yyyy/MM/dd", null);
                        ProgramStart = ProgramStart.Add(TimeSpan.Parse(pr.ProgramStartTime));
                        val.ProgramStartTime = ProgramStart;

                        DateTime ProgramEnd = DateTime.ParseExact(pr.ProgramDate1, "yyyy/MM/dd", null);
                        ProgramEnd = ProgramEnd.Add(TimeSpan.Parse(pr.ProgramEndTime));
                        val.ProgramEndTime = ProgramEnd;
                        val.SpeakerChosenProgramDate = pr.SpeakerChosenProgramDate;
                        val.ModeratorChosenProgramDate = pr.ModeratorChosenProgramDate;


                        val.MultiSession = pr.MultiSession;
                        val.CostPerPerson = Convert.ToDecimal(pr.CostPerPerson);
                        val.CostByParticipant = Convert.ToDecimal(pr.CostPerparticipants);
                        val.ProgramSpeakerID = pr.ProgramSpeakerID;
                        val.ProgramModeratorID = pr.ProgramModeratorID;
                        val.VenueContacted = pr.VenueContacted;
                        val.LocationType = pr.LocationType;
                        val.LocationName = pr.LocationName;
                        val.LocationAddress = pr.LocationAddress;
                        val.LocationCity = pr.LocationCity;
                        val.LocationProvince = pr.LocationProvince;
                        val.LocationPhoneNumber = pr.LocationPhoneNumber;
                        val.LocationWebsite = pr.LocationWebsite;
                        val.MealType = pr.MealType;
                        val.AVEquipment = pr.AVEquipment;
                        val.RequestStatus = 1;
                        val.ReadOnly = true;
                        val.SpeakerStatus = "No Response";

                        if (pr.ProgramModeratorID != null)
                        {
                            val.ModeratorStatus = "No Response";

                        }
                        else
                        {
                            val.ModeratorStatus = "Not Applicable";
                        }
                        val.Comments = pr.Comments;
                        val.SubmittedDate = DateTime.Now;
                        val.LastUpdatedDate = DateTime.Now;





                        Entities.SaveChanges();

                    }

                    var SessionCreditsToDelete = Entities.ProgramRequestSessionCredits.Where(x => x.ProgramRequestID == pr.ProgramRequestID).ToList();

                    if (SessionCreditsToDelete != null)
                    {
                        foreach (var row in SessionCreditsToDelete)
                        {

                            Entities.ProgramRequestSessionCredits.Remove(row);
                        }
                        Entities.SaveChanges();

                    }
                    if (pr.ProgramID == 1)
                    {
                        if (pr.SessionCredit1)
                        {
                            Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                            {
                                SessionCreditID = 1,
                                ProgramRequestID = pr.ProgramRequestID
                            });
                        }

                        if (pr.SessionCredit2)
                        {
                            Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                            {
                                SessionCreditID = 2,
                                ProgramRequestID = pr.ProgramRequestID
                            });
                        }

                        if (pr.SessionCredit3)
                        {
                            Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                            {
                                SessionCreditID = 3,
                                ProgramRequestID = pr.ProgramRequestID
                            });
                        }

                        if (pr.SessionCredit4)
                        {
                            Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                            {
                                SessionCreditID = 4,
                                ProgramRequestID = pr.ProgramRequestID
                            });
                        }

                        if (pr.SessionCredit5)
                        {
                            Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                            {
                                SessionCreditID = 5,
                                ProgramRequestID = pr.ProgramRequestID
                            });
                        }
                        if (pr.SessionCredit6)
                        {
                            Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                            {
                                SessionCreditID = 6,
                                ProgramRequestID = pr.ProgramRequestID
                            });
                        }
                        if (pr.SessionCredit7)
                        {
                            Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                            {
                                SessionCreditID = 7,
                                ProgramRequestID = pr.ProgramRequestID
                            });
                        }
                        if (pr.SessionCredit8)
                        {
                            Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                            {
                                SessionCreditID = 8,
                                ProgramRequestID = pr.ProgramRequestID
                            });
                        }
                        if (pr.SessionCredit9)
                        {
                            Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                            {
                                SessionCreditID = 9,
                                ProgramRequestID = pr.ProgramRequestID
                            });
                        }
                        if (pr.SessionCredit10)
                        {
                            Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                            {
                                SessionCreditID = 10,
                                ProgramRequestID = pr.ProgramRequestID
                            });
                        }
                        if (pr.SessionCredit11)
                        {
                            Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                            {
                                SessionCreditID = 11,
                                ProgramRequestID = pr.ProgramRequestID
                            });
                        }
                        if (pr.SessionCredit12)
                        {
                            Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                            {
                                SessionCreditID = 12,
                                ProgramRequestID = pr.ProgramRequestID
                            });
                        }
                    }
                    //else if (pr.ProgramID == 2)//the clinical exchange program is different from New Horizon program in CV
                    //{
                    //    if (pr.SessionCredit1)
                    //    {
                    //        Entities.ProgramRequestSessionCredits.Add(new CPDPortal.Data.ProgramRequestSessionCredit
                    //        {
                    //            SessionCreditID = 6,
                    //            ProgramRequestID = pr.ProgramRequestID
                    //        });
                    //    }

                    //    if (pr.SessionCredit2)
                    //    {
                    //        Entities.ProgramRequestSessionCredits.Add(new CPDPortal.Data.ProgramRequestSessionCredit
                    //        {
                    //            SessionCreditID = 7,
                    //            ProgramRequestID = pr.ProgramRequestID
                    //        });
                    //    }

                    //    if (pr.SessionCredit3)
                    //    {
                    //        Entities.ProgramRequestSessionCredits.Add(new CPDPortal.Data.ProgramRequestSessionCredit
                    //        {
                    //            SessionCreditID = 8,
                    //            ProgramRequestID = pr.ProgramRequestID
                    //        });
                    //    }

                    //    if (pr.SessionCredit4)
                    //    {
                    //        Entities.ProgramRequestSessionCredits.Add(new CPDPortal.Data.ProgramRequestSessionCredit
                    //        {
                    //            SessionCreditID = 9,
                    //            ProgramRequestID = pr.ProgramRequestID
                    //        });
                    //    }

                    //    if (pr.SessionCredit5)
                    //    {
                    //        Entities.ProgramRequestSessionCredits.Add(new CPDPortal.Data.ProgramRequestSessionCredit
                    //        {
                    //            SessionCreditID = 10,
                    //            ProgramRequestID = pr.ProgramRequestID
                    //        });
                    //    }
                    //    if (pr.SessionCredit6)
                    //    {
                    //        Entities.ProgramRequestSessionCredits.Add(new CPDPortal.Data.ProgramRequestSessionCredit
                    //        {
                    //            SessionCreditID = 11,
                    //            ProgramRequestID = pr.ProgramRequestID
                    //        });
                    //    }

                    //}

                    Entities.SaveChanges();

                }*/


        public bool CheckIfProgramRequestExists(int? ProgramRequestID)
        {
            bool val = false;

            val = Entities.ProgramRequests.Any(x => x.ProgramRequestID == ProgramRequestID);

            return val;

        }


        public bool checkifModeratorExist(int ProgramRequestID)
        {
            bool retVal = false;
            int? ModeratorId;

            var val = Entities.ProgramRequests.Where(x => x.ProgramRequestID == ProgramRequestID).SingleOrDefault();
            if (val != null)
            {
                if (val.ProgramModeratorID == null)
                {
                    retVal = false;
                }
                else
                {
                    ModeratorId = val.ProgramModeratorID;
                    var user = Entities.UserInfoes.Where(x => x.id == ModeratorId).SingleOrDefault();
                    if ((user != null) && (user.id > 0))
                    {
                        retVal = true;
                    }
                }

            }
            return retVal;
        }
        public bool checkifSpeaker2Exist(int ProgramRequestID)
        {
            bool retVal = false;
            int? Speaker2ID;

            var val = Entities.ProgramRequests.Where(x => x.ProgramRequestID == ProgramRequestID).SingleOrDefault();
            if (val != null)
            {
                if (val.ProgramSpeaker2ID == null)
                {
                    retVal = false;
                }
                else
                {
                    Speaker2ID = val.ProgramSpeaker2ID;
                    var user = Entities.UserInfoes.Where(x => x.id == Speaker2ID).SingleOrDefault();
                    if ((user != null) && (user.id > 0))
                    {
                        retVal = true;
                    }
                }

            }
            return retVal;
        }


        public bool CompareProgramDates(EventRequestModel FromUser)
        {
            DateTime? dt = null;
            bool retval = false;
            var FromDb = Entities.ProgramRequests.Where(x => x.ProgramRequestID == FromUser.ProgramRequestID).SingleOrDefault();
            DateTime ProgramDate1 = DateTime.ParseExact(FromUser.ProgramDate1, "yyyy/MM/dd", null);

            DateTime? ProgramDate2 = !(string.IsNullOrEmpty(FromUser.ProgramDate2)) ? (DateTime.ParseExact(FromUser.ProgramDate2, "yyyy/MM/dd", null)) : dt;
            DateTime? ProgramDate3 = !(string.IsNullOrEmpty(FromUser.ProgramDate3)) ? (DateTime.ParseExact(FromUser.ProgramDate3, "yyyy/MM/dd", null)) : dt;


            if (FromDb != null)
            {

                if ((FromDb.ProgramDate1 != ProgramDate1) || (FromDb.ProgramDate2 != ProgramDate2) || (FromDb.ProgramDate3 != ProgramDate3))
                {
                    retval = true;

                }

            }

            return retval;
        }
        /*
                public EventRequestModel GetProgramRequestForSpeaker(int ProgramRequestId)
                {

                    EventRequestModel pr = new EventRequestModel();


                    List<EventRequestSessionCredits> SessionList = new List<EventRequestSessionCredits>();

                    var objPr = Entities.ProgramRequests.Where(x => x.ProgramRequestID == ProgramRequestId).SingleOrDefault();

                    if (objPr != null)
                    {
                        pr.ContactName = objPr.ContactFirstName + " , " + objPr.ContactLastName;
                        pr.ContactFirstName = objPr.ContactFirstName;
                        pr.ContactLastName = objPr.ContactLastName;
                        pr.ContactEmail = objPr.ContactEmail;
                        pr.ContactPhone = objPr.ContactPhone;
                        pr.ProgramRequestID = objPr.ProgramRequestID;
                        pr.ProgramID = objPr.ProgramID ?? 0;
                        pr.UserID = objPr.UserID ?? 0;
                        //get approved speakers
                        pr.Speakers = Entities.UserInfoes.Where(x => ((x.UserType == 2) || (x.UserType == 3) && ((x.Status == 2) || (x.Status == 3) || (x.Status == 4)))).Select(c => new SelectListItem
                        {
                            Value = c.id.ToString(),
                            Text = c.FirstName + " " + c.LastName

                        });
                        //get approved moderators only
                        pr.Moderators = Entities.UserInfoes.Where(x => ((x.UserType == 2) || (x.UserType == 3) && ((x.Status == 2) || (x.Status == 3) || (x.Status == 4)))).Select(c => new SelectListItem
                        {
                            Value = c.id.ToString(),
                            Text = c.FirstName + " " + c.LastName

                        });

                        pr.ProgramSpeakerID = objPr.ProgramSpeakerID ?? 0;
                        pr.ProgramModeratorID = objPr.ProgramModeratorID ?? 0;

                        pr.AVEquipment = objPr.AVEquipment;
                        pr.Comments = objPr.Comments;
                        pr.CostPerparticipants = objPr.CostByParticipant ?? 0;
                        pr.CostPerPerson = objPr.CostPerPerson ?? 0;
                        pr.LocationAddress = objPr.LocationAddress;
                        pr.LocationCity = objPr.LocationCity;
                        pr.LocationName = objPr.LocationName;
                        pr.LocationPhoneNumber = objPr.LocationPhoneNumber;
                        pr.LocationProvince = objPr.LocationProvince;
                        pr.LocationType = objPr.LocationType;
                        pr.LocationWebsite = objPr.LocationWebsite;
                        pr.MealStartTime = objPr.MealStartTime.HasValue ? objPr.MealStartTime.Value.ToString("HH:mm") : "";
                        pr.MealType = objPr.MealType;
                        pr.MultiSession = objPr.MultiSession;

                        pr.ProgramDate1 = objPr.ProgramDate1.HasValue ? objPr.ProgramDate1.Value.ToString("yyyy-MM-dd") : "";
                        pr.ProgramDate2 = objPr.ProgramDate2.HasValue ? objPr.ProgramDate2.Value.ToString("yyyy-MM-dd") : "";
                        pr.ProgramDate3 = objPr.ProgramDate3.HasValue ? objPr.ProgramDate3.Value.ToString("yyyy-MM-dd") : "";
                        pr.ProgramEndTime = objPr.ProgramEndTime.HasValue ? objPr.ProgramEndTime.Value.ToString("HH:mm") : "";
                        pr.ProgramStartTime = objPr.ProgramStartTime.HasValue ? objPr.ProgramStartTime.Value.ToString("HH:mm") : "";

                        SessionList = GetSessionCredits(objPr.ProgramRequestID);

                        foreach (var item in SessionList)
                        {
                            if (item.id == 1)
                            {

                                pr.SessionCredit1 = true;
                            }

                            if (item.id == 2)
                            {

                                pr.SessionCredit2 = true;
                            }

                            if (item.id == 3)
                            {

                                pr.SessionCredit3 = true;
                            }

                            if (item.id == 4)
                            {

                                pr.SessionCredit4 = true;
                            }

                            if (item.id == 5)
                            {

                                pr.SessionCredit5 = true;
                            }
                            if (item.id == 6)
                            {

                                pr.SessionCredit6 = true;
                            }
                            if (item.id == 7)
                            {

                                pr.SessionCredit7 = true;
                            }
                            if (item.id == 8)
                            {

                                pr.SessionCredit8 = true;
                            }
                            if (item.id == 9)
                            {

                                pr.SessionCredit9 = true;
                            }
                            if (item.id == 10)
                            {

                                pr.SessionCredit10 = true;
                            }
                            if (item.id == 11)
                            {

                                pr.SessionCredit11 = true;
                            }
                            if (item.id == 12)
                            {

                                pr.SessionCredit12 = true;
                            }
                        }


                        pr.RequestStatus = (objPr.RequestStatus.HasValue ? objPr.RequestStatus.Value : 0).ToString();
                        pr.ReadOnly = objPr.ReadOnly.HasValue ? objPr.ReadOnly.Value : false;
                        pr.VenueContacted = objPr.VenueContacted;

                    }

                    return pr;


                }*/

        /*
    public ProgramRequest PopulateSpeakerModeratorDropdowns(int ProgramID)
    {

        ProgramRequest pr = new ProgramRequest();
        int? TherapeuticID = 0;

        var GetTherapeuticID = Entities.TherapeuticPrograms.Where(x => x.ProgramID == ProgramID).FirstOrDefault();

        if (GetTherapeuticID != null)
        {

            TherapeuticID = GetTherapeuticID.TherapeuticID ?? 0;
        }

        if (TherapeuticID == 1)
        {

            pr.Speakers = Entities.UserInfoes.Where(x => (((x.UserType == 2) || (x.UserType == 3)) && ((x.Status == 2) || (x.Status == 3) || (x.Status == 4)) && ((x.TherapeuticID == 1) || (x.TherapeuticID == 3)))).Select(c => new SelectListItem
            {
                Value = c.id.ToString(),
                Text = c.FirstName + " " + c.LastName

            });
            //get approved moderators only
            pr.Moderators = Entities.UserInfoes.Where(x => (((x.UserType == 2) || (x.UserType == 3)) && ((x.Status == 2) || (x.Status == 3) || (x.Status == 4)) && ((x.TherapeuticID == 1) || (x.TherapeuticID == 3)))).Select(c => new SelectListItem
            {
                Value = c.id.ToString(),
                Text = c.FirstName + " " + c.LastName

            });
        }


        if (TherapeuticID == 2)
        {
            pr.Speakers = Entities.UserInfoes.Where(x => (((x.UserType == 2) || (x.UserType == 3)) && ((x.Status == 2) || (x.Status == 3) || (x.Status == 4)) && ((x.TherapeuticID == 2) || (x.TherapeuticID == 3)))).Select(c => new SelectListItem
            {
                Value = c.id.ToString(),
                Text = c.FirstName + " " + c.LastName

            });
            //get approved moderators only
            pr.Moderators = Entities.UserInfoes.Where(x => (((x.UserType == 2) || (x.UserType == 3)) && ((x.Status == 2) || (x.Status == 3) || (x.Status == 4)) && ((x.TherapeuticID == 2) || (x.TherapeuticID == 3)))).Select(c => new SelectListItem
            {
                Value = c.id.ToString(),
                Text = c.FirstName + " " + c.LastName

            });
        }


        if (TherapeuticID == 3)
        {
            pr.Speakers = Entities.UserInfoes.Where(x => (((x.UserType == 2) || (x.UserType == 3)) && ((x.Status == 2) || (x.Status == 3) || (x.Status == 4)) && ((x.TherapeuticID == 3)))).Select(c => new SelectListItem
            {
                Value = c.id.ToString(),
                Text = c.FirstName + " " + c.LastName

            });
            //get approved moderators only
            pr.Moderators = Entities.UserInfoes.Where(x => (((x.UserType == 2) || (x.UserType == 3)) && ((x.Status == 2) || (x.Status == 3) || (x.Status == 4)) && ((x.TherapeuticID == 3)))).Select(c => new SelectListItem
            {
                Value = c.id.ToString(),
                Text = c.FirstName + " " + c.LastName

            });
        }
        return pr;


    }*/
        public bool CheckIfModeratorChanges(EventRequestModel FromUser)
        {
            int? ModeratorId = null;
            bool retval = false;
            var FromDb = Entities.ProgramRequests.Where(x => x.ProgramRequestID == FromUser.ProgramRequestID).SingleOrDefault();

            ModeratorId = FromUser.ProgramModeratorID;
            if (FromDb != null)
            {
                if ((FromDb.ProgramModeratorID != ModeratorId))
                {
                    retval = true;
                }
            }
            return retval;
        }

        public bool CheckIfSpeaker2Changes(EventRequestModel FromUser)
        {
            int? Speaker2Id = null;
            bool retval = false;
            var FromDb = Entities.ProgramRequests.Where(x => x.ProgramRequestID == FromUser.ProgramRequestID).SingleOrDefault();

            Speaker2Id = FromUser.ProgramSpeaker2ID;
            if (FromDb != null)
            {
                if ((FromDb.ProgramSpeaker2ID != Speaker2Id))
                {
                    retval = true;
                }
            }
            return retval;
        }
        public bool CheckIfSpeakerChanges(EventRequestModel FromUser)
        {

            bool retval = false;
            var FromDb = Entities.ProgramRequests.Where(x => x.ProgramRequestID == FromUser.ProgramRequestID).SingleOrDefault();

            int SpeakerId = FromUser.ProgramSpeakerID;

            if (FromDb != null)
            {
                if ((FromDb.ProgramSpeakerID != SpeakerId))
                {
                    retval = true;
                }
            }
            return retval;
        }
     
        public string GetSpeakerConfirmationDate(int ProgramRequestID)
        {
            string Date = "";

            var date = Entities.ProgramRequests.Where(x => x.ProgramRequestID == ProgramRequestID).FirstOrDefault();
            if (date != null)
            {
                Date = date.SpeakerConfirmedProgramDate.HasValue ? date.SpeakerConfirmedProgramDate.Value.ToString("yyyy/MM/dd") : "";

            }
            return Date;
        }
        public EventRequestModel GetProgramRequestForSpeaker(int ProgramRequestId)
        {

            EventRequestModel pr = new EventRequestModel();


            List<ProgramRequestSessionCredits> SessionList = new List<ProgramRequestSessionCredits>();

            var objPr = Entities.ProgramRequests.Where(x => x.ProgramRequestID == ProgramRequestId).SingleOrDefault();

            if (objPr != null)
            {
                pr.ContactName = objPr.ContactFirstName + " , " + objPr.ContactLastName;
                pr.ContactFirstName = objPr.ContactFirstName;
                pr.ContactLastName = objPr.ContactLastName;
                pr.ContactEmail = objPr.ContactEmail;
                pr.ContactPhone = objPr.ContactPhone;
                pr.ProgramRequestID = objPr.ProgramRequestID;
                pr.ProgramID = objPr.ProgramID ?? 0;
                pr.UserID = objPr.UserID ?? 0;
                //get approved speakers
                pr.Speakers = Entities.UserInfoes.Where(x => ((x.UserType == 2) || (x.UserType == 3) && ((x.Status == 2) || (x.Status == 3) || (x.Status == 4)))).Select(c => new SelectListItem
                {
                    Value = c.id.ToString(),
                    Text = c.FirstName + " " + c.LastName

                });
                //get approved moderators only
                pr.Moderators = Entities.UserInfoes.Where(x => ((x.UserType == 2) || (x.UserType == 3) && ((x.Status == 2) || (x.Status == 3) || (x.Status == 4)))).Select(c => new SelectListItem
                {
                    Value = c.id.ToString(),
                    Text = c.FirstName + " " + c.LastName

                });

                pr.ProgramSpeakerID = objPr.ProgramSpeakerID ?? 0;
                pr.ProgramSpeaker2ID = objPr.ProgramSpeaker2ID ?? 0;
                pr.ProgramModeratorID = objPr.ProgramModeratorID ?? 0;
                pr.AVEquipment = objPr.AVEquipment;
                pr.Comments = objPr.Comments;
                pr.CostPerparticipants = objPr.CostByParticipant ?? 0;
                pr.CostPerPerson = objPr.CostPerPerson ?? 0;
                pr.LocationAddress = objPr.LocationAddress;
                pr.LocationCity = objPr.LocationCity;
                pr.LocationName = objPr.LocationName;
                pr.LocationPhoneNumber = objPr.LocationPhoneNumber;
                pr.LocationProvince = objPr.LocationProvince;
                pr.LocationType = objPr.LocationType;
                pr.LocationWebsite = objPr.LocationWebsite;
                pr.MealStartTime = objPr.MealStartTime.HasValue ? objPr.MealStartTime.Value.ToString("HH:mm") : "";
                pr.MealType = objPr.MealType;
                pr.MultiSession = objPr.MultiSession;

                pr.ProgramDate1 = objPr.ProgramDate1.HasValue ? objPr.ProgramDate1.Value.ToString("yyyy-MM-dd") : "";
                pr.ProgramDate2 = objPr.ProgramDate2.HasValue ? objPr.ProgramDate2.Value.ToString("yyyy-MM-dd") : "";
                pr.ProgramDate3 = objPr.ProgramDate3.HasValue ? objPr.ProgramDate3.Value.ToString("yyyy-MM-dd") : "";
                pr.ProgramEndTime = objPr.ProgramEndTime.HasValue ? objPr.ProgramEndTime.Value.ToString("HH:mm") : "";
                pr.ProgramStartTime = objPr.ProgramStartTime.HasValue ? objPr.ProgramStartTime.Value.ToString("HH:mm") : "";
                pr.SessionAgendaFileName = objPr.SessionAgendaFileName;

                SessionList = GetSessionCredits(objPr.ProgramRequestID);

                foreach (var item in SessionList)
                {
                    if (item.id == 1)
                    {

                        pr.SessionCredit1 = true;
                    }

                    if (item.id == 2)
                    {

                        pr.SessionCredit2 = true;
                    }

                    if (item.id == 3)
                    {

                        pr.SessionCredit3 = true;
                    }

                    if (item.id == 4)
                    {

                        pr.SessionCredit4 = true;
                    }

                    if (item.id == 5)
                    {

                        pr.SessionCredit5 = true;
                    }
                    if (item.id == 6)
                    {

                        pr.SessionCredit6 = true;
                    }
                    if (item.id == 7)
                    {

                        pr.SessionCredit7 = true;
                    }
                    if (item.id == 8)
                    {

                        pr.SessionCredit8 = true;
                    }
                    if (item.id == 9)
                    {

                        pr.SessionCredit9 = true;
                    }
                    if (item.id == 10)
                    {

                        pr.SessionCredit10 = true;
                    }
                    if (item.id == 11)
                    {

                        pr.SessionCredit11 = true;
                    }
                    if (item.id == 12)
                    {

                        pr.SessionCredit12 = true;
                    }

                }

                pr.SessionAgendaUploaded = objPr.SessionAgendaUploaded;
                pr.RequestStatus = (objPr.RequestStatus.HasValue ? objPr.RequestStatus.Value : 0).ToString();
                pr.ReadOnly = objPr.ReadOnly.HasValue ? objPr.ReadOnly.Value : false;
                pr.VenueContacted = objPr.VenueContacted;

            }

            return pr;


        }

        public void UpdateSpeakerConfirmDateWhenNoModerator(int ProgramRequestId, string date)
        {
            var val = Entities.ProgramRequests.Where(x => x.ProgramRequestID == ProgramRequestId).SingleOrDefault();
            if (val != null)
            {

                val.SpeakerConfirmedProgramDate = DateTime.ParseExact(date, "yyyy/MM/dd", null);
                val.SpeakerConfirmationDate = DateTime.Now;
                val.SpeakerChosenProgramDate = true;
                val.SpeakerStatus = "Accepted";
                val.ConfirmedSessionDate = DateTime.ParseExact(date, "yyyy/MM/dd", null);
                val.SpeakerProgramDateNA = false;
                Entities.SaveChanges();

            }

        }
      
        public void UpdateProgramRequestWhenVenueChangedByAdmin(int ProgramRequestID)
        {

            var val = Entities.ProgramRequests.Where(x => x.ProgramRequestID == ProgramRequestID).SingleOrDefault();

            val.ReadOnly = false;
            val.RequestStatus = 7;

            Entities.SaveChanges();




        }


        public bool IsVenueChangesBySalesRep(EventRequestModel FromUser)
        {

            bool retval = false;
            var FromDb = Entities.ProgramRequests.Where(x => x.ProgramRequestID == FromUser.ProgramRequestID).SingleOrDefault();



            if (FromDb != null)
            {
                if (FromUser.VenueContacted != "NA")
                {
                    if (!(FromUser.LocationType.Equals(FromDb.LocationType)) || !(FromUser.LocationAddress.Equals(FromDb.LocationAddress))
                        || !(FromUser.LocationName.Equals(FromDb.LocationName)) || !(FromUser.LocationCity.Equals(FromDb.LocationCity))
                        || !(FromUser.LocationProvince.Equals(FromDb.LocationProvince))

                        )

                    {

                        retval = true;

                    }
                }
                else //FromUser.VenuContacted == "NA"
                {
                    if (!FromUser.VenueContacted.Equals(FromDb.VenueContacted))
                    {
                        retval = true;
                    }
                }
            }
            return retval;
        }



        public bool IsMealTimesChangesBySalesRep(EventRequestModel FromUser)
        {

            bool retval = false;
            var FromDb = Entities.ProgramRequests.Where(x => x.ProgramRequestID == FromUser.ProgramRequestID).SingleOrDefault();



            string ProgramEndTime = FromDb.ProgramEndTime.HasValue ? FromDb.ProgramEndTime.Value.ToString("HH:mm") : "";
            string ProgramStartTime = FromDb.ProgramStartTime.HasValue ? FromDb.ProgramStartTime.Value.ToString("HH:mm") : "";

            string MealStartTime = FromDb.MealStartTime.HasValue ? FromDb.MealStartTime.Value.ToString("HH:mm") : "";


            if (FromDb != null)
            {
                if (!(FromUser.ProgramStartTime.Equals(ProgramStartTime)) || !(FromUser.ProgramEndTime.Equals(ProgramEndTime)))
                    //|| !(FromUser.MealStartTime.Equals(MealStartTime)))

                {

                    retval = true;

                }
            }
            return retval;
        }

        public void ResetSpeakerModeratorConfirmDates(int ProgramRequestID)
        {


            var val = Entities.ProgramRequests.Where(x => x.ProgramRequestID == ProgramRequestID).SingleOrDefault();

            val.SpeakerConfirmedProgramDate = null;
            val.SpeakerDeclined = null;
            val.SpeakerConfirmationDate = null;
            val.SpeakerChosenProgramDate = null;
            val.ConfirmedSessionDate = null;
            val.SpeakerProgramDateNA = null;
            val.ModeratorChosenProgramDate = null;
            val.ModeratorConfirmationDate = null;
            val.ModeratorConfirmedProgramDate = null;
            val.ModeratorDeclined = null;
            val.ModeratorProgramDateNA = null;

            Entities.SaveChanges();


        }

        public void ResetModeratorstatusAndConfirmSessionDate(int ProgramRequestID)
        {

            var val = Entities.ProgramRequests.Where(x => x.ProgramRequestID == ProgramRequestID).SingleOrDefault();

            if (val != null)
            {
                val.ModeratorStatus = "No Response";
                val.ModeratorChosenProgramDate = null;
                val.ModeratorConfirmedProgramDate = null;
                val.ModeratorDeclined = null;
                val.ModeratorConfirmationDate = null;
                val.ModeratorProgramDateNA = null;
                val.ConfirmedSessionDate = null;
                Entities.SaveChanges();

            }


        }
        public void ResetSpeaker2statusAndConfirmSessionDate(int ProgramRequestID)
        {

            var val = Entities.ProgramRequests.Where(x => x.ProgramRequestID == ProgramRequestID).SingleOrDefault();

            if (val != null)
            {
                val.Speaker2Status = "No Response";
                val.Speaker2ChosenProgramDate = null;
                val.Speaker2ConfirmedProgramDate = null;
                val.Speaker2Declined = null;
                val.Speaker2ConfirmationDate = null;
                val.Speaker2ProgramDateNA = null;
                val.ConfirmedSessionDate = null;
                Entities.SaveChanges();

            }


        }
        public void UpdateRequestStatusByAdmin(int ProgramRequestID, int StatusId)
        {

            var val = Entities.ProgramRequests.Where(x => x.ProgramRequestID == ProgramRequestID).FirstOrDefault();

            if (val != null)
            {
                val.RequestStatus = StatusId;
                Entities.SaveChanges();
            }

        }


        public List<CompletedProgram> GetUpcomingProgramsByuserid(int CompletedProgramCount, int Speakeruserid)
        {
            List<CompletedProgram> liCompletedProgram = null;

            try
            {

                //confirmed program that are 1, under Review or 2, Active Regional Ethics Review Pending or 3, Active - Regional Ethics Approved  (ie. not closed)
                liCompletedProgram = Entities.ProgramRequests.Where(u => u.ProgramSpeakerID == Speakeruserid && (u.RequestStatus == 1 || u.RequestStatus == 2 || u.RequestStatus == 3) && u.ConfirmedSessionDate != null).
                Select(u => new CompletedProgram
                {

                    ProgramName = u.Program.ProgramName,
                    LocationName = u.LocationName,

                    ConfirmedSessionDate = u.ConfirmedSessionDate == null ? null : SqlFunctions.DateName("year", u.ConfirmedSessionDate) + "/" + SqlFunctions.DatePart("m", u.ConfirmedSessionDate) + "/" + SqlFunctions.DateName("day", u.ConfirmedSessionDate),


                }).ToList();


                return liCompletedProgram;
            }
            catch (Exception e)
            {
                return liCompletedProgram;
            }
        }


        public List<CompletedProgram> GetCompletedProgramsByuserid(int CompletedProgramCount, int Speakeruserid)
        {
            List<CompletedProgram> liCompletedProgram = null;

            try
            {


                liCompletedProgram = Entities.ProgramRequests.Where(u => u.ProgramSpeakerID == Speakeruserid && u.RequestStatus == 4).
                Select(u => new CompletedProgram
                {

                    ProgramName = u.Program.ProgramName,
                    LocationName = u.LocationName,

                    ConfirmedSessionDate = u.ConfirmedSessionDate == null ? null : SqlFunctions.DateName("year", u.ConfirmedSessionDate) + "/" + SqlFunctions.DatePart("m", u.ConfirmedSessionDate) + "/" + SqlFunctions.DateName("day", u.ConfirmedSessionDate),


                }).ToList();


                return liCompletedProgram;
            }
            catch (Exception e)
            {
                return liCompletedProgram;
            }
        }

        public bool SaveNewSession(EventRequestModel pr)
        {

            var val = Entities.ProgramRequests.Where(x => x.ProgramRequestID == pr.ProgramRequestID).SingleOrDefault();
            EventRequestSessionCredits credits = new EventRequestSessionCredits();

            if (val != null)
            {
                DateTime? dt = null;
                val.ProgramID = pr.ProgramID;
                val.UserID = pr.UserID;


                val.ContactFirstName = pr.ContactFirstName;
                val.ContactLastName = pr.ContactLastName;
                val.ContactEmail = pr.ContactEmail;
                val.ContactPhone = pr.ContactPhone;
                val.ProgramDate1 = DateTime.ParseExact(pr.ProgramDate1, "yyyy/MM/dd", null);

                val.ProgramDate2 = !(string.IsNullOrEmpty(pr.ProgramDate2)) ? (DateTime.ParseExact(pr.ProgramDate2, "yyyy/MM/dd", null)) : dt;
                val.ProgramDate3 = !(string.IsNullOrEmpty(pr.ProgramDate3)) ? (DateTime.ParseExact(pr.ProgramDate3, "yyyy/MM/dd", null)) : dt;


                DateTime startTime = DateTime.ParseExact(pr.ProgramDate1, "yyyy/MM/dd", null);
                if (!string.IsNullOrEmpty(pr.MealStartTime))
                {
                    val.MealStartTime = startTime.Add(TimeSpan.Parse(pr.MealStartTime));
                }
                val.ProgramStartTime = startTime.Add(TimeSpan.Parse(pr.ProgramStartTime));
                val.ProgramEndTime = startTime.Add(TimeSpan.Parse(pr.ProgramEndTime));
                if (!string.IsNullOrEmpty(pr.RegistrationArrivalTime))
                {
                    val.RegistrationArrivalTime = startTime.Add(TimeSpan.Parse(pr.RegistrationArrivalTime));
                }


                val.SpeakerChosenProgramDate = pr.SpeakerChosenProgramDate;
                val.SpeakerChosenProgramDate = pr.Speaker2ChosenProgramDate;
                val.ModeratorChosenProgramDate = pr.ModeratorChosenProgramDate;

                val.AdditionalSessionContact = pr.AdditionalSessionContact;
                val.AdditionalContactName = pr.AdditionalContactName;
                val.AdditionalContactPhone = pr.AdditionalContactPhone;
                val.AdditionalContactEmail = pr.AdditionalContactEmail;
                val.EventType = pr.EventType;
                val.EventTypeQuestion1 = pr.EventTypeQuestion1;
                val.EventTypeQuestion2 = pr.EventTypeQuestion2;
                val.EventTypeQuestion3 = pr.EventTypeQuestion3;
                val.EventTypeQuestion4 = pr.EventTypeQuestion4;
                val.EventTypeQuestion5 = pr.EventTypeQuestion5;
                val.MealOption = pr.MealOption;





                val.MultiSession = pr.MultiSession;
                if (pr.CostPerPerson != null)
                {
                    val.CostPerPerson = Convert.ToDecimal(pr.CostPerPerson);
                }
                val.CostByParticipant = Convert.ToDecimal(pr.CostPerparticipants);
                val.ProgramSpeakerID = pr.ProgramSpeakerID;
                val.ProgramSpeaker2ID = pr.ProgramSpeaker2ID;
                val.ProgramModeratorID = pr.ProgramModeratorID;
                val.VenueContacted = pr.VenueContacted;
                val.LocationType = pr.LocationType;
        val.LocationTypeOther = pr.LocationTypeOther;
                val.LocationName = pr.LocationName;
                val.LocationAddress = pr.LocationAddress;
                val.LocationCity = pr.LocationCity;
                val.LocationProvince = pr.LocationProvince;
                val.LocationPhoneNumber = pr.LocationPhoneNumber;
                val.LocationWebsite = pr.LocationWebsite;
                val.MealType = pr.MealType;
                val.AVEquipment = pr.AVEquipment;
                val.RequestStatus = 1;
                val.ReadOnly = true;
                val.SpeakerStatus = "No Response";

                val.SessionAgendaUploaded = pr.SessionAgendaUploaded;
                val.SessionAgendaFileName = pr.SessionAgendaFileName;

                if (pr.ProgramSpeaker2ID != null)
                {
                    val.Speaker2Status = "No Response";
                }
                else
                {
                    val.Speaker2Status = "Not Applicable";
                }


                if (pr.ProgramModeratorID != null)
                {
                    val.ModeratorStatus = "No Response";
                }
                else
                {
                    val.ModeratorStatus = "Not Applicable";
                }
                val.Comments = pr.Comments;
                val.SubmittedDate = DateTime.Now;
                val.LastUpdatedDate = DateTime.Now;


                try
                {
                    Entities.SaveChanges();
                }
                catch (Exception e)
                {
                    return false;
                }

            }
            /*
                        var SessionCreditsToDelete = Entities.ProgramRequestSessionCredits.Where(x => x.ProgramRequestID == pr.ProgramRequestID).ToList();

                        if (SessionCreditsToDelete != null)
                        {
                            foreach (var row in SessionCreditsToDelete)
                            {

                                Entities.ProgramRequestSessionCredits.Remove(row);
                            }
                            Entities.SaveChanges();

                        }*/
            if (pr.ProgramID == 4)
            {
                if (pr.SessionCredit1)
                {
                    Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                    {
                        SessionCreditID = 1,
                        ProgramRequestID = pr.ProgramRequestID
                    });
                }

                if (pr.SessionCredit2)
                {
                    Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                    {
                        SessionCreditID = 2,
                        ProgramRequestID = pr.ProgramRequestID
                    });
                }

                if (pr.SessionCredit3)
                {
                    Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                    {
                        SessionCreditID = 3,
                        ProgramRequestID = pr.ProgramRequestID
                    });
                }

                if (pr.SessionCredit4)
                {
                    Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                    {
                        SessionCreditID = 4,
                        ProgramRequestID = pr.ProgramRequestID
                    });
                }

                if (pr.SessionCredit5)
                {
                    Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                    {
                        SessionCreditID = 5,
                        ProgramRequestID = pr.ProgramRequestID
                    });
                }
                if (pr.SessionCredit6)
                {
                    Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                    {
                        SessionCreditID = 6,
                        ProgramRequestID = pr.ProgramRequestID
                    });
                }
                if (pr.SessionCredit7)
                {
                    Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                    {
                        SessionCreditID = 7,
                        ProgramRequestID = pr.ProgramRequestID
                    });
                }
                if (pr.SessionCredit8)
                {
                    Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                    {
                        SessionCreditID = 8,
                        ProgramRequestID = pr.ProgramRequestID
                    });
                }
                if (pr.SessionCredit9)
                {
                    Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                    {
                        SessionCreditID = 9,
                        ProgramRequestID = pr.ProgramRequestID
                    });
                }
                if (pr.SessionCredit10)
                {
                    Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                    {
                        SessionCreditID = 10,
                        ProgramRequestID = pr.ProgramRequestID
                    });
                }
                if (pr.SessionCredit11)
                {
                    Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                    {
                        SessionCreditID = 11,
                        ProgramRequestID = pr.ProgramRequestID
                    });
                }
                if (pr.SessionCredit12)
                {
                    Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                    {
                        SessionCreditID = 12,
                        ProgramRequestID = pr.ProgramRequestID
                    });
                }

            }
            //else if (pr.ProgramID == 2)//the clinical exchange program is different from New Horizon program in CV
            //{
            //    if (pr.SessionCredit1)
            //    {
            //        Entities.ProgramRequestSessionCredits.Add(new CPDPortal.Data.ProgramRequestSessionCredit
            //        {
            //            SessionCreditID = 6,
            //            ProgramRequestID = pr.ProgramRequestID
            //        });
            //    }

            //    if (pr.SessionCredit2)
            //    {
            //        Entities.ProgramRequestSessionCredits.Add(new CPDPortal.Data.ProgramRequestSessionCredit
            //        {
            //            SessionCreditID = 7,
            //            ProgramRequestID = pr.ProgramRequestID
            //        });
            //    }

            //    if (pr.SessionCredit3)
            //    {
            //        Entities.ProgramRequestSessionCredits.Add(new CPDPortal.Data.ProgramRequestSessionCredit
            //        {
            //            SessionCreditID = 8,
            //            ProgramRequestID = pr.ProgramRequestID
            //        });
            //    }

            //    if (pr.SessionCredit4)
            //    {
            //        Entities.ProgramRequestSessionCredits.Add(new CPDPortal.Data.ProgramRequestSessionCredit
            //        {
            //            SessionCreditID = 9,
            //            ProgramRequestID = pr.ProgramRequestID
            //        });
            //    }

            //    if (pr.SessionCredit5)
            //    {
            //        Entities.ProgramRequestSessionCredits.Add(new CPDPortal.Data.ProgramRequestSessionCredit
            //        {
            //            SessionCreditID = 10,
            //            ProgramRequestID = pr.ProgramRequestID
            //        });
            //    }
            //    if (pr.SessionCredit6)
            //    {
            //        Entities.ProgramRequestSessionCredits.Add(new CPDPortal.Data.ProgramRequestSessionCredit
            //        {
            //            SessionCreditID = 11,
            //            ProgramRequestID = pr.ProgramRequestID
            //        });
            //    }

            //}
            try
            {
                Entities.SaveChanges();
            }
            catch (Exception e)
            {
                return false;
            }
            return true;
        }
        public void UpdateSession(EventRequestModel pr)
        {

            var val = Entities.ProgramRequests.Where(x => x.ProgramRequestID == pr.ProgramRequestID).SingleOrDefault();
            EventRequestSessionCredits credits = new EventRequestSessionCredits();

            if (val != null)
            {
                DateTime? dt = null;
                val.ProgramID = pr.ProgramID;


                val.ContactFirstName = pr.ContactFirstName;
                val.ContactLastName = pr.ContactLastName;
                val.ContactEmail = pr.ContactEmail;
                val.ContactPhone = pr.ContactPhone;
                val.ProgramDate1 = DateTime.ParseExact(pr.ProgramDate1, "yyyy/MM/dd", null);

                val.ProgramDate2 = !(string.IsNullOrEmpty(pr.ProgramDate2)) ? (DateTime.ParseExact(pr.ProgramDate2, "yyyy/MM/dd", null)) : dt;
                val.ProgramDate3 = !(string.IsNullOrEmpty(pr.ProgramDate3)) ? (DateTime.ParseExact(pr.ProgramDate3, "yyyy/MM/dd", null)) : dt;

                DateTime startTime = DateTime.ParseExact(pr.ProgramDate1, "yyyy/MM/dd", null);
                if (!string.IsNullOrEmpty(pr.MealStartTime))
                {
                    val.MealStartTime = startTime.Add(TimeSpan.Parse(pr.MealStartTime));
                }else
                {
                    val.MealStartTime = null;
                }
                val.ProgramStartTime = startTime.Add(TimeSpan.Parse(pr.ProgramStartTime));
                val.ProgramEndTime = startTime.Add(TimeSpan.Parse(pr.ProgramEndTime));

                if (!string.IsNullOrEmpty(pr.RegistrationArrivalTime))
                {
                    val.RegistrationArrivalTime = startTime.Add(TimeSpan.Parse(pr.RegistrationArrivalTime));
                }

                val.AdditionalSessionContact = pr.AdditionalSessionContact;
                val.AdditionalContactName = pr.AdditionalContactName;
                val.AdditionalContactPhone = pr.AdditionalContactPhone;
                val.AdditionalContactEmail = pr.AdditionalContactEmail;
                val.EventType = pr.EventType;
                val.EventTypeQuestion1 = pr.EventTypeQuestion1;
                val.EventTypeQuestion2 = pr.EventTypeQuestion2;
                val.EventTypeQuestion3 = pr.EventTypeQuestion3;
                val.EventTypeQuestion4 = pr.EventTypeQuestion4;
                val.EventTypeQuestion5 = pr.EventTypeQuestion5;
                val.MealOption = pr.MealOption;

                val.SessionAgendaUploaded = pr.SessionAgendaUploaded;
                val.SessionAgendaFileName = pr.SessionAgendaFileName;
                val.LocationTypeOther = pr.LocationTypeOther;

                if (pr.IsAdmin == 1)
                {
                    val.AdminUserID = pr.AdminUserID;
                    val.AdminVenueAvailable = pr.AdminVenueConfirmed;
                    val.AdminEditDate = DateTime.Now;
                    val.AdminEdited = true;

                }
                else
                {
                    val.MultiSession = pr.MultiSession;
                    //val.SessionAgendaFileName = pr.SessionAgendaFileName;
                    //val.SessionAgendaUploaded = pr.SessionAgendaUploaded;
                }

                if (pr.CostPerPerson != null)
                {
                    val.CostPerPerson = Convert.ToDecimal(pr.CostPerPerson);
                }
                val.CostByParticipant = Convert.ToDecimal(pr.CostPerparticipants);
                val.ProgramSpeakerID = pr.ProgramSpeakerID;
                val.ProgramSpeaker2ID = pr.ProgramSpeaker2ID;
                val.ProgramModeratorID = pr.ProgramModeratorID;
                val.VenueContacted = pr.VenueContacted;
                val.LocationType = pr.LocationType;
                val.LocationName = pr.LocationName;
                val.LocationAddress = pr.LocationAddress;
                val.LocationCity = pr.LocationCity;
                val.LocationProvince = pr.LocationProvince;
                val.LocationPhoneNumber = pr.LocationPhoneNumber;
                val.LocationWebsite = pr.LocationWebsite;
                val.MealType = pr.MealType;
                val.AVEquipment = pr.AVEquipment;
                val.RequestStatus = 1;
                val.ReadOnly = true;
                val.Comments = pr.Comments;
                val.LastUpdatedDate = DateTime.Now;
                Entities.SaveChanges();

            }

            var SessionCreditsToDelete = Entities.ProgramRequestSessionCredits.Where(x => x.ProgramRequestID == pr.ProgramRequestID).ToList();

            if (SessionCreditsToDelete != null)
            {
                foreach (var row in SessionCreditsToDelete)
                {

                    Entities.ProgramRequestSessionCredits.Remove(row);
                }
                Entities.SaveChanges();

            }

            if (pr.ProgramID == 4)
            {
                if (pr.SessionCredit1)
                {
                    Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                    {
                        SessionCreditID = 1,
                        ProgramRequestID = pr.ProgramRequestID
                    });
                }

                if (pr.SessionCredit2)
                {
                    Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                    {
                        SessionCreditID = 2,
                        ProgramRequestID = pr.ProgramRequestID
                    });
                }

                if (pr.SessionCredit3)
                {
                    Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                    {
                        SessionCreditID = 3,
                        ProgramRequestID = pr.ProgramRequestID
                    });
                }

                if (pr.SessionCredit4)
                {
                    Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                    {
                        SessionCreditID = 4,
                        ProgramRequestID = pr.ProgramRequestID
                    });
                }

                if (pr.SessionCredit5)
                {
                    Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                    {
                        SessionCreditID = 5,
                        ProgramRequestID = pr.ProgramRequestID
                    });
                }
                if (pr.SessionCredit6)
                {
                    Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                    {
                        SessionCreditID = 6,
                        ProgramRequestID = pr.ProgramRequestID
                    });
                }
                if (pr.SessionCredit7)
                {
                    Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                    {
                        SessionCreditID = 7,
                        ProgramRequestID = pr.ProgramRequestID
                    });
                }
                if (pr.SessionCredit8)
                {
                    Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                    {
                        SessionCreditID = 8,
                        ProgramRequestID = pr.ProgramRequestID
                    });
                }
                if (pr.SessionCredit9)
                {
                    Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                    {
                        SessionCreditID = 9,
                        ProgramRequestID = pr.ProgramRequestID
                    });
                }
                if (pr.SessionCredit10)
                {
                    Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                    {
                        SessionCreditID = 10,
                        ProgramRequestID = pr.ProgramRequestID
                    });
                }
                if (pr.SessionCredit11)
                {
                    Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                    {
                        SessionCreditID = 11,
                        ProgramRequestID = pr.ProgramRequestID
                    });
                }
                if (pr.SessionCredit12)
                {
                    Entities.ProgramRequestSessionCredits.Add(new ClinicalConundrum2019.Data.ProgramRequestSessionCredit
                    {
                        SessionCreditID = 12,
                        ProgramRequestID = pr.ProgramRequestID
                    });
                }
            }
            //else if (pr.ProgramID == 2)//the clinical exchange program is different from New Horizon program in CV
            //{
            //    if (pr.SessionCredit1)
            //    {
            //        Entities.ProgramRequestSessionCredits.Add(new CPDPortal.Data.ProgramRequestSessionCredit
            //        {
            //            SessionCreditID = 6,
            //            ProgramRequestID = pr.ProgramRequestID
            //        });
            //    }

            //    if (pr.SessionCredit2)
            //    {
            //        Entities.ProgramRequestSessionCredits.Add(new CPDPortal.Data.ProgramRequestSessionCredit
            //        {
            //            SessionCreditID = 7,
            //            ProgramRequestID = pr.ProgramRequestID
            //        });
            //    }

            //    if (pr.SessionCredit3)
            //    {
            //        Entities.ProgramRequestSessionCredits.Add(new CPDPortal.Data.ProgramRequestSessionCredit
            //        {
            //            SessionCreditID = 8,
            //            ProgramRequestID = pr.ProgramRequestID
            //        });
            //    }

            //    if (pr.SessionCredit4)
            //    {
            //        Entities.ProgramRequestSessionCredits.Add(new CPDPortal.Data.ProgramRequestSessionCredit
            //        {
            //            SessionCreditID = 9,
            //            ProgramRequestID = pr.ProgramRequestID
            //        });
            //    }

            //    if (pr.SessionCredit5)
            //    {
            //        Entities.ProgramRequestSessionCredits.Add(new CPDPortal.Data.ProgramRequestSessionCredit
            //        {
            //            SessionCreditID = 10,
            //            ProgramRequestID = pr.ProgramRequestID
            //        });
            //    }
            //    if (pr.SessionCredit6)
            //    {
            //        Entities.ProgramRequestSessionCredits.Add(new CPDPortal.Data.ProgramRequestSessionCredit
            //        {
            //            SessionCreditID = 11,
            //            ProgramRequestID = pr.ProgramRequestID
            //        });
            //    }

            //}
            //else if (pr.ProgramID == 3)//bad to bones
            //{//update session credits here if new program is added here
            //    if (pr.SessionCredit1)
            //    {
            //        Entities.ProgramRequestSessionCredits.Add(new CPDPortal.Data.ProgramRequestSessionCredit
            //        {
            //            SessionCreditID = 12,
            //            ProgramRequestID = pr.ProgramRequestID
            //        });
            //    }

            //    if (pr.SessionCredit2)
            //    {
            //        Entities.ProgramRequestSessionCredits.Add(new CPDPortal.Data.ProgramRequestSessionCredit
            //        {
            //            SessionCreditID = 13,
            //            ProgramRequestID = pr.ProgramRequestID
            //        });
            //    }
            //}
            Entities.SaveChanges();

        }
        public SessionUpload GetSessionUpload(int ProgramRequestID)
        {
            if (ProgramRequestID != 0)
            {
                SessionUpload su;
                su = Entities.ProgramRequestFileUploads.Where(x => x.ProgramRequestID == ProgramRequestID).Select(ui =>
                         new SessionUpload
                         {
                             ProgramRequestID = ProgramRequestID,
                             EvaluationFullPath = ui.EvaluationFullPath,
                             EvaluationUploaded = ui.EvaluationUploaded,
                             EvaluationFileName = ui.EvaluationFileName,
                             EvaluationFileExt = ui.EvaluationFileExt,
                             SignInFullPath = ui.SignInFullPath,
                             SignInUploaded = ui.SignInUploaded,
                             SignInFileName = ui.SignInFileName,
                             SignInFileExt = ui.SignInFileExt,
                             UserOtherFullPath = ui.UserOtherFullPath,
                             UserOtherUploaded = ui.UserOtherUploaded,
                             UserOtherFileName = ui.UserOtherFileName,
                             UserOtherFileExt = ui.UserOtherFileExt,
                             SpeakerAgreementFileExt = ui.SpeakerAgreementFileExt,
                             SpeakerAgreementFileName = ui.SpeakerAgreementFileName,
                             SpeakerAgreementFullPath = ui.SpeakerAgreementFullPath,
                             SpeakerAgreementUploaded = ui.SpeakerAgreementUploaded,
                             FinalAttendanceListFileExt = ui.FinalAttendanceListFileExt,
                             FinalAttendanceListFileName = ui.FinalAttendanceListFileName,
                             FinalAttendanceListFullPath = ui.FinalAttendanceListFullPath,
                             FinalAttendanceListUploaded = ui.FinalAttendanceListUploaded


                         }).SingleOrDefault();

                return su;
            }
            else
                return null;
        }
        public ProgramRequestViewModel GetEditProgramRequestByID(int ProgramRequestID)
        {

            if (ProgramRequestID != 0)
            {
                ProgramRequestViewModel pr;
                pr = Entities.ProgramRequests.Where(x => x.ProgramRequestID == ProgramRequestID).Select(ui =>
                         new ProgramRequestViewModel
                         {
                             ProgramRequestID = ProgramRequestID,
                             ContactFirstName = ui.ContactFirstName,
                             ContactLastName = ui.ContactLastName,
                             ContactEmail = ui.ContactEmail,
                             ContactPhone = ui.ContactPhone,
                             ProgramDate1 = ui.ProgramDate1.HasValue ? ui.ProgramDate1.Value.ToString() : "",
                             ProgramStartTime = ui.ProgramStartTime.HasValue ? ui.ProgramStartTime.Value.ToString() : "",
                             ProgramEndTime = ui.ProgramEndTime.HasValue ? ui.ProgramEndTime.Value.ToString() : "",
                             SubmittedDate = ui.SubmittedDate.HasValue ? ui.SubmittedDate.Value.ToString() : "",
                             RequestStatus = ui.RequestStatus ?? 0,
                             Approved = ui.Approved.HasValue ? ui.Approved.Value : false


                         }).SingleOrDefault();

                return pr;
            }
            else
                return null;
        }
        public void EditProgramRequest(ProgramRequestViewModel pr)
        {
            var val = Entities.ProgramRequests.Where(x => x.ProgramRequestID == pr.ProgramRequestID).SingleOrDefault();
            if (val != null)
            {

                val.ContactFirstName = pr.ContactFirstName;
                val.ContactLastName = pr.ContactLastName;
                val.ContactEmail = pr.ContactEmail;
                val.ContactPhone = pr.ContactPhone;
                val.ProgramDate1 = Convert.ToDateTime(pr.ProgramDate1);
                val.ProgramStartTime = Convert.ToDateTime(pr.ProgramStartTime);
                val.ProgramEndTime = Convert.ToDateTime(pr.ProgramEndTime);
                val.SubmittedDate = Convert.ToDateTime(pr.SubmittedDate);
                val.RequestStatus = Convert.ToInt32(pr.RequestStatus);
                val.Approved = pr.Approved;

                Entities.SaveChanges();

            }

        }
        public List<SelectListItem> GetAllProgram()
        {
            List<SelectListItem> programList = new List<SelectListItem>();
            var programs = Entities.Programs.ToList();
            foreach (ClinicalConundrum2019.Data.Program p in programs)
            {
                programList.Add(new SelectListItem
                {
                    Text = p.ProgramName,
                    Value = p.ProgramID.ToString()

                });
            }
            return programList;

        }
        public void DeleteUser(UserModel um)
        {

            var val = Entities.Users.Where(x => x.UserID == um.UserID).SingleOrDefault();
            if (val != null)
            {

                val.IsDeleted = um.Deleted ?? false;
                Entities.SaveChanges();

            }
        }
        public void ApproveProgramRequest(ProgramRequestViewModel pr)
        {

            var val = Entities.ProgramRequests.Where(x => x.ProgramRequestID == pr.ProgramRequestID).SingleOrDefault();
            if (val != null)
            {

                val.Approved = pr.Approved;
                Entities.SaveChanges();

            }
        }
        public void UpdateProgramRequestFileUpload(SessionUpload su)
        {
            var session_upload = (from v in Entities.ProgramRequestFileUploads

                                  where v.ProgramRequestID == su.ProgramRequestID
                                  select v).FirstOrDefault();
            if (session_upload == null)
            {

                ClinicalConundrum2019.Data.ProgramRequestFileUpload objprful = new ClinicalConundrum2019.Data.ProgramRequestFileUpload();

                objprful.ProgramRequestID = su.ProgramRequestID;
                objprful.EvaluationFullPath = su.EvaluationFullPath;
                objprful.EvaluationUploaded = su.EvaluationUploaded;
                objprful.EvaluationFileName = su.EvaluationFileName;
                objprful.EvaluationFileExt = su.EvaluationFileExt;
                //signin sheet
                objprful.SignInFullPath = su.SignInFullPath;
                objprful.SignInUploaded = su.SignInUploaded;
                objprful.SignInFileName = su.SignInFileName;
                objprful.SignInFileExt = su.SignInFileExt;
                //speaker agreement
                objprful.SpeakerAgreementFullPath = su.SpeakerAgreementFullPath;
                objprful.SpeakerAgreementUploaded = su.SpeakerAgreementUploaded;
                objprful.SpeakerAgreementFileName = su.SpeakerAgreementFileName;
                objprful.SpeakerAgreementFileExt = su.SpeakerAgreementFileExt;
                //userother
                objprful.UserOtherFullPath = su.UserOtherFullPath;
                objprful.UserOtherUploaded = su.UserOtherUploaded;
                objprful.UserOtherFileName = su.UserOtherFileName;
                objprful.UserOtherFileExt = su.UserOtherFileExt;

                objprful.SpeakerAgreementFileExt = su.SpeakerAgreementFileExt;
                objprful.SpeakerAgreementFileName = su.SpeakerAgreementFileName;
                objprful.SpeakerAgreementFullPath = su.SpeakerAgreementFullPath;
                objprful.SpeakerAgreementUploaded = su.SpeakerAgreementUploaded;

                objprful.FinalAttendanceListFileExt = su.FinalAttendanceListFileExt;
                objprful.FinalAttendanceListFileName = su.FinalAttendanceListFileName;
                objprful.FinalAttendanceListFullPath = su.FinalAttendanceListFullPath;
                objprful.FinalAttendanceListUploaded = su.FinalAttendanceListUploaded;




                objprful.LastUpdated = DateTime.Now;
                Entities.ProgramRequestFileUploads.Add(objprful);
                Entities.SaveChanges();
                //PatientID = objTempMAF.PatientID;
            }
            else
            {
                //update patient
                if (su.EvaluationUploaded ?? false)
                {
                    session_upload.ProgramRequestID = su.ProgramRequestID;
                    session_upload.EvaluationFullPath = su.EvaluationFullPath;
                    session_upload.EvaluationUploaded = su.EvaluationUploaded;
                    session_upload.EvaluationFileName = su.EvaluationFileName;
                    session_upload.EvaluationFileExt = su.EvaluationFileExt;
                }
                //signin sheet
                if (su.SignInUploaded ?? false)
                {
                    session_upload.SignInFullPath = su.SignInFullPath;
                    session_upload.SignInUploaded = su.SignInUploaded;
                    session_upload.SignInFileName = su.SignInFileName;
                    session_upload.SignInFileExt = su.SignInFileExt;
                }
                //userother
                if (su.UserOtherUploaded ?? false)
                {
                    session_upload.UserOtherFullPath = su.UserOtherFullPath;
                    session_upload.UserOtherUploaded = su.UserOtherUploaded;
                    session_upload.UserOtherFileName = su.UserOtherFileName;
                    session_upload.UserOtherFileExt = su.UserOtherFileExt;
                }


                if (su.SpeakerAgreementUploaded ?? false)
                {

                    session_upload.SpeakerAgreementFileExt = su.SpeakerAgreementFileExt;
                    session_upload.SpeakerAgreementFileName = su.SpeakerAgreementFileName;
                    session_upload.SpeakerAgreementFullPath = su.SpeakerAgreementFullPath;
                    session_upload.SpeakerAgreementUploaded = su.SpeakerAgreementUploaded;
                }
                if (su.FinalAttendanceListUploaded ?? false)
                {

                    session_upload.FinalAttendanceListFileExt = su.FinalAttendanceListFileExt;
                    session_upload.FinalAttendanceListFileName = su.FinalAttendanceListFileName;
                    session_upload.FinalAttendanceListFullPath = su.FinalAttendanceListFullPath;
                    session_upload.FinalAttendanceListUploaded = su.FinalAttendanceListUploaded;
                }

                session_upload.LastUpdated = DateTime.Now;



                Entities.SaveChanges();

            }
        }


        public ProgramSessionPayment GetProgramSessionPayment(int ProgramRequestID)
        {//usertype 2 for speaker
            //usertype 3 for moderator
            try
            {
                if (ProgramRequestID != 0)
                {
                    ProgramSessionPayment psu;
                    psu = Entities.ProgramRequests.Where(x => x.ProgramRequestID == ProgramRequestID).Select(pr =>
                             new ProgramSessionPayment
                             {

                                 ProgramRequestID = pr.ProgramRequestID,
                                 ProgramSpeakerID = pr.ProgramSpeakerID ?? 0,
                                 ProgramSpeaker2ID = pr.ProgramSpeaker2ID ?? 0,
                                 ProgramModeratorID = pr.ProgramModeratorID ?? 0,
                                 SpeakerFirstName = pr.SpeakerInfo.FirstName,
                                 SpeakerLastName = pr.SpeakerInfo.LastName,
                                 Speaker2FirstName = pr.Speaker2Info.FirstName,
                                 Speaker2LastName = pr.Speaker2Info.LastName,
                                 ModeratorFirstName = pr.ModeratorInfo.FirstName,
                                 ModeratorLastName = pr.ModeratorInfo.LastName,
                                 SpeakerPaymentAmount = pr.AdminSpeakerHonorium.ToString(),
                                 SpeakerPaymentSentDate = (pr.AdminSpeakerPaymentSentDate == null) ? null : SqlFunctions.DateName("year", pr.AdminSpeakerPaymentSentDate) + "/" + SqlFunctions.DatePart("m", pr.AdminSpeakerPaymentSentDate) + "/" + SqlFunctions.DateName("day", pr.AdminSpeakerPaymentSentDate),
                                 Speaker2PaymentAmount = pr.AdminSpeaker2Honorium.ToString(),
                                 Speaker2PaymentSentDate = (pr.AdminSpeaker2PaymentSentDate == null) ? null : SqlFunctions.DateName("year", pr.AdminSpeaker2PaymentSentDate) + "/" + SqlFunctions.DatePart("m", pr.AdminSpeaker2PaymentSentDate) + "/" + SqlFunctions.DateName("day", pr.AdminSpeaker2PaymentSentDate),
                                 ModeratorPaymentAmount = pr.AdminModeratorHonorium.ToString(),
                                 ModeratorPaymentSentDate = (pr.AdminModeratorPaymentSentDate == null) ? null : SqlFunctions.DateName("year", pr.AdminModeratorPaymentSentDate) + "/" + SqlFunctions.DatePart("m", pr.AdminModeratorPaymentSentDate) + "/" + SqlFunctions.DateName("day", pr.AdminModeratorPaymentSentDate),
                                 VenueFees = pr.AdminVenueFees.ToString(),
                                 OtherFees = pr.AdminOtherFees.ToString(),
                                 AVFees = pr.AdminAVFees.ToString(),




                             }).SingleOrDefault();

                    return psu;
                }
                else
                    return null;
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
                return null;
            }

        }

        public ProgramRequest GetProgramRequest(int ProgramRequestID)
        {
            return Entities.ProgramRequests.Where(x => x.ProgramRequestID == ProgramRequestID).FirstOrDefault();
        }
    }
}
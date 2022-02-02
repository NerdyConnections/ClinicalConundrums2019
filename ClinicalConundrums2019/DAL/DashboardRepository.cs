using ClinicalConundrums2019.Models;
using ClinicalConundrums2019.Util;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web;

namespace ClinicalConundrums2019.DAL
{
    public class DashboardRepository : BaseRepository
    {
        public List<DashboardItem> GetAllDashboardItems(int UserID, int ProgramID)
        {
            List<DashboardItem> DashboardItems = null;
            string UserRole = UserHelper.GetRoleByUserID(UserID);
            //1, Sales Representative
            //2, Speaker
            //3, Moderator
            //4, Regional Manager
            //5, Brand Manager -- see all pfizer
            //6 CL Coordinator -- see all pfizer
            //7 Admin
            //8 CHE Manager - - everything in both bms and pifzer
            //9 Director - see everything in bms
            //10 National CME Lead -see everything in bms
            //11 National Sales Lead- see everything in bms
            int SponsorID = UserHelper.GetLoggedInUser().SponsorID;
            //SuperUser see every sessions for a program under BMS or Pfizer Clinicname no need to query for territoryID
            if (UserRole == Util.Constants.CHEManager)
            {


                DashboardItems = (from pr in Entities.ProgramRequests
                                  where pr.ProgramID == ProgramID
                                  // display all requests for now where pr.RequestStatus == 2 || pr.RequestStatus == 3  //show only submitted requests (either approved 3 or waiting to be approved  2 in the admin tool)
                                  orderby pr.SubmittedDate descending
                                  select new DashboardItem()
                                  {
                                      ProgramRequestID = pr.ProgramRequestID,
                                      ProgramID = pr.ProgramID ?? 0,
                                      UserID = UserID,
                                      RequestDate = (pr.SubmittedDate == null) ? null : SqlFunctions.DateName("year", pr.SubmittedDate) + "/" + SqlFunctions.DatePart("m", pr.SubmittedDate) + "/" + SqlFunctions.DateName("day", pr.SubmittedDate),
                                      FullSessionDetails = pr.ReadOnly ?? false, //if true eye image, if false pencil

                                      SessionStatus = pr.RequestStatusLookup.RequestStatusDescription,
                                      FinalAttendance = pr.AdminFinalAttendance == null ? null : pr.AdminFinalAttendance.ToString(),
                                      SessionStatusID = pr.RequestStatus ?? 0,
                                      SessionDate = (pr.ConfirmedSessionDate == null) ? null : SqlFunctions.DateName("year", pr.ConfirmedSessionDate) + "/" + SqlFunctions.DatePart("m", pr.ConfirmedSessionDate) + "/" + SqlFunctions.DateName("day", pr.ConfirmedSessionDate),
                                      Speaker = pr.SpeakerInfo.FirstName + "," + pr.SpeakerInfo.LastName,
                                      Moderator = (!String.IsNullOrEmpty(pr.ModeratorInfo.FirstName)) ? pr.ModeratorInfo.FirstName + "," + pr.ModeratorInfo.LastName : "",
                                      Speaker2 = (!String.IsNullOrEmpty(pr.Speaker2Info.FirstName)) ? pr.Speaker2Info.FirstName + "," + pr.Speaker2Info.LastName : "",

                                      Speaker1ProgramDateNA = pr.SpeakerProgramDateNA,
                                      Speaker1Declined = pr.SpeakerDeclined ?? false,
                                      Speaker2ProgramDateNA = pr.Speaker2ProgramDateNA,
                                      Speaker2Declined = pr.Speaker2Declined ?? false,
                                      //  SpeakerProgramDateNA = (Speaker1ProgramDateNA || Speaker2ProgramDateNA),
                                      ModeratorProgramDateNA = pr.ModeratorProgramDateNA,



                                      ModeratorDeclined = pr.ModeratorDeclined ?? false,
                                      VenueAvailable = pr.AdminVenueAvailable,

                                      Location = pr.LocationName,
                                      ReadOnly = pr.ReadOnly ?? false,
                                      WebinarEventUrl=pr.WebinarEventURL,
                                      EventType = pr.EventType


                                  }).ToList();

            }
            else if (UserRole == Util.Constants.NationalCMELead || UserRole == Util.Constants.NationalSalesLead || UserRole == Util.Constants.Director)
            {//BMSHead see all event requests by BMS clinic name
                var BMSUserIDs = (from ut in Entities.UserInfoes where ut.ClinicName == Constants.BMS select ut.UserID).ToList();
                DashboardItems = (from pr in Entities.ProgramRequests
                                  where pr.ProgramID == ProgramID && BMSUserIDs.Contains(pr.UserID)
                                  // display all requests for now where pr.RequestStatus == 2 || pr.RequestStatus == 3  //show only submitted requests (either approved 3 or waiting to be approved  2 in the admin tool)
                                  orderby pr.SubmittedDate descending
                                  select new DashboardItem()
                                  {
                                      ProgramRequestID = pr.ProgramRequestID,
                                      ProgramID = pr.ProgramID ?? 0,
                                      UserID = UserID,
                                      RequestDate = (pr.SubmittedDate == null) ? null : SqlFunctions.DateName("year", pr.SubmittedDate) + "/" + SqlFunctions.DatePart("m", pr.SubmittedDate) + "/" + SqlFunctions.DateName("day", pr.SubmittedDate),
                                      FullSessionDetails = pr.ReadOnly ?? false, //if true eye image, if false pencil
                                      SessionStatusID = pr.RequestStatus ?? 0,
                                      SessionStatus = pr.RequestStatusLookup.RequestStatusDescription,
                                      FinalAttendance = pr.AdminFinalAttendance == null ? null : pr.AdminFinalAttendance.ToString(),
                                      SessionDate = (pr.ConfirmedSessionDate == null) ? null : SqlFunctions.DateName("year", pr.ConfirmedSessionDate) + "/" + SqlFunctions.DatePart("m", pr.ConfirmedSessionDate) + "/" + SqlFunctions.DateName("day", pr.ConfirmedSessionDate),
                                      Speaker = pr.SpeakerInfo.FirstName + "," + pr.SpeakerInfo.LastName,
                                      Moderator = (!String.IsNullOrEmpty(pr.ModeratorInfo.FirstName)) ? pr.ModeratorInfo.FirstName + "," + pr.ModeratorInfo.LastName : "",
                                      Speaker2 = (!String.IsNullOrEmpty(pr.Speaker2Info.FirstName)) ? pr.Speaker2Info.FirstName + "," + pr.Speaker2Info.LastName : "",



                                      Location = pr.LocationName,

                                      Speaker1ProgramDateNA = pr.SpeakerProgramDateNA,
                                      Speaker1Declined = pr.SpeakerDeclined ?? false,
                                      Speaker2ProgramDateNA = pr.Speaker2ProgramDateNA,
                                      Speaker2Declined = pr.Speaker2Declined ?? false,
                                      //  SpeakerProgramDateNA = (Speaker1ProgramDateNA || Speaker2ProgramDateNA),
                                      ModeratorProgramDateNA = pr.ModeratorProgramDateNA,



                                      ModeratorDeclined = pr.ModeratorDeclined ?? false,
                                      VenueAvailable = pr.AdminVenueAvailable,

                                      ReadOnly = pr.ReadOnly ?? false,
                                      WebinarEventUrl = pr.WebinarEventURL,
                                      EventType = pr.EventType




                                  }).ToList();
            }
            else if (UserRole == Util.Constants.BrandManager || UserRole == Util.Constants.CLCoordinator)
            {//PfizerHead see all event requests by Pfizer clinic name
                var PFIZERUserIDs = (from ut in Entities.UserInfoes where ut.ClinicName == Constants.PFIZER select ut.UserID).ToList();
                DashboardItems = (from pr in Entities.ProgramRequests
                                  where pr.ProgramID == ProgramID && PFIZERUserIDs.Contains(pr.UserID)
                                  // display all requests for now where pr.RequestStatus == 2 || pr.RequestStatus == 3  //show only submitted requests (either approved 3 or waiting to be approved  2 in the admin tool)
                                  orderby pr.SubmittedDate descending
                                  select new DashboardItem()
                                  {
                                      ProgramRequestID = pr.ProgramRequestID,
                                      ProgramID = pr.ProgramID ?? 0,
                                      UserID = UserID,
                                      RequestDate = (pr.SubmittedDate == null) ? null : SqlFunctions.DateName("year", pr.SubmittedDate) + "/" + SqlFunctions.DatePart("m", pr.SubmittedDate) + "/" + SqlFunctions.DateName("day", pr.SubmittedDate),
                                      FullSessionDetails = pr.ReadOnly ?? false, //if true eye image, if false pencil
                                      SessionStatusID = pr.RequestStatus ?? 0,
                                      SessionStatus = pr.RequestStatusLookup.RequestStatusDescription,
                                      FinalAttendance = pr.AdminFinalAttendance == null ? null : pr.AdminFinalAttendance.ToString(),
                                      SessionDate = (pr.ConfirmedSessionDate == null) ? null : SqlFunctions.DateName("year", pr.ConfirmedSessionDate) + "/" + SqlFunctions.DatePart("m", pr.ConfirmedSessionDate) + "/" + SqlFunctions.DateName("day", pr.ConfirmedSessionDate),
                                      Speaker = pr.SpeakerInfo.FirstName + "," + pr.SpeakerInfo.LastName,
                                      Moderator = (!String.IsNullOrEmpty(pr.ModeratorInfo.FirstName)) ? pr.ModeratorInfo.FirstName + "," + pr.ModeratorInfo.LastName : "",
                                      Speaker2 = (!String.IsNullOrEmpty(pr.Speaker2Info.FirstName)) ? pr.Speaker2Info.FirstName + "," + pr.Speaker2Info.LastName : "",

                                      Location = pr.LocationName,

                                      Speaker1ProgramDateNA = pr.SpeakerProgramDateNA,
                                      Speaker1Declined = pr.SpeakerDeclined ?? false,
                                      Speaker2ProgramDateNA = pr.Speaker2ProgramDateNA,
                                      Speaker2Declined = pr.Speaker2Declined ?? false,
                                      //  SpeakerProgramDateNA = (Speaker1ProgramDateNA || Speaker2ProgramDateNA),
                                      ModeratorProgramDateNA = pr.ModeratorProgramDateNA,



                                      ModeratorDeclined = pr.ModeratorDeclined ?? false,
                                      VenueAvailable = pr.AdminVenueAvailable,

                                      ReadOnly = pr.ReadOnly ?? false,
                                      WebinarEventUrl = pr.WebinarEventURL,
                                      EventType = pr.EventType





                                  }).ToList();
            }
            //see all users under the manager's territoryID
            else if (UserRole == Util.Constants.RegionalManager)
            {  //need to retrieve all users with the same territoryID and get all dashboard items with the same territoryID

                //the manager's territoryID ie. 41
                var TerritoryID = (from ut in Entities.UserInfoes where ut.UserID == UserID select ut.TerritoryID).FirstOrDefault();
                //all userids under territoryID 41
                var TerritorialUserIDs = (from ut in Entities.UserInfoes where ut.TerritoryID == TerritoryID select ut.UserID).ToList();
                //dashboarditems of all userids belonging to territory 41
                DashboardItems = (from pr in Entities.ProgramRequests
                                  where pr.ProgramID == ProgramID && TerritorialUserIDs.Contains(pr.UserID)
                                  // display all requests for now where pr.RequestStatus == 2 || pr.RequestStatus == 3  //show only submitted requests (either approved 3 or waiting to be approved  2 in the admin tool)
                                  orderby pr.SubmittedDate descending
                                  select new DashboardItem()
                                  {
                                      ProgramRequestID = pr.ProgramRequestID,
                                      ProgramID = pr.ProgramID ?? 0,
                                      UserID = UserID,
                                      RequestDate = (pr.SubmittedDate == null) ? null : SqlFunctions.DateName("year", pr.SubmittedDate) + "/" + SqlFunctions.DatePart("m", pr.SubmittedDate) + "/" + SqlFunctions.DateName("day", pr.SubmittedDate),
                                      FullSessionDetails = pr.ReadOnly ?? false, //if true eye image, if false pencil
                                      SessionStatusID = pr.RequestStatus ?? 0,
                                      SessionStatus = pr.RequestStatusLookup.RequestStatusDescription,
                                      FinalAttendance = pr.AdminFinalAttendance == null ? null : pr.AdminFinalAttendance.ToString(),
                                      SessionDate = (pr.ConfirmedSessionDate == null) ? null : SqlFunctions.DateName("year", pr.ConfirmedSessionDate) + "/" + SqlFunctions.DatePart("m", pr.ConfirmedSessionDate) + "/" + SqlFunctions.DateName("day", pr.ConfirmedSessionDate),
                                      Speaker = pr.SpeakerInfo.FirstName + "," + pr.SpeakerInfo.LastName,
                                      Moderator = (!String.IsNullOrEmpty(pr.ModeratorInfo.FirstName)) ? pr.ModeratorInfo.FirstName + "," + pr.ModeratorInfo.LastName : "",
                                      Speaker2 = (!String.IsNullOrEmpty(pr.Speaker2Info.FirstName)) ? pr.Speaker2Info.FirstName + "," + pr.Speaker2Info.LastName : "",


                                      Location = pr.LocationName,

                                      Speaker1ProgramDateNA = pr.SpeakerProgramDateNA,
                                      Speaker1Declined = pr.SpeakerDeclined ?? false,
                                      Speaker2ProgramDateNA = pr.Speaker2ProgramDateNA,
                                      Speaker2Declined = pr.Speaker2Declined ?? false,
                                      //  SpeakerProgramDateNA = (Speaker1ProgramDateNA || Speaker2ProgramDateNA),
                                      ModeratorProgramDateNA = pr.ModeratorProgramDateNA,



                                      ModeratorDeclined = pr.ModeratorDeclined ?? false,
                                      VenueAvailable = pr.AdminVenueAvailable,

                                      ReadOnly = pr.ReadOnly ?? false,
                                      WebinarEventUrl = pr.WebinarEventURL,
                                      EventType = pr.EventType





                                  }).ToList();
            }
            else //sale rep only get to see he own session request
            {
                DashboardItems = (from pr in Entities.ProgramRequests
                                  where pr.ProgramID == ProgramID && pr.UserID == UserID
                                  // display all requests for now where pr.RequestStatus == 2 || pr.RequestStatus == 3  //show only submitted requests (either approved 3 or waiting to be approved  2 in the admin tool)
                                  orderby pr.SubmittedDate descending
                                  select new DashboardItem()
                                  {
                                      ProgramRequestID = pr.ProgramRequestID,
                                      ProgramID = pr.ProgramID ?? 0,
                                      UserID = UserID,
                                      RequestDate = (pr.SubmittedDate == null) ? null : SqlFunctions.DateName("year", pr.SubmittedDate) + "/" + SqlFunctions.DatePart("m", pr.SubmittedDate) + "/" + SqlFunctions.DateName("day", pr.SubmittedDate),
                                      FullSessionDetails = pr.ReadOnly ?? false, //if true eye image, if false pencil
                                      SessionStatusID = pr.RequestStatus ?? 0,
                                      SessionStatus = pr.RequestStatusLookup.RequestStatusDescription,
                                      FinalAttendance = pr.AdminFinalAttendance == null ? null : pr.AdminFinalAttendance.ToString(),
                                      SessionDate = (pr.ConfirmedSessionDate == null) ? null : SqlFunctions.DateName("year", pr.ConfirmedSessionDate) + "/" + SqlFunctions.DatePart("m", pr.ConfirmedSessionDate) + "/" + SqlFunctions.DateName("day", pr.ConfirmedSessionDate),
                                      Speaker = pr.SpeakerInfo.FirstName + "," + pr.SpeakerInfo.LastName,
                                      Moderator = (!String.IsNullOrEmpty(pr.ModeratorInfo.FirstName)) ? pr.ModeratorInfo.FirstName + "," + pr.ModeratorInfo.LastName : "",
                                      Speaker2 = (!String.IsNullOrEmpty(pr.Speaker2Info.FirstName)) ? pr.Speaker2Info.FirstName + "," + pr.Speaker2Info.LastName : "",
                                      Location = pr.LocationName,
                                      Speaker1ProgramDateNA = pr.SpeakerProgramDateNA,
                                      Speaker1Declined = pr.SpeakerDeclined ?? false,
                                      Speaker2ProgramDateNA = pr.Speaker2ProgramDateNA,
                                      Speaker2Declined = pr.Speaker2Declined ?? false,
                                      //  SpeakerProgramDateNA = (Speaker1ProgramDateNA || Speaker2ProgramDateNA),
                                      ModeratorProgramDateNA = pr.ModeratorProgramDateNA,



                                      ModeratorDeclined = pr.ModeratorDeclined ?? false,
                                      VenueAvailable = pr.AdminVenueAvailable,
                                      ReadOnly = pr.ReadOnly ?? false,
                                      WebinarEventUrl = pr.WebinarEventURL,
                                      EventType = pr.EventType




                                  }).ToList();

            }
            //setting MyActionItems
            foreach (DashboardItem item in DashboardItems)
            {
                int BusinessDays = 0;
                string Moderator, Speaker2;
                //if moderator does not present return nothing else return a slash and the name
                Moderator = string.IsNullOrEmpty(item.Moderator) ? "" : " / " + item.Moderator;
                Speaker2 = string.IsNullOrEmpty(item.Speaker2) ? "" : " / " + item.Speaker2;
                item.SessionInfo = (item.EventType=="InPerson") ? "Live Event" : "Webinar" + "<br/>" +  item.SessionDate + "<br/>" + item.Speaker + item.Speaker2 + Moderator + "<br/>" + item.Location;

                //calcuate if the session date is at least 2 business days more than today's date

                if (item.SessionDate != null)
                {


                    BusinessDays = BusinessDaysCalc.GetWorkingdays(DateTime.Now, Convert.ToDateTime(item.SessionDate));
                    //cannot cancell if business day less 2 or the session is completed
                    if (BusinessDays >= 2 && item.SessionStatusID != 4 && item.SessionStatusID != 5)
                    {
                        item.CanSessionBeCancelled = true;
                    }
                    else
                    {
                        item.CanSessionBeCancelled = false;
                    }
                }
                else
                {
                    //once event completed (4- Completed – Session Closed , 5-Completed – Items Pending)  you cannot cancel
                    if (item.SessionStatusID != 4 && item.SessionStatusID != 5)

                        item.CanSessionBeCancelled = true;
                    else
                        item.CanSessionBeCancelled = false;

                }
                Console.WriteLine("BusinessDays:" + BusinessDays);
                switch (item.SessionStatusID)
                {
                    case 1://under review
                        item.MyActionItems = Constants.NA;
                        break;
                    case 2://Active – Regional Ethics Review Pending 
                        item.MyActionItems = Constants.NA;
                        break;
                    case 3://Active – Regional Ethics Approved 
                        item.MyActionItems = Constants.NA;
                        break;
                    case 5://Completed – Items Pending 
                        item.MyActionItems = Constants.SubmitPostSessionMaterials + "<br/>";
                        break;
                    case 4://Completed – Session Closed 
                        item.MyActionItems = Constants.NA;
                        break;
                    case 6://Completed – Session Cancelled 
                        item.MyActionItems = Constants.NA;
                        break;


                }
                item.SpeakerProgramDateNA = false;
                //if either speaker is not available set the speaker not available flag is true;
                if (item.Speaker1ProgramDateNA ?? false)
                    item.SpeakerProgramDateNA = true;
                if (item.Speaker2ProgramDateNA ?? false)
                    item.Speaker2ProgramDateNA = true;

                //if either speaker is not available set the speaker decline flag is true;
                if (item.Speaker1Declined ?? false)
                    item.SpeakerDeclined = true;
                if (item.Speaker2Declined ?? false)
                    item.Speaker2Declined = true;

                //Overriding default ActionItems
                string ExistingActionItems = string.Empty;
                // if the following scenarios occur we will be override the default action items
                if (item.SpeakerProgramDateNA ?? false)//when speak select NA in email, it will set the Readonly to false in the programrequestID which change Session Status to Changes Required
                {

                    ExistingActionItems = ExistingActionItems + Constants.SpeakerNA + "<br/>";
                    item.MyActionItems = ExistingActionItems;
                }
                // if the following scenarios occur we will be override the default action items
                if (item.Speaker2ProgramDateNA ?? false)//when speak2 select NA in email, it will set the Readonly to false in the programrequestID which change Session Status to Changes Required
                {

                    ExistingActionItems = ExistingActionItems + Constants.Speaker2NA + "<br/>";
                    item.MyActionItems = ExistingActionItems;
                }
                if (item.ModeratorProgramDateNA ?? false)//when moderator select NA in email, it will set the Readonly to false in the programrequestID which change Session Status to Changes Required
                {

                    ExistingActionItems = ExistingActionItems + Constants.ModeratorNA + "<br/>";
                    item.MyActionItems = ExistingActionItems;
                }
                if (item.SpeakerDeclined ?? false)//when speak select NA in email, it will set the Readonly to false in the programrequestID which change Session Status to Changes Required
                {

                    ExistingActionItems = ExistingActionItems + " " + Constants.SpeakerDeclined + "<br/>";
                    item.MyActionItems = ExistingActionItems;
                }
                if (item.Speaker2Declined ?? false)//when speak select NA in email, it will set the Readonly to false in the programrequestID which change Session Status to Changes Required
                {

                    ExistingActionItems = ExistingActionItems + " " + Constants.Speaker2Declined + "<br/>";
                    item.MyActionItems = ExistingActionItems;
                }
                if (item.ModeratorDeclined ?? false)//when speak select NA in email, it will set the Readonly to false in the programrequestID which change Session Status to Changes Required
                {

                    ExistingActionItems = ExistingActionItems + " " + Constants.ModeratorDeclined + "<br/>";
                    item.MyActionItems = ExistingActionItems;
                }
                if (item.VenueAvailable == "N")//when chrc admin select No to Is Venue Available (under Manage Program Request)
                {

                    ExistingActionItems = ExistingActionItems + " " + ExistingActionItems + Constants.VenueNA + "<br/>";
                    item.MyActionItems = ExistingActionItems;
                }

                item.FinalAttendanceListUploaded = IsFinalAttendanceListUploaded(item.ProgramRequestID);
                item.FinalAttendanceListFileExt = GetFinalAttendanceListFileExt(item.ProgramRequestID);

                item.SignInUploaded = IsSignInUploaded(item.ProgramRequestID);
                item.SignInFileExt = GetSignInFileExt(item.ProgramRequestID);

                item.EvaluationUploaded = IsEvaluationUploaded(item.ProgramRequestID);
                item.EvaluationFileExt = GetEvaluationFileExt(item.ProgramRequestID);

            }


            return DashboardItems;
        }


        public bool IsFinalAttendanceListUploaded(int ProgramRequestID)
        {
          var  prfu = Entities.ProgramRequestFileUploads.Where(x => x.ProgramRequestID == ProgramRequestID).SingleOrDefault();
            if (prfu != null)
            {
                return prfu.FinalAttendanceListUploaded ?? false;
            }else
            {

                return false;
            }

        }
        public bool IsSignInUploaded(int ProgramRequestID)
        {
            var prfu = Entities.ProgramRequestFileUploads.Where(x => x.ProgramRequestID == ProgramRequestID).SingleOrDefault();
            if (prfu != null)
            {
                return prfu.SignInUploaded ?? false;
            }
            else
            {

                return false;
            }

        }
        public bool IsEvaluationUploaded(int ProgramRequestID)
        {
            var prfu = Entities.ProgramRequestFileUploads.Where(x => x.ProgramRequestID == ProgramRequestID).SingleOrDefault();
            if (prfu != null)
            {
                return prfu.EvaluationUploaded ?? false;
            }
            else
            {

                return false;
            }

        }
        public string GetFinalAttendanceListFileExt(int ProgramRequestID)
        {
            var prfu = Entities.ProgramRequestFileUploads.Where(x => x.ProgramRequestID == ProgramRequestID).SingleOrDefault();
            if (prfu != null)
            {
                return prfu.FinalAttendanceListFileExt ;
            }
            else
            {

                return string.Empty;
            }

        }
        public string GetEvaluationFileExt(int ProgramRequestID)
        {
            var prfu = Entities.ProgramRequestFileUploads.Where(x => x.ProgramRequestID == ProgramRequestID).SingleOrDefault();
            if (prfu != null)
            {
                return prfu.EvaluationFileExt;
            }
            else
            {

                return string.Empty;
            }

        }
        public string GetSignInFileExt(int ProgramRequestID)
        {
            var prfu = Entities.ProgramRequestFileUploads.Where(x => x.ProgramRequestID == ProgramRequestID).SingleOrDefault();
            if (prfu != null)
            {
                return prfu.SignInFileExt;
            }
            else
            {

                return string.Empty;
            }

        }
        public EvalFormModel GetEvalFormModel(int ProgramRequestID)
        {
            EvalFormModel efm;
            List<ProgramRequestSessionCredits> SessionList = new List<ProgramRequestSessionCredits>();
            ProgramRepository progRepo = new ProgramRepository();

            SessionList = progRepo.GetSessionCredits(ProgramRequestID);

            ProgramRepository prepo = new ProgramRepository();

            efm = Entities.ProgramRequests.Where(x => x.ProgramRequestID == ProgramRequestID).Select(ui =>
                     new EvalFormModel
                     {
                         EventRequestID = ProgramRequestID,
                         ProgramDate = ui.ConfirmedSessionDate.ToString(),
                         ProgramLocation = ui.LocationName,
                         Speaker1 = ui.SpeakerInfo.FirstName + ", " + ui.SpeakerInfo.LastName,
                         Speaker2 = ui.Speaker2Info.FirstName + ", " + ui.Speaker2Info.LastName,
                         Moderator = ui.ModeratorInfo.FirstName + ", " + ui.ModeratorInfo.LastName





                     }).SingleOrDefault();


            foreach (var item in SessionList)
            {
                if (item.id == 1)
                {

                    efm.SessionCredit1 = true;
                }

                if (item.id == 2)
                {

                    efm.SessionCredit2 = true;
                }

                if (item.id == 3)
                {

                    efm.SessionCredit3 = true;
                }
                if (item.id == 4)
                {

                    efm.SessionCredit4 = true;
                }
                if (item.id == 5)
                {

                    efm.SessionCredit5 = true;
                }
                if (item.id == 6)
                {

                    efm.SessionCredit6 = true;
                }
                if (item.id == 7)
                {

                    efm.SessionCredit7 = true;
                }
                if (item.id == 8)
                {

                    efm.SessionCredit8 = true;
                }
                if (item.id == 9)
                {

                    efm.SessionCredit9 = true;
                }
                if (item.id == 10)
                {

                    efm.SessionCredit10 = true;
                }
                if (item.id == 11)
                {

                    efm.SessionCredit11 = true;
                }
                if (item.id == 12)
                {

                    efm.SessionCredit12 = true;
                }


            }

            return efm;





        }
    }
}
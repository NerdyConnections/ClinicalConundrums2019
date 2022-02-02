using ClinicalConundrum2019.Data;
using ClinicalConundrumsSpeaker.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Web;

namespace ClinicalConundrumsSpeaker.DAL
{
    public class UserRepository : BaseRepository
    {
        public UserInfo GetUserInfoByEmail(string email)
        {
            return Entities.UserInfoes.Where(x => x.EmailAddress == email).FirstOrDefault();
        }

        public User GetUserByEmail(string email)
        {
            return Entities.Users.Where(x => x.EmailAddress == email).FirstOrDefault();
        }


        public int GetUserIdFromEmail(string Email)
        {

            var userId = Entities.UserInfoes.Where(x => x.EmailAddress == Email).Select(x => x.id).FirstOrDefault();

            return userId;

        }
        public bool IsActivated(string userName, string password)
        {


            // return Entities.Users.Count(i => i.Username == userName && i.Password == password) == 1;
            var user = Entities.Users.Where(x => x.UserName == userName && x.Password == password).SingleOrDefault();
            if (user != null)
            {//if username and password exist but the delete flag is turned on, then the user is no longer active and should not be authenticated
                if (user.IsActive)
                {
                    return true;
                }
                else
                {

                    return false;
                }
            }
            return false;

        }
        public string GetRoleByUserID(int UserID)
        {
            var userRole = Entities.UserInfoes.Where(x => x.UserID == UserID).Select(x => x.UserTypeLookUp.Description).SingleOrDefault();
            return userRole;


        }
        public bool Authenticate(string userName, string password)
        {


            // return Entities.Users.Count(i => i.Username == userName && i.Password == password) == 1;
            var user = Entities.Users.Where(x => x.UserName == userName && x.Password == password).SingleOrDefault();
            if (user != null)
            {//if username and password exist but the delete flag is turned on, then the user is no longer active and should not be authenticated
                if (user.IsDeleted)
                {
                    return false;
                }
                else
                {

                    return true;
                }
            }
            else
                return false;

        }
        public bool AuthenticateSpeaker(string userName, string password)
        {


            // return Entities.Users.Count(i => i.Username == userName && i.Password == password) == 1;
            var user = Entities.Users.Where(x => x.UserName == userName && x.Password == password).SingleOrDefault();
            if (user != null && (user.UserInfoes.FirstOrDefault().UserType == 2 || user.UserInfoes.FirstOrDefault().UserType == 3))
            {//if username and password exist but the delete flag is turned on, then the user is no longer active and should not be authenticated
                if (user.IsDeleted)
                {
                    return false;
                }
                else
                {

                    return true;
                }
            }
            else
                return false;

        }
        public string[] GetRolesAsArray(string userName)
        {
            return Entities.Users.First(u => u.UserName == userName).Roles.Select(r => r.Name).ToArray();

        }
        public string GetRoles(string userName)
        {
            string retStr = string.Empty;

            StringBuilder sb = new StringBuilder();

            foreach (string str in Entities.Users.First(u => u.UserName == userName).Roles.Select(r => r.Name).ToList())
            {
                sb.Append(str).Append("|");

            }

            if (sb.Length > 0)
                retStr = sb.ToString().TrimEnd("|".ToCharArray());

            return retStr;
        }

        public void EditSpeaker(UserModel um)
        {
            var objSpeaker = Entities.UserInfoes.Where(x => x.UserID == um.UserID).SingleOrDefault();
            if (objSpeaker != null)
            {

                objSpeaker.FirstName = um.FirstName;
                objSpeaker.LastName = um.LastName;
                objSpeaker.EmailAddress = um.EmailAddress;
                objSpeaker.Address = um.Address;
                objSpeaker.City = um.City;
                //  objSpeaker.Province = um.Province;
                objSpeaker.PostalCode = um.PostalCode;
                objSpeaker.Phone = um.Phone;
                objSpeaker.Fax = um.Fax;
                objSpeaker.SpeakerHonariumRange = um.HonariumRange;

                Entities.SaveChanges();


            }

        }
        public UserModel GetUserByUserID(int UserID)
        {
            UserModel um = null;

            um = Entities.UserInfoes.Where(x => x.UserID == UserID).Select(ui =>
                     new UserModel
                     {
                         UserID = UserID,
                         FirstName = ui.FirstName,
                         LastName = ui.LastName,
                         EmailAddress = ui.EmailAddress,
                         ClinicName = ui.ClinicName,
                         Address = ui.Address,
                         Address2 = ui.Address2,
                         City = ui.City,
                          //Province = ui.Province,
                          PostalCode = ui.PostalCode,
                         Phone = ui.Phone,
                         Fax = ui.Fax,
                         Specialty = ui.Specialty,
                         HonariumRange = ui.SpeakerHonariumRange

                     }).SingleOrDefault();

            return um;

        }
        public UserModel GetUserByuserid(int userid)
        {
            UserModel um = null;

            um = Entities.UserInfoes.Where(x => x.id == userid).Select(ui =>
                     new UserModel
                     {
                         ID = ui.id,
                         UserIDRequestedBy = ui.UserIDRequestedBy ?? 0,
                         FirstName = ui.FirstName,
                         LastName = ui.LastName,
                         EmailAddress = ui.EmailAddress,
                         ClinicName = ui.ClinicName,
                         Address = ui.Address,
                         Address2 = ui.Address2,
                         City = ui.City,
                         //Province = ui.Province,
                         PostalCode = ui.PostalCode,
                         Phone = ui.Phone,
                         Fax = ui.Fax,
                         Specialty = ui.Specialty,
                         HonariumRange = ui.SpeakerHonariumRange

                     }).SingleOrDefault();

            return um;

        }
        public List<UserModel> GetAllUsers()
        {
            List<UserModel> userList = null;
            userList = (from ui in Entities.UserInfoes
                        select new UserModel()
                        {
                            UserID = ui.UserID ?? 0,
                            FirstName = ui.FirstName,
                            LastName = ui.LastName,
                            EmailAddress = ui.EmailAddress,
                            ClinicName = ui.ClinicName,
                            Address = ui.Address,
                            Address2 = ui.Address2,
                            City = ui.City,
                            //Province = ui.Province,
                            PostalCode = ui.PostalCode,
                            Phone = ui.Phone,
                            Fax = ui.Fax,
                            Specialty = ui.Specialty,
                            HonariumRange = ui.SpeakerHonariumRange


                        }).ToList();
            return userList;


        }


        public List<UserModel> GetAllSpeakers()//it actually retrieve both speakers and moderators
        {
            List<UserModel> userList = null;
            userList = (from ui in Entities.UserInfoes
                        where ui.UserType == 2 || ui.UserType == 3 //speakers: usertype=2  moderator: usertype=3
                        orderby ui.Status, ui.LastName  //the un approved should be top of the grid
                        select new UserModel()
                        {
                            ID = ui.id,
                            UserID = ui.UserID ?? 0,
                            UserIDRequestedBy = ui.UserIDRequestedBy ?? 0,
                            UserType = ui.UserTypeLookUp.Description,
                            FirstName = ui.FirstName,
                            LastName = ui.LastName,
                            EmailAddress = ui.EmailAddress,
                            ClinicName = ui.ClinicName,
                            Address = ui.Address,
                            Address2 = ui.Address2,
                            City = ui.City,
                            //Province = ui.Province,
                            PostalCode = ui.PostalCode,
                            Phone = ui.Phone,
                            Fax = ui.Fax,
                            Specialty = ui.Specialty,
                            HonariumRange = ui.SpeakerHonariumRange,
                            SubmittedDate = (ui.SubmittedDate == null) ? null : SqlFunctions.DateName("year", ui.SubmittedDate) + "/" + SqlFunctions.DatePart("m", ui.SubmittedDate) + "/" + SqlFunctions.DateName("day", ui.SubmittedDate),

                            Status = ui.Status ?? 0

                        }).ToList();

            return userList;


        }
        public UserModel GetUserDetails(string username)
        {
            var user = Entities.Users.Where(x => x.UserName == username).SingleOrDefault();
            int UserID = 0;

            int userid = 0;
            int TherapeuticID = 0;
            if (user != null)
            {
                UserID = user.UserID;




            }
            var userInfo = Entities.UserInfoes.Where(x => x.UserID == UserID).SingleOrDefault();

            if (userInfo != null)
            {
                userid = userInfo.id;
                TherapeuticID = userInfo.TherapeuticID ?? 0;
            }

            var registrationInfo = Entities.UserRegistrations.Where(x => x.UserID == UserID).SingleOrDefault();


            List<SpeakerProgramDetail> spdList = new List<SpeakerProgramDetail>();
            //get all distinct program id involving the speaker
            //june 20, Lianne, speaker get to see the  programs by the therapeuticAreas they are assigned to not by programrequest
            // var ProgramIDList = Entities.ProgramRequests.Where(pr=>(pr.ProgramSpeakerID == userid || pr.ProgramModeratorID== userid)).Select(tt => tt.ProgramID).Distinct().ToList();
            //if new program is introduced need to enter the new program id and its therapeutic id to therapeuticProgram table so that the correct program id are selected
            //for example for bad to bones (program id 3), 2 entries are made  therapeuticID 2 and programid 3  and also therapeuticID 3 and programid 3
            // var ProgramIDList = Entities.TherapeuticPrograms.Where(pr => pr.TherapeuticID == TherapeuticID).Select(tt => tt.ProgramID).ToList();
            var ProgramIDList = Entities.Programs.ToList();

            foreach (var item in ProgramIDList)
            {

                if (item.ProgramID != 6)
                {
                    SpeakerProgramDetail spd = new SpeakerProgramDetail();
                    List<ConfirmedSession> CombinedConfirmedSessionList = null;
                    var objProgram = Entities.Programs.Where(p => p.ProgramID == item.ProgramID).SingleOrDefault();
                    spd.ProgramID = objProgram.ProgramID;
                    spd.ProgramName = objProgram.ProgramName;

                    if (!String.IsNullOrEmpty(objProgram.ExpirationDate) && objProgram.ExpirationDate != "TBC")
                        spd.ExpirationDate = Convert.ToDateTime(objProgram.ExpirationDate);
                    bool DisplayProgramMaterial = false;
                    var COISlidesUploaded = Entities.COISlidesUploads.Where(pr => pr.UserID == UserID && pr.ProgramID == item.ProgramID).SingleOrDefault();

                    if (COISlidesUploaded != null)
                    {

                        spd.COISlidesUploaded = true;
                        spd.COISlidesExt = Entities.COISlidesUploads.Where(pr => pr.UserID == UserID && pr.ProgramID == item.ProgramID).SingleOrDefault().COISlidesExt;
                    }
                    else
                        spd.COISlidesUploaded = false;

                    //user can be either speaker or moderator, so if he chose date as a speaker or moderator he gets program material
                    var SpeakerChosenProgramDate = Entities.ProgramRequests.Where(pr => pr.ProgramSpeakerID == userid && pr.ProgramID == item.ProgramID && (pr.SpeakerChosenProgramDate ?? false)).FirstOrDefault();
                    if (SpeakerChosenProgramDate != null)
                    {
                        DisplayProgramMaterial = true;

                    }

                    var ModeratorChosenProgramDate = Entities.ProgramRequests.Where(pr => pr.ProgramModeratorID == userid && pr.ProgramID == item.ProgramID && (pr.ModeratorChosenProgramDate ?? false)).FirstOrDefault();
                    if (ModeratorChosenProgramDate != null)
                    {
                        DisplayProgramMaterial = true;

                    }

                    spd.DisplayProgramMaterial = DisplayProgramMaterial;
                    var SpeakerConfirmedSession = (from pr in Entities.ProgramRequests

                                                   where pr.ProgramSpeakerID == userid && pr.ProgramID == item.ProgramID


                                                   select new
                                                   {
                                                       ProgramRequestID = pr.ProgramRequestID,
                                                       ConfirmedDate = (pr.ConfirmedSessionDate.HasValue) ? SqlFunctions.DateName("month", pr.ConfirmedSessionDate) + " " + SqlFunctions.DateName("day", pr.ConfirmedSessionDate) + "," + SqlFunctions.DateName("year", pr.ConfirmedSessionDate) : string.Empty,
                                                       StartTime = (pr.ProgramStartTime.HasValue) ? SqlFunctions.DateName("hh", pr.ProgramStartTime) + ":" + SqlFunctions.DateName("mi", pr.ProgramStartTime) : string.Empty,
                                                       EndTime = (pr.ProgramEndTime.HasValue) ? SqlFunctions.DateName("hh", pr.ProgramEndTime) + ":" + SqlFunctions.DateName("mi", pr.ProgramEndTime) : string.Empty,
                                                       Location = pr.LocationName
                                                   });


                    var ModeratorConfirmedSession = (from pr in Entities.ProgramRequests

                                                     where pr.ProgramModeratorID == userid && pr.ProgramID == item.ProgramID
                                                     select new
                                                     {
                                                         ProgramRequestID = pr.ProgramRequestID,
                                                         ConfirmedDate = (pr.ConfirmedSessionDate.HasValue) ? SqlFunctions.DateName("month", pr.ConfirmedSessionDate) + " " + SqlFunctions.DateName("day", pr.ConfirmedSessionDate) + "," + SqlFunctions.DateName("year", pr.ConfirmedSessionDate) : string.Empty,
                                                         StartTime = (pr.ProgramStartTime.HasValue) ? SqlFunctions.DateName("hh", pr.ProgramStartTime) + ":" + SqlFunctions.DateName("mi", pr.ProgramStartTime) : string.Empty,
                                                         EndTime = (pr.ProgramEndTime.HasValue) ? SqlFunctions.DateName("hh", pr.ProgramEndTime) + ":" + SqlFunctions.DateName("mi", pr.ProgramEndTime) : string.Empty,
                                                         Location = pr.LocationName
                                                     });


                    var CombinedSessions = SpeakerConfirmedSession.Union(ModeratorConfirmedSession).Select(
                          x => new ConfirmedSession()
                          {
                              ProgramRequestID = x.ProgramRequestID,
                              ConfirmedDate = x.ConfirmedDate,
                              StartTime = x.StartTime,
                              EndTime = x.EndTime,
                              Location = x.Location
                          }).Where(x => !String.IsNullOrEmpty(x.ConfirmedDate));


                    CombinedConfirmedSessionList = CombinedSessions.ToList();



                    spd.MySession = CombinedConfirmedSessionList;
                    if (!String.IsNullOrEmpty(objProgram.ExpirationDate) && objProgram.ExpirationDate != "TBC")//only add the program request if the expiration date is less than today
                    {
                        if (DateTime.Compare(DateTime.Now, spd.ExpirationDate) <= 0)
                            spdList.Add(spd);
                    }
                    else//if there is no expiration date, it is a new program add it to the program request list
                    {
                        spdList.Add(spd);
                    }
                }
            }


            // List<ConfirmedSession> SpeakerConfirmedSession = null;
            //  List<ConfirmedSession> ModeratorConfirmedSession = null;
            //union by userid using programrequest table the presenter could be a speaker or a moderator












            if (userInfo != null)
            {


                UserModel um = new UserModel()
                {
                    UserID = userInfo.UserID ?? 0,
                    // TerritoryID = userInfo.TerritoryID,
                    Username = username,
                    FirstName = userInfo.FirstName,
                    LastName = userInfo.LastName,
                    EmailAddress = userInfo.EmailAddress,
                    ClinicName = userInfo.ClinicName,
                    Address = userInfo.Address,
                    Address2 = userInfo.Address2,
                    City = userInfo.City,
                    // Province = userInfo.Province,
                    PostalCode = userInfo.PostalCode,
                    Phone = userInfo.Phone,
                    Fax = userInfo.Fax,
                    Specialty = userInfo.Specialty,
                    HonariumRange = userInfo.SpeakerHonariumRange,
                    SpeakerProgramDetails = spdList,
                    COIForm = (registrationInfo == null) ? false : registrationInfo.COIForm ?? false,
                    PayeeForm = (registrationInfo == null) ? false : registrationInfo.PayeeForm ?? false,





                };
                return um;//return usermodel object
            }
            else
                return null;
        }
        #region Ali's code
        public bool CheckIfEmailExist(string Email)
        {
            bool ret = false;

            ret = Entities.Users.Any(x => x.UserName == Email);

            return ret;
        }

        public bool CheckEmailInUserinfo(string Email)
        {
            bool ret = false;

            ret = Entities.UserInfoes.Any(x => x.EmailAddress == Email);

            return ret;
        }


        public void UpdateOptOutStatusAndProgramRequest(int SpeakerOrModeratorID)
        {

            var user = Entities.UserInfoes.Where(x => x.id == SpeakerOrModeratorID).FirstOrDefault();

            if (user != null)
            {

                user.Status = 5;
                Entities.SaveChanges();
            }

            var ProgramRequest = Entities.ProgramRequests.Where(x => ((x.ProgramSpeakerID == SpeakerOrModeratorID) || (x.ProgramModeratorID == SpeakerOrModeratorID))).ToList();

            if (ProgramRequest != null)
            {

                foreach (var item in ProgramRequest)
                {

                    item.SpeakerDeclined = true;
                    item.ModeratorDeclined = true;
                    item.ReadOnly = false;
                    item.RequestStatus = 7;
                }


                Entities.SaveChanges();

            }



        }

        public bool EmailPasswordMatch(string Email, string Password)
        {
            bool ret = false;

            Password = Util.Encryptor.Encrypt(Password);
            var match = Entities.Users.Where(x => x.UserName == Email && x.Password == Password).SingleOrDefault();
            if (match != null)
            {
                ret = true;
            }
            return ret;
        }
        public bool CheckIfActivated(string Email)
        {
            bool ret = false;
            var match = Entities.Users.Where(x => x.UserName == Email).SingleOrDefault();
            if (match != null)
            {
                if (match.IsActive == true)
                {
                    ret = true;
                }
            }
            return ret;
        }
        public void NewRegisterUser(string Email, string NewPassword, string oldPassword)
        {
            oldPassword = Util.Encryptor.Encrypt(oldPassword);

            var user = Entities.Users.Where(x => x.UserName == Email && x.Password == oldPassword).SingleOrDefault();
            var userinfo = Entities.UserInfoes.Where(x => x.EmailAddress == Email).SingleOrDefault();

            if (user != null && userinfo != null)
            {
                user.IsActive = true;
                user.UserName = Email;
                user.Password = Util.Encryptor.Encrypt(NewPassword);
                user.ActivationDate = DateTime.Now;
                userinfo.UserID = user.UserID;
                Entities.SaveChanges();
            }
        }

        // sending small Id to get userinfo
        public UserModel GetUserForConfirmEmail(int id)
        {
            UserModel um = null;

            um = Entities.UserInfoes.Where(x => x.id == id).Select(ui =>
                     new UserModel
                     {
                         UserID = ui.UserID ?? 0,
                         FirstName = ui.FirstName,
                         LastName = ui.LastName,
                         EmailAddress = ui.EmailAddress,
                         ClinicName = ui.ClinicName,
                         Address = ui.Address,
                         Address2 = ui.Address2,
                         City = ui.City,
                         //Province = ui.Province,
                         PostalCode = ui.PostalCode,
                         Phone = ui.Phone,
                         Fax = ui.Fax,
                         Specialty = ui.Specialty,
                         HonariumRange = ui.SpeakerHonariumRange

                     }).SingleOrDefault();

            return um;

        }
        //Sending big User ID"
        public string CheckUserStatusForEmail(int UserID)
        {
            string val = "";
            var RegisterUser = Entities.UserRegistrations.Where(x => x.UserID == UserID).SingleOrDefault();
            if (RegisterUser == null)
            {

                val = "NotRegistered";
            }
            else
            {
                if ((RegisterUser.COIForm == true) && (RegisterUser.PayeeForm == true))
                    val = "Registered";
                else
                    val = "RegistrationNotComplete";

            }
            return val;


        }

        public RegistrationStatus GetRegistrationStatus(int UserID)
        {
            RegistrationStatus rs = null;

            rs = Entities.UserRegistrations.Where(x => x.UserID == UserID).Select(ui =>
                     new RegistrationStatus
                     {
                         UserID = UserID,
                         COIForm = ui.COIForm,
                         COIFormExt = ui.COIFormExt,
                         PayeeForm = ui.PayeeForm


                     }).SingleOrDefault();
            if (rs == null)
            {

                rs = new RegistrationStatus()
                {
                    UserID = UserID,
                    COIForm = false,
                    PayeeForm = false

                };
            }

            return rs;

        }

        public RegisterModel GetUserCredentials(string Email)
        {
            RegisterModel model = new RegisterModel();
            var user = Entities.Users.Where(x => x.UserName == Email).SingleOrDefault();

            if (user != null)
            {
                model.Email = user.UserName;
                model.CurrentPassword = Util.Encryptor.Decrypt(user.Password);
            }
            return model;
        }

        public UserModel GetSpeakerDetails(string username)
        {
            var user = Entities.Users.Where(x => x.UserName == username).SingleOrDefault();
            int userID = 0;
            if (user != null)
                userID = user.UserID;
            var userInfo = Entities.UserInfoes.Where(x => x.UserID == userID).SingleOrDefault();


            if (userInfo != null)
            {


                return new UserModel()
                {
                    UserID = userInfo.UserID ?? 0,
                    // TerritoryID = userInfo.TerritoryID,
                    Username = username,
                    FirstName = userInfo.FirstName,
                    LastName = userInfo.LastName,
                    EmailAddress = userInfo.EmailAddress,
                    ClinicName = userInfo.ClinicName,
                    Address = userInfo.Address,
                    Address2 = userInfo.Address2,
                    City = userInfo.City,
                    // Province = userInfo.Province,
                    PostalCode = userInfo.PostalCode,
                    Phone = userInfo.Phone,
                    Fax = userInfo.Fax,
                    Specialty = userInfo.Specialty,
                    HonariumRange = userInfo.SpeakerHonariumRange,


                };//return usermodel object
            }
            else
                return null;
        }




        #endregion
    }
}
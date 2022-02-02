using ClinicalConundrum2019.Data;
using ClinicalConundrums2019.Models;
using ClinicalConundrums2019.Util;
using ClinicalConundrums2019.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Web;
using static ClinicalConundrums2019.Util.Constants;

namespace ClinicalConundrums2019.DAL
{
    public class UserRepository : BaseRepository
    {
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
                            AssignedRole = ui.AssignedRole,
                            FirstName = ui.FirstName,
                            LastName = ui.LastName,
                            EmailAddress = ui.EmailAddress,
                            ClinicName = ui.ClinicName,
                            Address = ui.Address,
                            Address2 = ui.Address2,
                            City = ui.City,
                            Province = ui.Province,
                            PostalCode = ui.PostalCode,
                            Phone = ui.Phone,
                            Fax = ui.Fax,
                            Specialty = ui.Specialty,
                            HonariumRange = ui.SpeakerHonariumRange,
                            SubmittedDate = (ui.SubmittedDate == null) ? null : SqlFunctions.DateName("year", ui.SubmittedDate) + "/" + SqlFunctions.DatePart("m", ui.SubmittedDate) + "/" + SqlFunctions.DateName("day", ui.SubmittedDate),

                            //StatusString = ui.UserStatus.UserStatusDescription,
                            Status = ui.Status ?? 0,
                            Approved = (ui.Status >= 2) ? true : false,
                            COIForm = Entities.UserRegistrations.Any(x => x.UserID == ui.UserID && x.COIForm == true) ? true : false,
                            COISlides = Entities.COISlidesUploads.Any(x => x.UserID == ui.UserID && x.COISlides == true) ? true : false,


                        }).ToList();

            foreach (var speaker in userList)
            {
                if (speaker.AssignedRole != null)
                {
                    var AssignedRole = Entities.AssignedRoles.Where(x => x.id == speaker.AssignedRole).SingleOrDefault();
                    speaker.AssignedRoleDesc = AssignedRole.RoleDescription;
                }
                var salesRep = Entities.UserInfoes.Where(x => x.UserID == speaker.UserIDRequestedBy).SingleOrDefault();
                if (salesRep != null)
                {
                    speaker.RequestedByFirstName = salesRep.FirstName;
                    speaker.RequestedByLastName = salesRep.LastName;
                    speaker.RequestedBySalesRep = salesRep.FirstName + " " + salesRep.LastName;
                    speaker.StatusString = speaker.StatusString + ", Request By:" + speaker.RequestedBySalesRep;
                }
            }

            return userList;


        }
        public List<UserModel> GetAllUsersExceptSpeakers()//it actually retrieve both speakers and moderators
        {
            List<UserModel> userList = null;
            List<int> list = new List<int> { 2, 3 };//speakers: usertype=2  moderator: usertype=3
            userList = (from ui in Entities.UserInfoes
                        where !list.Contains(ui.UserType) //get everyone except speakers for admin tool, speakers/moderator are managed in a different tab

                        orderby ui.LastName
                        select new UserModel()
                        {
                            ID = ui.id,
                            UserID = ui.UserID ?? 0,
                            UserType = ui.UserTypeLookUp.Description,
                            FirstName = ui.FirstName,
                            LastName = ui.LastName,
                            EmailAddress = ui.EmailAddress,
                            ClinicName = ui.ClinicName,
                            Address = ui.Address,
                            Address2 = ui.Address2,
                            City = ui.City,
                            Province = ui.Province,
                            PostalCode = ui.PostalCode,
                            Phone = ui.Phone,
                            Fax = ui.Fax,
                            Specialty = ui.Specialty,
                           // SponsorName = ui.Sponsor.SponsorName,

                            RepID = ui.RepID,
                            BoneWBSCode = ui.BoneWBSCode,
                            CVWBSCode = ui.CVWBSCode,
                            TerritoryID = (ui.TerritoryID == null) ? "" : ui.TerritoryID.ToString(),

                            Deleted = ui.User.IsDeleted ,

                            Activated = ui.User.IsActive

                        }).ToList();

            return userList;


        }
        public UserModel GetUserByuserid(int userid)
        {
            UserModel um = null;


            um = Entities.UserInfoes.Where(x => x.id == userid).Select(ui =>
                     new UserModel
                     {
                         ID = ui.id,
                         UserID = ui.UserID ?? 0,
                         UserIDRequestedBy = ui.UserIDRequestedBy ?? 0,
                         FirstName = ui.FirstName,
                         LastName = ui.LastName,
                         EmailAddress = ui.EmailAddress,
                         ClinicName = ui.ClinicName,
                         Address = ui.Address,
                         Address2 = ui.Address2,
                         City = ui.City,
                         Province = ui.Province,
                         PostalCode = ui.PostalCode,
                         Phone = ui.Phone,
                         Fax = ui.Fax,
                         Specialty = ui.Specialty,
                         HonariumRange = ui.SpeakerHonariumRange,
                         SpeakerHonariumRange = ui.SpeakerHonariumRange,
                         Speaker2HonariumRange = ui.Speaker2HonariumRange,
                         //    TherapeuticArea = ui.TherapeuticArea.TherapeuticName,
                         ModeratorHonariumRange = ui.ModeratorHonariumRange,
                         TherapeuticID = ui.TherapeuticID ?? 0

                     }).SingleOrDefault();

            return um;

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
        public void ApproveSpeaker(SpeakerViewModel spVM)
        {
            //speaker may not have registered yet, search by id
            var m_user = Entities.UserInfoes.Where(x => x.id == spVM.ID).SingleOrDefault();
            if (m_user != null)
            {

                m_user.Status = 2;//2 is Approved
                Entities.SaveChanges();

            }
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
        public string[] GetRolesAsArray(string userName)
        {
            return Entities.Users.First(u => u.UserName == userName).Roles.Select(r => r.Name).ToArray();

        }
        public string GetRoleByUserID(int UserID)
        {
            //1, Sales Representative
            //2, Speaker
            //3, Moderator
            //4, Regional Manager
            //5, Brand Manager
            //6 CL Coordinator
            //7 Admin
            //8 CHE Manager
            //9 Director
            //10 National CME Lead
            //11 National Sales Lead
            var userRole = Entities.UserInfoes.Where(x => x.UserID == UserID).Select(x => x.UserTypeLookUp.Description).SingleOrDefault();
            return userRole;


        }
        public UserModel GetUserDetails(string username)
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
                    UserTypeID=userInfo.UserType,
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
                    Specialty = userInfo.Specialty
                 




                };//return usermodel object
            }
            else
                return null;
        }
        public string GetUserPassword(string username)
        {
            var user = Entities.Users.Where(x => x.UserName == username).SingleOrDefault();
            string password = string.Empty;

            if (user != null)
            {
                password = Util.Encryptor.Decrypt(user.Password);
            }
            return password;
        }

        public bool IsUserInUserInfo(string email)
        {
            var userInfo = Entities.UserInfoes.Where(e => e.EmailAddress == email).SingleOrDefault();

            return userInfo == null ? false : true;
        }

        public bool IsUserExisted(string emailAddress)
        {
            return Entities.Users.Where(u => u.EmailAddress == emailAddress).Count() > 0 ? true : false;
        }

        public bool ActivateUser(ActivationSubmitModel model)
        {
            if (IsUserExisted(model.Email))
            {
                return false;
            }

            bool isSuccessful = true;

            var userInfo = Entities.UserInfoes.Where(u => u.EmailAddress == model.Email).FirstOrDefault();
            if(userInfo != null)
            {
                userInfo.FirstName = model.FirstName;
                userInfo.LastName = model.LastName;
                userInfo.Phone = model.PhoneNumber;
                userInfo.AdditionalPhone = model.AdditionalPhoneNumber;

                //either BMS or Pfizer
                userInfo.ClinicName = model.ClinicName;
                userInfo.RegionID = int.Parse(model.RegionId);
                

                User user = new User();
                user.EmailAddress = model.Email;
                user.UserName = model.Username;
                user.Password = Util.Encryptor.Encrypt(model.Password);
                user.IsActive = true;
                user.IsDeleted = false;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;

                try
                {
                    Entities.Users.Add(user);
                    Entities.SaveChanges();

                    userInfo.UserID = user.UserID;
                    userInfo.SubmittedDate = DateTime.Now;
                    Entities.SaveChanges();
                    UserHelper.SendActivationEmail(model.FirstName, model.Username, model.Password);
                }
                catch(Exception e)
                {
                    isSuccessful = false;
                }
            }
            else
            {
                isSuccessful = false;

            }

            return isSuccessful;
        }
        public UserModel GetUserForConfirmEmail(int id)
        {
            UserModel um = null;

            um = Entities.UserInfoes.Where(x => x.id == id).Select(ui =>
                     new UserModel
                     {
                         ID = ui.id,
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
                         HonariumRange = ui.SpeakerHonariumRange,
                         SpeakerHonariumRange = ui.SpeakerHonariumRange,
                       
                         ModeratorHonariumRange = ui.ModeratorHonariumRange,
                         Speaker2HonariumRange=ui.Speaker2HonariumRange


                     }).SingleOrDefault();

            return um;

        }

        public string CheckUserStatusForEmail(int UserID)
        {
            string val = "";
            var IfUserActivated = Entities.Users.Any(x => x.UserID == UserID);
            var RegisterUser = Entities.UserRegistrations.Where(x => x.UserID == UserID).SingleOrDefault();

            if (RegisterUser != null)
            {
                if ((RegisterUser.COIForm == true) && (RegisterUser.PayeeForm == true))
                {
                    val = "RegisteredCompleted";

                }

                else
                {
                    val = "RegistrationNotComplete";

                }

            }
            else
            {
                if (IfUserActivated)
                {
                    val = "RegistrationNotComplete";

                }
                else
                {
                    val = "NotRegistered";

                }

            }



            return val;


        }

        public UserModel GetUserByID(int id)
        {
            UserModel um = null;


            um = Entities.UserInfoes.Where(x => x.id == id).Select(ui =>
                     new UserModel
                     {
                         ID = ui.id,
                         UserID = ui.UserID ?? 0,
                         FirstName = ui.FirstName,
                         LastName = ui.LastName,
                         EmailAddress = ui.EmailAddress,
                         ClinicName = ui.ClinicName,
                         Address = ui.Address,
                         Address2 = ui.Address2,
                         City = ui.City,
                         Province = ui.Province,
                         TherapeuticID = ui.TherapeuticID ?? 0,
                         PostalCode = ui.PostalCode,
                         Phone = ui.Phone,
                         Fax = ui.Fax,
                         Specialty = ui.Specialty,
                         HonariumRange = ui.SpeakerHonariumRange,
                         SpeakerHonariumRange = ui.SpeakerHonariumRange,
                         Speaker2HonariumRange = ui.Speaker2HonariumRange,
                         ModeratorHonariumRange = ui.ModeratorHonariumRange,
                         TerritoryID = ui.TerritoryID,
                         RepID = ui.RepID,
                         BoneWBSCode = ui.BoneWBSCode,
                         CVWBSCode = ui.CVWBSCode


                     }).SingleOrDefault();

            return um;

        }

        public UserModel GetUserByUserID(int UserID)
        {
            UserModel um = null;


            um = Entities.UserInfoes.Where(x => x.UserID == UserID).Select(ui =>
                     new UserModel
                     {
                         ID = ui.id,
                         UserID = UserID,
                         FirstName = ui.FirstName,
                         LastName = ui.LastName,
                         EmailAddress = ui.EmailAddress,
                         ClinicName = ui.ClinicName,
                         Address = ui.Address,
                         Address2 = ui.Address2,
                         City = ui.City,
                         Province = ui.Province,
                         PostalCode = ui.PostalCode,
                         Phone = ui.Phone,
                         Fax = ui.Fax,
                         Specialty = ui.Specialty,
                         HonariumRange = ui.SpeakerHonariumRange,
                         SpeakerHonariumRange = ui.SpeakerHonariumRange,
                         ModeratorHonariumRange = ui.ModeratorHonariumRange


                     }).SingleOrDefault();

            return um;

        }
        public UserModel GetSalesRepForConfirmEmail(int UserID)
        {
            UserModel um = null;

            um = Entities.UserInfoes.Where(x => x.UserID == UserID).Select(ui =>
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
        public void EditSpeaker(SpeakerViewModel spVM)
        {


            var m_userInfo = (from v in Entities.UserInfoes
                              where v.id == spVM.ID

                              select v).FirstOrDefault();
            if (m_userInfo != null)
            {
                m_userInfo.FirstName = spVM.FirstName;
                m_userInfo.LastName = spVM.LastName;
                m_userInfo.EmailAddress = spVM.EmailAddress;
                m_userInfo.Address = spVM.Address;
                m_userInfo.City = spVM.City;
                m_userInfo.Province = spVM.Province;
                m_userInfo.PostalCode = spVM.PostalCode;
                m_userInfo.Phone = spVM.Phone;
                m_userInfo.Fax = spVM.Fax;
                m_userInfo.SpeakerHonariumRange = spVM.SpeakerHonariumRange;
                m_userInfo.Speaker2HonariumRange = spVM.Speaker2HonariumRange;
                m_userInfo.ModeratorHonariumRange = spVM.ModeratorHonariumRange;
                
                m_userInfo.LastUpdated = DateTime.Now;
                Entities.SaveChanges();//update patient info
            }
        }

        public void EditUser(UserViewModel um)
        {


            var usInfo = Entities.UserInfoes.Where(u => u.id == um.id).FirstOrDefault();
            if (usInfo != null)
            {
                usInfo.UserType = um.UserTypeID;
                usInfo.FirstName = um.FirstName;
                usInfo.LastName = um.LastName;
                usInfo.EmailAddress = um.EmailAddress;
                usInfo.Address = um.Address;
                usInfo.ClinicName = um.ClinicName;
                usInfo.City = um.City;
                usInfo.Province = um.Province;
                usInfo.SponsorID = um.SponsorID;
               // usInfo.PrivilegeID = um.TherapeuticID;
               // usInfo.TherapeuticID = um.TherapeuticID;

                usInfo.PostalCode = um.PostalCode;
                usInfo.Phone = um.Phone;
                usInfo.Fax = um.Fax;
                usInfo.SpeakerHonariumRange = um.HonariumRange;
              

                usInfo.TerritoryID = um.TerritoryID;
                usInfo.RepID = um.RepID;
                usInfo.BoneWBSCode = um.BoneWBSCode;
                usInfo.CVWBSCode = um.CVWBSCode;

                Entities.SaveChanges();
            }
        }


        public void AddUser(UserViewModel um)
        {

            if (Entities.UserInfoes.Where(u => u.EmailAddress == um.EmailAddress).Count() > 0)
            {

                throw new Exception();//cannot add a user with duplicated email address

            }
            

            UserInfo usInfo = new UserInfo();
            usInfo.UserType = um.UserTypeID;
            usInfo.FirstName = um.FirstName;
            usInfo.LastName = um.LastName;
            usInfo.EmailAddress = um.EmailAddress;
            usInfo.Address = um.Address;
            usInfo.ClinicName = um.ClinicName;
            usInfo.City = um.City;
            usInfo.Province = um.Province;
            usInfo.SponsorID = um.SponsorID;
           // usInfo.PrivilegeID = um.TherapeuticID;
          //  usInfo.TherapeuticID = um.TherapeuticID;

            usInfo.PostalCode = um.PostalCode;
            usInfo.Phone = um.Phone;
            usInfo.Fax = um.Fax;
            usInfo.SpeakerHonariumRange = um.HonariumRange;

            usInfo.TerritoryID = um.TerritoryID;
            usInfo.RepID = um.RepID;
            usInfo.BoneWBSCode = um.BoneWBSCode;
            usInfo.CVWBSCode = um.CVWBSCode;

            Entities.UserInfoes.Add(usInfo);
            Entities.SaveChanges();

        }
        public int AddSpeaker(SpeakerViewModel spVM)
        {
            //User us = new User();
            //int userID;
            //us.Username = spVM.EmailAddress;
            //us.Password = ConfigurationManager.AppSettings["defaultPassword"].ToString();//default password (password) for a new unactivated user
            //us.Actviated = false;
            //Entities.Users.Add(us);
            //Entities.SaveChanges();
            //userID = us.UserID;

            UserInfo usInfo = new UserInfo();

            // usInfo.UserID = userID;
            usInfo.UserType = spVM.UserType;
            usInfo.FirstName = spVM.FirstName;
            usInfo.LastName = spVM.LastName;
            usInfo.EmailAddress = spVM.EmailAddress;
            usInfo.Address = spVM.Address;
            usInfo.City = spVM.City;
            usInfo.Province = spVM.Province;
           
            usInfo.PostalCode = spVM.PostalCode;
            usInfo.Phone = spVM.Phone;
            usInfo.Fax = spVM.Fax;
            usInfo.SpeakerHonariumRange = spVM.SpeakerHonariumRange;
            usInfo.Speaker2HonariumRange = spVM.Speaker2HonariumRange;
            usInfo.ModeratorHonariumRange = spVM.ModeratorHonariumRange;
            usInfo.Status = 2;
            usInfo.AssignedRole = 1;
            usInfo.SubmittedDate = DateTime.Now;
            usInfo.LastUpdated = DateTime.Now;
            Entities.UserInfoes.Add(usInfo);
            Entities.SaveChanges();



            return usInfo.id;


        }


        public void UpdateUserActivation(ActivationModel model) //admin
        {
            var userInfo = Entities.UserInfoes.Where(u => u.EmailAddress == model.Email).FirstOrDefault();
            if (userInfo != null)
            {
                User user = Entities.Users.Where(u => u.EmailAddress == model.Email).FirstOrDefault();
                if(user == null)
                {
                    user = new User();
                    user.EmailAddress = userInfo.EmailAddress;
                    user.UserName = userInfo.EmailAddress;
                    string defaultPassword = ConfigurationManager.AppSettings["defaultPassword"];
                    user.Password = Util.Encryptor.Encrypt(defaultPassword);
                    user.IsActive = true;
                    user.IsDeleted = false;
                    user.FirstName = userInfo.FirstName;
                    user.LastName = userInfo.LastName;
                    user.ActivationDate = DateTime.Now;

                    Entities.Users.Add(user);
                    Entities.SaveChanges();

                    userInfo.UserID = user.UserID;
                    userInfo.SubmittedDate = DateTime.Now;
                    Entities.SaveChanges();
                }
            }
        }


        public UserInfo GetUserInfoByEmail(string email)
        {
            return Entities.UserInfoes.Where(x => x.EmailAddress == email).FirstOrDefault();
        }

        public User GetUserByEmail(string email)
        {
            return Entities.Users.Where(x => x.EmailAddress == email).FirstOrDefault();
        }
        public bool CheckIfActivated(string Email)
        {
            bool ret = false;
            var match = Entities.Users.Where(x => x.EmailAddress == Email).SingleOrDefault();
            if (match != null)
            {
                if (match.IsActive == true)
                {
                    ret = true;
                }
            }
            return ret;
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

    }
}
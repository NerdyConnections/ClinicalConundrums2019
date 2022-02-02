using ClinicalConundrum2019.Data;
using ClinicalConundrumsSpeaker.Models;
using ClinicalConundrumsSpeaker.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClinicalConundrumsSpeaker.DAL
{
    public class ActivateRepository : BaseRepository
    {
        public SpeakerActivationModel GetActivationSpeakerbyEmail(string email)
        {
            var userInfo = Entities.UserInfoes.Where(x => x.EmailAddress == email).FirstOrDefault();

            if (userInfo != null)
            {
                SpeakerActivationModel model = new SpeakerActivationModel();
                model.UserId = userInfo.UserID;
                model.FirstName = userInfo.FirstName;
                model.LastName = userInfo.LastName;
                model.Phone = userInfo.Phone;
                model.AdditionalPhone = userInfo.AdditionalPhone;
                model.Speciality = userInfo.Specialty;
                model.Clinic = userInfo.ClinicName;
                model.Address = userInfo.Address;
                model.City = userInfo.City;
                model.Province = userInfo.Province;
                model.Username = userInfo.EmailAddress;
                return model;
            }else
            {
                return null;
            }           
        }

        public User GetUserByEmail(string email)
        {
            return Entities.Users.Where(x => x.UserName == email).FirstOrDefault();
        }

        public UserInfo GetUserInfoByEmail(string email)
        {
            return Entities.UserInfoes.Where(x => x.EmailAddress == email).FirstOrDefault();
        }
        
        public void ActivateSpeaker(SpeakerActivationModel model, UserInfo userInfo)
        {
            User user= new User();
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.EmailAddress = model.Username;
            user.UserName = model.Username;
            user.Password = Encryptor.Encrypt(model.Password);
            user.IsActive = true;
            user.IsDeleted = false;
            user.ActivationDate = DateTime.Now;
            Entities.Users.Add(user);
            Entities.SaveChanges();

            userInfo.UserID = user.UserID;
            userInfo.EmailAddress = model.Username;
            userInfo.FirstName = model.FirstName;
            userInfo.LastName = model.LastName;
            userInfo.Phone = model.Phone;
            userInfo.AdditionalPhone = model.AdditionalPhone;
            userInfo.Specialty = model.Speciality;
            userInfo.ClinicName = model.Clinic;
            userInfo.Address = model.Address;
            userInfo.City = model.City;
            userInfo.Province = model.Province;
            Entities.SaveChanges();
        }     
    }
}
using ClinicalConundrum2019.Data;
using ClinicalConundrums2019.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClinicalConundrums2019.DAL
{
    public class HandoutRequestRepository : BaseRepository
    {
        public void SaveHandoutRequest(HandoutRequestModel model)
        {
            HandoutRequest request = new HandoutRequest();
            request.UserID = model.UserID;
            request.Number = model.Number;
            request.Language = model.Language;
            request.DateRequiredBy = DateTime.ParseExact(model.DateRequiredBy, "yyyy/MM/dd", null);
            request.ContactName = model.ContactName;
            request.EmailAddress = model.EmailAddress;
            request.MailingAddress = model.MailingAddress;
            request.City = model.City;
            request.Province = model.Province;
            request.PostalCode = model.PostalCode;
            request.PhoneNumber = model.PhoneNumber;
            request.MailToPresenter = model.MailToPresenter.Equals("Y") ? true : false;
            request.PresenterName = model.PresenterName;
            request.PresenterMailingAddress = model.PresenterMailingAddress;
            request.PresenterCity = model.PresenterCity;
            request.PresenterProvince = model.PresenterProvince;
            request.PresenterPostalCode = model.PresenterPostalCode;
            request.AdditionalInfo = model.AdditionalInfo;

            request.SubmittedOn = DateTime.Now;

            Entities.HandoutRequests.Add(request);

            try
            {
                Entities.SaveChanges();
            }
            catch (Exception e)
            {

            }
        }

        public HandoutRequestModel GetHandoutRequest(int userId)
        {
            HandoutRequest hr = Entities.HandoutRequests.Where(x => x.UserID == userId).ToList().OrderByDescending(d => d.SubmittedOn).FirstOrDefault();

            if (hr != null)
            {
                HandoutRequestModel model = new HandoutRequestModel();
                model.UserID = hr.UserID.HasValue ? hr.UserID.Value : 0;
                model.Number = hr.Number.HasValue ? hr.Number.Value : 0;
                model.Language = hr.Language;
                model.DateRequiredBy = hr.DateRequiredBy.HasValue ? hr.DateRequiredBy.Value.ToString("yyyy/MM/dd") : "";
                model.ContactName = hr.ContactName;
                model.EmailAddress = hr.EmailAddress;
                model.MailingAddress = hr.MailingAddress;
                model.City = hr.City;
                model.Province = hr.Province;
                model.PostalCode = hr.PostalCode;
                model.PhoneNumber = hr.PhoneNumber;
                model.MailToPresenter = hr.MailToPresenter.HasValue ? (hr.MailToPresenter.Value == true ? "Y" : "N") : "";
                model.PresenterName = hr.PresenterName;
                model.PresenterMailingAddress = hr.PresenterMailingAddress;
                model.PresenterCity = hr.PresenterCity;
                model.PresenterProvince = hr.PresenterProvince;
                model.PresenterPostalCode = hr.PresenterPostalCode;
                model.AdditionalInfo = hr.AdditionalInfo;

                return model;
            }else
            {
                return null;
            }            
        }

    }
}
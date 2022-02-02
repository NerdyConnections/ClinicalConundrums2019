using ClinicalConundrum2019.Data;
using ClinicalConundrumsSpeaker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClinicalConundrumsSpeaker.DAL
{
    public class PayeeRepository: BaseRepository
    {
        public PayeeModel GetPayeeModelByUserId(int userId)
        {
            PayeeInfo payeeInfo = Entities.PayeeInfoes.Where(x => x.UserID == userId).FirstOrDefault();
            PayeeModel model = new PayeeModel();

            model.UserId = userId;
            if (payeeInfo != null)
            {                
                model.PaymentMethod = payeeInfo.PaymentMethod;
                model.PayableTo = payeeInfo.ChequePayableTo;
                model.IRN = payeeInfo.InternalRefNum;
                model.MailingAddress1 = payeeInfo.MailingAddr1;
                model.MailingAddress2 = payeeInfo.MailingAddr2;
                model.AttentionTo = payeeInfo.AttentionTo;
                model.City = payeeInfo.City;
                model.Province = payeeInfo.ProvinceID;
                model.PostalCode = payeeInfo.PostalCode;
                model.TaxNumber = payeeInfo.TaxNumber;
                model.Instructions = payeeInfo.AdditionalInstructions;
            }
            return model;
        }

        public void AddOrUpdatePayeeInfo(PayeeModel model)
        {
            PayeeInfo payeeInfo = Entities.PayeeInfoes.Where(x => x.UserID == model.UserId).FirstOrDefault();
            if(payeeInfo == null)
            {
                payeeInfo = new PayeeInfo();
                payeeInfo.UserID = model.UserId;
                Entities.PayeeInfoes.Add(payeeInfo);
            }
            payeeInfo.PaymentMethod = model.PaymentMethod;
            payeeInfo.ChequePayableTo = model.PayableTo;
            payeeInfo.InternalRefNum = model.IRN;
            payeeInfo.MailingAddr1 = model.MailingAddress1;
            payeeInfo.MailingAddr2 = model.MailingAddress2;
            payeeInfo.AttentionTo = model.AttentionTo;
            payeeInfo.City = model.City;
            payeeInfo.ProvinceID = model.Province;
            payeeInfo.PostalCode = model.PostalCode;
            payeeInfo.TaxNumber = model.TaxNumber;
            payeeInfo.AdditionalInstructions = model.Instructions;
            payeeInfo.LastUpdated = DateTime.Now;

            UserRegistration reg = Entities.UserRegistrations.Where(x => x.UserID == model.UserId).FirstOrDefault();
            if(reg == null)
            {
                reg = new UserRegistration();
                reg.UserID = model.UserId;
                Entities.UserRegistrations.Add(reg);
            }
            reg.PayeeForm = true;
            reg.PayeeFormDate = DateTime.Now;

            Entities.SaveChanges();
        }
    }
}
    
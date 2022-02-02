using ClinicalConundrums2019.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClinicalConundrums2019.CustomValidation
{
    public class ValidateActivateEmail : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string email = (string)value;
            if (string.IsNullOrEmpty(email))
            {
                return new ValidationResult(App_GlobalResources.Resource.RequiredFieldWithAsterisk);
            }else
            {
                //July19 ,2019 this is for clinical conundrums no need to validate email domains
                //if(!email.Contains("@bms") && !email.Contains("@pfizer"))
                //{
                //    return new ValidationResult(App_GlobalResources.Resource.EMAIL_INVALID);
                //}

                UserRepository repo = new UserRepository();
                if (!repo.IsUserInUserInfo(email))
                {
                    return new ValidationResult(App_GlobalResources.Resource.CannotFindEmail);
                }
                if (repo.IsUserExisted(email))
                {
                    return new ValidationResult(App_GlobalResources.Resource.UserExist);
                }
            }
     
            return ValidationResult.Success;
        }
    }
}
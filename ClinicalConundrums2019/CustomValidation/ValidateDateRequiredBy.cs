using ClinicalConundrums2019.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClinicalConundrums2019.CustomValidation
{
    public class ValidateDateRequiredBy : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty((string)value))
            {              
                return new ValidationResult(App_GlobalResources.Resource.RequiredFieldWithAsterisk);
            }

            int numDaysLater = int.Parse(System.Configuration.ConfigurationManager.AppSettings["HandoutRequestNumDaysLater"]); 
            DateTime dateRequired = DateTime.Now.Date.AddDays(numDaysLater);
            DateTime dateValue = DateTime.ParseExact((string)value, "yyyy/MM/dd", null);

            if(DateTime.Compare(dateValue, dateRequired) < 0)
            {
                return new ValidationResult("Required date must be " + numDaysLater + " day(s) later than today");
            }

            return ValidationResult.Success;
        }
    }
}
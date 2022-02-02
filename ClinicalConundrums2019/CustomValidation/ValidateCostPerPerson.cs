using ClinicalConundrums2019.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicalConundrums2019.CustomValidation 
{
    public class ValidateCostPerPerson : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            EventRequestModel model = (EventRequestModel)validationContext.ObjectInstance;
            if (model.EventType == "InPerson" && model.CostPerPerson==null)
            {
                return new ValidationResult(App_GlobalResources.Resource.RequiredFieldWithAsterisk);
            }

            return ValidationResult.Success;
        }
    }
}
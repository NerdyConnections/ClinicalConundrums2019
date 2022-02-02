using ClinicalConundrums2019.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicalConundrums2019.CustomValidation
{
    public class ValidateEventLocationDetails : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            EventRequestModel model = (EventRequestModel)validationContext.ObjectInstance;
            if (model.VenueContacted == "Y" || model.VenueContacted == "N")
            {
                if (value == null || value != null && value.ToString() == "")
                {
                    return new ValidationResult(App_GlobalResources.Resource.RequiredFieldWithAsterisk);
                }
            }

            return ValidationResult.Success;
        }
    }
}
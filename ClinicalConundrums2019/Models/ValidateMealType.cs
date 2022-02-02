using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClinicalConundrums2019.Models
{
    public class ValidateMealType : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            EventRequestModel model = (EventRequestModel)validationContext.ObjectInstance;
            if (model.VenueContacted == "Y" || model.VenueContacted == "N")
            {
                if (string.IsNullOrEmpty((string)value) && (!string.IsNullOrEmpty(model.MealStartTime) || (model.MealOption != null && !model.MealOption.Equals("no"))))
                {
                    return new ValidationResult(App_GlobalResources.Resource.RequiredFieldWithAsterisk);
                }
            }

            return ValidationResult.Success;
        }
    }
}
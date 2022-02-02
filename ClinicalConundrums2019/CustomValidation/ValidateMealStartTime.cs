using ClinicalConundrums2019.Models;
using System.ComponentModel.DataAnnotations;


namespace ClinicalConundrums2019.CustomValidation
{
    public class ValidateMealStartTime : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            EventRequestModel model = (EventRequestModel)validationContext.ObjectInstance;
            if (string.IsNullOrEmpty(model.MealStartTime) && !string.IsNullOrEmpty(model.MealOption) && !model.MealOption.Equals("no"))
            {
                return new ValidationResult(App_GlobalResources.Resource.RequiredFieldWithAsterisk);
            }

            return ValidationResult.Success;
        }
    }
}
using ClinicalConundrums2019.Models;
using System.ComponentModel.DataAnnotations;

namespace ClinicalConundrums2019.CustomValidation
{
    public class ValidateEventTypeQuestions : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            EventRequestModel model = (EventRequestModel)validationContext.ObjectInstance;
            if (model.EventType == "Webcast")
            {
                if (string.IsNullOrEmpty((string)value))
                {
                    return new ValidationResult(App_GlobalResources.Resource.RequiredFieldWithAsterisk);
                }
            }

            return ValidationResult.Success;
        }
    }
}
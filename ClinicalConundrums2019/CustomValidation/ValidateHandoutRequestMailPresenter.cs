using ClinicalConundrums2019.Models;
using System.ComponentModel.DataAnnotations;


namespace ClinicalConundrums2019.CustomValidation
{
    public class ValidateHandoutRequestMailPresenter : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            HandoutRequestModel model = (HandoutRequestModel)validationContext.ObjectInstance;
            if (model.MailToPresenter == "Y")
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
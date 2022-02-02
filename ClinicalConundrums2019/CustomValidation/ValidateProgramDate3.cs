using ClinicalConundrums2019.Models;
using System.ComponentModel.DataAnnotations;

namespace ClinicalConundrums2019.CustomValidation
{
    public class ValidateProgramDate3 : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            EventRequestModel model = (EventRequestModel)validationContext.ObjectInstance;
            if (!string.IsNullOrEmpty(model.ProgramDate3))
            {
                if (model.ProgramDate3.Equals(model.ProgramDate1))
                {
                    return new ValidationResult(App_GlobalResources.Resource.Cannot_be_same_date_as_event_date);
                }
                if (model.ProgramDate3.Equals(model.ProgramDate2))
                {
                    return new ValidationResult(App_GlobalResources.Resource.Cannot_be_same_date_as_event_date2);
                }
            }

            return ValidationResult.Success;
        }
    }
}
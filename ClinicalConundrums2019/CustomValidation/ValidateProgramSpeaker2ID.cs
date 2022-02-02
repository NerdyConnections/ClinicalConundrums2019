using ClinicalConundrums2019.Models;
using System.ComponentModel.DataAnnotations;


namespace ClinicalConundrums2019.CustomValidation
{
    public class ValidateProgramSpeaker2ID : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            EventRequestModel model = (EventRequestModel)validationContext.ObjectInstance;
            if (model.ProgramSpeaker2ID != null)
            {
                if (model.ProgramSpeaker2ID == model.ProgramSpeakerID)
                {
                    return new ValidationResult(App_GlobalResources.Resource.Cannot_Be_Same_As_Speaker1);
                }
                
            }

            return ValidationResult.Success;
        }
    }
}
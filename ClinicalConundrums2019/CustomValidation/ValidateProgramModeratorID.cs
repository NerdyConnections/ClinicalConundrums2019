using ClinicalConundrums2019.Models;
using System.ComponentModel.DataAnnotations;


namespace ClinicalConundrums2019.CustomValidation
{
    public class ValidateProgramModeratorID : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            EventRequestModel model = (EventRequestModel)validationContext.ObjectInstance;
            if (model.ProgramModeratorID != null)
            {
                if (model.ProgramModeratorID == model.ProgramSpeakerID)
                {
                    return new ValidationResult(App_GlobalResources.Resource.Cannot_Be_Same_As_Speaker1);
                }
                if (model.ProgramModeratorID == model.ProgramSpeaker2ID)
                {
                    return new ValidationResult(App_GlobalResources.Resource.Cannot_Be_Same_As_Speaker2);
                }

            }

            return ValidationResult.Success;
        }
    }
}
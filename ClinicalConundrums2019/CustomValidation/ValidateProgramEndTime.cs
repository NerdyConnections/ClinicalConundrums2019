using ClinicalConundrums2019.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicalConundrums2019.CustomValidation
{
    public class ValidateProgramEndTime : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            EventRequestModel model = (EventRequestModel)validationContext.ObjectInstance;

            DateTime startTime = DateTime.ParseExact(model.ProgramDate1, "yyyy/MM/dd", null);
            DateTime programStartTime = startTime.Add(TimeSpan.Parse(model.ProgramStartTime));
            DateTime programEndTime = startTime.Add(TimeSpan.Parse(model.ProgramEndTime));
            if(programEndTime <= programStartTime)
            {
                return new ValidationResult(App_GlobalResources.Resource.GreaterThanStartTime);
            }

            return ValidationResult.Success;
        }
    }
}
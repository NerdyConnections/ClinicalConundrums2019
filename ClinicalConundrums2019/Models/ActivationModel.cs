using ClinicalConundrums2019.CustomValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClinicalConundrums2019.Models
{
    public class ActivationModel
    {
        [EmailAddress(ErrorMessage = "*Email is Invalid")]
        [ValidateActivateEmail]
        public string Email { get; set; }

    }
}
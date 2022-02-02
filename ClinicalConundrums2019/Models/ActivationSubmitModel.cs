using ClinicalConundrums2019.CustomValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClinicalConundrums2019.Models
{
    public class ActivationSubmitModel
    {
      
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public string FirstName { get; set; }
        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public string LastName { get; set; }
        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public string PhoneNumber { get; set; }
        public string AdditionalPhoneNumber { get; set; }
        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public string ClinicName { get; set; }
        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public string RegionId { get; set; }
        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        [EmailAddress(ErrorMessage = "*Email is Invalid")]
        public string Username { get; set; }
        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public string Password { get; set; }
        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        [Compare("Password", ErrorMessage = "*Password does not match")]
        public string ConfirmedPassword { get; set; }
    }
}
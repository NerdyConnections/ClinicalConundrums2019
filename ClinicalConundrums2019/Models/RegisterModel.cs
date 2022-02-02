using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClinicalConundrums2019.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "*Email is Invalid")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        [DataType(DataType.Password)]
        [Display(Name = "CURRENT PASSWORD")]
        public string CurrentPassword { get; set; }


        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        [DataType(DataType.Password)]
        [Display(Name = "NEW PASSWORD")]
        public string NewPassword { get; set; }

        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        [DataType(DataType.Password)]
        [Display(Name = "CONFIRM NEW PASSWORD")]
        public string ConfirmNewPassword { get; set; }
    }
}
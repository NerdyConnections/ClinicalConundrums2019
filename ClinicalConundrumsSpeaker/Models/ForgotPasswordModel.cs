using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClinicalConundrumsSpeaker.Models
{
    public class ForgotPasswordModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "*Email is Invalid")]
        public string Email { get; set; }
    }
}
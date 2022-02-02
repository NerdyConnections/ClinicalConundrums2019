using ClinicalConundrums2019.CustomValidation;
using System.ComponentModel.DataAnnotations;

namespace ClinicalConundrums2019.Models
{
    public class ForgotPasswordModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "*Email is Invalid")]
        public string Email { get; set; }
    }
}
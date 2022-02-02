using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClinicalConundrumsFrenchSpeaker.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "* Champ requis")]
       
      
        public string Email { get; set; }

        [Required(ErrorMessage = "* Champ requis")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
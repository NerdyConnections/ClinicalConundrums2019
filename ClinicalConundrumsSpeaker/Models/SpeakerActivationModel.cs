using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClinicalConundrumsSpeaker.Models
{
    public class SpeakerActivationModel
    {
        public int? UserId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public string FirstName { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public string LastName { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public string Phone { get; set; }
        public string AdditionalPhone { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public string Speciality { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public string Clinic { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public string Address { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public string City { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public string Province { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        [EmailAddress]
        public string Username { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }
}
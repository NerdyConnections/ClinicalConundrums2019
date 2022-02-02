using ClinicalConundrums2019.CustomValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClinicalConundrums2019.Models
{
    public class HandoutRequestModel
    {
        public int UserID { get; set; }
        public string UserEmail { get; set; }
        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public int Number { get; set; }
        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public string Language { get; set; }
        [ValidateDateRequiredBy]
        public string DateRequiredBy { get; set; }
        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public string ContactName { get; set; }
        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public string EmailAddress { get; set; }
        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public string MailingAddress { get; set; }
        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public string City { get; set; }
        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public string Province { get; set; }
        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public string PostalCode { get; set; }
        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public string MailToPresenter { get; set; }
        [ValidateHandoutRequestMailPresenter]
        public string PresenterName { get; set; }
        [ValidateHandoutRequestMailPresenter]
        public string PresenterMailingAddress { get; set; }
        [ValidateHandoutRequestMailPresenter]
        public string PresenterCity { get; set; }
        [ValidateHandoutRequestMailPresenter]
        public string PresenterProvince { get; set; }
        [ValidateHandoutRequestMailPresenter]
        public string PresenterPostalCode { get; set; }
        public string AdditionalInfo { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClinicalConundrums2019.ViewModels
{
    public class UserViewModel
    {
        public int? id { get; set; }
        public int? UserID { get; set; }
        public string UserType { get; set; }

        [Required]
        public int UserTypeID { get; set; }
        public string TerritoryID { get; set; }
        public string RepID { get; set; }
        public string BoneWBSCode { get; set; }
        public string CVWBSCode { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        [Required]
        public string EmailAddress { get; set; }
        public string ClinicName { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        [Required]
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string Specialty { get; set; }
        public string HonariumRange { get; set; }
        //public string TherapeuticArea { get; set; }
       // public int TherapeuticID { get; set; }
        public int PrivilegeID { get; set; }
        [Required]
        public int SponsorID { get; set; }
        public string SponsorName { get; set; }

        public string SubmittedDate { get; set; }
        public bool? Approved { get; set; }
        public bool? Activated { get; set; }
    }
}
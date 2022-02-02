using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClinicalConundrums2019.Models
{
   
        public class UserModel
        {



            public int ID { get; set; }
            public int UserIDRequestedBy { get; set; }
            public int UserID { get; set; }
            public string UserType { get; set; }
            public int UserTypeID { get; set; }
            public string Language { get; set; }
            public string SpecifiedCulture { get; set; }
            [Required]
            public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
            public string Password { get; set; }
            [Compare("Password")]
            public string ConfirmPassword { get; set; }

        public string RequestedByFirstName { get; set; }
        public string RequestedByLastName { get; set; }
        public string RequestedBySalesRep { get; set; }

        [Required]
            public string FirstName { get; set; }
            [Required]
            public string LastName { get; set; }
            [Required]
            public string EmailAddress { get; set; }
            public string ClinicName { get; set; }
            [Required]
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
        public string SpeakerHonariumRange { get; set; }
        public string Speaker2HonariumRange { get; set; }
        public string ModeratorHonariumRange { get; set; }
        public string TherapeuticArea { get; set; }
        public int TherapeuticID { get; set; }
        public int PrivilegeID { get; set; }

        public int SponsorID { get; set; }
        public string SponsorName { get; set; }
        public int? AssignedRole { get; set; }
        public string AssignedRoleDesc { get; set; }
        public string SubmittedDate { get; set; }
        public string Comment { get; set; }
        public bool? Approved { get; set; }
        public bool? Activated { get; set; }
        public bool? Deleted { get; set; }
        public int Status { get; set; }
        public string StatusString { get; set; }
        public bool COIForm { get; set; }
        public bool COISlides { get; set; }
        public string TerritoryID { get; set; }
        public string RepID { get; set; }
        public string BoneWBSCode { get; set; }
        public string CVWBSCode { get; set; }
    }
    
}
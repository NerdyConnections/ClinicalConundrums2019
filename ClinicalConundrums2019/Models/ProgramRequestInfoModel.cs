using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClinicalConundrums2019.Models
{
    public class ProgramRequestInfoModel
    {
        [DisplayFormat(DataFormatString = "{0:mm/dd/yyyy}")]
        public string ConfirmedSessionDate { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Speaker1FirstName { get; set; }
        public string Speaker1LastName { get; set; }
        public string Speaker2FirstName { get; set; }
        public string Speaker2LastName { get; set; }
        public string ModeratorFirstName { get; set; }
        public string ModeratorLastName { get; set; }
        public string SessionID { get; set; }
        public string VenueName { get; set; }
        public string VenueAddress { get; set; }
        public string VenueCity { get; set; }
        public string VenueProvince { get; set; }
        public string VenuePostalCode { get; set; }
        public string TotalSessionCredits { get; set; }
        public string filename { get; set; }
        public decimal TotalCredits { get; set; }

        //venue name address city province postal code
        public string location { get; set; }
        public string programstarttime { get; set; }
        public string programendtime { get; set; }
        public string RegistrationComments1 { get; set; }
        public string RegistrationComments2 { get; set; }
        public string RSVP { get; set; }
        public bool SessionCredit1 { get; set; }
        public bool SessionCredit2 { get; set; }
        public bool SessionCredit3 { get; set; }
        public bool SessionCredit4 { get; set; }
        public bool SessionCredit5 { get; set; }
        public bool SessionCredit6 { get; set; }
        public bool SessionCredit7 { get; set; }
        public bool SessionCredit8 { get; set; }
        public bool SessionCredit9 { get; set; }
        public bool SessionCredit10 { get; set; }
        public bool SessionCredit11 { get; set; }
        public bool SessionCredit12 { get; set; }

    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClinicalConundrums2019.Models
{
    public class ProgramRequestViewModel
    {
        public int ProgramRequestID { get; set; }

        public string ContactName { get; set; }
        [Required]
        public string ContactFirstName { get; set; }
        [Required]
        public string ContactLastName { get; set; }
        [Required]
        public string ContactPhone { get; set; }
        [Required]
        public string ContactEmail { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public string ProgramDate1 { get; set; }


        public string ProgramStartTime { get; set; }
        [Required]
        public string ProgramEndTime { get; set; }

        public string SubmittedDate { get; set; }
        [Required]
        public int RequestStatus { get; set; }
        public bool Approved { get; set; }
    }
}
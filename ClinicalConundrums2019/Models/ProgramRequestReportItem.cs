using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClinicalConundrums2019.Models
{
    public class ProgramRequestReportItem
    {

        public int ProgramRequestID { get; set; }
        public int UserID { get; set; }
        public int SponsorID { get; set; }
        public int ProgramID { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string SpeakerStatus { get; set; }
        public string ModeratorStatus { get; set; }

        public string Speaker { get; set; }
        public string Moderator { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public string ConfirmedSessionDate { get; set; }

        public string ConfirmedProgramDate { get; set; }


        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredField")]
        public string MealStartTime { get; set; }
        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredField")]
        public string ProgramStartTime { get; set; }
        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredField")]
        public string ProgramEndTime { get; set; }

        public bool SessionCredit1 { get; set; }
        public bool SessionCredit2 { get; set; }
        public bool SessionCredit3 { get; set; }
        public bool SessionCredit4 { get; set; }

        public bool SessionCredit5 { get; set; }

        public bool SessionCredit6 { get; set; }

        public string MultiSession { get; set; }

        public int ProgramSpeakerID { get; set; }


        public int? ProgramModeratorID { get; set; }

        public string VenueContacted { get; set; }

        public string LocationType { get; set; }

        public string LocationName { get; set; }

        public string LocationAddress { get; set; }

        public string LocationCity { get; set; }

        public string LocationProvince { get; set; }

        public string LocationPhoneNumber { get; set; }

        public string LocationWebsite { get; set; }


        public Decimal CostPerPerson { get; set; }


        public Decimal CostPerparticipants { get; set; }


        public string AVEquipment { get; set; }
        public string Comments { get; set; }
        public int FinalAttendance { get; set; }
        public string SubmittedDate { get; set; }
        public int RequestStatusID { get; set; }  //returning both the requeststatus ID and its description

        public string RequestStatus { get; set; }
        public bool ReadOnly { get; set; }

        public bool Approved { get; set; }
    }
}
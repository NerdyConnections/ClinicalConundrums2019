using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClinicalConundrums2019.ViewModels
{
    public class ProgramRequestCancellationVM
    {


        public int ProgramRequestID { get; set; }
        public int ProgramID { get; set; }
        public string ContactName { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }



        public string ConfirmedSessionDate { get; set; }


        public int ProgramSpeakerID { get; set; }

        public int? ProgramModeratorID { get; set; }

        public string SpeakerName { get; set; }
        public string Speaker2Name { get; set; }
        public string ModeratorName { get; set; }
        public string LocationName { get; set; }
        public string LocationAddress { get; set; }

        public string LocationCity { get; set; }

        public string LocationProvince { get; set; }
        public string LocationType { get; set; }
        public string LocationPhoneNumber { get; set; }

        public string LocationWebsite { get; set; }



        public string Comments { get; set; }
        [Required]
        public string CancellationReason { get; set; }
        public bool? CancellationRequested { get; set; }
        public string SubmittedDate { get; set; }
        public int RequestStatusID { get; set; }  //returning both the requeststatus ID and its description

        public string RequestStatus { get; set; }







    }
}
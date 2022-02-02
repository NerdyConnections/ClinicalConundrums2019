using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClinicalConundrums2019.Models
{
    public class DashboardItem
    {
        public int ProgramRequestID { get; set; }
        public int ProgramID { get; set; }
        public int UserID { get; set; }
        public string ProgramName { get; set; }
        [Required] //date when program request is submitted
        public string RequestDate { get; set; }
        //can only cancel session with 2 business days from Confirmed Session Date
        public bool CanSessionBeCancelled { get; set; }
        //if readonly display an eye if read/write display pencil
        public bool FullSessionDetails { get; set; }
        //N/A , Submit Post Session Materials 
        //Speaker Not Available for the selected date(s): Please click on the “pencil” icon and select a different speaker or change the session date 
        //Speaker Declined Participation: Please click on the “pencil” icon and select a different speaker 
        //The Venue is not available for the selected date(s): Please click on the “pencil” icon and enter new venue details
        public string MyActionItems { get; set; }
        //Under Review,Active – Regional Ethics Review Pending,Active – Regional Ethics Approved,Completed – Session Closed , Completed – Items Pending,Session Cancelled
        public string SessionStatus { get; set; }
        public string SessionInfo { get; set; }
        public int SessionStatusID { get; set; }
        public bool? Speaker1ProgramDateNA { get; set; }
        public bool? Speaker2ProgramDateNA { get; set; }
        public bool? SpeakerProgramDateNA { get; set; }
        public bool? Speaker1Declined { get; set; }
        public bool? Speaker2Declined { get; set; }
        public bool? SpeakerDeclined { get; set; }
     
        public bool? ModeratorProgramDateNA { get; set; }
        public bool? ModeratorDeclined { get; set; }
        public string VenueAvailable { get; set; }


        public string SessionDate { get; set; }
        public string Speaker { get; set; }
        public string Speaker2 { get; set; }
        public string Moderator { get; set; }
        public string Location { get; set; }
        public string Honorarium { get; set; }
        public string ThirdPartyPayments { get; set; }
        public string FinalAttendance { get; set; }
        public string EventType { get; set; }
        public string WebinarEventUrl { get; set; }
        public bool FinalAttendanceListUploaded { get; set; }
        public string FinalAttendanceListFileExt { get; set; }

        public bool SignInUploaded { get; set; }
        public string SignInFileExt { get; set; }

        public bool EvaluationUploaded { get; set; }
        public string EvaluationFileExt { get; set; }

        public bool ReadOnly { get; set; }
    }
}
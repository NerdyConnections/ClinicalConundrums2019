using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClinicalConundrums2019.Models
{
    public class SessionUpload
    {
        public int ProgramRequestID { get; set; }
        public int UserID { get; set; }
        //Evaluation Form
        //  public HttpPostedFileBase Evaluation_Uploader { get; set; }
        //full path
        public string EvaluationFullPath { get; set; }
        public string EvaluationFileName { get; set; }
        public string EvaluationFileExt { get; set; }
        public bool? EvaluationUploaded { get; set; }

        //Sign-In Sheet
        // public HttpPostedFileBase SignIn_Uploader { get; set; }
        public string SignInFullPath { get; set; }
        public string SignInFileName { get; set; }
        public string SignInFileExt { get; set; }
        public bool? SignInUploaded { get; set; }

        //Other
        // public HttpPostedFileBase Other_Uploader { get; set; }
        public string UserOtherFullPath { get; set; }
        public string UserOtherFileName { get; set; }
        public string UserOtherFileExt { get; set; }
        public bool? UserOtherUploaded { get; set; }


        public string SpeakerAgreementFullPath { get; set; }
        public string SpeakerAgreementFileName { get; set; }
        public string SpeakerAgreementFileExt { get; set; }
        public bool? SpeakerAgreementUploaded { get; set; }


        public string FinalAttendanceListFullPath { get; set; }
        public string FinalAttendanceListFileName { get; set; }
        public string FinalAttendanceListFileExt { get; set; }
        public bool? FinalAttendanceListUploaded { get; set; }
    }
}
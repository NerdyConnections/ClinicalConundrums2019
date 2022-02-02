using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClinicalConundrumsSpeaker.Models
{
    public class SpeakerProgramDetail
    {

        public int ProgramID { get; set; }
        public string ProgramName { get; set; }
        public bool DisplayProgramMaterial { get; set; }
        public bool COISlidesUploaded { get; set; }
        public string COISlidesExt { get; set; }
        public List<ConfirmedSession> MySession { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
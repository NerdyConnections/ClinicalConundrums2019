using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClinicalConundrumsSpeaker.Models
{
    public class ConfirmedSession
    {

        public int ProgramRequestID { get; set; }
        public string ConfirmedDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Location { get; set; }
    }
}
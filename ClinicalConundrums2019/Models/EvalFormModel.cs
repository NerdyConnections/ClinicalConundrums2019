using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClinicalConundrums2019.Models
{
    public class EvalFormModel
    {
        public int EventRequestID { get; set; }
        public string ProgramDate { get; set; }
        public string ProgramLocation { get; set; }
        public string Speaker1 { get; set; }
        public string Speaker2 { get; set; }
        public string Moderator { get; set; }
        public string FileName { get; set; }

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
        public bool DisplayPDF { get; set; }
        public DateTime? LastUpated { get; set; }
    }
}
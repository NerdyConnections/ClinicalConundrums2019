using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClinicalConundrums2019.Models
{
    public class COISlide
    {
        public int UserID { get; set; }
        public int ProgramID { get; set; }
        public string FileExtension { get; set; }
        public string ProgramName { get; set; }
        public string LastUpdated { get; set; }
    }
}
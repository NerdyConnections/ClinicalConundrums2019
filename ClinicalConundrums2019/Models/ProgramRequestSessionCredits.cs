using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClinicalConundrums2019.Models
{
    public class ProgramRequestSessionCredits
    {
        public int id { get; set; }
        public int ProgramRequestID { get; set; }
        public decimal SessionCreditValue { get; set; }
    }
}
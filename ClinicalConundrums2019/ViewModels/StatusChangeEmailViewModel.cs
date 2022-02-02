using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClinicalConundrums2019.ViewModels
{
    public class StatusChangeEmailViewModel
    {
        public int ProgramRequestID { get; set; }
        public string ProgramDate { get; set; }
        public string FirstName { get; set; }
        public string ProgramName { get; set; }
        public string EventID { get; set; }
        public string Email { get; set; }
    }
}
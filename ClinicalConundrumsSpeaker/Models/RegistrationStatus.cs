using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClinicalConundrumsSpeaker.Models
{
   
        public class RegistrationStatus
        {

            public int UserID { get; set; }
            public bool? COIForm { get; set; }

            public bool? PayeeForm { get; set; }
            public string COIFormExt { get; set; }



        }
    
}
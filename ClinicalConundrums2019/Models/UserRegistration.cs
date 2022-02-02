using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClinicalConundrums2019.Models
{
    public class UserRegistration
    {

        public int UserID { get; set; }
        public bool COIForm { get; set; }
        public bool PayeeForm { get; set; }
        public string COIFormExt { get; set; }
        public bool COISlides { get; set; }
        public string COISlidesExt { get; set; }

    }
}
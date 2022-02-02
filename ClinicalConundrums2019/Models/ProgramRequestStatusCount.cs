using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClinicalConundrums2019.Models
{
    public class ProgramRequestStatusCount
    {
        public int UserID { get; set; }
        public int Percent_Active { get; set; }//this is the id column of the UserInfo Table.  sometimes if the speaker has not register with us there is no UserID available, the id field become the only field to uniquely identify the person*/
        public int Percent_Attention { get; set; }
        public int Percent_Cancelled { get; set; }
        public int Percent_Completed { get; set; }
    }
}
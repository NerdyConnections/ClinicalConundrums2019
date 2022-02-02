using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClinicalConundrums2019.Util
{
    public static class Constants
    {

        //1, Sales Representative
        //2, Speaker
        //3, Moderator
        //4, Regional Manager
        //5, Brand Manager
        //6 CL Coordinator
        //7 Admin
        //8 CHE Manager
        //9 Director
        //10 National CME Lead
        //11 National Sales Lead

        public static readonly string USER = "USER";
        public static readonly string RegionalManager = "Regional Manager";
        public static readonly string SalesRepresentative = "Sales Representative";
        public static readonly string Speaker = "Speaker";
        public static readonly string Moderator = "Moderator";
        public static readonly string BrandManager = "Brand Manager";
        public static readonly string CLCoordinator = "CL Coordinator";
        public static readonly string Administrator = "Administrator";
        public static readonly string CHEManager = "CHE Manager";
        public static readonly string Director = "Director";
        public static readonly string NationalCMELead = "National CME Lead";
        public static readonly string NationalSalesLead = "National Sales Lead";
        public static readonly string BMSHead = "BMS Head";
        public static readonly string PFIZERHead = "PFIZER Head";
        public static readonly string SuperUser = "Super User";
        public static readonly string BMS = "bms";
        public static readonly string PFIZER = "pfizer";
        public static readonly string Admin = "Admin";
        public static readonly string SubmitPostSessionMaterials = "Submit Post Session Materials";
        public static readonly string SpeakerNA = "Speaker Not Available for the selected date(s): Please click on the “pencil” icon and select a different speaker or change the session date";
        public static readonly string SpeakerDeclined = "Speaker Declined Participation: Please click on the “pencil” icon and select a different speaker ";
        public static readonly string Speaker2NA = "Speaker 2 Not Available for the selected date(s): Please click on the “pencil” icon and select a different speaker 2 or change the session date";
        public static readonly string Speaker2Declined = "Speaker  2 Declined Participation: Please click on the “pencil” icon and select a different speaker 2";

        public static readonly string ModeratorNA = "Moderator Not Available for the selected date(s): Please click on the “pencil” icon and select a different moderator or change the session date";
        public static readonly string ModeratorDeclined = "Moderator Declined Participation: Please click on the “pencil” icon and select a different moderator ";

        public static readonly string VenueNA = "The Venue is not available for the selected date(s): Please click on the “pencil” icon and enter new venue details";

        public static readonly string NA = "N/A";

            public enum UserRole
            {
                Admin = 1,  //Administrator
                PI = 2,//Principal investigator
                SI = 3,//Sub Investigator
                SC = 4//Study Coordinator


            }


        
    }
}
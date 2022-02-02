using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClinicalConundrumsSpeaker.Util
{
   
        public static class Constants
        {
            public static readonly string SalesDirector = "Sales Director";
            public static readonly string SalesRep = "Sales Representative";
            public static readonly string RegionalManager = "Regional Manager";
            public static readonly string HeadOffice = "Head Office";
            public static readonly string Speaker = "Speaker";
       

            public static readonly string USER = "USER";


            public static readonly string LogoImagePath = "/Images/CPDLOGO/";

            public static readonly string NA = "N/A";
            public static readonly string SubmitPostSessionMaterials = "Submit Post Session Materials";
            public static readonly string SpeakerNA = "Speaker Not Available for the selected date(s): Please click on the “pencil” icon and select a different speaker or change the session date";
            public static readonly string SpeakerDeclined = "Speaker Declined Participation: Please click on the “pencil” icon and select a different speaker ";
            public static readonly string ModeratorNA = "Moderator Not Available for the selected date(s): Please click on the “pencil” icon and select a different moderator or change the session date";
            public static readonly string ModeratorDeclined = "Moderator Declined Participation: Please click on the “pencil” icon and select a different moderator ";

            public static readonly string VenueNA = "The Venue is not available for the selected date(s): Please click on the “pencil” icon and enter new venue details";
            public enum UserRole
            {
                SalesRep = 1,
                RegionalManager = 2,
                HeadOffice = 3,
                Speaker = 4,
                Admin = 5

            }
            public enum Therapeutic
            {
                CV = 1,
                Bone = 2,
                CVBone = 3

            }

        }
    
}
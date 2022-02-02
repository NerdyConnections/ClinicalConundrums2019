using ClinicalConundrums2019.CustomValidation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClinicalConundrums2019.Models
{
    public class EventRequestModel
    {
        public int ProgramRequestID { get; set; }
        public int UserID { get; set; }
      
        public int ProgramID { get; set; }
        public string ContactInformation { get; set; }
        public string ContactName { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string SpeakerStatus { get; set; }
        public string Speaker2Status { get; set; }
        public string ModeratorStatus { get; set; }
        //[ValidateFileSize]
        public HttpPostedFileBase sessionagenda_Uploader { get; set; }
        [VadidateProgramAgendaUpload]
        public string SessionAgendaFileName { get; set; }
        public string SessionAgendaFileExt { get; set; }
        public bool? SessionAgendaUploaded { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public string ConfirmedSessionDate { get; set; }
        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public string ProgramDate1 { get; set; }    
        [ValidateProgramDate2]
        public string ProgramDate2 { get; set; }      
        [ValidateProgramDate3]
        public string ProgramDate3 { get; set; }
        [ValidateMealStartTime]
        public string MealStartTime { get; set; }
        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public string ProgramStartTime { get; set; }
        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        [ValidateProgramEndTime]
        public string ProgramEndTime { get; set; }

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

        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public string MultiSession { get; set; }
       

        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public int ProgramSpeakerID { get; set; }
        [ValidateProgramSpeaker2ID]
        public int? ProgramSpeaker2ID { get; set; }
        public IEnumerable<SelectListItem> Speakers { get; set; }
        [ValidateProgramModeratorID]
        public int? ProgramModeratorID { get; set; }
        public IEnumerable<SelectListItem> Moderators { get; set; }

        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public string VenueContacted { get; set; }
        //[ValidateEventLocationDetails]
        [ValidateLocationType]
        public string LocationType { get; set; }
        [ValidateLocationTypeOther]
        public string LocationTypeOther { get; set; }
        //[ValidateEventLocationDetails]
        [ValidateLocationName]
        public string LocationName { get; set; }
        //[ValidateEventLocationDetails]
       // [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        [ValidateLocationAddress]
        public string LocationAddress { get; set; }
        //[ValidateEventLocationDetails]
        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public string LocationCity { get; set; }
        //[ValidateEventLocationDetails]
        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public string LocationProvince { get; set; }

        public string LocationPhoneNumber { get; set; }

        public string LocationWebsite { get; set; }
        [ValidateMealType]
        public string MealType { get; set; }
        [DataType(DataType.Currency)]
        //[ValidateEventLocationDetails]
        [ValidateCostPerPerson]
       // [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public Decimal? CostPerPerson { get; set; }

        [DataType(DataType.Currency)]
      
        // [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public Decimal CostPerparticipants { get; set; }

        [ValidateAVEquipment]
       // [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public string AVEquipment { get; set; }
        public string Comments { get; set; }

        public string SubmittedDate { get; set; }
        public int RequestStatusID { get; set; }  //returning both the requeststatus ID and its description

        public string RequestStatus { get; set; }
        public bool ReadOnly { get; set; }

        public bool Approved { get; set; }
        public bool ShowPopup { get; set; }

        public string AdminVenueConfirmed { get; set; }
        public bool AdminVenueNA { get; set; }
        public int AdminUserID { get; set; }
        public int IsAdmin { get; set; }
        public bool SpeakerChosenProgramDate { get; set; }
        public bool Speaker2ChosenProgramDate { get; set; }
        public bool ModeratorChosenProgramDate { get; set; }
        public bool FromQueryStringBySalesRep { get; set; }
       
        public string message { get; set; }

        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public string AdditionalSessionContact { get; set; }
        [ValidateAdditionalContact]
        public string AdditionalContactName { get; set; }
        [ValidateAdditionalContact]
        public string AdditionalContactPhone { get; set; }
        [ValidateAdditionalContact]
        [EmailAddress(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "EMAIL_INVALID")]
        public string AdditionalContactEmail { get; set; }
        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public string EventType { get; set; }
        [ValidateEventTypeQuestions]
        public string EventTypeQuestion1 { get; set; }
        [ValidateEventTypeQuestions]
        public string EventTypeQuestion2 { get; set; }
        [ValidateEventTypeQuestions]
        public string EventTypeQuestion3 { get; set; }
        [ValidateEventTypeQuestions]
        public string EventTypeQuestion4 { get; set; }
        [ValidateEventTypeQuestions]
        public string EventTypeQuestion5 { get; set; }
        public string RegistrationArrivalTime { get; set; }
        [Required(ErrorMessageResourceType = typeof(App_GlobalResources.Resource), ErrorMessageResourceName = "RequiredFieldWithAsterisk")]
        public string MealOption { get; set; }

    }
}
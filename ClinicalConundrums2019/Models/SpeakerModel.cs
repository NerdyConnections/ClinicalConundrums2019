using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace ClinicalConundrums2019.Models
{
    public class SpeakerModel
    {
        public int user_id { get; set; }//this is the id column of the UserInfo Table.  sometimes if the speaker has not register with us there is no UserID available, the id field become the only field to uniquely identify the person*/
        public int UserID { get; set; }
        //public string UserType { get; set; }

        //[DisplayName("Role")]
        //public int UserTypeID { get; set; }

        //public string Username { get; set; }
        //public string Password { get; set; }
        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }
        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        [DisplayName("Email Address")]
        public string EmailAddress { get; set; }

        [DisplayName("Clinic Name")]
        public string ClinicName { get; set; }
        [Required]
        public string Address { get; set; }
        //public string Address2 { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Province { get; set; }
        [Required]
        [DisplayName("Postal Code")]
        public string PostalCode { get; set; }
        //[Required(ErrorMessage = "*")]
        [Required]
        public string Phone { get; set; }
        public string Fax { get; set; }
        [Required]
        public string Specialty { get; set; }
        //public string HonariumRange { get; set; }

        public string SpeakerHonoraria { get; set; }
        public string ModeratorHonoraria { get; set; }
        public string UpcomingPrograms { get; set; }
        public string CompletedPrograms { get; set; }
        public bool COISlides { get; set; }
        public string COISlidesExt { get; set; }
        public string SubmittedDate { get; set; }
        public string Comment { get; set; }
        //public int Status { get; set; }
        //[Required]
        //public string TherapeuticID { get; set; }
        [DisplayName("Role")]
        // 0 - neither , 1 - speaker, 2 , moderator 3 speaker or moderator
        public int AssignedRole { get; set; }

        //public string TherapeuticArea { get; set; }
        //for COISlides if there is one, need the programID to get the path
        //public int ProgramID { get; set; }
        public string StatusString { get; set; }
        //public bool? Activated { get; set; }
        public string TrainingStatus { get; set; }
    }
}
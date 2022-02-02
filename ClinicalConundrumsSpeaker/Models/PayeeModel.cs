using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ClinicalConundrumsSpeaker.Models
{
    public class PayeeModel
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "*Required")]
        public string PaymentMethod { get; set; }

        [Required(ErrorMessage = "*Required")]
        public string PayableTo { get; set; }

        public string IRN { get; set; }

        [Required(ErrorMessage = "*Required")]
        public string MailingAddress1 { get; set; }

        public string MailingAddress2 { get; set; }

        public string AttentionTo { get; set; }

        [Required(ErrorMessage = "*Required")]
        public string City { get; set; }

        [Required(ErrorMessage = "*Required")]
        public string Province { get; set; }

        [Required(ErrorMessage = "*Required")]
        public string PostalCode { get; set; }


        public string TaxNumber { get; set; }

        public string Instructions { get; set; }

        
    }
}
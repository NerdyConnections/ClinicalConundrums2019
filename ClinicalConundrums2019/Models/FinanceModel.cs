using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClinicalConundrums2019.Models
{
    public class FinanceModel
    {

        public string EventInfo { get; set; }
        public Decimal CFPC { get; set; }
        public Decimal VenueFee { get; set; }
        public Decimal AVFees { get; set; }
        public Decimal SpeakerFees { get; set; }
        public Decimal Speaker2Fees { get; set; }
        public Decimal ModeratorFees { get; set; }
        public Decimal OtherFees { get; set; }
        public Decimal SubTotal { get; set; }
        public Decimal TaxesCombined { get; set; }
        public Decimal EventTotal { get; set; }

    }
}
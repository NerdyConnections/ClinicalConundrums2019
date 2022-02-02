using ClinicalConundrums2019.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClinicalConundrums2019.DAL
{
    public class PostSessionRepository: BaseRepository
    {
        public PostSessionViewModel GetPostSessionByProgramRequestID(int ProgramRequestID)
        {
            int UserID = Util.UserHelper.GetLoggedInUser().UserID;
            PostSessionViewModel psvm = new PostSessionViewModel();

            var objPr = Entities.ProgramRequests.Where(x => x.ProgramRequestID == ProgramRequestID).SingleOrDefault();

            if (objPr != null)
            {
                psvm.ProgramRequestID = ProgramRequestID;
                psvm.UserID = UserID;
                psvm.WebinarEventURL = objPr.WebinarEventURL;
                psvm.CCASent = objPr.AdminCCASent ?? false;
                psvm.VenueReceipt = objPr.AdminVenueReceipt ?? false;
                psvm.VenueNotes = objPr.AdminVenueNotes;
                psvm.CFPCDateApproved = objPr.AdminCFPCDateApproved.HasValue ? objPr.AdminCFPCDateApproved.Value.ToString("dd-MM-yyyy") : "";
                psvm.CFPCDateSubmitted = objPr.AdminCFPCDateSubmitted.HasValue ? objPr.AdminCFPCDateSubmitted.Value.ToString("dd-MM-yyyy") : "";
                psvm.CFPCFees = objPr.AdminCFPCFees;
                psvm.VenueFees = objPr.AdminVenueFees;

                psvm.AdminSessionID = objPr.AdminSessionID;
                psvm.CFPCFeeTaxes = objPr.AdminCFPCFeesTaxes;
                psvm.CFPCImplementationFees = objPr.AdminCFPCImplementationFees;
                psvm.CFPCImplementationFeesTaxes = objPr.AdminCFPCImplementationFeesTaxes;
                psvm.SpeakerExpenses = objPr.AdminSpeakerExpense;
                psvm.SpeakerExpensesTaxes = objPr.AdminSpeakerExpenseTaxes;
                psvm.Speaker2Expenses = objPr.AdminSpeaker2Expense;
                psvm.Speaker2ExpensesTaxes = objPr.AdminSpeaker2ExpenseTaxes;
                psvm.ModeratorExpenses = objPr.AdminModeratorExpense;
                psvm.ModeratorExpensesTaxes = objPr.AdminModeratorExpenseTaxes;


                psvm.AVFees = objPr.AdminAVFees;
                psvm.OtherFees = objPr.AdminOtherFees;

                psvm.VenueFeesTaxes = objPr.AdminVenueFeesTaxes;
                psvm.AVFeesTaxes = objPr.AdminAVFeesTaxes;
                psvm.OtherFeesTaxes = objPr.AdminOtherFeesTaxes;

                psvm.FinalAttendance = objPr.AdminFinalAttendance;
                psvm.SpeakerHonorium = objPr.AdminSpeakerHonorium;
                psvm.SpeakerHonoriumTaxes = objPr.AdminSpeakerHonoriumTaxes;
                psvm.SpeakerPaymentMethod = objPr.AdminSpeakerPaymentMethod;
                psvm.SpeakerPaymentSentDate = objPr.AdminSpeakerPaymentSentDate.HasValue ? objPr.AdminSpeakerPaymentSentDate.Value.ToString("dd-MM-yyyy") : "";
                psvm.Speaker2Honorium = objPr.AdminSpeaker2Honorium;
                psvm.Speaker2HonoriumTaxes = objPr.AdminSpeaker2HonoriumTaxes;
                psvm.Speaker2PaymentMethod = objPr.AdminSpeaker2PaymentMethod;
                psvm.Speaker2PaymentSentDate = objPr.AdminSpeaker2PaymentSentDate.HasValue ? objPr.AdminSpeaker2PaymentSentDate.Value.ToString("dd-MM-yyyy") : "";
                psvm.ModeratorHonorium = objPr.AdminModeratorHonorium;
                psvm.ModeratorHonoriumTaxes = objPr.AdminModeratorHonoriumTaxes;
                psvm.ModeratorPaymentMethod = objPr.AdminModeratorPaymentMethod;
                psvm.ModeratorPaymentSentDate = objPr.AdminModeratorPaymentSentDate.HasValue ? objPr.AdminModeratorPaymentSentDate.Value.ToString("dd-MM-yyyy") : "";


            }
            return psvm;

        }
    }
}
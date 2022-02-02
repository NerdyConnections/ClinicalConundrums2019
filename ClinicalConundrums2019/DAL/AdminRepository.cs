using ClinicalConundrum2019.Data;
using ClinicalConundrums2019.Models;
using ClinicalConundrums2019.Util;
using ClinicalConundrums2019.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace ClinicalConundrums2019.DAL
{
    public class AdminRepository: BaseRepository
    {

        public ClinicalConundrums2019.Models.UserRegistration GetUserRegistration(int UserID)
        {//usertype 2 for speaker
         //usertype 3 for moderator


            var val = Entities.UserRegistrations.Where(x => x.UserID == UserID).Select(pr =>
                     new Models.UserRegistration
                     {

                         UserID = UserID,
                         COIForm = pr.COIForm ?? false,
                         PayeeForm = pr.PayeeForm ?? false,
                         COIFormExt = pr.COIFormExt,






                     }).SingleOrDefault();

            if (val == null)
            {

                val = new Models.UserRegistration()
                {
                    UserID = UserID,
                    COIForm = false,
                    PayeeForm = false,
                    COISlides = false
                };
            }

            return val;



        }
        public bool UpdateCOIForm(int UserID, string COIFormExt)
        {

            try
            {
                var val = Entities.UserRegistrations.Where(x => x.UserID == UserID).SingleOrDefault();
                if (val != null)
                {

                    val.COIForm = true;
                    val.COIFormExt = COIFormExt;
                    Entities.SaveChanges();
                    return true;
                }
                else
                {
                    ClinicalConundrum2019.Data.UserRegistration objUserRegistration = new ClinicalConundrum2019.Data.UserRegistration();
                    objUserRegistration.UserID = UserID;
                    objUserRegistration.COIForm = true;
                    objUserRegistration.COIFormExt = COIFormExt;
                    Entities.UserRegistrations.Add(objUserRegistration);
                    Entities.SaveChanges();
                    return true;



                }
            }
            catch (Exception e)
            {

                return false;
            }


        }
        public void SavePostSession(PostSessionViewModel psvm)
        {
            DateTime? dt = null;

            var objpr = (from v in Entities.ProgramRequests

                         where v.ProgramRequestID == psvm.ProgramRequestID
                         select v).FirstOrDefault();
            if (objpr != null)
            {



                objpr.ProgramRequestID = psvm.ProgramRequestID;
                objpr.WebinarEventURL = psvm.WebinarEventURL;
                objpr.AdminCCASent = psvm.CCASent;
                objpr.AdminVenueReceipt = psvm.VenueReceipt;
                objpr.AdminCFPCDateApproved = !(string.IsNullOrEmpty(psvm.CFPCDateApproved)) ? (DateTime.ParseExact(psvm.CFPCDateApproved, "dd/MM/yyyy", null)) : dt;
                objpr.AdminCFPCDateSubmitted = !(string.IsNullOrEmpty(psvm.CFPCDateSubmitted)) ? (DateTime.ParseExact(psvm.CFPCDateSubmitted, "dd/MM/yyyy", null)) : dt;
                objpr.AdminVenueNotes = psvm.VenueNotes;

                objpr.AdminCFPCFees = psvm.CFPCFees;
                objpr.AdminCFPCFeesTaxes = psvm.CFPCFeeTaxes;
                objpr.AdminCFPCImplementationFees = psvm.CFPCImplementationFees;
                objpr.AdminCFPCImplementationFeesTaxes = psvm.CFPCImplementationFeesTaxes;

                objpr.AdminVenueFees = psvm.VenueFees;
                objpr.AdminVenueFeesTaxes = psvm.VenueFeesTaxes;
                objpr.AdminAVFeesTaxes = psvm.AVFeesTaxes;
                objpr.AdminOtherFeesTaxes = psvm.OtherFeesTaxes;

                objpr.AdminAVFees = psvm.AVFees;
                objpr.AdminOtherFees = psvm.OtherFees;
                objpr.AdminFinalAttendance = psvm.FinalAttendance;
                //signin sheet
                objpr.AdminSessionID = psvm.AdminSessionID;
                objpr.AdminSpeakerHonorium = psvm.SpeakerHonorium;
                objpr.AdminSpeakerHonoriumTaxes = psvm.SpeakerHonoriumTaxes;
                objpr.AdminSpeakerExpense = psvm.SpeakerExpenses;
                objpr.AdminSpeakerExpenseTaxes = psvm.SpeakerExpensesTaxes;
                objpr.AdminSpeaker2Honorium = psvm.Speaker2Honorium;
                objpr.AdminSpeaker2HonoriumTaxes = psvm.Speaker2HonoriumTaxes;
                objpr.AdminSpeaker2Expense = psvm.Speaker2Expenses;
                objpr.AdminSpeaker2ExpenseTaxes = psvm.Speaker2ExpensesTaxes;
                objpr.AdminModeratorExpense = psvm.ModeratorExpenses;
                objpr.AdminModeratorExpenseTaxes = psvm.ModeratorExpensesTaxes;


                objpr.AdminSpeakerPaymentMethod = "Cheque";
                objpr.AdminSpeakerPaymentSentDate = !(string.IsNullOrEmpty(psvm.SpeakerPaymentSentDate)) ? DateTime.ParseExact(psvm.SpeakerPaymentSentDate, "dd/MM/yyyy", null) : dt;
                objpr.AdminSpeaker2PaymentMethod = "Cheque";
                objpr.AdminSpeaker2PaymentSentDate = !(string.IsNullOrEmpty(psvm.Speaker2PaymentSentDate)) ? DateTime.ParseExact(psvm.Speaker2PaymentSentDate, "dd/MM/yyyy", null) : dt;

                objpr.AdminModeratorHonorium = psvm.ModeratorHonorium;
                objpr.AdminModeratorHonoriumTaxes = psvm.ModeratorHonoriumTaxes;


                objpr.AdminModeratorPaymentMethod = "Cheque";
                objpr.AdminModeratorPaymentSentDate = !(string.IsNullOrEmpty(psvm.ModeratorPaymentSentDate)) ? DateTime.ParseExact(psvm.ModeratorPaymentSentDate, "dd/MM/yyyy", null) : dt;




                objpr.AdminEditDate = DateTime.Now;
                objpr.AdminUserID = psvm.UserID;

                Entities.SaveChanges();
                //PatientID = objTempMAF.PatientID;
            }

        }

        public PayeeModel GetPayeeByUserID(int UserID)
        {

            PayeeModel payee = new PayeeModel();
            var query = Entities.PayeeInfoes.Where(p => p.UserID == UserID).SingleOrDefault();


            if (query != null)
            {

                payee.UserId = query.UserID;
                payee.PaymentMethod = query.PaymentMethod;
                payee.PayableTo = query.ChequePayableTo;
                payee.IRN = query.InternalRefNum;
                payee.MailingAddress1 = query.MailingAddr1;
                payee.MailingAddress2 = query.MailingAddr2;
                payee.AttentionTo = query.AttentionTo;
                payee.City = query.City;
                payee.Province = query.ProvinceID;
                payee.PostalCode = query.PostalCode;
                payee.TaxNumber = query.TaxNumber;
                payee.Instructions = query.AdditionalInstructions;

            }
            else
            {
                payee.UserId = UserID;

            }

            return payee;
        }
        public void UpdatePayeeInformation(int userid)
        {
            ClinicalConundrum2019.Data.UserRegistration UserReg = new ClinicalConundrum2019.Data.UserRegistration();

            var user = Entities.UserRegistrations.Where(x => x.UserID == userid).SingleOrDefault();

            if (user != null)
            {
                user.PayeeForm = true;
                Entities.SaveChanges();

            }

            else
            {
                UserReg.UserID = userid;
                UserReg.PayeeForm = true;
                Entities.UserRegistrations.Add(UserReg);
                Entities.SaveChanges();

            }

        }
        public bool UpdatePayee(PayeeModel payee)
        {


            var payeeInfo = Entities.PayeeInfoes.Where(p => p.UserID == payee.UserId).SingleOrDefault();

            try
            {
                if (payeeInfo != null)
                {

                    payeeInfo.UserID = payee.UserId.Value;
                    payeeInfo.PaymentMethod = payee.PaymentMethod;
                    payeeInfo.ChequePayableTo = payee.PayableTo;
                    payeeInfo.InternalRefNum = payee.IRN;
                    payeeInfo.MailingAddr1 = payee.MailingAddress1;
                    payeeInfo.MailingAddr2 = payee.MailingAddress2;
                    payeeInfo.AttentionTo = payee.AttentionTo;
                    payeeInfo.City = payee.City;
                    payeeInfo.ProvinceID = payee.Province;
                    payeeInfo.PostalCode = payee.PostalCode;
                    payeeInfo.TaxNumber = payee.TaxNumber;
                    payeeInfo.AdditionalInstructions = payee.Instructions;
                    payeeInfo.LastUpdated = DateTime.Now;

                    Entities.SaveChanges();
                    return true;
                }
                else
                {
                    PayeeInfo objPayee = new PayeeInfo();
                    objPayee.UserID = payee.UserId ?? 0;
                    objPayee.PaymentMethod = payee.PaymentMethod;
                    objPayee.ChequePayableTo = payee.PayableTo;
                    objPayee.InternalRefNum = payee.IRN;
                    objPayee.MailingAddr1 = payee.MailingAddress1;
                    objPayee.MailingAddr2 = payee.MailingAddress2;
                    objPayee.AttentionTo = payee.AttentionTo;
                    objPayee.City = payee.City;
                    objPayee.ProvinceID = payee.Province;
                    objPayee.PostalCode = payee.PostalCode;
                    objPayee.TaxNumber = payee.TaxNumber;
                    objPayee.AdditionalInstructions = payee.Instructions;
                    objPayee.LastUpdated = DateTime.Now;
                    Entities.PayeeInfoes.Add(objPayee);
                    UpdatePayeeInformation(payee.UserId ?? 0);//update UserRegistration table
                    Entities.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {

                return false;
            }

        }

        public StatusChangeEmailViewModel GetStatusChangeByAdminEmail(int ProgramRequestID)
        {

            StatusChangeEmailViewModel sc = new StatusChangeEmailViewModel();

            var val = Entities.ProgramRequests.Where(x => x.ProgramRequestID == ProgramRequestID).FirstOrDefault();

            if (val != null)
            {

                sc.FirstName = val.ContactFirstName;
                sc.ProgramDate = val.ConfirmedSessionDate.HasValue ? val.ConfirmedSessionDate.Value.ToString("MMMM dd, yyyy") : "";
                sc.ProgramName = Entities.Programs.Where(x => x.ProgramID == val.ProgramID).Select(x => x.ProgramName).SingleOrDefault();
                sc.EventID = val.AdminSessionID;
                sc.Email = Entities.UserInfoes.Where(x => x.UserID == val.UserID).Select(x => x.EmailAddress).SingleOrDefault();

            }

            return sc;


        }


        public bool CheckConfirmedSessionDate(int ProgramRequestID)
        {
            bool retVal = false;

            var val = Entities.ProgramRequests.Where(x => x.ProgramRequestID == ProgramRequestID).FirstOrDefault();

            if (val != null)
            {

                if (val.ConfirmedSessionDate.HasValue)
                {
                    retVal = true;

                }

            }


            return retVal;
        }

        public bool CheckAdminSessionID(int ProgramRequestID)
        {
            bool retVal = false;

            var val = Entities.ProgramRequests.Where(x => x.ProgramRequestID == ProgramRequestID).FirstOrDefault();

            if (val != null)
            {

                if (!(string.IsNullOrEmpty(val.AdminSessionID)))
                {
                    retVal = true;

                }

            }


            return retVal;
        }

        public List<ProgramListViewModel> GetProgramList(int UserID)
        {
           
            List<ProgramListViewModel> list = new List<ProgramListViewModel>();

            //  var TherapeuticId = Entities.UserInfoes.Where(x => x.UserID == UserID).Select(x => x.TherapeuticID).SingleOrDefault();

            //  var ProgramList = Entities.TherapeuticPrograms.Where(x => x.TherapeuticID == TherapeuticId).ToList();

            var ProgramList = Entities.Programs.ToList();

            foreach (var item in ProgramList)
            {

                list.Add(new ProgramListViewModel()
                {
                    Id = item.ProgramID,
                    Name = Entities.Programs.Where(x => x.ProgramID == item.ProgramID).Select(x => x.ProgramName).FirstOrDefault()

                });

            }



            return list;


        }
        public List<ProgramRequestReportItem> GetProgramRequestReport()
        {

            List<ProgramRequestReportItem> list = new List<ProgramRequestReportItem>();

            //list = (from item in Entities.ProgramRequests
            //           select new ProgramRequestReportItem
            //           {
            //               ProgramRequestID = item.ProgramRequestID,
            //               SubmittedDate = item.SubmittedDate.HasValue ? item.ConfirmedSessionDate.Value.ToString("MMMM dd, yyyy") : "",
            //               ContactFirstName = item.ContactFirstName,
            //               ContactLastName = item.ContactLastName,
            //               RequestStatus = item.RequestStatusLookup.RequestStatusDescription,
            //               ConfirmedProgramDate = item.ConfirmedSessionDate.HasValue ? item.ConfirmedSessionDate.Value.ToString("MMMM dd, yyyy") : "",

            //               LocationName = item.LocationName,
            //               LocationAddress = item.LocationAddress,
            //               LocationCity = item.LocationCity,
            //               LocationProvince = item.LocationProvince,
            //               FinalAttendance = item.AdminFinalAttendance ?? 0

            //           }).ToList();
            try
            {
                var ProgramRequestList = Entities.ProgramRequests.Where(x => x.ProgramID != null && x.UserID != null).ToList();
                string CompanyName=string.Empty;

                foreach (var item in ProgramRequestList)
                {
                    if (item.UserID != null)
                    {
                        var userInfo = Entities.UserInfoes.Where(x => x.UserID == item.UserID).FirstOrDefault();
                        if (userInfo != null)
                        {

                            CompanyName = userInfo.ClinicName;
                        }
                    }

                    list.Add(new ProgramRequestReportItem()
                    {
                        ProgramRequestID = item.ProgramRequestID,
                        SubmittedDate = item.ConfirmedSessionDate != null ? item.ConfirmedSessionDate.Value.ToString("MM/dd/yyyy") : "",
                        ContactFirstName = item.ContactFirstName,
                        ContactLastName = item.ContactLastName,
                        RequestStatus = item.RequestStatus.HasValue ? item.RequestStatusLookup.RequestStatusDescription : "",
                        ConfirmedProgramDate = (item.ConfirmedSessionDate != null) ? item.ConfirmedSessionDate.Value.ToString("MMMM dd, yyyy") : "",
                        CompanyName = CompanyName,
                        LocationName = item.LocationName,
                        LocationAddress = item.LocationAddress,
                        Speaker = (item.SpeakerInfo != null) ? item.SpeakerInfo.FirstName + "," + item.SpeakerInfo.LastName : "",
                        Moderator = (item.ModeratorInfo != null) ? item.ModeratorInfo.FirstName + "," + item.ModeratorInfo.LastName : "",
                        LocationCity = item.LocationCity,
                        LocationProvince = item.LocationProvince,
                        FinalAttendance = item.AdminFinalAttendance ?? 0



                    });

                }



                return list;
            }
            catch (Exception e)
            {
                UserHelper.WriteToLog("Error Message:" + e.Message);
                var st = new StackTrace(e, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();

                UserHelper.WriteToLog("Error Location:" + line);
                return list;
            }


        }
        public List<UserModel> GetUserReport()
        {

            List<UserModel> list = new List<UserModel>();

            //list = (from item in Entities.ProgramRequests
            //           select new ProgramRequestReportItem
            //           {
            //               ProgramRequestID = item.ProgramRequestID,
            //               SubmittedDate = item.SubmittedDate.HasValue ? item.ConfirmedSessionDate.Value.ToString("MMMM dd, yyyy") : "",
            //               ContactFirstName = item.ContactFirstName,
            //               ContactLastName = item.ContactLastName,
            //               RequestStatus = item.RequestStatusLookup.RequestStatusDescription,
            //               ConfirmedProgramDate = item.ConfirmedSessionDate.HasValue ? item.ConfirmedSessionDate.Value.ToString("MMMM dd, yyyy") : "",

            //               LocationName = item.LocationName,
            //               LocationAddress = item.LocationAddress,
            //               LocationCity = item.LocationCity,
            //               LocationProvince = item.LocationProvince,
            //               FinalAttendance = item.AdminFinalAttendance ?? 0

            //           }).ToList();
            try
            {
                var UserList = Entities.UserInfoes.Where(x=>x.id > 455).ToList();
                string CompanyName = string.Empty;

                foreach (var item in UserList)
                {
                    //if (item.UserID != null)
                    //{
                    //    var userInfo = Entities.UserInfoes.Where(x => x.UserID == item.UserID).FirstOrDefault();
                    //    if (userInfo != null)
                    //    {

                    //        CompanyName = userInfo.ClinicName;
                    //    }
                    //}
                    //ws.Cells["A1"].Value = "ID";
                    //ws.Cells["B1"].Value = "UserID";
                    //ws.Cells["C1"].Value = "UserType";
                    //ws.Cells["D1"].Value = "TerritoryID";
                    //ws.Cells["E1"].Value = "RepID";
                    //ws.Cells["F1"].Value = "First Name";
                    //ws.Cells["G1"].Value = "Last Name";
                    //ws.Cells["H1"].Value = "Email Address";
                    //ws.Cells["I1"].Value = "Company";
                    //ws.Cells["J1"].Value = "Address";
                    //ws.Cells["K1"].Value = "City";
                    //ws.Cells["L1"].Value = "Province";
                    //ws.Cells["M1"].Value = "Phone";
                    //ws.Cells["N1"].Value = "Activated";
                    list.Add(new UserModel()
                    {
                        ID = item.id,
                        UserID = item.UserID??0,
                        UserType = item.UserTypeLookUp.Description,
                        TerritoryID = item.TerritoryID,
                        RepID = item.RepID,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        EmailAddress = item.EmailAddress,
                        ClinicName = item.ClinicName,
                        Address = item.Address,
                        City = item.City,
                        Province = item.Province,
                        Phone = item.Phone,
                        Activated = (item.UserID==null) ? false : true



                    });

                }



                return list;
            }
            catch (Exception e)
            {
                UserHelper.WriteToLog("Error Message:" + e.Message);
                var st = new StackTrace(e, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();

                UserHelper.WriteToLog("Error Location:" + line);
                return list;
            }


        }

    }
}
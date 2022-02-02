using ClinicalConundrums2019.DAL;
using ClinicalConundrums2019.Models;
using ClinicalConundrums2019.Util;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClinicalConundrums2019.Controllers
{
    public class DynamicPDFController : Controller
    {
        private const string SignInSheetPATH = "~/PDF/SignInSheet/";
        private const string COAPATH = "~/PDF/COA/";//Certificate of attendance
        // GET: DynamicPDF

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SignInSheet(int ProgramRequestID)
        {
            ProgramRequestInfoModel prim = new ProgramRequestInfoModel();
            ProgramRepository progRepo = new ProgramRepository();

            prim = progRepo.GetProgramRequestInfo(ProgramRequestID);

            

            byte[] bytes = GetSignInSheetPDF(prim);

            if (bytes == null)
            {
                return new ContentResult
                {
                    Content = "Could not generate Sign In Sheet Pdf. Please contact info@clinicalconundrums2019.com for assistance"
                };
                //UIHelper.Alert(this, "Could not generate Pdf");
                //return;
            }
            else
            {
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.BinaryWrite(bytes);
                //this is how you return a byte array to be a pdf.
                MemoryStream ms = new MemoryStream(bytes);

                return new FileStreamResult(ms, "application/pdf");
                // return Redirect(prim.filename);
            }

            //return Redirect(fileName);
        }
        public ActionResult COA(int ProgramRequestID)
        {
            ProgramRequestInfoModel prim = new ProgramRequestInfoModel();
            ProgramRepository progRepo = new ProgramRepository();

            prim = progRepo.GetProgramRequestInfo(ProgramRequestID);



            byte[] bytes = GetCertificateOfAttendancePDF(prim);

            if (bytes == null)
            {
                return new ContentResult
                {
                    Content = "Could not generate Sign In Sheet Pdf. Please contact info@clinicalconundrums2019.com for assistance"
                };
                //UIHelper.Alert(this, "Could not generate Pdf");
                //return;
            }
            else
            {
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";
                Response.BinaryWrite(bytes);
                //this is how you return a byte array to be a pdf.
                MemoryStream ms = new MemoryStream(bytes);

                return new FileStreamResult(ms, "application/pdf");
                // return Redirect(prim.filename);
            }

            //return Redirect(fileName);
        }
        public byte[] GetSignInSheetPDF(ProgramRequestInfoModel prim)
        {

            //  List<Invitee> lstInvitee = new List<Invitee>();

            // InviteeRepository invRepos = new InviteeRepository();

            try
            {

                GenPdf genpdf = new GenPdf(Server.MapPath(SignInSheetPATH));
                System.DateTime ThisMoment = System.DateTime.Now;
               // String RandomModifer = ThisMoment.Year + "_" + ThisMoment.Month + "_" + ThisMoment.Day + "_" + ThisMoment.Hour + "_" + ThisMoment.Minute + "_" + ThisMoment.Second + "_" + ThisMoment.Millisecond;

                string formname = "SignInSheetTemplate.pdf";

                //  List<Invitee> invList = new List<Invitee>();

                //List<int> lstIDInt = new List<int>();

                // foreach (string id in Request.QueryString["physlst"].Split(",".ToCharArray()).ToList())
                // {
                //     lstIDInt.Add(Int32.Parse(id));
                //  }


                //  invList = invRepos.GetInviteeData(lstIDInt);

                genpdf.Create();

                //f1
                genpdf.AddForm(formname);
                //string nameA = doc.LastName + ", ";

                string fullName = string.Empty;
                string lastName = string.Empty;

                string speakername = string.Empty;
                if (string.IsNullOrEmpty(prim.Speaker2FirstName))
                    speakername = "Dr. " + prim.Speaker1FirstName + " " + prim.Speaker1LastName;
                else
                    speakername = "Dr. " + prim.Speaker1FirstName + " " + prim.Speaker1LastName + " " + "Dr. " + prim.Speaker2FirstName + " " + prim.Speaker2LastName;


                //genpdf.AddField("Fax", inv.Fax, 0);
                genpdf.AddField("Program Date", prim.ConfirmedSessionDate, 0);
                genpdf.AddField("City and Province", prim.VenueCity + ", " + prim.VenueProvince, 0);
                genpdf.AddField("Speaker Name", speakername, 0);
                if (!String.IsNullOrEmpty(prim.ModeratorFirstName))
                    genpdf.AddField("Moderator Name", "Dr. " + prim.ModeratorFirstName + " " + prim.ModeratorLastName, 0);
                genpdf.Save();
                genpdf.FinalizeForm(true);
                prim.filename = SignInSheetPATH + formname;


                return genpdf.Close();
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

            //GenPdf genpdf = new GenPdf(Server.MapPath(PATH));
            //List<Physician> doclist = new List<Physician>();
            //genpdf.Create();
            //string idlst = Request.QueryString["physlst"];
            //string[] idarr = idlst.Split(",".ToCharArray());
            //doclist = Physician.GetPhysicianList(idarr);
            //foreach (Physician doc in doclist)  //loop by physician
            //{   //f1
            //    genpdf.AddForm(formname);
            //    //string nameA = doc.LastName + ", ";

            //    //string nameB = nameA;
            //    string id = doc.PhysicianID;
            //    string regid = doc.PhysicianID;
            //    string name = doc.FirstName + " " + doc.LastName + ",";
            //    //genpdf.AddField("Last Name", nameA, 0);
            //    genpdf.AddField("name", name, 0);
            //    genpdf.AddField("id", id, 0);
            //    genpdf.AddField("regid", regid, 0);
            //    // genpdf.Save();
            //    genpdf.FinalizeForm(true);
            //}
            //return genpdf.Close();

        }
        public byte[] GetCertificateOfAttendancePDF(ProgramRequestInfoModel prim)
        {

            //  List<Invitee> lstInvitee = new List<Invitee>();

            // InviteeRepository invRepos = new InviteeRepository();

            try
            {

                GenPdf genpdf = new GenPdf(Server.MapPath(COAPATH));
                System.DateTime ThisMoment = System.DateTime.Now;
                // String RandomModifer = ThisMoment.Year + "_" + ThisMoment.Month + "_" + ThisMoment.Day + "_" + ThisMoment.Hour + "_" + ThisMoment.Minute + "_" + ThisMoment.Second + "_" + ThisMoment.Millisecond;

                string formname = "COATemplate.pdf";

                //  List<Invitee> invList = new List<Invitee>();

                //List<int> lstIDInt = new List<int>();

                // foreach (string id in Request.QueryString["physlst"].Split(",".ToCharArray()).ToList())
                // {
                //     lstIDInt.Add(Int32.Parse(id));
                //  }


                //  invList = invRepos.GetInviteeData(lstIDInt);

                genpdf.Create();

                //f1
                genpdf.AddForm(formname);
                //string nameA = doc.LastName + ", ";

                string fullName = string.Empty;
                string lastName = string.Empty;

             
                //genpdf.AddField("Fax", inv.Fax, 0);
                genpdf.AddField("Session ID", prim.SessionID, 0);
                genpdf.AddField("Prov", prim.Province, 0);
                genpdf.AddField("Credits", prim.TotalSessionCredits, 0);
                if (!String.IsNullOrEmpty(prim.ConfirmedSessionDate))
                    genpdf.AddField("Program Date",  prim.ConfirmedSessionDate, 0);
                

                    genpdf.AddField("Venu, City, Prov",  prim.VenueName + " " + prim.VenueCity + " " + prim.VenueProvince, 0);
                genpdf.Save();
                genpdf.FinalizeForm(true);
                prim.filename = COAPATH + formname;


                return genpdf.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

            //GenPdf genpdf = new GenPdf(Server.MapPath(PATH));
            //List<Physician> doclist = new List<Physician>();
            //genpdf.Create();
            //string idlst = Request.QueryString["physlst"];
            //string[] idarr = idlst.Split(",".ToCharArray());
            //doclist = Physician.GetPhysicianList(idarr);
            //foreach (Physician doc in doclist)  //loop by physician
            //{   //f1
            //    genpdf.AddForm(formname);
            //    //string nameA = doc.LastName + ", ";

            //    //string nameB = nameA;
            //    string id = doc.PhysicianID;
            //    string regid = doc.PhysicianID;
            //    string name = doc.FirstName + " " + doc.LastName + ",";
            //    //genpdf.AddField("Last Name", nameA, 0);
            //    genpdf.AddField("name", name, 0);
            //    genpdf.AddField("id", id, 0);
            //    genpdf.AddField("regid", regid, 0);
            //    // genpdf.Save();
            //    genpdf.FinalizeForm(true);
            //}
            //return genpdf.Close();

        }

        [HttpGet]
        public ActionResult NationalInvitation(int ProgramRequestID)
        {
            int CaseCounter = 1;
            string fileName = string.Empty; //contains the file name of the resultant pdf
            bool HasBorder = false;  //set to false when development is completed. 
            System.DateTime ThisMoment = System.DateTime.Now;
            String RandomModifer = ThisMoment.Year + "_" + ThisMoment.Month + "_" + ThisMoment.Day + "_" + ThisMoment.Hour + "_" + ThisMoment.Minute + "_" + ThisMoment.Second + "_" + ThisMoment.Millisecond;
            string path = Server.MapPath("/PDF/NationalInvitation");
            string imagepath = Server.MapPath("/Images/NationalInvitation");
            var TargetDocument = new Document(PageSize.A4, 18, 18, 18, 18);
            //ViewBag.ProgramRequestStatusCounts = UserHelper.GetProgramRequestStatusCounts(Convert.ToInt32(Session["ProgramID"]));
            PdfWriter writer=PdfWriter.GetInstance(TargetDocument, new FileStream(path + "/NationalInvitation" + RandomModifer + ".pdf", FileMode.Create));
            BaseFont TimesBoldBase = BaseFont.CreateFont(BaseFont.TIMES_BOLD, BaseFont.CP1252, false);
            BaseFont COURIERBase = BaseFont.CreateFont(BaseFont.COURIER_BOLD, BaseFont.CP1252, false);
            iTextSharp.text.Font BigArial = FontFactory.GetFont("Arial", BaseFont.CP1252, BaseFont.EMBEDDED, 9, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font SmallArial = FontFactory.GetFont("Arial", BaseFont.CP1252, BaseFont.EMBEDDED, 6, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
         //   iTextSharp.text.Font ArialBold14 = FontFactory.GetFont("Arial Narrow", BaseFont.CP1252, BaseFont.EMBEDDED, 14, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
            iTextSharp.text.Font ArialBold14 = FontFactory.GetFont("Arial Narrow", BaseFont.CP1252, BaseFont.EMBEDDED, 14, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
            BaseFont HelveticaBase = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
            iTextSharp.text.Font Helvetica = new iTextSharp.text.Font(HelveticaBase, 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font DefaultFont = Helvetica;
            DashboardRepository DBRepo = new DashboardRepository();
            ProgramRequestInfoModel prim = new ProgramRequestInfoModel();
            ProgramRepository progRepo = new ProgramRepository();

            prim = progRepo.GetProgramRequestInfo(ProgramRequestID);
            iTextSharp.text.Font TimesBold = new iTextSharp.text.Font(TimesBoldBase, 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);


            try
            {
                //set up top image
                //set up top image
                TargetDocument.Open();
                TargetDocument.NewPage();
                iTextSharp.text.Image EvalTop1 = iTextSharp.text.Image.GetInstance(imagepath + "/Header1.jpg");

                EvalTop1.ScalePercent(54f);
                TargetDocument.Add(EvalTop1);
                //end of set up top image

                float[] widths = new float[] { 1f, 1f, 1f, 1f };//equal width for now
                PdfPTable DateLocation = new PdfPTable(4);
                DateLocation.KeepTogether = true;
                DateLocation.TotalWidth = 523f;
                DateLocation.SetWidths(widths);
                DateLocation.LockedWidth = true;
                DateLocation.SpacingBefore = 10f;


                DateLocation.SpacingAfter = 10f;
                DateLocation.HorizontalAlignment = 0;//0=Left, 1=Centre, 2=Right

                //cell 1
                iTextSharp.text.Image DateLabel = iTextSharp.text.Image.GetInstance(imagepath + "/DateLabel.jpg");
                DateLabel.ScalePercent(30f);
                PdfPCell DateLabelCell = new PdfPCell(DateLabel);
                //cell 2
                PdfPCell DateCell = new PdfPCell(new Phrase(prim.ConfirmedSessionDate, DefaultFont));


                DateCell.HorizontalAlignment = 1;
                DateCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                DateCell.BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255);
                //cell 3
                PdfPCell RowOneFiller1 = new PdfPCell();
                PdfPCell RowOneFiller2 = new PdfPCell();


                iTextSharp.text.Image LocationLabel = iTextSharp.text.Image.GetInstance(imagepath + "/LocationLabel.jpg");
                LocationLabel.ScalePercent(30f);
                PdfPCell LocationLabelCell = new PdfPCell(LocationLabel);
                LocationLabelCell.HorizontalAlignment = 0; //alignleft
                                                           //cell 4
                                                           //iTextSharp.text.Image SpaceFieldLoc = iTextSharp.text.Image.GetInstance(imagepath + "/SpaceField.jpg");
                                                           //DateLabel.ScalePercent(30f);
                                                           //PdfPCell SpaceFieldCellLocation = new PdfPCell(SpaceFieldLoc);

                PdfPCell LocationCell = new PdfPCell(new Phrase(prim.VenueName, DefaultFont));
                LocationCell.HorizontalAlignment = 0;
                LocationCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                LocationCell.BackgroundColor = new BaseColor(255, 255, 255);

                PdfPCell RowTwoFiller1 = new  PdfPCell(new Phrase(prim.VenueAddress, DefaultFont));
                RowTwoFiller1.Colspan = 2;
                RowTwoFiller1.VerticalAlignment = Element.ALIGN_MIDDLE;
               // PdfPCell RowTwoFiller2 = new PdfPCell(new Phrase(""));
               // PdfPCell RowTwoFiller2 = new  PdfPCell(new Phrase(prim.VenueCity + " " + prim.VenueProvince, new iTextSharp.text.Font(HelveticaBase, 12f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
               // RowTwoFiller2.VerticalAlignment = Element.ALIGN_MIDDLE;

                //program start time cell
                iTextSharp.text.Image ProgramStartLabel = iTextSharp.text.Image.GetInstance(imagepath + "/ProgramStartLabel.jpg");
                ProgramStartLabel.ScalePercent(30f);
                PdfPCell ProgramStartLabelCell = new PdfPCell(ProgramStartLabel);
                
                PdfPCell ProgramStartCell = new PdfPCell(new Phrase(prim.programstarttime, DefaultFont));


                ProgramStartCell.HorizontalAlignment = 1;
                ProgramStartCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                ProgramStartCell.BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255);


                //program end time cell
                iTextSharp.text.Image ProgramEndLabel = iTextSharp.text.Image.GetInstance(imagepath + "/ProgramEndLabel.jpg");
                ProgramEndLabel.ScalePercent(30f);
                PdfPCell ProgramEndLabelCell = new PdfPCell(ProgramEndLabel);

                PdfPCell ProgramEndCell = new PdfPCell(new Phrase(prim.programendtime, DefaultFont));


                ProgramEndCell.HorizontalAlignment = 1;
                ProgramEndCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                ProgramEndCell.BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255);

                //speaker 1
               
                iTextSharp.text.Image Speaker1Label = iTextSharp.text.Image.GetInstance(imagepath + "/SpeakerLabel.jpg");
                Speaker1Label.ScalePercent(30f);
                PdfPCell Speaker1LabelCell = new PdfPCell(Speaker1Label);

                PdfPCell Speaker1Cell = new PdfPCell(new Phrase("Dr. "  + prim.Speaker1FirstName + " " + prim.Speaker1LastName, DefaultFont));


                Speaker1Cell.HorizontalAlignment = 0;
                Speaker1Cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                Speaker1Cell.BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255);

              
                //speaker 2 if available
                string speaker2FirstName = prim.Speaker2FirstName;
                PdfPCell Speaker2Filler1;
                PdfPCell Speaker2Filler2;
                PdfPCell Speaker2Filler3;
                PdfPCell Speaker2Cell;
              
              

                //Registration Comments
                iTextSharp.text.Image RegistrationCommentsLabel = iTextSharp.text.Image.GetInstance(imagepath + "/RegistrationCommentsLabel.jpg");
                RegistrationCommentsLabel.ScalePercent(30f);
                PdfPCell RegistrationCommentsLabelCell = new PdfPCell(RegistrationCommentsLabel);

                PdfPCell RegistrationCommentsCell1 = new PdfPCell(new Phrase(""));
                PdfPCell RegistrationCommentsCell2 = new PdfPCell(new Phrase(""));

                if (HasBorder == false)
                {
                    RowOneFiller1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    RowOneFiller2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    RowTwoFiller1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    //RowTwoFiller2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    ProgramStartLabelCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    ProgramStartCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    ProgramEndLabelCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    ProgramEndCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                   Speaker1Label.Border = iTextSharp.text.Rectangle.NO_BORDER;
                   Speaker1LabelCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                  
                    Speaker1Cell.Border= iTextSharp.text.Rectangle.NO_BORDER;
                   
                    RegistrationCommentsLabelCell.Border= iTextSharp.text.Rectangle.NO_BORDER;
                    RegistrationCommentsCell1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    RegistrationCommentsCell2.Border = iTextSharp.text.Rectangle.NO_BORDER;

                }

                if (HasBorder == false)
                    DateLabelCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                if (HasBorder == false)
                    DateCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                if (HasBorder == false)
                    LocationLabelCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                if (HasBorder == false)
                    LocationCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

             

                DateLocation.AddCell(DateLabelCell);
                DateLocation.AddCell(DateCell);
                DateLocation.AddCell(RowOneFiller1);
                DateLocation.AddCell(RowOneFiller2);

                DateLocation.AddCell(LocationLabelCell);
                DateLocation.AddCell(LocationCell);
                //RowTwoFiller1.Colspan = 2;//The address of the venue needs 2 columns to fit
                DateLocation.AddCell(RowTwoFiller1);
                //DateLocation.AddCell(RowTwoFiller2);
                DateLocation.AddCell(ProgramStartLabelCell);
                DateLocation.AddCell(ProgramStartCell);
                DateLocation.AddCell(ProgramEndLabelCell);
                DateLocation.AddCell(ProgramEndCell);
                
                //speaker/moderator row start here



                //speaker/moderator row start here
                DateLocation.AddCell(Speaker1LabelCell);
                DateLocation.AddCell(Speaker1Cell);

                //moderator
                if (!String.IsNullOrEmpty(prim.ModeratorFirstName))
                {
                    iTextSharp.text.Image ModeratorLabel = iTextSharp.text.Image.GetInstance(imagepath + "/ModeratorLabel.jpg");
                    ModeratorLabel.ScalePercent(30f);
                    PdfPCell ModeratorLabelCell = new PdfPCell(ModeratorLabel);

                    PdfPCell ModeratorCell = new PdfPCell(new Phrase("Dr. " + prim.ModeratorFirstName + " " + prim.ModeratorLastName, DefaultFont));

                    ModeratorLabel.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    ModeratorLabelCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    ModeratorCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    ModeratorCell.HorizontalAlignment = 1;
                    ModeratorCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    ModeratorCell.BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255);
                    DateLocation.AddCell(ModeratorLabelCell);
                    DateLocation.AddCell(ModeratorCell);
                }
               //if speaker 2 is here add it

                if (!String.IsNullOrEmpty(prim.Speaker2FirstName))
                {
                    Speaker2Filler1 = new PdfPCell();
                    Speaker2Filler2 = new PdfPCell();
                    Speaker2Filler3 = new PdfPCell();
                    Speaker2Cell = new PdfPCell(new Phrase("Dr. " + prim.Speaker2FirstName + " " + prim.Speaker2LastName, DefaultFont));
                   Speaker2Cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                   Speaker2Filler1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    Speaker2Filler2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                   Speaker2Filler3.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    Speaker2Cell.HorizontalAlignment = Element.ALIGN_MIDDLE;
                    DateLocation.AddCell(Speaker2Filler1);
                    DateLocation.AddCell(Speaker2Cell);
                    DateLocation.AddCell(Speaker2Filler2);
                    DateLocation.AddCell(Speaker2Filler3);
                }
                //Registration Comments Table start here
                float[] Registrationwidths = new float[] { 1f, 4f };//equal width for now
                PdfPTable RegistrationTable = new PdfPTable(2);
                RegistrationTable.KeepTogether = true;
                RegistrationTable.TotalWidth = 523f;
                RegistrationTable.SetWidths(Registrationwidths);
                RegistrationTable.LockedWidth = true;
                RegistrationTable.SpacingBefore = 10f;


                RegistrationTable.SpacingAfter = 10f;
                RegistrationTable.HorizontalAlignment = 1;//0=Left, 1=Centre, 2=Right

                //cell 1
                iTextSharp.text.Image RegistrationLabel = iTextSharp.text.Image.GetInstance(imagepath + "/RegistrationCommentsLabel.jpg");
                RegistrationLabel.ScalePercent(30f);
                PdfPCell RegistrationLabelCell = new PdfPCell(RegistrationLabel);
                RegistrationLabelCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                //cell 2
               // PdfPCell RegistrationCell = new PdfPCell(new Phrase(prim.RegistrationComments1 + " " + prim.RegistrationComments2 + "\n\n", new iTextSharp.text.Font(HelveticaBase, 12f, iTextSharp.text.Font.NORMAL, BaseColor.BLACK)));
                PdfPCell RegistrationCell = new PdfPCell(new Phrase(""));

                RegistrationCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                
                RegistrationCell.HorizontalAlignment = 1;
                RegistrationCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                RegistrationCell.BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255);
                RegistrationTable.AddCell(RegistrationLabelCell);
                RegistrationTable.AddCell(RegistrationCell);


                TargetDocument.Add(DateLocation);
                TargetDocument.Add(RegistrationTable);

                //end of set up program date/location

                //set up RSVP image
                iTextSharp.text.Image ImgRSVP = iTextSharp.text.Image.GetInstance(imagepath + "/RSVP.jpg");
                ImgRSVP.Alignment = Element.ALIGN_CENTER;

                ImgRSVP.ScalePercent(54f);
                TargetDocument.Add(ImgRSVP);

                //RSVP information

                float[] RSVPwidths = new float[] {  5f };//equal width for now
                PdfPTable RSVPTable = new PdfPTable(1);
                RSVPTable.KeepTogether = true;
                RSVPTable.TotalWidth = 523f;
                RSVPTable.SetWidths(RSVPwidths);
                RSVPTable.LockedWidth = true;
                RSVPTable.SpacingBefore = 10f;


                RSVPTable.SpacingAfter = 10f;
                RSVPTable.HorizontalAlignment = 1;//0=Left, 1=Centre, 2=Right

                //cell 1
                PdfPCell RSVPFiller = new PdfPCell();

               

               
                PdfPCell RSVPContents = new PdfPCell(new Phrase(prim.RSVP, DefaultFont));
                RSVPContents.VerticalAlignment = Element.ALIGN_CENTER;



                RSVPContents.HorizontalAlignment = 1;//0=Left, 1=Centre, 2=Right

                RSVPContents.BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255);
                RSVPContents.Border = iTextSharp.text.Rectangle.NO_BORDER;
                RSVPFiller.Border = iTextSharp.text.Rectangle.NO_BORDER;
                





                //RSVPTable.AddCell(RSVPFiller);
                RSVPTable.AddCell(RSVPContents);
                RSVPTable.SpacingAfter = 10f;
                TargetDocument.Add(RSVPTable);
                //set up Agenda & Learning Objectives
                //set up RSVP image
                iTextSharp.text.Image ImgAgendaObjectives = iTextSharp.text.Image.GetInstance(imagepath + "/AgendaObjectives.jpg");
               // ImgAgendaObjectives.Alignment = Element.ALIGN_CENTER;
                ImgAgendaObjectives.SpacingAfter = 30f;
                ImgAgendaObjectives.ScalePercent(50f);
                TargetDocument.Add(ImgAgendaObjectives);
                //set up session credits
                if (prim.SessionCredit1)
                {
                    iTextSharp.text.Image EvalCase1 = iTextSharp.text.Image.GetInstance(imagepath + "/Case1.jpg");
                   
                    EvalCase1.ScalePercent(50f);
                    TargetDocument.Add(EvalCase1);
                    CaseCounter++;

                }

                if (prim.SessionCredit2)
                {
                    iTextSharp.text.Image EvalCase2 = iTextSharp.text.Image.GetInstance(imagepath + "/Case2.jpg");
                    EvalCase2.ScalePercent(50f);
                    TargetDocument.Add(EvalCase2);
                    CaseCounter++;
                }
                if (CaseCounter >= 3)
                {
                    TargetDocument.NewPage();
                    CaseCounter = 0;
                }
                if (prim.SessionCredit3)
                {
                    iTextSharp.text.Image EvalCase3 = iTextSharp.text.Image.GetInstance(imagepath + "/Case3.jpg");
                    EvalCase3.ScalePercent(50f);
                    TargetDocument.Add(EvalCase3);
                    CaseCounter++;
                }
                if (CaseCounter >= 3)
                {
                    TargetDocument.NewPage();
                    CaseCounter = 0;
                }
                if (prim.SessionCredit4)
                {
                    iTextSharp.text.Image EvalCase4 = iTextSharp.text.Image.GetInstance(imagepath + "/Case4.jpg");
                    EvalCase4.ScalePercent(50f);
                    TargetDocument.Add(EvalCase4);
                    CaseCounter++;
                }
                if (CaseCounter >= 3)
                {
                    TargetDocument.NewPage();
                    CaseCounter = 0;
                }
                if (prim.SessionCredit5)
                {
                    iTextSharp.text.Image EvalCase5 = iTextSharp.text.Image.GetInstance(imagepath + "/Case5.jpg");
                    EvalCase5.ScalePercent(50f);
                    TargetDocument.Add(EvalCase5);
                    CaseCounter++;
                }
                if (CaseCounter >= 3)
                {
                    TargetDocument.NewPage();
                    CaseCounter = 0;
                }
                if (prim.SessionCredit6)
                {
                    iTextSharp.text.Image EvalCase6 = iTextSharp.text.Image.GetInstance(imagepath + "/Case6.jpg");
                    EvalCase6.ScalePercent(50f);
                    TargetDocument.Add(EvalCase6);
                    CaseCounter++;
                }
                if (CaseCounter >= 3)
                {
                    TargetDocument.NewPage();
                    CaseCounter = 0;
                }
                if (prim.SessionCredit7)
                {
                    iTextSharp.text.Image EvalCase7 = iTextSharp.text.Image.GetInstance(imagepath + "/Case7.jpg");
                    EvalCase7.ScalePercent(50f);
                    TargetDocument.Add(EvalCase7);
                    CaseCounter++;
                }
                if (CaseCounter >= 3)
                {
                    TargetDocument.NewPage();
                    CaseCounter = 0;
                }
                if (prim.SessionCredit8)
                {
                    iTextSharp.text.Image EvalCase8 = iTextSharp.text.Image.GetInstance(imagepath + "/Case8.jpg");
                    EvalCase8.ScalePercent(50f);
                    TargetDocument.Add(EvalCase8);
                    CaseCounter++;
                }
                if (CaseCounter >= 3)
                {
                    TargetDocument.NewPage();
                    CaseCounter = 0;
                }
                if (prim.SessionCredit9)
                {
                    iTextSharp.text.Image EvalCase9 = iTextSharp.text.Image.GetInstance(imagepath + "/Case9.jpg");
                    EvalCase9.ScalePercent(50f);
                    TargetDocument.Add(EvalCase9);
                    CaseCounter++;
                }
                if (CaseCounter >= 3)
                {
                    TargetDocument.NewPage();
                    CaseCounter = 0;
                }
                if (prim.SessionCredit10)
                {
                    iTextSharp.text.Image EvalCase10 = iTextSharp.text.Image.GetInstance(imagepath + "/Case10.jpg");
                    EvalCase10.ScalePercent(50f);
                    TargetDocument.Add(EvalCase10);
                    CaseCounter++;
                }
                if (CaseCounter >= 3)
                {
                    TargetDocument.NewPage();
                    CaseCounter = 0;
                }
                if (prim.SessionCredit11)
                {
                    iTextSharp.text.Image EvalCase11 = iTextSharp.text.Image.GetInstance(imagepath + "/Case11.jpg");
                    EvalCase11.ScalePercent(50f);
                    TargetDocument.Add(EvalCase11);
                    CaseCounter++;
                }
                if (CaseCounter >= 3)
                {
                    TargetDocument.NewPage();
                    CaseCounter = 0;
                }

                if (prim.SessionCredit12)
                {
                    iTextSharp.text.Image EvalCase12 = iTextSharp.text.Image.GetInstance(imagepath + "/Case12.jpg");
                    EvalCase12.ScalePercent(50f);
                    TargetDocument.Add(EvalCase12);
                    CaseCounter++;
                }

                float[] Footerwidths = new float[] { 4f,1f };//equal width for now
                PdfPTable FooterTable = new PdfPTable(2);
                FooterTable.KeepTogether = true;
                FooterTable.TotalWidth = 523f;
                FooterTable.SetWidths(Footerwidths);
                FooterTable.LockedWidth = true;
                FooterTable.SpacingBefore = 50f;
                FooterTable.SpacingAfter = 10f;
                

                
                PdfPCell Footer1Cell = new PdfPCell(new Phrase("\n This Group Learning program has been reviewed by the College of Family Physicians of Canada and is awaiting final certification by the College's "  + prim.Province , BigArial));
                Footer1Cell.BackgroundColor = new iTextSharp.text.BaseColor(44, 194, 211);
                Footer1Cell.HorizontalAlignment = Element.ALIGN_CENTER;
                Footer1Cell.BorderColor= new iTextSharp.text.BaseColor(44, 194, 211);
                Footer1Cell.Border = iTextSharp.text.Rectangle.TOP_BORDER;
                // Footer1Cell.Border = iTextSharp.text.Rectangle.NO_BORDER;


                PdfPCell Footer2Cell = new PdfPCell(new Phrase("\n\n This program has received an educational grant and in-kind support from BMS/Pfizer Alliance Canada", SmallArial));
                Footer2Cell.BackgroundColor = new iTextSharp.text.BaseColor(44, 194, 211);
                Footer2Cell.HorizontalAlignment = Element.ALIGN_CENTER;
               Footer2Cell.Border = iTextSharp.text.Rectangle.NO_BORDER;


                //PdfPCell Footer3Cell = new PdfPCell();
                //Footer3Cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                //      Footer3Cell.BackgroundColor = new iTextSharp.text.BaseColor(44, 194, 211);
                //PdfPCell Footer3Cell = new PdfPCell(new Phrase("This program has received an educational grant and in-kind support from BMS/Pfizer Alliance Canada", SmallArial));
                //Footer3Cell.BackgroundColor = new iTextSharp.text.BaseColor(44, 194, 211);
                //Footer3Cell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                iTextSharp.text.Image ImgCHRCLogo = iTextSharp.text.Image.GetInstance(imagepath + "/chrclogo.jpg");
                ImgCHRCLogo.ScalePercent(20f);
                ImgCHRCLogo.Alignment = Element.ALIGN_LEFT;
                PdfPCell CHRCLogoCell = new PdfPCell(ImgCHRCLogo, false);

                //  CHRCLogoCell.Border = PdfPCell.NO_BORDER;
                //ImgCHRCLogo.Border = iTextSharp.text.Rectangle.NO_BORDER;

               // CHRCLogoCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                
              
                //iTextSharp.text.Rectangle.NO_BORDER;
                
              
                
                CHRCLogoCell.Rowspan = 2;

                FooterTable.AddCell(Footer1Cell);
               FooterTable.AddCell(CHRCLogoCell);
                FooterTable.DefaultCell.Border = PdfPCell.NO_BORDER;
                FooterTable.AddCell(Footer2Cell);
                //  FooterTable.AddCell(Footer3Cell);


                int first_row = 0;
                int last_row = -1;
                //write table to the bottom of the page but chrc logo is not printing out properly
              //  FooterTable.WriteSelectedRows(0, -1, TargetDocument.Left, TargetDocument.Bottom + FooterTable.TotalHeight,writer.DirectContent);


               TargetDocument.Add(FooterTable);


                //FooterTable.SpacingAfter = 10f;
                //FooterTable.HorizontalAlignment = 1;//0=Left, 1=Centre, 2=Right

                ////footer 1
                //iTextSharp.text.Image ImgFooter1 = iTextSharp.text.Image.GetInstance(imagepath + "/footer1.jpg");
                //ImgFooter1.Alignment = Element.ALIGN_CENTER;
                //ImgFooter1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                //ImgFooter1.ScalePercent(54f);
                //PdfPCell Footer1Cell = new PdfPCell(ImgFooter1);
                ////footer 2
                //iTextSharp.text.Image ImgFooter2 = iTextSharp.text.Image.GetInstance(imagepath + "/footer2.jpg");
                //ImgFooter2.Alignment = Element.ALIGN_CENTER;
                //ImgFooter2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                //ImgFooter2.ScalePercent(54f);
                //PdfPCell Footer2Cell = new PdfPCell(ImgFooter2);
                ////footer 3
                //iTextSharp.text.Image ImgFooter3 = iTextSharp.text.Image.GetInstance(imagepath + "/footer3.jpg");
                //ImgFooter3.Alignment = Element.ALIGN_CENTER;
                //ImgFooter3.Border = iTextSharp.text.Rectangle.NO_BORDER;
                //ImgFooter3.ScalePercent(54f);
                //PdfPCell Footer3Cell = new PdfPCell(ImgFooter3);
                ////footer 4
                //iTextSharp.text.Image ImgFooter4 = iTextSharp.text.Image.GetInstance(imagepath + "/footer4.jpg");
                //ImgFooter4.Alignment = Element.ALIGN_CENTER;
                //ImgFooter4.Border = iTextSharp.text.Rectangle.NO_BORDER;
                //ImgFooter4.ScalePercent(54f);
                //PdfPCell Footer4Cell = new PdfPCell(ImgFooter4);

                ////footer 5

                //PdfPCell Footer5Cell = new PdfPCell(new Phrase(prim.Province, new iTextSharp.text.Font(HelveticaBase, 12f, iTextSharp.text.Font.NORMAL, Color.BLACK)));
                //Footer5Cell.BackgroundColor = new iTextSharp.text.Color(44, 194, 211);

                ////footer 6
                //iTextSharp.text.Image ImgFooter6 = iTextSharp.text.Image.GetInstance(imagepath + "/footer6.jpg");
                //ImgFooter6.Alignment = Element.ALIGN_CENTER;
                //ImgFooter6.Border = iTextSharp.text.Rectangle.NO_BORDER;
                //ImgFooter6.ScalePercent(54f);
                //PdfPCell Footer6Cell = new PdfPCell(ImgFooter6);

                ////footer 7
                //iTextSharp.text.Image ImgFooter7 = iTextSharp.text.Image.GetInstance(imagepath + "/footer7.jpg");
                //ImgFooter7.Alignment = Element.ALIGN_CENTER;
                //ImgFooter7.Border = iTextSharp.text.Rectangle.NO_BORDER;
                //ImgFooter7.ScalePercent(54f);
                //PdfPCell Footer7Cell = new PdfPCell(ImgFooter7);


                ////footer 8
                //iTextSharp.text.Image ImgFooter8 = iTextSharp.text.Image.GetInstance(imagepath + "/footer8.jpg");
                //ImgFooter8.Alignment = Element.ALIGN_CENTER;
                //ImgFooter8.Border = iTextSharp.text.Rectangle.NO_BORDER;
                //ImgFooter8.ScalePercent(54f);
                //PdfPCell Footer8Cell = new PdfPCell(ImgFooter8);

                //FooterTable.AddCell(Footer1Cell);
                //FooterTable.AddCell(Footer2Cell);
                //FooterTable.AddCell(Footer3Cell);
                //FooterTable.AddCell(Footer4Cell);

                //FooterTable.AddCell(Footer5Cell);
                //FooterTable.AddCell(Footer6Cell);
                //FooterTable.AddCell(Footer7Cell);







                //if (CaseCounter >= 3)
                //{
                //    EvaluationDoc.NewPage();
                //    CaseCounter = 0;
                //}




                fileName = "/PDF/NationalInvitation/NationalInvitation" + RandomModifer + ".pdf";
                //  return File(new FileStream(Server.MapPath("~/App_Data/" + fileName), FileMode.Open), "application/octetstream", fileName);
                return Redirect(fileName);
            }
            catch (Exception e)
            {
                return Redirect(fileName);
                // return View("ProgramMaterials", efm);
            }
            finally
            {
                TargetDocument.Close();
            }
        }
        [HttpGet]
        public ActionResult RegionalInvitation(int ProgramRequestID)
        {
            int CaseCounter = 1;
            string fileName = string.Empty; //contains the file name of the resultant pdf
            bool HasBorder = false;  //set to false when development is completed. 
            System.DateTime ThisMoment = System.DateTime.Now;
            String RandomModifer = ThisMoment.Year + "_" + ThisMoment.Month + "_" + ThisMoment.Day + "_" + ThisMoment.Hour + "_" + ThisMoment.Minute + "_" + ThisMoment.Second + "_" + ThisMoment.Millisecond;
            string path = Server.MapPath("/PDF/RegionalInvitation");
            string imagepath = Server.MapPath("/Images/RegionalInvitation");
            var TargetDocument = new Document(PageSize.A4, 18, 18, 18, 18);
            //ViewBag.ProgramRequestStatusCounts = UserHelper.GetProgramRequestStatusCounts(Convert.ToInt32(Session["ProgramID"]));
            PdfWriter writer = PdfWriter.GetInstance(TargetDocument, new FileStream(path + "/RegionalInvitation" + RandomModifer + ".pdf", FileMode.Create));

            BaseFont HelveticaBase = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
            iTextSharp.text.Font BigArial = FontFactory.GetFont("Arial", BaseFont.CP1252, BaseFont.EMBEDDED, 9 ,iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font SmallArial = FontFactory.GetFont("Arial", BaseFont.CP1252, BaseFont.EMBEDDED, 6, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);

            DashboardRepository DBRepo = new DashboardRepository();
            ProgramRequestInfoModel prim = new ProgramRequestInfoModel();
            ProgramRepository progRepo = new ProgramRepository();

            prim = progRepo.GetProgramRequestInfo(ProgramRequestID);
           
           
            iTextSharp.text.Font Helvetica = new iTextSharp.text.Font(HelveticaBase, 14, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font DefaultFont = Helvetica;

            try
            {
                //set up top image
                //set up top image
                TargetDocument.Open();
                TargetDocument.NewPage();
                iTextSharp.text.Image EvalTop1 = iTextSharp.text.Image.GetInstance(imagepath + "/Header1.jpg");

                EvalTop1.ScalePercent(54f);
                TargetDocument.Add(EvalTop1);
                //end of set up top image

                float[] widths = new float[] { 1f, 1f, 1f, 1f };//equal width for now
                PdfPTable DateLocation = new PdfPTable(4);
                DateLocation.KeepTogether = true;
                DateLocation.TotalWidth = 523f;
                DateLocation.SetWidths(widths);
                DateLocation.LockedWidth = true;
                DateLocation.SpacingBefore = 10f;


                DateLocation.SpacingAfter = 10f;
                DateLocation.HorizontalAlignment = 1;//0=Left, 1=Centre, 2=Right

                //cell 1
                iTextSharp.text.Image DateLabel = iTextSharp.text.Image.GetInstance(imagepath + "/DateLabel.jpg");
                DateLabel.ScalePercent(30f);
                PdfPCell DateLabelCell = new PdfPCell(DateLabel);
                //cell 2
                PdfPCell DateCell = new PdfPCell(new Phrase(prim.ConfirmedSessionDate, DefaultFont));


                DateCell.HorizontalAlignment = 1;
                DateCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                DateCell.BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255);
                //cell 3
                PdfPCell RowOneFiller1 = new PdfPCell();
                PdfPCell RowOneFiller2 = new PdfPCell();


                iTextSharp.text.Image LocationLabel = iTextSharp.text.Image.GetInstance(imagepath + "/LocationLabel.jpg");
                LocationLabel.ScalePercent(30f);
                PdfPCell LocationLabelCell = new PdfPCell(LocationLabel);
                LocationLabelCell.HorizontalAlignment = 0; //alignleft
                                                           //cell 4
                                                           //iTextSharp.text.Image SpaceFieldLoc = iTextSharp.text.Image.GetInstance(imagepath + "/SpaceField.jpg");
                                                           //DateLabel.ScalePercent(30f);
                                                           //PdfPCell SpaceFieldCellLocation = new PdfPCell(SpaceFieldLoc);

                PdfPCell LocationCell = new PdfPCell(new Phrase(prim.VenueName, DefaultFont));
                LocationCell.HorizontalAlignment = 1;
                LocationCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                LocationCell.BackgroundColor = new BaseColor(255, 255, 255);

                PdfPCell RowTwoFiller1 = new PdfPCell(new Phrase(prim.VenueAddress, DefaultFont));
                RowTwoFiller1.VerticalAlignment = Element.ALIGN_MIDDLE;

                PdfPCell RowTwoFiller2 = new PdfPCell(new Phrase(prim.VenueCity + " " + prim.VenueProvince, DefaultFont));
                RowTwoFiller2.VerticalAlignment = Element.ALIGN_MIDDLE;

                //program start time cell
                iTextSharp.text.Image ProgramStartLabel = iTextSharp.text.Image.GetInstance(imagepath + "/ProgramStartLabel.jpg");
                ProgramStartLabel.ScalePercent(30f);
                PdfPCell ProgramStartLabelCell = new PdfPCell(ProgramStartLabel);

                PdfPCell ProgramStartCell = new PdfPCell(new Phrase(prim.programstarttime, DefaultFont));


                ProgramStartCell.HorizontalAlignment = 1;
                ProgramStartCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                ProgramStartCell.BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255);


                //program end time cell
                iTextSharp.text.Image ProgramEndLabel = iTextSharp.text.Image.GetInstance(imagepath + "/ProgramEndLabel.jpg");
                ProgramEndLabel.ScalePercent(30f);
                PdfPCell ProgramEndLabelCell = new PdfPCell(ProgramEndLabel);

                PdfPCell ProgramEndCell = new PdfPCell(new Phrase(prim.programendtime, DefaultFont));


                ProgramEndCell.HorizontalAlignment = 1;
                ProgramEndCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                ProgramEndCell.BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255);

                //speaker 1

                iTextSharp.text.Image Speaker1Label = iTextSharp.text.Image.GetInstance(imagepath + "/SpeakerLabel.jpg");
                Speaker1Label.ScalePercent(30f);
                PdfPCell Speaker1LabelCell = new PdfPCell(Speaker1Label);

                PdfPCell Speaker1Cell = new PdfPCell(new Phrase("Dr. " + prim.Speaker1FirstName + " " + prim.Speaker1LastName, DefaultFont));


                Speaker1Cell.HorizontalAlignment = 0;
                Speaker1Cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                Speaker1Cell.BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255);


                //speaker 2 if available
                string speaker2FirstName = prim.Speaker2FirstName;
                PdfPCell Speaker2Filler1;
                PdfPCell Speaker2Filler2;
                PdfPCell Speaker2Filler3;
                PdfPCell Speaker2Cell;



                //Registration Comments Table start here
                float[] Registrationwidths = new float[] { 1f, 4f };//equal width for now
                PdfPTable RegistrationTable = new PdfPTable(2);
                RegistrationTable.KeepTogether = true;
                RegistrationTable.TotalWidth = 523f;
                RegistrationTable.SetWidths(Registrationwidths);
                RegistrationTable.LockedWidth = true;
                RegistrationTable.SpacingBefore = 10f;


                RegistrationTable.SpacingAfter = 10f;
                RegistrationTable.HorizontalAlignment = 1;//0=Left, 1=Centre, 2=Right

                //cell 1
                iTextSharp.text.Image RegistrationLabel = iTextSharp.text.Image.GetInstance(imagepath + "/RegistrationCommentsLabel.jpg");
                RegistrationLabel.ScalePercent(30f);
                PdfPCell RegistrationLabelCell = new PdfPCell(RegistrationLabel);
                RegistrationLabelCell.Border= iTextSharp.text.Rectangle.NO_BORDER;
                //cell 2
                PdfPCell RegistrationCell = new PdfPCell(new Phrase(prim.RegistrationComments1 + " "  + prim.RegistrationComments2, DefaultFont));
                RegistrationCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                RegistrationCell.HorizontalAlignment = 1;
                RegistrationCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                RegistrationCell.BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255);
                RegistrationTable.AddCell(RegistrationLabelCell);
                RegistrationTable.AddCell(RegistrationCell);
              



                
                    RowOneFiller1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    RowOneFiller2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    RowTwoFiller1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    RowTwoFiller2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    ProgramStartLabelCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    ProgramStartCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    ProgramEndLabelCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    ProgramEndCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                    Speaker1Label.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    Speaker1LabelCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                    Speaker1Cell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                    RegistrationLabelCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    RegistrationCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                 

                

                if (HasBorder == false)
                    DateLabelCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                if (HasBorder == false)
                    DateCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                if (HasBorder == false)
                    LocationLabelCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                if (HasBorder == false)
                    LocationCell.Border = iTextSharp.text.Rectangle.NO_BORDER;



                DateLocation.AddCell(DateLabelCell);
                DateLocation.AddCell(DateCell);
                DateLocation.AddCell(RowOneFiller1);
                DateLocation.AddCell(RowOneFiller2);

                DateLocation.AddCell(LocationLabelCell);
                DateLocation.AddCell(LocationCell);
                DateLocation.AddCell(RowTwoFiller1);
                DateLocation.AddCell(RowTwoFiller2);
                DateLocation.AddCell(ProgramStartLabelCell);
                DateLocation.AddCell(ProgramStartCell);
                DateLocation.AddCell(ProgramEndLabelCell);
                DateLocation.AddCell(ProgramEndCell);

                //speaker/moderator row start here



                //speaker/moderator row start here
                DateLocation.AddCell(Speaker1LabelCell);
                DateLocation.AddCell(Speaker1Cell);

                //moderator
                if (!String.IsNullOrEmpty(prim.ModeratorFirstName))
                {
                    iTextSharp.text.Image ModeratorLabel = iTextSharp.text.Image.GetInstance(imagepath + "/ModeratorLabel.jpg");
                    ModeratorLabel.ScalePercent(30f);
                    PdfPCell ModeratorLabelCell = new PdfPCell(ModeratorLabel);

                    PdfPCell ModeratorCell = new PdfPCell(new Phrase("Dr. " + prim.ModeratorFirstName + " " + prim.ModeratorLastName, DefaultFont));

                    ModeratorLabel.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    ModeratorLabelCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    ModeratorCell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    ModeratorCell.HorizontalAlignment = 1;
                    ModeratorCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    ModeratorCell.BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255);
                    DateLocation.AddCell(ModeratorLabelCell);
                    DateLocation.AddCell(ModeratorCell);
                }
                //if speaker 2 is here add it

                if (!String.IsNullOrEmpty(prim.Speaker2FirstName))
                {
                    Speaker2Filler1 = new PdfPCell();
                    Speaker2Filler2 = new PdfPCell();
                    Speaker2Filler3 = new PdfPCell();
                    Speaker2Cell = new PdfPCell(new Phrase("Dr. " + prim.Speaker2FirstName + " " + prim.Speaker2LastName, DefaultFont));
                    Speaker2Cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    Speaker2Filler1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    Speaker2Filler2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    Speaker2Filler3.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    Speaker2Cell.HorizontalAlignment = Element.ALIGN_MIDDLE;
                    DateLocation.AddCell(Speaker2Filler1);
                    DateLocation.AddCell(Speaker2Cell);
                    DateLocation.AddCell(Speaker2Filler2);
                    DateLocation.AddCell(Speaker2Filler3);
                }
               

                TargetDocument.Add(DateLocation);
                //Registration Comments start after Speakers 2 and before RSVP table
                TargetDocument.Add(RegistrationTable);
                //end of set up program date/location

                //set up RSVP image
                iTextSharp.text.Image ImgRSVP = iTextSharp.text.Image.GetInstance(imagepath + "/RSVP.jpg");
                ImgRSVP.Alignment = Element.ALIGN_CENTER;

                ImgRSVP.ScalePercent(54f);
                TargetDocument.Add(ImgRSVP);

                //RSVP information

                float[] RSVPwidths = new float[] { 5f };//equal width for now
                PdfPTable RSVPTable = new PdfPTable(1);
                RSVPTable.KeepTogether = true;
                RSVPTable.TotalWidth = 523f;
                RSVPTable.SetWidths(RSVPwidths);
                RSVPTable.LockedWidth = true;
                RSVPTable.SpacingBefore = 10f;


                RSVPTable.SpacingAfter = 10f;
                RSVPTable.HorizontalAlignment = 1;//0=Left, 1=Centre, 2=Right

                //cell 1
                PdfPCell RSVPFiller = new PdfPCell();




                PdfPCell RSVPContents = new PdfPCell(new Phrase(prim.RSVP, DefaultFont));
                RSVPContents.VerticalAlignment = Element.ALIGN_CENTER;



                RSVPContents.HorizontalAlignment = 1;//0=Left, 1=Centre, 2=Right

                RSVPContents.BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255);
                RSVPContents.Border = iTextSharp.text.Rectangle.NO_BORDER;
                RSVPFiller.Border = iTextSharp.text.Rectangle.NO_BORDER;






                //RSVPTable.AddCell(RSVPFiller);
                RSVPTable.AddCell(RSVPContents);
                RSVPTable.SpacingAfter = 10f;
                TargetDocument.Add(RSVPTable);
                //set up Agenda & Learning Objectives
                //set up RSVP image
                iTextSharp.text.Image ImgAgendaObjectives = iTextSharp.text.Image.GetInstance(imagepath + "/AgendaObjectives.jpg");
                // ImgAgendaObjectives.Alignment = Element.ALIGN_CENTER;
                ImgAgendaObjectives.SpacingAfter = 30f;
              
                ImgAgendaObjectives.ScalePercent(50f);
                TargetDocument.Add(ImgAgendaObjectives);
                //set up session credits
                if (prim.SessionCredit1)
                {
                    iTextSharp.text.Image EvalCase1 = iTextSharp.text.Image.GetInstance(imagepath + "/Case1.jpg");

                    EvalCase1.ScalePercent(50f);
                    TargetDocument.Add(EvalCase1);
                    CaseCounter++;

                }

                if (prim.SessionCredit2)
                {
                    iTextSharp.text.Image EvalCase2 = iTextSharp.text.Image.GetInstance(imagepath + "/Case2.jpg");
                    EvalCase2.ScalePercent(50f);
                    TargetDocument.Add(EvalCase2);
                    CaseCounter++;
                }
                if (CaseCounter >= 3)
                {
                    TargetDocument.NewPage();
                    CaseCounter = 0;
                }
                if (prim.SessionCredit3)
                {
                    iTextSharp.text.Image EvalCase3 = iTextSharp.text.Image.GetInstance(imagepath + "/Case3.jpg");
                    EvalCase3.ScalePercent(50f);
                    TargetDocument.Add(EvalCase3);
                    CaseCounter++;
                }
                if (CaseCounter >= 3)
                {
                    TargetDocument.NewPage();
                    CaseCounter = 0;
                }
                if (prim.SessionCredit4)
                {
                    iTextSharp.text.Image EvalCase4 = iTextSharp.text.Image.GetInstance(imagepath + "/Case4.jpg");
                    EvalCase4.ScalePercent(50f);
                    TargetDocument.Add(EvalCase4);
                    CaseCounter++;
                }
                if (CaseCounter >= 3)
                {
                    TargetDocument.NewPage();
                    CaseCounter = 0;
                }
                if (prim.SessionCredit5)
                {
                    iTextSharp.text.Image EvalCase5 = iTextSharp.text.Image.GetInstance(imagepath + "/Case5.jpg");
                    EvalCase5.ScalePercent(50f);
                    TargetDocument.Add(EvalCase5);
                    CaseCounter++;
                }
                if (CaseCounter >= 3)
                {
                    TargetDocument.NewPage();
                    CaseCounter = 0;
                }
                if (prim.SessionCredit6)
                {
                    iTextSharp.text.Image EvalCase6 = iTextSharp.text.Image.GetInstance(imagepath + "/Case6.jpg");
                    EvalCase6.ScalePercent(50f);
                    TargetDocument.Add(EvalCase6);
                    CaseCounter++;
                }
                if (CaseCounter >= 3)
                {
                    TargetDocument.NewPage();
                    CaseCounter = 0;
                }
                if (prim.SessionCredit7)
                {
                    iTextSharp.text.Image EvalCase7 = iTextSharp.text.Image.GetInstance(imagepath + "/Case7.jpg");
                    EvalCase7.ScalePercent(50f);
                    TargetDocument.Add(EvalCase7);
                    CaseCounter++;
                }
                if (CaseCounter >= 3)
                {
                    TargetDocument.NewPage();
                    CaseCounter = 0;
                }
                if (prim.SessionCredit8)
                {
                    iTextSharp.text.Image EvalCase8 = iTextSharp.text.Image.GetInstance(imagepath + "/Case8.jpg");
                    EvalCase8.ScalePercent(50f);
                    TargetDocument.Add(EvalCase8);
                    CaseCounter++;
                }
                if (CaseCounter >= 3)
                {
                    TargetDocument.NewPage();
                    CaseCounter = 0;
                }
                if (prim.SessionCredit9)
                {
                    iTextSharp.text.Image EvalCase9 = iTextSharp.text.Image.GetInstance(imagepath + "/Case9.jpg");
                    EvalCase9.ScalePercent(50f);
                    TargetDocument.Add(EvalCase9);
                    CaseCounter++;
                }
                if (CaseCounter >= 3)
                {
                    TargetDocument.NewPage();
                    CaseCounter = 0;
                }
                if (prim.SessionCredit10)
                {
                    iTextSharp.text.Image EvalCase10 = iTextSharp.text.Image.GetInstance(imagepath + "/Case10.jpg");
                    EvalCase10.ScalePercent(50f);
                    TargetDocument.Add(EvalCase10);
                    CaseCounter++;
                }
                if (CaseCounter >= 3)
                {
                    TargetDocument.NewPage();
                    CaseCounter = 0;
                }
                if (prim.SessionCredit11)
                {
                    iTextSharp.text.Image EvalCase11 = iTextSharp.text.Image.GetInstance(imagepath + "/Case11.jpg");
                    EvalCase11.ScalePercent(50f);
                    TargetDocument.Add(EvalCase11);
                    CaseCounter++;
                }
                if (CaseCounter >= 3)
                {
                    TargetDocument.NewPage();
                    CaseCounter = 0;
                }

                if (prim.SessionCredit12)
                {
                    iTextSharp.text.Image EvalCase12 = iTextSharp.text.Image.GetInstance(imagepath + "/Case12.jpg");
                    EvalCase12.ScalePercent(50f);
                    TargetDocument.Add(EvalCase12);
                    CaseCounter++;
                }

                float[] Footerwidths = new float[] { 4f, 1f };//equal width for now
                PdfPTable FooterTable = new PdfPTable(2);
                FooterTable.KeepTogether = true;
                FooterTable.TotalWidth = 500f;
                FooterTable.SetWidths(Footerwidths);
                FooterTable.LockedWidth = true;
                FooterTable.SpacingBefore = 50f;
                FooterTable.SpacingAfter = 10f;



                PdfPCell Footer1Cell = new PdfPCell(new Phrase("\n This one-credit-per-hour Group Learning program has been certified by the \n College of Family Physicians of Canada and the " + prim.Province + "\n for up to " + prim.TotalSessionCredits + " Mainpro+ credits", BigArial));
                Footer1Cell.BackgroundColor = new iTextSharp.text.BaseColor(44, 194, 211);
                Footer1Cell.HorizontalAlignment = Element.ALIGN_CENTER;
                // Footer1Cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                Footer1Cell.BorderColor = new iTextSharp.text.BaseColor(44, 194, 211);
                Footer1Cell.Border = iTextSharp.text.Rectangle.TOP_BORDER;

                PdfPCell Footer2Cell = new PdfPCell(new Phrase("\n This program has received an educational grant and in-kind support from BMS/Pfizer Alliance Canada", SmallArial));
                Footer2Cell.BackgroundColor = new iTextSharp.text.BaseColor(44, 194, 211);
                Footer2Cell.HorizontalAlignment = Element.ALIGN_CENTER;
                Footer2Cell.Border = iTextSharp.text.Rectangle.NO_BORDER;


                //PdfPCell Footer3Cell = new PdfPCell();
                //Footer3Cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                //      Footer3Cell.BackgroundColor = new iTextSharp.text.BaseColor(44, 194, 211);
                //PdfPCell Footer3Cell = new PdfPCell(new Phrase("This program has received an educational grant and in-kind support from BMS/Pfizer Alliance Canada", SmallArial));
                //Footer3Cell.BackgroundColor = new iTextSharp.text.BaseColor(44, 194, 211);
                //Footer3Cell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                iTextSharp.text.Image ImgCHRCLogo = iTextSharp.text.Image.GetInstance(imagepath + "/chrclogo.jpg");
                ImgCHRCLogo.ScalePercent(20f);
                ImgCHRCLogo.Alignment = Element.ALIGN_LEFT;
                PdfPCell CHRCLogoCell = new PdfPCell(ImgCHRCLogo, false);

                //  CHRCLogoCell.Border = PdfPCell.NO_BORDER;
                //ImgCHRCLogo.Border = iTextSharp.text.Rectangle.NO_BORDER;

                CHRCLogoCell.Border = iTextSharp.text.Rectangle.NO_BORDER;


                //iTextSharp.text.Rectangle.NO_BORDER;



                CHRCLogoCell.Rowspan = 2;

                FooterTable.AddCell(Footer1Cell);
                FooterTable.AddCell(CHRCLogoCell);
                FooterTable.DefaultCell.Border = PdfPCell.NO_BORDER;
                FooterTable.AddCell(Footer2Cell);
                //  FooterTable.AddCell(Footer3Cell);


                int first_row = 0;
                int last_row = -1;
                //write table to the bottom of the page but chrc logo is not printing out properly
                //  FooterTable.WriteSelectedRows(0, -1, TargetDocument.Left, TargetDocument.Bottom + FooterTable.TotalHeight,writer.DirectContent);


                TargetDocument.Add(FooterTable);


                //FooterTable.SpacingAfter = 10f;
                //FooterTable.HorizontalAlignment = 1;//0=Left, 1=Centre, 2=Right

                ////footer 1
                //iTextSharp.text.Image ImgFooter1 = iTextSharp.text.Image.GetInstance(imagepath + "/footer1.jpg");
                //ImgFooter1.Alignment = Element.ALIGN_CENTER;
                //ImgFooter1.Border = iTextSharp.text.Rectangle.NO_BORDER;
                //ImgFooter1.ScalePercent(54f);
                //PdfPCell Footer1Cell = new PdfPCell(ImgFooter1);
                ////footer 2
                //iTextSharp.text.Image ImgFooter2 = iTextSharp.text.Image.GetInstance(imagepath + "/footer2.jpg");
                //ImgFooter2.Alignment = Element.ALIGN_CENTER;
                //ImgFooter2.Border = iTextSharp.text.Rectangle.NO_BORDER;
                //ImgFooter2.ScalePercent(54f);
                //PdfPCell Footer2Cell = new PdfPCell(ImgFooter2);
                ////footer 3
                //iTextSharp.text.Image ImgFooter3 = iTextSharp.text.Image.GetInstance(imagepath + "/footer3.jpg");
                //ImgFooter3.Alignment = Element.ALIGN_CENTER;
                //ImgFooter3.Border = iTextSharp.text.Rectangle.NO_BORDER;
                //ImgFooter3.ScalePercent(54f);
                //PdfPCell Footer3Cell = new PdfPCell(ImgFooter3);
                ////footer 4
                //iTextSharp.text.Image ImgFooter4 = iTextSharp.text.Image.GetInstance(imagepath + "/footer4.jpg");
                //ImgFooter4.Alignment = Element.ALIGN_CENTER;
                //ImgFooter4.Border = iTextSharp.text.Rectangle.NO_BORDER;
                //ImgFooter4.ScalePercent(54f);
                //PdfPCell Footer4Cell = new PdfPCell(ImgFooter4);

                ////footer 5

                //PdfPCell Footer5Cell = new PdfPCell(new Phrase(prim.Province, new iTextSharp.text.Font(HelveticaBase, 12f, iTextSharp.text.Font.NORMAL, Color.BLACK)));
                //Footer5Cell.BackgroundColor = new iTextSharp.text.Color(44, 194, 211);

                ////footer 6
                //iTextSharp.text.Image ImgFooter6 = iTextSharp.text.Image.GetInstance(imagepath + "/footer6.jpg");
                //ImgFooter6.Alignment = Element.ALIGN_CENTER;
                //ImgFooter6.Border = iTextSharp.text.Rectangle.NO_BORDER;
                //ImgFooter6.ScalePercent(54f);
                //PdfPCell Footer6Cell = new PdfPCell(ImgFooter6);

                ////footer 7
                //iTextSharp.text.Image ImgFooter7 = iTextSharp.text.Image.GetInstance(imagepath + "/footer7.jpg");
                //ImgFooter7.Alignment = Element.ALIGN_CENTER;
                //ImgFooter7.Border = iTextSharp.text.Rectangle.NO_BORDER;
                //ImgFooter7.ScalePercent(54f);
                //PdfPCell Footer7Cell = new PdfPCell(ImgFooter7);


                ////footer 8
                //iTextSharp.text.Image ImgFooter8 = iTextSharp.text.Image.GetInstance(imagepath + "/footer8.jpg");
                //ImgFooter8.Alignment = Element.ALIGN_CENTER;
                //ImgFooter8.Border = iTextSharp.text.Rectangle.NO_BORDER;
                //ImgFooter8.ScalePercent(54f);
                //PdfPCell Footer8Cell = new PdfPCell(ImgFooter8);

                //FooterTable.AddCell(Footer1Cell);
                //FooterTable.AddCell(Footer2Cell);
                //FooterTable.AddCell(Footer3Cell);
                //FooterTable.AddCell(Footer4Cell);

                //FooterTable.AddCell(Footer5Cell);
                //FooterTable.AddCell(Footer6Cell);
                //FooterTable.AddCell(Footer7Cell);







                //if (CaseCounter >= 3)
                //{
                //    EvaluationDoc.NewPage();
                //    CaseCounter = 0;
                //}




                fileName = "/PDF/RegionalInvitation/RegionalInvitation" + RandomModifer + ".pdf";
                //  return File(new FileStream(Server.MapPath("~/App_Data/" + fileName), FileMode.Open), "application/octetstream", fileName);
                return Redirect(fileName);
            }
            catch (Exception e)
            {
                return Redirect(fileName);
                // return View("ProgramMaterials", efm);
            }
            finally
            {
                TargetDocument.Close();
            }
        }


    }
}
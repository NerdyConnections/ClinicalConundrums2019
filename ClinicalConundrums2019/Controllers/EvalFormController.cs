using ClinicalConundrums2019.DAL;
using ClinicalConundrums2019.Models;
using ClinicalConundrums2019.Util;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;

using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClinicalConundrums2019.Controllers
{
    public class EvalFormController : Controller
    {
        // GET: EvalForm
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Index(int ProgramRequestID)
        {
            int CaseCounter = 1;
            string fileName=string.Empty; //contains the file name of the resultant pdf
            bool HasBorder = false;  //set to false when development is completed. 
            System.DateTime ThisMoment = System.DateTime.Now;
            String RandomModifer = ThisMoment.Year + "_" + ThisMoment.Month + "_" + ThisMoment.Day + "_" + ThisMoment.Hour + "_" + ThisMoment.Minute + "_" + ThisMoment.Second + "_" + ThisMoment.Millisecond;
            string path = Server.MapPath("/PDF/EvalForm");
            string imagepath = Server.MapPath("/Images/EvalForm");
            var EvaluationDoc = new Document(PageSize.A4, 18, 18, 18, 18);
            //ViewBag.ProgramRequestStatusCounts = UserHelper.GetProgramRequestStatusCounts(Convert.ToInt32(Session["ProgramID"]));
            PdfWriter.GetInstance(EvaluationDoc, new FileStream(path + "/Evaluation" + RandomModifer + ".pdf", FileMode.Create));
          
            DashboardRepository DBRepo = new DashboardRepository();
            EvalFormModel efm=DBRepo.GetEvalFormModel(ProgramRequestID);
            iTextSharp.text.Font ArialBold14 = FontFactory.GetFont("Arial", BaseFont.CP1252, BaseFont.EMBEDDED, 14, iTextSharp.text.Font.BOLD, BaseColor.BLACK);
            BaseFont HelveticaBase = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
            iTextSharp.text.Font Helvetica = new iTextSharp.text.Font(HelveticaBase, 12, iTextSharp.text.Font.NORMAL, BaseColor.BLACK);
            iTextSharp.text.Font DefaultFont = Helvetica;
            try
            {
                //set up top image
                //set up top image
                EvaluationDoc.Open();
                EvaluationDoc.NewPage();
                iTextSharp.text.Image EvalTop1 = iTextSharp.text.Image.GetInstance(imagepath + "/MainHeader.jpg");

                EvalTop1.ScalePercent(54f);
                EvaluationDoc.Add(EvalTop1);
                //end of set up top image

                float[] widths = new float[] { 1f, 2f, 1f, 2f };//speaker column is 2 times wider than evaluation column
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

                string ProgramDate = efm.ProgramDate;
                if (!String.IsNullOrEmpty(efm.ProgramDate))
                {
                    ProgramDate = Convert.ToDateTime(efm.ProgramDate).ToString("MMMM-dd-yyyy");

                }
                //cell 2
                PdfPCell DateCell = new PdfPCell(new Phrase(ProgramDate, DefaultFont));


                DateCell.HorizontalAlignment = 1;
                DateCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                DateCell.BackgroundColor = new iTextSharp.text.BaseColor(202, 204, 206);
                //cell 3
                iTextSharp.text.Image LocationLabel = iTextSharp.text.Image.GetInstance(imagepath + "/LocationLabel.jpg");
                LocationLabel.ScalePercent(30f);
                PdfPCell LocationLabelCell = new PdfPCell(LocationLabel);
                LocationLabelCell.HorizontalAlignment = 0; //alignleft
                                                           //cell 4
                                                           //iTextSharp.text.Image SpaceFieldLoc = iTextSharp.text.Image.GetInstance(imagepath + "/SpaceField.jpg");
                                                           //DateLabel.ScalePercent(30f);
                                                           //PdfPCell SpaceFieldCellLocation = new PdfPCell(SpaceFieldLoc);

                PdfPCell LocationCell = new PdfPCell(new Phrase(efm.ProgramLocation, DefaultFont));
                LocationCell.HorizontalAlignment = 1;
                LocationCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                LocationCell.BackgroundColor = new BaseColor(202, 204, 206);

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
                DateLocation.AddCell(LocationLabelCell);
                DateLocation.AddCell(LocationCell);



                EvaluationDoc.Add(DateLocation);


                //end of set up program date/location

                //set up top image
                iTextSharp.text.Image EvalTop2 = iTextSharp.text.Image.GetInstance(imagepath + "/ClinicalText.jpg");
                EvalTop2.Alignment = Element.ALIGN_CENTER;

                EvalTop2.ScalePercent(52f);
                EvaluationDoc.Add(EvalTop2);
                iTextSharp.text.Image EvalTop3 = iTextSharp.text.Image.GetInstance(imagepath + "/Demographics.jpg");
                EvalTop3.Alignment = Element.ALIGN_CENTER;

                EvalTop3.ScalePercent(52f);
                EvaluationDoc.Add(EvalTop3);




                if (efm.SessionCredit1)
                {
                    iTextSharp.text.Image EvalCase1 = iTextSharp.text.Image.GetInstance(imagepath + "/Case1.jpg");
                    EvalCase1.Alignment = Element.ALIGN_CENTER;
                    EvalCase1.ScalePercent(50f);
                    EvaluationDoc.Add(EvalCase1);
                    CaseCounter++;
                    
                }

                if (efm.SessionCredit2)
                {
                    iTextSharp.text.Image EvalCase2 = iTextSharp.text.Image.GetInstance(imagepath + "/Case2.jpg");
                    EvalCase2.ScalePercent(50f);
                    EvaluationDoc.Add(EvalCase2);
                    CaseCounter++;
                }
                if (CaseCounter >= 3)
                {
                    EvaluationDoc.NewPage();
                    CaseCounter = 0;
                }
                if (efm.SessionCredit3)
                {
                    iTextSharp.text.Image EvalCase3 = iTextSharp.text.Image.GetInstance(imagepath + "/Case3.jpg");
                    EvalCase3.ScalePercent(50f);
                    EvaluationDoc.Add(EvalCase3);
                    CaseCounter++;
                }
                if (CaseCounter >= 3)
                {
                    EvaluationDoc.NewPage();
                    CaseCounter = 0;
                }
                if (efm.SessionCredit4)
                {
                    iTextSharp.text.Image EvalCase4 = iTextSharp.text.Image.GetInstance(imagepath + "/Case4.jpg");
                    EvalCase4.ScalePercent(50f);
                    EvaluationDoc.Add(EvalCase4);
                    CaseCounter++;
                }
                if (CaseCounter >= 3)
                {
                    EvaluationDoc.NewPage();
                    CaseCounter = 0;
                }
                if (efm.SessionCredit5)
                {
                    iTextSharp.text.Image EvalCase5 = iTextSharp.text.Image.GetInstance(imagepath + "/Case5.jpg");
                    EvalCase5.ScalePercent(50f);
                    EvaluationDoc.Add(EvalCase5);
                    CaseCounter++;
                }
                if (CaseCounter >= 3)
                {
                    EvaluationDoc.NewPage();
                    CaseCounter = 0;
                }
                if (efm.SessionCredit6)
                {
                    iTextSharp.text.Image EvalCase6 = iTextSharp.text.Image.GetInstance(imagepath + "/Case6.jpg");
                    EvalCase6.ScalePercent(50f);
                    EvaluationDoc.Add(EvalCase6);
                    CaseCounter++;
                }
                if (CaseCounter >= 3)
                {
                    EvaluationDoc.NewPage();
                    CaseCounter = 0;
                }
                if (efm.SessionCredit7)
                {
                    iTextSharp.text.Image EvalCase7 = iTextSharp.text.Image.GetInstance(imagepath + "/Case7.jpg");
                    EvalCase7.ScalePercent(50f);
                    EvaluationDoc.Add(EvalCase7);
                    CaseCounter++;
                }
                if (CaseCounter >= 3)
                {
                    EvaluationDoc.NewPage();
                    CaseCounter = 0;
                }
                if (efm.SessionCredit8)
                {
                    iTextSharp.text.Image EvalCase8 = iTextSharp.text.Image.GetInstance(imagepath + "/Case8.jpg");
                    EvalCase8.ScalePercent(50f);
                    EvaluationDoc.Add(EvalCase8);
                    CaseCounter++;
                }
                if (CaseCounter >= 3)
                {
                    EvaluationDoc.NewPage();
                    CaseCounter = 0;
                }
                if (efm.SessionCredit9)
                {
                    iTextSharp.text.Image EvalCase9 = iTextSharp.text.Image.GetInstance(imagepath + "/Case9.jpg");
                    EvalCase9.ScalePercent(50f);
                    EvaluationDoc.Add(EvalCase9);
                    CaseCounter++;
                }
                if (CaseCounter >= 3)
                {
                    EvaluationDoc.NewPage();
                    CaseCounter = 0;
                }
                if (efm.SessionCredit10)
                {
                    iTextSharp.text.Image EvalCase10 = iTextSharp.text.Image.GetInstance(imagepath + "/Case10.jpg");
                    EvalCase10.ScalePercent(50f);
                    EvaluationDoc.Add(EvalCase10);
                    CaseCounter++;
                }
                if (CaseCounter >= 3)
                {
                    EvaluationDoc.NewPage();
                    CaseCounter = 0;
                }
                if (efm.SessionCredit11)
                {
                    iTextSharp.text.Image EvalCase11 = iTextSharp.text.Image.GetInstance(imagepath + "/Case11.jpg");
                    EvalCase11.ScalePercent(50f);
                    EvaluationDoc.Add(EvalCase11);
                    CaseCounter++;
                }
                if (CaseCounter >= 3)
                {
                    EvaluationDoc.NewPage();
                    CaseCounter = 0;
                }

                if (efm.SessionCredit12)
                {
                    iTextSharp.text.Image EvalCase12 = iTextSharp.text.Image.GetInstance(imagepath + "/Case12.jpg");
                    EvalCase12.ScalePercent(50f);
                    EvaluationDoc.Add(EvalCase12);
                    CaseCounter++;
                }
                if (CaseCounter >= 3)
                {
                    EvaluationDoc.NewPage();
                    CaseCounter = 0;
                }



                  //make sure speaker evaluation starts on a new page
                                          //setting up speaker evaluation

                if (!String.IsNullOrEmpty(efm.Speaker1))
                {
                    iTextSharp.text.Image SpeakerTitle = iTextSharp.text.Image.GetInstance(imagepath + "/SpeakerTitle.jpg");
                    SpeakerTitle.ScalePercent(50f);
                    EvaluationDoc.Add(SpeakerTitle);

                    float[] SpeakerTableWidths = new float[] { 1f};//speaker column is 2 times wider than evaluation column
                    PdfPTable SpeakerTable = new PdfPTable(1);
                    SpeakerTable.KeepTogether = true;
                    SpeakerTable.TotalWidth = 523f;
                    SpeakerTable.SetWidths(SpeakerTableWidths);
                    SpeakerTable.LockedWidth = true;
                    SpeakerTable.SpacingBefore = 10f;

                    SpeakerTable.SpacingAfter = 10f;
                    SpeakerTable.HorizontalAlignment = 1;//0=Left, 1=Centre, 2=Right

                   
                    //cell 1
                    PdfPCell SpeakerCell = new PdfPCell(new Phrase(efm.Speaker1, DefaultFont));
                    SpeakerCell.HorizontalAlignment = 0;
                    SpeakerCell.BackgroundColor = new BaseColor(255, 255, 255);
                    SpeakerCell.VerticalAlignment = Element.ALIGN_MIDDLE;

                  
                    if (HasBorder == false)
                        SpeakerCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                    
                    SpeakerTable.AddCell(SpeakerCell);
                    EvaluationDoc.Add(SpeakerTable);

                    iTextSharp.text.Image SpeakerEval1 = iTextSharp.text.Image.GetInstance(imagepath + "/SpeakerEval1.jpg");

                    SpeakerEval1.Alignment = Element.ALIGN_CENTER;
                    SpeakerEval1.ScalePercent(52f);
                    EvaluationDoc.Add(SpeakerEval1);
                    iTextSharp.text.Image SpeakerEval2 = iTextSharp.text.Image.GetInstance(imagepath + "/SpeakerEval2.jpg");

                    SpeakerEval2.Alignment = Element.ALIGN_CENTER;
                    SpeakerEval2.ScalePercent(52f);
                    EvaluationDoc.Add(SpeakerEval2);
                    CaseCounter++;

                }
                if (CaseCounter >= 3)
                {
                    EvaluationDoc.NewPage();
                    CaseCounter = 0;
                }
                string[] speaker2FirstLastName = efm.Speaker2.Split(',');
                //firstname and lastname are separated by comma if the first word is null than
                if (!String.IsNullOrEmpty(speaker2FirstLastName[0]))
                {
                    //speaker 2 if available
                    iTextSharp.text.Image SpeakerTitle = iTextSharp.text.Image.GetInstance(imagepath + "/SpeakerTitle.jpg");
                    SpeakerTitle.ScalePercent(50f);
                    EvaluationDoc.Add(SpeakerTitle);

                    float[] SpeakerTableWidths = new float[] { 1f };//speaker column is 2 times wider than evaluation column
                    PdfPTable SpeakerTable = new PdfPTable(1);
                    SpeakerTable.KeepTogether = true;
                    SpeakerTable.TotalWidth = 523f;
                    SpeakerTable.SetWidths(SpeakerTableWidths);
                    SpeakerTable.LockedWidth = true;
                    SpeakerTable.SpacingBefore = 10f;

                    SpeakerTable.SpacingAfter = 10f;
                    SpeakerTable.HorizontalAlignment = 1;//0=Left, 1=Centre, 2=Right


                    //speaker2 
                    PdfPCell Speaker2Cell = new PdfPCell(new Phrase(efm.Speaker2, DefaultFont));
                    Speaker2Cell.HorizontalAlignment = 0;
                    Speaker2Cell.BackgroundColor = new BaseColor(255, 255, 255);
                    Speaker2Cell.VerticalAlignment = Element.ALIGN_MIDDLE;


                    if (HasBorder == false)
                        Speaker2Cell.Border = iTextSharp.text.Rectangle.NO_BORDER;


                    SpeakerTable.AddCell(Speaker2Cell);

                    EvaluationDoc.Add(SpeakerTable);

                    iTextSharp.text.Image SpeakerEval1 = iTextSharp.text.Image.GetInstance(imagepath + "/SpeakerEval1.jpg");

                    SpeakerEval1.Alignment = Element.ALIGN_CENTER;
                    SpeakerEval1.ScalePercent(52f);
                    EvaluationDoc.Add(SpeakerEval1);
                    iTextSharp.text.Image SpeakerEval2 = iTextSharp.text.Image.GetInstance(imagepath + "/SpeakerEval2.jpg");

                    SpeakerEval2.Alignment = Element.ALIGN_CENTER;
                    SpeakerEval2.ScalePercent(52f);
                    EvaluationDoc.Add(SpeakerEval2);
                    CaseCounter++;

                }
                if (CaseCounter >= 3)
                {
                    EvaluationDoc.NewPage();
                    CaseCounter = 0;
                }
                string[] moderatorFirstLastName = efm.Moderator.Split(',');
                if (!String.IsNullOrEmpty(moderatorFirstLastName[0]))
                {
                    iTextSharp.text.Image SpeakerTitle = iTextSharp.text.Image.GetInstance(imagepath + "/SpeakerTitle.jpg");
                    SpeakerTitle.ScalePercent(50f);
                    EvaluationDoc.Add(SpeakerTitle);

                    float[] SpeakerTableWidths = new float[] { 1f };//speaker column is 2 times wider than evaluation column
                    PdfPTable SpeakerTable = new PdfPTable(1);
                    SpeakerTable.KeepTogether = true;
                    SpeakerTable.TotalWidth = 523f;
                    SpeakerTable.SetWidths(SpeakerTableWidths);
                    SpeakerTable.LockedWidth = true;
                    SpeakerTable.SpacingBefore = 10f;

                    SpeakerTable.SpacingAfter = 10f;
                    SpeakerTable.HorizontalAlignment = 1;//0=Left, 1=Centre, 2=Right


                  
                    //cell 2
                    PdfPCell SpeakerCell = new PdfPCell(new Phrase(efm.Moderator, DefaultFont));
                    SpeakerCell.HorizontalAlignment = 0;
                    SpeakerCell.BackgroundColor = new BaseColor(255, 255, 255);
                    SpeakerCell.VerticalAlignment = Element.ALIGN_MIDDLE;

                   
                    if (HasBorder == false)
                        SpeakerCell.Border = iTextSharp.text.Rectangle.NO_BORDER;

                    
                    SpeakerTable.AddCell(SpeakerCell);
                    EvaluationDoc.Add(SpeakerTable);

                    iTextSharp.text.Image SpeakerEval1 = iTextSharp.text.Image.GetInstance(imagepath + "/SpeakerEval1.jpg");

                    SpeakerEval1.Alignment = Element.ALIGN_CENTER;
                    SpeakerEval1.ScalePercent(52f);
                    EvaluationDoc.Add(SpeakerEval1);
                    iTextSharp.text.Image SpeakerEval2 = iTextSharp.text.Image.GetInstance(imagepath + "/SpeakerEval2.jpg");

                    SpeakerEval2.Alignment = Element.ALIGN_CENTER;
                    SpeakerEval2.ScalePercent(52f);
                    EvaluationDoc.Add(SpeakerEval2);
                    CaseCounter++;

                }
               
                    EvaluationDoc.NewPage();
                    CaseCounter = 0;
                


               

                //setting up this program
                iTextSharp.text.Image LastPage = iTextSharp.text.Image.GetInstance(imagepath + "/LastPage.jpg");
                LastPage.Alignment = Element.ALIGN_CENTER;
                LastPage.ScalePercent(52f);
                EvaluationDoc.Add(LastPage);
                efm.DisplayPDF = true;

                 fileName = "/PDF/EvalForm/Evaluation" + RandomModifer + ".pdf";
                //  return File(new FileStream(Server.MapPath("~/App_Data/" + fileName), FileMode.Open), "application/octetstream", fileName);
                return Redirect(fileName);
            }
            catch (Exception e)
            {
                UserHelper.WriteToLog(e.Message);
                return Redirect(fileName);
                // return View("ProgramMaterials", efm);
            }
            finally
            {
                EvaluationDoc.Close();
            }
        }
    }
}
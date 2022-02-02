using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace ClinicalConundrums2019.Util
{
    public class GenPdf
    {

        #region vars

        // PDF data
        string templatePath;
        private string filename;
        private Stream outStream = null;
        private iTextSharp.text.Document document;
        private PdfCopy copier;

        // streams and PDF objects used for adding extra forms
        FileStream inFileStream;
        MemoryStream outFileStream;
        PdfReader m_pdfReader;
        PdfStamper m_stamper;

        // field data
        private string[] values = null;

        #endregion vars

        #region interface

        public GenPdf(string path)
        {
            templatePath = path;
        }

        public bool Save()
        {
            try
            {
                document.Close();
                copier.Close();

                outStream = null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
                string methodName = st.GetFrame(0).GetMethod().Name;

                string innerMsg = "";
                if (ex.InnerException != null)
                    innerMsg = ex.InnerException.Message;

                Log(methodName, ex.Message, innerMsg);
                return false;
            }
            return true;
        }

        public byte[] Close()
        {
            byte[] b = null;
            try
            {
                document.Close();
                copier.Close();

                if (outStream.GetType() == typeof(MemoryStream))
                {
                    MemoryStream ms = (MemoryStream)outStream;

                    b = ms.ToArray();
                }

                if (null == outStream)
                    return null;

                return b;
            }
            catch (Exception ex)
            {
                System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
                string methodName = st.GetFrame(0).GetMethod().Name;

                string innerMsg = "";
                if (ex.InnerException != null)
                    innerMsg = ex.InnerException.Message;

                Log(methodName, ex.Message, innerMsg);
            }
            return null;
        }

        /// <summary>
        /// Start new form generation with out to file
        /// </summary>
        /// <param name="Output PDF file name"></param>
        /// <returns></returns>
        public bool Create(string name)
        {
            inFileStream = null;
            outFileStream = null;
            m_pdfReader = null;
            m_stamper = null;
            try
            {
                values = null;
                filename = name;
                outStream = new FileStream(filename, FileMode.Create);
                document = new iTextSharp.text.Document();

                copier = new PdfCopy(document, outStream);
                document.Open();
            }
            catch (Exception ex)
            {
                System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
                string methodName = st.GetFrame(0).GetMethod().Name;

                string innerMsg = "";
                if (ex.InnerException != null)
                    innerMsg = ex.InnerException.Message;

                Log(methodName, ex.Message, innerMsg);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Start new form generation with out to stream
        /// </summary>
        /// <param name="Output PDF file name"></param>
        /// <returns></returns>
        public bool Create()
        {
            inFileStream = null;
            outFileStream = null;
            m_pdfReader = null;
            m_stamper = null;
            try
            {
                values = null;
                filename = "";
                outStream = new MemoryStream();
                document = new iTextSharp.text.Document();

                copier = new PdfCopy(document, outStream);
                document.Open();
            }
            catch (Exception ex)
            {
                System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
                string methodName = st.GetFrame(0).GetMethod().Name;

                string innerMsg = "";
                if (ex.InnerException != null)
                    innerMsg = ex.InnerException.Message;

                Log(methodName, ex.Message, innerMsg);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Add new form to the output pdf
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool AddForm(string name)
        {
            if (null == outStream)
            {
                Log("AddForm() called when output PDF wasn't opened");
                return false;
            }

            string formName = Path.Combine(templatePath, name);
            if (!File.Exists(formName))
            {
                Log("File '" + name + "' not found");
                return false;
            }

            try
            {
                inFileStream = new FileStream(formName, FileMode.Open, FileAccess.Read, FileShare.Read);
                outFileStream = new MemoryStream();
                // Open existing PDF  
                m_pdfReader = new PdfReader(formName);

                // PdfStamper, which will create  
                m_stamper = new PdfStamper(m_pdfReader, outFileStream);
            }
            catch (Exception ex)
            {
                System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
                string methodName = st.GetFrame(0).GetMethod().Name;

                string innerMsg = "";
                if (ex.InnerException != null)
                    innerMsg = ex.InnerException.Message;

                Log(methodName, ex.Message, innerMsg);

                inFileStream = null;
                outFileStream = null;
                m_pdfReader = null;
                m_stamper = null;

                return false;
            }

            return true;
        }

        /// <summary>
        /// Add form to output pdf and close it
        /// </summary>
        /// <returns></returns>
        public bool AddField(string name, string val, int type)
        {
            if (null == outStream)
            {
                Log("AddField() called when output PDF wasn't opened");
                return false;
            }

            if (null == inFileStream ||
                null == outFileStream ||
                null == m_pdfReader ||
                null == m_stamper)
            {
                Log("AddField() when input form wasn't initialized");
                return false;
            }

            try
            {
                iTextSharp.text.pdf.PdfContentByte underContent;
                underContent = m_stamper.GetOverContent(1);

                AcroFields form = m_stamper.AcroFields;

                string text = "";
                switch (type)
                {
                    case 0:
                        text = val;
                        break;
                    case 1:
                        text = (val == "0") ? "No" : "Yes";
                        break;
                }
                if (!form.SetField(name, text))
                {
                    Log("FAILED: Field +'" +
                            name +
                            "', value '" +
                            text + "'");
                }
            }
            catch (Exception ex)
            {
                inFileStream = null;
                outFileStream = null;
                m_pdfReader = null;
                m_stamper = null;

                System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
                string methodName = st.GetFrame(0).GetMethod().Name;

                string innerMsg = "";
                if (ex.InnerException != null)
                    innerMsg = ex.InnerException.Message;

                Log(methodName, ex.Message, innerMsg);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Add form to output pdf and cose it
        /// </summary>
        /// <returns></returns>
        public bool FinalizeForm(bool IsFlat)
        {
            try
            {
                if (IsFlat)
                    m_stamper.FormFlattening = true;
                else
                    m_stamper.FormFlattening = false;
                m_stamper.Writer.Flush();
                m_stamper.Writer.CloseStream = false;
                m_stamper.Close();

                m_pdfReader.Close();

                Add2Pdf(outFileStream, outStream);

                // reset streams, etc.
                inFileStream = null;
                outFileStream = null;
                m_pdfReader = null;
                m_stamper = null;
            }
            catch (Exception ex)
            {
                inFileStream = null;
                outFileStream = null;
                m_pdfReader = null;
                m_stamper = null;

                System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
                string methodName = st.GetFrame(0).GetMethod().Name;

                string innerMsg = "";
                if (ex.InnerException != null)
                    innerMsg = ex.InnerException.Message;

                Log(methodName, ex.Message, innerMsg);
                return false;
            }

            return true;
        }

        #endregion interface

        #region impl



        private bool Add2Pdf(Stream instream, Stream outstream)
        {
            try
            {
                instream.Position = 0;
                PdfReader reader = new PdfReader(instream);

                int n = reader.NumberOfPages;
                // add content, page-by-page
                PdfImportedPage page;
                for (int p = 0; p < n; p++)
                {
                    document.SetPageSize(reader.GetPageSizeWithRotation(p + 1));
                    document.NewPage();
                    page = copier.GetImportedPage(reader, p + 1);
                    copier.AddPage(page);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
                string methodName = st.GetFrame(0).GetMethod().Name;

                string innerMsg = "";
                if (ex.InnerException != null)
                    innerMsg = ex.InnerException.Message;

                Log(methodName, ex.Message, innerMsg);
                return false;
            }
            return true;
        }

        private void Log(string msg)
        {
            System.Diagnostics.Debug.WriteLine(msg);
        }

        private void Log(string method, string msg, string innerMsg)
        {
            Log("Exception in " + method + "():");
            Log(msg);
            Log(innerMsg);
        }

        #endregion impl

    }
}
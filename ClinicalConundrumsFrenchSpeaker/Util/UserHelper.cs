
using ClinicalConundrumsFrenchSpeaker.Util;
using ClinicalConundrumsFrenchSpeaker.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ClinicalConundrumsFrenchSpeaker.Util
{
    public class UserHelper
    {
        public static object Constants { get; private set; }

        public static void WriteToLog(string errorMessage)
        {


            using (StreamWriter w = File.AppendText(HttpContext.Current.Server.MapPath("~/Log/log.txt")))
            {
                Log(errorMessage, w);

            }


        }
        private static void Log(string logMessage, TextWriter w)
        {
            w.Write("\r\nLog Entry : ");
            w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                DateTime.Now.ToLongDateString());
            w.WriteLine("  :");
            w.WriteLine("  :{0}", logMessage);
            w.WriteLine("-------------------------------");
        }

        private static void LoadDataIntoSession()
        {
            // PhysicianController usrControler = new PhysicianController();
       //     UserRepository UserRepo = new UserRepository();

          //  Models.UserModel user = UserRepo.GetUserDetails(System.Web.HttpContext.Current.User.Identity.Name);
            UserModel user = new UserModel();
            user.FirstName = "Speaker";
            if (user != null)
            {
                HttpContext.Current.Session["USER"] = user;
            }

        }


        public static void ReloadUser()
        {
            LoadDataIntoSession();
        }
        public static UserModel GetLoggedInUser()
        {
            if (HttpContext.Current.Session["USER"] == null)

                LoadDataIntoSession();

            return HttpContext.Current.Session["USER"] as UserModel;

        }
     

        //pass in a comma delimited string of roles and determine if current user is in any one of them
        public static bool IsInRole(string roles)
        {

            String[] ArRoles = roles.Split(',');
            var user = HttpContext.Current.User;
            foreach (string role in ArRoles)
            {
                if (user.IsInRole(role))
                    return true;
            }

            return false;

        }
        public static void SetLoggedInUser(UserModel user, System.Web.SessionState.HttpSessionState session)
        {

            session["USER"] = user;

        }

        public static bool SendEmailToAdmin(string emailTo, string emailBody, string emailSubject)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();

                mailMessage.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["emailGeneral"]);
                mailMessage.To.Add(new System.Net.Mail.MailAddress(emailTo));
                //testing  mailMessage.To.Add(new System.Net.Mail.MailAddress("amanullaha@chrc.net"));
                mailMessage.Subject = emailSubject;

                mailMessage.IsBodyHtml = true;
                //AlternateView htmlView = AlternateView.CreateAlternateViewFromString(GetRegistrationEmailBody(string.Empty, string.Empty, string.Empty, string.Empty), null, "text/html");
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(emailBody, null, "text/html");


                //LinkedResource imagelink = new LinkedResource(Server.MapPath("~/images/regEmailImage.jpg"), "image/jpg");

                //imagelink.ContentId = "imageId";

                //imagelink.TransferEncoding = System.Net.Mime.TransferEncoding.Base64;

                //htmlView.LinkedResources.Add(imagelink);

                mailMessage.AlternateViews.Add(htmlView);
                //  mailMessage.Attachments.Add(new Attachment(Server.MapPath("~/pdf/CHOLESTABETES Needs Assessment.pdf")));

                SendMail(mailMessage);
                return true;


            }

            catch (Exception e)
            {
                // Response.Write("fail in sendEmailNotification+++++" + e.Message.ToString());

                return false;
            }
        }
        public static HttpCookie GetAuthorizationCookie(string userName, string userData)
        {
            HttpCookie httpCookie = FormsAuthentication.GetAuthCookie(userName, true);
            FormsAuthenticationTicket currentTicket = FormsAuthentication.Decrypt(httpCookie.Value);
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(currentTicket.Version, currentTicket.Name, currentTicket.IssueDate, currentTicket.Expiration, currentTicket.IsPersistent, userData);
            httpCookie.Value = FormsAuthentication.Encrypt(ticket);
            return httpCookie;
        }




        public static void SendMail(MailMessage Message)
        {
            SmtpClient client = new SmtpClient();
            try
            {

                client.Host = ConfigurationManager.AppSettings["smtpServer"];

                System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                // NetworkCred.UserName = "webmaster@questionaf.ca";
                //NetworkCred.Password = "xkc232v";
                NetworkCred.UserName = ConfigurationManager.AppSettings["smtpUser"];
                NetworkCred.Password = ConfigurationManager.AppSettings["smtpPassword"];
                client.UseDefaultCredentials = false;
                client.Credentials = NetworkCred;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                // client.Port = 25;
                client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["smtpPort"]);

                client.Timeout = 20000;
                client.Send(Message);

            }
            catch (Exception e)
            {
                client = null;
                String Error = e.Message.ToString();
                //Utility.WriteToLog("SendMail Error: " + Error);
            }

        }


        public static IEnumerable<SelectListItem> GetProvinces()
        {
            List<SelectListItem> provinces = new List<SelectListItem>
            {

                      new SelectListItem {Text = "AB", Value = "AB"},
                      new SelectListItem {Text = "BC", Value = "BC"},
                      new SelectListItem {Text = "MB", Value = "MB"},
                      new SelectListItem {Text = "NS", Value = "NS"},
                      new SelectListItem {Text = "NB", Value = "NB"},
                      new SelectListItem {Text = "NL", Value = "NL"},
                      new SelectListItem {Text = "ON", Value = "ON"},
                      new SelectListItem {Text = "PE", Value = "PE"},
                      new SelectListItem {Text = "QC", Value = "QC"},
                      new SelectListItem {Text = "SK", Value = "SK"},


            };
            return provinces;

        }

        public static void EmailSaleRep_SpeakerApproved(UserModel UMSalesRep, UserModel UMSpeaker)
        {
            try
            {
                string html = string.Empty;

                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                mailMessage.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["emailGeneral"]);
                mailMessage.To.Add(new System.Net.Mail.MailAddress(UMSalesRep.EmailAddress));
                mailMessage.Subject = "Speaker Request Approved";
                mailMessage.IsBodyHtml = true;


                html = @"<table width = '900' border = '0' cellspacing = '0' cellpadding = '10'>
				<tr>                
				<td align = 'left'><span style = 'font-size: 30px; font-family: Arial, Helvetica, sans-serif;'> Dear {SRFirstName} </span></td> 
									   
				 </tr>  
				<tr>                
				<td align = 'left'><span style ='font-color:red; font-size: 30px; font-family: Arial, Helvetica, sans-serif;'> Your speaker request has been approved and Dr. {SpeakerFirstName} {SpeakerLastName} has been forwarded an invitation to register through the speaker resource centre. Should the speaker decline the invitation, the “Status” in the column will be updated accordingly.<br/><br/>  </td> 
									   
				 </tr>   
				<tr>
				<td align='left'>
						Please do not hesitate to contact us should you have any questions or require any assistance.<br/><br/>
				</td>

				</tr>  
				<tr>
				<td align='left'>
						The CHRC Web Portal Team <br/><br/>
				</td>

				</tr>          
				<tr>
				<td align='left'>
					   E: <a mailto:'info@clinicalconundrums.ca'>info@clinicalconundrums.ca</a> <br/><br/>
				</td>

				</tr>   

			</table> ";
                html = html.Replace("{SRFirstName}", UMSalesRep.FirstName.ToString());

                html = html.Replace("{SpeakerFirstName}", UMSpeaker.FirstName);
                html = html.Replace("{SpeakerLastName}", UMSpeaker.LastName);

                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(html, null, "text/html");
                mailMessage.AlternateViews.Add(htmlView);

                UserHelper.SendMail(mailMessage);
            }

            catch (Exception e)
            {
                string msg = e.Message;
            }
        }

      
        public static void SendEmailForgotPassword(string email, string password)
        {

            try
            {
                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();

                mailMessage.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["emailGeneral"]);
                mailMessage.To.Add(new System.Net.Mail.MailAddress(email));
                mailMessage.Subject = "Forgot Password";
                mailMessage.IsBodyHtml = true;

                string contentString = "<strong><p>Please keep this information confidential and do not share or forward your username and password.</p>"
                    + "<p>Your Email Address: {email}</p>"
                    + "<p>Your Password: {password}</p><strong>";
                string content = contentString.Replace("{email}", email).Replace("{password}", password);

                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(content, null, "text/html");
                mailMessage.AlternateViews.Add(htmlView);

                SendMail(mailMessage);
            }

            catch (Exception e)
            {
                string msg = e.Message;
            }
        }
      


        public static string GetFileNameForEmailAttachment(string path, int ProgramRequestID)
        {


            string retVal = "";




            if (!(string.IsNullOrEmpty(path)))
            {
                var FileName = Path.GetFileName(path);
                retVal = ProgramRequestID + "/Agenda/" + FileName;
            }
            return retVal;

        }

        public static string ConvertTimetoProperFormat(string date)
        {
            string retDate = "";

            if (!string.IsNullOrEmpty(date))
            {
                DateTime dt = DateTime.ParseExact(date, "yyyy/MM/dd", null);
                retDate = dt.ToLongDateString();
            }

            return retDate;
        }

        public static bool IsEmailValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

   
      
        public static void SpeakerActivationEmail(string LastName, string email, string password, Controller controller)
        {

            try
            {
                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();

                mailMessage.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["emailGeneral"]);
                mailMessage.To.Add(new System.Net.Mail.MailAddress(email));
                mailMessage.Subject = "Welcome to your personal Speaker Resource Portal: Your Account has been Successfully Activated";

                mailMessage.IsBodyHtml = true;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(SpeakerActivationEmailBody(LastName, email, password), null, "text/html");

                mailMessage.AlternateViews.Add(htmlView);
                mailMessage.Attachments.Add(new Attachment(controller.HttpContext.Server.MapPath("~/PDF/Cookies.pdf")));
                UserHelper.SendMail(mailMessage);
            }

            catch (Exception e)
            {

                string msg = e.Message;


            }


        }
        private static string SpeakerActivationEmailBody(string LastName, string userName, string password)
        {
            string speakerSiteUrl = ConfigurationManager.AppSettings["SpeakerSiteURL"];
            string contactEmail = ConfigurationManager.AppSettings["ContactEmail"];

            string html = string.Empty;

            html = @"<table width='500' border='0' cellspacing='0' cellpadding='10'>
					   
					<tr>
						 
					<td align='left'>
						Dear  {LastName}, <br /> &nbsp;<br />
						You have successfully activated your personal Speaker Resource Portal.<br /> &nbsp;<br />
						Please take note of your <strong> username </strong> and <strong> password </strong> which you will be required to enter each time you access your personal online portal at <strong>
				  
					   <strong style='color:#125d9f'>" + speakerSiteUrl + @"</strong><br />
				   
						</strong> <br />  <strong>
						Please keep this information confidential and do not share or forward your username and password:<br />
						&nbsp;<br />
						Your Username:
						</strong> <span>					
						<strong style='color:#125d9f'> {userName} </strong> &nbsp; </span> &nbsp;<br /> <strong>Your Password:</strong> <strong style='color:#125d9f'> {password} </strong>
				
						<p style='color:#125d9f'><strong> Next Steps:</strong>  </p>
					 
						<p>Please ensure that you<strong style='color:#125d9f'> <u> enable cookies </u></strong> in your browser. Please reference the attached guide for a step by step process. </p>
						   
						 <p> Please sign-in with your username and password at <strong style='color:#125d9f'>" + speakerSiteUrl + @"</strong> to familiarize with the web portal and its’ functionalities. <br />
						 &nbsp;<br />  
	
					</p>
	
				</td>
	
			</tr>         
			
	
			<tr>    
				<td align='left'>
					Should you have any questions or require any assistance, please do not hesitate to contact us at <strong><U><a style='color:#125d9f' href='mailto:" + contactEmail + @"'>" + contactEmail + @"</a></U></strong><strong>
			 
								 <br />
			 
							 </strong> <br />
						   
							 With best regards, <br />          
			 
							 <strong style='color:#125d9f'>
							 The CCPDHM Web Portal Team <br />          
							<strong>E:</strong> <strong><U><a href='mailto:" + contactEmail + @"'>" + contactEmail + @"</a></U></strong><br />
											   
				</td>                                              
			</tr>                                             
											  
	 </table>";

            html = html.Replace("{LastName}", LastName);
            html = html.Replace("{userName}", userName);
            html = html.Replace("{password}", password);

            return html;

        }
    }
}
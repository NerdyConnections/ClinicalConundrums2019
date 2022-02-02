using ClinicalConundrums2019.DAL;
using ClinicalConundrums2019.Models;
using ClinicalConundrums2019.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ClinicalConundrums2019.Util
{
    public static class UserHelper
    {
        public static string GetRoleByUserID(int UserID)
        {
            UserRepository UserRepo = new UserRepository();
            return UserRepo.GetRoleByUserID(UserID);

        }
        private static void LoadDataIntoSession()
        {
            // PhysicianController usrControler = new PhysicianController();
            UserRepository UserRepo = new UserRepository();

            Models.UserModel user = UserRepo.GetUserDetails(System.Web.HttpContext.Current.User.Identity.Name);


            if (user != null)
            {

                if (user.Language == "spa")
                    user.SpecifiedCulture = "es-MX";
                else if (user.Language == "por")
                    user.SpecifiedCulture = "pt-BR";
                else
                    user.SpecifiedCulture = "en-US";

                HttpContext.Current.Session[Constants.USER] = user;
            }

        }
        public static UserModel GetLoggedInUser()
        {
            if (HttpContext.Current.Session[Constants.USER] == null)

                LoadDataIntoSession();

            return HttpContext.Current.Session[Constants.USER] as UserModel;

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

            session[Constants.USER] = user;

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
        public static void SendAdminProgramRequest(EventRequestModel pr, string speakername, string speaker2name, string ModeratorName, string SessionCredit, string ProgramName, Controller controller)
        {

            try
            {

                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                mailMessage.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["emailGeneral"]);
                mailMessage.To.Add(new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["AdminEmail"]));
                mailMessage.Subject = ProgramName + "  Certification Request - ProgramDate " + pr.ProgramDate1;
                mailMessage.IsBodyHtml = true;
                
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(GetEmailBodyAdmin(pr, speakername,speaker2name, ModeratorName, SessionCredit), null, "text/html");
                mailMessage.AlternateViews.Add(htmlView);

                if (!string.IsNullOrEmpty(pr.SessionAgendaFileName))
                {
                    string FileName = GetFileNameForEmailAttachment(pr.SessionAgendaFileName, pr.ProgramRequestID);
                    mailMessage.Attachments.Add(new Attachment(controller.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["UserFileUploadPath"] + FileName)));
                }
                UserHelper.SendMail(mailMessage);
            }

            catch (Exception e)
            {
                string msg = e.Message;
            }
        }

        public static void SendEmailAfterHandoutRequest(HandoutRequestModel model, bool toAdmin)
        {

            try
            {
                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                mailMessage.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["emailGeneral"]);
                if (toAdmin)
                {
                    mailMessage.To.Add(new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["AdminEmail"]));
                }
                else
                {
                    mailMessage.To.Add(new System.Net.Mail.MailAddress(model.EmailAddress));
                }

                mailMessage.Subject = "Your 2020 Clinical Conundrums Pocket Guide Request";
                mailMessage.IsBodyHtml = true;
          
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(GetEmailContentForHandoutRequest(toAdmin, model), null, "text/html");
                mailMessage.AlternateViews.Add(htmlView);

                UserHelper.SendMail(mailMessage);
            }

            catch (Exception e)
            {
                string msg = e.Message;
            }
        }


        private static string GetEmailContentForHandoutRequest(bool toAdmin, HandoutRequestModel model)
        {
            string content;
            if (toAdmin)
            {
                content = "<div>User Email: " + model.UserEmail + "</div>";
            }
            else
            {
                content = "<div>Thank you. You have successfully completed the form.</div>"
                    + "<div><font color=\"red\">To note: </font>When you receive your manual you will see changes from last year. "
                    + "The key slides from each case will be included in the manual and there will also be a handy pocket guide along with other added bonus materials which we believe will benefit the physicians in attendance. "
                    + "Each tab (case) will have a pocket insert in which you may include additional handouts or photocopies of the full case slide deck if you wish. (this is up to each individual rep and not mandatory)</div><br/>"
                    + "<div>Merci! Vous avez completez le formulaire.</div>"
                    + "<div><font color=\"red\">Noter: </font>Le manuel comprendra des diapositives clés de chaque cas. Il y aura aussi un guide de poche pratique et d’autres bonus que nous pensons avantageux pour les participants. "
                    + "Chaque onglet (cas) aura une pochette pour insérer des feuillets ou des copies en noir et blanc du cas complet si vous voulez. (C'est à chaque représentant individuel et non obligatoire)</div><br/>";
            }

            content += "<div>Date Submitted: {dateSubmitted}</div>"
                    + "<div>Number of Pocket Guides: {number}</div>"
                    + "<div>Pocket Guide Language: {language}</div>"
                    + "<div>Date Required By/ Date Requis par: {dateRequired}</div>"
                    + "<div>Contact Name (Ship to) / Nom du contact (Expédier à): {contactName}</div>"
                    + "<div>Email / Courriel: {email}</div>"
                    + "<div>Mailing Address / Adresse mail: {mailAddress}</div>"
                    //+ "<div>Mailing Address1: {mailAddress1}</div>"
                    + "<div>City: {city}</div>"
                    + "<div>Province: {province}</div>"
                    + "<div>Postal Code: {postalCode}</div>"
                    + "<div>Phone Number: {phoneNum}</div>"
                    + "<div>Would you like a copy mailed to the presenter? {copyMail}</div>"
                    + "<div>Presenter Name / Nom du Conférencier: {presenterName}</div>"
                    + "<div>Presenter Mailing Address / Addresse: {presenterMailAdd}</div>"
                    + "<div>Presenter City: {presenterCity}</div>"
                    + "<div>Presenter Province: {presenterProvince}</div>"
                    + "<div>Presenter Postal Code: {presenterPostalCode}</div>"
                    + "<div>Additional Information: {additionalInfo}</div>";

            content = content.Replace("{dateSubmitted}", DateTime.Now.ToShortDateString())
                    .Replace("{number}", model.Number.ToString())
                    .Replace("{language}", model.Language)
                    .Replace("{dateRequired}", model.DateRequiredBy)
                    .Replace("{contactName}", model.ContactName)
                    .Replace("{email}", model.EmailAddress)
                    .Replace("{mailAddress}", model.MailingAddress)
                    //.Replace("{mailAddress1}", model.PresenterMailingAddress)
                    .Replace("{city}", model.City)
                    .Replace("{province}", model.Province)
                    .Replace("{postalCode}", model.PostalCode)
                    .Replace("{phoneNum}", model.PhoneNumber)
                    .Replace("{copyMail}", model.MailToPresenter.Equals("Y") ? "Yes" : "No")
                    .Replace("{presenterName}", model.PresenterName ?? "")
                    .Replace("{presenterMailAdd}", model.PresenterMailingAddress ?? "")
                    .Replace("{presenterCity}", model.PresenterCity ?? "")
                    .Replace("{presenterProvince}", model.PresenterProvince ?? "")
                    .Replace("{presenterPostalCode}", model.PresenterPostalCode ?? "")
                    .Replace("{additionalInfo}", model.AdditionalInfo ?? "");

            return content;
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
        public static void WriteToLog(string errorMessage)
        {


            using (StreamWriter w = File.AppendText(HttpContext.Current.Server.MapPath("~/log/log.txt")))
            {
                Log(errorMessage, w);

            }


        }
        private static string GetEmailBodyAdmin(EventRequestModel pr, string speakername, string speaker2name, string ModeratorName, string SessionCredit)
        {
            string html = string.Empty;

            string Materials = "";

            


            html = App_GlobalResources.Resource.AdminEmailText;

            html = html.Replace("{RecordID}", pr.ProgramRequestID.ToString());
            html = html.Replace("{DateSubmitted}", DateTime.Now.ToString());
            html = html.Replace("{SessionContact}", pr.ContactName);
            html = html.Replace("{ContactPhoneNumber}", pr.ContactPhone);
            html = html.Replace("{ContactEmailAddress}", pr.ContactEmail);
            html = html.Replace("{SessionDate1}", ConvertTimetoProperFormat(pr.ProgramDate1));
            html = html.Replace("{SessionDate2}", ConvertTimetoProperFormat(pr.ProgramDate2));
            html = html.Replace("{SessionDate3}", ConvertTimetoProperFormat(pr.ProgramDate3));
            html = html.Replace("{MealStartTime}", pr.MealStartTime);
            html = html.Replace("{SessionStartTime}", pr.ProgramStartTime);
            html = html.Replace("{SessionEndTime}", pr.ProgramEndTime);
            html = html.Replace("{SessionCredits}", SessionCredit);
            html = html.Replace("{MultiSessionEvent}", pr.MultiSession.ToString());
            html = html.Replace("{CopyofAgenda}", Materials);
            html = html.Replace("{CostsbyParticipants}", pr.CostPerparticipants.ToString());
            html = html.Replace("{SessionSpeaker}", speakername);
            html = html.Replace("{SessionSpeaker2}", speaker2name);
            html = html.Replace("{SessionSpeaker2Status}", "");
            html = html.Replace("{SessionSpeakerStatus}", "");
            html = html.Replace("{SessionModerator}", ModeratorName);

            html = html.Replace("{SessionModeratorStatus}", "");
            html = html.Replace("{VenueContacted}", pr.VenueContacted);
            html = html.Replace("{LocationType}", pr.LocationType);


            html = html.Replace("{LocationName}", pr.LocationName);

            html = html.Replace("{Address}", pr.LocationAddress);

            html = html.Replace("{City}", pr.LocationCity);

            html = html.Replace("{Province}", pr.LocationProvince);

            html = html.Replace("{PhoneNumber}", pr.LocationPhoneNumber);

            html = html.Replace("{Website}", pr.LocationWebsite);

            html = html.Replace("{MealType}", pr.MealType);

            html = html.Replace("{CostPerPerson}", pr.CostPerPerson.ToString());

            html = html.Replace("{AudioVisual}", pr.AVEquipment);

            html = html.Replace("{AdditionalInfo}", pr.Comments);








            return html;

        }
        public static HttpCookie GetAuthorizationCookie(string userName, string userData)
        {
            HttpCookie httpCookie = FormsAuthentication.GetAuthCookie(userName, true);
            FormsAuthenticationTicket currentTicket = FormsAuthentication.Decrypt(httpCookie.Value);
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(currentTicket.Version, currentTicket.Name, currentTicket.IssueDate, currentTicket.Expiration, currentTicket.IsPersistent, userData);
            httpCookie.Value = FormsAuthentication.Encrypt(ticket);
            return httpCookie;
        }




        public static bool SendMail(MailMessage message)
        {

            SmtpClient client = new System.Net.Mail.SmtpClient(ConfigurationManager.AppSettings["smtpServer"]);

            try
            {

                client.Host = ConfigurationManager.AppSettings["smtpServer"];
                client.EnableSsl= Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);
                System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                // NetworkCred.UserName = "webmaster@questionaf.ca";
                //NetworkCred.Password = "xkc232v";
                NetworkCred.UserName = ConfigurationManager.AppSettings["smtpUser"];
                NetworkCred.Password = ConfigurationManager.AppSettings["smtpPassword"];
                
                client.UseDefaultCredentials = false;
                client.Credentials = NetworkCred;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                //client.Port = 587;
                client.Port = Convert.ToInt32(ConfigurationManager.AppSettings["smtpPort"]);

                client.Timeout = 20000;
                client.EnableSsl = true;
                
                client.Send(message);

                return true;
            }
            catch (Exception e)
            {
                client = null;
                String Error = e.Message.ToString();
                //Utility.WriteToLog("SendMail Error: " + Error);

                return false;
            }

/*
            MailMessage message = new MailMessage();

            //Adding recipients
            message.To.Add(new MailAddress("lig@chrc.net"));

            //Adding Subject
            message.Subject = "This is the Subject line";

            //Adding sender
            message.From = new MailAddress("webmaster@chrc.net");

            //Adding body text
            message.Body = "content";

            //Configuring SMTP
            SmtpClient smtp = new System.Net.Mail.SmtpClient("outlook.office365.com");
            smtp.EnableSsl = true;
            smtp.Port = 587;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

            //Setting credentials
            //IMPORTANT!!! You must use your tenant crendential using the email address format and NOT the domain username
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("webmaster@chrc.net", "N0rm@lB!0m3tr!c");


            try
            {
                //Sending message
                smtp.Send(message);
                return true;
            }


            catch (Exception ex)
            {
                return false;
            }*/



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

        private static void Log(string logMessage, TextWriter w)
        {
            w.Write("\r\nLog Entry : ");
            w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                DateTime.Now.ToLongDateString());
            w.WriteLine("  :");
            w.WriteLine("  :{0}", logMessage);
            w.WriteLine("-------------------------------");
        }
        public static void SendRegisteredEmail(EventRequestModel pr, UserModel um, string ProgramName, string Speaker2Name, string ModeratorName, string SessionCredit, Controller controller)
        {
            try
            {
                string date1 = ConvertTimetoProperFormat(pr.ProgramDate1);

                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                mailMessage.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["emailGeneral"]);
                //Production Import for Dija
               mailMessage.To.Add(new System.Net.Mail.MailAddress(um.EmailAddress));
               // mailMessage.To.Add(new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["AdminEmail"]));
                mailMessage.Bcc.Add(new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["AdminEmail"]));
                mailMessage.Subject = ProgramName + " - Invitation to Present - ProgramDate: " + date1 + " -PLEASE REVIEW AND RESPOND";
                mailMessage.IsBodyHtml = true;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(RegisteredEmailBody(pr, um, ProgramName, Speaker2Name, ModeratorName, SessionCredit), null, "text/html");
                mailMessage.AlternateViews.Add(htmlView);
                if (!string.IsNullOrEmpty(pr.SessionAgendaFileName))
                {
                    string FileName = GetFileNameForEmailAttachment(pr.SessionAgendaFileName, pr.ProgramRequestID);
                    mailMessage.Attachments.Add(new Attachment(controller.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["UserFileUploadPath"] + FileName)));
                }
                UserHelper.SendMail(mailMessage);
            }

            catch (Exception e)
            {
                string msg = e.Message;
            }
        }
        public static void SendRegistrationNotComplete(EventRequestModel pr, UserModel um, string ProgramName, string Speaker2Name, string ModeratorName, string SessionCredit, Controller controller)
        {

            try
            {
                string date1 = ConvertTimetoProperFormat(pr.ProgramDate1);

                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                mailMessage.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["emailGeneral"]);
                //Production Import for Dija
                 mailMessage.To.Add(new System.Net.Mail.MailAddress(um.EmailAddress));
              //  mailMessage.To.Add(new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["AdminEmail"]));
                mailMessage.Bcc.Add(new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["AdminEmail"]));
                mailMessage.Subject = ProgramName + " - Invitation to Present - ProgramDate: " + date1 + " -PLEASE REVIEW AND RESPOND";
                mailMessage.IsBodyHtml = true;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(RegistrationNotCompleteBody(pr, um, ProgramName,Speaker2Name,  ModeratorName, SessionCredit), null, "text/html");
                mailMessage.AlternateViews.Add(htmlView);
                if (!string.IsNullOrEmpty(pr.SessionAgendaFileName))
                {
                    string FileName = GetFileNameForEmailAttachment(pr.SessionAgendaFileName, pr.ProgramRequestID);
                    mailMessage.Attachments.Add(new Attachment(controller.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["UserFileUploadPath"] + FileName)));
                }
                UserHelper.SendMail(mailMessage);
            }

            catch (Exception e)
            {
                string msg = e.Message;
            }
        }
        public static void SendNotRegistered(EventRequestModel pr, UserModel um, string ProgramName, string Speaker2Name, string ModeratorName, string SessionCredit, Controller controller)
        {
            try
            {
                string date1 = ConvertTimetoProperFormat(pr.ProgramDate1);

                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                mailMessage.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["emailGeneral"]);
                //Production Import for Dija
                 mailMessage.To.Add(new System.Net.Mail.MailAddress(um.EmailAddress));
                //mailMessage.To.Add(new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["AdminEmail"]));
                mailMessage.Bcc.Add(new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["AdminEmail"]));
                mailMessage.Subject = ProgramName + " - Invitation to Present - ProgramDate: " + date1 + " -PLEASE REVIEW AND RESPOND";
                mailMessage.IsBodyHtml = true;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(NotRegisteredBody(pr, um, ProgramName, Speaker2Name, ModeratorName, SessionCredit), null, "text/html");
                mailMessage.AlternateViews.Add(htmlView);
                if (!string.IsNullOrEmpty(pr.SessionAgendaFileName))
                {
                    string FileName = GetFileNameForEmailAttachment(pr.SessionAgendaFileName, pr.ProgramRequestID);
                    mailMessage.Attachments.Add(new Attachment(controller.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["UserFileUploadPath"] + FileName)));
                }

                UserHelper.SendMail(mailMessage);
            }
            catch (Exception e)
            {
                string msg = e.Message;

            }
        }
        public static void SelectInvitationOption(EventRequestModel pr,string Speaker2Name, string ModeratorName, string SessionCredit, Controller controller)
        {
            ProgramRepository repo = new ProgramRepository();
            UserRepository userRepo = new UserRepository();


            string ProgramName = repo.GetProgramRequestName(pr.ProgramID);
            UserModel um = new UserModel();

            if (pr != null)
            {
                um = userRepo.GetUserForConfirmEmail(pr.ProgramSpeakerID);
                //get the id from Userinfo
                string SendEmail = userRepo.CheckUserStatusForEmail(um.UserID);

                if (SendEmail.Equals("RegisteredCompleted"))
                {
                    SendRegisteredEmail(pr, um, ProgramName, Speaker2Name, ModeratorName, SessionCredit, controller);
                }
                if (SendEmail.Equals("RegistrationNotComplete"))
                {
                    SendRegistrationNotComplete(pr, um, ProgramName, Speaker2Name,  ModeratorName, SessionCredit, controller);
                }

                if (SendEmail.Equals("NotRegistered"))
                {
                    SendNotRegistered(pr, um, ProgramName, Speaker2Name, ModeratorName, SessionCredit, controller);
                }
            }

        }
        private static string RegisteredEmailBody(EventRequestModel pr, UserModel um, string ProgramName, string Speaker2Name, string ModeratorName, string SessionCredits)
        {
            string html = string.Empty;


            string SpeakerConfirmUrl = System.Configuration.ConfigurationManager.AppSettings["SpeakerConfirmUrl"];

            string Materials = "";



            if (!string.IsNullOrEmpty(pr.SessionAgendaFileName))
            {
                Materials = Path.GetFileName(pr.SessionAgendaFileName);


            }

            if (string.IsNullOrEmpty(pr.ProgramDate2) && (string.IsNullOrEmpty(pr.ProgramDate3)))
            {
                //Email1 option4
                html = @"<table width = '900' border = '0' cellspacing = '0' cellpadding = '10'>
				<tr>                
				<td align = 'center'><strong style = 'font-size: 30px; font-family: Arial, Helvetica, sans-serif;'>  Registration Completed </strong></td>                         
				 </tr>                       
		  
				 <tr>                         
				<td align = 'left'> 					
					
				<br /> 
		
				<p>Dear Dr. {LastName}, </p> 
				<p>You are invited to present the accredited BMS/Pfizer CPD program entitled {ProgramName}. 
				Please review the details and confirm your availability by clicking the Confirm or Not Available button </p> <br />
			
					 <table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
							<a style='display: block;color: #ffffff;font-size: 10px;text-decoration: none;' 
							href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate={querydate1}'>
								Confirm
							</a>
														
						</td>
													
						<td>
														
								&nbsp;&nbsp;&nbsp;&nbsp;Program Date : {SessionDate1} <br /> 
								&nbsp;&nbsp;&nbsp;&nbsp;Meal Start Time : {MealStartTime} &nbsp;&nbsp;&nbsp;&nbsp;Session Start Time : {SessionStartTime} &nbsp;&nbsp;&nbsp;&nbsp; Session End Time : {SessionEndTime}
						</td>
					</tr>
				</table>
				
											
				<table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
							<a style='display:block; color: #ffffff; font-size: 10px;' href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate=NotAvailable' >
									Not Available
								</a>
 
							</td>
 
							<td>
									&nbsp; &nbsp; &nbsp; &nbsp; Not Available

						</td>

					</tr>

				</table>

				<hr>


			Program Location:</strong> <span><strong> {LocationName} </strong> &nbsp; </span > &nbsp;<br />
			Your Honorarium:</strong> <span><strong> {Honorarium} </strong> &nbsp; </span> &nbsp;<br />
			AdditionalPresenter: if applicable </strong><span><strong> {Speaker2Name} </strong> &nbsp; </span> &nbsp;<span><strong> {ModeratorName} </strong> &nbsp; </span> &nbsp;<br />
			Materials to be presented: </strong> <span><strong> {SessionCredits} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact: if applicable </strong> <span><strong> {SessionContact} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact Number:</strong> <span><strong>{ContactPhoneNumber} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact Address:</strong> <span><strong>{ContactEmailAddress} </strong> &nbsp; </span > &nbsp;<br />                                                                     

		   <p> Please visit <a href=https://speaker.clinicalconundrums.ca'> https://speaker.clinicalconundrums.ca </a> to gain access to the program materials. </p>
																				 
		   <p> Should you have any questions, please do not hesitate to contact your CHRC program coordinator: {SessionContact} – email address: {ContactEmailAddress}
				phone number: {ContactPhoneNumber}


		</p>


		</td>

	</tr>


</table> ";
            }


            if (!string.IsNullOrEmpty(pr.ProgramDate2) && (string.IsNullOrEmpty(pr.ProgramDate3)))
            {
                //Email1 option2
                html = html = html = @"<table width = '900' border = '0' cellspacing = '0' cellpadding = '10'>
				<tr>                
				<td align = 'center'><strong style = 'font-size: 30px; font-family: Arial, Helvetica, sans-serif;'>  Registration Completed  </strong></td>                         
				 </tr>                       
		  
				 <tr>                         
				<td align = 'left'> 					
					
				<br /> 
		
				<p>Dear Dr. {LastName}, </p> 
				<p>You are invited to present the accredited CPD program entitled {ProgramName}. 
				Please review the details and confirm your availability by clicking the Confirm or Not Available button </p> <br />
			 <table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
							<a style='display: block;color: #ffffff;font-size: 10px;text-decoration: none;' 
							href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate={querydate1}'>
								Confirm
							</a>
														
						</td>
													
						<td>
														
								&nbsp;&nbsp;&nbsp;&nbsp;Program Date : {SessionDate1} <br /> 
								&nbsp;&nbsp;&nbsp;&nbsp;Meal Start Time : {MealStartTime} &nbsp;&nbsp;&nbsp;&nbsp;Session Start Time : {SessionStartTime} &nbsp;&nbsp;&nbsp;&nbsp; Session End Time : {SessionEndTime}
						</td>
					</tr>
				</table>
				<table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
							<a style='display: block;color: #ffffff;font-size: 10px;text-decoration: none;'
							href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate={querydate2}'>
								Confirm
							</a>
														
						</td>
													
						<td>														
								&nbsp;&nbsp;&nbsp;&nbsp;Program Date(2nd Option) : {SessionDate2} <br /> 
								&nbsp;&nbsp;&nbsp;&nbsp;Meal Start Time : {MealStartTime} &nbsp;&nbsp;&nbsp;&nbsp;Session Start Time : {SessionStartTime} &nbsp;&nbsp;&nbsp;&nbsp; Session End Time : {SessionEndTime}
						</td>	
					</tr>
				</table>
				
											
				<table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
							<a style='display:block; color: #ffffff; font-size: 10px;' href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate=NotAvailable' >
									Not Available
								</a>
 
							</td>
 
							<td>
									&nbsp; &nbsp; &nbsp; &nbsp; Not Available

						</td>

					</tr>

				</table>




				<hr>


			Program Location:</strong> <span><strong> {LocationName} </strong> &nbsp; </span > &nbsp;<br />
			Your Honorarium:</strong> <span><strong> {Honorarium} </strong> &nbsp; </span> &nbsp;<br />
			AdditionalPresenter: if applicable </strong><span><strong> {Speaker2Name} </strong> &nbsp; </span> &nbsp;<span><strong> {ModeratorName} </strong> &nbsp; </span> &nbsp;<br />
			Materials to be presented: </strong> <span><strong> {SessionCredits} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact: if applicable </strong> <span><strong> {SessionContact} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact Number:</strong> <span><strong>{ContactPhoneNumber} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact Address:</strong> <span><strong>{ContactEmailAddress} </strong> &nbsp; </span > &nbsp;<br />                                                                     

		   <p> Please visit <a href='https://speaker.clinicalconundrums.ca'>speaker.clinicalconundrums.ca</a> to gain access to the program materials. </p>
																				 
		   <p> Should you have any questions, please do not hesitate to contact your CHRC program coordinator: {SessionContact} – email address: {ContactEmailAddress}
				phone number: {ContactPhoneNumber}


		</p>


		</td>

	</tr>


</table> ";
            }

            if (!string.IsNullOrEmpty(pr.ProgramDate3) && (string.IsNullOrEmpty(pr.ProgramDate2)))
            {


                //Email1 option3
                html = @"<table width = '900' border = '0' cellspacing = '0' cellpadding = '10'>
				<tr>                
				<td align = 'center'><strong style = 'font-size: 30px; font-family: Arial, Helvetica, sans-serif;'>  Registration Completed  </strong></td>                         
				 </tr>                       
		  
				 <tr>                         
				<td align = 'left'> 					
					
				<br /> 
		
				<p>Dear Dr. {LastName}, </p> 
				<p>You are invited to present the accredited CPD program entitled {ProgramName}. 
				Please review the details and confirm your availability by clicking the Confirm or Not Available button </p> <br />
			
			  <table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
							<a style='display: block;color: #ffffff;font-size: 10px;text-decoration: none;' 
							href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate={querydate1}'>
								Confirm
							</a>
														
						</td>
													
						<td>
														
								&nbsp;&nbsp;&nbsp;&nbsp;Program Date : {SessionDate1} <br /> 
								&nbsp;&nbsp;&nbsp;&nbsp;Meal Start Time : {MealStartTime} &nbsp;&nbsp;&nbsp;&nbsp;Session Start Time : {SessionStartTime} &nbsp;&nbsp;&nbsp;&nbsp; Session End Time : {SessionEndTime}
						</td>
					</tr>
				</table>
				
				<table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
						<a style='display: block;color: #ffffff;font-size: 10px;text-decoration: none;'
							href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate={querydate3}'>
								Confirm
							</a>
														
						</td>
													
						<td>
														
								&nbsp;&nbsp;&nbsp;&nbsp;Program Date(3rd Option) : {SessionDate3} <br /> 
								&nbsp;&nbsp;&nbsp;&nbsp;Meal Start Time : {MealStartTime} &nbsp;&nbsp;&nbsp;&nbsp;Session Start Time : {SessionStartTime} &nbsp;&nbsp;&nbsp;&nbsp; Session End Time : {SessionEndTime}
						</td>
														
					</tr>
				</table>
			

											
				<table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
							<a style='display:block; color: #ffffff; font-size: 10px;' href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate=NotAvailable' >
									Not Available
								</a>
 
							</td>
 
							<td>
									&nbsp; &nbsp; &nbsp; &nbsp; Not Available

						</td>

					</tr>

				</table>


				<hr>


			Program Location:</strong> <span><strong> {LocationName} </strong> &nbsp; </span > &nbsp;<br />
			Your Honorarium:</strong> <span><strong> {Honorarium} </strong> &nbsp; </span> &nbsp;<br />
			AdditionalPresenter: if applicable </strong><span><strong> {Speaker2Name} </strong> &nbsp; </span> &nbsp;<span><strong> {ModeratorName} </strong> &nbsp; </span> &nbsp;<br />
			Materials to be presented: </strong> <span><strong> {SessionCredits} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact: if applicable </strong> <span><strong> {SessionContact} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact Number:</strong> <span><strong>{ContactPhoneNumber} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact Address:</strong> <span><strong>{ContactEmailAddress} </strong> &nbsp; </span > &nbsp;<br />                                                                     

		   <p> Please visit <a href='https://speaker.clinicalconundrums.ca'>speaker.clinicalconundrums.ca</a> to gain access to the program materials. </p>
																				 
		   <p> Should you have any questions, please do not hesitate to contact your CHRC program coordinator: {SessionContact} – email address: {ContactEmailAddress}
				phone number: {ContactPhoneNumber}


		</p>


		</td>

	</tr>


</table> ";

            }

            if (!string.IsNullOrEmpty(pr.ProgramDate3) && (!string.IsNullOrEmpty(pr.ProgramDate2)))
            {
                //EmailText1
                html = @"<table width = '900' border = '0' cellspacing = '0' cellpadding = '10'>
				<tr>                
				<td align = 'center'><strong style = 'font-size: 30px; font-family: Arial, Helvetica, sans-serif;'> Registration Completed  </strong></td>                         
				 </tr>                       
		  
				 <tr>                         
				<td align = 'left'> 					
					
				<br /> 
		
				<p>Dear Dr. {LastName}, </p> 
				<p>You are invited to present the accredited CPD program entitled {ProgramName}. 
				Please review the details and confirm your availability by clicking the Confirm or Not Available button </p> <br />
			
					<table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
							<a style='display: block;color: #ffffff;font-size: 10px;text-decoration: none;' 
							href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate={querydate1}'>
								Confirm
							</a>
														
						</td>
													
						<td>
														
								&nbsp;&nbsp;&nbsp;&nbsp;Program Date : {SessionDate1} <br /> 
								&nbsp;&nbsp;&nbsp;&nbsp;Meal Start Time : {MealStartTime} &nbsp;&nbsp;&nbsp;&nbsp;Session Start Time : {SessionStartTime} &nbsp;&nbsp;&nbsp;&nbsp; Session End Time : {SessionEndTime}
						</td>
					</tr>
				</table>
				<table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
							<a style='display: block;color: #ffffff;font-size: 10px;text-decoration: none;'
							href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate={querydate2}'>
								Confirm
							</a>
														
						</td>
													
						<td>														
								&nbsp;&nbsp;&nbsp;&nbsp;Program Date(2nd Option) : {SessionDate2} <br /> 
								&nbsp;&nbsp;&nbsp;&nbsp;Meal Start Time : {MealStartTime} &nbsp;&nbsp;&nbsp;&nbsp;Session Start Time : {SessionStartTime} &nbsp;&nbsp;&nbsp;&nbsp; Session End Time : {SessionEndTime}
						</td>	
					</tr>
				</table>
				<table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
						<a style='display: block;color: #ffffff;font-size: 10px;text-decoration: none;'
							href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate={querydate3}'>
								Confirm
							</a>
														
						</td>
													
						<td>
														
								&nbsp;&nbsp;&nbsp;&nbsp;Program Date(3rd Option) : {SessionDate3} <br /> 
								&nbsp;&nbsp;&nbsp;&nbsp;Meal Start Time : {MealStartTime} &nbsp;&nbsp;&nbsp;&nbsp;Session Start Time : {SessionStartTime} &nbsp;&nbsp;&nbsp;&nbsp; Session End Time : {SessionEndTime}
						</td>
														
					</tr>
				</table>
											
				<table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
							<a style='display:block; color: #ffffff; font-size: 10px;' href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate=NotAvailable' >
									Not Available
								</a>
 
							</td>
 
							<td>
									&nbsp; &nbsp; &nbsp; &nbsp; Not Available

						</td>

					</tr>

				</table>


				<hr>


			Program Location:</strong> <span><strong> {LocationName} </strong> &nbsp; </span > &nbsp;<br />
			Your Honorarium:</strong> <span><strong> {Honorarium} </strong> &nbsp; </span> &nbsp;<br />
			AdditionalPresenter: if applicable </strong><span><strong> {Speaker2Name} </strong> &nbsp; </span> &nbsp;<span><strong> {ModeratorName} </strong> &nbsp; </span> &nbsp;<br />
			Materials to be presented: </strong> <span><strong> {SessionCredits} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact: if applicable </strong> <span><strong> {SessionContact} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact Number:</strong> <span><strong>{ContactPhoneNumber} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact Address:</strong> <span><strong>{ContactEmailAddress} </strong> &nbsp; </span > &nbsp;<br />                                                                     

		   <p> Please visit <a href='https://speaker.clinicalconundrums.ca'>speaker.clinicalconundrums.ca</a> to gain access to the program materials. </p>
																				 
		   <p> Should you have any questions, please do not hesitate to contact your CHRC program coordinator: {SessionContact} – email address: {ContactEmailAddress}
				phone number: {ContactPhoneNumber}


		</p>


		</td>

	</tr>


</table> ";


            }



            html = html.Replace("{ProgramRequestID}", pr.ProgramRequestID.ToString());
            html = html.Replace("{LastName}", um.LastName);
            html = html.Replace("{ProgramName}", ProgramName);

            html = html.Replace("{querydate1}", pr.ProgramDate1);
            html = html.Replace("{querydate2}", pr.ProgramDate2);
            html = html.Replace("{querydate3}", pr.ProgramDate3);

            html = html.Replace("{SessionDate1}", ConvertTimetoProperFormat(pr.ProgramDate1));
            if (!string.IsNullOrEmpty(pr.ProgramDate2))
            {
                html = html.Replace("{SessionDate2}", ConvertTimetoProperFormat(pr.ProgramDate2));
            }
            if (!string.IsNullOrEmpty(pr.ProgramDate3))
            {
                html = html.Replace("{SessionDate3}", ConvertTimetoProperFormat(pr.ProgramDate3));
            }



            html = html.Replace("{MealStartTime}", pr.MealStartTime);
            html = html.Replace("{SessionStartTime}", pr.ProgramStartTime);
            html = html.Replace("{SessionEndTime}", pr.ProgramEndTime);


            html = html.Replace("{LocationName}", pr.LocationName);
            html = html.Replace("{Honorarium}", um.SpeakerHonariumRange);
            html = html.Replace("{Speaker2Name}", Speaker2Name);
            html = html.Replace("{ModeratorName}", ModeratorName);
            html = html.Replace("{Materials}", Materials);
            html = html.Replace("{SessionContact}", pr.ContactName);
            html = html.Replace("{ContactPhoneNumber}", pr.ContactPhone);
            html = html.Replace("{ContactEmailAddress}", pr.ContactEmail);
            html = html.Replace("{SpeakerConfirmUrl}", SpeakerConfirmUrl);

            html = html.Replace("{SessionCredits}", SessionCredits);


            return html;

        }
        private static string RegistrationNotCompleteBody(EventRequestModel pr, UserModel um, string ProgramName, string Speaker2Name, string ModeratorName, string SessionCredits)
        {
            string html = string.Empty;

            string Materials = "";

            string SpeakerConfirmUrl = System.Configuration.ConfigurationManager.AppSettings["SpeakerConfirmUrl"];

            if (!string.IsNullOrEmpty(pr.SessionAgendaFileName))
            {
                Materials = Path.GetFileName(pr.SessionAgendaFileName);


            }

            if (string.IsNullOrEmpty(pr.ProgramDate2) && (string.IsNullOrEmpty(pr.ProgramDate3)))
            {
                //Email2Option4
                html = html = @"<table width = '900' border = '0' cellspacing = '0' cellpadding = '10'>
				<tr>                
				<td align = 'center'><strong style = 'font-size: 30px; font-family: Arial, Helvetica, sans-serif;'>  Registration Not Completed </strong></td>                         
				 </tr>                       
		  
				 <tr>                         
				<td align = 'left'> 					
					
				<br /> 
		
				<p>Dear Dr. {LastName}, </p> 
				<p>You are invited to present the accredited BMS/Pfizer CPD program entitled {ProgramName}. 
				Please review the details and confirm your availability by clicking the Confirm or Not Available button </p> <br />
			
					<table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
							<a style='display: block;color: #ffffff;font-size: 10px;text-decoration: none;' 
							href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate={querydate1}'>
								Confirm
							</a>
														
						</td>
													
						<td>
														
								&nbsp;&nbsp;&nbsp;&nbsp;Program Date : {SessionDate1} <br /> 
								&nbsp;&nbsp;&nbsp;&nbsp;Meal Start Time : {MealStartTime} &nbsp;&nbsp;&nbsp;&nbsp;Session Start Time : {SessionStartTime} &nbsp;&nbsp;&nbsp;&nbsp; Session End Time : {SessionEndTime}
						</td>
					</tr>
				</table>
				
											
				<table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
							<a style='display:block; color: #ffffff; font-size: 10px;' href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate=NotAvailable' >
									Not Available
								</a>
 
							</td>
 
							<td>
									&nbsp; &nbsp; &nbsp; &nbsp; Not Available

						</td>

					</tr>

				</table>


				<hr>


			Program Location:</strong> <span><strong> {LocationName} </strong> &nbsp; </span > &nbsp;<br />
			Your Honorarium:</strong> <span><strong> {Honorarium} </strong> &nbsp; </span> &nbsp;<br />
			AdditionalPresenter: if applicable </strong><span><strong> {Speaker2Name} </strong> &nbsp; </span><span><strong> {ModeratorName} </strong> &nbsp; </span> &nbsp;<br />
			Materials to be presented: </strong> <span><strong> {SessionCredits} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact: if applicable </strong> <span><strong> {SessionContact} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact Number:</strong> <span><strong>{ContactPhoneNumber} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact Address:</strong> <span><strong>{ContactEmailAddress} </strong> &nbsp; </span > &nbsp;<br />                                                                     

		   <p> Please visit <a href='https://speaker.clinicalconundrums.ca'>speaker.clinicalconundrums.ca</a> to gain access to the program materials. </p>
																				 
		   <p>Next Steps: Please visit <a href='https://speaker.clinicalconundrums.ca'>speaker.clinicalconundrums.ca</a> and complete the required forms (payee and COI forms) at your earliest convenience.
			The Speaker Resource Centre includes all the pertinent program materials for your reference and perusal.</p>  
			<p>
			Should you have any questions, please do not hesitate to contact your CHRC program coordinator: {SessionContact} – email address: {ContactEmailAddress}
			phone number: {ContactPhoneNumber}

			</p>


		</td>

	</tr>


</table> ";
            }


            if (!string.IsNullOrEmpty(pr.ProgramDate2) && (string.IsNullOrEmpty(pr.ProgramDate3)))
            {


                //Email2Option2
                html = @"<table width = '900' border = '0' cellspacing = '0' cellpadding = '10'>
				<tr>                
				<td align = 'center'><strong style = 'font-size: 30px; font-family: Arial, Helvetica, sans-serif;'>  Registration Not Completed </strong></td>                         
				 </tr>                       
		  
				 <tr>                         
				<td align = 'left'> 					
					
				<br /> 
		
				<p>Dear Dr. {LastName}, </p> 
				<p>You are invited to present the accredited BMS/Pfizer CPD program entitled {ProgramName}. 
				Please review the details and confirm your availability by clicking the Confirm or Not Available button </p> <br />
			
					<table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
							<a style='display: block;color: #ffffff;font-size: 10px;text-decoration: none;' 
							href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate={querydate1}'>
								Confirm
							</a>
														
						</td>
													
						<td>
														
								&nbsp;&nbsp;&nbsp;&nbsp;Program Date : {SessionDate1} <br /> 
								&nbsp;&nbsp;&nbsp;&nbsp;Meal Start Time : {MealStartTime} &nbsp;&nbsp;&nbsp;&nbsp;Session Start Time : {SessionStartTime} &nbsp;&nbsp;&nbsp;&nbsp; Session End Time : {SessionEndTime}
						</td>
					</tr>
				</table>
				<table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
							<a style='display: block;color: #ffffff;font-size: 10px;text-decoration: none;'
							href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate={querydate2}'>
								Confirm
							</a>
														
						</td>
													
						<td>														
								&nbsp;&nbsp;&nbsp;&nbsp;Program Date(2nd Option) : {SessionDate2} <br /> 
								&nbsp;&nbsp;&nbsp;&nbsp;Meal Start Time : {MealStartTime} &nbsp;&nbsp;&nbsp;&nbsp;Session Start Time : {SessionStartTime} &nbsp;&nbsp;&nbsp;&nbsp; Session End Time : {SessionEndTime}
						</td>	
					</tr>
				</table>
				
											
				<table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
							<a style='display:block; color: #ffffff; font-size: 10px;' href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate=NotAvailable' >
									Not Available
								</a>
 
							</td>
 
							<td>
									&nbsp; &nbsp; &nbsp; &nbsp; Not Available

						</td>

					</tr>

				</table>


				<hr>


			Program Location:</strong> <span><strong> {LocationName} </strong> &nbsp; </span > &nbsp;<br />
			Your Honorarium:</strong> <span><strong> {Honorarium} </strong> &nbsp; </span> &nbsp;<br />
			AdditionalPresenter: if applicable </strong><span><strong> {Speaker2Name} </strong> &nbsp; </span><span><strong> {ModeratorName} </strong> &nbsp; </span> &nbsp;<br />
			Materials to be presented: </strong> <span><strong> {SessionCredits} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact: if applicable </strong> <span><strong> {SessionContact} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact Number:</strong> <span><strong>{ContactPhoneNumber} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact Address:</strong> <span><strong>{ContactEmailAddress} </strong> &nbsp; </span > &nbsp;<br />                                                                     

		   <p> Please visit <a href='https://speaker.clinicalconundrums.ca'>speaker.clinicalconundrums.ca</a> to gain access to the program materials. </p>
																				 
		   <p>Next Steps: Please visit <a href='https://speaker.clinicalconundrums.ca'>speaker.clinicalconundrums.ca</a> and complete the required forms (payee and COI forms) at your earliest convenience.
			The Speaker Resource Centre includes all the pertinent program materials for your reference and perusal.</p>  
			<p>
			Should you have any questions, please do not hesitate to contact your CHRC program coordinator: {SessionContact} – email address: {ContactEmailAddress}
			phone number: {ContactPhoneNumber}

			</p>


		</td>

	</tr>


</table> ";
            }

            if (!string.IsNullOrEmpty(pr.ProgramDate3) && (string.IsNullOrEmpty(pr.ProgramDate2)))
            {

                //Email2Option 3
                html = @"<table width = '900' border = '0' cellspacing = '0' cellpadding = '10'>
				<tr>                
				<td align = 'center'><strong style = 'font-size: 30px; font-family: Arial, Helvetica, sans-serif;'>  Registration Not Completed </strong></td>                         
				 </tr>                       
		  
				 <tr>                         
				<td align = 'left'> 					
					
				<br /> 
		
				<p>Dear Dr. {LastName}, </p> 
				<p>You are invited to present the accredited CPD program entitled {ProgramName}. 
				Please review the details and confirm your availability by clicking the Confirm or Not Available button </p> <br />
			
					<table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
							<a style='display: block;color: #ffffff;font-size: 10px;text-decoration: none;' 
							href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate={querydate1}'>
								Confirm
							</a>
														
						</td>
													
						<td>
														
								&nbsp;&nbsp;&nbsp;&nbsp;Program Date : {SessionDate1} <br /> 
								&nbsp;&nbsp;&nbsp;&nbsp;Meal Start Time : {MealStartTime} &nbsp;&nbsp;&nbsp;&nbsp;Session Start Time : {SessionStartTime} &nbsp;&nbsp;&nbsp;&nbsp; Session End Time : {SessionEndTime}
						</td>
					</tr>
				</table>
				
				<table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
						<a style='display: block;color: #ffffff;font-size: 10px;text-decoration: none;'
							href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate={querydate3}'>
								Confirm
							</a>
														
						</td>
													
						<td>
														
								&nbsp;&nbsp;&nbsp;&nbsp;Program Date(3rd Option) : {SessionDate3} <br /> 
								&nbsp;&nbsp;&nbsp;&nbsp;Meal Start Time : {Meal3} &nbsp;&nbsp;&nbsp;&nbsp;Session Start Time : {ProgramStart3} &nbsp;&nbsp;&nbsp;&nbsp; Session End Time : {ProgramEnd3}
						</td>
														
					</tr>
				</table>
											
				<table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
							<a style='display:block; color: #ffffff; font-size: 10px;' href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate=NotAvailable' >
									Not Available
								</a>
 
							</td>
 
							<td>
									&nbsp; &nbsp; &nbsp; &nbsp; Not Available

						</td>

					</tr>

				</table>


				<hr>


			Program Location:</strong> <span><strong> {LocationName} </strong> &nbsp; </span > &nbsp;<br />
			Your Honorarium:</strong> <span><strong> {Honorarium} </strong> &nbsp; </span> &nbsp;<br />
			AdditionalPresenter: if applicable </strong><span><strong> {Speaker2Name} </strong> &nbsp; </span><span><strong> {ModeratorName} </strong> &nbsp; </span> &nbsp;<br />
			Materials to be presented: </strong> <span><strong> {SessionCredits} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact: if applicable </strong> <span><strong> {SessionContact} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact Number:</strong> <span><strong>{ContactPhoneNumber} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact Address:</strong> <span><strong>{ContactEmailAddress} </strong> &nbsp; </span > &nbsp;<br />                                                                     

		   <p> Please visit <a href='https://speaker.clinicalconundrums.ca'>speaker.clinicalconundrums.ca</a> to gain access to the program materials. </p>
																				 
		   <p>Next Steps: Please visit <a href=https://speaker.clinicalconundrums.ca'>speaker.clinicalconundrums.ca</a> and complete the required forms (payee and COI forms) at your earliest convenience.
			The Speaker Resource Centre includes all the pertinent program materials for your reference and perusal.</p>  
			<p>
			Should you have any questions, please do not hesitate to contact your CHRC program coordinator: {SessionContact} – email address: {ContactEmailAddress}
			phone number: {ContactPhoneNumber}

			</p>


		</td>

	</tr>


</table> ";
            }

            if (!string.IsNullOrEmpty(pr.ProgramDate3) && (!string.IsNullOrEmpty(pr.ProgramDate2)))
            {
                //EmailText2
                html = @"<table width = '900' border = '0' cellspacing = '0' cellpadding = '10'>
				<tr>                
				<td align = 'center'><strong style = 'font-size: 30px; font-family: Arial, Helvetica, sans-serif;'>  Registration Not Completed </strong></td>                         
				 </tr>                       
		  
				 <tr>                         
				<td align = 'left'> 					
					
				<br /> 
		
				<p>Dear Dr. {LastName}, </p> 
				<p>You are invited to present the accredited CPD program entitled {ProgramName}. 
				Please review the details and confirm your availability by clicking the Confirm or Not Available button </p> <br />
			
					   <table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
							<a style='display: block;color: #ffffff;font-size: 10px;text-decoration: none;' 
							href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate={querydate1}'>
								Confirm
							</a>
														
						</td>
													
						<td>
														
								&nbsp;&nbsp;&nbsp;&nbsp;Program Date : {SessionDate1} <br /> 
								&nbsp;&nbsp;&nbsp;&nbsp;Meal Start Time : {MealStartTime} &nbsp;&nbsp;&nbsp;&nbsp;Session Start Time : {SessionStartTime} &nbsp;&nbsp;&nbsp;&nbsp; Session End Time : {SessionEndTime}
						</td>
					</tr>
				</table>
				<table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
							<a style='display: block;color: #ffffff;font-size: 10px;text-decoration: none;'
							href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate={querydate2}'>
								Confirm
							</a>
														
						</td>
													
						<td>														
								&nbsp;&nbsp;&nbsp;&nbsp;Program Date(2nd Option) : {SessionDate2} <br /> 
								&nbsp;&nbsp;&nbsp;&nbsp;Meal Start Time : {MealStartTime} &nbsp;&nbsp;&nbsp;&nbsp;Session Start Time : {SessionStartTime} &nbsp;&nbsp;&nbsp;&nbsp; Session End Time : {SessionEndTime}
						</td>	
					</tr>
				</table>
				<table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
						<a style='display: block;color: #ffffff;font-size: 10px;text-decoration: none;'
							href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate={querydate3}'>
								Confirm
							</a>
														
						</td>
													
						<td>
														
								&nbsp;&nbsp;&nbsp;&nbsp;Program Date(3rd Option) : {SessionDate3} <br /> 
								&nbsp;&nbsp;&nbsp;&nbsp;Meal Start Time : {MealStartTime} &nbsp;&nbsp;&nbsp;&nbsp;Session Start Time : {SessionStartTime} &nbsp;&nbsp;&nbsp;&nbsp; Session End Time : {SessionEndTime}
						</td>
														
					</tr>
				</table>
											
				<table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
							<a style='display:block; color: #ffffff; font-size: 10px;' href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate=NotAvailable' >
									Not Available
								</a>
 
							</td>
 
							<td>
									&nbsp; &nbsp; &nbsp; &nbsp; Not Available

						</td>

					</tr>

				</table>


				<hr>


			Program Location:</strong> <span><strong> {LocationName} </strong> &nbsp; </span > &nbsp;<br />
			Your Honorarium:</strong> <span><strong> {Honorarium} </strong> &nbsp; </span> &nbsp;<br />
			AdditionalPresenter: if applicable </strong><span><strong> {Speaker2Name} </strong> &nbsp; </span><span><strong> {ModeratorName} </strong> &nbsp; </span> &nbsp;<br />
			Materials to be presented: </strong> <span><strong> {SessionCredits} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact: if applicable </strong> <span><strong> {SessionContact} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact Number:</strong> <span><strong>{ContactPhoneNumber} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact Address:</strong> <span><strong>{ContactEmailAddress} </strong> &nbsp; </span > &nbsp;<br />                                                                     

		   <p> Please visit <a href='https://speaker.clinicalconundrums.ca'>speaker.clinicalconundrums.ca</a> to gain access to the program materials. </p>
																				 
		   <p>Next Steps: Please visit <a href='https://speaker.clinicalconundrums.ca'>speaker.clinicalconundrums.ca</a> and complete the required forms (payee and COI forms) at your earliest convenience.
			The Speaker Resource Centre includes all the pertinent program materials for your reference and perusal.</p>  
			<p>
			Should you have any questions, please do not hesitate to contact your CHRC program coordinator: {SessionContact} – email address: {ContactEmailAddress}
			phone number: {ContactPhoneNumber}

			</p>


		</td>

	</tr>


</table> ";

            }

            html = html.Replace("{ProgramRequestID}", pr.ProgramRequestID.ToString());
            html = html.Replace("{LastName}", um.LastName);
            html = html.Replace("{ProgramName}", ProgramName);

            html = html.Replace("{querydate1}", pr.ProgramDate1);
            html = html.Replace("{querydate2}", pr.ProgramDate2);
            html = html.Replace("{querydate3}", pr.ProgramDate3);

            html = html.Replace("{SessionDate1}", ConvertTimetoProperFormat(pr.ProgramDate1));
            if (!string.IsNullOrEmpty(pr.ProgramDate2))
            {
                html = html.Replace("{SessionDate2}", ConvertTimetoProperFormat(pr.ProgramDate2));
            }
            if (!string.IsNullOrEmpty(pr.ProgramDate3))
            {
                html = html.Replace("{SessionDate3}", ConvertTimetoProperFormat(pr.ProgramDate3));
            }

            html = html.Replace("{MealStartTime}", pr.MealStartTime);
            html = html.Replace("{SessionStartTime}", pr.ProgramStartTime);
            html = html.Replace("{SessionEndTime}", pr.ProgramEndTime);


            html = html.Replace("{LocationName}", pr.LocationName);
            html = html.Replace("{Honorarium}", um.SpeakerHonariumRange);
            html = html.Replace("{Speaker2Name}", Speaker2Name);
            html = html.Replace("{ModeratorName}", ModeratorName);
            html = html.Replace("{Materials}", Materials);
            html = html.Replace("{SessionContact}", pr.ContactName);
            html = html.Replace("{ContactPhoneNumber}", pr.ContactPhone);
            html = html.Replace("{ContactEmailAddress}", pr.ContactEmail);
            html = html.Replace("{SpeakerConfirmUrl}", SpeakerConfirmUrl);
            html = html.Replace("{SessionCredits}", SessionCredits);

            return html;

        }
        private static string NotRegisteredBody(EventRequestModel pr, UserModel um, string ProgramName,string Speaker2Name, string ModeratorName, string SessionCredits)
        {
            string html = string.Empty;

            string Materials = "";
            string SpeakerConfirmUrl = System.Configuration.ConfigurationManager.AppSettings["SpeakerConfirmUrl"];

            if (!string.IsNullOrEmpty(pr.SessionAgendaFileName))
            {
                Materials = Path.GetFileName(pr.SessionAgendaFileName);


            }

            if (string.IsNullOrEmpty(pr.ProgramDate2) && (string.IsNullOrEmpty(pr.ProgramDate3)))
            {
                //Email3 option 4
                html = @"<table width = '900' border = '0' cellspacing = '0' cellpadding = '10'>
				<tr>                
				<td align='center'><strong style='font-size: 30px; font-family: Arial, Helvetica, sans-serif;'>  Not Registered </strong></td>                         
				 </tr>                       
		  
				 <tr>                         
				<td align='left'> 					
					
				<br /> 
		
				<p>Dear Dr. {LastName}, </p> 
				<p>You are invited to present the accredited BMS/Pfizer CPD program entitled {ProgramName}. 
				Please review the details and confirm your availability by clicking the Confirm or Not Available button </p> <br />
			
				 <table>
					<tr>
						<td style='background-color:#4ecdc4; padding:0px 2px; text-align:center;'>
							<a style='display:block;color:#ffffff;font-size: 10px;text-decoration:none;' 
							href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate={querydate1}'>
								Confirm
							</a>
														
						</td>
													
						<td>
														
								&nbsp;&nbsp;&nbsp;&nbsp;Program Date : {SessionDate1} <br /> 
								&nbsp;&nbsp;&nbsp;&nbsp;Meal Start Time : {MealStartTime} &nbsp;&nbsp;&nbsp;&nbsp;Session Start Time : {SessionStartTime} &nbsp;&nbsp;&nbsp;&nbsp; Session End Time : {SessionEndTime}
						</td>
					</tr>
                 </table>
				
											
				<table>
					<tr>
						<td style='background-color:#4ecdc4; padding:0px 2px; text-align:center;'>
							<a style='display:block; color: #ffffff; font-size: 10px;' href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate=NotAvailable'>
									Not Available
								</a>
 
							</td>
 
							<td>
									&nbsp; &nbsp; &nbsp; &nbsp; Not Available

						</td>

					</tr>

				</table>


				<hr>


			Program Location: <span><strong> {LocationName} </strong> &nbsp; </span> &nbsp;<br />
			Your Honorarium: <span><strong> {Honorarium} </strong> &nbsp; </span> &nbsp;<br />
			AdditionalPresenter: if applicable </strong><span><strong> {Speaker2Name} </strong> &nbsp; </span><span><strong> {ModeratorName} </strong> &nbsp; </span> &nbsp;<br />
			Materials to be presented:  <span><strong> {SessionCredits} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact: if applicable  <span><strong> {SessionContact} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact Number: <span><strong>{ContactPhoneNumber} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact Address:<span><strong>{ContactEmailAddress} </strong> &nbsp; </span > &nbsp;<br />                                                                     

		   <p>Next Steps: </p> <br />


			1. Please visit <a href='{SpeakerActivation}'> {SpeakerActivation} </a> and register - your username is: {SpeakerEmail} <br />
			2. The Speaker Resource Centre includes all the pertinent program materials for your reference.<br />
			  Please log in <a href='{SpeakerSiteURL}'>{SpeakerSiteURL}</a> and complete the required forms (payee and COI forms) at your earliest convenience.<br /> 
			
			3. Access the Speaker Resource Centre to gain access to all the pertinent program materials for your reference. <br />
			
			Should you have any questions, please do not hesitate to contact your CHRC program coordinator: {SessionContact} – email address: {ContactEmailAddress}
			phone number: {ContactPhoneNumber}

		</p>


		</td>

	</tr>


</table> ";
            }


            if (!string.IsNullOrEmpty(pr.ProgramDate2) && (string.IsNullOrEmpty(pr.ProgramDate3)))
            {

                //Email3 option2
                html = @"<table width = '900' border = '0' cellspacing = '0' cellpadding = '10'>
				<tr>                
				<td align = 'center'><strong style = 'font-size: 30px; font-family: Arial, Helvetica, sans-serif;'>   Not Registered </strong></td>                         
				 </tr>                       
		  
				 <tr>                         
				<td align = 'left'> 					
					
				<br /> 
		
				<p>Dear Dr. {LastName}, </p> 
				<p>You are invited to present the accredited CPD program entitled {ProgramName}. 
				Please review the details and confirm your availability by clicking the Confirm or Not Available button </p> <br />
			
		 <table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
							<a style='display: block;color: #ffffff;font-size: 10px;text-decoration: none;' 
							href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate={querydate1}'>
								Confirm
							</a>
														
						</td>
													
						<td>
														
								&nbsp;&nbsp;&nbsp;&nbsp;Program Date : {SessionDate1} <br /> 
								&nbsp;&nbsp;&nbsp;&nbsp;Meal Start Time : {MealStartTime} &nbsp;&nbsp;&nbsp;&nbsp;Session Start Time : {SessionStartTime} &nbsp;&nbsp;&nbsp;&nbsp; Session End Time : {SessionEndTime}
						</td>
					</tr>
				</table>
				<table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
							<a style='display: block;color: #ffffff;font-size: 10px;text-decoration: none;'
							href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate={querydate2}'>
								Confirm
							</a>
														
						</td>
													
						<td>														
								&nbsp;&nbsp;&nbsp;&nbsp;Program Date(2nd Option) : {SessionDate2} <br /> 
								&nbsp;&nbsp;&nbsp;&nbsp;Meal Start Time : {MealStartTime} &nbsp;&nbsp;&nbsp;&nbsp;Session Start Time : {SessionStartTime} &nbsp;&nbsp;&nbsp;&nbsp; Session End Time : {SessionEndTime}
						</td>	
					</tr>
				</table>
				
											
				<table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
							<a style='display:block; color: #ffffff; font-size: 10px;' href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate=NotAvailable' >
									Not Available
								</a>
 
							</td>
 
							<td>
									&nbsp; &nbsp; &nbsp; &nbsp; Not Available

						</td>

					</tr>

				</table>


				<hr>


			Program Location:</strong> <span><strong> {LocationName} </strong> &nbsp; </span > &nbsp;<br />
			Your Honorarium:</strong> <span><strong> {Honorarium} </strong> &nbsp; </span> &nbsp;<br />
			AdditionalPresenter: if applicable </strong><span><strong> {Speaker2Name} </strong> &nbsp; </span><span><strong> {ModeratorName} </strong> &nbsp; </span> &nbsp;<br />
			Materials to be presented: </strong> <span><strong> {SessionCredits} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact: if applicable </strong> <span><strong> {SessionContact} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact Number:</strong> <span><strong>{ContactPhoneNumber} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact Address:</strong> <span><strong>{ContactEmailAddress} </strong> &nbsp; </span > &nbsp;<br />                                                                     

		   <p>Next Steps: <p> <br />

	        1. Please visit <a href='{SpeakerActivation}'>{SpeakerActivation}</a> and register - your username is: {SpeakerEmail} <br />
			2. The Speaker Resource Centre includes all the pertinent program materials for your reference.<br />
			Please log in <a href='{SpeakerSiteURL}'> {SpeakerSiteURL}</a> and complete the required forms (payee and COI forms) at your earliest convenience.<br /> 
			
			3. Access the Speaker Resource Centre to gain access to all the pertinent program materials for your reference. <br />
			
			Should you have any questions, please do not hesitate to contact your CHRC program coordinator: {SessionContact} – email address: {ContactEmailAddress}
			phone number: {ContactPhoneNumber}

		</p>


		</td>

	</tr>


</table> ";
            }

            if (!string.IsNullOrEmpty(pr.ProgramDate3) && (string.IsNullOrEmpty(pr.ProgramDate2)))
            {
                //Email3 option3
                html = @"<table width = '900' border = '0' cellspacing = '0' cellpadding = '10'>
				<tr>                
				<td align = 'center'><strong style = 'font-size: 30px; font-family: Arial, Helvetica, sans-serif;'>   Not Registered </strong></td>                         
				 </tr>                       
		  
				 <tr>                         
				<td align = 'left'> 					
					
				<br /> 
		
				<p>Dear Dr. {LastName}, </p> 
				<p>You are invited to present the accredited CPD program entitled {ProgramName}. 
				Please review the details and confirm your availability by clicking the Confirm or Not Available button </p> <br />
			
			<table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
							<a style='display: block;color: #ffffff;font-size: 10px;text-decoration: none;' 
							href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate={querydate1}'>
								Confirm
							</a>
														
						</td>
													
						<td>
														
								&nbsp;&nbsp;&nbsp;&nbsp;Program Date : {SessionDate1} <br /> 
								&nbsp;&nbsp;&nbsp;&nbsp;Meal Start Time : {MealStartTime} &nbsp;&nbsp;&nbsp;&nbsp;Session Start Time : {SessionStartTime} &nbsp;&nbsp;&nbsp;&nbsp; Session End Time : {SessionEndTime}
						</td>
					</tr>
				</table>
				
				<table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
						<a style='display: block;color: #ffffff;font-size: 10px;text-decoration: none;'
							href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate={querydate3}'>
								Confirm
							</a>
														
						</td>
													
						<td>
														
								&nbsp;&nbsp;&nbsp;&nbsp;Program Date(3rd Option) : {SessionDate3} <br /> 
								&nbsp;&nbsp;&nbsp;&nbsp;Meal Start Time : {MealStartTime} &nbsp;&nbsp;&nbsp;&nbsp;Session Start Time : {SessionStartTime} &nbsp;&nbsp;&nbsp;&nbsp; Session End Time : {SessionEndTime}
						</td>
														
					</tr>
				</table>
											
				<table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
							<a style='display:block; color: #ffffff; font-size: 10px;' href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate=NotAvailable' >
									Not Available
								</a>
 
							</td>
 
							<td>
									&nbsp; &nbsp; &nbsp; &nbsp; Not Available

						</td>

					</tr>

				</table>


				<hr>


			Program Location:</strong> <span><strong> {LocationName} </strong> &nbsp; </span > &nbsp;<br />
			Your Honorarium:</strong> <span><strong> {Honorarium} </strong> &nbsp; </span> &nbsp;<br />
			AdditionalPresenter: if applicable </strong><span><strong> {Speaker2Name} </strong> &nbsp; </span><span><strong> {ModeratorName} </strong> &nbsp; </span> &nbsp;<br />
			Materials to be presented: </strong> <span><strong> {SessionCredits} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact: if applicable </strong> <span><strong> {SessionContact} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact Number:</strong> <span><strong>{ContactPhoneNumber} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact Address:</strong> <span><strong>{ContactEmailAddress} </strong> &nbsp; </span > &nbsp;<br />                                                                     

		   <p>Next Steps: <p> <br />


			1. Please visit <a href='{SpeakerActivation}'> {SpeakerActivation} </a> and register - your username is: {SpeakerEmail} <br />
			2. The Speaker Resource Centre includes all the pertinent program materials for your reference.<br />
			  Please log in <a href='{SpeakerSiteURL}'> {SpeakerSiteURL}</a> and complete the required forms (payee and COI forms) at your earliest convenience.<br /> 
			
			3. Access the Speaker Resource Centre to gain access to all the pertinent program materials for your reference. <br />
			
			Should you have any questions, please do not hesitate to contact your CHRC program coordinator: {SessionContact} – email address: {ContactEmailAddress}
			phone number: {ContactPhoneNumber}

		</p>


		</td>

	</tr>


</table> ";
            }

            if (!string.IsNullOrEmpty(pr.ProgramDate3) && (!string.IsNullOrEmpty(pr.ProgramDate2)))
            {
                //EmailText3
                html = @"<table width = '900' border = '0' cellspacing = '0' cellpadding = '10'>
				<tr>                
				<td align = 'center'><strong style = 'font-size: 30px; font-family: Arial, Helvetica, sans-serif;'>   Not Registered </strong></td>                         
				 </tr>                       
		  
				 <tr>                         
				<td align = 'left'> 					
					
				<br /> 
		
				<p>Dear Dr. {LastName}, </p> 
				<p>You are invited to present the accredited CPD program entitled {ProgramName}. 
				Please review the details and confirm your availability by clicking the Confirm or Not Available button </p> <br />
			
					<table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
							<a style='display: block;color: #ffffff;font-size: 10px;text-decoration: none;' 
							href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate={querydate1}'>
								Confirm
							</a>
														
						</td>
													
						<td>
														
								&nbsp;&nbsp;&nbsp;&nbsp;Program Date : {SessionDate1} <br /> 
								&nbsp;&nbsp;&nbsp;&nbsp;Meal Start Time : {MealStartTime} &nbsp;&nbsp;&nbsp;&nbsp;Session Start Time : {SessionStartTime} &nbsp;&nbsp;&nbsp;&nbsp; Session End Time : {SessionEndTime}
						</td>
					</tr>
				</table>
				<table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
							<a style='display: block;color: #ffffff;font-size: 10px;text-decoration: none;'
							href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate={querydate2}'>
								Confirm
							</a>
														
						</td>
													
						<td>														
								&nbsp;&nbsp;&nbsp;&nbsp;Program Date(2nd Option) : {SessionDate2} <br /> 
								&nbsp;&nbsp;&nbsp;&nbsp;Meal Start Time : {MealStartTime} &nbsp;&nbsp;&nbsp;&nbsp;Session Start Time : {SessionStartTime} &nbsp;&nbsp;&nbsp;&nbsp; Session End Time : {SessionEndTime}
						</td>	
					</tr>
				</table>
				<table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
						<a style='display: block;color: #ffffff;font-size: 10px;text-decoration: none;'
							href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate={querydate3}'>
								Confirm
							</a>
														
						</td>
													
						<td>
														
								&nbsp;&nbsp;&nbsp;&nbsp;Program Date(3rd Option) : {SessionDate3} <br /> 
								&nbsp;&nbsp;&nbsp;&nbsp;Meal Start Time : {MealStartTime} &nbsp;&nbsp;&nbsp;&nbsp;Session Start Time : {SessionStartTime} &nbsp;&nbsp;&nbsp;&nbsp; Session End Time : {SessionEndTime}
						</td>
														
					</tr>
				</table>
											
				<table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
							<a style='display:block; color: #ffffff; font-size: 10px;' href='{SpeakerConfirmUrl}{ProgramRequestID}&ProgramDate=NotAvailable' >
									Not Available
								</a>
 
							</td>
 
							<td>
									&nbsp; &nbsp; &nbsp; &nbsp; Not Available

						</td>

					</tr>

				</table>


				<hr>


			Program Location:</strong> <span><strong> {LocationName} </strong> &nbsp; </span > &nbsp;<br />
			Your Honorarium:</strong> <span><strong> {Honorarium} </strong> &nbsp; </span> &nbsp;<br />
			AdditionalPresenter: if applicable </strong><span><strong> {Speaker2Name} </strong> &nbsp; </span><span><strong> {ModeratorName} </strong> &nbsp; </span> &nbsp;<br />
			Materials to be presented: </strong> <span><strong> {SessionCredits} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact: if applicable </strong> <span><strong> {SessionContact} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact Number:</strong> <span><strong>{ContactPhoneNumber} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact Address:</strong> <span><strong>{ContactEmailAddress} </strong> &nbsp; </span > &nbsp;<br />                                                                     

		   <p>Next Steps: <p> <br />


			1. Please visit <a href='{SpeakerActivation}'>{SpeakerActivation}</a> and register - your username is: {SpeakerEmail} <br />
			2. The Speaker Resource Centre includes all the pertinent program materials for your reference.<br/>
			 Please log in <a href='{SpeakerSiteURL}'>{SpeakerSiteURL}</a> and complete the required forms (payee and COI forms) at your earliest convenience.<br /> 
			3. Access the Speaker Resource Centre to gain access to all the pertinent program materials for your reference. <br />
			
			Should you have any questions, please do not hesitate to contact your CHRC program coordinator: {SessionContact} – email address: {ContactEmailAddress}
			phone number: {ContactPhoneNumber}

		</p>


		</td>

	</tr>


</table> ";


            }

            html = html.Replace("{ProgramRequestID}", pr.ProgramRequestID.ToString());
            html = html.Replace("{LastName}", um.LastName);
            html = html.Replace("{ProgramName}", ProgramName);

            html = html.Replace("{querydate1}", pr.ProgramDate1);
            html = html.Replace("{querydate2}", pr.ProgramDate2);
            html = html.Replace("{querydate3}", pr.ProgramDate3);

            html = html.Replace("{SessionDate1}", ConvertTimetoProperFormat(pr.ProgramDate1));
            if (!string.IsNullOrEmpty(pr.ProgramDate2))
            {
                html = html.Replace("{SessionDate2}", ConvertTimetoProperFormat(pr.ProgramDate2));
            }
            if (!string.IsNullOrEmpty(pr.ProgramDate3))
            {
                html = html.Replace("{SessionDate3}", ConvertTimetoProperFormat(pr.ProgramDate3));
            }

            html = html.Replace("{MealStartTime}", pr.MealStartTime);
            html = html.Replace("{SessionStartTime}", pr.ProgramStartTime);
            html = html.Replace("{SessionEndTime}", pr.ProgramEndTime);

            html = html.Replace("{LocationName}", pr.LocationName);
            html = html.Replace("{Honorarium}", um.SpeakerHonariumRange);
            html = html.Replace("{Speaker2Name}", Speaker2Name);
            html = html.Replace("{ModeratorName}", ModeratorName);
            html = html.Replace("{Materials}", Materials);
            html = html.Replace("{SessionContact}", pr.ContactName);
            html = html.Replace("{ContactPhoneNumber}", pr.ContactPhone);
            html = html.Replace("{ContactEmailAddress}", pr.ContactEmail);

            html = html.Replace("{SpeakerEmail}", um.EmailAddress);
            html = html.Replace("{SpeakerConfirmUrl}", SpeakerConfirmUrl);
            html = html.Replace("{SessionCredits}", SessionCredits);

            string speakerActivation = System.Configuration.ConfigurationManager.AppSettings["SpeakerBaseURL"] + "activation/index";
            string speakerSite = System.Configuration.ConfigurationManager.AppSettings["SpeakerBaseURL"] + "account/login";
            html = html.Replace("{SpeakerActivation}", speakerActivation).Replace("{SpeakerSiteURL}", speakerSite);

            return html;

        }
        public static void FromModeratorToAdmin(EventRequestModel pr, UserModel um, string ProgramName, string ChosenDate)
        {
            try
            {


                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                mailMessage.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["emailGeneral"]);
                mailMessage.To.Add(new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["AdminEmail"]));
                mailMessage.Subject = "From Moderator to Admin";
                mailMessage.IsBodyHtml = true;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(FromModeratorToAdminBody(pr, um, ProgramName, ChosenDate), null, "text/html");
                mailMessage.AlternateViews.Add(htmlView);

                UserHelper.SendMail(mailMessage);
            }

            catch (Exception e)
            {
                string msg = e.Message;
            }
        }
        private static string FromModeratorToAdminBody(EventRequestModel pr, UserModel um, string ProgramName, string ChosenDate)
        {
            string html = string.Empty;

            if (ChosenDate.Equals("NotAvailable"))
            {

                html = @" <style>
					   
						.emailBodyWrapper
						{
							padding: 5px;
							font-family: Candara;
							 font-size:14px;
						}
					 li
							{
							  padding-top:28px;   
							}
					</style>
					<div class='emailBodyWrapper'>
						<p>
							Dear Admin,
						</p>
						<p>
						   Please note that the selected moderator, Dr. {FirstName} {LastName}, is not available for the session date you had requested. 
						</p>
						
					   
					   
					</div>";



            }
            else
            {


                html = @" <style>
					   
						.emailBodyWrapper
						{
							padding: 5px;
							font-family: Candara;
							 font-size:14px;
						}
					 li
							{
							  padding-top:28px;   
							}
					</style>
					<div class='emailBodyWrapper'>
						<p>
							  Dear Admin,
						</p>
						<p>
						   Please note that the selected moderator, Dr. {FirstName} {LastName}, is  available for {ChosenDate} date.
						</p>
												
					   
					</div>";

            }



            html = html.Replace("{ProgramRequestID}", pr.ProgramRequestID.ToString());

            html = html.Replace("{ChosenDate}", ChosenDate);
            html = html.Replace("{LastName}", um.LastName);
            html = html.Replace("{FirstName}", um.FirstName);
            html = html.Replace("{ProgramName}", ProgramName);
            html = html.Replace("{ContactFirstName}", pr.ContactFirstName);



            return html;

        }
        public static void FromSpeaker2ToAdmin(EventRequestModel pr, UserModel um, string ProgramName, string ChosenDate)
        {
            try
            {


                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                mailMessage.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["emailGeneral"]);
                mailMessage.To.Add(new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["AdminEmail"]));
                mailMessage.Subject = "From Speaker2 to Admin";
                mailMessage.IsBodyHtml = true;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(FromSpeaker2ToAdminBody(pr, um, ProgramName, ChosenDate), null, "text/html");
                mailMessage.AlternateViews.Add(htmlView);

                UserHelper.SendMail(mailMessage);
            }

            catch (Exception e)
            {
                string msg = e.Message;
            }
        }
        private static string FromSpeaker2ToAdminBody(EventRequestModel pr, UserModel um, string ProgramName, string ChosenDate)
        {
            string html = string.Empty;

            if (ChosenDate.Equals("NotAvailable"))
            {

                html = @" <style>
					   
						.emailBodyWrapper
						{
							padding: 5px;
							font-family: Candara;
							 font-size:14px;
						}
					 li
							{
							  padding-top:28px;   
							}
					</style>
					<div class='emailBodyWrapper'>
						<p>
							Dear Admin,
						</p>
						<p>
						   Please note that the selected speaker2, Dr. {FirstName} {LastName}, is not available for the session date you had requested. 
						</p>
						
					   
					   
					</div>";



            }
            else
            {


                html = @" <style>
					   
						.emailBodyWrapper
						{
							padding: 5px;
							font-family: Candara;
							 font-size:14px;
						}
					 li
							{
							  padding-top:28px;   
							}
					</style>
					<div class='emailBodyWrapper'>
						<p>
							  Dear Admin,
						</p>
						<p>
						   Please note that the selected speaker2, Dr. {FirstName} {LastName}, is  available for {ChosenDate} date.
						</p>
												
					   
					</div>";

            }



            html = html.Replace("{ProgramRequestID}", pr.ProgramRequestID.ToString());

            html = html.Replace("{ChosenDate}", ChosenDate);
            html = html.Replace("{LastName}", um.LastName);
            html = html.Replace("{FirstName}", um.FirstName);
            html = html.Replace("{ProgramName}", ProgramName);
            html = html.Replace("{ContactFirstName}", pr.ContactFirstName);



            return html;

        }


        public static void FromSpeakerToModerator(EventRequestModel pr, UserModel um, string ProgramName, string ChosenDate, string SessionCredits)
        {
            try
            {
                //string date1 = ConvertTimetoProperFormat(pr.ProgramDate1);

                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                mailMessage.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["emailGeneral"]);
                //mailMessage.To.Add(new System.Net.Mail.MailAddress(um.EmailAddress));
                //Production Import for Dija
                 mailMessage.To.Add(new System.Net.Mail.MailAddress(um.EmailAddress));
               // mailMessage.To.Add(new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["AdminEmail"]));

               mailMessage.Bcc.Add(new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["AdminEmail"]));
                mailMessage.Subject = "From Speaker to Moderator";
                mailMessage.IsBodyHtml = true;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(FromSpeakerToModeratorBody(pr, um, ProgramName, ChosenDate, SessionCredits), null, "text/html");
                mailMessage.AlternateViews.Add(htmlView);
               
                UserHelper.SendMail(mailMessage);
            }

            catch (Exception e)
            {
                string msg = e.Message;
            }
        }

        public static void FromSpeakerToSpeaker2(EventRequestModel pr, UserModel um, string ProgramName, string ChosenDate, string SessionCredits)
        {
            try
            {
                //string date1 = ConvertTimetoProperFormat(pr.ProgramDate1);

                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                mailMessage.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["emailGeneral"]);
                //Production Import for Dija
                mailMessage.To.Add(new System.Net.Mail.MailAddress(um.EmailAddress));
               // mailMessage.To.Add(new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["AdminEmail"]));

                //mailMessage.To.Add(new System.Net.Mail.MailAddress(um.EmailAddress));
                mailMessage.Bcc.Add(new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["AdminEmail"]));
                mailMessage.Subject = "From Speaker to Speaker 2";
                mailMessage.IsBodyHtml = true;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(FromSpeakerToSpeaker2Body(pr, um, ProgramName, ChosenDate, SessionCredits), null, "text/html");
                mailMessage.AlternateViews.Add(htmlView);

                UserHelper.SendMail(mailMessage);
            }

            catch (Exception e)
            {
                string msg = e.Message;
            }
        }

        private static string FromSpeakerToModeratorBody(EventRequestModel pr, UserModel um, string ProgramName, string ChosenDate, string SessionCredits)
        {
            string Materials = "";
            string ModeratorConfirmUrl = System.Configuration.ConfigurationManager.AppSettings["ModeratorConfirmUrl"];

           
            string html = string.Empty;
            html = @"<table width = '900' border = '0' cellspacing = '0' cellpadding = '10'>
				<tr>                
				<td align = 'center'><strong style = 'font-size: 30px; font-family: Arial, Helvetica, sans-serif;'> Speaker to Moderator </strong></td>                         
				 </tr>                       
		  
				 <tr>                         
				<td align = 'left'> 					
					
				<br /> 
		
				<p>Dear {LastName}, </p> 
				<p>You are invited to present the accredited BMS/Pfizer CPD program entitled {ProgramName}. 
				Please review the details and confirm your availability by clicking the Confirm or Not Available button </p> <br />
			
					<table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
							<a style='display: block;color: #ffffff;font-size: 10px;text-decoration: none;' 
							href='{ModeratorConfirmUrl}{ProgramRequestID}&ProgramDate={ProgramDate}'>
								Confirm
							</a>
														
						</td>
													
						<td>
														
								&nbsp;&nbsp;&nbsp;&nbsp;Program Date : {ProgramDate} <br /> 
								&nbsp;&nbsp;&nbsp;&nbsp;Meal Start Time : {MealStartTime} &nbsp;&nbsp;&nbsp;&nbsp;Session Start Time : {SessionStartTime} &nbsp;&nbsp;&nbsp;&nbsp; Session End Time : {SessionEndTime}
						</td>
					</tr>
				</table>
			
											
				<table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
							<a style='display:block; color: #ffffff; font-size: 10px;' href='{ModeratorConfirmUrl}{ProgramRequestID}&ProgramDate=NotAvailable' >
									Not Available
								</a>
 
							</td>
							<td>
									&nbsp; &nbsp; &nbsp; &nbsp; Not Available

						</td>

					</tr>

				</table>


				<hr>


			Program Location:</strong> <span><strong> {LocationName} </strong> &nbsp; </span > &nbsp;<br />       
			Materials to be presented: </strong> <span><strong> {SessionCredits} </strong> &nbsp; </span> &nbsp;<br />
	        Your Honorarium:</strong> <span><strong> {Honorarium} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact: if applicable </strong> <span><strong> {SessionContact} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact Number:</strong> <span><strong>{ContactPhoneNumber} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact Address:</strong> <span><strong>{ContactEmailAddress} </strong> &nbsp; </span > &nbsp;<br />                                                                     

				Next Steps: <br/>
            1. Please visit https://speaker.clinicalconundrums.ca/activation/index and register - your username is: {login} &nbsp;<br/>
            2. The Speaker Resource Centre includes all the pertinent program materials for your reference.<br/>
                Please log in https://speaker.clinicalconundrums.ca/account/login and complete the required forms (payee and COI forms) at your earliest convenience.<br/>
            3. Access the Speaker Resource Centre to gain access to all the pertinent program materials for your reference. <br/>

			
			Should you have any questions, please do not hesitate to contact your CHRC program coordinator: {SessionContact} – email address: {ContactEmailAddress}
			phone number: {ContactPhoneNumber}

		</p>


		</td>

	</tr>


		</table> ";

            html = html.Replace("{ProgramRequestID}", pr.ProgramRequestID.ToString());
            html = html.Replace("{ProgramDate}", ChosenDate);
            html = html.Replace("{LastName}", um.LastName);
            html = html.Replace("{ProgramName}", ProgramName);
            html = html.Replace("{MealStartTime}", pr.MealStartTime);
            html = html.Replace("{SessionStartTime}", pr.ProgramStartTime);
            html = html.Replace("{SessionEndTime}", pr.ProgramEndTime);

            html = html.Replace("{LocationName}", pr.LocationName);
            html = html.Replace("{AdditionalPresenter}", "AdditionalPresenter");
            html = html.Replace("{Materials}", Materials);
            html = html.Replace("{SessionContact}", pr.ContactName);
            html = html.Replace("{ContactPhoneNumber}", pr.ContactPhone);
            html = html.Replace("{ContactEmailAddress}", pr.ContactEmail);
            html = html.Replace("{ModeratorConfirmUrl}", ModeratorConfirmUrl);
            html = html.Replace("{Honorarium}", um.ModeratorHonariumRange);
            html = html.Replace("{SessionCredits}", SessionCredits);
            html = html.Replace("{login}", um.EmailAddress);


            return html;

        }

        private static string FromSpeakerToSpeaker2Body(EventRequestModel pr, UserModel um, string ProgramName, string ChosenDate, string SessionCredits)
        {
            string Materials = "";
            string Speaker2ConfirmUrl = System.Configuration.ConfigurationManager.AppSettings["Speaker2ConfirmUrl"];


            string html = string.Empty;
            html = @"<table width = '900' border = '0' cellspacing = '0' cellpadding = '10'>
				<tr>                
				<td align = 'center'><strong style = 'font-size: 30px; font-family: Arial, Helvetica, sans-serif;'> Speaker to Speaker2 </strong></td>                         
				 </tr>                       
		  
				 <tr>                         
				<td align = 'left'> 					
					
				<br /> 
		
				<p>Dear {LastName}, </p> 
				<p>You are invited to present the accredited BMS/Pfizer CPD program entitled {ProgramName}. 
				Please review the details and confirm your availability by clicking the Confirm or Not Available button </p> <br />
			
					<table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
							<a style='display: block;color: #ffffff;font-size: 10px;text-decoration: none;' 
							href='{Speaker2ConfirmUrl}{ProgramRequestID}&ProgramDate={ProgramDate}'>
								Confirm
							</a>
														
						</td>
													
						<td>
														
								&nbsp;&nbsp;&nbsp;&nbsp;Program Date : {ProgramDate} <br /> 
								&nbsp;&nbsp;&nbsp;&nbsp;Meal Start Time : {MealStartTime} &nbsp;&nbsp;&nbsp;&nbsp;Session Start Time : {SessionStartTime} &nbsp;&nbsp;&nbsp;&nbsp; Session End Time : {SessionEndTime}
						</td>
					</tr>
				</table>
			
											
				<table>
					<tr>
						<td style='background-color: #4ecdc4; padding:0px 2px; text-align: center;'>
							<a style='display:block; color: #ffffff; font-size: 10px;' href='{Speaker2ConfirmUrl}{ProgramRequestID}&ProgramDate=NotAvailable' >
									Not Available
								</a>
 
							</td>
							<td>
									&nbsp; &nbsp; &nbsp; &nbsp; Not Available

						</td>

					</tr>

				</table>


				<hr>


			Program Location:</strong> <span><strong> {LocationName} </strong> &nbsp; </span > &nbsp;<br />       
			Materials to be presented: </strong> <span><strong> {SessionCredits} </strong> &nbsp; </span> &nbsp;<br />
	        Your Honorarium:</strong> <span><strong> {Honorarium} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact: if applicable </strong> <span><strong> {SessionContact} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact Number:</strong> <span><strong>{ContactPhoneNumber} </strong> &nbsp; </span> &nbsp;<br />
			Session Contact Address:</strong> <span><strong>{ContactEmailAddress} </strong> &nbsp; </span > &nbsp;<br />                                                                     

				
			
			Should you have any questions, please do not hesitate to contact your CHRC program coordinator: {SessionContact} – email address: {ContactEmailAddress}
			phone number: {ContactPhoneNumber}

		</p>


		</td>

	</tr>


		</table> ";

            html = html.Replace("{ProgramRequestID}", pr.ProgramRequestID.ToString());
            html = html.Replace("{ProgramDate}", ChosenDate);
            html = html.Replace("{LastName}", um.LastName);
            html = html.Replace("{ProgramName}", ProgramName);
            html = html.Replace("{MealStartTime}", pr.MealStartTime);
            html = html.Replace("{SessionStartTime}", pr.ProgramStartTime);
            html = html.Replace("{SessionEndTime}", pr.ProgramEndTime);

            html = html.Replace("{LocationName}", pr.LocationName);
            html = html.Replace("{AdditionalPresenter}", "AdditionalPresenter");
            html = html.Replace("{Materials}", Materials);
            html = html.Replace("{SessionContact}", pr.ContactName);
            html = html.Replace("{ContactPhoneNumber}", pr.ContactPhone);
            html = html.Replace("{ContactEmailAddress}", pr.ContactEmail);
            html = html.Replace("{Speaker2ConfirmUrl}", Speaker2ConfirmUrl);
            html = html.Replace("{Honorarium}", um.Speaker2HonariumRange);
            html = html.Replace("{SessionCredits}", SessionCredits);


            return html;

        }
        public static void EmailSalesRepWhenAdminMakeChanges(EventRequestModel pr, UserModel SalesRep, string ModeratorName, string SessionCredit, string ProgramName)
        {

            try
            {

                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                mailMessage.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["emailGeneral"]);
                mailMessage.To.Add(new System.Net.Mail.MailAddress(SalesRep.EmailAddress));
                mailMessage.Subject = ProgramName + "  Certification Request - ProgramDate " + pr.ProgramDate1;
                mailMessage.IsBodyHtml = true;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(EmailSalesRepWhenAdminMakeChangesBody(pr, SalesRep.LastName, ModeratorName, SessionCredit), null, "text/html");
                mailMessage.AlternateViews.Add(htmlView);
                if (!string.IsNullOrEmpty(pr.SessionAgendaFileName))
                {
                    string FileName = GetFileNameForEmailAttachment(pr.SessionAgendaFileName, pr.ProgramRequestID);
                    mailMessage.Attachments.Add(new Attachment(HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["UserFileUploadPath"] + FileName)));
                }

                UserHelper.SendMail(mailMessage);
            }

            catch (Exception e)
            {
                string msg = e.Message;
            }
        }

        private static string EmailSalesRepWhenAdminMakeChangesBody(EventRequestModel pr, string speakername, string ModeratorName, string SessionCredit)
        {
            string html = string.Empty;

            string Materials = "";

            //if (!string.IsNullOrEmpty(pr.SessionAgendaFileName))
            //{
            //    Materials = Path.GetFileName(pr.SessionAgendaFileName);
            //}



            html = App_GlobalResources.Resource.SaleRepEmailText;

            html = html.Replace("{RecordID}", pr.ProgramRequestID.ToString());
            html = html.Replace("{DateSubmitted}", DateTime.Now.ToString());
            html = html.Replace("{SessionContact}", pr.ContactName);
            html = html.Replace("{ContactPhoneNumber}", pr.ContactPhone);
            html = html.Replace("{ContactEmailAddress}", pr.ContactEmail);
            html = html.Replace("{SessionDate1}", ConvertTimetoProperFormat(pr.ProgramDate1));
            html = html.Replace("{SessionDate2}", ConvertTimetoProperFormat(pr.ProgramDate2));
            html = html.Replace("{SessionDate3}", ConvertTimetoProperFormat(pr.ProgramDate3));
            html = html.Replace("{MealStartTime}", pr.MealStartTime);
            html = html.Replace("{SessionStartTime}", pr.ProgramStartTime);
            html = html.Replace("{SessionEndTime}", pr.ProgramEndTime);
            html = html.Replace("{SessionCredits}", SessionCredit);
            html = html.Replace("{MultiSessionEvent}", pr.MultiSession.ToString());
            html = html.Replace("{CopyofAgenda}", Materials);
            html = html.Replace("{CostsbyParticipants}", pr.CostPerparticipants.ToString());
            html = html.Replace("{SessionSpeaker}", speakername);
            html = html.Replace("{SessionSpeakerStatus}", "");
            html = html.Replace("{SessionModerator}", ModeratorName);

            html = html.Replace("{SessionModeratorStatus}", "");
            html = html.Replace("{VenueContacted}", pr.VenueContacted);
            html = html.Replace("{LocationType}", pr.LocationType);


            html = html.Replace("{LocationName}", pr.LocationName);

            html = html.Replace("{Address}", pr.LocationAddress);

            html = html.Replace("{City}", pr.LocationCity);

            html = html.Replace("{Province}", pr.LocationProvince);

            html = html.Replace("{PhoneNumber}", pr.LocationPhoneNumber);

            html = html.Replace("{Website}", pr.LocationWebsite);

            html = html.Replace("{MealType}", pr.MealType);

            html = html.Replace("{CostPerPerson}", pr.CostPerPerson.ToString());

            html = html.Replace("{AudioVisual}", pr.AVEquipment);

            html = html.Replace("{AdditionalInfo}", pr.Comments);





            return html;

        }
        public static void AdminToSalesRep(string LocationName, UserModel um, string ProgramName, string ChosenDate)
        {
            try
            {


                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                mailMessage.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["AdminEmail"]);
                mailMessage.To.Add(new System.Net.Mail.MailAddress(um.EmailAddress));
                mailMessage.Subject = "Changes Required" + ProgramName + " – Session Request:  " + ChosenDate;
                mailMessage.IsBodyHtml = true;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(AdminToSalesRepBody(LocationName, um, ProgramName, ChosenDate), null, "text/html");
                mailMessage.AlternateViews.Add(htmlView);

                UserHelper.SendMail(mailMessage);
            }

            catch (Exception e)
            {
                string msg = e.Message;
            }
        }
        private static string AdminToSalesRepBody(String LocationName, UserModel um, string ProgramName, string ChosenDate)
        {
            string html = string.Empty;

            html = @" <style>
					   
						.emailBodyWrapper
						{
							padding: 5px;
							font-family: Candara;
							 font-size:14px;
						}
					 li
							{
							  padding-top:28px;   
							}
					</style>
					<div class='emailBodyWrapper'>
						<p>
							Dear  {LastName},
						</p>
						<p>
							Please note that the selected venue, {VenueName}, is not available for the session date you had requested. 
						</p>
						
						<p>
						   Next Steps:
						   
						<p>
							1.	Please log in to your portal: https://www.clinicalconundrums.ca  <br />
							2.	Visit your dashboard for the Program Name <br />
							3.	Click on the <strong>pencil</strong> icon under the <strong>Full Session Details</strong> column  <br />
							4.	Provide alternative venue details for this session <br /> 
						</p>
						
						<p>
							Please do not hesitate to contact us should you have any questions or require any assistance. <br />
							E: info@clinicalconundrums.ca
						</p>
					   
					</div>";






            html = html.Replace("{ChosenDate}", ChosenDate);
            html = html.Replace("{LastName}", um.LastName);
            html = html.Replace("{VenueName}", LocationName);


            return html;

        }

        public static ProgramRequestStatusCount GetProgramRequestStatusCounts(int ProgramID)
        {
            ProgramRequestStatusCount prsc = new ProgramRequestStatusCount();


            ProgramRepository pr = new ProgramRepository();
            //prsc = pr.GetProgramRequestStatusCounts(ProgramID);

            return prsc;
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
        public static void AdminChangeRequestStatusID2(StatusChangeEmailViewModel sc)
        {
            try
            {


                string html = string.Empty;
                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                mailMessage.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["emailGeneral"]);
                mailMessage.To.Add(new System.Net.Mail.MailAddress(sc.Email));
                mailMessage.Subject = sc.ProgramName + "  – Session Request: " + sc.ProgramDate + " – Your Event Information has been submitted for Regional Ethics Review";
                mailMessage.IsBodyHtml = true;
                html = @"<table width = '900' border = '0' cellspacing = '0' cellpadding = '10'>
				<tr>                
				<td>Dear {FirstName}, </td>
									   
				 </tr>  
            
				 <tr>
                <td>Your <b>{ProgramName}</b> event which will be hosted on <b>{Date}</b> has been submitted to the CFPC chapter for regional ethics review </td>
                 <tr>

                <td> 
                    <tr> You may now initiate recruitment utilizing the National Invitation Template.   </tr>
                </td>



				<tr>

                <b>Next Steps: </b> <br/> <br/>

				<td align='left'>
						1.	Please log in to your portal: https://www.clinicalconundrums.ca<br/>
                        2.	Access the Program Materials tab and download the modifiable National Invitation Template pdf<br/>
                        3.	Follow the instructions to complete the pertinent fields and print your invitations<br/>
                        
				</td>

				</tr>  
				       
				

			</table> ";
                html = html.Replace("{FirstName}", sc.FirstName);
                html = html.Replace("{ProgramName}", sc.ProgramName);
                html = html.Replace("{Date}", sc.ProgramDate);






                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(html, null, "text/html");
                mailMessage.AlternateViews.Add(htmlView);

                UserHelper.SendMail(mailMessage);
            }

            catch (Exception e)
            {
                string msg = e.Message;
            }
        }

        public static void AdminChangeRequestStatusID3(StatusChangeEmailViewModel sc)
        {
            try
            {


                string html = string.Empty;
                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                mailMessage.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["emailGeneral"]);
                mailMessage.To.Add(new System.Net.Mail.MailAddress(sc.Email));
                mailMessage.Subject = sc.ProgramName + "  – Session Request: " + sc.ProgramDate + " – Your event has received regional ethics approval ";
                mailMessage.IsBodyHtml = true;
                html = @"<table width = '900' border = '0' cellspacing = '0' cellpadding = '10'>
				<tr>                
				<td>Dear {FirstName}, </td>
									   
				 </tr>  
            
				 <tr>
                <td>Your <b>{ProgramName}</b> event which will be hosted on <b>{Date}</b> has received regional ethics approval.  Please note your event ID: <b> {EventId} </b> – the event ID is to be included in the Certificate of Attendance.  </td>
                 <tr>

                <td> 
                    <tr> You may continue recruitment utilizing the Regional Invitation Template and download the remainder of the program materials   </tr>
                </td>



				<tr>

                <b>Next Steps: </b> <br/> <br/>

				<td align='left'>
						1.	Please log in to your portal: https://www.clinicalconundrums.ca<br/>
                        2.	Access the Program Materials tab and download the modifiable Regional Invitation Template pdf., Certificate of Attendance, Evaluation Forms and the Sign-In Sheet. <br/>
                        3.	Follow the instructions to complete the pertinent fields and print your materials<br/>
                        
				</td>

				</tr>  
				       
				

			</table> ";
                html = html.Replace("{FirstName}", sc.FirstName);
                html = html.Replace("{ProgramName}", sc.ProgramName);
                html = html.Replace("{EventId}", sc.EventID);
                html = html.Replace("{Date}", sc.ProgramDate);






                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(html, null, "text/html");
                mailMessage.AlternateViews.Add(htmlView);

                UserHelper.SendMail(mailMessage);
            }

            catch (Exception e)
            {
                string msg = e.Message;
            }
        }

        public static void AdminChangeRequestStatusID5(StatusChangeEmailViewModel sc)
        {
            try
            {


                string html = string.Empty;
                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                mailMessage.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["emailGeneral"]);
                mailMessage.To.Add(new System.Net.Mail.MailAddress(sc.Email));
                mailMessage.Subject = sc.ProgramName + " – " + sc.ProgramDate + " – Please upload the post-program materials";
                mailMessage.IsBodyHtml = true;
                html = @"
                        <style>
					   
						.moveright
						{
							margin-left:25px;
						}
					 
					</style>

                <table width = '900' border = '0' cellspacing = '0' cellpadding = '10'>
				<tr>                
				<td>Dear {FirstName}, </td>
									   
				 </tr>  
            
				 <tr>
                <td>We hope that your <b>{ProgramName}</b> on <b>{ProgramDate}</b> was a success and of educational value to the physicians in attendance. </td>
                 <tr>

                <td> 
                    <tr>Please upload the required program materials within 5 business days of today’s date.  </tr>
                </td>

				<tr>

                <b>Next Steps: </b> <br/> <br/>

				<td align='left'>
						1.	Please log in to your portal: https://www.clinicalconundrums.ca<br/>
                        2.	Visit your dashboard for the {ProgramName} . <br/>
                        3.	Click the “upload” icon under the Post-Event BMS/Pfizer column header.<br/>
                        4.	Please submit the completed Evaluation Forms, Sign-In Sheet, Speaker Agreement Form and any other pertinent documents.<b>Alternatively you may submit the materials as following: </b> <br/><br />
                                
                            <p class='moveright'> <b> a. Email </b>  email the completed forms to info@clinicalconundrums.ca – please include your event date in the subject line </p> 
                            <p class='moveright'> <b> b. Fax: </b> fax the completed forms to 416-977-8020 or 1-800-238-5335 – attention BMS/Pfizer Programs </p>  
                            <p class='moveright'> <b> c. Mail:</b> CHRC c/o BMS/Pfizer 200-259 Yorkland Rd, North York, ON, M2J0B5 </p>
				</td>

				</tr>  
				       
				

			</table> ";
                html = html.Replace("{FirstName}", sc.FirstName);
                html = html.Replace("{ProgramName}", sc.ProgramName);
                html = html.Replace("{ProgramDate}", sc.ProgramDate);



                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(html, null, "text/html");
                mailMessage.AlternateViews.Add(htmlView);

                UserHelper.SendMail(mailMessage);
            }

            catch (Exception e)
            {
                string msg = e.Message;
            }
        }
        public static void EmailSpeaker_SpeakerApproved(UserModel UMSpeaker)
        {


            try
            {
                string html = string.Empty;

                string SpeakerActivation = System.Configuration.ConfigurationManager.AppSettings["SpeakerBaseURL"] + "activation/index";
                string SpeakerModeratorOptout = System.Configuration.ConfigurationManager.AppSettings["SpeakerBaseURL"] + "optout/index";

                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                mailMessage.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["emailGeneral"]);
                mailMessage.To.Add(new System.Net.Mail.MailAddress(UMSpeaker.EmailAddress));
                mailMessage.Subject = "Invitation to Participate as a Speaker: Activate Your Speaker Resource Account";
                mailMessage.IsBodyHtml = true;


                html = @"<table width = '100%' border = '0' cellspacing = '0' cellpadding = '10'>
                                                 <tr>                
                                                 <td align = 'left'><span style = 'font-size: 22px; font-family: Arial, Helvetica, sans-serif;'> Dear {SpeakerLastName} </span></td> 
                                                                                                                 
                                                  </tr>  
                                                 <tr>                
                                                 <td align='left'><span style ='font-color:red; font-size: 14px; font-family: Arial, Helvetica, sans-serif;'> 
                 On behalf of the Canadian Heart Research Centre, we are pleased to invite you to activate your personal speaker resource portal.<br/>
                 This portal has been developed to assist you in facilitating upcoming certified activities that have been developed by our physician organization with unrestricted educational grant support from BMS/Pfizer Alliance Canada.  
                <br/><br/>  </td> 
                                                                                                                 
                                                  </tr>   
                                                 <tr>
                                                 <td align='left'>
                                                              <b>Next Steps: If you ARE INTERESTED in being invited as a speaker and/or moderator (the program date(s) would be scheduled at a time and location that is convenient for you). </b><br/><br/>
                                      • Please activate your account by following this link <a href='{SpeakerActivation}'>{SpeakerActivation}</a> and entering your username: {Username} <br/>
                                      • Once your account is activated, you will be able to log-in and complete the COI and Payee Forms as well as access the portal’s functionalities and program materials. <br/> <br/> <br/>
                     <b>Next Steps: If you are <span style='color:red;'>NOT INTERESTED </span> in being invited as a speaker and/or moderator,</b><br/><br/>
                                     • Please follow this link to opt-out <a href='{SpeakerModeratorOptOut}'>{SpeakerModeratorOptOut}</a> and entering your username: {Username} <br/> <br/> <br/>
                     Please do not hesitate to contact us should you have any questions or require any assistance. 

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


                html = html.Replace("{SpeakerFirstName}", UMSpeaker.FirstName);
                html = html.Replace("{SpeakerLastName}", UMSpeaker.LastName);
                html = html.Replace("{Username}", UMSpeaker.EmailAddress);
                html = html.Replace("{SpeakerActivation}", SpeakerActivation);
                html = html.Replace("{SpeakerModeratorOptOut}", SpeakerModeratorOptout);
                html = html.Replace("{userid}", UMSpeaker.ID.ToString());




                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(html, null, "text/html");
                mailMessage.AlternateViews.Add(htmlView);

                UserHelper.SendMail(mailMessage);
            }

            catch (Exception e)
            {
                string msg = e.Message;
            }
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
						The CHRC Team <br/><br/>
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
        public static void EmailFromSaleRepToAdmin_Modify(String FromEmailAddress, ProgramRequestModifyVM pcvm)
        {
            try
            {


                string html = string.Empty;
                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                //mailMessage.From = new System.Net.Mail.MailAddress(FromEmailAddress);
                mailMessage.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["smtpUser"]);
                mailMessage.To.Add(new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["AdminEmail"]));
                mailMessage.Subject = "Session Modify Request";
                mailMessage.IsBodyHtml = true;
                html = @"<table width = '900' border = '0' cellspacing = '0' cellpadding = '10'>
				<tr>                
				<td align = 'left'><span style = 'font-size: 30px; font-family: Arial, Helvetica, sans-serif;'> Session Modify Request </span></td> 
									   
				 </tr>  
				  
				<tr>
				<td align='left'>
                        Email From: {emailFrom}<br/>
						Session ID: {Session ID}<br/>
                        Session Contact:  {Session Contact}<br/>
                        Session Date: {Session Date}<br/>
                        Session Location: {Session Location}<br/>
                        Session Speaker: {Session Speaker}<br/>
                        Session Speaker 2 (if applicable) : {Session Speaker2}<br/>
                        Session Moderator (if applicable) : {Session Moderator}<br/>
                        Reason: {Reason}<br/>
				</td>

				</tr>  
				       
				

			</table> ";
                html = html.Replace("{emailFrom}", FromEmailAddress);
                html = html.Replace("{Session ID}", pcvm.ProgramRequestID.ToString());
                html = html.Replace("{Session Contact}", pcvm.ContactName);
                html = html.Replace("{Session Date}", pcvm.ConfirmedSessionDate);
                html = html.Replace("{Session Location}", pcvm.LocationName);
                html = html.Replace("{Session Speaker}", pcvm.SpeakerName);
                html = html.Replace("{Session Speaker2}", pcvm.Speaker2Name);
                html = html.Replace("{Session Moderator}", pcvm.ModeratorName);
                html = html.Replace("{Reason}", pcvm.ModifyReason);

                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(html, null, "text/html");
                mailMessage.AlternateViews.Add(htmlView);

                UserHelper.SendMail(mailMessage);
            }

            catch (Exception e)
            {
                string msg = e.Message;
            }
        }
        public static void EmailFromSaleRepToAdmin_Cancellation(string fromEmailAddress, ProgramRequestCancellationVM pcvm)
        {
            try
            {


                string html = string.Empty;
                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                //mailMessage.From = new System.Net.Mail.MailAddress(FromEmailAddress);
                mailMessage.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["smtpUser"]);
                mailMessage.To.Add(new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["AdminEmail"]));
                mailMessage.Subject = "Session Cancellation Request";
                mailMessage.IsBodyHtml = true;
                html = @"<table width = '900' border = '0' cellspacing = '0' cellpadding = '10'>
				<tr>                
				<td align = 'left'><span style = 'font-size: 30px; font-family: Arial, Helvetica, sans-serif;'> Session Cancellation Request </span></td> 
									   
				 </tr>  
				  
				<tr>
				<td align='left'>
                        Email From: {emailFrom}<br/>
						Session ID: {Session ID}<br/>
                        Session Contact:  {Session Contact}<br/>
                        Session Date: {Session Date}<br/>
                        Session Location: {Session Location}<br/>
                        Session Speaker: {Session Speaker}<br/>
                        Session Speaker 2 (if applicable) : {Session Speaker2}<br/>
                        Session Moderator (if applicable) : {Session Moderator}<br/>
                        Reason: {Reason}<br/>
				</td>

				</tr>  
				       
				

			</table> ";
                html = html.Replace("{emailFrom}", fromEmailAddress);
                html = html.Replace("{Session ID}", pcvm.ProgramRequestID.ToString());
                html = html.Replace("{Session Contact}", pcvm.ContactName);
                html = html.Replace("{Session Date}", pcvm.ConfirmedSessionDate);
                html = html.Replace("{Session Location}", pcvm.LocationName);
                html = html.Replace("{Session Speaker}", pcvm.SpeakerName);
                html = html.Replace("{Session Speaker2}", pcvm.Speaker2Name);
                html = html.Replace("{Session Moderator}", pcvm.ModeratorName);
                html = html.Replace("{Reason}", pcvm.CancellationReason);

                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(html, null, "text/html");
                mailMessage.AlternateViews.Add(htmlView);

                UserHelper.SendMail(mailMessage);
            }

            catch (Exception e)
            {
                string msg = e.Message;
            }
        }
        public static void FromSpeakerToSalesRep(EventRequestModel pr, UserModel um, string ProgramName, string ChosenDate)
        {
            try
            {


                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                mailMessage.From = new System.Net.Mail.MailAddress(um.EmailAddress);
                mailMessage.To.Add(new System.Net.Mail.MailAddress(pr.ContactEmail));

                if (ChosenDate.Equals("NotAvailable"))
                {
                    mailMessage.Subject = "Speaker Changes Required " + ProgramName + "  – Session Request: " + ChosenDate;
                }
                else
                {
                    mailMessage.Subject = "Speaker Selected " + ProgramName + "  – Session Date: " + ChosenDate;

                }

                mailMessage.IsBodyHtml = true;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(FromSpeakerToSalesRepBody(pr, um, ProgramName, ChosenDate), null, "text/html");
                mailMessage.AlternateViews.Add(htmlView);

                UserHelper.SendMail(mailMessage);
            }

            catch (Exception e)
            {
                string msg = e.Message;
            }
        }
        private static string FromSpeakerToSalesRepBody(EventRequestModel pr, UserModel um, string ProgramName, string ChosenDate)
        {
            string html = string.Empty;

            if (ChosenDate.Equals("NotAvailable"))
            {

                html = @" <style>
					   
						.emailBodyWrapper
						{
							padding: 5px;
							font-family: Candara;
							 font-size:14px;
						}
					 li
							{
							  padding-top:28px;   
							}
					</style>
					<div class='emailBodyWrapper'>
						<p>
							Dear {ContactFirstName},
						</p>
						<p>
						   Please note that the selected speaker, Dr. {FirstName} {LastName}, is not available for the session date you had requested. 
						</p>
						
						<p>
						   Next Steps:
						   
						<p>
							1.	Please log in to your portal: https://www.clinicalconundrums.ca  <br />
							2.	Visit your dashboard for the Program Name <br />
							3.	Click on the <strong>pencil</strong> icon under the <strong>Full Session Details</strong> column  <br />
							4.	Select a different session date or a different speaker from the drop-down menu   <br /> 
						</p>
						
						<p>
							Please do not hesitate to contact us should you have any questions or require any assistance. <br />
							E: info@clinicalconundrums.ca
						</p>
					   
					</div>";




            }
            else
            {


                html = @" <style>
					   
						.emailBodyWrapper
						{
							padding: 5px;
							font-family: Candara;
							 font-size:14px;
						}
					 li
							{
							  padding-top:28px;   
							}
					</style>
					<div class='emailBodyWrapper'>
						<p>
							 Dear {ContactFirstName},
						</p>
						<p>
						   Please note that the selected speaker, Dr. {FirstName} {LastName}, is  available for {ChosenDate} date.
						</p>
												
					   
					</div>";

            }



            html = html.Replace("{ProgramRequestID}", pr.ProgramRequestID.ToString());

            html = html.Replace("{ChosenDate}", ChosenDate);
            html = html.Replace("{LastName}", um.LastName);
            html = html.Replace("{FirstName}", um.FirstName);
            html = html.Replace("{ProgramName}", ProgramName);
            html = html.Replace("{ContactFirstName}", pr.ContactFirstName);



            return html;


        }
        public static void FromSpeakerToAdmin(EventRequestModel pr, UserModel um, string ProgramName, string ChosenDate)
        {
            try
            {



                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                mailMessage.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["AdminEmail"]);
                // mailMessage.From = new System.Net.Mail.MailAddress(um.EmailAddress);
                mailMessage.To.Add(new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["AdminEmail"]));
                mailMessage.Subject = "From Speaker to Admin";
                mailMessage.IsBodyHtml = true;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(FromSpeakerToAdminBody(pr, um, ProgramName, ChosenDate), null, "text/html");
                mailMessage.AlternateViews.Add(htmlView);

                UserHelper.SendMail(mailMessage);
            }

            catch (Exception e)
            {
                string msg = e.Message;
            }
        }
        private static string FromSpeakerToAdminBody(EventRequestModel pr, UserModel um, string ProgramName, string ChosenDate)
        {
            string html = string.Empty;

            if (ChosenDate.Equals("NotAvailable"))
            {

                html = @" <style>
					   
						.emailBodyWrapper
						{
							padding: 5px;
							font-family: Candara;
							 font-size:14px;
						}
					 li
							{
							  padding-top:28px;   
							}
					</style>
					<div class='emailBodyWrapper'>
						<p>
							Dear Admin,
						</p>
						<p>
						   Please note that the selected speaker, Dr. {FirstName} {LastName}, is not available for the session date you had requested. Program Request ID: {ProgramRequestID}
						</p>
						
					   
					   
					</div>";



            }
            else
            {


                html = @" <style>
					   
						.emailBodyWrapper
						{
							padding: 5px;
							font-family: Candara;
							 font-size:14px;
						}
					 li
							{
							  padding-top:28px;   
							}
					</style>
					<div class='emailBodyWrapper'>
						<p>
							  Dear Admin,
						</p>
						<p>
						   Please note that the selected speaker, Dr. {FirstName} {LastName}, is  available for {ChosenDate} date. Program Request ID: {ProgramRequestID}
						</p>
												
					   
					</div>";

            }



            html = html.Replace("{ProgramRequestID}", pr.ProgramRequestID.ToString());

            html = html.Replace("{ChosenDate}", ChosenDate);
            html = html.Replace("{LastName}", um.LastName);
            html = html.Replace("{FirstName}", um.FirstName);
            html = html.Replace("{ProgramName}", ProgramName);
            html = html.Replace("{ContactFirstName}", pr.ContactFirstName);



            return html;
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
        public static void SendActivationEmail(string FirstName, string email, string password)
        {

            try
            {
                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();

                mailMessage.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["emailGeneral"]);
                mailMessage.To.Add(new System.Net.Mail.MailAddress(email));
                mailMessage.Subject = "Welcome to your BMS/Pfizer personal CPD Portal: Your Account has been Successfully Activated";

                mailMessage.IsBodyHtml = true;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(GetActivationEmailBody(FirstName, email, password), null, "text/html");

                mailMessage.AlternateViews.Add(htmlView);
                mailMessage.Attachments.Add(new Attachment(HttpContext.Current.Server.MapPath("~/PDF/Cookies.pdf")));
                UserHelper.SendMail(mailMessage);
            }

            catch (Exception e)
            {

                string msg = e.Message;


            }


        }
        public static string GetActivationEmailBody(string FirstName, string email, string password)
        {


            try
            {
                string html = string.Empty;



                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                mailMessage.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["emailGeneral"]);
                mailMessage.To.Add(new System.Net.Mail.MailAddress(email));
               
                    mailMessage.Subject = "Welcome to your personal BMS/Pfizer CPD Portal: Your Account has been Successfully Activated";
               
                mailMessage.IsBodyHtml = true;


                html = @"<h1>REGISTRATION COMPLETE</h1><table width = '100%' border = '0' cellspacing = '0' cellpadding = '10'>
                                                 <tr>                
                                                 <td align = 'left'><span style = 'font-size: 22px; font-family: Arial, Helvetica, sans-serif;'> Dear Dr. {SpeakerLastName} </span></td> 
                                                                                                                 
                                                  </tr>  
                                                 <tr>                
                                                 <td align='left'><span style ='font-size: 14px; font-family: Arial, Helvetica, sans-serif;'> 
                You have successfully activated your personal BMS/Pfizer CPD Portal. </td> 
                                                                                                                 
                                                  </tr>   
                                     
                                                 <tr>
                                                 <td align='left'>
Please take note of your <b>username</b> and <b>password</b> which you will be required to enter each time you access your personal online portal at <a href='{ProductionURL}'>{ProductionURL}</a>

                                                            

                </td>       
                                                 

                                                 </tr>  
                                                <tr>
                                                 <td align='left'>
                                                                        <b>Please keep this information confidential and do not share or forward your username and password:</b><br/>
                                                                        

                                                 </td>

                                                 </tr> 
                                                    <tr>
                                                 <td align='left'>
                                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>Your Username:</b> {Username}<br/>
                                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<b>Your Password:</b> {Password}
                                                                        

                                                 </td>

                                                 </tr> 
<tr>
                                                 <td align='left'>
                                                <b>Next Steps:</b><br/><br/>
                                                                      Please sign-in with your username and password at <a href='{ProductionURL}'>{ProductionURL}</a> to familiarize with the web portal and its’ functionalities.
                                                                        

                                                 </td>

                                                 </tr> 
                                                 <tr>
                                                 <td align='left'><p>Should you have any questions or require any assistance, please do not hesitate to contact us at  <a href='mailto:info@clinicalconundrums.ca'>info@clinicalconundrums.ca</a></p>
                                                       
                                                 </td>

                                                 </tr>          
                                                 <tr>
                                                 <td align='left'>
                                                                With best regards, CCMEP<br/>
                                                                <b>The CHRC Team</b> <br/>
                                                                                  
                            

                                                                E: <a mailto:'info@clinicalconundrums.ca'>info@clinicalconundrums.ca</a> <br/>
                                                              
                                                 </td>

                                                 </tr>   

                                     </table> ";


                html = html.Replace("{SpeakerFirstName}", "");
                html = html.Replace("{SpeakerLastName}", FirstName);
                html = html.Replace("{Username}", email);
                html = html.Replace("{Password}",password);
                html = html.Replace("{ProductionURL}", ConfigurationManager.AppSettings["BaseURL"]);



                return html;

                
            }

            catch (Exception e)
            {
                return "An error has occurred please email info@clinicalconundrums.ca for assistance ";
            }
        }
        public static void SendSpeakerConfirmEmail(int EventRequestID)
        {

            try
            {
                System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();

                mailMessage.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["emailGeneral"]);
                mailMessage.To.Add(new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["emailGeneral"]));
                mailMessage.Subject = "Speaker Confirm Event Request";

                mailMessage.IsBodyHtml = true;
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(GetActivationEmailBody("Speaker1 confirms Event Request: " + Convert.ToString(EventRequestID), null, "text/html"));

                mailMessage.AlternateViews.Add(htmlView);
                mailMessage.Attachments.Add(new Attachment(HttpContext.Current.Server.MapPath("~/PDF/Cookies.pdf")));
                UserHelper.SendMail(mailMessage);
            }

            catch (Exception e)
            {

                string msg = e.Message;


            }


        }
        public static string GetProvinceFullName(string ProvinceCode)
        {
            string ProvinceFullName = string.Empty;
            switch (ProvinceCode.ToUpper())
            {
                case "AB":
                    ProvinceFullName = "Alberta";
                    break;
                case "BC":
                    ProvinceFullName = "British Columbia";
                    break;
                case "NB":
                    ProvinceFullName = "New Brunswick";
                    break;
                case "NL":
                    ProvinceFullName = "Newfoundland";
                    break;
                case "NS":
                    ProvinceFullName = "Nova Scotia";
                    break;
                case "NT":
                    ProvinceFullName = "Northwest Territories";
                    break;
                case "NU":
                    ProvinceFullName = "Nunavut";
                    break;
                case "ON":
                    ProvinceFullName = "Ontario";
                    break;
                case "PEI":
                    ProvinceFullName = "Prince Edward Island";
                    break;
                case "QC":
                    ProvinceFullName = "Quebec";
                    break;
                case "SK":
                    ProvinceFullName = "Saskatchewan";
                    break;
                case "YT":
                    ProvinceFullName = "Yukon";
                    break;
            }
            return ProvinceFullName;
        }
      


    }
}

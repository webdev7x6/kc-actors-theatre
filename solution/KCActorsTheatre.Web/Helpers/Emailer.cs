using System;
using System.Net.Mail;
using System.Net;
using System.Web;

namespace KCActorsTheatre.Web.Helpers
{
    public class Emailer
    {
        //TODO: specify correct mail server values
        private const String MailServer = "localhost";
        private const String MailServerAddress = "noreply@kcactors.org";
        //private const String MailServerPassword = "PK0m9iEadjPdKDRfK0Dz";
        private const String MailServerDisplayTitle = "KCAT Website";

        public String From { get; set; }
        public String To { get; set; }
        public String Subject { get; set; }
        public String Message { get; set; }
        private String ErrorMessage = string.Empty;

        public Emailer()
            : this(string.Empty, string.Empty, string.Empty)
        {
        }

        public Emailer(string to, string subject, string message)
        {
            this.From = MailServerAddress;
            this.To = to;
            this.Subject = subject;
            this.Message = message;
        }

        public void Send()
        {
            if (ValidateEmail())
            {
                //Converts message to html format:
                var html = AlternateView.CreateAlternateViewFromString(this.Message, null, "text/html");

                var mailmessage = new MailMessage();
                mailmessage.From = new MailAddress(this.From, MailServerDisplayTitle);
                mailmessage.To.Add(this.To);
                mailmessage.Subject = this.Subject;
                mailmessage.AlternateViews.Add(html);

                var smtp = new SmtpClient(MailServer);
                smtp.Port = 26;
                smtp.EnableSsl = false;
                //smtp.Credentials = new NetworkCredential(this.From, MailServerPassword);
                smtp.Send(mailmessage);
            }
            else
            {
                throw new Exception(string.Format("Unable to send email: {0}", this.ErrorMessage));
            }
        }

        private Boolean ValidateEmail()
        {
            var result = true;

            if (string.IsNullOrEmpty(this.To))
            {
                result = false;
                this.ErrorMessage = "The [To] field was null or empty.";
            }

            if (!ValidEmailAddress(this.To))
            {
                result = false;
                this.ErrorMessage = "The [To] field is not a valid email address.";
            }

            if (string.IsNullOrEmpty(this.Subject))
            {
                result = false;
                this.ErrorMessage = "The [Subject] field was null or empty.";
            }

            if (string.IsNullOrEmpty(this.Message))
            {
                result = false;
                this.ErrorMessage = "The [Message] field was null or empty.";
            }

            return result;
        }

        private Boolean ValidEmailAddress(string email)
        {
            try
            {
                var test = new MailAddress(email);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
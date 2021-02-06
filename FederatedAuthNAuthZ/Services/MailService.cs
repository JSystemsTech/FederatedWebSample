using ServiceProvider.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace FederatedAuthNAuthZ.Services
{
    public class EmailSendConfigure
    {
        public IEnumerable<string> TOs { get; set; }
        public IEnumerable<string> CCs { get; set; }
        public IEnumerable<string> BCCs { get; set; }
        public string FromDisplayName { get; set; }
        public string Subject { get; set; }
        public MailPriority Priority { get; set; }
        public string ClientCredentialUserName { get; set; }
        public string ClientCredentialPassword { get; set; }
        internal EmailSendConfigure() { }
    }

    public class EmailContent
    {
        public bool IsHtml { get; set; }
        public string Content { get; set; }
        public string AttachFileName { get; set; }
        internal EmailContent() { }
    }


    public interface IMailServiceResponse
    {
        bool Sent { get; }
        string Error { get; }
        Exception Exception { get; }
    }
    internal  class MailServiceResponse: IMailServiceResponse
    {
        public bool Sent { get; private set; }
        public string Error { get; private set; }
        public Exception Exception { get; private set; }
        internal static IMailServiceResponse Success() => new MailServiceResponse() { Sent = true };
        internal static IMailServiceResponse Fail(Exception e) => new MailServiceResponse() { Sent = false, Exception= e, Error = $"Error occured sending email: {e.Message}" };
    }
    public interface IMailService
    {
        IMailServiceResponse SendMail(Action<EmailSendConfigure> configureMessage);
        IMailServiceResponse SendMail(Action<EmailSendConfigure, EmailContent> configureEmail);
    }
    public sealed class MailService: Service,IMailService
    {
        private SmtpClient SmtpClient { get; set; }
        private string From { get => ((NetworkCredential)SmtpClient.Credentials).UserName; }
        public MailService() : base() { }
        protected override void Init()
        {
            SmtpClient = new SmtpClient();
        }



        private void AddToMailAddressCollection(MailAddressCollection collection, IEnumerable<string> emails)
        {
            if (emails != null)
            {
                foreach (string email in emails)
                {
                    if (!string.IsNullOrEmpty(email))
                    {
                        collection.Add(email);
                    }
                }
            }
        }
        
        private MailMessage ConstructEmailMessage(EmailSendConfigure emailConfig, EmailContent content)
        {
            MailMessage msg = new System.Net.Mail.MailMessage();
            AddToMailAddressCollection(msg.To, emailConfig.TOs);
            AddToMailAddressCollection(msg.CC, emailConfig.CCs);
            AddToMailAddressCollection(msg.Bcc, emailConfig.BCCs);

            msg.From = new MailAddress(From,
                                       emailConfig.FromDisplayName,
                                       Encoding.UTF8);
            msg.IsBodyHtml = content.IsHtml;
            msg.Body = content.Content;
            msg.Priority = emailConfig.Priority;
            msg.Subject = emailConfig.Subject;
            msg.BodyEncoding = Encoding.UTF8;
            msg.SubjectEncoding = Encoding.UTF8;

            if (content.AttachFileName != null)
            {
                Attachment data = new Attachment(content.AttachFileName, MediaTypeNames.Application.Zip);
                msg.Attachments.Add(data);
            }

            return msg;
        }

        public IMailServiceResponse SendMail(Action<EmailSendConfigure> configureMessage) => SendMail((m, c) => configureMessage(m));
        public IMailServiceResponse SendMail(Action<EmailSendConfigure, EmailContent> configureEmail)
        {
            EmailSendConfigure messageConfig = new EmailSendConfigure();
            EmailContent contentConfig = new EmailContent();
            configureEmail(messageConfig, contentConfig);

            using (MailMessage message = ConstructEmailMessage(messageConfig, contentConfig))
            {
                try
                {
                    SmtpClient.Send(message);
                    return MailServiceResponse.Success();
                }
                catch (Exception e)
                {
                    return MailServiceResponse.Fail(e);
                }
            };
        }


        public void SendMail(EmailSendConfigure emailConfig, EmailContent content)
        {
            MailMessage msg = ConstructEmailMessage(emailConfig, content);
            Send(msg, emailConfig);
        }

        //Send the email using the SMTP server  
        private void Send(MailMessage message, EmailSendConfigure emailConfig)
        {
            //SmtpClient client = new SmtpClient();
            //client.UseDefaultCredentials = false;
            //client.Credentials = new System.Net.NetworkCredential(
            //                      emailConfig.ClientCredentialUserName,
            //                      emailConfig.ClientCredentialPassword);
            //client.Host = m_HostName;
            //client.Port = 25;  // this is critical
            //client.EnableSsl = true;  // this is critical
            using (message) {
                try
                {
                    SmtpClient.Send(message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error in Send email: {0}", e.Message);
                    throw;
                }
            };
        }
    }
}

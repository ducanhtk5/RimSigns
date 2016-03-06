using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using CMS.Service.Repository;
using CMS.Data;
using System.Threading;

namespace CMS.Service.Common
{
    public class SendEmail
    {
        static Thread SendMail_Thread;

        public static void StartThreadSendMail()
        {
            SendMail_Thread = new Thread(new ThreadStart(SendMailbyThread));
            SendMail_Thread.Start();
        }

        public class EmailQueue
        {
            public string Email { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
            public bool is_finished { get; set; }

            public EmailQueue()
            {
                Email = "";
                Subject = "";
                Body = "";
                is_finished = false;
            }
        }

        public static bool SendMail(string emailto, string subject, string body, bool InsertToQueue = true)
        {
            if (InsertToQueue)
            {
                EmailQueue emailQueue = new EmailQueue();
                emailQueue.Email = emailto;
                emailQueue.Subject = subject;
                emailQueue.Body = body;
                EmailQueueList.Add(emailQueue);
                return true;
            }
            else
            {
                string displayname = ConfigSettings.ReadSetting("DisplayName");
                if (displayname == null) { displayname = "Real Estate Media"; }

                string email_account = ConfigSettings.ReadSetting("Email");

                string email_admin = ConfigSettings.ReadSetting("EmailRecive");
                if (emailto == "")
                    emailto = email_admin;

                string password_account = ConfigSettings.ReadSetting("Password");

                string host = ConfigSettings.ReadSetting("Host");

                int port = int.Parse(ConfigSettings.ReadSetting("Port"));

                bool enablessl = Convert.ToBoolean(ConfigSettings.ReadSetting("EnableSSL"));

                SmtpClient SmtpServer = new SmtpClient();
                SmtpServer.Credentials = new System.Net.NetworkCredential(email_account, password_account);
                SmtpServer.Port = port;
                SmtpServer.Host = host;
                SmtpServer.EnableSsl = enablessl;
                MailMessage mail = new MailMessage();

                try
                {
                    mail.From = new MailAddress(email_account, displayname, System.Text.Encoding.UTF8);
                    mail.To.Add(emailto);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                    mail.ReplyTo = new MailAddress(email_account);
                    mail.Priority = MailPriority.High;
                    mail.IsBodyHtml = true;
                    SmtpServer.Send(mail);
                    return true;
                }
                catch (Exception) { return false; }
            }
        }

        public static List<EmailQueue> EmailQueueList = new List<EmailQueue>();
        public static void SendEmailOrderActivities(Order orderEntity, Enum.OrderActions action, List<string> mailList, string host)
        {
            string subject = "";
            string body = "";
            switch (action)
            {
                case Enum.OrderActions.ChangeStatus:
                    subject = orderEntity.Title + " change status";
                    body = "Change status to " + orderEntity.Status;
                    body += "<br/>Click on this link to view details: <a href='" + host + "/Order/Details/" + orderEntity.Id + "'>Click here</a>";
                    body += "<br/>Or copy this link to browser: http://" + host + "/Order/Details/" + orderEntity.Id;
                    break;
                case Enum.OrderActions.Comment:
                    subject = "Comment on " + orderEntity.Title;
                    body = "New comment on " + orderEntity.Title;
                    body += "<br/>Click on this link to view details: <a href='" + host + "/Order/Details/" + orderEntity.Id + "'>Click here</a>";
                    body += "<br/>Or copy this link to browser: http://" + host + "/Order/Details/" + orderEntity.Id;
                    break;
                case Enum.OrderActions.UpdatePercent:
                    subject = "Update percent on " + orderEntity.Title + " to " + orderEntity.OrderPercent;
                    body = "Update pecent to " + orderEntity.OrderPercent;
                    body += "<br/>Click on this link to view details: <a href='" + host + "/Order/Details/" + orderEntity.Id + "'>Click here</a>";
                    body += "<br/>Or copy this link to browser: http://" + host + "/Order/Details/" + orderEntity.Id;
                    break;
                case Enum.OrderActions.UploadFile:
                    subject = "Upload file on " + orderEntity.Title;
                    body = "New File uploaded on " + orderEntity.Title;
                    body += "<br/>Click on this link to view details: <a href='" + host + "/Order/Details/" + orderEntity.Id + "'>Click here</a>";
                    body += "<br/>Or copy this link to browser: http://" + host + "/Order/Details/" + orderEntity.Id;
                    break;
                default:
                    break;
            }

            foreach (var item in mailList)
            {
                EmailQueue emailQueue = new EmailQueue();
                emailQueue.Email = item;
                emailQueue.Subject = subject;
                emailQueue.Body = body;
                EmailQueueList.Add(emailQueue);
            }
        }


        static void SendMailbyThread()
        {
            while (SendMail_Thread.IsAlive)
            {
                while (EmailQueueList.Count > 0)
                {
                    try
                    {
                        var item = EmailQueueList.Where(m => !m.is_finished).FirstOrDefault();
                        if (item == null)
                            break;

                        item.is_finished = true;
                        SendMail(item.Email, item.Subject, item.Body, false);
                        EmailQueueList.Remove(item);
                        Thread.Sleep(2500);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                Thread.Sleep(2500);
            }

        }

        /* while (SendMail_Thread.IsAlive)
         {

             for (int i = 0; i < EmailQueueList.Count; i++)
             {
                 try
                 {
                     if (!EmailQueueList[i].is_finished)
                     {
                         SendMail(EmailQueueList[i].Email, EmailQueueList[i].Subject, EmailQueueList[i].Body, false);
                         EmailQueueList[i].is_finished = true;
                         Thread.Sleep(3000);
                     }
                 }
                 catch (Exception ex)
                 {

                 }
             }
             Thread.Sleep(10000);
         }
        
     }*/
    }
}

using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using UnityEngine;


public static class MailManger 
{

    public static void Initlize()
    {
        Debug.Log("mail initlize");

    }



    //Actual Mail(WORKING)
    public static async Task setupMailClient(string _mailID, string _imageLocation)
    {
        await Task.Run(() =>
        {
            try
            {

                Debug.Log("Seting UP MAIL SERVER");
                MailMessage mail = new MailMessage();

                SmtpClient smtpServer = new SmtpClient("mail.eventactivation.website");
                //SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
                smtpServer.Timeout = 100000;
                smtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpServer.UseDefaultCredentials = false;
                //smtpServer.Port = 587; gamil
                //Bluhost
                //smtpServer.Port = 465;
                smtpServer.Port = 587;

                //  mail.From = new MailAddress("usbekistantest@gmail.com");
                mail.From = new MailAddress("otm_mumbai2023_qatar@eventactivation.website", "Badge");
                //mail.From = new MailAddress("badge.5gvr.online@gmail.com", "Badge");
                mail.To.Add(new MailAddress(_mailID));
                Debug.Log("MAIL ID UPDATE");

                //CameraManger.OpenCVImageConverter();
                string body = string.Empty;
                string htmlloc = Application.streamingAssetsPath + "/index.html";
                Debug.Log("HTML Location !" + htmlloc);
                using (StreamReader reader = new StreamReader(htmlloc))
                {
                    body = reader.ReadToEnd();
                }
                //body = body.Replace("{Greating}", "Greating..");
                //if(CurrentPlayer == PlayerSelection.Goalie)
                //{
                //    body = body.Replace("{UserName}", _firstName + " " + _lastName);

                //}else if(CurrentPlayer == PlayerSelection.Striker)
                //{
                //    body = body.Replace("{UserName}", _firstName + " " + _lastName);

                //}


                mail.Subject = "OTM Mumbai 2023 Visit QATAR";
                mail.Body = body;
            
                mail.IsBodyHtml = true;

                var avHtml = AlternateView.CreateAlternateViewFromString(mail.Body, null,MediaTypeNames.Text.Html);
                Debug.Log("Message Readed!!!");
                var inline = new LinkedResource(_imageLocation);//passing image here
                inline.ContentId = "myImageID";
                avHtml.LinkedResources.Add(inline);
                mail.AlternateViews.Add(avHtml);
                var val = new Attachment(_imageLocation);
                mail.Attachments.Add(val);
                Debug.Log("Before ATTACHMENT");

                // smtpServer.Credentials = new NetworkCredential("usbekistantest@gmail.com", "creative@123")as ICredentialsByHost;
                smtpServer.Credentials = new NetworkCredential("riyadhseason2022_mada@eventactivation.website", "u~WkKPQ%zG%v") as ICredentialsByHost;
                //smtpServer.Credentials = new NetworkCredential("badge.5gvr.online@gmail.com", "stc5gexp2021") as ICredentialsByHost;
                smtpServer.EnableSsl = false;

                ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                Debug.Log("DATA DONE");
                mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                smtpServer.Send(mail);
                Debug.Log("MAIL SEND!!");
                Task.Delay(1000);
                avHtml.Dispose();
                //inline.Dispose();
                mail.Dispose();
                avHtml = null;
                //inline = null;
                mail = null;
               // isMailSend = true;


            }
            catch (Exception e)
            {
               // isMailSend = false;
                Debug.Log("ERROR SENDING MAIL::" + e.Message);
            }
        });

    }


    static void MailDeliveryComplete(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
    {
        Debug.Log("Message" + e.UserState);
        if (e.Error != null)
            Debug.Log("Error sending email.");
        else if (e.Cancelled)
            Debug.Log("Sending of email cancelled.");
        else
            Debug.Log("Message sent.");
    }
}

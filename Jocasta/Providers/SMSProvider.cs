using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Mail;
using System.Net;
using System.Web;

namespace Jocasta.Providers
{
    public class SMSProvider
    {
        public static bool SendOTPViaEmail(string email, string code, string title, string message)
        {
            try
            {
                System.Net.Mail.SmtpClient clientDetails = new System.Net.Mail.SmtpClient();
                clientDetails.Port = 587;
                clientDetails.Host = "smtp.gmail.com";
                clientDetails.EnableSsl = true;
                clientDetails.DeliveryMethod = SmtpDeliveryMethod.Network;
                clientDetails.UseDefaultCredentials = false;
                clientDetails.Credentials = new NetworkCredential("hotelmailinh@gmail.com", "usnrpgjwngavepjx");

                //Message Details
                MailMessage mailDetails = new MailMessage();
                mailDetails.From = new MailAddress("hotelmailinh@gmail.com");
                mailDetails.To.Add(email);
                mailDetails.Subject = "[MaiLinhHotel] " + title;
                mailDetails.IsBodyHtml = false;
                mailDetails.Body = message + " " + code;
                clientDetails.Send(mailDetails);
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public class SMSResponse
        {
            public string CodeResult { get; set; }
            public string CountRegenerate { get; set; }
            public string SMSID { get; set; }
        }
    }
}
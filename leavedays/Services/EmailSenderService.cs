﻿using leavedays.Models;
using leavedays.Models.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace leavedays.Services
{
    public class EmailSenderService
    {
        public static void Send(IEnumerable<AppUser> users)
        {
            foreach (var user in users)
            {
                using (MailMessage message = new MailMessage(ConfigurationManager.AppSettings["CompanyEmail"], user.FirstName))
                {
                    message.Subject = "Payment reminders";
                    string messageBody = "Dear " + user.FirstName + " " + user.LastName + Environment.NewLine;
                    messageBody += "Let me remind you, that it's time to pay for our services." + Environment.NewLine;
                    messageBody += "In the case of non-payment of accounts up to 5 numbers of current month, your license will be blocked" + Environment.NewLine;
                    message.Body = messageBody;
                    using (SmtpClient client = new SmtpClient
                    {
                        EnableSsl = true,
                        Host = "smtp.yandex.ru",
                        Port = 25,
                        Credentials = new NetworkCredential(ConfigurationManager.AppSettings["CompanyEmail"], ConfigurationManager.AppSettings["CompanyEmailPassword"])
                    })
                    {
                        client.Send(message);
                    }
                }
            }
        }
    }
}
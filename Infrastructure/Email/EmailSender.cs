﻿using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Email
{
    public class EmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string userEmail, string emailSubject, string msg)
        {
            var client = new SendGridClient(_config["SendGrid:Key"]);
            //var message = new SendGridMessage
            //{
            //    From = new EmailAddress("gavipro123@gmail.com", _config["SendGrid:User"]),
            //    Subject = emailSubject,
            //    PlainTextContent = msg,
            //    HtmlContent = msg
            //};
            //message.AddTo(new EmailAddress(userEmail, userEmail));
            var message = MailHelper.CreateSingleEmail(new EmailAddress("gavipro123@gmail.com", _config["SendGrid:User"]), new EmailAddress(userEmail, userEmail), emailSubject, "a", msg);
            message.SetClickTracking(false, false);
            var response = await client.SendEmailAsync(message);
        }
    }
}

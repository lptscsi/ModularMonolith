﻿using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using FluentScheduler;
using Microsoft.Extensions.Options;
using Shop.Communication.Infrastructure.Interfaces.DataAccess;
using Shop.Communication.Infrastructure.Interfaces.Options;

namespace Shop.Communication.Infrastructure.Implementation.BackgroundJobs
{
    internal class SendEmailsJob : IJob
    {
        private readonly ICommunicationDbContext _dbContext;
        private readonly EmailOptions _emailOptions;

        public SendEmailsJob(ICommunicationDbContext dbContext, IOptions<EmailOptions> emailOptions)
        {
            _dbContext = dbContext;
            _emailOptions = emailOptions.Value;
        }

        public void Execute()
        {
            var client = new SmtpClient(_emailOptions.Server, _emailOptions.Port);
            client.Credentials = new NetworkCredential(_emailOptions.Email, _emailOptions.Password);
            client.EnableSsl = true;

            foreach (var email in _dbContext.Emails.Where(x => !x.IsSended && x.Attempts < 3))
            {
                try
                {
                    var from = new MailAddress(_emailOptions.Email, _emailOptions.FromName);
                    var to = new MailAddress(email.Address);
                    var message = new MailMessage(from, to);
                    message.Subject = email.Subject;
                    message.Body = message.Body;
                    
                    client.Send(message);

                    email.IsSended = true;
                }
                catch (Exception ex)
                {
                    //logging

                    email.Attempts++;
                }
            }

            _dbContext.SaveChanges();
        }
    }
}
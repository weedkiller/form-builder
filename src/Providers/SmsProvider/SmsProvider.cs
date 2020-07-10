using System.Collections.Generic;
using System.Linq;
using form_builder.Configuration;
using Microsoft.Extensions.Options;
using Notify.Interfaces;
using Notify.Models.Responses;

namespace form_builder.Providers.SmsProvider
{
    public interface ISmsProvider
    {
        SmsNotificationResponse SendSms(string phoneNumber, string content, string template);

        EmailNotificationResponse SendEmail(string toEmail, string content, string template);

        EmailNotificationResponse SendEmailWithTemplate(string toEmail, Dictionary<string, dynamic> personalisation, string template);
    }

    public class SmsProvider : ISmsProvider
    {
        private readonly INotificationClient _notificationClient;
        private readonly NotifySmsConfiguration _configuration;

        public SmsProvider(INotificationClient notificationClient, IOptions<NotifySmsConfiguration> configuration)
        {
            _notificationClient = notificationClient;
            _configuration = configuration.Value;
        }

        public SmsNotificationResponse SendSms(string phoneNumber, string content, string template)
        {
            var personalisation = new Dictionary<string, dynamic>
            {
                {"content", content}
            };

            var response = _notificationClient.SendSms(
                mobileNumber: phoneNumber,
                templateId: _configuration.Templates.FirstOrDefault(_ => _.Name.Equals(template)).Id,
                personalisation: personalisation
            );

            return response;
        }

        public EmailNotificationResponse SendEmail(string toEmail, string content, string template)
        {
            var personalisation = new Dictionary<string, dynamic>
            {
                {
                    "content", content
                }
            };
            var response = _notificationClient.SendEmail(
                emailAddress: toEmail,
                templateId: _configuration.Templates.FirstOrDefault(_ => _.Name.Equals(template)).Id,
                personalisation: personalisation
                );

            return response;
        }

        public EmailNotificationResponse SendEmailWithTemplate(string toEmail, Dictionary<string, dynamic> personalisation, string template)
        {
            var response = _notificationClient.SendEmail(
                emailAddress: toEmail,
                templateId: _configuration.Templates.FirstOrDefault(_ => _.Name.Equals(template)).Id,
                personalisation: personalisation
            );

            return response;
        }
    }
}

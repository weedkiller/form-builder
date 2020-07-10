using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using form_builder.Enum;
using form_builder.Helpers.ActionsHelpers;
using form_builder.Helpers.Session;
using form_builder.Models;
using form_builder.Providers.EmailProvider;
using form_builder.Providers.SmsProvider;
using form_builder.Providers.StorageProvider;
using Newtonsoft.Json;

namespace form_builder.Services.ActionService
{
    public interface IActionService
    {
        Task Process(FormSchema baseForm, string caseRef);
    }

    public class ActionService : IActionService
    {
        private readonly ISessionHelper _sessionHelper;
        private readonly IDistributedCacheWrapper _distributedCache;
        private readonly IEmailProvider _emailProvider;
        private readonly IActionHelper _actionHelper;
        private readonly ISmsProvider _smsProvider;

        public ActionService(ISessionHelper sessionHelper, IDistributedCacheWrapper distributedCache, IEmailProvider emailProvider, IActionHelper actionHelper, ISmsProvider smsProvider)
        {
            _sessionHelper = sessionHelper;
            _distributedCache = distributedCache;
            _emailProvider = emailProvider;
            _actionHelper = actionHelper;
            _smsProvider = smsProvider;
        }
        public async Task Process(FormSchema baseForm, string caseRef)
        {
            try
            {
                var sessionGuid = _sessionHelper.GetSessionGuid();

                if (string.IsNullOrEmpty(sessionGuid))
                {
                    throw new Exception("ActionService::Process: Session has expired");
                }

                var formData = _distributedCache.GetString(sessionGuid);
                var formAnswers = JsonConvert.DeserializeObject<FormAnswers>(formData);
                formAnswers.Pages.Add(new PageAnswers
                {
                    Answers = new List<Answers>
                    {
                        new Answers
                        {
                            QuestionId = "caseRef",
                            Response = caseRef
                        }
                    }
                });
                foreach (var action in baseForm.FormActions)
                {
                    switch (action.Type)
                    {
                        case EFormActionType.UserEmail:
                            var awsEmailcontent = _actionHelper.InsertFormAnswersIntoContent(action, formAnswers);
                            var message = new EmailMessage(
                                action.Properties.Subject,
                                awsEmailcontent,
                                action.Properties.From,
                                _actionHelper.InsertFormAnswersIntoProperty(action, formAnswers));

                            await _emailProvider.SendAwsSesEmail(message);
                            break;

                        case EFormActionType.BackOfficeEmail:
                            var emailRecipients = _actionHelper.InsertFormAnswersIntoProperty(action, formAnswers).Split(",");
                            var emailContent = _actionHelper.InsertFormAnswersIntoParameters(action, formAnswers).Split(",");
                            foreach (var recipient in emailRecipients)
                            {
                                if (!string.IsNullOrEmpty(recipient))
                                    _smsProvider.SendEmail(recipient, emailContent, action.Properties.Template);
                            }
                            //SendUserEmail(action, formAnswers);
                            break;

                        case EFormActionType.UserSms:
                            var recipients = _actionHelper.InsertFormAnswersIntoProperty(action, formAnswers).Split(",");
                            var content = _actionHelper.InsertFormAnswersIntoContent(action, formAnswers);
                            foreach (var recipient in recipients)
                            {
                                if (!string.IsNullOrEmpty(recipient))
                                    _smsProvider.SendSms(recipient, content, action.Properties.Template);
                            }
                            
                            break;

                        default: break;
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void SendUserEmail(FormAction action, FormAnswers formAnswers)
        {
            var client = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("", ""),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(action.Properties.From),
                Subject = action.Properties.Subject,
                Body = action.Properties.Content,
                IsBodyHtml = true
            };

            var toEmails = _actionHelper.InsertFormAnswersIntoProperty(action, formAnswers).Split(",");

            foreach (var email in toEmails)
            {
                if (!string.IsNullOrEmpty(email))
                    mailMessage.To.Add(email);
            }

            try
            {
                client.Send(mailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
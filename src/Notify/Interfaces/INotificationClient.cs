using Notify.Models;
using Notify.Models.Responses;
using System.Collections.Generic;

namespace Notify.Interfaces
{
    public interface INotificationClient : IBaseClient
    {
        TemplatePreviewResponse GenerateTemplatePreview(string templateId, Dictionary<string, dynamic> personalisation = null);

        TemplateList GetAllTemplates(string templateType = "");

        Notification GetNotificationById(string notificationId);

        NotificationList GetNotifications(string templateType = "", string status = "", string reference = "", string olderThanId = "");

        ReceivedTextListResponse GetReceivedTexts(string olderThanId = "");

        TemplateResponse GetTemplateById(string templateId);

        TemplateResponse GetTemplateByIdAndVersion(string templateId, int version = 0);

        //SmsNotificationResponse SendSms(string phoneNumber, string templateId, Dictionary<string, dynamic> personalisation = null, string clientReference = null, string smsSenderId = null, string statusCallbackUrl = null, string statusCallbackBearerToken = null);
        SmsNotificationResponse SendSms(string phoneNumber, string templateId, Dictionary<string, dynamic> personalisation = null, string clientReference = null, string smsSenderId = null);
        EmailNotificationResponse SendEmail(string emailAddress, string templateId, Dictionary<string, dynamic> personalisation = null, string clientReference = null, string emailReplyToId = null);

        //EmailNotificationResponse SendEmail(string emailAddress, string templateId, Dictionary<string, dynamic> personalisation = null, string clientReference = null, string emailReplyToId = null, string statusCallbackUrl = null, string statusCallbackBearerToken = null);

        LetterNotificationResponse SendLetter(string templateId, Dictionary<string, dynamic> personalisation, string clientReference = null);

        LetterNotificationResponse SendPrecompiledLetter(string clientReference, byte[] pdfContents, string postage = null);
    }
}

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Notify.Interfaces;
using Notify.Models;
using Notify.Models.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Notify.Client
{
    public class NotifyClient : NotificationClient
    {
        public NotifyClient(string apiKey) : base(apiKey)
        {
        }

        public NotifyClient(string baseUrl, string apiKey) : base(baseUrl, apiKey)
        {
        }

        public NotifyClient(IHttpClient client, string apiKey) : base(client, apiKey)
        {
        }

        public async Task<SmsNotificationResponse> SendSmsAsync(string phoneNumber, string templateId,
            Dictionary<string, dynamic> personalisation = null, string clientReference = null,
            string smsSenderId = null, string statusCallbackUrl = null, string statusCallbackBearerToken = null)
        {
            var o = CreateRequestParams(templateId, personalisation, clientReference);
            o.AddFirst(new JProperty("phone_number", phoneNumber));

            if (smsSenderId != null)
            {
                o.Add(new JProperty("sms_sender_id", smsSenderId));
            }
            if (statusCallbackUrl != null)
            {
                o.Add(new JProperty("status_callback_url", statusCallbackUrl));
            }
            if (statusCallbackBearerToken != null)
            {
                o.Add(new JProperty("status_callback_bearer_token", statusCallbackBearerToken));
            }

            var response = await POST(SEND_SMS_NOTIFICATION_URL, o.ToString(Formatting.None)).ConfigureAwait(false);

            return JsonConvert.DeserializeObject<SmsNotificationResponse>(response);
        }

        public async Task<EmailNotificationResponse> SendEmailAsync(string emailAddress, string templateId,
            Dictionary<string, dynamic> personalisation = null, string clientReference = null,
            string emailReplyToId = null, string statusCallbackUrl = null, string statusCallbackBearerToken = null)
        {
            var o = CreateRequestParams(templateId, personalisation, clientReference);
            o.AddFirst(new JProperty("email_address", emailAddress));

            if (emailReplyToId != null)
            {
                o.Add(new JProperty("email_reply_to_id", emailReplyToId));
            }
            if (statusCallbackUrl != null)
            {
                o.Add(new JProperty("status_callback_url", statusCallbackUrl));
            }
            if (statusCallbackBearerToken != null)
            {
                o.Add(new JProperty("status_callback_bearer_token", statusCallbackBearerToken));
            }

            var response = await POST(SEND_EMAIL_NOTIFICATION_URL, o.ToString(Formatting.None)).ConfigureAwait(false);

            return JsonConvert.DeserializeObject<EmailNotificationResponse>(response);
        }

        public SmsNotificationResponse SendSms(string phoneNumber, string templateId, Dictionary<string, dynamic> personalisation = null, string clientReference = null, string smsSenderId = null, string statusCallbackUrl = null, string statusCallbackBearerToken = null)
        {
            try
            {
                return SendSmsAsync(phoneNumber, templateId, personalisation, clientReference, smsSenderId, statusCallbackUrl, statusCallbackBearerToken).Result;
            }
            catch (AggregateException ex)
            {
                throw HandleAggregateException(ex);
            }
        }

        public EmailNotificationResponse SendEmail(string emailAddress, string templateId, Dictionary<string, dynamic> personalisation = null, string clientReference = null, string emailReplyToId = null, string statusCallbackUrl = null, string statusCallbackBearerToken = null)
        {
            try
            {
                return SendEmailAsync(emailAddress, templateId, personalisation, clientReference, emailReplyToId, statusCallbackUrl, statusCallbackBearerToken).Result;
            }
            catch (AggregateException ex)
            {
                throw HandleAggregateException(ex);
            }
        }

        private static JObject CreateRequestParams(string templateId, Dictionary<string, dynamic> personalisation = null,
            string clientReference = null)
        {
            var personalisationJson = new JObject();

            if (personalisation != null)
            {
                personalisationJson = JObject.FromObject(personalisation);
            }

            var o = new JObject
            {
                {"template_id", templateId},
                {"personalisation", personalisationJson}
            };

            if (clientReference != null)
            {
                o.Add("reference", clientReference);
            }

            return o;
        }

        private static Exception HandleAggregateException(AggregateException ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException("ex");
            }

            if (ex.InnerExceptions != null && ex.InnerExceptions.Count == 1)
            {
                return ex.InnerException;
            }
            else
            {
                return ex;
            }
        }



    }
}
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Notify.Client;
using Notify.Exceptions;
using Notify.Interfaces;
using Notify.Models;
using Notify.Models.Responses;
using NUnit.Framework;

namespace Notify.Tests.UnitTests
{
    [TestFixture]
    public class NotifyClientUnitTests
    {

        private Mock<HttpMessageHandler> handler;
        private NotifyClient client;

        [SetUp]
        public void SetUp()
        {
            handler = new Mock<HttpMessageHandler>();

            var w = new HttpClientWrapper(new HttpClient(handler.Object));
            client = new NotifyClient(w, Constants.fakeApiKey);
        }

        [TearDown]
        public void TearDown()
        {
            handler = null;
            client = null;
        }

        [Test, Category("Unit/NotifyClient")]
        public void NotifyClientSatisfiesInterfaces()
        {
            var client = new NotifyClient(Constants.fakeApiKey);
            Assert.IsTrue(client is INotificationClient);
            Assert.IsTrue(client is IAsyncNotificationClient);
        }

        [Test, Category("Unit/NotifyClient")]
        public void NotifyClientWrapsNotificationClient()
        {
            Assert.IsTrue(typeof(NotifyClient).IsSubclassOf(typeof(NotificationClient)));
            Assert.IsTrue(typeof(NotificationClient).IsAssignableFrom(typeof(NotifyClient)));
        }

        [Test, Category("Unit/NotifyClient")]
        public void SendEmailNotifyGeneratesExpectedRequest()
        {
            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
                {
                    { "name", "someone" }
                };
            JObject expected = new JObject
            {
                { "email_address", Constants.fakeEmail },
                { "template_id", Constants.fakeTemplateId },
                { "personalisation", JObject.FromObject(personalisation) },
                { "reference", Constants.fakeNotificationReference }
            };

            MockRequest(Constants.fakeTemplatePreviewResponseJson,
                client.SEND_EMAIL_NOTIFICATION_URL,
                AssertValidRequest,
                HttpMethod.Post,
                AssertGetExpectedContent, expected.ToString(Formatting.None));

            EmailNotificationResponse response = client.SendEmail(Constants.fakeEmail, Constants.fakeTemplateId, personalisation, Constants.fakeNotificationReference);
        }

        [Test, Category("Unit/NotifyClient")]
        public void SendEmailNotifyGeneratesExpectedResponse()
        {
            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
                {
                    { "name", "someone" }
                };
            EmailNotificationResponse expectedResponse = JsonConvert.DeserializeObject<EmailNotificationResponse>(Constants.fakeEmailNotificationResponseJson);

            MockRequest(Constants.fakeEmailNotificationResponseJson);

            EmailNotificationResponse actualResponse = client.SendEmail(Constants.fakeEmail, Constants.fakeTemplateId, personalisation, Constants.fakeNotificationReference);

            Assert.IsTrue(expectedResponse.Equals(actualResponse));

        }

        [Test, Category("Unit/NotifyClient")]
        public void SendEmailNotifyStatusCallbackUrlGeneratesExpectedRequest()
        {
            var personalisation = new Dictionary<string, dynamic>
            {
                { "name", "someone" }
            };

            var expected = new JObject
            {
                { "email_address", Constants.fakeEmail },
                { "template_id", Constants.fakeTemplateId },
                { "personalisation", JObject.FromObject(personalisation) },
                { "reference", Constants.fakeNotificationReference },
                { "status_callback_url", Constants.fakeStatusCallbackUrl},
                { "status_callback_bearer_token", Constants.fakeStatusCallbackBearerToken}
            };

            MockRequest(Constants.fakeTemplateEmailListResponseJson,
                client.SEND_EMAIL_NOTIFICATION_URL,
                AssertValidRequest,
                HttpMethod.Post,
                AssertGetExpectedContent,
                expected.ToString(Formatting.None));

            var response = client.SendEmail(Constants.fakeEmail, Constants.fakeTemplateId, personalisation: personalisation, clientReference: Constants.fakeNotificationReference, statusCallbackUrl: Constants.fakeStatusCallbackUrl, statusCallbackBearerToken: Constants.fakeStatusCallbackBearerToken);
        }

        [Test, Category("Unit/NotifyClient")]
        public void SendEmailNotifyWithStatusCallbackUrlGeneratesExpectedResponse()
        {
            var personalisation = new Dictionary<string, dynamic>
            {
                { "name", "someone" }
            };

            var expectedResponse = JsonConvert.DeserializeObject<EmailNotificationResponse>(Constants.fakeEmailNotificationResponseJson);

            MockRequest(Constants.fakeEmailNotificationResponseJson);

            var actualResponse = client.SendEmail(Constants.fakeEmail, Constants.fakeTemplateId, personalisation, Constants.fakeNotificationReference, Constants.fakeStatusCallbackUrl, Constants.fakeStatusCallbackBearerToken);

            Assert.IsTrue(expectedResponse.Equals(actualResponse));
        }

        [Test, Category("Unit/NotifyClient")]
        public void SendSmsNotifyWithStatusCallbackUrlGeneratesExpectedRequest()
        {
            var personalisation = new Dictionary<string, dynamic>
                {
                    { "name", "someone" }
                };
            var expected = new JObject
            {
                { "phone_number", Constants.fakePhoneNumber },
                { "template_id", Constants.fakeTemplateId },
                { "personalisation", JObject.FromObject(personalisation) },
                { "status_callback_url", Constants.fakeStatusCallbackUrl },
                { "status_callback_bearer_token", Constants.fakeStatusCallbackBearerToken}
            };

            MockRequest(Constants.fakeSmsNotificationWithSMSSenderIdResponseJson,
                client.SEND_SMS_NOTIFICATION_URL,
                AssertValidRequest,
                HttpMethod.Post,
                AssertGetExpectedContent, expected.ToString(Formatting.None));

            var response = client.SendSms(
                Constants.fakePhoneNumber, Constants.fakeTemplateId, personalisation: personalisation, statusCallbackUrl: Constants.fakeStatusCallbackUrl, statusCallbackBearerToken: Constants.fakeStatusCallbackBearerToken);
        }

        private static void AssertGetExpectedContent(string expected, string content)
        {
            Assert.IsNotNull(content);
            Assert.AreEqual(expected, content);
        }

        private void AssertValidRequest(string uri, HttpRequestMessage r, HttpMethod method = null)
        {
            if (method == null)
            {
                method = HttpMethod.Get;
            }

            Assert.AreEqual(r.Method, method);
            Assert.AreEqual(r.RequestUri.ToString(), client.BaseUrl + uri);
            Assert.IsNotNull(r.Headers.Authorization);
            Assert.IsNotNull(r.Headers.UserAgent);
            Assert.AreEqual(r.Headers.UserAgent.ToString(), client.GetUserAgent());
            Assert.AreEqual(r.Headers.Accept.ToString(), "application/json");
        }

        private void MockRequest(string content, string uri,
                  Action<string, HttpRequestMessage, HttpMethod> _assertValidRequest = null,
                  HttpMethod method = null,
                  Action<string, string> _assertGetExpectedContent = null,
                  string expected = null,
                  HttpStatusCode status = HttpStatusCode.OK)
        {
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage
                {
                    StatusCode = status,
                    Content = new StringContent(content)
                }))
                .Callback<HttpRequestMessage, CancellationToken>((r, c) =>
                {
                    _assertValidRequest(uri, r, method);

                    if (r.Content == null || _assertGetExpectedContent == null) return;

                    var response = r.Content.ReadAsStringAsync().Result;
                    _assertGetExpectedContent(expected, response);
                });
        }

        private void MockRequest(string content)
        {

            handler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(content)
                }));
        }

    }
}

using Notify.Interfaces;

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

    }
}
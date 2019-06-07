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

    }
}

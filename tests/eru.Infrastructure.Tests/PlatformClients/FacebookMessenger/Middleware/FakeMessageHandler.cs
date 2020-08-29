using System.Collections.Generic;
using System.Threading.Tasks;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook.Messages;

namespace eru.Infrastructure.Tests.PlatformClients.FacebookMessenger.Middleware
{
    public class FakeMessageHandler : IMessageHandler
    {
        public List<Messaging> MessageDump { get; set; }

        public FakeMessageHandler()
        {
            MessageDump = new List<Messaging>();
        }
        public Task Handle(Messaging message)
        {
            MessageDump.Add(message);
            return Task.CompletedTask;
        }
    }
}
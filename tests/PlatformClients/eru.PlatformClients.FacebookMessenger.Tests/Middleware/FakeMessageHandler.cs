using System.Collections.Generic;
using System.Threading.Tasks;
using eru.PlatformClients.FacebookMessenger.MessageHandlers;
using eru.PlatformClients.FacebookMessenger.Models.Webhook.Messages;

namespace eru.PlatformClients.FacebookMessenger.Tests.Middleware
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
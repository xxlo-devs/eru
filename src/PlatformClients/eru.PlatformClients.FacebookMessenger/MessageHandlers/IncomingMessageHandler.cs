using System;
using System.Threading.Tasks;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.UnknownUser;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers
{
    public class IncomingMessageHandler : MessageHandler<IncomingMessageHandler>
    {
        private readonly IServiceProvider _provider;

        public IncomingMessageHandler(IServiceProvider provider, ILogger<IncomingMessageHandler> logger) : base(logger)
        {
            _provider = provider;
        }

        protected override async Task Base(Messaging message)
        {
            if (await _provider.GetService<IMediator>().Send(new GetSubscriberQuery(message.Sender.Id, FacebookMessengerPlatformClient.PId)) != null)
            {
                await _provider.GetService<MessageHandler<KnownUserMessageHandler>>().Handle(message);
                return;
            }

            if (await _provider.GetService<IRegistrationDbContext>().IncompleteUsers.FindAsync(message.Sender.Id) != null)
            {
                await _provider.GetService<MessageHandler<RegisteringUserMessageHandler>>().Handle(message);
                return;
            }

            await _provider.GetService<MessageHandler<StartRegistrationMessageHandler>>().Handle(message);        
        }
    }
}
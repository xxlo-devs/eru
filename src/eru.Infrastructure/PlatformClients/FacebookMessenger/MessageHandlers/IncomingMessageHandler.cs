using System;
using System.Threading.Tasks;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.UnknownUser;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook.Messages;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using MediatR;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers
{
    public class IncomingMessageHandler : IMessageHandler
    {
        private readonly IKnownUserMessageHandler _knownUserHandler;
        private readonly IRegisteringUserMessageHandler _registeringUserHandler;
        private readonly IUnkownUserMessageHandler _unknownUserHandler;
        private readonly IMediator _mediator;
        private readonly IRegistrationDbContext _localDbContext;
        
        public IncomingMessageHandler(IKnownUserMessageHandler knownUserHandler, IRegisteringUserMessageHandler registeringMessageHandler, IUnkownUserMessageHandler unknownUserHandler, IMediator mediator, IRegistrationDbContext localDbContext)
        {
            _knownUserHandler = knownUserHandler;
            _registeringUserHandler = registeringMessageHandler;
            _unknownUserHandler = unknownUserHandler;
            _mediator = mediator;
            _localDbContext = localDbContext;
        }
        public async Task Handle(Messaging message)
        {
            if (await _mediator.Send(new GetSubscriberQuery(message.Sender.Id, "FacebookMessenger")) != null)
            {
                await _knownUserHandler.Handle(message.Sender.Id, message.Message);
                return;
            }

            if (await _localDbContext.IncompleteUsers.FindAsync(message.Sender.Id) != null)
            {
                await _registeringUserHandler.Handle(message.Sender.Id, message.Message);
                return;
            }

            await _unknownUserHandler.Handle(message.Sender.Id);
        }
    }
}
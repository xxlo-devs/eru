using System;
using System.Threading.Tasks;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.KnownUserMessageHandler;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUserMessageHandler;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.UnknownUserMessageHandler;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook.Messages;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDbContext;
using MediatR;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers
{
    public class IncomingMessageHandler : IMessageHandler
    {
        private readonly IKnownUserMessageHandler _knownUserHandler;
        private readonly IRegisteringUserMessageHandler _registeringUserHandler;
        private readonly IUnknownUserMessageHandler _unknownUserHandler;
        private readonly IMediator _mediator;
        private readonly IRegistrationDbContext _localDbContext;
        
        public IncomingMessageHandler(IKnownUserMessageHandler knownUserHandler, IRegisteringUserMessageHandler registeringUserHandler, IUnknownUserMessageHandler unknownUserHandler, IMediator mediator, IRegistrationDbContext localDbContext)
        {
            _knownUserHandler = knownUserHandler;
            _registeringUserHandler = registeringUserHandler;
            _unknownUserHandler = unknownUserHandler;
            _mediator = mediator;
            _localDbContext = localDbContext;
        }
        public async Task Handle(Messaging message)
        {
            if (await _mediator.Send(new GetUserQuery {UserId = message.Sender.Id, Platform = "FacebookMessenger"}) != null)
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
            
            throw new NotImplementedException();
        }
    }
}
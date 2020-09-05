using System.Threading.Tasks;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.UnknownUser;
using eru.PlatformClients.FacebookMessenger.Models.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers
{
    public class IncomingMessageHandler : IMessageHandler
    {
        private readonly IKnownUserMessageHandler _knownUserHandler;
        private readonly IRegisteringUserMessageHandler _registeringUserHandler;
        private readonly IUnkownUserMessageHandler _unknownUserHandler;
        private readonly IMediator _mediator;
        private readonly IRegistrationDbContext _localDbContext;
        private readonly ILogger _logger; 
        
        public IncomingMessageHandler(IKnownUserMessageHandler knownUserHandler, IRegisteringUserMessageHandler registeringMessageHandler, IUnkownUserMessageHandler unknownUserHandler, IMediator mediator, IRegistrationDbContext localDbContext, ILogger logger)
        {
            _knownUserHandler = knownUserHandler;
            _registeringUserHandler = registeringMessageHandler;
            _unknownUserHandler = unknownUserHandler;
            _mediator = mediator;
            _localDbContext = localDbContext;
            _logger = logger;
        }
        public async Task Handle(Messaging message)
        {
            _logger.LogTrace($"eru.PlatformClients.FacebookMessenger: IncomingMessageHandler.Handle got a message (uid: {message.Sender.Id})");
            
            if (await _mediator.Send(new GetSubscriberQuery(message.Sender.Id, "FacebookMessenger")) != null)
            {
                _logger.LogTrace($"eru.PlatformClients.FacebookMessenger: IncomingMessageHandler.Handle redirected message to KnownUserHandler");
                await _knownUserHandler.Handle(message.Sender.Id, message.Message);
                return;
            }

            if (await _localDbContext.IncompleteUsers.FindAsync(message.Sender.Id) != null)
            {
                _logger.LogTrace($"eru.PlatformClients.FacebookMessenger: IncomingMessageHandler.Handle redirected message to KnownUserHandler");
                await _registeringUserHandler.Handle(message.Sender.Id, message.Message);
                return;
            }

            _logger.LogTrace($"eru.PlatformClients.FacebookMessenger: IncomingMessageHandler.Handle redirected message to UnknownUserHandler");
            await _unknownUserHandler.Handle(message.Sender.Id);
        }
    }
}
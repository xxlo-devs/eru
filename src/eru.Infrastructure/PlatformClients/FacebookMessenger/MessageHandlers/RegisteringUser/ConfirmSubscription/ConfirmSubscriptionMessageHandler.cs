using System;
using System.Threading;
using System.Threading.Tasks;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using MediatR;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.ConfirmSubscription
{
    public class ConfirmSubscriptionMessageHandler : IConfirmSubscriptionMessageHandler
    {
        private readonly IMediator _mediator;
        private readonly IRegistrationDbContext _dbContext;
        private readonly ISendApiClient _apiClient;

        public ConfirmSubscriptionMessageHandler(IMediator mediator, IRegistrationDbContext dbContext, ISendApiClient apiClient)
        {
            _mediator = mediator;
            _apiClient = apiClient;
            _dbContext = dbContext;
        }

        public async Task Handle(string uid, string payload)
        {
            if (payload == ReplyPayloads.SubscribePayload)
            {
                await Confirm(uid);
            }
            else
            {
                await UnsupportedCommand(uid);
            }
        }

        private async Task Confirm(string uid)
        {
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            await _mediator.Send(user.ToCreateSubscriptionCommand());
            
            _dbContext.IncompleteUsers.Remove(user);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            
            var response = new SendRequest(uid, new Message("Congratulations! You've successfully subscribed to eru Messenger notifications :)", new []
            {
                new QuickReply("Cancel", ReplyPayloads.CancelPayload), 
            }));
            await _apiClient.Send(response);
        }

        private async Task UnsupportedCommand(string uid)
        {
            throw new NotImplementedException();
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
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
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;

        public ConfirmSubscriptionMessageHandler(IMediator mediator, IRegistrationDbContext dbContext, ISendApiClient apiClient, ITranslator<FacebookMessengerPlatformClient> translator)
        {
            _mediator = mediator;
            _apiClient = apiClient;
            _dbContext = dbContext;
            _translator = translator;
        }

        public async Task Handle(string uid, Payload payload)
        {
            if (payload.Type == Type.Subscribe)
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
            
            var response = new SendRequest(uid, new Message(await _translator.TranslateString("confirmation", user.PreferredLanguage), new []
            {
                new QuickReply(await _translator.TranslateString("cancel-button", user.PreferredLanguage), new Payload(Type.Cancel).ToJson()), 
            }));
            await _apiClient.Send(response);
        }

        private async Task UnsupportedCommand(string uid)
        {
            throw new NotImplementedException();
        }
    }
}
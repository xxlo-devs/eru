using System;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.Infrastructure.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Selector;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.ConfirmSubscription
{
    public class ConfirmSubscriptionMessageHandler : IConfirmSubscriptionMessageHandler
    {
        private readonly IMediator _mediator;
        private readonly IRegistrationDbContext _dbContext;
        private readonly ISendApiClient _apiClient;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;
        private readonly ISelector _selector;

        public ConfirmSubscriptionMessageHandler(IMediator mediator, IRegistrationDbContext dbContext, ISendApiClient apiClient, ITranslator<FacebookMessengerPlatformClient> translator, ISelector selector)
        {
            _mediator = mediator;
            _apiClient = apiClient;
            _dbContext = dbContext;
            _translator = translator;
            _selector = selector;
        }

        public async Task Handle(string uid, Payload payload)
        {
            if (payload.Type == PayloadType.Subscribe)
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
            
            var response = new SendRequest(uid, new Message(await _translator.TranslateString("confirmation", user.PreferredLanguage), await _selector.GetCancelSelector(user.PreferredLanguage)));
            await _apiClient.Send(response);
        }

        private async Task UnsupportedCommand(string uid)
        {
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            
            var response = new SendRequest(uid, new Message(await _translator.TranslateString("unsupported-command", user.PreferredLanguage), await _selector.GetConfirmationSelector(user.PreferredLanguage)));
            await _apiClient.Send(response);
        }
    }
}
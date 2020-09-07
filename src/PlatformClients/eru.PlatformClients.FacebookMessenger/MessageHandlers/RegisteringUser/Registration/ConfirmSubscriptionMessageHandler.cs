using System;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.ConfirmSubscription
{
    public class ConfirmSubscriptionMessageHandler : RegistrationMessageHandler<ConfirmSubscriptionMessageHandler>
    {
        private readonly IRegistrationDbContext _dbContext;
        private readonly ISendApiClient _apiClient;
        private readonly IMediator _mediator;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;
        
        public ConfirmSubscriptionMessageHandler(IServiceProvider provider, ILogger<ConfirmSubscriptionMessageHandler> logger, ITranslator<FacebookMessengerPlatformClient> translator) : base(translator)
        {
            _dbContext = provider.GetService<IRegistrationDbContext>();
            _apiClient = provider.GetService<ISendApiClient>();
            _mediator = provider.GetService<IMediator>();
            _translator = provider.GetService<ITranslator<FacebookMessengerPlatformClient>>();
        }
        protected override async Task GatherBase(IncompleteUser user, string data)
        {
            await _mediator.Send(user.ToCreateSubscriptionCommand());
                 
            _dbContext.IncompleteUsers.Remove(user);
            await _dbContext.SaveChangesAsync(CancellationToken.None);

            var response = new SendRequest(user.Id, new Message(
                await _translator.TranslateString("congratulations", user.PreferredLanguage), new[]
                {
                    new QuickReply(await _translator.TranslateString("cancel-button", user.PreferredLanguage),
                        new Payload(PayloadType.Cancel).ToJson())
                }));
            await _apiClient.Send(response);
        }

        protected override async Task ShowInstructionBase(IncompleteUser user, int page)
        {
            var response = new SendRequest(user.Id, new Message(await _translator.TranslateString("confirmation", user.PreferredLanguage), new[]
            {
                new QuickReply(await _translator.TranslateString("subscribe-button", user.PreferredLanguage), new Payload(PayloadType.Subscribe).ToJson()), 
                new QuickReply(await _translator.TranslateString("cancel-button", user.PreferredLanguage), new Payload(PayloadType.Cancel).ToJson())
            })); 
            await _apiClient.Send(response);
        }

        protected override async Task ShowUnsupportedCommandBase(IncompleteUser user)
        {
            var response = new SendRequest(user.Id, new Message(await _translator.TranslateString("unsupported-command", user.PreferredLanguage), new[]
            {
                new QuickReply(await _translator.TranslateString("subscribe-button", user.PreferredLanguage), new Payload(PayloadType.Subscribe).ToJson()), 
                new QuickReply(await _translator.TranslateString("cancel-button", user.PreferredLanguage), new Payload(PayloadType.Cancel).ToJson())
            })); 
            await _apiClient.Send(response);
        }
    }
}
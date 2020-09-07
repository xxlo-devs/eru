using System;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationEnd;
using eru.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.PlatformClients.FacebookMessenger.Models.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Message = eru.PlatformClients.FacebookMessenger.Models.SendApi.Message;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.ConfirmSubscription
{
    public class ConfirmSubscriptionMessageHandler : RegistrationEndMessageHandler<ConfirmSubscriptionMessageHandler>
    {
        private readonly IRegistrationDbContext _dbContext;
        private readonly ISendApiClient _apiClient;
        private readonly IMediator _mediator;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;
        
        public ConfirmSubscriptionMessageHandler(IServiceProvider provider, ILogger<ConfirmSubscriptionMessageHandler> logger, ITranslator<FacebookMessengerPlatformClient> translator)
        {
            _dbContext = provider.GetService<IRegistrationDbContext>();
            _apiClient = provider.GetService<ISendApiClient>();
            _mediator = provider.GetService<IMediator>();
            _translator = provider.GetService<ITranslator<FacebookMessengerPlatformClient>>();
        }
        protected override async Task EndRegistration(Messaging message)
        {
            var uid = message.Sender.Id;
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            
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

        public override async Task ShowInstruction(IncompleteUser user)
        {
            var response = new SendRequest(user.Id, new Message(await _translator.TranslateString("confirmation", user.PreferredLanguage), new[]
            {
                new QuickReply(await _translator.TranslateString("subscribe-button", user.PreferredLanguage), new Payload(PayloadType.Subscribe).ToJson()), 
                new QuickReply(await _translator.TranslateString("cancel-button", user.PreferredLanguage), new Payload(PayloadType.Cancel).ToJson())
            })); 
            await _apiClient.Send(response);
        }
    }
}
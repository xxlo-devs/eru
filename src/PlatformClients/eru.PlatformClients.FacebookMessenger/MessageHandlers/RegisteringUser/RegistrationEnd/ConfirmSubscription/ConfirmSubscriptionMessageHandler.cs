﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests;
using MediatR;
using Microsoft.Extensions.Logging;
using Message = eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests.Message;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationEnd.ConfirmSubscription
{
    public class ConfirmSubscriptionMessageHandler : RegistrationEndMessageHandler<ConfirmSubscriptionMessageHandler>, IConfirmSubscriptionMessageHandler
    {
        private readonly IRegistrationDbContext _dbContext;
        private readonly IMediator _mediator;
        private readonly ISendApiClient _apiClient;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;
        
        public ConfirmSubscriptionMessageHandler(IRegistrationDbContext dbContext, IMediator mediator,
            ISendApiClient apiClient, ITranslator<FacebookMessengerPlatformClient> translator,
            ILogger<ConfirmSubscriptionMessageHandler> logger) : base(logger)
        {
            _dbContext = dbContext;
            _mediator = mediator;
            _apiClient = apiClient;
            _translator = translator;
        }
        protected override async Task EndRegistration(IncompleteUser user)
        {
            await _mediator.Send(user.ToCreateSubscriptionCommand());
                 
            _dbContext.IncompleteUsers.Remove(user);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            
            await _apiClient.Send(new SendRequest(user.Id, new Message(
                await _translator.TranslateString("congratulations", user.PreferredLanguage), new[]
            {
                new QuickReply(await _translator.TranslateString("cancel-button", user.PreferredLanguage),
                    new Payload(PayloadType.Cancel).ToJson())
            })));
        }

        public async Task ShowInstruction(IncompleteUser user)
        {
            await _apiClient.Send(new SendRequest(user.Id,
                new Message(await _translator.TranslateString("confirmation", user.PreferredLanguage),
                    await GetConfirmationButtons(user.PreferredLanguage))));
        }

        protected override async Task UnsupportedCommand(IncompleteUser user)
        {
            await _apiClient.Send(new SendRequest(user.Id,
                new Message(await _translator.TranslateString("unsupported-command", user.PreferredLanguage),
                    await GetConfirmationButtons(user.PreferredLanguage))));
        }

        private async Task<IEnumerable<QuickReply>> GetConfirmationButtons(string lang)
            => new[]
            {
                new QuickReply(await _translator.TranslateString("subscribe-button", lang),
                    new Payload(PayloadType.Subscribe).ToJson()),
                new QuickReply(await _translator.TranslateString("cancel-button", lang),
                    new Payload(PayloadType.Cancel).ToJson())
            };
    }
}